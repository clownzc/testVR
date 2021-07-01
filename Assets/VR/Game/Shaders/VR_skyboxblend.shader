Shader "VR/skyboxBlend" {
Properties {
//	_Tint ("Tint Color", Color) = (.5, .5, .5, .5)
//	[Gamma] _Exposure ("Exposure", Range(0, 8)) = 1.0
	_ZenithColor("ZenithColor", Color) = (0, 0, 0, 0)
	_HorizonColor("HorizonColor", Color) = (0, 0, 0, 0)

	_Sky_Tex("SkyTex", 2D) = "black" {}

	[Toggle]_BLEND_ABLE("Enable BlendTex", Int) = 1
	_MixTex("MixMap (R:Cloud G:Star)", 2D) = "black" {}
	
	_Cloud_Tint ("CloudTint", Color) = (.5, .5, .5, .5)
	_Speed("CloudSpeed", float) = 1.0
	_CloudTile("CloudXTile", float) = 1.0

	_BlendTile("Star_Tile", float) = 1.0
	_StarInten("StarInten", Range(0,8)) = 1.0

}

SubShader {
	Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
	Cull Off ZWrite On

	Pass {
		
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		//#pragma shader_feature _BLEND_ABLE_ON

		#include "UnityCG.cginc"

		sampler2D _Sky_Tex;
		half4 _ZenithColor;
		half4 _HorizonColor;

//		#ifdef _BLEND_ABLE_ON
		sampler _MixTex;
//		#endif	
		half _BlendTile;
		half _StarInten;

		half4 _Tint;
		sampler2D _Cloud;
		half4 _Cloud_Tint;

		half _Speed;
		half _CloudTile;

		struct appdata_t {
			float4 vertex : POSITION;
			float3 texcoord0 : TEXCOORD0;
			//float3 texcoord1 : TEXCOORD1;
			fixed4 Color : COLOR;
		};

		struct v2f {
			float4 vertex : SV_POSITION;
			float3 texcoord0 : TEXCOORD0;
			//float3 texcoord1 : TEXCOORD1;
			fixed4 vertColor : COLOR;
		};

		v2f vert (appdata_t v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex); //vertex rotate
			o.texcoord0 = v.texcoord0;
			//o.texcoord1 = v.texcoord1;
			o.vertColor = v.Color;
			return o;
		}

		fixed4 frag (v2f i) : SV_Target
		{
			half4 skytex = tex2D (_Sky_Tex, i.texcoord0);
			half4 Time = _Time;
			half2 uv_transform = half2((Time.r * 0.02), 0.0);
			half cloud = tex2D(_MixTex, uv_transform * half2(_Speed,1) + i.texcoord0 * half2(_CloudTile, 1)).r;

			half blend = tex2D(_MixTex, i.texcoord0 * _BlendTile).g;
			half mask = 1 - (i.texcoord0.g * i.texcoord0.g);
			half mask2 = mask * mask;
			half3 a = lerp(_ZenithColor, _HorizonColor, i.vertColor.a);

			a = a + (blend * _StarInten * (1 - cloud) + cloud * _Cloud_Tint) * (1 - i.vertColor.a) + skytex.rgb * skytex.a;
			return half4(a, 1);
		}
		ENDCG 
	}
} 	


Fallback Off

}
