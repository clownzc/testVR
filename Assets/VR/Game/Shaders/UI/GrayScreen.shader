// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "VR/Gray Screen"
{
	SubShader
	{
		// Draw ourselves after all opaque geometry
		Tags{ "Queue" = "Transparent-1" "RenderType" = "Transparent"}

		// Grab the screen behind the object into _BackgroundTexture
		GrabPass
		{
			"_BackgroundTexture"
		}

		// Render the object with the texture generated above, and invert the colors
		Pass
		{
			Cull Front
			ZWrite Off
			//ZTest Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 grabPos : TEXCOORD0;
				float4 pos : SV_POSITION;
			};

			v2f vert(appdata_base v) {
				v2f o;
				//v.vertex.y = clamp(v.vertex.y, 0, 1);
				o.pos = UnityObjectToClipPos(v.vertex);
				// use ComputeGrabScreenPos function from UnityCG.cginc
				// to get the correct texture coordinate
				o.grabPos = ComputeGrabScreenPos(o.pos);
				return o;
			}

			sampler2D _BackgroundTexture;

			half4 frag(v2f i) : SV_Target
			{
				half4 bgcolor = tex2Dproj(_BackgroundTexture, i.grabPos);
				half gray = (bgcolor.r + bgcolor.g + bgcolor.b) / 3;
				return half4(gray, gray, gray, bgcolor.a);
				//return bgcolor.rrra;
				//return half4(1, 1, 1, 1);
			}
			ENDCG
		}

	}
}