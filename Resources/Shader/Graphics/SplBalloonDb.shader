
Shader "Spl/Balloon dB"
{
    Properties 
	{
        [NoScaleOffset]_MainTex ("MainTex", 2D) = "gray" {}
        _Ramp ("Ramp", 2D) = "gray" {}
        _HeightResolution ("Height Resolution", Range(1, 8)) = 1
        _HeightAmount ("Height Amount", Range(0, 1)) = 1
        _MindB ("Min dB", Float ) = -30
        _MaxdB ("Max dB", Float ) = 0
    }
    SubShader 
	{
        Tags { "RenderType"="Opaque" }
        Pass 
		{
            Name "FORWARD"
            
            CGPROGRAM
			#include "SplBalloonCG.cginc"
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag

            float4 frag(VertexOutput i) : COLOR 
			{
                float4 tex = tex2Dlod(_MainTex, float4(i.uv0,0,0));
                float magnitude = tex.r; // magnitude
                float real = (tex.g * magnitude); // real
                float imag = (tex.b * magnitude); // imag
                float db = 20.0 * log10(sqrt((real * real) + (imag * imag)));
                float t = saturate((db - _MindB) / (_MaxdB - _MindB));
                float4 col = tex2D(_Ramp,TRANSFORM_TEX(float2(t, 0.5), _Ramp));
                return fixed4(col.rgb,1);
            }
            ENDCG
        }
    }
}
