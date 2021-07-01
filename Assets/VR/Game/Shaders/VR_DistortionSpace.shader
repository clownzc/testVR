// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4795,x:32724,y:32693,varname:node_4795,prsc:2|emission-1841-OUT,alpha-2741-OUT,refract-2432-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:32194,y:32367,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:66ce6f7dd5cd637468cdd310c05088b2,ntxv:0,isnm:False|UVIN-3231-OUT;n:type:ShaderForge.SFN_Color,id:797,x:32193,y:32589,ptovrint:True,ptlb:Tint,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Slider,id:7330,x:32036,y:32800,ptovrint:False,ptlb:Emmision,ptin:_Emmision,varname:node_7330,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:5.353121,max:10;n:type:ShaderForge.SFN_TexCoord,id:6922,x:31739,y:32275,varname:node_6922,prsc:2,uv:0;n:type:ShaderForge.SFN_Add,id:3231,x:32011,y:32285,varname:node_3231,prsc:2|A-6922-UVOUT,B-1447-OUT;n:type:ShaderForge.SFN_TexCoord,id:5262,x:31061,y:32827,varname:node_5262,prsc:2,uv:0;n:type:ShaderForge.SFN_Panner,id:3561,x:31249,y:32827,varname:node_3561,prsc:2,spu:0.1,spv:0.1|UVIN-5262-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:6164,x:31456,y:32827,ptovrint:False,ptlb:Desortion Tex,ptin:_DesortionTex,varname:node_6164,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:e1f99598032c19e409d6d42147b0e406,ntxv:0,isnm:False|UVIN-3561-UVOUT;n:type:ShaderForge.SFN_Slider,id:8409,x:31353,y:33082,ptovrint:False,ptlb:Desortion Mul,ptin:_DesortionMul,varname:node_8409,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.4378882,max:1;n:type:ShaderForge.SFN_Append,id:4472,x:31648,y:32844,varname:node_4472,prsc:2|A-6164-R,B-6164-G;n:type:ShaderForge.SFN_Multiply,id:1447,x:31838,y:32948,varname:node_1447,prsc:2|A-4472-OUT,B-8409-OUT;n:type:ShaderForge.SFN_Fresnel,id:9062,x:31818,y:31950,varname:node_9062,prsc:2;n:type:ShaderForge.SFN_Multiply,id:9938,x:32374,y:32589,varname:node_9938,prsc:2|A-6074-RGB,B-797-RGB,C-7330-OUT;n:type:ShaderForge.SFN_Slider,id:8961,x:31739,y:31869,ptovrint:False,ptlb:Fresnel Mul,ptin:_FresnelMul,varname:node_8961,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:7.81034,max:10;n:type:ShaderForge.SFN_Multiply,id:9302,x:32113,y:32068,varname:node_9302,prsc:2|A-8961-OUT,B-9062-OUT,C-5484-RGB;n:type:ShaderForge.SFN_Color,id:5484,x:31818,y:32116,ptovrint:False,ptlb:Fresnel Color,ptin:_FresnelColor,varname:node_5484,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.3906663,c2:0.0989403,c3:0.8970588,c4:1;n:type:ShaderForge.SFN_Slider,id:2741,x:32265,y:32911,ptovrint:False,ptlb:Opacity,ptin:_Opacity,varname:node_2741,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5675214,max:1;n:type:ShaderForge.SFN_Slider,id:8288,x:31960,y:33112,ptovrint:False,ptlb:Refraction Desortion ,ptin:_RefractionDesortion,varname:node_8288,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.05930739,max:1;n:type:ShaderForge.SFN_Multiply,id:2432,x:32372,y:33020,varname:node_2432,prsc:2|A-1447-OUT,B-8288-OUT,C-4092-OUT;n:type:ShaderForge.SFN_Add,id:1841,x:32546,y:32488,varname:node_1841,prsc:2|A-9302-OUT,B-9938-OUT;n:type:ShaderForge.SFN_Vector1,id:4092,x:32081,y:33274,varname:node_4092,prsc:2,v1:0.1;proporder:6074-797-7330-6164-8409-8961-5484-2741-8288;pass:END;sub:END;*/

Shader "VR/VR_DistortionSpace" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _TintColor ("Tint", Color) = (0.5,0.5,0.5,1)
        _Emmision ("Emmision", Range(0, 10)) = 5.353121
        _DesortionTex ("Desortion Tex", 2D) = "white" {}
        _DesortionMul ("Desortion Mul", Range(0, 1)) = 0.4378882
        _FresnelMul ("Fresnel Mul", Range(0, 10)) = 7.81034
        _FresnelColor ("Fresnel Color", Color) = (0.3906663,0.0989403,0.8970588,1)
        _Opacity ("Opacity", Range(0, 1)) = 0.5675214
        _RefractionDesortion ("Refraction Desortion ", Range(0, 1)) = 0.05930739
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            ZWrite Off
			Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _TintColor;
            uniform float _Emmision;
            uniform sampler2D _DesortionTex; uniform float4 _DesortionTex_ST;
            uniform float _DesortionMul;
            uniform float _FresnelMul;
            uniform float4 _FresnelColor;
            uniform float _Opacity;
            uniform float _RefractionDesortion;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
                UNITY_FOG_COORDS(4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float4 node_2392 = _Time + _TimeEditor;
                float2 node_3561 = (i.uv0+node_2392.g*float2(0.1,0.1));
                float4 _DesortionTex_var = tex2D(_DesortionTex,TRANSFORM_TEX(node_3561, _DesortionTex));
                float2 node_1447 = (float2(_DesortionTex_var.r,_DesortionTex_var.g)*_DesortionMul);
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (node_1447*_RefractionDesortion*0.1);
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
////// Lighting:
////// Emissive:
                float2 node_3231 = (i.uv0+node_1447);
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_3231, _MainTex));
                float3 emissive = ((_FresnelMul*(1.0-max(0,dot(normalDirection, viewDirection)))*_FresnelColor.rgb)+(_MainTex_var.rgb*_TintColor.rgb*_Emmision));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(lerp(sceneColor.rgb, finalColor,_Opacity),1);
                UNITY_APPLY_FOG_COLOR(i.fogCoord, finalRGBA, fixed4(0,0,0,1));
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
