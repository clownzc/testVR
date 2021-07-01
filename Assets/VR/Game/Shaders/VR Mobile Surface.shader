// Upgrade NOTE: replaced '_LightMatrix0' with 'unity_WorldToLight'
////////////////////////////////////////////////////////////////////
// 2017.8.4  VertexLighting should multiply diffuseColor c.rgb += diffuse * o.Albedo;
//
////////////////////////////////////////////////////////////////////
Shader "VR/Mobile Surface" {
Properties {
	//Switch
	 [Enum(Off,0,Front,1,Back,2)] _Cull ("Cull Mode", Float) = 2.0
	 [Enum(OFF,0,ON,1)] _ZWrite ("Zwrite Mode", Float) = 1.0

	//First Map
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) RefStrength (A)", 2D) = "white" {}
	//Second Map
	[HideInInspector] _MainTex2("Second UV", 2D) = "black" {}

	//Normal
	[Enum(OFF,0,ON,1)] _NormalMode ("Mormal Mode", Float) = 1.0
	_NormalInten("Normal Inten", Range(0,10)) = 1.0
	_BumpMap ("Normalmap", 2D) = "bump" {}

	//Metal
	[Enum(OFF,0,ON,1)] _MetalMode ("Metal Mode", Float) = 1.0
	_Mixmap("Mixmap", 2D) = "black" {}
	_Gloss("Gloss", Range(0,1)) = 1.0
	_Occlusion("Occlusion", Range(0,1)) = 1.0
	_Cube ("Reflection Cubemap", Cube) = "_Skybox" { }
	_ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
	_RefInten ("RefInten", Range(0,100)) = 1.0

	//Fresnel
	[Enum(OFF,0,ON,1)] _FresnelMode ("Fresnel Mode", Float) = 0
	[Enum(Back,0,Front,2)] _FresnelDirection("Fresnel Direction", Float) = 2
	[Enum(World,0,Normal,1)] _FresnelSource("Fresnel Source", Float) = 0
	_FresnelColor("Fresnel Color", Color) = (0,0,0)
	_FresnelAdd("Fresnel Add", Range(-10, 10)) = 0
	_FresnelMul("Fresnel Mul", Range(0, 10)) = 0
	_FresnelExp("Fresnel Exp", Range(0.01, 10)) = 0.01

	//Emission
	[Enum(OFF,0,ON,1)] _EmissionMode ("Emission Mode", Float) = 0
	[Enum(UV0,0,UV1,1)] _EmissionUV ("Emission UV", Float) = 0
	_EmissionMap("Emission", 2D) = "white" {}
	_EmissionColor("Emission Color", Color) = (0,0,0,0)
	_EmissionMul("Emission Mul", Range(0,40)) = 1

	//Emission Effect
	[Enum(OFF,0,FLOW,1,ANIM,2)] _EmissionEffectMode ("Emission Effect Mode", Float) = 0
	//Anim
	_EmissionAni("EmissionAin", Range(0, 2)) = 2
	//Flow
	_FlowMap("Flow Map", 2D) = "black" {}
	_FlowColor("Flow Color", Color) = (0,0,0,0)
	_FlowMul("Flow Mul", Range(0,11)) = 1
	_FlowSpeed("Flow Speed", Range(-1,1)) = 0.2

	//Light
	[Enum(OFF,0,VECTEX,1)] _LightMode("Vectex Light Mode", Float) = 0

	//Fog
	[Enum(UNITY FOG,0,DAYDREAM FOG,1,DAYDREAM HEIGHT FOG,2)] _FogMode("Fog Mode", Float) = 0

	//Transparency
	//_Transparency("Transparency", Range(0, 1)) = 1
}

