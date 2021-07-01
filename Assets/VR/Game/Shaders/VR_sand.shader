// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.16 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.16;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:0,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:False,mssp:False,bkdf:True,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:False,rfrpn:Refraction,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:True;n:type:ShaderForge.SFN_Final,id:636,x:33665,y:32553,varname:node_636,prsc:2|diff-2301-OUT,spec-4910-OUT,normal-577-OUT;n:type:ShaderForge.SFN_Vector4Property,id:7293,x:32905,y:32372,ptovrint:False,ptlb:base_color,ptin:_base_color,varname:node_7293,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5,v2:0.5,v3:0.5,v4:0;n:type:ShaderForge.SFN_Tex2d,id:2579,x:31796,y:32372,ptovrint:False,ptlb:Specular_map,ptin:_Specular_map,varname:node_2579,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:1afbfe274e9e05c489882e22677a6620,ntxv:0,isnm:False|MIP-3468-OUT;n:type:ShaderForge.SFN_Power,id:1478,x:32005,y:32604,varname:node_1478,prsc:2|VAL-2579-G,EXP-5104-W;n:type:ShaderForge.SFN_Tex2d,id:6882,x:32086,y:32785,ptovrint:False,ptlb:Normalmap,ptin:_Normalmap,varname:node_6882,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:a5444dab840e23e4b9ef0b579e559b37,ntxv:3,isnm:True|UVIN-6583-OUT;n:type:ShaderForge.SFN_Multiply,id:6353,x:32666,y:32729,varname:node_6353,prsc:2|A-5652-OUT,B-9046-OUT;n:type:ShaderForge.SFN_ValueProperty,id:9046,x:32297,y:32917,ptovrint:False,ptlb:Normal_Inten,ptin:_Normal_Inten,varname:node_9046,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_ComponentMask,id:5652,x:32333,y:32721,varname:node_5652,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-6882-RGB;n:type:ShaderForge.SFN_Append,id:5281,x:32935,y:32757,varname:node_5281,prsc:2|A-6353-OUT,B-6882-B;n:type:ShaderForge.SFN_Vector1,id:3468,x:31405,y:32266,varname:node_3468,prsc:2,v1:-0.5;n:type:ShaderForge.SFN_Tex2d,id:1176,x:32416,y:32346,ptovrint:False,ptlb:Mip_Mask,ptin:_Mip_Mask,varname:node_1176,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:66a55af695a24f348bda9e683a483c08,ntxv:0,isnm:False|MIP-3468-OUT;n:type:ShaderForge.SFN_Multiply,id:6219,x:32416,y:32537,varname:node_6219,prsc:2|A-5104-XYZ,B-1478-OUT;n:type:ShaderForge.SFN_Vector4Property,id:5104,x:31600,y:32531,ptovrint:False,ptlb:specular_color&Exp,ptin:_specular_colorExp,varname:_base_color_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1,v2:1,v3:1,v4:0;n:type:ShaderForge.SFN_Multiply,id:4910,x:32905,y:32558,varname:node_4910,prsc:2|A-1176-RGB,B-6219-OUT;n:type:ShaderForge.SFN_NormalVector,id:2860,x:31682,y:33014,prsc:2,pt:False;n:type:ShaderForge.SFN_Dot,id:8947,x:32032,y:33077,varname:node_8947,prsc:2,dt:1|A-7908-XYZ,B-4872-XYZ;n:type:ShaderForge.SFN_Transform,id:4872,x:31839,y:33198,varname:node_4872,prsc:2,tffrom:2,tfto:0|IN-7174-OUT;n:type:ShaderForge.SFN_Add,id:776,x:32422,y:33070,varname:node_776,prsc:2|A-8947-OUT,B-3819-W;n:type:ShaderForge.SFN_Power,id:5116,x:32625,y:33055,varname:node_5116,prsc:2|VAL-776-OUT,EXP-6188-OUT;n:type:ShaderForge.SFN_Vector1,id:6188,x:32453,y:33261,varname:node_6188,prsc:2,v1:2;n:type:ShaderForge.SFN_Clamp01,id:7440,x:32820,y:33038,varname:node_7440,prsc:2|IN-5116-OUT;n:type:ShaderForge.SFN_Lerp,id:577,x:33295,y:32799,varname:node_577,prsc:2|A-5281-OUT,B-5016-OUT,T-7440-OUT;n:type:ShaderForge.SFN_Vector3,id:5016,x:32935,y:32905,varname:node_5016,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Normalize,id:7174,x:31598,y:33223,varname:node_7174,prsc:2|IN-3819-XYZ;n:type:ShaderForge.SFN_Transform,id:7908,x:31839,y:33014,varname:node_7908,prsc:2,tffrom:0,tfto:2|IN-2860-OUT;n:type:ShaderForge.SFN_Vector4Property,id:3819,x:31368,y:33241,ptovrint:False,ptlb:Wind_direction&amount,ptin:_Wind_directionamount,varname:node_3819,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.3,v2:1,v3:0,v4:1;n:type:ShaderForge.SFN_Tex2d,id:6264,x:32799,y:32059,ptovrint:False,ptlb:SandWind_Map,ptin:_SandWind_Map,varname:node_6264,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:e8715b4e48814534fb39390dd15da8fd,ntxv:0,isnm:False|UVIN-7132-OUT;n:type:ShaderForge.SFN_TexCoord,id:6158,x:32109,y:31723,varname:node_6158,prsc:2,uv:0;n:type:ShaderForge.SFN_Add,id:7132,x:32606,y:32059,varname:node_7132,prsc:2|A-9035-OUT,B-9673-OUT;n:type:ShaderForge.SFN_Time,id:2536,x:31089,y:32444,varname:node_2536,prsc:2;n:type:ShaderForge.SFN_Multiply,id:419,x:32109,y:32036,varname:node_419,prsc:2|A-2536-TSL,B-3189-X;n:type:ShaderForge.SFN_Multiply,id:6324,x:32109,y:32193,varname:node_6324,prsc:2|A-2536-TSL,B-3189-Y;n:type:ShaderForge.SFN_Append,id:9673,x:32350,y:32109,varname:node_9673,prsc:2|A-419-OUT,B-6324-OUT;n:type:ShaderForge.SFN_Add,id:2301,x:33398,y:32336,varname:node_2301,prsc:2|A-3321-OUT,B-7293-XYZ;n:type:ShaderForge.SFN_Multiply,id:3321,x:33104,y:32178,varname:node_3321,prsc:2|A-6264-R,B-2058-XYZ;n:type:ShaderForge.SFN_Vector4Property,id:2058,x:31393,y:32001,ptovrint:False,ptlb:SandWind_Color&Distort_Inten,ptin:_SandWind_ColorDistort_Inten,varname:node_2058,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.2,v2:0.2,v3:0.1,v4:0;n:type:ShaderForge.SFN_Tex2d,id:254,x:31685,y:31736,ptovrint:False,ptlb:Distortion_Map,ptin:_Distortion_Map,varname:node_254,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:56c80aa8eb3968e4e85860fa95a8aed1,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Add,id:9035,x:32350,y:31944,varname:node_9035,prsc:2|A-6158-UVOUT,B-3808-OUT;n:type:ShaderForge.SFN_ComponentMask,id:4217,x:31859,y:31736,varname:node_4217,prsc:2,cc1:0,cc2:0,cc3:-1,cc4:-1|IN-254-RGB;n:type:ShaderForge.SFN_Multiply,id:3808,x:32109,y:31875,varname:node_3808,prsc:2|A-4217-OUT,B-2058-W;n:type:ShaderForge.SFN_Vector4Property,id:3189,x:30903,y:32662,ptovrint:False,ptlb:Normal&Sand_Panner,ptin:_NormalSand_Panner,varname:node_3189,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:-1,v3:0,v4:0;n:type:ShaderForge.SFN_TexCoord,id:288,x:31507,y:32723,varname:node_288,prsc:2,uv:0;n:type:ShaderForge.SFN_Add,id:6583,x:31728,y:32797,varname:node_6583,prsc:2|A-288-UVOUT,B-3169-OUT;n:type:ShaderForge.SFN_Append,id:3169,x:31507,y:32901,varname:node_3169,prsc:2|A-3016-OUT,B-6414-OUT;n:type:ShaderForge.SFN_Multiply,id:3016,x:31302,y:32723,varname:node_3016,prsc:2|A-2536-TSL,B-3189-Z;n:type:ShaderForge.SFN_Multiply,id:6414,x:31302,y:32913,varname:node_6414,prsc:2|A-2536-TSL,B-3189-W;n:type:ShaderForge.SFN_NormalVector,id:5642,x:33137,y:32506,prsc:2,pt:False;n:type:ShaderForge.SFN_Vector1,id:3794,x:32048,y:33252,varname:node_3794,prsc:2,v1:0.8;n:type:ShaderForge.SFN_Clamp01,id:2387,x:32231,y:33052,varname:node_2387,prsc:2|IN-8947-OUT;proporder:7293-2579-6882-9046-1176-5104-3819-6264-254-2058-3189;pass:END;sub:END;*/

