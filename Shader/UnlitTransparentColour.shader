
Shader "Unlit/Transparent Colour" 
{
    Properties 
	{
        _Color ("Color", Color) = (0.07843138,0.3921569,0.7843137,1)

		_Factor ("Offset Factor", Float) = 1.0
		_Units ("Offset Units", Float) = 1.0
    }
    SubShader
	{
        Tags
		{
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass
		{
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
			Offset [_Factor], [_Units]
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 2.0
            uniform float4 _Color;
            struct VertexInput 
			{
                float4 vertex : POSITION;
            };
            struct VertexOutput 
			{
                float4 pos : SV_POSITION;
            };
            VertexOutput vert (VertexInput v) 
			{
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR 
			{
                return _Color;
            }
            ENDCG
        }
    }
    
}
