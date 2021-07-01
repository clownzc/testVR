// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

#ifndef UNITY_STANDARD_INPUT_INCLUDED
#define UNITY_STANDARD_INPUT_INCLUDED

#include "UnityCG.cginc"
#include "UnityShaderVariables.cginc"
#include "UnityStandardConfig.cginc"
#include "UnityPBSLighting.cginc" // TBD: remove
#include "UnityStandardUtils.cginc"

//---------------------------------------
// Directional lightmaps & Parallax require tangent space too
#if (_NORMALMAP || !DIRLIGHTMAP_OFF || _PARALLAXMAP)
    #define _TANGENT_TO_WORLD 1
#endif

#if (_DETAIL_MULX2 || _DETAIL_MUL || _DETAIL_ADD || _DETAIL_LERP)
    #define _DETAIL 1
#endif

//---------------------------------------
half4       _Color;
half        _Cutoff;

sampler2D   _MainTex;
float4      _MainTex_ST;

sampler2D   _DetailAlbedoMap;
float4      _DetailAlbedoMap_ST;

sampler2D   _BumpMap;
half        _BumpScale;

sampler2D   _DetailMask;
sampler2D   _DetailNormalMap;
half        _DetailNormalMapScale;

sampler2D   _SpecGlossMap;
//Mix Texture Metallic (R) and Occlusion (G) and Smoothness (B)
sampler2D   _MetallicGlossMap;
half        _Metallic;
half        _Glossiness;

//Use Mix Texture
//sampler2D _OcclusionMap;
half        _OcclusionStrength;

//sampler2D _ParallaxMap;
//half      _Parallax;
half        _UVSec;

half4       _EmissionColor;
sampler2D   _EmissionMap;
half		_EmissionMul;
half		_EmissionMask;
half		_EmissionMaskMin;
half		_EmissionMaskMax;
half		_EmissionMaskMul;

half		_EmissionAddIndex;
half		_EmissionAddMul;

half4 		_FresnelColor;
sampler2D   _FresnelMap;
half		_FresnelAdd;
half		_FresnelMul;
half		_FresnelExp;

half		_AlbedoStrength;
half		_MetallicStrength;
half		_GlossStrength;
half		_GlossMin;
half		_GlossMax;

half		_FresnelAlphaAdd;
half		_FresnelAlphaMul;
half		_FresnelAlphaExp;
half		_Transparency;

//Light
half4 _VectorLightWorldPosition[16];
half4 _VectorLightAtten[16];//R:Range G:Intensity
half4 _VectorLightColor[16];


#ifdef _DRAGON
sampler2D _TransparencyMaskTex;uniform float4 _TransparencyMaskTex_ST;
half _TransparencyMask;
half _Scale;
#endif

//-------------------------------------------------------------------------------------
// Input functions

struct VertexInput
{
    float4 vertex   : POSITION;
    half3 normal    : NORMAL;
    float2 uv0      : TEXCOORD0;
    float2 uv1      : TEXCOORD1;
#if defined(DYNAMICLIGHTMAP_ON) || defined(UNITY_PASS_META)
    float2 uv2      : TEXCOORD2;
#endif
    half4 tangent   : TANGENT;
};

float4 TexCoords(VertexInput v)
{
    float4 texcoord;
    texcoord.xy = TRANSFORM_TEX(v.uv0, _MainTex); // Always source from uv0
    texcoord.zw = TRANSFORM_TEX(((_UVSec == 0) ? v.uv0 : v.uv1), _DetailAlbedoMap);
    return texcoord;
}

half DetailMask(float2 uv)
{
    return tex2D (_DetailMask, uv).a;
}

half3 Albedo(float4 texcoords)
{
    half3 albedo = _Color.rgb * tex2D (_MainTex, texcoords.xy).rgb;
//  Gamma Correction
//	half3 albedo = pow(_Color.rgb * tex2D(_MainTex, texcoords.xy).rgb,2.2);
#if _DETAIL
    #if (SHADER_TARGET < 30)
        // SM20: instruction count limitation
        // SM20: no detail mask
        half mask = 1;
    #else
        half mask = DetailMask(texcoords.xy);
    #endif
    half3 detailAlbedo = tex2D (_DetailAlbedoMap, texcoords.zw).rgb;
    #if _DETAIL_MULX2
        albedo *= LerpWhiteTo (detailAlbedo * unity_ColorSpaceDouble.rgb, mask);
    #elif _DETAIL_MUL
        albedo *= LerpWhiteTo (detailAlbedo, mask);
    #elif _DETAIL_ADD
        albedo += detailAlbedo * mask;
    #elif _DETAIL_LERP
        albedo = lerp (albedo, detailAlbedo, mask);
    #endif
#endif
//Add _AlbedoStrength By HZX
    return albedo * _AlbedoStrength;
}