Shader "VR/Sand" {
    Properties {
        _base_color ("base_color", Vector) = (0.5,0.5,0.5,0)
        _Specular_map ("Specular_map", 2D) = "white" {}
        _Normalmap ("Normalmap", 2D) = "bump" {}
        _Normal_Inten ("Normal_Inten", Float ) = 0.5
        _Mip_Mask ("Mip_Mask", 2D) = "white" {}
        _specular_colorExp ("specular_color&Exp", Vector) = (1,1,1,0)
        _Wind_directionamount ("Wind_direction&amount", Vector) = (0.3,1,0,1)
        _SandWind_Map ("SandWind_Map", 2D) = "white" {}
        _Distortion_Map ("Distortion_Map", 2D) = "white" {}
        _SandWind_ColorDistort_Inten ("SandWind_Color&Distort_Inten", Vector) = (0.2,0.2,0.1,0)
        _NormalSand_Panner ("Normal&Sand_Panner", Vector) = (0,-1,0,0)
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
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
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile_fog
            uniform float4 _TimeEditor;
            uniform float4 _base_color;
            uniform sampler2D _Specular_map; uniform float4 _Specular_map_ST;
            uniform sampler2D _Normalmap; uniform float4 _Normalmap_ST;
            uniform float _Normal_Inten;
            uniform sampler2D _Mip_Mask; uniform float4 _Mip_Mask_ST;
            uniform float4 _specular_colorExp;
            uniform float4 _Wind_directionamount;
            uniform sampler2D _SandWind_Map; uniform float4 _SandWind_Map_ST;
            uniform float4 _SandWind_ColorDistort_Inten;
            uniform sampler2D _Distortion_Map; uniform float4 _Distortion_Map_ST;
            uniform float4 _NormalSand_Panner;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD10;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #elif UNITY_SHOULD_SAMPLE_SH
            #endif
            #ifdef DYNAMICLIGHTMAP_ON
                o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
            #endif
            o.normalDir = UnityObjectToWorldNormal(v.normal);
            o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
            o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
            o.posWorld = mul(unity_ObjectToWorld, v.vertex);
            float3 lightColor = _LightColor0.rgb;
            o.pos = UnityObjectToClipPos(v.vertex);
            UNITY_TRANSFER_FOG(o,o.pos);
            TRANSFER_VERTEX_TO_FRAGMENT(o)
            return o;
        }
        float4 frag(VertexOutput i) : COLOR {
            i.normalDir = normalize(i.normalDir);
            float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
/// Vectors:
            float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
            float4 node_2536 = _Time + _TimeEditor;
            float2 node_6583 = (i.uv0+float2((node_2536.r*_NormalSand_Panner.b),(node_2536.r*_NormalSand_Panner.a)));
            float3 _Normalmap_var = UnpackNormal(tex2D(_Normalmap,TRANSFORM_TEX(node_6583, _Normalmap)));
            float node_8947 = max(0,dot(mul( tangentTransform, i.normalDir ).xyz.rgb,mul( normalize(_Wind_directionamount.rgb), tangentTransform ).xyz.rgb));
            float3 normalLocal = lerp(float3((_Normalmap_var.rgb.rg*_Normal_Inten),_Normalmap_var.b),float3(0,0,1),saturate(pow((node_8947+_Wind_directionamount.a),2.0)));
            float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
            float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
            float3 lightColor = _LightColor0.rgb;
            float3 halfDirection = normalize(viewDirection+lightDirection);
// Lighting:
            float attenuation = LIGHT_ATTENUATION(i);
            float3 attenColor = attenuation * _LightColor0.xyz;
///// Gloss:
            float gloss = 0.5;
            float specPow = exp2( gloss * 10.0+1.0);
/// GI Data:
            UnityLight light;
            #ifdef LIGHTMAP_OFF
                light.color = lightColor;
                light.dir = lightDirection;
                light.ndotl = LambertTerm (normalDirection, light.dir);
            #else
                light.color = half3(0.f, 0.f, 0.f);
                light.ndotl = 0.0f;
                light.dir = half3(0.f, 0.f, 0.f);
            #endif
            UnityGIInput d;
            d.light = light;
            d.worldPos = i.posWorld.xyz;
            d.worldViewDir = viewDirection;
            d.atten = attenuation;
            #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                d.ambient = 0;
                d.lightmapUV = i.ambientOrLightmapUV;
            #else
                d.ambient = i.ambientOrLightmapUV;
            #endif
            UnityGI gi = UnityGlobalIllumination (d, 1, gloss, normalDirection);
            lightDirection = gi.light.dir;
            lightColor = gi.light.color;
// Specular:
            float NdotL = max(0, dot( normalDirection, lightDirection ));
            float node_3468 = (-0.5);
            float4 _Mip_Mask_var = tex2Dlod(_Mip_Mask,float4(TRANSFORM_TEX(i.uv0, _Mip_Mask),0.0,node_3468));
            float4 _Specular_map_var = tex2Dlod(_Specular_map,float4(TRANSFORM_TEX(i.uv0, _Specular_map),0.0,node_3468));
            float3 specularColor = (_Mip_Mask_var.rgb*(_specular_colorExp.rgb*pow(_Specular_map_var.g,_specular_colorExp.a)));
            float3 directSpecular = 1 * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
            float3 specular = directSpecular;
/// Diffuse:
            NdotL = max(0.0,dot( normalDirection, lightDirection ));
            float3 directDiffuse = max( 0.0, NdotL) * attenColor;
            float3 indirectDiffuse = float3(0,0,0);
            indirectDiffuse += gi.indirect.diffuse;
            float4 _Distortion_Map_var = tex2D(_Distortion_Map,TRANSFORM_TEX(i.uv0, _Distortion_Map));
            float2 node_7132 = ((i.uv0+(_Distortion_Map_var.rgb.rr*_SandWind_ColorDistort_Inten.a))+float2((node_2536.r*_NormalSand_Panner.r),(node_2536.r*_NormalSand_Panner.g)));
            float4 _SandWind_Map_var = tex2D(_SandWind_Map,TRANSFORM_TEX(node_7132, _SandWind_Map));
            float3 diffuseColor = ((_SandWind_Map_var.r*_SandWind_ColorDistort_Inten.rgb)+_base_color.rgb);
            float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
// Final Color:
            float3 finalColor = diffuse + specular;
            fixed4 finalRGBA = fixed4(finalColor,1);
            UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
            return finalRGBA;
        }
        ENDCG
    }
    Pass {
        Name "Meta"
        Tags {
            "LightMode"="Meta"
        }
        Cull Off
        
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #define UNITY_PASS_META 1
        #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
        #include "UnityCG.cginc"
        #include "Lighting.cginc"
        #include "UnityPBSLighting.cginc"
        #include "UnityStandardBRDF.cginc"
        #include "UnityMetaPass.cginc"
        #pragma fragmentoption ARB_precision_hint_fastest
        #pragma multi_compile_shadowcaster
        #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
        #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
        #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
        #pragma multi_compile_fog
        uniform float4 _TimeEditor;
        uniform float4 _base_color;
        uniform sampler2D _Specular_map; uniform float4 _Specular_map_ST;
        uniform sampler2D _Mip_Mask; uniform float4 _Mip_Mask_ST;
        uniform float4 _specular_colorExp;
        uniform sampler2D _SandWind_Map; uniform float4 _SandWind_Map_ST;
        uniform float4 _SandWind_ColorDistort_Inten;
        uniform sampler2D _Distortion_Map; uniform float4 _Distortion_Map_ST;
        uniform float4 _NormalSand_Panner;
        struct VertexInput {
            float4 vertex : POSITION;
            float2 texcoord0 : TEXCOORD0;
            float2 texcoord1 : TEXCOORD1;
            float2 texcoord2 : TEXCOORD2;
        };
        struct VertexOutput {
            float4 pos : SV_POSITION;
            float2 uv0 : TEXCOORD0;
            float2 uv1 : TEXCOORD1;
            float2 uv2 : TEXCOORD2;
            float4 posWorld : TEXCOORD3;
        };
        VertexOutput vert (VertexInput v) {
            VertexOutput o = (VertexOutput)0;
            o.uv0 = v.texcoord0;
            o.uv1 = v.texcoord1;
            o.uv2 = v.texcoord2;
            o.posWorld = mul(unity_ObjectToWorld, v.vertex);
            o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
            return o;
        }
        float4 frag(VertexOutput i) : SV_Target {
/// Vectors:
            float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
            UnityMetaInput o;
            UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
            
            o.Emission = 0;
            
            float4 _Distortion_Map_var = tex2D(_Distortion_Map,TRANSFORM_TEX(i.uv0, _Distortion_Map));
            float4 node_2536 = _Time + _TimeEditor;
            float2 node_7132 = ((i.uv0+(_Distortion_Map_var.rgb.rr*_SandWind_ColorDistort_Inten.a))+float2((node_2536.r*_NormalSand_Panner.r),(node_2536.r*_NormalSand_Panner.g)));
            float4 _SandWind_Map_var = tex2D(_SandWind_Map,TRANSFORM_TEX(node_7132, _SandWind_Map));
            float3 diffColor = ((_SandWind_Map_var.r*_SandWind_ColorDistort_Inten.rgb)+_base_color.rgb);
            float node_3468 = (-0.5);
            float4 _Mip_Mask_var = tex2Dlod(_Mip_Mask,float4(TRANSFORM_TEX(i.uv0, _Mip_Mask),0.0,node_3468));
            float4 _Specular_map_var = tex2Dlod(_Specular_map,float4(TRANSFORM_TEX(i.uv0, _Specular_map),0.0,node_3468));
            float3 specColor = (_Mip_Mask_var.rgb*(_specular_colorExp.rgb*pow(_Specular_map_var.g,_specular_colorExp.a)));
            o.Albedo = diffColor + specColor * 0.125; // No gloss connected. Assume it's 0.5
            
            return UnityMetaFragment( o );
        }
        ENDCG
    }
}
FallBack "Diffuse"
CustomEditor "ShaderForgeMaterialInspector"
}
