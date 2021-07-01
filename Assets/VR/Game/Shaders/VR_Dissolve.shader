// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: commented out 'float4 unity_DynamicLightmapST', a built-in variable
Shader "VR/Dissolve" {
    Properties {
		[Enum(Off,0,On,1)] _Lighting_Mode("Lighting Mode", Float) = 1
		[Enum(LEqual,4,Always,8)] _ZTest("ZTest Mode", Float) = 4
        _Albedo ("Albedo", 2D) = "white" {}
        _Tint_Color ("Tint_Color", Color) = (0.4,0.4,1,1)
        _Normal ("Normal", 2D) = "bump" {}
        _Mix_Texture ("Mix_Texture", 2D) = "white" {}
        _Dissolve_Color ("Dissolve_Color", Color) = (1,1,1,1)
        _TransparencyMask ("Dissolve", Float ) = 1.5
        _Dissolve_Color_Mul ("Dissolve_Color_Mul", Float ) = 1
        _Fresnel_Color ("Fresnel_Color", Color) = (0.5,0.5,0.5,1)
        _Emissive_Color ("Emissive_Color", Color) = (0.5,0.5,0.5,1)
        _Emissive_Inten ("Emissive_Inten", Float ) = 1
        _Edge_Thickness ("Edge_Thickness", Range(1,8)) = 1
        _Fresnel_Add ("Fresnel_Add", Float ) = 0
        _Fresnel_Pow ("Fresnel_Pow", Float ) = 1
        _Fresnel_Mul ("Fresnel_Mul", Float ) = 1
        _Mix_Texture_01 ("Mix_Texture_01", 2D) = "black" {}
        [HideInInspector] _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
		[Enum(Off,0,On,1)] _Fog("Fog Mode", Float) = 0
		[Enum(Off,0,Front,1,Back,2)]_Cull("Cull Mode", Float) = 2
    }
    SubShader {
        Tags {
			"Queue" = "Overlay"
            "RenderType"="TransparentCutout"
        }
        LOD 200
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
			Cull [_Cull]
			ZTest [_ZTest]
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fog
			#pragma shader_feature _Fog
			#pragma shader_feature _LIGHTING
            uniform float4 _LightColor0;
            #ifdef DYNAMICLIGHTMAP_ON
            #endif
            uniform sampler2D _Mix_Texture; uniform fixed4 _Mix_Texture_ST;
            uniform fixed _TransparencyMask;
            uniform sampler2D _Albedo; uniform fixed4 _Albedo_ST;
            uniform sampler2D _Normal; uniform fixed4 _Normal_ST;
            uniform fixed _Edge_Thickness;
            uniform fixed _Fresnel_Add;
            uniform fixed _Fresnel_Pow;
            uniform fixed4 _Tint_Color;
            uniform fixed4 _Fresnel_Color;
            uniform fixed _Fresnel_Mul;
            uniform fixed4 _Dissolve_Color;
            uniform fixed _Dissolve_Color_Mul;
            uniform fixed _Emissive_Inten;
            uniform sampler2D _Mix_Texture_01; uniform fixed4 _Mix_Texture_01_ST;
            uniform fixed4 _Emissive_Color;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
                float3 tangentDir : TEXCOORD4;
                float3 binormalDir : TEXCOORD5;
                LIGHTING_COORDS(6,7)
				#if _Fog
                UNITY_FOG_COORDS(8)
				#endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.normalDir = mul(unity_ObjectToWorld, fixed4(v.normal,0)).xyz;
                o.tangentDir = normalize( mul( unity_ObjectToWorld, fixed4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
				#if _Fog
                UNITY_TRANSFER_FOG(o,o.pos);
				#endif
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
				//WorldSpace TBN
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);

				//NormalMap Sampler
                fixed3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(i.uv0, _Normal)));

				//Transform to WorldNormal
                fixed3 normalDirection = normalize(mul(_Normal_var, tangentTransform )); // Perturbed normals

				//Other Vectors
				fixed3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
				fixed3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				fixed3 halfDirection = normalize(viewDirection + lightDirection);

                fixed4 _Mix_Texture_var = tex2D(_Mix_Texture,TRANSFORM_TEX(i.uv1, _Mix_Texture));
                fixed Clipmask = pow(saturate((((_Mix_Texture_var.b+1.0)- (1.9 - 1.1 * _TransparencyMask))*5.0)),_Edge_Thickness);

				//Clip AlphaTest
                clip(Clipmask - 0.5);
				
				//Constant Define
				fixed3 diffuse = fixed3(0, 0, 0);
				fixed3 specular = fixed3(0, 0, 0);
				fixed4 _Mix_Texture_01_var = fixed4(0, 0, 0, 0);

				fixed4 _Albedo_var = tex2D(_Albedo, TRANSFORM_TEX(i.uv0, _Albedo));
				_Albedo_var = _Albedo_var * _Tint_Color;
////// Specular:
				#if _LIGHTING
				//Gloss to SpecPow
				fixed gloss = 0.5;
				fixed specPow = exp2(gloss * 10.0 + 1.0);

				//Lighting Atten
				fixed3 lightColor = _LightColor0.rgb;
				fixed attenuation = LIGHT_ATTENUATION(i);
				fixed3 attenColor = attenuation * _LightColor0.xyz;

				_Mix_Texture_01_var = tex2D(_Mix_Texture_01, TRANSFORM_TEX(i.uv0, _Mix_Texture_01));
				fixed NdotL = max(0, dot(normalDirection, lightDirection));
                fixed3 specularColor = fixed3(_Mix_Texture_01_var.g,_Mix_Texture_01_var.g,_Mix_Texture_01_var.g);
                fixed3 directSpecular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),specPow);
                specular = directSpecular * specularColor;
/////// Diffuse:
                fixed3 indirectDiffuse = fixed3(0,0,0);
                fixed3 directDiffuse = max( 0.0, NdotL) * lightColor;
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                diffuse = directDiffuse + indirectDiffuse;
				#endif
////// Emissive:
                fixed3 emissive = ((((pow((1.0 - max(0,dot(normalize(mul( _Normal_var.rgb, tangentTransform ).xyz.rgb),viewDirection))),_Fresnel_Pow)+_Fresnel_Add)*_Fresnel_Color.rgb)*_Fresnel_Mul)+((_Emissive_Inten*_Mix_Texture_01_var.r)*_Emissive_Color.rgb)) + lerp((_Dissolve_Color.rgb*_Dissolve_Color_Mul), _Albedo_var, Clipmask);
/// Final Color:
				fixed3 finalColor = diffuse + specular + emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);

				#if _Fog
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
				#endif

                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "VRDistortionShaderGUI"
}
