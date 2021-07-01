// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "VR/Character" {
    Properties {
		[Enum(Off,0,Front,1,Back,2)] _CullMode ("Cull Mode", Float) = 2.0
		[Enum(OFF,0,ON,1)] _FogMode ("Fog Mode", Float) = 1.0

		_Albedo("Albedo", 2D) = "white" {}
		_AlbedoTint("Albedo Tint", Color) = (1,1,1,1)
		_Normal("Normal", 2D) = "bump" {}
		_MixMap("MixMap", 2D) = "white" {}
		_Bump_Inten("Bump_Inten",Range(0,15)) = 1
		_OccStrength("Occlusion Strength",Range(0, 1)) = 1
        _Metallic1 ("Metallic", Range(0, 1) ) = 0
		_Gloss("Gloss", Range(0 ,1)) = 0
        _Glossmin ("GlossMin", Range(0 ,1) ) = 0
		_Glossmax ("GlossMax", Range(0 ,1) ) = 0
		_Minnaert_Power("Minnaert_Pow", Range(0.01 ,3)) = 0 // 没用？ By HZX
		_Cube ("Reflection Cubemap", Cube) = "_Skybox" { }
		_ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
		_RefInten ("RefInten", Range(0,10)) = 1.0

		_EmissionColor("Emission Color", Color) = (0,0,0)
		_EmissionMap("Emission", 2D) = "white" {}
		_EmissionMul("Emission Mul", Range(0, 10)) = 1

		_FresnelColor("FresnelColor", Color) = (0,0,0)
		_FresnelMap("Fresnel", 2D) = "white" {}
		_FresnelAdd("Fresnel Add", Range(-2, 2)) = 0
		_FresnelMul("Fresnel Mul", Range(0, 10)) = 1
		_FresnelExp("Fresnel Exp", Range(0.01, 10)) = 1

		_Transparency("Transparency", Range(0, 1)) = 1
    }
    SubShader {
        Tags {
			"Queue"="Geometry+1" "RenderType"="Opaque" "PerformanceChecks"="False"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			#include "UnityStandardBRDF.cginc"
			#include "BRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma target 3.0

			#pragma shader_feature _EMISSION
			#pragma shader_feature _FRESNEL
			#pragma shader_feature _FOG

			half4 _AlbedoTint;
            half _Metallic1;
			half _Gloss;
            half _Glossmin;
			half _Glossmax;
			half _Minnaert_Power;
			half _Bump_Inten;
			half _OccStrength;
            sampler2D _Albedo; half4 _Albedo_ST;
			sampler2D _Normal; half4 _Normal_ST;
			sampler2D _MixMap; half4 _MixMap_ST;
			samplerCUBE _Cube;
			half4 _ReflectColor;
			half _RefInten;

			half4 _EmissionColor;
			sampler2D _EmissionMap;
			half _EmissionMul;

			half4 _FresnelColor;
			sampler2D _FresnelMap;
			half _FresnelAdd;
			half _FresnelMul;
			half _FresnelExp;

			half _Transparency;

            struct a2f {
                half4 vertex : POSITION;
                half3 normal : NORMAL;
                half4 tangent : TANGENT;
                half2 texcoord0 : TEXCOORD0;
                half2 texcoord1 : TEXCOORD1;
                half2 texcoord2 : TEXCOORD2;
            };
            struct v2f {
                half4 pos : SV_POSITION;
                half4 uv : TEXCOORD0;
                half3 eyeVec : TEXCOORD2;
                half4 posWorld : TEXCOORD3;
                half3 normalDir : TEXCOORD4;
                half3 tangentDir : TEXCOORD5;
                half3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(1)
            };
			v2f vert (a2f v) {
				v2f o;
                o.uv.xy = v.texcoord0;
                o.uv.zw = v.texcoord1;
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.eyeVec = _WorldSpaceCameraPos - o.posWorld;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, half4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                half3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            half4 frag(v2f i) : COLOR {
////// TBN Matrix:
                i.normalDir = normalize(i.normalDir);
                half3x3 tangentTransform = half3x3( i.tangentDir, i.bitangentDir, i.normalDir);

////// Normal:
				half3 Normal_Vector = UnpackScaleNormal(tex2D(_Normal,TRANSFORM_TEX(i.uv.xy, _Normal)),_Bump_Inten);
				half3 normalLocal = Normal_Vector.rgb;
                half3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals

				half3 viewDirection = normalize(i.eyeVec);

                half3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                half3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                half3 lightColor = _LightColor0.rgb;
                half3 halfDirection = normalize(viewDirection+lightDirection);

				half3 Mixmap = tex2D(_MixMap, TRANSFORM_TEX(i.uv.xy, _MixMap));

////// Ambient Occlusion:
				half occ = LerpOneTo(Mixmap.g, _OccStrength);
////// Metallic:
				half Metallic = Mixmap.r * _Metallic1;

////// Lighting:
                half attenuation = LIGHT_ATTENUATION(i);
                half3 attenColor = attenuation * _LightColor0.xyz;
                half Pi = 3.141592654;
                half InvPi = 0.31830988618;
///////// Gloss:
				half gloss = lerp(_Glossmin, _Glossmax, Mixmap.b * _Gloss);
                //half specPow = exp2( gloss * 10.0+1.0);
/////// GI Data:
                UnityLight light;
                light.color = lightColor;
                light.dir = lightDirection;
                light.ndotl = LambertTerm (normalDirection, light.dir);
				//【1】实例化一个UnityGIInput的对象
                UnityGIInput d;
				UNITY_INITIALIZE_OUTPUT(UnityGIInput, d);
				//【2】填充此UnityGIInput对象的各个值
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;

				//By HZX Move To FS
				half4 ambientOrLightmapUV = half4(0, 0, 0, 0);
                #ifdef LIGHTMAP_ON
                    ambientOrLightmapUV.xy = i.uv.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    ambientOrLightmapUV.zw = 0;
                #elif DYNAMICLIGHTMAP_ON
                    ambientOrLightmapUV.zw = i.uv.zw * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif

                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = ambientOrLightmapUV;
                #else
					d.ambient = ambientOrLightmapUV;
                #endif

				d.probeHDR[0] = unity_SpecCube0_HDR;

                //d.boxMax[0] = unity_SpecCube0_BoxMax;
                //d.boxMin[0] = unity_SpecCube0_BoxMin;
                //d.probePosition[0] = unity_SpecCube0_ProbePosition;
                //d.probeHDR[0] = unity_SpecCube0_HDR;
				half3 r = reflect(i.eyeVec, i.normalDir);
				d.probeHDR[0] = texCUBE(_Cube, r) * _ReflectColor * _RefInten;

                //d.boxMax[1] = unity_SpecCube1_BoxMax;
                //d.boxMin[1] = unity_SpecCube1_BoxMin;
                //d.probePosition[1] = unity_SpecCube1_ProbePosition;
                //d.probeHDR[1] = unity_SpecCube1_HDR;

                Unity_GlossyEnvironmentData g;
                g.roughness = 1.0 - gloss;
                g.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, occ, normalDirection, g);//1 is Occlusion

                lightDirection = gi.light.dir;
                lightColor = gi.light.color;

////// Vector:
                half NdotL = saturate(dot(normalDirection, lightDirection));
				half RdotL = saturate(dot(viewReflectDirection, lightDirection));
				half NdotV = max(0.0, dot(normalDirection, viewDirection));
////// Albedo
                half3 diffuseColor = tex2D(_Albedo,TRANSFORM_TEX(i.uv.xy, _Albedo)) * _AlbedoTint;
				
                half specularMonochrome;
                half3 specularColor;
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, Metallic, specularColor, specularMonochrome );
                specularMonochrome = 1-specularMonochrome;

				//Specular BRDF
				specularColor = EnvBRDFApprox(specularColor, 1.0 - gloss, NdotL);

				half3 directSpecular = specularColor * PhongApprox(1.0 - gloss, RdotL) * attenColor * NdotL; 

                half3 indirectSpecular = gi.indirect.specular;
                indirectSpecular *= specularColor;
                half3 specular = indirectSpecular + directSpecular;

/////// Diffuse:
				half3 directDiffuse = NdotL * attenColor;
				half3 indirectDiffuse = gi.indirect.diffuse;
                half3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;

/////// Emission:
				//#ifdef _EMISSION
					half3 emission = tex2D(_EmissionMap, i.uv.xy).rgb * _EmissionColor.rgb * _EmissionMul;
				//#endif

/////// Fresnel:
				//#ifdef _FRESNEL
					half3 fresnel = (1 - saturate(dot(viewDirection, normalDirection)) + _FresnelAdd) * _FresnelColor.rgb;
					fresnel = pow(fresnel, _FresnelExp) * _FresnelMul;
				//#endif

/// Final Color:
                half3 finalColor = diffuse + specular + emission + fresnel;
                half4 finalRGBA = half4(finalColor, _Transparency);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    //FallBack "Diffuse"
    //CustomEditor "ShaderForgeMaterialInspector"
}
