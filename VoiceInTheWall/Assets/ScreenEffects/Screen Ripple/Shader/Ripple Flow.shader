Shader "Screen Ripple/Ripple Flow" {
	Properties {
		_MainTex ("Main", 2D) = "white" {}
		_SplashTex ("Splash", 2D) = "white" {}
		_SizeX ("SizeX", Float) = 1.0
		_SizeY ("SizeY", Float) = 1.0
		_Speed ("Speed", Float) = 1.0
		_Distortion ("Distortion", Float) = 0.87
		_Overlay ("Overlay", Color) = (1, 1, 1, 1)
	}
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex, _SplashTex;
			float _Speed, _SizeX, _SizeY, _Distortion;
			float2 _MainTex_TexelSize;
			fixed4 _Overlay;
			
			fixed4 frag (v2f_img i) : SV_Target
			{
				float2 uv = i.uv;
#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					_Speed = 1 - _Speed;
#endif
				float t = _Time.y * _Speed;
				float3 c1 = tex2D(_SplashTex, float2(uv.x * 1.15 * _SizeX,       (uv.y * _SizeY * 1.1) + t * 0.15)).rgb / _Distortion;
				float3 c2 = tex2D(_SplashTex, float2(uv.x * 1.25 * _SizeX - 0.1, (uv.y * _SizeY * 1.2) + t * 0.2)).rgb / _Distortion;
				float3 c3 = tex2D(_SplashTex, float2(uv.x * _SizeX * 0.9,        (uv.y * _SizeY * 1.25) + t * 0.032)).rgb / _Distortion;
//				float2 offset = 2.0 * ((c1.xy + c2.xy + c3.xy) / 3.0) - 1.0;
				float2 offset = (c1.xy - c2.xy - c3.xy) / 3.0;
				float2 finalUv = uv + offset;
				return fixed4(tex2D(_MainTex, finalUv).rgb * _Overlay, 1.0);
			}
			ENDCG
		}
	}
}