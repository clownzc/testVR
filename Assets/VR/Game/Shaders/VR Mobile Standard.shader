Shader "VR/Mobile Standard"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}

		_Transparency("Transparency", Range(0, 1)) = 1
		
		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		[Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
		_MetallicGlossMap("Metallic", 2D) = "white" {}

		_BumpScale("Scale", Float) = 1.0
		_BumpMap("Normal Map", 2D) = "bump" {}

		_Parallax ("Height Scale", Range (0.005, 0.08)) = 0.02
		_ParallaxMap ("Height Map", 2D) = "black" {}

		_OcclusionStrength("Occlusion Strength", Range(0.0, 1.0)) = 1.0
		_OcclusionMap("Occlusion", 2D) = "white" {}

		_EmissionColor("Color", Color) = (0,0,0)
		_EmissionMap("Emission", 2D) = "white" {}
		_EmissionMul("Emission Mul", Range(0, 10)) = 1
		_EmissionMask("Emission Mask", Range(0, 1)) = 0
		_EmissionMaskMin("Emission Mask Min", Range(-1, 1)) = 0
		_EmissionMaskMax("Emission Mask Max", Range(0, 2)) = 2
		_EmissionMaskMul("Emission Mask Mul", Range(0, 50)) = 3

		[Enum(Point 0, 0, Point 1, 1, Point 2, 2, Point 3, 3,Point 4, 4)] _EmissionAddIndex("Emission Add Index", Float) = 0
		_EmissionAddMul("Emission Add Mul", Range(0, 10)) = 1

		_FresnelColor("FresnelColor", Color) = (0,0,0)
		_FresnelMap("Fresnel", 2D) = "white" {}
		_FresnelAdd("Fresnel Add", Range(-2, 2)) = 0
		_FresnelMul("Fresnel Mul", Range(0, 10)) = 1
		_FresnelExp("Fresnel Exp", Range(0.01, 10)) = 1
		
		_DetailMask("Detail Mask", 2D) = "white" {}

		_DetailAlbedoMap("Detail Albedo x2", 2D) = "grey" {}
		_DetailNormalMapScale("Scale", Float) = 1.0
		_DetailNormalMap("Normal Map", 2D) = "bump" {}

		[Enum(UV0,0,UV1,1)] _UVSec ("UV Set for secondary textures", Float) = 0


		// Blending state
		[HideInInspector] _Mode ("__mode", Float) = 0.0
		[HideInInspector] _SrcBlend ("__src", Float) = 1.0
		[HideInInspector] _DstBlend ("__dst", Float) = 0.0
		[HideInInspector] _ZWrite ("__zw", Float) = 1.0
		[Enum(Off,0,Front,1,Back,2)] _CullMode ("Cull Mode", Float) = 2.0
		[Enum(OFF,0,ON,1)] _FogMode ("Fog Mode", Float) = 1.0

		[HideInInspector] _EmissionMode ("__emissionMode", Float) = 0.0
		
		_AlbedoStrength("Albedo Strength", Range(0,10)) = 1.0
		_MetallicStrength("Metallic Strength", Range(0.0, 1.0)) = 1.0
		_GlossStrength("Gloss Strength", Range(0.0, 1.0)) = 1.0
		_GlossMin("Gloss Min", Range(0.0, 1.0)) = 0.0
		_GlossMax("Gloss Max", Range(0.0, 1.0)) = 1.0
		
		//unity_Lut("Unity Lut Map", 2D) = "white" {}

		[Enum(OFF,0,ON,1)] _OutLineSwitch ("OutLine Switch", Float) = 0
		[SerializeField] _OutLineFactor ("OutLine Factor(0-1)", Range(0, 1)) = 0.5
		[SerializeField] _OutLineWidth ("OutLine Width(0-1)", Range(0, 0.1)) = 0.5
		[SerializeField] _OutLineAdd ("OutLine Add", Range(-10, 10)) = 0
		[SerializeField] _OutLineMul ("OutLine Mul(0-", Range(0, 10)) = 1
		[SerializeField] _OutLineExp ("OutLine Exp(0", Range(0.01, 10)) = 1
        [SerializeField] _OutLineColor ("Outline Color(RGB)", Color) = (1,1,1,1)

		[Enum(OFF,0,ON,1)] _FresnelAlphaMode ("FresnelAlphaMode", Float) = 0.0
		_FresnelAlphaAdd("Fresnel Alpha Add", Range(-2, 2)) = 0
		_FresnelAlphaMul("Fresnel Alpha Mul", Range(0, 10)) = 1
		_FresnelAlphaExp("Fresnel Alpha Exp", Range(0.01, 10)) = 1

		//Light
		[Enum(OFF,0,VECTEX,1)] _LightMode("Vectex Light Mode", Float) = 0
	}

	CGINCLUDE
		#define UNITY_SETUP_BRDF_INPUT MetallicSetup
	ENDCG

	SubShader
	{
		Tags { "Queue"="Geometry+1" "RenderType"="Opaque" "PerformanceChecks"="False" }
		LOD 300
	
		//Pass {
  //          Name "Outline"
		//	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		//	Blend One OneMinusSrcAlpha 
  //          Cull Front  
		//	ZWrite Off
  //          Offset -5,-2


  //          CGPROGRAM
  //          #pragma vertex vert
  //          #pragma fragment frag
  //          #include "UnityCG.cginc"
  //          #pragma target 3.0
  //          uniform half4 _OutLineColor;
		//	uniform half _OutLineFactor;
  //          uniform half _OutLineWidth;
		//	uniform half _OutLineAdd;
		//	uniform half _OutLineMul;
		//	uniform half _OutLineExp;
		//	uniform half _OutLineSwitch;
  //          struct VertexInput {
  //              half4 vertex : POSITION;
  //              half4 normal : NORMAL;
  //          };
  //          struct VertexOutput {
  //              half4 pos : SV_POSITION;
  //              float4 posWorld : TEXCOORD1;
  //              float3 normalDir : TEXCOORD2;
  //          };
  //          VertexOutput vert (VertexInput v) {
		//		VertexOutput o = (VertexOutput)0;
  //              o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		//		half3 dir = normalize(v.vertex.xyz);
		//		half3 dir2 = v.normal;
		//		dir = lerp(dir,dir2,_OutLineFactor);
		//		dir = mul((half3x3)UNITY_MATRIX_IT_MV, dir);
		//		half2 offset = TransformViewToProjection(dir.xy);
		//		offset = normalize(offset);
		//		o.pos.xy += offset * o.pos.z * _OutLineWidth * _OutLineSwitch;

  //              o.normalDir = UnityObjectToWorldNormal(-v.normal);
  //              o.posWorld = mul(_Object2World, o.pos);
  //              return o;
  //          }
  //          float4 frag(VertexOutput i) : COLOR {
		//		i.normalDir = normalize(i.normalDir);
  //              float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
  //              float3 normalDirection = i.normalDir;

		//		half4 c = (1 - saturate(dot(-viewDirection, normalDirection) + _OutLineAdd)) * _OutLineColor.rgba;
		//		c = pow(c, _OutLineExp) * _OutLineMul;
  //              return c * _OutLineSwitch;
  //          }
  //          ENDCG
  //      }

		//Pass
		//{
		//	Name "ZWrite" 
		//	ZWrite On
		//	ColorMask 0
		//	CGPROGRAM
		//	#pragma target 3.0
		//	#pragma vertex vert
  //          #pragma fragment frag
		//	struct VertexInput {
  //              half4 vertex : POSITION;
  //          };
  //          struct VertexOutput {
  //              half4 pos : SV_POSITION;
  //          };
  //          VertexOutput vert (VertexInput v) {
		//		VertexOutput o = (VertexOutput)0;
  //              o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
  //              return o;
  //          }
  //          half4 frag(VertexOutput i) : COLOR {
  //              return half4(0,0,0,0);
  //          }
		//	ENDCG
		//}

		// ------------------------------------------------------------------
		//  Base forward pass (directional light, emission, lightmaps, ...)
		Pass
		{
			Name "FORWARD" 
			Tags { "LightMode" = "ForwardBase" }
			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]
			Cull [_CullMode]

			CGPROGRAM
			#pragma target 3.0
			// TEMPORARY: GLES2.0 temporarily disabled to prevent errors spam on devices without textureCubeLodEXT
			#pragma exclude_renderers gles
			
			// -------------------------------------
					
			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _EMISSION
			#pragma shader_feature _ _EMISSION_MASK _EMISSION_ADD
			#pragma shader_feature _FRESNEL
			#pragma shader_feature _FRESNEL_ALPHA
			#pragma shader_feature _METALLICGLOSSMAP 
			#pragma shader_feature ___ _DETAIL_MULX2
			#pragma shader_feature _PARALLAXMAP
			#pragma shader_feature _FOG
			#pragma shader_feature _VERTEX_LIGHT
			#pragma shader_feature _ _4_POINT_LIGHT _8_POINT_LIGHT _12_POINT_LIGHT _16_POINT_LIGHT
			
			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog
				
			#pragma vertex vertForwardBase
			#pragma fragment fragForwardBase

			#include "UnityStandardCore.cginc"

			ENDCG
		}
		// ------------------------------------------------------------------
		//  Additive forward pass (one light per pass)
