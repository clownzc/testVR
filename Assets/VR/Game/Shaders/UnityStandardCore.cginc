// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

#ifndef UNITY_STANDARD_CORE_INCLUDED
#define UNITY_STANDARD_CORE_INCLUDED

#include "UnityCG.cginc"
#include "UnityShaderVariables.cginc"
#include "UnityStandardConfig.cginc"
#include "UnityStandardInput.cginc"
#include "UnityPBSLighting.cginc"
#include "UnityStandardUtils.cginc"
#include "UnityGBuffer.cginc"
#include "UnityStandardBRDF.cginc"

#include "AutoLight.cginc"


//--------------------------【函数NormalizePerVertexNormal】-----------------------------
// 用途：归一化每顶点法线
// 说明：若满足特定条件，便归一化每顶点法线并返回，否则，直接返回原始值
// 输入：half3类型的法线坐标
// 输出：若满足判断条件，返回half3类型的、经过归一化后的法线坐标，否则返回输入的值
//-----------------------------------------------------------------------------------------------
// counterpart for NormalizePerPixelNormal
// skips normalization per-vertex and expects normalization to happen per-pixel
half3 NormalizePerVertexNormal (half3 n)
{
	//满足着色目标模型的版本小于Shader Model 3.0，或者定义了UNITY_STANDARD_SIMPLE宏，返回归一化后的值
    #if (SHADER_TARGET < 30) || UNITY_STANDARD_SIMPLE
        return normalize(n);
	//否则，直接返回输入的参数，后续应该会进行逐像素的归一化
    #else
        return n; // will normalize per-pixel instead
    #endif
}

half3 NormalizePerPixelNormal (half3 n)
{
    #if (SHADER_TARGET < 30) || UNITY_STANDARD_SIMPLE
        return n;
    #else
        return normalize(n);
    #endif
}

//-------------------------------------------------------------------------------------
UnityLight MainLight ()
{
    UnityLight l;

    l.color = _LightColor0.rgb;
    l.dir = _WorldSpaceLightPos0.xyz;
    return l;
}

UnityLight AdditiveLight (half3 lightDir, half atten)
{
    UnityLight l;

    l.color = _LightColor0.rgb;
    l.dir = lightDir;
    #ifndef USING_DIRECTIONAL_LIGHT
        l.dir = NormalizePerPixelNormal(l.dir);
    #endif

    // shadow the light
    l.color *= atten;
    return l;
}

UnityLight DummyLight ()
{
    UnityLight l;
    l.color = 0;
    l.dir = half3 (0,1,0);
    return l;
}

UnityIndirect ZeroIndirect ()
{
    UnityIndirect ind;
    ind.diffuse = 0;
    ind.specular = 0;
    return ind;
}

//-------------------------------------------------------------------------------------
// Common fragment setup

// deprecated
half3 WorldNormal(half4 tan2world[3])
{
    return normalize(tan2world[2].xyz);
}

// deprecated
#ifdef _TANGENT_TO_WORLD
    half3x3 ExtractTangentToWorldPerPixel(half4 tan2world[3])
    {
        half3 t = tan2world[0].xyz;
        half3 b = tan2world[1].xyz;
        half3 n = tan2world[2].xyz;

    #if UNITY_TANGENT_ORTHONORMALIZE
        n = NormalizePerPixelNormal(n);

        // ortho-normalize Tangent
        t = normalize (t - n * dot(t, n));

        // recalculate Binormal
        half3 newB = cross(n, t);
        b = newB * sign (dot (newB, b));
    #endif

        return half3x3(t, b, n);
    }
#else
    half3x3 ExtractTangentToWorldPerPixel(half4 tan2world[3])
    {
        return half3x3(0,0,0,0,0,0,0,0,0);
    }
#endif

half3 PerPixelWorldNormal(float4 i_tex, half4 tangentToWorld[3])
{
#ifdef _NORMALMAP
    half3 tangent = tangentToWorld[0].xyz;
    half3 binormal = tangentToWorld[1].xyz;
    half3 normal = tangentToWorld[2].xyz;

    #if UNITY_TANGENT_ORTHONORMALIZE
        normal = NormalizePerPixelNormal(normal);

        // ortho-normalize Tangent
        tangent = normalize (tangent - normal * dot(tangent, normal));

        // recalculate Binormal
        half3 newB = cross(normal, tangent);
        binormal = newB * sign (dot (newB, binormal));
    #endif

    half3 normalTangent = NormalInTangentSpace(i_tex);
    half3 normalWorld = NormalizePerPixelNormal(tangent * normalTangent.x + binormal * normalTangent.y + normal * normalTangent.z); // @TODO: see if we can squeeze this normalize on SM2.0 as well
#else
    half3 normalWorld = normalize(tangentToWorld[2].xyz);
#endif
    return normalWorld;
}

#ifdef _PARALLAXMAP
    #define IN_VIEWDIR4PARALLAX(i) NormalizePerPixelNormal(half3(i.tangentToWorldAndPackedData[0].w,i.tangentToWorldAndPackedData[1].w,i.tangentToWorldAndPackedData[2].w))
    #define IN_VIEWDIR4PARALLAX_FWDADD(i) NormalizePerPixelNormal(i.viewDirForParallax.xyz)
#else
    #define IN_VIEWDIR4PARALLAX(i) half3(0,0,0)
    #define IN_VIEWDIR4PARALLAX_FWDADD(i) half3(0,0,0)
#endif

#if UNITY_REQUIRE_FRAG_WORLDPOS
    #if UNITY_PACK_WORLDPOS_WITH_TANGENT
        #define IN_WORLDPOS(i) half3(i.tangentToWorldAndPackedData[0].w,i.tangentToWorldAndPackedData[1].w,i.tangentToWorldAndPackedData[2].w)
    #else
        #define IN_WORLDPOS(i) i.posWorld
    #endif
    #define IN_WORLDPOS_FWDADD(i) i.posWorld
