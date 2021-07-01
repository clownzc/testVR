// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/OverdrawDebugReplacement" 
{
	SubShader 
	{
		Tags {"RenderType" = "Opaque"}
		LOD 100
		
		Pass
		{
			Blend One One
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			half4 vert(half4 vertex : POSITION) : SV_POSITION
			{
				return UnityObjectToClipPos(vertex);
			}
			
			fixed4 frag(half4 vertex : SV_POSITION) : COLOR
			{
				return fixed4(0.1, 0, 0, 1.0);
			}
			ENDCG
		}
	} 
}