//		Pass
//		{
//			Name "FORWARD_DELTA"
//			Tags { "LightMode" = "ForwardAdd" }
//			Blend [_SrcBlend] One
//			Fog { Color (0,0,0,0) } // in additive pass fog should be black
//			ZWrite Off
//			ZTest LEqual
//
//			CGPROGRAM
//			#pragma target 3.0
//			// GLES2.0 temporarily disabled to prevent errors spam on devices without textureCubeLodEXT
//			#pragma exclude_renderers gles
//
//			// -------------------------------------
//
//			#pragma shader_feature _FRESNEL_ALPHA
//			#pragma shader_feature _NORMALMAP
//			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
//			#pragma shader_feature _METALLICGLOSSMAP
//			#pragma shader_feature ___ _DETAIL_MULX2
//			#pragma shader_feature _PARALLAXMAP
//			
//			#pragma multi_compile_fwdadd_fullshadows
//			#pragma multi_compile_fog
//			
//			#pragma vertex vertForwardAdd
//			#pragma fragment fragForwardAdd
//
//			#include "UnityStandardCore.cginc"
//
//			ENDCG
//		}
		// ------------------------------------------------------------------
		//  Shadow rendering pass
//		Pass {
//			Name "ShadowCaster"
//			Tags { "LightMode" = "ShadowCaster" }
//			
//			ZWrite On ZTest LEqual
//
//			CGPROGRAM
//			#pragma target 3.0
//			// TEMPORARY: GLES2.0 temporarily disabled to prevent errors spam on devices without textureCubeLodEXT
//			#pragma exclude_renderers gles
//			
//			// -------------------------------------
//
//
//			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
//			#pragma multi_compile_shadowcaster
//
//			#pragma vertex vertShadowCaster
//			#pragma fragment fragShadowCaster
//
//			#include "UnityStandardShadow.cginc"
//
//			ENDCG
//		}
		// ------------------------------------------------------------------
		//  Deferred pass