#else
    #define IN_WORLDPOS(i) half3(0,0,0)
    #define IN_WORLDPOS_FWDADD(i) half3(0,0,0)
#endif

#define IN_LIGHTDIR_FWDADD(i) half3(i.tangentToWorldAndLightDir[0].w, i.tangentToWorldAndLightDir[1].w, i.tangentToWorldAndLightDir[2].w)

#define FRAGMENT_SETUP(x) FragmentCommonData x = \
    FragmentSetup(i.tex, i.eyeVec, IN_VIEWDIR4PARALLAX(i), i.tangentToWorldAndPackedData, IN_WORLDPOS(i));

#define FRAGMENT_SETUP_FWDADD(x) FragmentCommonData x = \
    FragmentSetup(i.tex, i.eyeVec, IN_VIEWDIR4PARALLAX_FWDADD(i), i.tangentToWorldAndLightDir, IN_WORLDPOS_FWDADD(i));

struct FragmentCommonData
{
    half3 diffColor, specColor;
    // Note: smoothness & oneMinusReflectivity for optimization purposes, mostly for DX9 SM2.0 level.
    // Most of the math is being done on these (1-x) values, and that saves a few precious ALU slots.
    half oneMinusReflectivity, smoothness;
    half3 normalWorld, eyeVec, posWorld;
    half alpha;

#if UNITY_STANDARD_SIMPLE
    half3 reflUVW;
#endif

#if UNITY_STANDARD_SIMPLE
    half3 tangentSpaceNormal;
#endif
};

#ifndef UNITY_SETUP_BRDF_INPUT
    #define UNITY_SETUP_BRDF_INPUT SpecularSetup
#endif

inline FragmentCommonData SpecularSetup (float4 i_tex)
{
    half4 specGloss = SpecularGloss(i_tex.xy);
    half3 specColor = specGloss.rgb;
    half smoothness = specGloss.a;

    half oneMinusReflectivity;
    half3 diffColor = EnergyConservationBetweenDiffuseAndSpecular (Albedo(i_tex), specColor, /*out*/ oneMinusReflectivity);

    FragmentCommonData o = (FragmentCommonData)0;
    o.diffColor = diffColor;
    o.specColor = specColor;
    o.oneMinusReflectivity = oneMinusReflectivity;
    o.smoothness = smoothness;
    return o;
}

//片段着色函数 Metallic workflow公共数据
inline FragmentCommonData MetallicSetup (float4 i_tex)
{
	half2 metallicGloss = MetallicGloss(i_tex.xy);
	half metallic = metallicGloss.x;
	//gloss
	half smoothness = metallicGloss.y; // this is 1 minus the square root of real roughness m.

    half oneMinusReflectivity;
    half3 specColor;
    half3 diffColor = DiffuseAndSpecularFromMetallic (Albedo(i_tex), metallic, /*out*/ specColor, /*out*/ oneMinusReflectivity);

    FragmentCommonData o = (FragmentCommonData)0;
    o.diffColor = diffColor;
    o.specColor = specColor;
    o.oneMinusReflectivity = oneMinusReflectivity;
    o.smoothness = smoothness;
    return o;
}

inline FragmentCommonData FragmentSetup (float4 i_tex, half3 i_eyeVec, half3 i_viewDirForParallax, half4 tangentToWorld[3], half3 i_posWorld)
{
    i_tex = Parallax(i_tex, i_viewDirForParallax);

    half alpha = Alpha(i_tex.xy);
    #if defined(_ALPHATEST_ON)
        clip (alpha - _Cutoff);
    #endif

    FragmentCommonData o = UNITY_SETUP_BRDF_INPUT (i_tex);
    o.normalWorld = PerPixelWorldNormal(i_tex, tangentToWorld);
    o.eyeVec = NormalizePerPixelNormal(i_eyeVec);
    o.posWorld = i_posWorld;

    // NOTE: shader relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)
    o.diffColor = PreMultiplyAlpha (o.diffColor, alpha, o.oneMinusReflectivity, /*out*/ o.alpha);
    return o;
}
//----------------------------------------------------------FragmentGI----------------------------------------------------------------------
//函数：片段着色部分全局光照的处理函数
inline UnityGI FragmentGI (FragmentCommonData s, half occlusion, half4 i_ambientOrLightmapUV, half atten, UnityLight light, bool reflections)
{
	//【1】实例化一个UnityGIInput的对象
    UnityGIInput d;
	//【2】填充此UnityGIInput对象的各个值
    d.light = light;
    d.worldPos = s.posWorld;
    d.worldViewDir = -s.eyeVec;
    d.atten = atten;
    #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
        d.ambient = 0;
        d.lightmapUV = i_ambientOrLightmapUV;
    #else
        d.ambient = i_ambientOrLightmapUV.rgb;
        d.lightmapUV = 0;
    #endif

    d.probeHDR[0] = unity_SpecCube0_HDR;
    d.probeHDR[1] = unity_SpecCube1_HDR;
    #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
      d.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
    #endif
    #ifdef UNITY_SPECCUBE_BOX_PROJECTION
      d.boxMax[0] = unity_SpecCube0_BoxMax;
      d.probePosition[0] = unity_SpecCube0_ProbePosition;
      d.boxMax[1] = unity_SpecCube1_BoxMax;
      d.boxMin[1] = unity_SpecCube1_BoxMin;
      d.probePosition[1] = unity_SpecCube1_ProbePosition;
    #endif

	//【3】根据填充好的UnityGIInput结构体对象，调用一下UnityGlobalIllumination函数
    if(reflections)
    {
        Unity_GlossyEnvironmentData g = UnityGlossyEnvironmentSetup(s.smoothness, -s.eyeVec, s.normalWorld, s.specColor);
        // Replace the reflUVW if it has been compute in Vertex shader. Note: the compiler will optimize the calcul in UnityGlossyEnvironmentSetup itself
        #if UNITY_STANDARD_SIMPLE
            g.reflUVW = s.reflUVW;
        #endif

        return UnityGlobalIllumination (d, occlusion, s.normalWorld, g);
    }
    else
    {
        return UnityGlobalIllumination (d, occlusion, s.normalWorld);
    }
}

