Shader "VR/Unlit Transparent" {
	Properties{
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		_Tint("Tint", Color) = (1,1,1,1)

		//Fog
		[Enum(OFF,0,DAYDREAM FOG,1)] _FogMode("Fog Mode", Float) = 1
	}

		SubShader{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"

			#pragma multi_compile _ _DAYDREAM_FOG _DAYDREAM_HEIGHT_FOG
			sampler2D _MainTex;
			float4 _MainTex_ST;
			half4 _Tint;

			half _FogMode;
			half  _fogMode;
			half4 _fogLinear;
			half4 _fogHeight;
			half4 _fogNearColor;
			half4 _fogFarColor;

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f {
				half4 pos : SV_POSITION;
				half2 uv0 : TEXCOORD0;
				half3 worldPos : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			half4 computeFog(half3 posWorld)
			{
				half4 fogValue;
				half3 relPos = posWorld.xyz - _WorldSpaceCameraPos.xyz;	//3
				half  sceneDepth = length(relPos);
#if defined(_DAYDREAM_HEIGHT_FOG)
				half3 dir = relPos / sceneDepth;						//4 (or 2 if dir.xy is unused later)
				half dH = _fogHeight.y; //dH = maxY - minY
				half test = saturate((posWorld.y - _fogHeight.y) / (_fogHeight.x - _fogHeight.y));
				half L = max(0.0, test * 3.5 + sceneDepth * 0.01 * _fogHeight.z);

				//half L = dh / abs(dir.y);
#else
				//the camera is considered to be inside an infinite volume of fog so only the distance from the camera to the surface is important.
				half L = sceneDepth;
#endif

				half Lw;
				if (_fogMode == 0.0)
				{
					Lw = saturate(L*_fogLinear.x + _fogLinear.y);	//2
					fogValue.w = Lw;
				}
				else if (_fogMode == 1.0)
				{
					Lw = L*_fogHeight.w;							//8
					fogValue.w = 1.0 - exp(-Lw);
				}
				else if (_fogMode == 2.0)
				{
					Lw = L*_fogHeight.w;							//9
					half e = exp(-Lw);
					fogValue.w = 1.0 - e*e;
				}
				half colorFactor = smoothstep(0.0, 1.0, Lw * _fogLinear.w);
				fogValue.rgb = lerp(_fogNearColor.rgb, _fogFarColor.rgb, colorFactor);
				fogValue.w *= _fogLinear.z;

				return fogValue;
			}

			v2f vert(appdata_t v)
			{
				v2f o;
				o.uv0 = v.texcoord;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 finalColor = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
				fixed4 finalRGBA = finalColor * _Tint;
				#if defined(_DAYDREAM_FOG) || defined(_DAYDREAM_HEIGHT_FOG)
					half4 fogValue = computeFog(i.worldPos);
					finalRGBA.rgb = lerp(finalRGBA.rgb, lerp(finalRGBA.rgb, fogValue.rgb, saturate(fogValue.w)), _FogMode);
					finalRGBA.a = lerp(finalRGBA.a, 1.0 - (1.0 - finalRGBA.a) * max(1.0 - fogValue.w, 0.0), _FogMode);
				#endif
				return finalRGBA;
			}
			ENDCG
		}
	}

}
