// Simplified Diffuse shader. Differences from regular Diffuse one:
// - no Main Color
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "VR/Mobile DiffuseVerBlend" {
Properties {
	_Layer1 ("Layer_01", 2D) = "white" {}
	_Layer1_Tint ("Layer01_Tint", Color) = (0.5,0.5,0.5,1)
	_Layer2 ("Layer_02", 2D) = "white" {}
	_Layer2_Tint ("Layer02_Tint", Color) = (0.5,0.5,0.5,1)
	_Layer3 ("Layer_02", 2D) = "white" {}
	_Layer3_Tint ("Layer03_Tint", Color) = (0.5,0.5,0.5,1)
	_Layer4 ("Layer_02", 2D) = "white" {}
	_Layer4_Tint ("Layer04_Tint", Color) = (0.5,0.5,0.5,1)
	_Weight("Blend Weight" , Range(0.001,1)) = 0.2

}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 150

CGPROGRAM
#pragma surface surf Lambert

sampler2D _Layer1;
sampler2D _Layer2;
sampler2D _Layer3;
sampler2D _Layer4;
fixed4 _Layer1_Tint;
fixed4 _Layer2_Tint;
fixed4 _Layer3_Tint;
fixed4 _Layer4_Tint;
float _Weight;

struct Input {
	half2 uv_Layer1;
	half2 uv_Layer2;
	half2 uv_Layer3;
	half2 uv_Layer4;
	fixed4 vertColor : COLOR;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 Layer1 = tex2D(_Layer1, IN.uv_Layer1) * _Layer1_Tint;
	fixed4 Layer2 = tex2D(_Layer2, IN.uv_Layer2) * _Layer2_Tint;
	fixed4 Layer3 = tex2D(_Layer3, IN.uv_Layer3) * _Layer3_Tint;
	fixed4 Layer4 = tex2D(_Layer4, IN.uv_Layer4) * _Layer4_Tint;

//	half4 blend = Blend(Layer1.a, Layer2.a, Layer3.a, Layer4.a, IN.vertColor, _Weight);
	half4 blend;
	half vertColorA = 1 - saturate(IN.vertColor.r + IN.vertColor.g + IN.vertColor.b);

	blend.r = Layer1.a * IN.vertColor.r;
	blend.g = Layer2.a * IN.vertColor.g;
	blend.b = Layer3.a * IN.vertColor.b;
	blend.a = Layer4.a * vertColorA;

	half ma = max(blend.r, max(blend.g, max(blend.b, blend.a)));
	blend.rgb = max(blend - ma + _Weight, 0) * IN.vertColor.rgb;
	blend.a = max(blend.a - ma + _Weight, 0) * vertColorA;
	blend = (Layer1 * blend.r + Layer2 * blend.g + Layer3 * blend.b + Layer4 * blend.a) / (blend.r + blend.g + blend.b + blend.a);
	//o.Albedo = lerp((Layer1.rgb * IN.vertColor.r + Layer2.rgb * IN.vertColor.g + Layer3.rgb * IN.vertColor.b), Layer4.rgb, (1 - IN.vertColor.a));
	//o.Albedo = lerp((Layer1.rgb * IN.vertColor.r * Layer1.a+ Layer2.rgb * IN.vertColor.g * Layer2.a+ Layer3.rgb * IN.vertColor.b * Layer3.a), Layer4.rgb * Layer4.a, (1 - IN.vertColor.a));
	//o.Albedo = lerp((Layer1.rgb * blend.r + Layer2.rgb * blend.g + Layer3.rgb * blend.b), Layer4.rgb, (1 - blend.a));
	//o.Albedo = Layer1.rgb * blend.r + Layer2.rgb * blend.g + Layer3.rgb * blend.b + Layer4.rgb * blend.a;
	o.Albedo = blend;
	o.Alpha = 0;
}
ENDCG
}

Fallback "Mobile/VertexLit"
}
