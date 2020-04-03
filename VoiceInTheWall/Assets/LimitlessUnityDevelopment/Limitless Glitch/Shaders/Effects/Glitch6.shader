Shader "LimitlessGlitch/Glitch6" 
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
		sampler2D _MainTex;

		half strength = 1;
		float mod(float x, float y)
		{
			return x - y * floor(x / y);
		}
		float jitterVFreq = 0.5;
		float jitterVRate = 0.5;

		#pragma shader_feature VHS_JITTER_V_ON
		#pragma shader_feature JITTER_V_CUSTOM
		float jitterVAmount = 1.0; 
		float jitterVSpeed = 1.0;			
		float2 JitterVRandSeed;
		float time_ = 0.0;

		half3 rgb2yiq(half3 c)
			{   
			return half3(
				(0.2989*c.x + 0.5959*c.y + 0.2115*c.z),
				(0.5870*c.x - 0.2744*c.y - 0.5229*c.z),
				(0.1140*c.x - 0.3216*c.y + 0.3114*c.z)
				);
			};
		half3 yiq2rgb(half3 c)
			{				
			return half3(
				(	 1.0*c.x +	  1.0*c.y + 	1.0*c.z),
				( 0.956*c.x - 0.2720*c.y - 1.1060*c.z),
				(0.6210*c.x - 0.6474*c.y + 1.7046*c.z)
				);
			};


			float rnd_rd(float2 co)
			{
			    float a = 22.9898;
				float b = 58.233;
				float c = 56058.5453;
				float dt= dot(co.xy ,float2(a,b));
				float sn= fmod(dt,3.14);
				return frac(sin(sn) * c);
			}

			float4 yiqDist(float2 uv, float m, float t)
				{					
				m *= 0.001; 
				float3 offsetX = float3( uv.x, uv.x, uv.x );	
				offsetX.r +=  sin(rnd_rd(float2(t*0.2, uv.y)))*m;
				offsetX.g +=  sin(t*9.0)*m;
				half4 signal = half4(0.0, 0.0, 0.0, 0.0);
				signal.r = rgb2yiq( tex2D( _MainTex, float2(offsetX.r, uv.y) ).rgb ).x;
				signal.g = rgb2yiq( tex2D( _MainTex, float2(offsetX.g, uv.y) ).rgb ).y;
				signal.b = rgb2yiq( tex2D( _MainTex, float2(offsetX.b, uv.y) ).rgb ).z;
				signal.a = tex2D( _MainTex,uv).a + tex2D(_MainTex, float2(offsetX.g, uv.y)).a + tex2D(_MainTex, float2(offsetX.r, uv.y)).a ;
				return signal;					    
				}


        float4 Frag(VaryingsDefault i) : SV_Target
        {
			float t = time_;			
			float2 p = i.texcoordStereo.xy;				

			half4 col = half4(0.0,0.0,0.0,0.0);
			half4 signal = half4(0.0,0.0,0.0,0.0);					
			#if JITTER_V_CUSTOM
				strength = 1;
			#else
				strength = smoothstep(jitterVFreq * jitterVRate, jitterVFreq, jitterVFreq - mod(_Time.y, jitterVFreq));
			#endif			
			   	signal = yiqDist(p, jitterVAmount*strength, t*jitterVSpeed);
			col.rgb = yiq2rgb(signal.rgb);
			return half4(col.rgb, (yiqDist(p, jitterVAmount*strength, t*jitterVSpeed)).a); 
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