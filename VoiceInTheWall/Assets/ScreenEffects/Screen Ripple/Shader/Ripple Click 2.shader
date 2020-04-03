Shader "Screen Ripple/Ripple Click 2" {
	Properties {
		_MainTex   ("Main", 2D) = "white" {}
		_MaskTex   ("Mask", 2D) = "black" {}
		_Spread    ("Spread", Float) = 0.3
		_Frequence ("Frequence", Float) = 12
		_Velocity  ("Velocity", Float) = 4
		_Ripple1   ("Ripple 1", Vector) = (0.5, 0.5, 0, 0)
		_Ripple2   ("Ripple 2", Vector) = (0.5, 0.5, 0, 0)
		_Ripple3   ("Ripple 3", Vector) = (0.5, 0.5, 0, 0)
	}
	CGINCLUDE
	#include "UnityCG.cginc"
	sampler2D _MainTex, _MaskTex;
	float _Spread, _Frequence, _Velocity;
	float4 _Ripple1, _Ripple2, _Ripple3;  // center.xy, progress, strength

	float getDisp (in float2 uv, in float4 ripple)
	{
		float ratio = (_ScreenParams.x / _ScreenParams.y);
		float2 normUv = float2(uv.x * ratio, uv.y);
		float2 normRippleUv = float2(ripple.x * ratio, ripple.y);
	
		float2 p = normRippleUv - normUv;
		float len = length(p);
		float2 disp = (p / len) * cos(len * _Frequence - ripple.z * _Velocity) * ripple.w;
		float decay = 1.0 - smoothstep(0, _Spread, len);
		return disp * decay;
	}
	float4 frag (v2f_img i) : SV_Target
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
