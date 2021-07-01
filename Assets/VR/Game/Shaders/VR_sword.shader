// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.16 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.16;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:4795,x:32998,y:32649,varname:node_4795,prsc:2|emission-2393-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:32386,y:32434,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:False;n:type:ShaderForge.SFN_Multiply,id:2393,x:32821,y:32661,varname:node_2393,prsc:2|A-8382-OUT,B-1817-OUT;n:type:ShaderForge.SFN_Vector1,id:9248,x:32463,y:32611,varname:node_9248,prsc:2,v1:3;n:type:ShaderForge.SFN_Fresnel,id:6559,x:32067,y:32493,varname:node_6559,prsc:2|EXP-7928-OUT;n:type:ShaderForge.SFN_Multiply,id:7110,x:32245,y:32681,varname:node_7110,prsc:2|A-6559-OUT,B-8124-RGB;n:type:ShaderForge.SFN_Multiply,id:8382,x:32636,y:32495,varname:node_8382,prsc:2|A-6074-B,B-9248-OUT;n:type:ShaderForge.SFN_Multiply,id:2058,x:32430,y:32703,varname:node_2058,prsc:2|A-7110-OUT,B-7241-OUT;n:type:ShaderForge.SFN_Vector1,id:7241,x:32236,y:32820,varname:node_7241,prsc:2,v1:4;n:type:ShaderForge.SFN_Vector1,id:7928,x:31809,y:32590,varname:node_7928,prsc:2,v1:1.2;n:type:ShaderForge.SFN_Color,id:8124,x:31807,y:32805,ptovrint:False,ptlb:Color,ptin:_Color,varname:_node_4198_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:8739,x:32577,y:32787,varname:node_8739,prsc:2|A-2058-OUT,B-8124-A;n:type:ShaderForge.SFN_VertexColor,id:2821,x:32236,y:32952,varname:node_2821,prsc:2;n:type:ShaderForge.SFN_Multiply,id:1817,x:32735,y:33001,varname:node_1817,prsc:2|A-8739-OUT,B-5288-OUT;n:type:ShaderForge.SFN_Multiply,id:5288,x:32481,y:32952,varname:node_5288,prsc:2|A-2821-RGB,B-2821-A;proporder:6074-8124;pass:END;sub:END;*/

Shader "VR/sword" {
    Properties {
        _MainTex ("MainTex", 2D) = "bump" {}
        _Color ("Color", Color) = (1,1,1,1)
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
            #pragma exclude_renderers d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _Color;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 emissive = ((_MainTex_var.b*3.0)*((((pow(1.0-max(0,dot(normalDirection, viewDirection)),1.2)*_Color.rgb)*4.0)*_Color.a)*(i.vertexColor.rgb*i.vertexColor.a)));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG_COLOR(i.fogCoord, finalRGBA, fixed4(0,0,0,1));
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
