Shader "LimitlessGlitch/Glitch7" 
{
    HLSLINCLUDE
        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
		uniform sampler2D _MainTex;
		uniform float _TimeX;
		uniform float Offset;
		half _Noise;
		uniform float Fade;
		float random(float2 seed)
		{
			return frac(sin(dot(seed * floor(_TimeX * 30.0), float2(127.1,311.7))) * 43758.5453123);
		}

		float random(float seed)
		{
			return random(float2(seed, 1.0));
		}

        float4 Frag(VaryingsDefault i) : SV_Target
        {
			float2 uv = i.texcoord.xy;

			float2 blockS = floor(uv * float2(24., 9.));
			float2 blockL = floor(uv * float2(8., 4.));

			float lineNoise = pow(random(blockS), 8.0) *Offset* pow(random(blockL), 3.0) - pow(random(7.2341), 17.0) * 2.;

			float4 col1 = tex2D(_MainTex, uv);
			float4 col2 = tex2D(_MainTex, uv + float2(lineNoise * 0.05 * random(5.0), 0));
			float4 col3 = tex2D(_MainTex, uv - float2(lineNoise * 0.05 * random(31.0), 0));

			float4 result = float4(float3(col1.x, col2.y, col3.z), col1.a+col2.a+col3.a);
			result = lerp(col1,result, Fade);

			return result;
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