//		Pass
//		{
//			Name "DEFERRED"
//			Tags { "LightMode" = "Deferred" }
//
//			CGPROGRAM
//			#pragma target 3.0
//			// TEMPORARY: GLES2.0 temporarily disabled to prevent errors spam on devices without textureCubeLodEXT
//			#pragma exclude_renderers nomrt gles
//			
//
//			// -------------------------------------
//
//			#pragma shader_feature _NORMALMAP
//			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
//			#pragma shader_feature _EMISSION
//			#pragma shader_feature _FRESNEL
//			#pragma shader_feature _METALLICGLOSSMAP
//			#pragma shader_feature ___ _DETAIL_MULX2
//			#pragma shader_feature _PARALLAXMAP
//
//			#pragma multi_compile ___ UNITY_HDR_ON
//			#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
//			#pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
//			#pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
//			
//			#pragma vertex vertDeferred
//			#pragma fragment fragDeferred
//
//			#include "UnityStandardCore.cginc"
//
//			ENDCG
//		}

		// ------------------------------------------------------------------
		// Extracts information for lightmapping, GI (emission, albedo, ...)
		// This pass it not used during regular rendering.
		//Pass
		//{
		//	Name "META" 
		//	Tags { "LightMode"="Meta" }

		//	Cull Off

		//	CGPROGRAM
		//	#pragma vertex vert_meta
		//	#pragma fragment frag_meta

		//	#pragma shader_feature _EMISSION
		//	#pragma shader_feature _FRESNEL
		//	#pragma shader_feature _FRESNEL_ALPHA
		//	#pragma shader_feature _METALLICGLOSSMAP
		//	#pragma shader_feature ___ _DETAIL_MULX2

		//	#include "UnityStandardMeta.cginc"
		//	ENDCG
		//}
	}

	SubShader
	{
		Tags { "Queue"="Geometry+1" "RenderType"="Opaque" "PerformanceChecks"="False" }
		LOD 150

		//Pass {
  //          Name "Outline"
		//	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		//	Blend One OneMinusSrcAlpha 
  //          Cull Front  
		//	ZWrite Off
  //          Offset -5,-2


  //          CGPROGRAM
  //          #pragma vertex vert
  //          #pragma fragment frag
  //          #include "UnityCG.cginc"
  //          #pragma target 2.0
  //          uniform half4 _OutLineColor;
		//	uniform half _OutLineFactor;
  //          uniform half _OutLineWidth;
		//	uniform half _OutLineExp;
		//	uniform half _OutLinePower;
		//	uniform half _OutLineSwitch;
  //          struct VertexInput {
  //              half4 vertex : POSITION;
  //              half4 normal : NORMAL;
  //          };
  //          struct VertexOutput {
  //              half4 pos : SV_POSITION;
  //              float4 posWorld : TEXCOORD1;
  //              float3 normalDir : TEXCOORD2;
  //          };
  //          VertexOutput vert (VertexInput v) {
		//		VertexOutput o = (VertexOutput)0;
  //              o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		//		half3 dir = normalize(v.vertex.xyz);
		//		half3 dir2 = v.normal;
		//		half D  = dot(dir, dir2);
		//		D = (D / _OutLineFactor + 1) / (1 + 1/_OutLineFactor);
		//		dir = lerp(dir,dir2,D);
		//		dir = mul((half3x3)UNITY_MATRIX_IT_MV, dir);
		//		half2 offset = TransformViewToProjection(dir.xy);
		//		offset = normalize(offset);
		//		o.pos.xy += offset * o.pos.z * _OutLineWidth * _OutLineSwitch;

  //              o.normalDir = UnityObjectToWorldNormal(-v.normal);
  //              o.posWorld = mul(_Object2World, o.pos);
  //              return o;
  //          }
  //          float4 frag(VertexOutput i) : COLOR {
		//		i.normalDir = normalize(i.normalDir);
  //              float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
  //              float3 normalDirection = i.normalDir;

		//		half4 c = max(0,dot(normalDirection, viewDirection)) * _OutLineColor.rgba;
		//		c = pow(c, _OutLinePower) * _OutLineExp;
  //              return c * _OutLineSwitch;
  //          }
  //          ENDCG
  //      }

		//Pass
		//{
		//	Name "ZWrite" 
		//	ZWrite On
		//	ColorMask 0
		//	CGPROGRAM
		//	#pragma target 3.0
		//	#pragma vertex vert
  //          #pragma fragment frag
		//	struct VertexInput {
  //              half4 vertex : POSITION;
  //          };
  //          struct VertexOutput {
  //              half4 pos : SV_POSITION;
  //          };
  //          VertexOutput vert (VertexInput v) {
		//		VertexOutput o = (VertexOutput)0;
  //              o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
  //              return o;
  //          }
  //          half4 frag(VertexOutput i) : COLOR {
  //              return half4(0,0,0,0);
  //          }
		//	ENDCG
		//}

		// ------------------------------------------------------------------
		//  Base forward pass (directional light, emission, lightmaps, ...)
		Pass
		{
			Name "FORWARD" 
			Tags { "LightMode" = "ForwardBase" }

			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]
			Cull [_CullMode]

			CGPROGRAM
			#pragma target 2.0
			
			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _EMISSION
			#pragma shader_feature _ _EMISSION_MASK _EMISSION_ADD
			#pragma shader_feature _FRESNEL
			#pragma shader_feature _FRESNEL_ALPHA
			#pragma shader_feature _METALLICGLOSSMAP 
			#pragma shader_feature ___ _DETAIL_MULX2
			// SM2.0: NOT SUPPORTED shader_feature _PARALLAXMAP
			#pragma shader_feature _FOG
			#pragma shader_feature _VERTEX_LIGHT
			#pragma shader_feature _ _4_POINT_LIGHT _8_POINT_LIGHT _12_POINT_LIGHT _16_POINT_LIGHT

			#pragma skip_variants SHADOWS_SOFT DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE

			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog
	
			#pragma vertex vertForwardBase
			#pragma fragment fragForwardBase

			#include "UnityStandardCore.cginc"

			ENDCG
		}
		// ------------------------------------------------------------------
		//  Additive forward pass (one light per pass)