half Alpha(float2 uv)
{
    return tex2D(_MainTex, uv).a * _Color.a;
}		

half4 FresnelAlpha(float2 uv, float3 pos, half4 final, half3 viewDir, float3 posWorld)
{
#ifdef _FRESNEL_ALPHA
	final.a = (1 - saturate(dot(viewDir, posWorld) + _FresnelAlphaAdd)) * final.a;
	final.a = pow(final.a, _FresnelAlphaExp) * _FresnelAlphaMul;
#ifdef _DRAGON
	half a = 230 * _Scale;
	half b = 135 * _Scale;
	half c = 0.1;
	half d = 0.005;
	final *= 1 - smoothstep(_TransparencyMask - c, _TransparencyMask + c, (pos.x + b) / a) + smoothstep(_TransparencyMask - d, _TransparencyMask + d, (pos.x + b) / a) * smoothstep(_TransparencyMask - d, (pos.x + b) / a + d,  _TransparencyMask) * tex2D(_TransparencyMaskTex, TRANSFORM_TEX(uv, _TransparencyMaskTex)).a * 3;
#endif
	final *= _Transparency;
#endif
	return final;
}	

half Occlusion(float2 uv)
{
//Support SM2.0 By HZX
//#if (SHADER_TARGET < 30)
    // SM20: instruction count limitation
    // SM20: simpler occlusion
//  return tex2D(_MetallicGlossMap, uv).g;
//#else
    half occ = tex2D(_MetallicGlossMap, uv).g;
//	occ = pow(occ,2.2);
    return LerpOneTo (occ, _OcclusionStrength);
//#endif
}

half4 SpecularGloss(float2 uv)
{
    half4 sg;
#ifdef _SPECGLOSSMAP
    sg = tex2D(_SpecGlossMap, uv.xy);
#else
    sg = half4(_SpecColor.rgb, _Glossiness);
#endif
    return sg;
}

half2 MetallicGloss(float2 uv)
{
    half2 mg;
#ifdef _METALLICGLOSSMAP
    mg = tex2D(_MetallicGlossMap, uv.xy).rb;
#else
    mg = half2(_Metallic, _Glossiness);
#endif

//Add _MetallicStrength By HZX
    mg.r *= _MetallicStrength;
//Add _GlossStrength _GlossMin _GlossMax By HZX
//  Gamma Correction
//	mg.g = pow(mg.g, 2.2);
//	改为gloss改为A通道，Linear Gamma space切换正常,引擎在切换Linear Gamma时rgb通道是自己会做一个pow2.2......，只有A通道幸免
//	mg.g = clamp(mg.g * _GlossStrength, _GlossMin, _GlossMax);
    mg.g = lerp(_GlossMin, _GlossMax, mg.g * _GlossStrength);
    return mg;
}

half3 Emission(float2 uv, half3 viewDir, float3 posWorld)
{
	half3 e = 0;
	half3 f = 0;

#ifdef _EMISSION
	e = tex2D(_EmissionMap, uv).rgb * _EmissionColor.rgb * _EmissionMul;
#endif

#ifdef _EMISSION_MASK
	half a = 0.1;
	half b = 0.1;
	e *= 1 - smoothstep(_EmissionMask - a, _EmissionMask + a, smoothstep(_EmissionMaskMin, _EmissionMaskMax, tex2D(_EmissionMap, uv).a)) 
		+ (smoothstep(_EmissionMask - b, _EmissionMask + b, smoothstep(_EmissionMaskMin, _EmissionMaskMax, tex2D(_EmissionMap, uv).a)) 
		* smoothstep(_EmissionMask - b, smoothstep(_EmissionMaskMin, _EmissionMaskMax, tex2D(_EmissionMap, uv).a) + b,  _EmissionMask) 
		* _EmissionMaskMul);
	//e *= clamp((_EmissionMaskMul -  tex2D(_EmissionMap, uv).a + 0.2) * 5, 0, 1);
	//e = tex2D(_EmissionMap, uv).rgb * _EmissionColor.rgb * clamp(_EmissionMaskMul > tex2D(_EmissionMap, uv).a, 0, 1) * _EmissionMul;
#endif
#ifdef _EMISSION_ADD
	half a = tex2D(_EmissionMap, uv).a;
	if (_EmissionAddIndex >= 0 && _EmissionAddIndex < 1 && a > 0 && a <= 0.2)
	{
		e *= _EmissionAddMul;
	}
	else if (_EmissionAddIndex >= 1 && _EmissionAddIndex < 2 && a > 0.2 && a <= 0.4)
	{
		e *= _EmissionAddMul;
	}
	else if (_EmissionAddIndex >= 2 && _EmissionAddIndex < 3 && a > 0.4 && a <= 0.6)
	{
		e *= _EmissionAddMul;
	}
	else if (_EmissionAddIndex >= 3 && _EmissionAddIndex < 4  && a > 0.6 && a <= 0.8)
	{
		e *= _EmissionAddMul;
	}
	else if (_EmissionAddIndex >= 4 && _EmissionAddIndex < 5  && a > 0.8 && a <= 1)
	{
		e *= _EmissionAddMul;
	}
	else
	{
		e = 0;
	}
#endif

#ifdef _FRESNEL
	f = (1 - saturate(dot(viewDir, posWorld)) + _FresnelAdd) * tex2D(_FresnelMap, uv).rgb * _FresnelColor.rgb;
	f = pow(f, _FresnelExp) * _FresnelMul;
#endif
	return e + f;
}

