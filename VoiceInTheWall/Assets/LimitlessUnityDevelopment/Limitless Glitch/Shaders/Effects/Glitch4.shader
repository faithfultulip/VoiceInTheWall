Shader "LimitlessGlitch/Glitch4" 
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
        #include "noiseSimplex.cginc"
        sampler2D _MainTex;
		uniform float _GlitchInterval;
		uniform float _GlitchRate;
        uniform float _RGBSplit;
        uniform float _Speed;
        uniform float _Amount;
		float2 _Res;


		float random(float2 c) 
        {
			return frac(sin(dot(c.xy, float2(12.9898, 78.233))) * 43758.5453);
		}
		float mod(float x, float y)
		{
			return x - y * floor(x / y);
		}


        float4 Frag(VaryingsDefault i) : SV_Target
        {
half strength = 0.;
				float2 shake = float2(0., 0.);

				strength = smoothstep(_GlitchInterval * _GlitchRate, _GlitchInterval, _GlitchInterval - mod(_Time.y, _GlitchInterval));
				shake = float2(strength , strength )* float2(random(float2(_Time.xy)) * 2.0 - 1.0, random(float2(_Time.y * 2.0, _Time.y * 2.0)) * 2.0 - 1.0) / float2(_Res.x, _Res.y);

				float y = i.texcoord.y * _Res.y;
				float rgbWave = 0.;
				
					rgbWave = (
						snoise(float2( y * 0.01, _Time.y * _Speed*20)) * ( strength * _Amount*32.0) 
						* snoise(float2( y * 0.02, _Time.y * _Speed*10)) * (strength * _Amount*4.0) 

						) / _Res.x;
				
				float rgbDiff = (_RGBSplit*50 + (20.0 * strength + 1.0)) / _Res.x;
				rgbDiff = rgbDiff*rgbWave ;
				float rgbUvX = i.texcoord.x + rgbWave;
				
				float4 g = tex2D(_MainTex, float2(rgbUvX, i.texcoord.y) + shake);
				float4 rb = tex2D(_MainTex, float2(rgbUvX +rgbDiff, i.texcoord.y) + shake);

				float4 ret = float4(rb.x, g.y, rb.z, rb.a+g.a );

				return ret;
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }
    }
}