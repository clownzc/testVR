// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "VR/VR CubeUI" {
    Properties {
        _Cubemap ("Cubemap", Cube) = "_Skybox" {}
        _Power ("Edge Power", Float ) = 1
		_Add ("Edge Add", Float) = 1
		_Color ("Edge Color", Color ) = (0.5,0.5,0.5,1)
		_FresnelColor ("Fresnel Color", Color) = (0.2,0.2,0.2,1)
		_Edgethickness ("Edge Thickness", Range(0,1)) = 0.1
		[MaterialToggle] _Bleach ("Bleach Color", Float ) = 0.3215686
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
        }

        LOD 200

		Pass {
            Name "FORWARD"
            Tags {
				"LightMode" = "ForwardBase"
            }
            ZWrite On
            
			Cull Front
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fog
            #pragma target 3.0
            samplerCUBE _Cubemap;
            uniform fixed _Power;
			fixed _Add;
			fixed _Bleach;
			fixed4 _FresnelColor;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
				float3 viewDir : TEXCOORD2;
				float3 reflectDir : TEXCOORD3;
                UNITY_FOG_COORDS(4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				o.viewDir = normalize(_WorldSpaceCameraPos.xyz - o.posWorld.xyz);

				float3 normalDirection = normalize(o.normalDir);
				o.reflectDir = reflect(o.viewDir, normalDirection);

                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                half3 normalDirection = normalize(i.normalDir);

                fixed3 emissive = texCUBE(_Cubemap,i.reflectDir).rgb;
				fixed4 finalRGBA = fixed4(lerp(_FresnelColor, emissive.rgb, pow(saturate(dot(i.normalDir, -i.viewDir) + _Add), _Power)),1);
				finalRGBA.rgb = lerp(finalRGBA.rgb, dot(finalRGBA.rgb, half3(0.3, 0.59, 0.11)), _Bleach);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }

        Pass {
            Name "Outline"
            Tags {
            }
            Cull front
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fog
            #pragma target 3.0
			fixed4 _Color;
			fixed _Edgethickness;
			fixed _Bleach;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                UNITY_FOG_COORDS(0)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos(half4(v.vertex.xyz + v.normal*_Edgethickness,1) );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target{
                return float4(lerp(_Color.rgb, dot(_Color.rgb, half3(0.3, 0.59, 0.11)), _Bleach), _Color.a);
            }
            ENDCG
        }
        
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
