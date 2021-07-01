// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "VR/Totem_Dissolve" {
    Properties {
        _Mix_Texture ("Mix_Texture", 2D) = "white" {}
        _Dissolve ("Dissolve", Float ) = 2
        _Emissive_Color ("Emissive_Color", Color) = (1,0.8068966,0,1)
        _Emissive_Inten ("Emissive_Inten", Float ) = 1.2
        _Edge_Thickness ("Edge_Thickness", Float ) = 0
		_Mask_Map ("_Mask_Map", 2D) = "white" {}
        _Edge_Color ("Edge_Color", Color) = (0.5,0.5,0.5,1)
//      _Distort_Map ("_Distort_Map", 2D) = "white" {}
        _Intensity ("Intensity", Float ) = 2
//		_Glint_Map("_Glint_Map", 2D) = "white" {}
        _Alpha_Inten ("Alpha_Inten", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 200
        Pass {
            Name "ForwardBase"
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
            #pragma target 2.0
            sampler2D _Mix_Texture; half4 _Mix_Texture_ST;
			fixed _Dissolve;
            fixed _Edge_Thickness;
            fixed _Emissive_Inten;
            fixed4 _Emissive_Color;
            sampler2D _Mask_Map; half4 _Mask_Map_ST;
            fixed4 _Edge_Color;
//          sampler2D _Distort_Map; half4 _Distort_Map_ST;
            fixed _Intensity;
//          sampler2D _Glint_Map; half4 _Glint_Map_ST;
            fixed _Alpha_Inten;

			//--------------------------------DissolveCal---------------------------------------
			fixed DissolveCal(fixed a, fixed b, fixed c)
			{
				fixed d = pow(saturate((a - b) * 5.0), c);
				return d;
			}
			//----------------------------------------------------------------------------------

            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                #ifndef LIGHTMAP_OFF
                    float4 uvLM : TEXCOORD2;
                #else
                    float3 shLight : TEXCOORD2;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
/////// Vectors:
////// Lighting:
////// Emissive:

				//------------------------------Distortion map-------------------------------------------------------------------
                half2 uv1 = (i.uv0+ _Time.g * fixed2(0.06,0.08));
                fixed4 Distort_map = tex2D(_Mix_Texture,TRANSFORM_TEX(uv1, _Mix_Texture));
                fixed Distort_Inten = Distort_map.b * 0.015;  // solid 0.015
                half2 distort_uv = i.uv0 + half2(Distort_Inten , Distort_Inten);
				//-----------------------------------------------------------------------------------------------------------------

				fixed4 Mask = tex2D(_Mask_Map, TRANSFORM_TEX(distort_uv, _Mask_Map)); // R : Hard Edge G : Blur B : R G Combine

				//------------------------------Dissolve Cal--------------------------------------------------------------------
				fixed4 _Mix_Texture_var = tex2D(_Mix_Texture, TRANSFORM_TEX(distort_uv, _Mix_Texture));

                fixed Dissolve_Param = _Mix_Texture_var.g + 1.0;
				fixed Dissolve_Param_02 = _Dissolve - 0.1;
                fixed edge_thick = _Edge_Thickness + 1.0;
 
				fixed Dissolve_Alpha_01 = DissolveCal(Dissolve_Param, Dissolve_Param_02, edge_thick);
				fixed Dissolve_Alpha_02 = DissolveCal(Dissolve_Param, _Dissolve, edge_thick);
                fixed Dissolve_Alpha = pow(saturate(((Dissolve_Param - (_Dissolve-0.1)) * 5.0)), edge_thick);
				//---------------------------------------------------------------------------------------------------------------

				//------------------------------glint panner----------------------------------------
                half2 uv2 = (i.uv0+ _Time.g * fixed2(0.001,0.003));
                fixed4 glint = tex2D(_Mix_Texture,TRANSFORM_TEX(uv2, _Mix_Texture));
				//----------------------------------------------------------------------------------

                fixed3 emissive = lerp(((_Emissive_Inten * _Emissive_Color.rgb * Mask.g)+(Mask.r * _Emissive_Color.rgb)),(_Intensity * _Edge_Color.rgb),saturate(((Dissolve_Alpha_01 - Dissolve_Alpha_02) * glint.r * Mask.r * 2.0)));
                fixed3 finalColor = emissive;

				//------------------------------AlphaCal--------------------------------------------
                fixed AlphaCal = Mask.b * Dissolve_Alpha_01 * _Alpha_Inten;
                fixed4 finalRGBA = fixed4(finalColor, AlphaCal);
				//----------------------------------------------------------------------------------

                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
