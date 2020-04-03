Shader "Screen Ripple/Ripple Water Distortion" {
	Properties {
		_MainTex    ("Main", 2D) = "white" {}
		_NormalTex  ("Normal", 2D) = "black" {}
		_ReliefTex  ("Relief", 2D) = "black" {}
		_Relief     ("Relief Intensity", Float) = 1.5
		_Darkness   ("Darkness", Float) = 10.0
		_Strength   ("Distortion Strength", Float) = 50.0
		_Color      ("Color", Color) = (1, 1, 1, 1)
	}
	SubShader {
		Pass {
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _NormalTex, _MainTex, _ReliefTex;
			half4 _MainTex_TexelSize;
			fixed4 _Color;
		 	half _Strength, _Relief, _Darkness;

			fixed4 frag (v2f_img i) : SV_Target
			{
				fixed4 relf = tex2D(_ReliefTex, i.uv);
				relf.a = saturate(relf.r * relf.g * relf.b);
				
				fixed2 norm = UnpackNormal(tex2D(_NormalTex, i.uv)).rg;
				float2 uv = i.uv;
				uv -= _Strength * _MainTex_TexelSize.x * norm.rg;

				fixed4 c = tex2D(_MainTex, uv);
				c.rgb += (relf.rgb * _Color.rgb) * _Color.a;
				c.rgb *= saturate(1.0 - _Darkness * _Color.a * relf.a);
				c.rgb *= (1.0 - norm.r * _Relief);
				return c;
			}
			ENDCG
		}
	}
}
