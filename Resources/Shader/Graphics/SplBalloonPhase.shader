
Shader "Spl/Balloon Phase"
{
    Properties
	{
        [NoScaleOffset]_MainTex ("MainTex", 2D) = "white" {}
        _Ramp ("Ramp", 2D) = "white" {}
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
                float pi = 3.141592654;
                float4 spl = tex2D(_MainTex,i.uv0);
				float u = (atan2(spl.b, spl.g) + pi) / (2.0 * pi);
				float4 col = tex2D(_Ramp,TRANSFORM_TEX(float2(u, 0.5), _Ramp));
                return fixed4(col.rgb, 1);
            }
            ENDCG
        }
    }
}