#ifdef _NORMALMAP
half3 NormalInTangentSpace(float4 texcoords)
{
    half3 normalTangent = UnpackScaleNormal(tex2D (_BumpMap, texcoords.xy), _BumpScale);

#if _DETAIL && defined(UNITY_ENABLE_DETAIL_NORMALMAP)
    half mask = DetailMask(texcoords.xy);
    half3 detailNormalTangent = UnpackScaleNormal(tex2D (_DetailNormalMap, texcoords.zw), _DetailNormalMapScale);
    #if _DETAIL_LERP
        normalTangent = lerp(
            normalTangent,
            detailNormalTangent,
            mask);
    #else
        normalTangent = lerp(
            normalTangent,
            BlendNormals(normalTangent, detailNormalTangent),
            mask);
    #endif
#endif

    return normalTangent;
}
#endif

float4 Parallax (float4 texcoords, half3 viewDir)
{
// D3D9/SM30 supports up to 16 samplers, skip the parallax map in case we exceed the limit
#define EXCEEDS_D3D9_SM3_MAX_SAMPLER_COUNT  (defined(LIGHTMAP_ON) && defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) && defined(_NORMALMAP) && \
                                             defined(_EMISSION) && defined(_DETAIL) && (defined(_METALLICGLOSSMAP) || defined(_SPECGLOSSMAP)))

#if !defined(_PARALLAXMAP) || (SHADER_TARGET < 30) || (defined(SHADER_API_D3D9) && EXCEEDS_D3D9_SM3_MAX_SAMPLER_COUNT)
    // SM20: instruction count limitation
    // SM20: no parallax
    return texcoords;
#else
    half h = tex2D (_ParallaxMap, texcoords.xy).g;
    float2 offset = ParallaxOffset1Step (h, _Parallax, viewDir);
    return float4(texcoords.xy + offset, texcoords.zw + offset);
#endif

#undef EXCEEDS_D3D9_SM3_MAX_SAMPLER_COUNT
}

#endif // UNITY_STANDARD_INPUT_INCLUDED

static const float3 c_basis0 = float3(-0.40824829046386301636621401245098f, -0.70710678118654752440084436210485f, 0.57735026918962576450914878050195f);
static const float3 c_basis1 = float3(-0.40824829046386301636621401245098f, 0.70710678118654752440084436210485f, 0.57735026918962576450914878050195f);
static const float3 c_basis2 = float3(0.81649658092772603273242802490196f, 0.0f, 0.57735026918962576450914878050195f);

half4 computeAttenuation(half4 len2, half4 attenConst)
{
	half cutoff = 0.00001;	//attenuation value that is remapped to zero.
						//Calculate Attenuation
	half4 atten = 1.0 / (1.0 + half4(len2.x * attenConst.x, len2.y * attenConst.y, len2.z * attenConst.z, len2.w * attenConst.w));
	//Smooth cutoff so the light doesn't extent forever.
	atten = (atten - cutoff) / (1.0 - cutoff);
	return max(atten, 0.0);
}