SubShader {
	Tags { "Queue"="Geometry" "RenderType"="Opaque" "PerformanceChecks"="False"}
	Cull [_Cull]
	ZWrite [_ZWrite]
	LOD 300

	// ---- forward rendering base pass:
	Pass {
		Name "FORWARD"
		Tags { "LightMode" = "ForwardBase" }

	CGPROGRAM
	// compile directives
	#pragma target 3.0
	#pragma vertex vert_surf
	#pragma fragment frag_surf
	//#pragma multi_compile_instancing
	#pragma multi_compile_fog
	#pragma shader_feature _METAL
	#pragma shader_feature _EMISSION
	#pragma shader_feature _FRESNEL
	#pragma shader_feature _ _FLOW _ANIM
	#pragma shader_feature _VERTEX_LIGHT
	#pragma shader_feature _DAYDREAM_FOG 
	#pragma shader_feature _DAYDREAM_HEIGHT_FOG
	#pragma shader_feature _ _4_POINT_LIGHT _8_POINT_LIGHT _12_POINT_LIGHT _16_POINT_LIGHT
	#pragma multi_compile_fwdbase nodynlightmap
	#include "HLSLSupport.cginc"
	#include "UnityShaderVariables.cginc"
	#include "UnityStandardUtils.cginc"

	#define UNITY_PASS_FORWARDBASE
	#include "UnityCG.cginc"
	#include "Lighting.cginc"
	#include "AutoLight.cginc"

	#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;

	//TANGENT SPACE BASIS VECTORS:
	// (-1/sqrt(6), -1/sqrt(2), 1/sqrt(3))
	// (-1/sqrt(6),  1/sqrt(2), 1/sqrt(3))
	// (sqrt(2/3),   0        , 1/sqrt(3))
	static const float3 c_basis0 = float3(-0.40824829046386301636621401245098f, -0.70710678118654752440084436210485f, 0.57735026918962576450914878050195f);
	static const float3 c_basis1 = float3(-0.40824829046386301636621401245098f, 0.70710678118654752440084436210485f, 0.57735026918962576450914878050195f);
	static const float3 c_basis2 = float3(0.81649658092772603273242802490196f, 0.0f, 0.57735026918962576450914878050195f);

		sampler2D	_MainTex;
		fixed4		_Color;
		sampler2D	_MainTex2;

		sampler2D	_BumpMap;
		fixed		_NormalInten;

		sampler2D		_Mixmap;
		fixed		_Gloss;
		fixed		_Occlusion;
		samplerCUBE _Cube;
		fixed4		_ReflectColor;
		fixed		_RefInten;

		sampler2D	_EmissionMap;
		fixed		_EmissionUV;
		fixed4		_EmissionColor;
		fixed		_EmissionMul;

		half 		_FresnelDirection;
		half		_FresnelSource;
		half4 		_FresnelColor;
		half		_FresnelAdd;
		half		_FresnelMul;
		half		_FresnelExp;

		fixed		_EmissionAni;
		sampler2D	_FlowMap;
		fixed4		_FlowColor;
		fixed		_FlowMul;
		fixed		_FlowSpeed;

		//Fog
		half  _fogMode;
		//near, far, scale, colorScale
		half4 _fogLinear;
		//minHeight, maxHeight, thickness, density
		half4 _fogHeight;
		//colors
		//Fog NearColor
		half4 _fogNearColor;
		//Fog FarColor
		half4 _fogFarColor;

		//Light
		half4 _VectorLightWorldPosition[16];
		half4 _VectorLightAtten[16];//R:Range G:Intensity
		half4 _VectorLightColor[16];

		struct Input {
			float3 viewDir;
			float2 uv_MainTex;
			float2 uv2_MainTex2 : TEXCOORD1;
			float3 worldRefl;
			float3 worldNormal;
			float3 worldPos;
			INTERNAL_DATA
		};

		half4 computeAttenuation(half4 len2, half4 attenConst)
		{
			half cutoff = 0.01;	//attenuation value that is remapped to zero.
										//Calculate Attenuation
			half4 atten = 1.0 / (1.0 + half4(len2.x * attenConst.x, len2.y * attenConst.y, len2.z * attenConst.z, len2.w * attenConst.w));
			//Smooth cutoff so the light doesn't extent forever.
			atten = (atten - cutoff) / (1.0 - cutoff);
			return max(atten, 0.0);
		}

		////////////////////////////////////////////////////////
		//Distance Fog: 15 - 24 ALU ops
		//Height Fog: +14 ALU ops
		//Sphere Fog: +36 ALU ops
		//Total fog cost (approx) in ALU ops:
		//   Base 15 - 24
		//      +Height Fog 29 - 38 (if not using SphereFog - this may be 2 ops cheaper since dir.xy isn't needed, 27-36)
		//		+Sphere Fog 51 - 60
		//		+Height +Sphere 65 - 74
		////////////////////////////////////////////////////////
		// Conclusions: Height fog is a reasonable extra cost Sphere fog is most likely
		//				too expensive  in many cases (especially when using both height & sphere fog)
		////////////////////////////////////////////////////////
		// ComputeFog rgb is Fog Color a is Fog Mask
		////////////////////////////////////////////////////////
		half4 computeFog(half3 posWorld)
		{
			half4 fogValue;
			half3 relPos = posWorld.xyz - _WorldSpaceCameraPos.xyz;	//3
			//half  sceneDepth = length(relPos)/1024;						//10
			half  sceneDepth = length(relPos);
		
			//half dist = sceneDepth - _ProjectionParams.y;
			#if defined(_DAYDREAM_HEIGHT_FOG)
				half3 dir = relPos / sceneDepth;						//4 (or 2 if dir.xy is unused later)

				//Volume height
				half dH = _fogHeight.y; //dH = maxY - minY
				//Ray/Volume intersections (y0,y1) scaled to the (-inf, 1] range.
				//Note that the ray is clipped to the top of the volume but allowed to pass through the bottom.
				//[To clip to the volume bottom as well, change min(y, 1) to saturate(y) for both y0 and y1]

				half test = saturate((posWorld.y - _fogHeight.y) / (_fogHeight.x - _fogHeight.y));

				//half y0 = min(_WorldSpaceCameraPos.y - _fogHeight.x, dH);	//2
				//half y1 = min(posWorld.y - _fogHeight.x, dH);				//2
				//Magnitude of the difference in Y between intersections scaled by the fog "thickness"
				//half dh = abs(y0 - y1)*_fogHeight.z;						//3

				//compute the length of the ray passing through the volume bounding by [_fogHeight.x, _fogHeight.y] with a thickness of [_fogHeight.z]
				//the ray is clipped by the height bounds, this handles the camera being inside or outside of the volume
				//Note that the thickness artificially scales the length of the ray in order to simulate thicker height fog.
				//--------------------------------------------------------------------------------------------------------------------------------------
				//If the ray is not clipped by the volume, the camera is inside and the "thickness" is 1.0 - then this is equivalent to depth based fog. important abs(dir.y)
				
				//half test1 = lerp(1, abs(dir.y), 1);
				//half L = dh / test1;									//3

				//test + sceneDepth   0.01 is Correction factor

				half L = max(0.0, test * 3.5 + sceneDepth * 0.01 * _fogHeight.z);

				//half L = dh / abs(dir.y);
			#else
				//the camera is considered to be inside an infinite volume of fog so only the distance from the camera to the surface is important.
				half L = sceneDepth;
			#endif

			half Lw;
			//linear
			if (_fogMode == 0.0)
			{
				//_fogLinear  _fogLinear.x = 1.0f / (m_fogLinear.y - m_fogLinear.x)  _fogLinear.y = -m_fogLinear.x * 1.0f / (m_fogLinear.y - m_fogLinear.x)
				Lw = saturate(L*_fogLinear.x + _fogLinear.y);	//2
				fogValue.w = Lw;
			}
			//Exponential  _fogHeight.w is Density
			else if (_fogMode == 1.0)
			{
				Lw = L*_fogHeight.w;							//8
				fogValue.w = 1.0 - exp(-Lw);
			}
			//Exponential Squared	_fogHeight.w is Density
			else if (_fogMode == 2.0)
			{
				Lw = L*_fogHeight.w;							//9
				half e = exp(-Lw);
				fogValue.w = 1.0 - e*e;
			}

			//fog depth color _fog
			half colorFactor = smoothstep(0.0, 1.0, Lw * _fogLinear.w);
			fogValue.rgb = lerp(_fogNearColor.rgb, _fogFarColor.rgb, colorFactor);

			//fogValue.rgb = float3(colorFactor, colorFactor, colorFactor);
			//overall fog opacity _fogLinear.z
			fogValue.w *= _fogLinear.z;

			return fogValue;
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
				_VectorLightColor[index + 3].r, _VectorLightColor[index + 3].g, _VectorLightColor[index + 3].b, _VectorLightAtten[index + 3].g};
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

		void surf (Input IN, inout SurfaceOutput o) {
			
		}

	struct v2f_surf {
	  float4 pos : SV_POSITION;
	  float4 pack0 : TEXCOORD0; // _MainTex _MainTex2
	  //TBN Matrix WorldPos
	  float4 tSpace0 : TEXCOORD1;
	  float4 tSpace1 : TEXCOORD2;
	  float4 tSpace2 : TEXCOORD3;
	  #ifdef LIGHTMAP_ON
	  float4 lmap : TEXCOORD4;
	  #else
	  half3 sh : TEXCOORD4; //SH
	  #endif
	  #if !defined(_DAYDREAM_FOG) && !defined(_FOG_HEIGHT)
	  UNITY_FOG_COORDS(5)
      #endif
	  #if _VERTEX_LIGHT
	  half3 color0 : TEXCOORD6;
	  half3 color1 : TEXCOORD7;
	  half3 color2 : TEXCOORD8;
	  #endif
	  UNITY_SHADOW_COORDS(9)
	  UNITY_VERTEX_INPUT_INSTANCE_ID
	  UNITY_VERTEX_OUTPUT_STEREO
	};
	float4 _MainTex_ST;
	float4 _MainTex2_ST;

	// vertex shader
	v2f_surf vert_surf (appdata_full v) {
	  UNITY_SETUP_INSTANCE_ID(v);
	  v2f_surf o;
	  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
	  UNITY_TRANSFER_INSTANCE_ID(v,o);
	  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
	  o.pos = UnityObjectToClipPos(v.vertex);
	  o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
	  o.pack0.zw = TRANSFORM_TEX(v.texcoord1, _MainTex2);

	  //World Pos
	  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
	  //TBN Matrix
	  fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
	  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
	  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
	  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;

	  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
	  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
	  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);

	  #ifdef LIGHTMAP_ON
	  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
	  #endif

	  // SH/ambient and vertex lights
	  #ifndef LIGHTMAP_ON
		#if UNITY_SHOULD_SAMPLE_SH
		  o.sh = 0;
		  // Approximated illumination from non-important point lights
		  #ifdef VERTEXLIGHT_ON
			o.sh += Shade4PointLights (
			  unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
			  unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
			  unity_4LightAtten0, worldPos, worldNormal);
		  #endif
		  o.sh = ShadeSHPerVertex (worldNormal, o.sh);
		#endif
	  #endif

	  UNITY_TRANSFER_SHADOW(o,v.texcoord1.xy);

	  #if !defined(_DAYDREAM_FOG) && !defined(_FOG_HEIGHT)
		UNITY_TRANSFER_FOG(o,o.pos);
	  #endif
	  #if _VERTEX_LIGHT
		computeVectorLight(v.vertex, v.normal, v.tangent, o.color0, o.color1, o.color2);
	  #endif
	  return o;
	}

	// fragment shader
	fixed4 frag_surf (v2f_surf IN) : SV_Target {
	  UNITY_SETUP_INSTANCE_ID(IN);
	  // prepare and unpack data
	  Input surfIN;
	  UNITY_INITIALIZE_OUTPUT(Input,surfIN);

	  surfIN.uv_MainTex = IN.pack0.xy;
	  surfIN.uv2_MainTex2 = IN.pack0.zw;
	  float3 worldPos = float3(IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w);
	  #ifndef USING_DIRECTIONAL_LIGHT
		fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
	  #else
		fixed3 lightDir = _WorldSpaceLightPos0.xyz;
	  #endif
	  surfIN.worldNormal = 0.0;
	  surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
	  surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
	  surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
	  surfIN.worldPos = worldPos;
	  #ifdef UNITY_COMPILER_HLSL
	  SurfaceOutput o = (SurfaceOutput)0;
	  #else
	  SurfaceOutput o;
	  #endif
	  o.Albedo = 0.0;
	  o.Emission = 0.0;
	  o.Specular = 0.0;
	  o.Alpha = 0.0;
	  o.Gloss = 0.0;

			fixed3 worldnormal = fixed3(IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z);

			fixed4 tex = tex2D(_MainTex, IN.pack0.xy);
			fixed4 a = tex * _Color;
			fixed3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - worldPos.xyz);

			o.Normal = UnpackScaleNormal(tex2D(_BumpMap, IN.pack0.xy), _NormalInten);

			#if defined(_METAL)
			fixed3 mix = tex2D(_Mixmap, IN.pack0.xy);
			a -= tex * mix.r * _Color;
			fixed mip = (1 - mix.b) * 8;
			mip = mip * _Gloss;

			fixed3 viewReflectDirection = reflect(-viewDirection, (worldnormal));
			fixed4 reflcol = texCUBElod(_Cube, fixed4(viewReflectDirection, mip));
			fixed occ = LerpOneTo (mix.g, _Occlusion);
			o.Emission += reflcol.rgb * _ReflectColor.rgb * mix.r * _RefInten * tex * occ;
			#endif
	
			o.Albedo = a.rgb;

			fixed3 worldN;
			half3 tsNormal = normalize(o.Normal);
			worldN.x = dot(IN.tSpace0.xyz, o.Normal);
			worldN.y = dot(IN.tSpace1.xyz, o.Normal);
			worldN.z = dot(IN.tSpace2.xyz, o.Normal);
			o.Normal = worldN;

			float3 fresnel = 0;
			float4 emission = 0;
			fixed emissani = 1;

			#if defined(_FRESNEL)
			fresnel = (1 - max(0, dot(lerp(worldnormal, o.Normal, _FresnelSource), (_FresnelDirection - 1) * viewDirection)) + _FresnelAdd) * _FresnelColor.rgb;
			fresnel = pow(fresnel, _FresnelExp) * _FresnelMul;
			#endif

			#if defined(_EMISSION)
			emission = tex2D(_EmissionMap, lerp(IN.pack0.xy, IN.pack0.zw, _EmissionUV));
			emission.rgb = _EmissionMul * emission.rgb * _EmissionColor.rgb;
			#endif

			#if defined(_ANIM) && defined(_EMISSION)
			emissani = saturate((emission.a - (1 - _EmissionAni)) * 5);
			#endif

			o.Emission += (emission.rgb + fresnel) * emissani ;

			#if defined(_FLOW)
			fixed4 uv2 = tex2D(_FlowMap, IN.pack0.zw + fixed2(0,_Time.g * _FlowSpeed));
			o.Emission += uv2 * _FlowMul * _FlowColor.rgb;
			#endif

	  // compute lighting & shadowing factor
	  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
	  fixed4 c = 0;

	  // Setup lighting environment
	  UnityGI gi;
	  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
	  gi.indirect.diffuse = 0;
	  gi.indirect.specular = 0;
	  gi.light.color = _LightColor0.rgb;
	  gi.light.dir = lightDir;
	  // Call GI (lightmaps/SH/reflections) lighting function
	  UnityGIInput giInput;
	  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
	  giInput.light = gi.light;
	  giInput.worldPos = worldPos;
	  giInput.atten = atten;
	  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
		giInput.lightmapUV = IN.lmap;
	  #else
		giInput.lightmapUV = 0.0;
	  #endif
	  #if UNITY_SHOULD_SAMPLE_SH
		giInput.ambient = IN.sh;
	  #else
		giInput.ambient.rgb = 0.0;
	  #endif
	  giInput.probeHDR[0] = unity_SpecCube0_HDR;
	  giInput.probeHDR[1] = unity_SpecCube1_HDR;
	  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
		giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .wholds lerp value for blending
	  #endif
	  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
		giInput.boxMax[0] = unity_SpecCube0_BoxMax;
		giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
		giInput.boxMax[1] = unity_SpecCube1_BoxMax;
		giInput.boxMin[1] = unity_SpecCube1_BoxMin;
		giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
	  #endif
	  LightingLambert_GI(o, giInput, gi);

	  // realtime lighting: call lighting function

	  c += LightingLambert (o, gi);

	  #if _VERTEX_LIGHT
		half3 diffuse = IN.color0 * dot(c_basis0, tsNormal);
		diffuse += IN.color1 * dot(c_basis1, tsNormal);
		diffuse += IN.color2 * dot(c_basis2, tsNormal);
		c.rgb += diffuse * o.Albedo;
	  #endif

	  c.rgb += o.Emission;
	  #if defined(_DAYDREAM_FOG) || defined(_FOG_HEIGHT)
		half4 fogValue = computeFog(worldPos);
		c.rgb = lerp(c.rgb, fogValue.rgb, saturate(fogValue.w));
		c.a = 1.0 - (1.0 - c.a) * max(1.0 - fogValue.w, 0.0);
	  #else
		UNITY_APPLY_FOG(IN.fogCoord, c);
	  #endif 
	  UNITY_OPAQUE_ALPHA(c.a);
	  return c;
	}

	ENDCG
	}
	// ---- forward rendering additive lights pass:
	Pass {
		Name "FORWARD"
		Tags { "LightMode" = "ForwardAdd" }
		ZWrite Off Blend One One

CGPROGRAM
// compile directives
#pragma vertex vert_surf
#pragma fragment frag_surf
#pragma multi_compile_instancing
#pragma skip_variants INSTANCING_ON
#pragma multi_compile_fog
#pragma shader_feature _METAL
#pragma shader_feature _EMISSION
#pragma shader_feature _FRESNEL
#pragma shader_feature _ _FLOW _ANIM
#pragma multi_compile_fwdadd_fullshadows nodynlightmap 
#include "HLSLSupport.cginc"
#include "UnityShaderVariables.cginc"
#include "UnityStandardUtils.cginc"

#define UNITY_PASS_FORWARDADD
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))


	sampler2D	_MainTex;
	fixed4		_Color;
	sampler2D	_MainTex2;

	sampler2D	_BumpMap;
	fixed		_NormalInten;

	sampler2D	_Mixmap;
	fixed		_Gloss;
	fixed		_Occlusion;
	samplerCUBE _Cube;
	fixed4		_ReflectColor;
	fixed		_RefInten;

	sampler2D	_EmissionMap;
	fixed		_EmissionUV;
	fixed4		_EmissionColor;
	fixed		_EmissionMul;

	half 		_FresnelDirection;
	half4 		_FresnelColor;
	half		_FresnelAdd;
	half		_FresnelMul;
	half		_FresnelExp;

	fixed		_EmissionAni;
	sampler2D	_FlowMap;
	fixed4		_FlowColor;
	fixed		_FlowMul;
	fixed		_FlowSpeed;

	struct Input {
		float3 viewDir;
		float2 uv_MainTex;
		float2 uv2_MainTex2 : TEXCOORD1;
		float3 worldRefl;
		float3 worldNormal;
		float3 worldPos;
		INTERNAL_DATA
	};

	void surf (Input IN, inout SurfaceOutput o) {
		fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
		fixed4 c = tex * _Color;
		fixed3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - IN.worldPos.xyz);

		o.Normal = UnpackScaleNormal(tex2D(_BumpMap, IN.uv_MainTex), _NormalInten);

		#if defined(_METAL)
		fixed3 mix = tex2D(_Mixmap, IN.uv_MainTex);
		c -= tex * mix.r * _Color;
		fixed mip = (1 - mix.b) * 8;
		mip = mip * _Gloss;