inline UnityGI FragmentGI (FragmentCommonData s, half occlusion, half4 i_ambientOrLightmapUV, half atten, UnityLight light)
{
    return FragmentGI(s, occlusion, i_ambientOrLightmapUV, atten, light, true);
}
//--------------------------------------------------------------------------------------------------------------------------------------

//-------------------------------------------------------------------------------------
half4 OutputForward (half4 output, half alphaFromSurface)
{
    #if defined(_ALPHABLEND_ON) || defined(_ALPHAPREMULTIPLY_ON)
        output.a = alphaFromSurface;
    #else
        UNITY_OPAQUE_ALPHA(output.a);
    #endif
    return output;
}

//---------------------------------------------------------VertexGIForward---------------------------------------------------------------
//函数：顶点着色部分正向光照的处理函数
inline half4 VertexGIForward(VertexInput v, float3 posWorld, half3 normalWorld)
{
    half4 ambientOrLightmapUV = 0;
    // Static lightmaps
    #ifdef LIGHTMAP_ON
        ambientOrLightmapUV.xy = v.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
        ambientOrLightmapUV.zw = 0;
    // Sample light probe for Dynamic objects only (no static or dynamic lightmaps)
    #elif UNITY_SHOULD_SAMPLE_SH
		//ambientOrLightmapUV.rgb = max(half3(0, 0, 0), ShadeSH9(half4(normalWorld, 1.0)));
		//#if UNITY_SAMPLE_FULL_SH_PER_PIXEL  // TODO: remove this path
			//ambientOrLightmapUV.rgb = 0;
		//#elif (SHADER_TARGET < 30) || UNITY_STANDARD_SIMPLE
			//ambientOrLightmapUV.rgb = ShadeSHPerVertex(normalWorld, ambientOrLightmapUV.rgb);
			//ambientOrLightmapUV.rgb = ShadeSH9(half4(normalWorld, 1.0));
		//#else
			// Optimization: L2 per-vertex, L0..L1 per-pixel
			// Use in the Android
			//ambientOrLightmapUV.rgb = ShadeSH3Order(half4(normalWorld, 1.0));
		//#endif
		// Add approximated illumination from non-important point lights
		#ifdef _VERTEX_LIGHT
			half3 color0;
			half3 color1;
			half3 color2;
			computeVectorLight(v.vertex, v.normal, v.tangent, color0, color1, color2);
			//0.3184 = 1/pi
			ambientOrLightmapUV.rgb += (color0 + color1 + color2) * 0.3184;
		#elif VERTEXLIGHT_ON
            ambientOrLightmapUV.rgb += Shade4PointLights (
                unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                unity_4LightAtten0, posWorld, normalWorld);
        #endif
    #endif

    #ifdef DYNAMICLIGHTMAP_ON
        ambientOrLightmapUV.zw = v.uv2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
    #endif

    return ambientOrLightmapUV;
}
//----------------------------------------------------------------------------------------------------------------------------------------

// ------------------------------------------------------------【VertexForwardBase函数】---------------------------------------------------
//  Base forward pass (directional light, emission, lightmaps, ...)

#if UNITY_STANDARD_SIMPLE == 0

struct VertexOutputForwardBase
{
    float4 pos                          : SV_POSITION;//像素坐标
    float4 tex                          : TEXCOORD0;//一级纹理
    half3 eyeVec                        : TEXCOORD1;//二级纹理(视线向量)
    half4 tangentToWorldAndPackedData[3]    : TEXCOORD2;    // [3x3:tangentToWorld | 1x3:viewDirForParallax]//3x3为切线到世界矩阵的值，1x3为视差方向的值
//	half3 normalDir						: TEXCOORD3;
    half4 ambientOrLightmapUV           : TEXCOORD5;    // SH or Lightmap UV // 球谐函数（Spherical harmonics）或光照贴图的UV坐标
    UNITY_SHADOW_COORDS(6)//阴影坐标
    #if _FOG
    UNITY_FOG_COORDS(7)//雾效坐标
    #endif
    //若定义了镜面立方体投影宏，定义一个posWorld 
    // next ones would not fit into SM2.0 limits, but they are always for SM3.0+
    //#if UNITY_SPECCUBE_BOX_PROJECTION
		float3 posWorld					: TEXCOORD8;
	//#endif

	//若定义了优化纹理的立方体LOD宏，还将定义如下的参数reflUVW
	#if UNITY_OPTIMIZE_TEXCUBELOD
		#if UNITY_SPECCUBE_BOX_PROJECTION
			half3 reflUVW				: TEXCOORD9;
		#else
			half3 reflUVW				: TEXCOORD10;
		#endif
	#endif

	float3 vex							: TEXCOORD11;
};

