
Shader "Spl/Plane dB"
{
	Properties
	{
		[NoScaleOffset]_MainTex("MainTex", 2D) = "black" {}
		[NoScaleOffset]_Ramp("Ramp", 2D) = "white" {}
		_Alpha("Alpha", Range(0, 1)) = 1
		_MindB("Min dB", Float) = 50
		_MaxdB("Max dB", Float) = 80
		_Factor ("Offset Factor", Float) = 1.0
		_Units ("Offset Units", Float) = 1.0
	}
	SubShader
	{
		Tags
		{
			"IgnoreProjector" = "True"
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
		}
		Pass
		{
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }
			Blend SrcAlpha OneMinusSrcAlpha
			Offset [_Factor], [_Units]
			ZWrite On

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#pragma multi_compile_fwdbase
			#pragma only_renderers d3d9 d3d11 glcore gles metal
			#pragma target 3.0

			uniform sampler2D _Ramp; uniform float4 _Ramp_ST;
			uniform sampler2D _MainTex;
			uniform float _Alpha;
			uniform float _MindB;
			uniform float _MaxdB;

			struct VertexInput
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct VertexOutput
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			VertexOutput vert(VertexInput v)
			{
				VertexOutput o = (VertexOutput)0;
				o.uv = v.texcoord;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}

			float4 frag(VertexOutput i) : COLOR
			{
				float4 col = tex2D(_MainTex, i.uv);
				float u = saturate((col.a - _MindB) / (_MaxdB - _MindB));
				float4 rampCol = tex2D(_Ramp, float2(u, 0.5));
				return fixed4(rampCol.rgb, _Alpha);
			}

			ENDCG
		}
	}
	FallBack "Diffuse"
}
