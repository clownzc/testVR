Shader "VR/skybox_bake" {
Properties {
	_Tint ("Tint Color", Color) = (.5, .5, .5, .5)
	[Gamma] _Exposure ("Exposure", Range(0, 8)) = 1.0
	_Rotation ("Rotation", Range(0, 360)) = 0
	[NoScaleOffset] _Tex ("Cubemap   (HDR)", Cube) = "grey" {}
}

SubShader {
	Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
	Cull Off ZWrite On

	Pass {
		
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

		samplerCUBE _Tex;
		samplerCUBE _Exposuremap;
		half4 _Tex_HDR;
		half4 _Tint;
		half _Exposure;
		float _Rotation;

		float4 RotateAroundYInDegrees (float4 vertex, float degrees)
		{
			float alpha = degrees * UNITY_PI / 180.0;
			float sina, cosa;
			sincos(alpha, sina, cosa);
			float2x2 m = float2x2(cosa, -sina, sina, cosa);
			return float4(mul(m, vertex.xz), vertex.yw).xzyw;
		}
		
		struct appdata_t {
			float4 vertex : POSITION;
		};

		struct v2f {
			float4 vertex : SV_POSITION;
			float3 texcoord : TEXCOORD0;
		};

		v2f vert (appdata_t v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(RotateAroundYInDegrees(v.vertex, _Rotation));
			o.texcoord = v.vertex.xyz;
			return o;
		}

		fixed4 frag (v2f i) : SV_Target
		{
			half4 c = texCUBE (_Tex, i.texcoord);
			c.a *= _Exposure;
			c.rgb = DecodeHDR (c, _Tex_HDR);
			c.rgb = c.rgb * _Tint.rgb * (1 + c.a) * unity_ColorSpaceDouble.rgb;
//			c *= _Exposure;
			return half4(c.rgb, 1);
		}
		ENDCG 
	}
} 	


Fallback Off

}