VertexOutputForwardBase vertForwardBase (VertexInput v)
{
    VertexOutputForwardBase o;
    UNITY_INITIALIZE_OUTPUT(VertexOutputForwardBase, o);

    o.vex = v.vertex;

    float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
    //#if UNITY_SPECCUBE_BOX_PROJECTION
        o.posWorld = posWorld.xyz;
    //#endif
    o.pos = UnityObjectToClipPos(v.vertex);

    o.tex = TexCoords(v);
    o.eyeVec = NormalizePerVertexNormal(posWorld.xyz - _WorldSpaceCameraPos);
    float3 normalWorld = UnityObjectToWorldNormal(v.normal);

//	o.normalDir = normalWorld.rgb;
    #ifdef _TANGENT_TO_WORLD
        float4 tangentWorld = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);

        float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);
        o.tangentToWorldAndPackedData[0].xyz = tangentToWorld[0];
        o.tangentToWorldAndPackedData[1].xyz = tangentToWorld[1];
        o.tangentToWorldAndPackedData[2].xyz = tangentToWorld[2];
    #else
        o.tangentToWorldAndPackedData[0].xyz = 0;
        o.tangentToWorldAndPackedData[1].xyz = 0;
        o.tangentToWorldAndPackedData[2].xyz = normalWorld;
    #endif

    //We need this for shadow receving
    TRANSFER_SHADOW(o);

    o.ambientOrLightmapUV = VertexGIForward(v, posWorld, normalWorld);

    //#ifdef _PARALLAXMAP
    //	TANGENT_SPACE_ROTATION;
    //	half3 viewDirForParallax = mul (rotation, ObjSpaceViewDir(v.vertex));
    //	o.tangentToWorldAndPackedData[0].w = viewDirForParallax.x;
    //	o.tangentToWorldAndPackedData[1].w = viewDirForParallax.y;
    //	o.tangentToWorldAndPackedData[2].w = viewDirForParallax.z;
    //#endif

    #if UNITY_OPTIMIZE_TEXCUBELOD
        o.reflUVW 		= reflect(o.eyeVec, normalWorld);
    #endif

    #if _FOG
    UNITY_TRANSFER_FOG(o,o.pos);
    #endif

    return o;
}
// ----------------------------------------------------------------------------------------------------------------------------------------

//----------------------------------------【fragForwardBase函数】-------------------------------------------
//  用途：正向渲染基础通道的片段着色函数
//  输入：VertexOutputForwardBase结构体
//  输出：一个half4类型的颜色值
//------------------------------------------------------------------------------------------------------------------
half4 fragForwardBase (VertexOutputForwardBase i) : SV_Target
{
	//定义并初始化类型为FragmentCommonData的变量s
	FRAGMENT_SETUP(s)
	//若定义了UNITY_OPTIMIZE_TEXCUBELOD，则由输入的顶点参数来设置反射光方向向量
#if UNITY_OPTIMIZE_TEXCUBELOD
	s.reflUVW		= i.reflUVW;
#endif

	half3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);

//	half3 normal = PerPixelWorldNormal(float4 i_tex, half4 tangentToWorld[3]);
	//设置主光照
	UnityLight mainLight = MainLight ();

	//设置阴影的衰减系数
	UNITY_LIGHT_ATTENUATION(atten, i, s.posWorld);


	//计算全局光照
	half occlusion = Occlusion(i.tex.xy);
	UnityGI gi = FragmentGI (s, occlusion, i.ambientOrLightmapUV, atten, mainLight);

//	half4 c = 0;
	//加上BRDF-基于物理的光照

	half4 c = UNITY_BRDF_PBS (s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, gi.light, gi.indirect);
	//half4 c = half4(gi.indirect.specular, 1);
	//加上BRDF-全局光照
	//加上lightmap
	c.rgb += UNITY_BRDF_GI (s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, occlusion, gi);
	//加上自发光
	c.rgb += Emission(i.tex.xy, viewDirection , s.normalWorld);
	//c.rgb = gi.indirect.diffuse.rgb;
	//c.rgb = gi.indirect.diffuse.rgb * 1 + 1 - 1;
	//c.rgb = i.ambientOrLightmapUV.xyz;
//	c.rgb = pow(c, 1 / 2.2);
	//设置雾效
	#if _FOG
	UNITY_APPLY_FOG(i.fogCoord, c.rgb);
	#endif

	//返回最终的颜色
//	return OutputForward (pow(c,1/2.2), s.alpha);
	half4 final = OutputForward (c, s.alpha);

	final = FresnelAlpha(i.tex.xy, i.vex.xyz, final, viewDirection, s.normalWorld);
	return final;
}

#else // UNITY_STANDARD_SIMPLE

//  Does not support: _PARALLAXMAP, DIRLIGHTMAP_COMBINED, DIRLIGHTMAP_SEPARATE
#define GLOSSMAP (defined(_SPECGLOSSMAP) || defined(_METALLICGLOSSMAP))

#ifndef SPECULAR_HIGHLIGHTS
	#define SPECULAR_HIGHLIGHTS 1
#endif

struct VertexOutputBaseSimple
{
	float4 pos							: SV_POSITION;
	float4 tex							: TEXCOORD0;
	half4 eyeVec 						: TEXCOORD1; // w: grazingTerm

	half4 ambientOrLightmapUV			: TEXCOORD2; // SH or Lightmap UV
	SHADOW_COORDS(3)

	#if _FOG
	UNITY_FOG_COORDS(4)
	#endif

	half3 reflectVec					: TEXCOORD5;
	half4 normalWorld					: TEXCOORD6; // w: fresnelTerm

#ifdef _NORMALMAP
	half3 tangentSpaceLightDir			: TEXCOORD7;
	#if SPECULAR_HIGHLIGHTS
		half3 tangentSpaceEyeVec		: TEXCOORD8;
	#endif
#endif
#if UNITY_SPECCUBE_BOX_PROJECTION
	float3 posWorld						: TEXCOORD9;
#endif

	float3 vex							: TEXCOORD10;	
};

// UNIFORM_REFLECTIVITY(): workaround to get (uniform) reflecivity based on UNITY_SETUP_BRDF_INPUT
half MetallicSetup_Reflectivity()
{
	return 1.0h - OneMinusReflectivityFromMetallic(_Metallic);
}

half SpecularSetup_Reflectivity()
{
	return SpecularStrength(_SpecColor.rgb);
}