//		Pass
//		{
//			Name "FORWARD_DELTA"
//			Tags { "LightMode" = "ForwardAdd" }
//			Blend [_SrcBlend] One
//			Fog { Color (0,0,0,0) } // in additive pass fog should be black
//			ZWrite Off
//			ZTest LEqual
//			
//			CGPROGRAM
//			#pragma target 2.0
//
//			#pragma shader_feature _NORMALMAP
//			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
//			#pragma shader_feature _METALLICGLOSSMAP
//			#pragma shader_feature ___ _DETAIL_MULX2
//			// SM2.0: NOT SUPPORTED shader_feature _PARALLAXMAP
//			#pragma skip_variants SHADOWS_SOFT
//			
//			#pragma multi_compile_fwdadd_fullshadows
//			#pragma multi_compile_fog
//			
//			#pragma vertex vertForwardAdd
//			#pragma fragment fragForwardAdd
//
//			#include "UnityStandardCore.cginc"
//
//			ENDCG
//		}
		// ------------------------------------------------------------------
		//  Shadow rendering pass
//		Pass {
//			Name "ShadowCaster"
//			Tags { "LightMode" = "ShadowCaster" }
//			
//			ZWrite On ZTest LEqual
//
//			CGPROGRAM
//			#pragma target 2.0
//
//			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
//			#pragma skip_variants SHADOWS_SOFT
//			#pragma multi_compile_shadowcaster
//
//			#pragma vertex vertShadowCaster
//			#pragma fragment fragShadowCaster
//
//			#include "UnityStandardShadow.cginc"
//
//			ENDCG
//		}

		// ------------------------------------------------------------------
		// Extracts information for lightmapping, GI (emission, albedo, ...)
		// This pass it not used during regular rendering.
		//Pass
		//{
		//	Name "META" 
		//	Tags { "LightMode"="Meta" }

		//	Cull Off

		//	CGPROGRAM
		//	#pragma vertex vert_meta
		//	#pragma fragment frag_meta

		//	#pragma shader_feature _EMISSION
		//	#pragma shader_feature _FRESNEL
		//	#pragma shader_feature _FRESNEL_ALPHA
		//	#pragma shader_feature _METALLICGLOSSMAP
		//	#pragma shader_feature ___ _DETAIL_MULX2

		//	#include "UnityStandardMeta.cginc"
		//	ENDCG
		//}
	}


	FallBack "VertexLit"
	CustomEditor "VRMobileStandardShaderGUI"
}
