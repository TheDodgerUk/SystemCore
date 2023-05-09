
Shader "Shader Forge/ShimmerFX"
{
    Properties
    {
        _ShimmerColor ("Shimmer Colour", Color) = (0.07843138,0.3921569,0.7843137,1)
        _ShimmerAlpha("Shimmer Alpha", Range(0, 1)) = 1
		_InnerColour ("Inner Colour", Color) = (1, 1, 1, 1)
        _Fresnel ("Fresnel", Range(0, 7)) = 3.393162
        _TimeScale ("TimeScale", Range(0, 4)) = 1
        _Remap_iMin ("Remap_iMin", Float ) = -1
        _Remap_iMax ("Remap_iMax", Float ) = 1
        _Remap_oMin ("Remap_oMin", Float ) = -3
        _Remap_oMax ("Remap_oMax", Float ) = 3
        _FresnelEmission ("FresnelEmission", Range(0, 4)) = 1
        _Direction ("Direction", Vector) = (0.5,0.5,0.5,0)

		_DepthColor ("Depth Colour", Color) = (0.07843138,0.3921569,0.7843137,1)
    }

    SubShader
    {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {

            Blend One One
            ZWrite Off
			ZTest Less
			Offset -1, -1
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 2.0

            uniform float4 _ShimmerColor;
			uniform float _ShimmerAlpha;
            uniform float _Fresnel;
            uniform float _TimeScale;
            uniform float _Remap_iMin;
            uniform float _Remap_iMax;
            uniform float _Remap_oMin;
            uniform float _Remap_oMax;
            uniform float _FresnelEmission;
            uniform float4 _Direction;
            uniform float4 _InnerColour;

            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
            };

            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float node_4597 = pow(1.0-max(0,dot(normalDirection, viewDirection)),_Fresnel);
                float3 FresnelFXA = (_ShimmerColor.rgb*node_4597*_ShimmerColor.a*_FresnelEmission);
                float3 node_9306 = FresnelFXA;
                float3 node_5396 = (_InnerColour.rgb*_InnerColour.a);
                float4 node_8788 = _Time;
                float3 emissive = ((node_9306+node_5396)*saturate((1.0 - distance((objPos.rgb+((_Remap_oMin + ( (tan((node_8788.g*_TimeScale)) - _Remap_iMin) * (_Remap_oMax - _Remap_oMin) ) / (_Remap_iMax - _Remap_iMin))*_Direction.rgb)),i.posWorld.rgb))));
                float3 finalColor = emissive * _ShimmerAlpha;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
