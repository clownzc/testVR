// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.16 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.16;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:3138,x:32719,y:32712,varname:node_3138,prsc:2|emission-1872-OUT;n:type:ShaderForge.SFN_SceneColor,id:7021,x:32383,y:32870,varname:node_7021,prsc:2|UVIN-9070-OUT;n:type:ShaderForge.SFN_ScreenPos,id:6252,x:31769,y:32871,varname:node_6252,prsc:2,sctp:2;n:type:ShaderForge.SFN_Tex2d,id:2192,x:31758,y:33087,ptovrint:False,ptlb:distortion_tex,ptin:_distortion_tex,varname:node_2192,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:2,isnm:False;n:type:ShaderForge.SFN_ComponentMask,id:2236,x:31946,y:33087,varname:node_2236,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-2192-RGB;n:type:ShaderForge.SFN_Lerp,id:2444,x:32326,y:33187,varname:node_2444,prsc:2|A-762-OUT,B-2236-OUT,T-9714-OUT;n:type:ShaderForge.SFN_Vector2,id:762,x:31959,y:33237,varname:node_762,prsc:2,v1:0,v2:0;n:type:ShaderForge.SFN_Slider,id:9714,x:31599,y:33430,ptovrint:False,ptlb:distortion_intensity,ptin:_distortion_intensity,varname:node_9714,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Add,id:9070,x:32144,y:32881,varname:node_9070,prsc:2|A-6252-UVOUT,B-2444-OUT;n:type:ShaderForge.SFN_Fresnel,id:75,x:31769,y:32350,varname:node_75,prsc:2;n:type:ShaderForge.SFN_Add,id:1872,x:32502,y:32665,varname:node_1872,prsc:2|A-1584-OUT,B-7021-RGB;n:type:ShaderForge.SFN_Color,id:9105,x:31769,y:32495,ptovrint:False,ptlb:rimcolor,ptin:_rimcolor,varname:node_9105,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Multiply,id:1584,x:32290,y:32511,varname:node_1584,prsc:2|A-75-OUT,B-9105-RGB,C-2429-RGB,D-7589-OUT;n:type:ShaderForge.SFN_Tex2d,id:2429,x:31769,y:32668,ptovrint:False,ptlb:rim_tex,ptin:_rim_tex,varname:node_2429,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Vector1,id:7589,x:32290,y:32704,varname:node_7589,prsc:2,v1:2;proporder:2192-9714-9105-2429;pass:END;sub:END;*/

Shader "VR/distortion" {
    Properties {
        _distortion_tex ("distortion_tex", 2D) = "black" {}
        _distortion_intensity ("distortion_intensity", Range(0, 1)) = 0
        _rimcolor ("rimcolor", Color) = (0,0,0,1)
        _rim_tex ("rim_tex", 2D) = "white" {}

		[Enum(Off,0,Front,1,Back,2)]_cull_mode ("Cull Mode", Float) = 1
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
			Cull [_cull_mode]
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            uniform sampler2D _GrabTexture;
            uniform sampler2D _distortion_tex; uniform float4 _distortion_tex_ST;
            uniform float _distortion_intensity;
            uniform float4 _rimcolor;
            uniform sampler2D _rim_tex; uniform float4 _rim_tex_ST;
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
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
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
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5;
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float4 _rim_tex_var = tex2D(_rim_tex,TRANSFORM_TEX(i.uv0, _rim_tex));
                float4 _distortion_tex_var = tex2D(_distortion_tex,TRANSFORM_TEX(i.uv0, _distortion_tex));
                float3 emissive = (((1.0-max(0,dot(normalDirection, viewDirection)))*_rimcolor.rgb*_rim_tex_var.rgb*2.0)+tex2D( _GrabTexture, (sceneUVs.rg+lerp(float2(0,0),_distortion_tex_var.rgb.rg,_distortion_intensity))).rgb);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
