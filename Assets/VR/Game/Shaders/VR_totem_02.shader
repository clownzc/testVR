// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.16 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.16;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:4795,x:32724,y:32693,varname:node_4795,prsc:2|emission-6478-OUT,alpha-8953-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:31542,y:32312,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:244cd4855863f834caff5fa6343b72a4,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:8805,x:31731,y:32742,ptovrint:False,ptlb:PannerTex,ptin:_PannerTex,varname:_MainTex_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:1fecaa5e2aac7d94ea3467f308def47f,ntxv:2,isnm:False|UVIN-5054-UVOUT;n:type:ShaderForge.SFN_Panner,id:5054,x:31528,y:32742,varname:node_5054,prsc:2,spu:-0.3,spv:0|UVIN-6128-UVOUT;n:type:ShaderForge.SFN_Multiply,id:9848,x:32146,y:32574,varname:node_9848,prsc:2|A-8898-OUT,B-6415-OUT;n:type:ShaderForge.SFN_VertexColor,id:894,x:32082,y:32906,varname:node_894,prsc:2;n:type:ShaderForge.SFN_Multiply,id:3610,x:32320,y:32679,varname:node_3610,prsc:2|A-9848-OUT,B-894-RGB;n:type:ShaderForge.SFN_Vector1,id:6847,x:32283,y:32495,varname:node_6847,prsc:2,v1:6;n:type:ShaderForge.SFN_Multiply,id:6478,x:32529,y:32639,varname:node_6478,prsc:2|A-6847-OUT,B-3610-OUT;n:type:ShaderForge.SFN_Vector1,id:2952,x:31765,y:32934,varname:node_2952,prsc:2,v1:4;n:type:ShaderForge.SFN_Multiply,id:6415,x:31933,y:32728,varname:node_6415,prsc:2|A-8805-RGB,B-2952-OUT;n:type:ShaderForge.SFN_TexCoord,id:6128,x:31323,y:32758,varname:node_6128,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:8953,x:32472,y:32962,varname:node_8953,prsc:2|A-6074-A,B-894-A;n:type:ShaderForge.SFN_Power,id:8898,x:31882,y:32413,varname:node_8898,prsc:2|VAL-6074-RGB,EXP-471-OUT;n:type:ShaderForge.SFN_Vector1,id:471,x:31561,y:32496,varname:node_471,prsc:2,v1:2;proporder:6074-8805;pass:END;sub:END;*/

Shader "VR/totem_02_ad" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _PannerTex ("PannerTex", 2D) = "black" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _PannerTex; uniform float4 _PannerTex_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
/////// Vectors:
////// Lighting:
////// Emissive:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float4 node_8482 = _Time + _TimeEditor;
                float2 node_5054 = (i.uv0+node_8482.g*float2(-0.3,0));
                float4 _PannerTex_var = tex2D(_PannerTex,TRANSFORM_TEX(node_5054, _PannerTex));
                float3 emissive = (6.0*((pow(_MainTex_var.rgb,2.0)*(_PannerTex_var.rgb*4.0))*i.vertexColor.rgb));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,(_MainTex_var.a*i.vertexColor.a));
                UNITY_APPLY_FOG_COLOR(i.fogCoord, finalRGBA, fixed4(0,0,0,1));
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
