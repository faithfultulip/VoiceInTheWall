Shader "Screen Ripple/Ripple 4" {
	Properties {
		_MainTex   ("Main", 2D) = "white" {}
		_MaskTex   ("Mask", 2D) = "white" {}
		_Speed     ("Speed", Float) = 0.8
		_Frequency ("Frequency", Float) = 8
		_Amplitude ("Amplitude", Float) = 0.01
	}
	CGINCLUDE
	#include "UnityCG.cginc"
	sampler2D _MainTex, _MaskTex;
	float _Speed, _Frequency, _Amplitude;
	float2 shift (float2 p)
	{
		float d = _Time.y * _Speed;
		float2 f = _Frequency * (p + d);
		float2 q = cos(float2(
				cos(f.x - f.y) * cos(f.y),
				sin(f.x + f.y) * sin(f.y)));
		return q;
	}
	float4 frag (v2f_img i) : SV_Target
	{
		float2 p = shift(i.uv);
		float2 q = shift(i.uv + 1.0);
		float m = tex2D(_MaskTex, i.uv).r;
		float2 s = i.uv + (_Amplitude * (p - q)) * m;
		return tex2D(_MainTex, s);
	}
	ENDCG
	SubShader {
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            ENDCG
        }
	}
	FallBack Off
}
