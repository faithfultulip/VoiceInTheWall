Shader "LimitlessGlitch/Glitch8" 
{
    HLSLINCLUDE
        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
        uniform sampler2D _MainTex;
        uniform float _TimeX;
        uniform float Offset;
        uniform float Amount;
        uniform float resM;
        half alpha;
        float sat( float t ) { return clamp( t, 0.0, 1.0 ); }
        float2 sat( float2 t ) { return clamp( t, 0.0, 1.0 ); }
        float rand( float2 n ) { return frac(sin(dot(n.xy, float2(12.9898, 78.233)))* 43758.5453); }
        float trunc( float x, float num_levels ) { return floor(x*num_levels) / num_levels; }
        float2 trunc( float2 x, float2 num_levels ) { return floor(x*num_levels) / num_levels; }

        float3 rgb2yuv( float3 rgb )
        {
        float3 yuv;
        yuv.x = dot( rgb, float3(0.299,0.587,0.114) );
        yuv.y = dot( rgb, float3(-0.14713, -0.28886, 0.436) );
        yuv.z = dot( rgb, float3(0.615, -0.51499, -0.10001) );
        return yuv;
        }

        float3 yuv2rgb( float3 yuv )
        {
        float3 rgb;
        rgb.r = yuv.x + yuv.z * 1.13983;
        rgb.g = yuv.x + dot( float2(-0.39465, -0.58060), yuv.yz );
        rgb.b = yuv.x + yuv.y * 2.03211;
        return rgb;
        }

        float4 Frag(VaryingsDefault i) : SV_Target
        {
        float _TimeX_s = _TimeX;
        float2 uv = i.texcoord.xy;
        float ct = trunc( _TimeX_s, 4.0 );
        float change_rnd = rand( trunc(uv.yy,float2(16,16)) + 150.0 * ct);
        float tf = 24.0*change_rnd;
        float t = 12.0 * trunc( _TimeX_s, tf);
        float vt_rnd = 0.5 * rand( trunc(uv.yy + t, float2(11*resM,11*resM)));
        vt_rnd += 0.5 * rand(trunc(uv.yy + t, float2(7,7)));
        vt_rnd = vt_rnd * 2.0 - 1.0;
        vt_rnd = sign(vt_rnd) * sat( ( abs(vt_rnd) -Amount) / (0.4) );
        vt_rnd = lerp(0, vt_rnd, Offset);
        float2 uv_nm = uv;
        uv_nm = sat( uv_nm + float2(0.1*vt_rnd, 0));
        float rn= trunc( _TimeX_s, 2.0 );
        float rnd = rand( float2(rn,rn));
        uv_nm.y = (rnd>lerp(1.0, 0.975, sat(0.4))) ? 1.0-uv_nm.y : uv_nm.y;

        float4 sample = tex2D( _MainTex, uv_nm);
        float3 sample_yuv = rgb2yuv( sample.rgb); 
        sample_yuv.y /= 1.0-3.0 * abs(vt_rnd) * sat( 0.5 - vt_rnd);
        sample_yuv.z += 0.125 * vt_rnd * sat( vt_rnd - 0.5);

        return lerp(tex2D( _MainTex, i.texcoord),float4( yuv2rgb(sample_yuv), sample.a),alpha);
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