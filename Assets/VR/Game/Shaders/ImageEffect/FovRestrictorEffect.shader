// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "VR/FovRestrictorEffect"
{
	Properties
	{
		_MainTex ("Base", 2D) = "" {}
	}
	
	CGINCLUDE
	
	#include "UnityCG.cginc"
	
	struct v2f
	{
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
	};
	
	sampler2D _MainTex;
	float inCircleRadius;
	float outCircleRadius;
		
	v2f vert(appdata_img v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	} 
	
	half4 frag(v2f i) : COLOR
	{ 
		half dx = (i.uv.x - 0.5f);
		half dy = (i.uv.y - 0.5f);
		half dist = sqrt(dx * dx + dy * dy);
		half a = (dist - inCircleRadius) / (outCircleRadius - inCircleRadius);
		a = clamp(a, 0.0f, 1.0f);
		return lerp(tex2D(_MainTex, i.uv), half4(0.0f, 0.0f, 0.0f, a), a);
	}

	ENDCG 
	
	Subshader
	{
		Pass
		{
			ZTest Always Cull Off ZWrite Off
			Fog
			{
				Mode off
			}

			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest 
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}

	Fallback off	
} 