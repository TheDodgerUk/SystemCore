// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:False,qofs:800,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33102,y:32718,varname:node_3138,prsc:2|emission-6279-OUT,alpha-7241-A,clip-8982-OUT,voffset-1428-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32526,y:32568,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_VertexColor,id:7512,x:32434,y:33009,varname:node_7512,prsc:2;n:type:ShaderForge.SFN_Slider,id:2979,x:32277,y:32934,ptovrint:False,ptlb:Outline,ptin:_Outline,varname:node_2979,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.0940171,max:1;n:type:ShaderForge.SFN_Multiply,id:1428,x:32745,y:33010,varname:node_1428,prsc:2|A-2979-OUT,B-7512-RGB;n:type:ShaderForge.SFN_DepthBlend,id:8982,x:32543,y:32768,varname:node_8982,prsc:2|DIST-4471-OUT;n:type:ShaderForge.SFN_Multiply,id:6279,x:32826,y:32697,varname:node_6279,prsc:2|A-7241-RGB,B-8982-OUT;n:type:ShaderForge.SFN_Slider,id:4471,x:32214,y:32792,ptovrint:False,ptlb:Depth,ptin:_Depth,varname:node_4471,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.2222222,max:2;proporder:7241-2979-4471;pass:END;sub:END;*/

Shader "Shader Forge/OutlineUnlit" {
    Properties {
        _Color ("Color", Color) = (0.07843138,0.3921569,0.7843137,1)
        _Outline ("Outline", Range(0, 1)) = 0.0940171
        _Depth ("Depth", Range(0, 2)) = 0.2222222
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="Geometry+800"
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            ZWrite Off

            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 2.0
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _Color;
            uniform float _Outline;
            uniform float _Depth;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.vertexColor = v.vertexColor;
                v.vertex.xyz += (_Outline*o.vertexColor.rgb);
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float node_8982 = saturate((sceneZ-partZ)/_Depth);
                clip(node_8982 - 0.5);
////// Lighting:
////// Emissive:
                float3 emissive = (_Color.rgb*node_8982);
                float3 finalColor = emissive;
                return fixed4(finalColor,_Color.a);
            }
            ENDCG
        }
    }
}