#define JOIN2(a, b) a##b
#define JOIN(a, b) JOIN2(a,b)
#define UNIFORM_REFLECTIVITY JOIN(UNITY_SETUP_BRDF_INPUT, _Reflectivity)


#ifdef _NORMALMAP

half3 TransformToTangentSpace(half3 tangent, half3 binormal, half3 normal, half3 v)
{
	// Mali400 shader compiler prefers explicit dot product over using a half3x3 matrix
	return half3(dot(tangent, v), dot(binormal, v), dot(normal, v));
}

void TangentSpaceLightingInput(half3 normalWorld, half4 vTangent, half3 lightDirWorld, half3 eyeVecWorld, out half3 tangentSpaceLightDir, out half3 tangentSpaceEyeVec)
{
	half3 tangentWorld = UnityObjectToWorldDir(vTangent.xyz);
	half sign = half(vTangent.w) * half(unity_WorldTransformParams.w);
	half3 binormalWorld = cross(normalWorld, tangentWorld) * sign;
	tangentSpaceLightDir = TransformToTangentSpace(tangentWorld, binormalWorld, normalWorld, lightDirWorld);
	#if SPECULAR_HIGHLIGHTS
		tangentSpaceEyeVec = normalize(TransformToTangentSpace(tangentWorld, binormalWorld, normalWorld, eyeVecWorld));
	#else
		tangentSpaceEyeVec = 0;
	#endif
}

#endif // _NORMALMAP

VertexOutputBaseSimple vertForwardBase (VertexInput v)
{
	VertexOutputBaseSimple o;
	UNITY_INITIALIZE_OUTPUT(VertexOutputBaseSimple, o);

	float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
#if UNITY_SPECCUBE_BOX_PROJECTION
	o.posWorld = posWorld.xyz;
#endif
	o.pos = UnityObjectToClipPos(v.vertex);
	o.tex = TexCoords(v);
	o.vex = v.vertex;
	
	half3 eyeVec = normalize(posWorld.xyz - _WorldSpaceCameraPos);
	half3 normalWorld = UnityObjectToWorldNormal(v.normal);
	
	o.normalWorld.xyz = normalWorld;
	o.eyeVec.xyz = eyeVec;

	#ifdef _NORMALMAP
		half3 tangentSpaceEyeVec;
		TangentSpaceLightingInput(normalWorld, v.tangent, _WorldSpaceLightPos0.xyz, eyeVec, o.tangentSpaceLightDir, tangentSpaceEyeVec);
		#if SPECULAR_HIGHLIGHTS
			o.tangentSpaceEyeVec = tangentSpaceEyeVec;
		#endif
	#endif

	//We need this for shadow receiving
	TRANSFER_SHADOW(o);

	o.ambientOrLightmapUV = VertexGIForward(v, posWorld, normalWorld);

	o.reflectVec.xyz = reflect(eyeVec, normalWorld);

	o.normalWorld.w = Pow4(1 - DotClamped (normalWorld, -eyeVec)); // fresnel term
	#if !GLOSSMAP
		o.eyeVec.w = saturate(_Glossiness + UNIFORM_REFLECTIVITY()); // grazing term
	#endif

	#if _FOG
	UNITY_TRANSFER_FOG(o, o.pos);
	#endif

	return o;
}


FragmentCommonData FragmentSetupSimple(VertexOutputBaseSimple i)
{
	half alpha = Alpha(i.tex.xy);
	#if defined(_ALPHATEST_ON)
		clip (alpha - _Cutoff);
	#endif

	FragmentCommonData s = UNITY_SETUP_BRDF_INPUT (i.tex);

	// NOTE: shader relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)
	s.diffColor = PreMultiplyAlpha (s.diffColor, alpha, s.oneMinusReflectivity, /*out*/ s.alpha);

	s.normalWorld = i.normalWorld.xyz;
	s.eyeVec = i.eyeVec.xyz;
	s.posWorld = IN_WORLDPOS(i);
	s.reflUVW = i.reflectVec;

	#ifdef _NORMALMAP
		s.tangentSpaceNormal =  NormalInTangentSpace(i.tex);
	#else
		s.tangentSpaceNormal =  0;
	#endif

	return s;
}

UnityLight MainLightSimple(VertexOutputBaseSimple i, FragmentCommonData s)
{
	UnityLight mainLight = MainLight(s.normalWorld);
	#if defined(LIGHTMAP_OFF) && defined(_NORMALMAP)
		mainLight.ndotl = LambertTerm(s.tangentSpaceNormal, i.tangentSpaceLightDir);
	#endif
	return mainLight;
}

half PerVertexGrazingTerm(VertexOutputBaseSimple i, FragmentCommonData s)
{
	#if GLOSSMAP
		return saturate(s.oneMinusRoughness + (1-s.oneMinusReflectivity));
	#else
		return i.eyeVec.w;
	#endif
}

half PerVertexFresnelTerm(VertexOutputBaseSimple i)
{
	return i.normalWorld.w;
}

#if !SPECULAR_HIGHLIGHTS
#	define REFLECTVEC_FOR_SPECULAR(i, s) half3(0, 0, 0)
#elif defined(_NORMALMAP)
#	define REFLECTVEC_FOR_SPECULAR(i, s) reflect(i.tangentSpaceEyeVec, s.tangentSpaceNormal)
#else
#	define REFLECTVEC_FOR_SPECULAR(i, s) s.reflUVW
#endif

half3 LightDirForSpecular(VertexOutputBaseSimple i, UnityLight mainLight)
{
	#if SPECULAR_HIGHLIGHTS && defined(_NORMALMAP)
		return i.tangentSpaceLightDir;
	#else
		return mainLight.dir;
	#endif
}

