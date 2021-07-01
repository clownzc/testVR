// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "VR/Character_Shader_01" {
    Properties {
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
		_Minnaert_Power("Minnaert_Pow", Range(0.01 ,3)) = 0
		_EmissionTint("Emission Tint", Color) = (0,0,0,0)
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
			#include "UnityStandardCore.cginc"
			#include "UnityPBSLighting.cginc"
			#include "UnityStandardBRDF.cginc"
			#include "BRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma target 3.0
			uniform half4 _AlbedoTint;
            uniform half _Metallic1;
			uniform half _Gloss;
            uniform half _Glossmin;
			uniform half _Glossmax;
			uniform half _Minnaert_Power;
			uniform half _Bump_Inten;
			uniform half _OccStrength;
            uniform sampler2D _Albedo; uniform half4 _Albedo_ST;
			uniform sampler2D _Normal; uniform half4 _Normal_ST;
			uniform sampler2D _MixMap; uniform half4 _MixMap_ST;
			uniform sampler2D _Emission; uniform half4 _Emission_ST;
			uniform half4 _EmissionTint;
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
                //half2 uv1 : TEXCOORD1;
                half3 eyeVec : TEXCOORD2;
                half4 posWorld : TEXCOORD3;
                half3 normalDir : TEXCOORD4;
                half3 tangentDir : TEXCOORD5;
                half3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(1)
                //#if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                //    half4 ambientOrLightmapUV : TEXCOORD10;
                //#endif
            };
			v2f vert (a2f v) {
				v2f o;
                o.uv.xy = v.texcoord0;
                o.uv.zw = v.texcoord1;
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.eyeVec = normalize(o.posWorld - _WorldSpaceCameraPos);
				/*By HZX Move To FS
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #elif UNITY_SHOULD_SAMPLE_SH
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
				*/
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, half4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                //o.posWorld = mul(unity_ObjectToWorld, v.vertex);
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

				half3 viewDirection = normalize(-i.eyeVec);
                //half3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);

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

                d.boxMax[0] = unity_SpecCube0_BoxMax;
                d.boxMin[0] = unity_SpecCube0_BoxMin;
                d.probePosition[0] = unity_SpecCube0_ProbePosition;
                d.probeHDR[0] = unity_SpecCube0_HDR;

                d.boxMax[1] = unity_SpecCube1_BoxMax;
                d.boxMin[1] = unity_SpecCube1_BoxMin;
                d.probePosition[1] = unity_SpecCube1_ProbePosition;
                d.probeHDR[1] = unity_SpecCube1_HDR;

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

                //half NdotH = max(0.0,dot( normalDirection, halfDirection));
				//half LdotH = saturate(dot(lightDirection, halfDirection));

                //half visTerm = SmithBeckmannVisibilityTerm( NdotL, NdotV, 1.0-gloss );
				//Blinn-Phong
                //half normTerm = max(0.0, NDFBlinnPhongNormalizedTerm(NdotH, RoughnessToSpecPower(1.0-gloss)));
				//Specular BRDF
                //half specularPBL = max(0, (NdotL*visTerm*normTerm) * (UNITY_PI / 4));
                //half3 directSpecular = 1 * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularPBL*lightColor*FresnelTerm(specularColor, LdotH);

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

/// Final Color:
                half3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    //FallBack "Diffuse"
    //CustomEditor "ShaderForgeMaterialInspector"
}
