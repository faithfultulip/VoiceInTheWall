Shader "Screen Ripple/Unlit Tex" {
	Properties {
		_MainTex ("Main", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		Pass {
			Cull Off
			
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			sampler2D _MainTex;
			float4 _MainTex_ST;

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 tex : TEXCOORD0;
			};
			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.tex = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}
			float4 frag (v2f i) : SV_Target
			{
				return tex2D(_MainTex, i.tex);
			}
			ENDCG
		}
	}
	FallBack "VertexLit"
}