half3 BRDF3DirectSimple(half3 diffColor, half3 specColor, half oneMinusRoughness, half rl)
{
	#if SPECULAR_HIGHLIGHTS
		return BRDF3_Direct(diffColor, specColor, Pow4(rl), oneMinusRoughness);
	#else
		return diffColor;
	#endif
}

half4 fragForwardBase (VertexOutputBaseSimple i) : SV_Target
{
	FragmentCommonData s = FragmentSetupSimple(i);

	UnityLight mainLight = MainLightSimple(i, s);	
	UNITY_LIGHT_ATTENUATION(atten, i, s.posWorld);
	
	half occlusion = Occlusion(i.tex.xy);
	half rl = dot(REFLECTVEC_FOR_SPECULAR(i, s), LightDirForSpecular(i, mainLight));
	
	UnityGI gi = FragmentGI (s, occlusion, i.ambientOrLightmapUV, atten, mainLight);
	half3 attenuatedLightColor = gi.light.color * mainLight.ndotl;

	half3 c = BRDF3_Indirect(s.diffColor, s.specColor, gi.indirect, PerVertexGrazingTerm(i, s), PerVertexFresnelTerm(i));
	c += BRDF3DirectSimple(s.diffColor, s.specColor, s.oneMinusRoughness, rl) * attenuatedLightColor;
	c += UNITY_BRDF_GI (s.diffColor, s.specColor, s.oneMinusReflectivity, s.oneMinusRoughness, s.normalWorld, -s.eyeVec, occlusion, gi);
	c += Emission(i.tex.xy, -i.eyeVec, s.normalWorld);

	#if _FOG
	UNITY_APPLY_FOG(i.fogCoord, c);
	#endif

	half4 final = OutputForward (half4(c, 1), s.alpha);
	final = FresnelAlpha(i.tex.xy, i.vex.xyz, final, -i.eyeVec, s.normalWorld);
	return final;
}

#endif

//----------------------------------------【vertexForwardAdd函数】----------------------------------------------
//  Additive forward pass (one light per pass)

#if UNITY_STANDARD_SIMPLE == 0

struct VertexOutputForwardAdd
{
    float4 pos                          : SV_POSITION;
    float4 tex                          : TEXCOORD0;
    half3 eyeVec                        : TEXCOORD1;
    half4 tangentToWorldAndLightDir[3]  : TEXCOORD2;    // [3x3:tangentToWorld | 1x3:lightDir]
    float3 posWorld                     : TEXCOORD5;
    UNITY_SHADOW_COORDS(6)
    UNITY_FOG_COORDS(7)

    // next ones would not fit into SM2.0 limits, but they are always for SM3.0+
#if defined(_PARALLAXMAP)
    half3 viewDirForParallax            : TEXCOORD8;
#endif
};

VertexOutputForwardAdd vertForwardAdd (VertexInput v)
{
	VertexOutputForwardAdd o;
	UNITY_INITIALIZE_OUTPUT(VertexOutputForwardAdd, o);

	float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
	o.pos = UnityObjectToClipPos(v.vertex);
	o.tex = TexCoords(v);
	o.eyeVec = NormalizePerVertexNormal(posWorld.xyz - _WorldSpaceCameraPos);
	float3 normalWorld = UnityObjectToWorldNormal(v.normal);
	#ifdef _TANGENT_TO_WORLD
		float4 tangentWorld = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);

		float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);
		o.tangentToWorldAndLightDir[0].xyz = tangentToWorld[0];
		o.tangentToWorldAndLightDir[1].xyz = tangentToWorld[1];
		o.tangentToWorldAndLightDir[2].xyz = tangentToWorld[2];
	#else
		o.tangentToWorldAndLightDir[0].xyz = 0;
		o.tangentToWorldAndLightDir[1].xyz = 0;
		o.tangentToWorldAndLightDir[2].xyz = normalWorld;
	#endif
	//We need this for shadow receiving
	TRANSFER_VERTEX_TO_FRAGMENT(o);

	float3 lightDir = _WorldSpaceLightPos0.xyz - posWorld.xyz * _WorldSpaceLightPos0.w;
	#ifndef USING_DIRECTIONAL_LIGHT
		lightDir = NormalizePerVertexNormal(lightDir);
	#endif
	o.tangentToWorldAndLightDir[0].w = lightDir.x;
	o.tangentToWorldAndLightDir[1].w = lightDir.y;
	o.tangentToWorldAndLightDir[2].w = lightDir.z;

	#ifdef _PARALLAXMAP
		TANGENT_SPACE_ROTATION;
		o.viewDirForParallax = mul (rotation, ObjSpaceViewDir(v.vertex));
	#endif
	
	UNITY_TRANSFER_FOG(o,o.pos);
	return o;
}
//------------------------------------------------------------------------------------------------------------------

//----------------------------------------【fragForwardAdd函数】-------------------------------------------
//  用途：正向渲染基础通道的片段着色函数  ForwardAdd 点光
//  输入：VertexOutputForwardAdd结构体
//  输出：一个half4类型的颜色值
//------------------------------------------------------------------------------------------------------------------
half4 fragForwardAdd (VertexOutputForwardAdd i) : SV_Target
{
	//定义并初始化类型为FragmentCommonData的变量s
	FRAGMENT_SETUP_FWDADD(s)

	UNITY_LIGHT_ATTENUATION(atten, i, s.posWorld)
    UnityLight light = AdditiveLight (IN_LIGHTDIR_FWDADD(i), atten);
	//UnityLight light = AdditiveLight (IN_LIGHTDIR_FWDADD(i), LIGHT_ATTENUATION(i));
	UnityIndirect noIndirect = ZeroIndirect ();

	//加上BRDF-基于物理的光照
	half4 c = UNITY_BRDF_PBS (s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, light, noIndirect);
	
	//设置雾效
	UNITY_APPLY_FOG_COLOR(i.fogCoord, c.rgb, half4(0,0,0,0)); // fog towards black in additive pass
	
	//返回最终的颜色
	half4 final = OutputForward (c, s.alpha);
	return final;
}