//		fixed3 worldRefl = WorldReflectionVector (IN, o.Normal);
		fixed3 worldnormal = WorldNormalVector(IN, o.Normal);
		fixed3 viewReflectDirection = reflect(-viewDirection, (worldnormal));
		fixed4 reflcol = texCUBElod(_Cube, fixed4(viewReflectDirection, mip));
		fixed occ = LerpOneTo (mix.g, _Occlusion);
		o.Emission += reflcol.rgb * _ReflectColor.rgb * mix.r * _RefInten * tex * occ;
		#endif
	
		o.Albedo = c.rgb;

		#if defined(_FRESNEL)
		float3 emission = (1 - max(0, dot(WorldNormalVector (IN, o.Normal), (_FresnelDirection - 1) * viewDirection)) + _FresnelAdd) * _FresnelColor.rgb;
		emission = pow(emission, _FresnelExp) * _FresnelMul;
		o.Emission += emission;
		#endif

		#if defined(_EMISSION)
		float4 emiss = tex2D(_EmissionMap, lerp(IN.uv_MainTex, IN.uv2_MainTex2, _EmissionUV));
		#if defined(_ANIM)
		fixed emissani = saturate((emiss.a - (1 -_EmissionAni)) * 5);
		o.Emission += _EmissionMul * emiss.rgb * _EmissionColor.rgb * emissani;
		#else
		o.Emission += _EmissionMul * emiss.rgb * _EmissionColor.rgb;
		#endif
		#endif

		#if defined(_FLOW)
		fixed4 uv2 = tex2D(_FlowMap, IN.uv2_MainTex2 + fixed2(0,_Time.g * _FlowSpeed));
		o.Emission += uv2 * _FlowMul * _FlowColor.rgb;
		#endif
	}
	

