Shader "Screen Ripple/Droplet" {
	Properties {
		_BumpTex  ("Bump", 2D) = "bump" {}
		_Strength ("Strength", Range(0, 1)) = 0.8
	}
	SubShader {
		Tags { "Queue"="Transparent+1" "RenderType"="Transparent" }
		
		// as unity doc says, "grabpass will only do that once per frame for the first object that uses the given texture name."
		// see here: https://docs.unity3d.com/Manual/SL-GrabPass.html
		GrabPass { "_BackgroundTex" }
		
		Blend SrcAlpha OneMinusSrcAlpha
		Zwrite Off
		ZTest Always
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _BackgroundTex, _BumpTex;
			fixed _Strength;

			struct v2f
			{
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;
				float4 scrpos : TEXCOORD0;
				float2 uv : TEXCOORD1;
			};
			v2f vert (appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.scrpos = ComputeGrabScreenPos(o.pos);
				o.color = v.color;
				o.uv = v.texcoord;
				return o;
			}
			fixed4 frag (v2f i) : SV_Target
			{
				float3 bump = UnpackNormal(tex2D(_BumpTex, i.uv));
				float4 scrpos = i.scrpos;
				scrpos.xy += bump.xy / bump.z * _Strength * i.color.a;
				return tex2Dproj(_BackgroundTex, scrpos);
			}
			ENDCG
		}
	}
	FallBack "Particle/AlphaBlended"
}
