Shader "Screen Ripple/Ripple Click 3" {
	Properties {
		_MainTex ("Main", 2D) = "white" {}
	}
	CGINCLUDE
	#include "UnityCG.cginc"
	sampler2D _MainTex, _MaskTex;
	float4 _MainTex_TexelSize;
	float _Frequence, _Speed, _Strength, _WaveWidth, _Spread;
	float4 _Ripple1, _Ripple2, _Ripple3;  // center.xy, progress

	float getDisp (in float2 uv, in float4 ripple)
	{
		float2 dv = ripple.xy - uv;
		dv *= float2(_ScreenParams.x / _ScreenParams.y, 1.0);
		float len = length(dv);
		float s = sin(len * _Frequence + _Time.y * _Speed) * _Strength * 0.01;
        float d = clamp(_WaveWidth - abs(ripple.z - len), 0.0, 1.0) / _WaveWidth;
        float2 offset = normalize(dv) * s * d;
		float decay = 1.0 - smoothstep(0.0, _Spread, len);
        return offset * decay;
	}
	fixed4 frag (v2f_img i) : SV_Target
	{
		float2 uv = i.uv;
		uv += getDisp (uv, _Ripple1);
		uv += getDisp (uv, _Ripple2);
		uv += getDisp (uv, _Ripple3);
#ifdef SR_MASK
		float4 mask = tex2D(_MaskTex, i.uv);
		float4 ripple = tex2D(_MainTex, uv);
		float4 orig = tex2D(_MainTex, i.uv);
		return lerp(ripple, orig, mask.r);
#else
		return tex2D(_MainTex, uv);
#endif
    }  
	ENDCG
	SubShader {
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
			#pragma multi_compile _ SR_MASK
            ENDCG
        }
    }
}