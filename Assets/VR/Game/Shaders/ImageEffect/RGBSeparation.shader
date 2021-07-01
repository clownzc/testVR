Shader "VR/RGBSeparation"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Separation_X ("Separation_X", Range(0,1)) = 0.02
		_Separation_Y ("Separation_Y", Range(0,1)) = 0.01
    }

    SubShader
    {
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            Fog { Mode off }
            
            CGPROGRAM

            #pragma vertex vert_img
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest 
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed _Separation_X;
			fixed _Separation_Y;

            fixed4 frag(v2f_img i):COLOR
            {
//				fixed r = tex2D(_MainTex, i.uv - fixed2(_Separation_X, _Separation_Y)).r;
//				fixed g = tex2D(_MainTex, i.uv).g;
//				fixed b = tex2D(_MainTex, i.uv + fixed2(_Separation_X, _Separation_Y)).b;
//
//				fixed4 c = fixed4(r,g,b,1);
				fixed4 c = tex2D(_MainTex, i.uv);
				c += tex2D(_MainTex, i.uv - fixed2(_Separation_X, _Separation_Y));
				c += tex2D(_MainTex, i.uv + fixed2(_Separation_X, _Separation_Y));
                return c / 3;
            }

            ENDCG
        }
    }

    FallBack off
}