// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.16 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.16;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:3138,x:32719,y:32712,varname:node_3138,prsc:2|emission-2944-OUT;n:type:ShaderForge.SFN_SceneColor,id:7021,x:32383,y:32870,varname:node_7021,prsc:2|UVIN-9070-OUT;n:type:ShaderForge.SFN_ScreenPos,id:6252,x:31506,y:32840,varname:node_6252,prsc:2,sctp:2;n:type:ShaderForge.SFN_Tex2d,id:2192,x:31634,y:33146,ptovrint:False,ptlb:distortion_tex,ptin:_distortion_tex,varname:node_2192,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_ComponentMask,id:2236,x:31946,y:33087,varname:node_2236,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-2192-RGB;n:type:ShaderForge.SFN_Lerp,id:2444,x:32326,y:33187,varname:node_2444,prsc:2|A-762-OUT,B-2236-OUT,T-9714-OUT;n:type:ShaderForge.SFN_Vector2,id:762,x:31983,y:33237,varname:node_762,prsc:2,v1:0,v2:0;n:type:ShaderForge.SFN_Slider,id:9714,x:31802,y:33379,ptovrint:False,ptlb:distortion_intensity,ptin:_distortion_intensity,varname:node_9714,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Add,id:9070,x:32144,y:32881,varname:node_9070,prsc:2|A-6252-UVOUT,B-2444-OUT;n:type:ShaderForge.SFN_Transform,id:2996,x:31768,y:32531,varname:node_2996,prsc:2,tffrom:2,tfto:0|IN-2192-RGB;n:type:ShaderForge.SFN_Fresnel,id:3295,x:32176,y:32469,varname:node_3295,prsc:2|NRM-2996-XYZ;n:type:ShaderForge.SFN_Color,id:1479,x:31943,y:32636,ptovrint:False,ptlb:rim_color,ptin:_rim_color,varname:node_1479,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Multiply,id:506,x:32389,y:32482,varname:node_506,prsc:2|A-3295-OUT,B-1479-RGB,C-1109-RGB,D-9452-OUT;n:type:ShaderForge.SFN_Add,id:2944,x:32579,y:32754,varname:node_2944,prsc:2|A-506-OUT,B-7021-RGB;n:type:ShaderForge.SFN_Tex2d,id:1109,x:31900,y:32810,ptovrint:False,ptlb:rim_tex,ptin:_rim_tex,varname:node_1109,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Vector1,id:9452,x:32389,y:32712,varname:node_9452,prsc:2,v1:2;proporder:2192-9714-1479-1109;pass:END;sub:END;*/

Shader "VR/distortion_normaltex" {
    Properties {
        _distortion_tex ("distortion_tex", 2D) = "bump" {}
        _distortion_intensity ("distortion_intensity", Range(0, 1)) = 0
        _rim_color ("rim_color", Color) = (0,0,0,1)
        _rim_tex ("rim_tex", 2D) = "white" {}
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
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform sampler2D _distortion_tex; uniform float4 _distortion_tex_ST;
            uniform float _distortion_intensity;
            uniform float4 _rim_color;
            uniform sampler2D _rim_tex; uniform float4 _rim_tex_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float4 screenPos : TEXCOORD5;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
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
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float3 _distortion_tex_var = UnpackNormal(tex2D(_distortion_tex,TRANSFORM_TEX(i.uv0, _distortion_tex)));
                float4 _rim_tex_var = tex2D(_rim_tex,TRANSFORM_TEX(i.uv0, _rim_tex));
                float3 emissive = (((1.0-max(0,dot(mul( _distortion_tex_var.rgb, tangentTransform ).xyz.rgb, viewDirection)))*_rim_color.rgb*_rim_tex_var.rgb*2.0)+tex2D( _GrabTexture, (sceneUVs.rg+lerp(float2(0,0),_distortion_tex_var.rgb.rg,_distortion_intensity))).rgb);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