#else

struct VertexOutputForwardAddSimple
{
	float4 pos							: SV_POSITION;
	float4 tex							: TEXCOORD0;

	LIGHTING_COORDS(1,2)
	UNITY_FOG_COORDS(3)

	half3 lightDir						: TEXCOORD4;

#if defined(_NORMALMAP)
	#if SPECULAR_HIGHLIGHTS
		half3 tangentSpaceEyeVec		: TEXCOORD5;
	#endif
#else
	half3 normalWorld					: TEXCOORD5;
	#if SPECULAR_HIGHLIGHTS
		half3 reflectVec				: TEXCOORD6;
	#endif
#endif
};

VertexOutputForwardAddSimple vertForwardAdd (VertexInput v)
{
	VertexOutputForwardAddSimple o;
	UNITY_INITIALIZE_OUTPUT(VertexOutputForwardAddSimple, o);

	float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
	o.pos = UnityObjectToClipPos(v.vertex);
	o.tex = TexCoords(v);

	//We need this for shadow receiving
	TRANSFER_VERTEX_TO_FRAGMENT(o);

	half3 lightDir = _WorldSpaceLightPos0.xyz - posWorld.xyz * _WorldSpaceLightPos0.w;
	#ifndef USING_DIRECTIONAL_LIGHT
		lightDir = NormalizePerVertexNormal(lightDir);
	#endif

	#if SPECULAR_HIGHLIGHTS
		half3 eyeVec = normalize(posWorld.xyz - _WorldSpaceCameraPos);
	#endif

	half3 normalWorld = UnityObjectToWorldNormal(v.normal);

	#ifdef _NORMALMAP
		#if SPECULAR_HIGHLIGHTS
			TangentSpaceLightingInput(normalWorld, v.tangent, lightDir, eyeVec, o.lightDir, o.tangentSpaceEyeVec);
		#else
			half3 ignore;
			TangentSpaceLightingInput(normalWorld, v.tangent, lightDir, 0, o.lightDir, ignore);
		#endif
	#else
		o.lightDir = lightDir;
		o.normalWorld = normalWorld;
		#if SPECULAR_HIGHLIGHTS
			o.reflectVec = reflect(eyeVec, normalWorld);
		#endif
	#endif

	UNITY_TRANSFER_FOG(o,o.pos);
	return o;
}

FragmentCommonData FragmentSetupSimpleAdd(VertexOutputForwardAddSimple i)
{
	half alpha = Alpha(i.tex.xy);
	#if defined(_ALPHATEST_ON)
		clip (alpha - _Cutoff);
	#endif

	FragmentCommonData s = UNITY_SETUP_BRDF_INPUT (i.tex);

	// NOTE: shader relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)
	s.diffColor = PreMultiplyAlpha (s.diffColor, alpha, s.oneMinusReflectivity, /*out*/ s.alpha);

	s.eyeVec = 0;
	s.posWorld = 0;

	#ifdef _NORMALMAP
		s.tangentSpaceNormal = NormalInTangentSpace(i.tex);
		s.normalWorld = 0;
	#else
		s.tangentSpaceNormal = 0;
		s.normalWorld = i.normalWorld;
	#endif

	#if SPECULAR_HIGHLIGHTS && !defined(_NORMALMAP)
		s.reflUVW = i.reflectVec;
	#else
		s.reflUVW = 0;
	#endif

	return s;
}

half3 LightSpaceNormal(VertexOutputForwardAddSimple i, FragmentCommonData s)
{
	#ifdef _NORMALMAP
		return s.tangentSpaceNormal;
	#else
		return i.normalWorld;
	#endif
}

half4 fragForwardAdd (VertexOutputForwardAddSimple i) : SV_Target
{
	FragmentCommonData s = FragmentSetupSimpleAdd(i);

	half3 c = BRDF3DirectSimple(s.diffColor, s.specColor, s.oneMinusRoughness, dot(REFLECTVEC_FOR_SPECULAR(i, s), i.lightDir));

	#if SPECULAR_HIGHLIGHTS // else diffColor has premultiplied light color
		c *= _LightColor0.rgb;
	#endif

	c *= LIGHT_ATTENUATION(i) * LambertTerm(LightSpaceNormal(i, s), i.lightDir);
	
	UNITY_APPLY_FOG_COLOR(i.fogCoord, c.rgb, half4(0,0,0,0)); // fog towards black in additive pass

	half4 final = OutputForward (half4(c, 1), s.alpha);
	return final;
}

#endif

//----------------------------------------【vertexDeffered函数】-------------------------------------------
//  Deferred pass

struct VertexOutputDeferred
{
    float4 pos                          : SV_POSITION;
    float4 tex                          : TEXCOORD0;
    half3 eyeVec                        : TEXCOORD1;
    half4 tangentToWorldAndPackedData[3]: TEXCOORD2;    // [3x3:tangentToWorld | 1x3:viewDirForParallax]
    half4 ambientOrLightmapUV           : TEXCOORD5;    // SH or Lightmap UVs			
	#if UNITY_SPECCUBE_BOX_PROJECTION
		float3 posWorld						: TEXCOORD6;
	#endif
	#if UNITY_OPTIMIZE_TEXCUBELOD
		#if UNITY_SPECCUBE_BOX_PROJECTION
			half3 reflUVW				: TEXCOORD7;
		#else
			half3 reflUVW				: TEXCOORD6;
		#endif
	#endif

};
//------------------------------------------------------------------------------------------------------

