Shader "Screen Ripple/Ripple 3" {
	Properties {
		_MainTex ("Main", 2D) = "white" {}
		_Level ("Level", Float) = 0.3
		_ReflectionLevel ("Reflection Level", Float) = 0.6
		_Amplitude ("Amplitude", Float) = 0.005
		_Velocity ("Velocity", Float) = 3
		_Overlay ("Overlay", Color) = (1, 1, 1, 1)
	}
	CGINCLUDE
	#include "UnityCG.cginc"
	sampler2D _MainTex;
	float _Level, _Amplitude, _Velocity, _ReflectionLevel;
	float4 _Overlay;

	float4 frag (v2f_img i) : SV_Target
	{
		float2 uv = i.uv;
		float h = 0.005 * cos(_Time.y * 3.0);

		float4 c;
		if (uv.y > _Level + h)
		{
			c = tex2D(_MainTex, uv);
		}
		else
		{
			float xoffset = _Amplitude * cos(_Time.y * _Velocity + 200.0 * uv.y);
			float yoffset = ((0.3 - uv.y) / 0.3) * 0.01 * (1.0 + cos(_Time.y * _Velocity + 50.0 * uv.y));
			float2 uvNew = float2(uv.x + xoffset, (_ReflectionLevel - uv.y + yoffset));
			c = tex2D(_MainTex, uvNew);
			c = lerp(c, _Overlay, 0.25);
		}
		return c;
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
}