void computeFourLight(half index, half3 worldPos, half3 viewPos, half3x3 viewToTangent, inout half3 lightColor0, inout half3 lightColor1, inout half3 lightColor2)
{
	half4x3 lightViewpos = { UnityWorldToViewPos(_VectorLightWorldPosition[index]), UnityWorldToViewPos(_VectorLightWorldPosition[index + 1]), UnityWorldToViewPos(_VectorLightWorldPosition[index + 2]), UnityWorldToViewPos(_VectorLightWorldPosition[index + 3]) };
	half4x3 lightDir = lightViewpos - half4x3(viewPos, viewPos, viewPos, viewPos);
	lightDir = half4x3(mul(viewToTangent, lightDir[0]), mul(viewToTangent, lightDir[1]), mul(viewToTangent, lightDir[2]), mul(viewToTangent, lightDir[3]));
	half4 lengthSq = half4(dot(lightDir[0], lightDir[0]), dot(lightDir[1], lightDir[1]), dot(lightDir[2], lightDir[2]), dot(lightDir[3], lightDir[3]));
	lengthSq = max(lengthSq, 0.000001);
	lightDir = half4x3(lightDir[0] * rsqrt(lengthSq[0]), lightDir[1] * rsqrt(lengthSq[1]), lightDir[2] * rsqrt(lengthSq[2]), lightDir[3] * rsqrt(lengthSq[3]));

	half4 lightRange = half4(_VectorLightAtten[index].r, _VectorLightAtten[index + 1].r, _VectorLightAtten[index + 2].r, _VectorLightAtten[index + 3].r);
	lightRange = max(lightRange, 0.000001);
	half4 dist = half4(distance(worldPos, _VectorLightWorldPosition[index]), distance(worldPos, _VectorLightWorldPosition[index + 1]), distance(worldPos, _VectorLightWorldPosition[index + 2]), distance(worldPos, _VectorLightWorldPosition[index + 3]));
	half4 lightAtten = clamp(half4(dist.x * dist.x / (lightRange.x * lightRange.x), dist.y * dist.y / (lightRange.y * lightRange.y), dist.z * dist.z / (lightRange.z * lightRange.z), dist.w * dist.w / (lightRange.w * lightRange.w)), 0, 1);
	lightAtten = half4(lightAtten.x * lightAtten.x, lightAtten.y * lightAtten.y, lightAtten.z * lightAtten.z, lightAtten.w * lightAtten.w);
	lightAtten = computeAttenuation(lengthSq, lightAtten);

	half4x4 lightColor = { _VectorLightColor[index].r, _VectorLightColor[index].g, _VectorLightColor[index].b, _VectorLightAtten[index].g,
		_VectorLightColor[index + 1].r, _VectorLightColor[index + 1].g, _VectorLightColor[index + 1].b, _VectorLightAtten[index + 1].g,
		_VectorLightColor[index + 2].r, _VectorLightColor[index + 2].g, _VectorLightColor[index + 2].b, _VectorLightAtten[index + 2].g,
		_VectorLightColor[index + 3].r, _VectorLightColor[index + 3].g, _VectorLightColor[index + 3].b, _VectorLightAtten[index + 3].g };
	lightColor = max(lightColor, 0.000001);

	half4x3 color = half4x3(lightColor[0].rgb * lightColor[0].a * lightAtten[0], lightColor[1].rgb * lightColor[1].a * lightAtten[1], lightColor[2].rgb * lightColor[2].a * lightAtten[2], lightColor[3].rgb * lightColor[3].a * lightAtten[3]);

	lightColor0 += (max(0.0, dot(lightDir[0], c_basis0)) * color[0]
		+ max(0.0, dot(lightDir[1], c_basis0)) * color[1]
		+ max(0.0, dot(lightDir[2], c_basis0)) * color[2]
		+ max(0.0, dot(lightDir[3], c_basis0)) * color[3]);

	lightColor1 += (max(0.0, dot(lightDir[0], c_basis1)) * color[0]
		+ max(0.0, dot(lightDir[1], c_basis1)) * color[1]
		+ max(0.0, dot(lightDir[2], c_basis1)) * color[2]
		+ max(0.0, dot(lightDir[3], c_basis1)) * color[3]);

	lightColor2 += (max(0.0, dot(lightDir[0], c_basis2)) * color[0]
		+ max(0.0, dot(lightDir[1], c_basis2)) * color[1]
		+ max(0.0, dot(lightDir[2], c_basis2)) * color[2]
		+ max(0.0, dot(lightDir[3], c_basis2)) * color[3]);

	//for (int i = 0; i < 16; i++) {
	//	//Calculate the scaled light offsets.
	//	half3 lightViewpos = UnityWorldToViewPos(_VectorLightWorldPosition[i]);
	//	half3 lightDir = lightViewpos - viewPos;
	//	//Transform light offsets into tangent space
	//	lightDir = mul(viewToTangent, lightDir);
	//	//Squared length
	//	half lengthSq = dot(lightDir, lightDir);
	//	lengthSq = max(lengthSq, 0.000001);
	//	//Attenuation Constants
	//	// 把点坐标转换到点光源的坐标空间中_LightMatrix0由引擎代码计算后传递到shader中，这里包含了对点光源范围的计算，具体可参考Unity引擎源码。经过_LightMatrix0变换后，在点光源中心处lightCoord为(0, 0, 0)，在点光源的范围边缘处lightCoord为1
	//	//half3 lightCoord = ，half3(0.5, 0.5, 0.5);//mul(unity_WorldToLight, float4(worldPos, 1)).xyz;
	//	// 使用点到光源中心距离的平方dot(lightCoord, lightCoord)构成二维采样坐标，对衰减纹理_LightTexture0采样。_LightTexture0纹理具体长什么样可以看后面的内容
	//	// UNITY_ATTEN_CHANNEL是衰减值所在的纹理通道，可以在内置的HLSLSupport.cginc文件中查看。一般PC和主机平台的话UNITY_ATTEN_CHANNEL是r通道，移动平台的话是a通道
	//	//half lightAtten = tex2D(_LightTextureB0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL;
	//	//Calculate Attenuation
	//	_VectorLightAtten[i] = max(_VectorLightAtten[i], 0.000001);
	//	half dist = distance(worldPos, _VectorLightWorldPosition[i]);

	//	half lightAtten = clamp(dist*dist / (_VectorLightAtten[i].r*_VectorLightAtten[i].r), 0.0, 1.0);
	//	lightAtten *= lightAtten;
	//	half atten = computeAttenuation(lengthSq, lightAtten);
	//	//Normalize light directions.
	//	lightDir *= rsqrt(lengthSq.x);
	//	//Add each secondary light contribution to each basis vector
	//	lightColor0 += max(0.0, dot(lightDir, c_basis0)) * _VectorLightColor[i].rgb * atten * _VectorLightAtten[i].g;
	//	lightColor1 += max(0.0, dot(lightDir, c_basis1)) * _VectorLightColor[i].rgb * atten * _VectorLightAtten[i].g;
	//	lightColor2 += max(0.0, dot(lightDir, c_basis2)) * _VectorLightColor[i].rgb * atten * _VectorLightAtten[i].g;
	//}
}