//----------------------------------------【fragDeffered函数】-------------------------------------------
VertexOutputDeferred vertDeferred (VertexInput v)
{
    VertexOutputDeferred o;
    UNITY_INITIALIZE_OUTPUT(VertexOutputDeferred, o);

    float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
    #if UNITY_SPECCUBE_BOX_PROJECTION
        o.posWorld = posWorld;
    #endif
    o.pos = UnityObjectToClipPos(v.vertex);
    o.tex = TexCoords(v);
    o.eyeVec = NormalizePerVertexNormal(posWorld.xyz - _WorldSpaceCameraPos);
    float3 normalWorld = UnityObjectToWorldNormal(v.normal);
    #ifdef _TANGENT_TO_WORLD
        float4 tangentWorld = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);

        float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);
        o.tangentToWorldAndPackedData[0].xyz = tangentToWorld[0];
        o.tangentToWorldAndPackedData[1].xyz = tangentToWorld[1];
        o.tangentToWorldAndPackedData[2].xyz = tangentToWorld[2];
    #else
        o.tangentToWorldAndPackedData[0].xyz = 0;
        o.tangentToWorldAndPackedData[1].xyz = 0;
        o.tangentToWorldAndPackedData[2].xyz = normalWorld;
    #endif

    #ifdef LIGHTMAP_ON
        o.ambientOrLightmapUV.xy = v.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
        o.ambientOrLightmapUV.zw = 0;
    #elif UNITY_SHOULD_SAMPLE_SH
        #if (SHADER_TARGET < 30)
            o.ambientOrLightmapUV.rgb = ShadeSH9(half4(normalWorld, 1.0));
        #else
            // Optimization: L2 per-vertex, L0..L1 per-pixel
            o.ambientOrLightmapUV.rgb = ShadeSH3Order(half4(normalWorld, 1.0));
        #endif
    #endif
    #ifdef DYNAMICLIGHTMAP_ON
        o.ambientOrLightmapUV.zw = v.uv2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
    #endif

    #ifdef _PARALLAXMAP
        TANGENT_SPACE_ROTATION;
        half3 viewDirForParallax = mul (rotation, ObjSpaceViewDir(v.vertex));
        o.tangentToWorldAndPackedData[0].w = viewDirForParallax.x;
        o.tangentToWorldAndPackedData[1].w = viewDirForParallax.y;
        o.tangentToWorldAndPackedData[2].w = viewDirForParallax.z;
    #endif

    #if UNITY_OPTIMIZE_TEXCUBELOD
        o.reflUVW		= reflect(o.eyeVec, normalWorld);
    #endif

    return o;
}

void fragDeferred (
    VertexOutputDeferred i,
    out half4 outGBuffer0 : SV_Target0,
    out half4 outGBuffer1 : SV_Target1,
    out half4 outGBuffer2 : SV_Target2,
    out half4 outEmission : SV_Target3          // RT3: emission (rgb), --unused-- (a)
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    ,out half4 outShadowMask : SV_Target4       // RT4: shadowmask (rgba)
#endif
)
{
    #if (SHADER_TARGET < 30)
        outGBuffer0 = 1;
        outGBuffer1 = 1;
        outGBuffer2 = 0;
        outEmission = 0;
        #if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
            outShadowMask = 1;
        #endif
        return;
    #endif

    FRAGMENT_SETUP(s)

    // no analytic lights in this pass
    UnityLight dummyLight = DummyLight ();
    half atten = 1;

    // only GI
    half occlusion = Occlusion(i.tex.xy);
#if UNITY_ENABLE_REFLECTION_BUFFERS
    bool sampleReflectionsInDeferred = false;
#else
    bool sampleReflectionsInDeferred = true;
#endif

    UnityGI gi = FragmentGI (s, occlusion, i.ambientOrLightmapUV, atten, dummyLight, sampleReflectionsInDeferred);

	half3 color = UNITY_BRDF_PBS (s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, gi.light, gi.indirect).rgb;

	color += Emission (i.tex.xy, -s.eyeVec, s.normalWorld);

	#ifndef UNITY_HDR_ON
		color.rgb = exp2(-color.rgb);
	#endif

    UnityStandardData data;
    data.diffuseColor   = s.diffColor;
    data.occlusion      = occlusion;
    data.specularColor  = s.specColor;
    data.smoothness     = s.smoothness;
    data.normalWorld    = s.normalWorld;

    UnityStandardDataToGbuffer(data, outGBuffer0, outGBuffer1, outGBuffer2);
//-----------------------------------------------------------------------------------------------------------
	outEmission = half4(color, 1);
}
//------------------------------------------------------------------------------------------------------------

//
// Old FragmentGI signature. Kept only for backward compatibility and will be removed soon
//

inline UnityGI FragmentGI(
    float3 posWorld,
    half occlusion, half4 i_ambientOrLightmapUV, half atten, half smoothness, half3 normalWorld, half3 eyeVec,
    UnityLight light,
    bool reflections)
{
    // we init only fields actually used
    FragmentCommonData s = (FragmentCommonData)0;
    s.smoothness = smoothness;
    s.normalWorld = normalWorld;
    s.eyeVec = eyeVec;
    s.posWorld = posWorld;
#if UNITY_OPTIMIZE_TEXCUBELOD
	s.reflUVW = reflect(eyeVec, normalWorld);
#endif
    return FragmentGI(s, occlusion, i_ambientOrLightmapUV, atten, light, reflections);
}
inline UnityGI FragmentGI (
    float3 posWorld,
    half occlusion, half4 i_ambientOrLightmapUV, half atten, half smoothness, half3 normalWorld, half3 eyeVec,
    UnityLight light)
{
    return FragmentGI (posWorld, occlusion, i_ambientOrLightmapUV, atten, smoothness, normalWorld, eyeVec, light, true);
}

#endif // UNITY_STANDARD_CORE_INCLUDED
