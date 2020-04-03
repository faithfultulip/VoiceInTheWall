Shader "Screen Ripple/Droplet2" {
	Properties {
		[HideInInspector] _MainTex ("Main", 2D) = "white" {}
		_Size       ("Size", Float) = 8
		_Distortion ("Distortion", Float) = 1
		_Blur       ("Blur", Range(0, 8)) = 1
	}
	CGINCLUDE
	#include "UnityCG.cginc"
	sampler2D _MainTex, _Global_ScreenTex;
	float4 _MainTex_ST;
	float _Size, _Distortion, _Blur;

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
		float4 grabUv : TEXCOORD1;
	};
	v2f vert (appdata_base v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
		o.grabUv = UNITY_PROJ_COORD(ComputeGrabScreenPos(o.vertex));
		return o;
	}
	float N21 (float2 p)
	{
		p = frac(p * float2(123.34, 345.45));
		p += dot(p, p + 34.345);
		return frac(p.x * p.y);
	}
	float3 DropletLayer (float2 UV, float t)
	{
		float2 aspect = float2(2, 1);
		float2 uv = UV * _Size * aspect;
		uv.y += t * 0.25;
		float2 gv = frac(uv) - 0.5;
		
		float2 id = floor(uv);
		float n = N21(id);
		t += n * 6.286;

		float w = UV.y * 10;
		float x = (n - 0.5) * 0.8;
		x += (0.4 - abs(x)) * sin(3 * w) * pow(sin(w), 6) * 0.45;
		float y = -sin(t + sin(t + sin(t) * 0.5)) * 0.45;
		y -= (gv.x - x) * (gv.x - x);
		
		float2 dropPos =(gv - float2(x, y))/aspect;
		float drop = smoothstep(0.05, 0.03, length(dropPos));

		float2 dropTrailPos = (gv - float2(x, t * 0.25)) / aspect;
		dropTrailPos.y = (frac(dropTrailPos.y * 8) / 8) - 0.03;
		float dropTrail = smoothstep(0.03, 0.02, length(dropTrailPos));

		float fogTrail = smoothstep(-0.05, 0.05, dropPos.y);
		fogTrail *= smoothstep(0.5, y, gv.y);
		dropTrail *= fogTrail;
		fogTrail *= smoothstep(0.05, 0.04, abs(dropPos.x));

		float2 offs = drop * dropPos + dropTrail * dropTrailPos;
		return float3(offs, fogTrail);
	}
	fixed4 frag (v2f i) : SV_Target
	{
		float t = fmod(_Time.y, 7200);

		float3 drops = DropletLayer(i.uv, t);
		drops += DropletLayer(i.uv * 1.35 + 7.51, t);
		drops += DropletLayer(i.uv * 0.95 + 1.54, t);
		drops += DropletLayer(i.uv * 1.57 - 6.54, t);
		
		float fade = 1.0 - saturate(fwidth(i.uv));
		float blur = _Blur * (1.0 - drops.z * fade);

		float4 col = tex2Dlod(_Global_ScreenTex, float4(i.uv + drops.xy * _Distortion, 0, blur));

		float2 projUv = i.grabUv.xy / i.grabUv.w;
		projUv.y = 1.0 - projUv.y;
		projUv += drops.xy * _Distortion * fade;
		blur *= 0.01;
		const float numSamples = 32;
		float a = N21(i.uv) * 6.2831;
		for (float i = 0; i < numSamples; i++)
		{
			float2 offs = float2(sin(a), cos(a)) * blur;
			float d = frac(sin((i + 1) * 546) * 5421);
			d = sqrt(d); 
			offs *= d;
			col += tex2D(_Global_ScreenTex, projUv + offs);
			a++;
		}
		col /= numSamples;
		return col;
	}
	ENDCG
	SubShader {
		Tags { "RenderType" = "Opaque" "Queue" = "Transparent"}
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
}