void computeVectorLight(half4 vertex, half3 normal, half4 tangent, out half3 lightColor0, out half3 lightColor1, out half3 lightColor2)
{
	lightColor0 = 0.0;
	lightColor1 = 0.0;
	lightColor2 = 0.0;
#if !defined(_4_POINT_LIGHT) && !defined(_8_POINT_LIGHT) && !defined(_12_POINT_LIGHT) && !defined(_16_POINT_LIGHT)
	return;
#endif

	half index = 0;
	half3 worldPos = mul(unity_ObjectToWorld, vertex).xyz;
	half3 viewPos = UnityObjectToViewPos(vertex);
	half3 viewNormal = normalize(mul(UNITY_MATRIX_MV, float4(normal.xyz, 0)).xyz);
	half3 viewTangent = normalize(mul(UNITY_MATRIX_MV, float4(tangent.xyz, 0)).xyz);
	half3 binormal = cross(viewNormal, viewTangent) * tangent.w;
	half3x3 viewToTangent = float3x3(viewTangent, binormal, viewNormal);

#if defined(_4_POINT_LIGHT) || defined(_8_POINT_LIGHT) || defined(_12_POINT_LIGHT) || defined(_16_POINT_LIGHT)
	index = 0;
	computeFourLight(index, worldPos, viewPos, viewToTangent, lightColor0, lightColor1, lightColor2);
#endif

#if defined(_8_POINT_LIGHT) || defined(_12_POINT_LIGHT) || defined(_16_POINT_LIGHT)
	index = 4;
	computeFourLight(index, worldPos, viewPos, viewToTangent, lightColor0, lightColor1, lightColor2);
#endif

#if defined(_12_POINT_LIGHT) || defined(_16_POINT_LIGHT)
	index = 8;
	computeFourLight(index, worldPos, viewPos, viewToTangent, lightColor0, lightColor1, lightColor2);
#endif

#if defined(_16_POINT_LIGHT)
	index = 12;
	computeFourLight(index, worldPos, viewPos, viewToTangent, lightColor0, lightColor1, lightColor2);
#endif
}