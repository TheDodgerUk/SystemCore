
Shader "Spl/Plane Wave"
{
	Properties
	{
		[NoScaleOffset]_MainTex("MainTex", 2D) = "black" {}
		[NoScaleOffset]_Ramp("Ramp", 2D) = "white" {}
		_Alpha("Alpha", Range(0, 1)) = 1
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

			float2 mag_phase_to_complex(float2 mp)
			{
				return float2(mp.x * cos(mp.y), mp.x * sin(mp.y));
			}
			float2 read_mag_phase(float4 col)
			{
				static float EPSILON = 1e-16;
				float m = col.r - EPSILON;
				float p = atan2(col.b + EPSILON, col.g);
				return float2(m, p);
			}

			float2 read_colour(float4 col)
			{
				return mag_phase_to_complex(read_mag_phase(col));
			}

			float4 frag(VertexOutput i) : COLOR
			{
				float4 col = tex2D(_MainTex, i.uv);
				float2 mp = read_mag_phase(col);
				mp.y += _Time.z *-1.0;
				float2 c = mag_phase_to_complex(mp);
				float u = c.x * 0.0001;

				float4 rampCol = tex2D(_Ramp, float2(u + 0.5, 0.5));
				return fixed4(rampCol.rgb, _Alpha);
			}

			ENDCG
		}
	}
	FallBack "Diffuse"
}
