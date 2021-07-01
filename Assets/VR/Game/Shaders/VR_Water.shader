// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "VR/VR_Water" {
    Properties {
        _Normal_01 ("Normal_01", 2D) = "bump" {}
        _Uspeed_01 ("Uspeed_01", Range(0,2)) = 1
        _Vspeed_01 ("Vspeed_01", Range(0,2)) = 0
        _Normal_02 ("Normal_02", 2D) = "bump" {}
        _Vspeed_02 ("Vspeed_02", Range(0,2)) = 0.5
        _Uspeed_02 ("Uspeed_02", Range(0,2)) = 1
		_Normal_Inten("Normal_Inten", Range(0, 5)) = 3

        _Mask_Texture ("Mask_Texture", 2D) = "white" {}
		_Alpha_Inten("Alpha_Inten", Range(-1, 5)) = 0

		_Specular_Tex("Specular_Map", 2D) = "white" {}
        _FresnelColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
		_Specular_Inten("Specular_Inten", Range(0,3)) = 1
        _Fresnel_Edge ("Specular_Edge", Range(0.01, 10)) = 0.01

        _Muddy_Color ("Muddy_Color", Color) = (0,0,0,1)
		_Alpha("Muddy Degree", Range(0, 1)) = 0.5

		_DiffuseColor("Diffuse Color", Color) = (0.5,0.5,0.5,1)
		_Diffuse_Alpha("Diffuse_Alpha", Float) = 0
        _Camera_Range ("Camera_Range", Float ) = 0
        _range_constrast ("range_constrast", Float ) = 0
		
        _River_Bed_Map ("River_Bed_Map", 2D) = "white" {}
		_River_bed_Color("River_bed_Color", Color) = (0.1,0.01,0.5,1)
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent-1"
            "RenderType"="Transparent"
        }
        LOD 200
        Pass {
            Name "FORWARD"
			//
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fog

            sampler2D _Normal_01; half4 _Normal_01_ST;
            half _Uspeed_01;
            half _Vspeed_01;
            sampler2D _Normal_02; half4 _Normal_02_ST;
            half _Vspeed_02;
            half _Uspeed_02;
            sampler2D _River_Bed_Map; half4 _River_Bed_Map_ST;
            half4 _River_bed_Color;
            half _Alpha;
            half _Normal_Inten;
            half4 _DiffuseColor;
            half4 _FresnelColor;
            half _Fresnel_Edge;
            half4 _Muddy_Color;
            sampler2D _Specular_Tex; half4 _Specular_Tex_ST;
            half _Camera_Range;
            half _range_constrast;
            half _Alpha_Inten;
            half _Specular_Inten;
            sampler2D _Mask_Texture; half4 _Mask_Texture_ST;
            half _Diffuse_Alpha;

			// Formula
			half2 GetUV(half2 uv, half time, half uspeed, half vspeed)
			{
				return uv + half2((time * uspeed), time * vspeed);
			}

            struct a2v {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(5)
            };
            v2f vert (a2v v) {
                v2f o;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(v2f i) : SV_Target {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
				half3 Disappear_Dis = _WorldSpaceCameraPos.xyz - i.posWorld.xyz;
                half3 viewDirection = normalize(Disappear_Dis);
                half3 normalDirection = i.normalDir;
////// Emissive:
				//------------------------Planner Two Normal-----------------------------------
				// Two Normal Map
                half2 uv_01 = GetUV(i.uv0, _Time.r, _Uspeed_01, _Vspeed_01);
				half2 uv_02 = GetUV(i.uv0, _Time.r, _Uspeed_02, _Vspeed_02);
                half3 _NormalMap_01 = UnpackNormal(tex2D(_Normal_01,TRANSFORM_TEX(uv_01, _Normal_01)));                
                half3 _NormalMap_02 = UnpackNormal(tex2D(_Normal_02,TRANSFORM_TEX(uv_02, _Normal_02)));
				// Combine Normalize Normal Map
                half3 Blend_Nor = _NormalMap_01.rgb*_NormalMap_02.rgb;
                half3 Normalize_Nor = normalize(float3((Blend_Nor.rg*_Normal_Inten), Blend_Nor.b));
				//-----------------------------------------------------------------------------

				//------------------------Reflection information--------------------------------
				// Reflection uv
                half2 Ref_uv = ((reflect(viewDirection,Normalize_Nor).rb+half2(1,1))*0.5);
				// Reflection Color
				half4 Ref_Color = tex2D(_Specular_Tex, TRANSFORM_TEX(Ref_uv, _Specular_Tex));
				//------------------------------------------------------------------------------

				// Distort uv
                half2 Distort_uv = ((Blend_Nor.rg*0.08)+i.uv0);
				// River Bed Color
                half4 Bed_Color = tex2D(_River_Bed_Map, TRANSFORM_TEX(Distort_uv, _River_Bed_Map));

				//-------------------------Fresnel_Color----------------------------------------
				// Fresnel_Color
                half Disappear_f = pow(min(1.0,(length(Disappear_Dis)/_Camera_Range)),_range_constrast);
                half3 Fresnel_Color = (Disappear_f * _DiffuseColor.rgb);
				//------------------------------------------------------------------------------

				// Combine
                half3 finalColor = (((_FresnelColor.rgb*pow((1.0 - abs(dot(viewDirection,mul(Normalize_Nor, tangentTransform ).xyz.rgb))),_Fresnel_Edge)) * Ref_Color.rgb * _Specular_Inten) + lerp(lerp((Bed_Color.rgb*_River_bed_Color.rgb),_Muddy_Color.rgb,_Alpha), Fresnel_Color ,(Disappear_f*_Diffuse_Alpha)));
                half4 Mask_Tex = tex2D(_Mask_Texture,TRANSFORM_TEX(i.uv0, _Mask_Texture));
                fixed4 finalRGBA = fixed4(finalColor,(((Disappear_f * Mask_Tex.r)+_Alpha_Inten) * i.vertexColor.a));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "VRwaterGUI"
}
