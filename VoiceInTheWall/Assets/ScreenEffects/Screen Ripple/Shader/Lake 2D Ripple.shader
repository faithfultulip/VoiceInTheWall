Shader "Screen Ripple/Lake 2D Ripple" {
	Properties {
		_Color      ("Color", Color) = (1, 1, 1, 1)
		_MainTex    ("Normalmap", 2D) = "bump" {}
		_Distortion ("Magnitude", Float) = 0.05
	}
	Category {
		Tags { "Queue"="Transparent" }
		SubShader {
			GrabPass { "_BackgroundTex" }
			
			Blend SrcAlpha OneMinusSrcAlpha
			Pass {
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				struct v2f {
					float4 vertex : SV_POSITION;
					float4 uvgrab : TEXCOORD0;
					float2 uvmain : TEXCOORD1;
				};
				fixed4 _Color;
				sampler2D _MainTex, _BackgroundTex;
				float4 _MainTex_ST;
				float  _Distortion;

				v2f vert (appdata_base v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
#if UNITY_UV_STARTS_AT_TOP
					float scale = -1.0;
#else
					float scale = 1.0;
#endif
					o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y * scale) + o.vertex.w) * 0.5;
					o.uvgrab.zw = o.vertex.zw;
					o.uvmain = TRANSFORM_TEX(v.texcoord, _MainTex);
					return o;
				}
				fixed4 frag (v2f i) : SV_Target
				{
					half2 bump = UnpackNormal(tex2D(_MainTex, i.uvmain)).rg;
					i.uvgrab.xy += bump * _Distortion;

					fixed4 c = tex2Dproj(_BackgroundTex, UNITY_PROJ_COORD(i.uvgrab));
					c *= _Color;
					return c;
				}
				ENDCG
			}
		}
	}
	FallBack "Particle/AlphaBlended"
}