// vertex-to-fragment interpolation data
	struct v2f_surf {
		float4 pos : SV_POSITION;
		float2 pack0 : TEXCOORD0; // _MainTex
		fixed3 tSpace0 : TEXCOORD1;
		fixed3 tSpace1 : TEXCOORD2;
		fixed3 tSpace2 : TEXCOORD3;
		float3 worldPos : TEXCOORD4;
		UNITY_SHADOW_COORDS(5)
		#if !defined(_DAYDREAM_FOG) && !defined(_FOG_HEIGHT)
		UNITY_FOG_COORDS(6)
		#endif
		UNITY_VERTEX_INPUT_INSTANCE_ID
		UNITY_VERTEX_OUTPUT_STEREO
	};
	float4 _MainTex_ST;

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  o.pos = UnityObjectToClipPos(v.vertex);
  o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  o.tSpace0 = fixed3(worldTangent.x, worldBinormal.x, worldNormal.x);
  o.tSpace1 = fixed3(worldTangent.y, worldBinormal.y, worldNormal.y);
  o.tSpace2 = fixed3(worldTangent.z, worldBinormal.z, worldNormal.z);
  o.worldPos = worldPos;
  #if !defined(_DAYDREAM_FOG) && !defined(_FOG_HEIGHT)
    UNITY_TRANSFER_FOG(o,o.pos); 
  #endif

  UNITY_TRANSFER_SHADOW(o,v.texcoord1.xy);
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  // prepare and unpack data
  Input surfIN;
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.viewDir.x = 1.0;
  surfIN.uv_MainTex.x = 1.0;
  surfIN.uv2_MainTex2.x = 1.0;
  surfIN.worldRefl.x = 1.0;
  surfIN.worldNormal.x = 1.0;
  surfIN.worldPos.x = 1.0;
  surfIN.uv_MainTex = IN.pack0.xy;
  float3 worldPos = IN.worldPos;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;
  fixed3 worldN;
  worldN.x = dot(IN.tSpace0.xyz, o.Normal);
  worldN.y = dot(IN.tSpace1.xyz, o.Normal);
  worldN.z = dot(IN.tSpace2.xyz, o.Normal);
  o.Normal = worldN;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  gi.light.color *= atten;
  c += LightingLambert (o, gi);
  c.a = 0.0;
  UNITY_OPAQUE_ALPHA(c.a);
  #if !defined(_DAYDREAM_FOG) && !defined(_FOG_HEIGHT)
    UNITY_APPLY_FOG(IN.fogCoord, c);
  #endif
  return c;
}

ENDCG

}

}
Fallback "Diffuse"
CustomEditor "VRSurfaceShaderGUI"
}
