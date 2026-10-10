// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:False,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:False,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:3,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32819,y:32265,varname:node_3138,prsc:2|emission-4116-OUT,clip-7155-OUT;n:type:ShaderForge.SFN_Slider,id:7773,x:30745,y:33004,ptovrint:False,ptlb:HologramAmount,ptin:_HologramAmount,varname:node_7773,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.6416152,max:1;n:type:ShaderForge.SFN_RemapRange,id:5349,x:31100,y:33011,varname:node_5349,prsc:2,frmn:0,frmx:1,tomn:-0.01,tomx:1.5708|IN-7773-OUT;n:type:ShaderForge.SFN_Tex2d,id:816,x:31269,y:33201,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:node_816,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Add,id:794,x:31467,y:33062,varname:node_794,prsc:2|A-6054-OUT,B-816-R;n:type:ShaderForge.SFN_Set,id:7233,x:32011,y:33062,varname:mask,prsc:2|IN-5547-OUT;n:type:ShaderForge.SFN_Floor,id:5547,x:31824,y:33062,varname:node_5547,prsc:2|IN-5843-OUT;n:type:ShaderForge.SFN_Sin,id:6054,x:31269,y:33011,varname:node_6054,prsc:2|IN-5349-OUT;n:type:ShaderForge.SFN_Clamp01,id:5843,x:31641,y:33062,varname:node_5843,prsc:2|IN-794-OUT;n:type:ShaderForge.SFN_Color,id:7383,x:31355,y:32468,ptovrint:False,ptlb:HologramColour,ptin:_HologramColour,varname:node_7383,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.3921569,c3:0.7843137,c4:0.5;n:type:ShaderForge.SFN_Get,id:4240,x:31334,y:32368,varname:node_4240,prsc:2|IN-7233-OUT;n:type:ShaderForge.SFN_Multiply,id:7967,x:31612,y:32425,varname:node_7967,prsc:2|A-4240-OUT,B-7383-RGB;n:type:ShaderForge.SFN_Clamp01,id:6655,x:31811,y:32425,varname:node_6655,prsc:2|IN-7967-OUT;n:type:ShaderForge.SFN_Multiply,id:4116,x:32012,y:32370,varname:node_4116,prsc:2|A-4240-OUT,B-6655-OUT;n:type:ShaderForge.SFN_Fresnel,id:1404,x:31419,y:32665,varname:node_1404,prsc:2;n:type:ShaderForge.SFN_OneMinus,id:7057,x:31609,y:32665,varname:node_7057,prsc:2|IN-1404-OUT;n:type:ShaderForge.SFN_Multiply,id:65,x:31792,y:32613,varname:node_65,prsc:2|A-7383-A,B-7057-OUT;n:type:ShaderForge.SFN_Get,id:6659,x:31969,y:32724,varname:node_6659,prsc:2|IN-7233-OUT;n:type:ShaderForge.SFN_Multiply,id:8340,x:32198,y:32613,varname:node_8340,prsc:2|A-65-OUT,B-6659-OUT;n:type:ShaderForge.SFN_Vector1,id:4303,x:32198,y:32494,varname:node_4303,prsc:2,v1:1;n:type:ShaderForge.SFN_Lerp,id:1482,x:32368,y:32684,varname:node_1482,prsc:2|A-4303-OUT,B-8340-OUT,T-6659-OUT;n:type:ShaderForge.SFN_Lerp,id:7155,x:32582,y:32684,varname:node_7155,prsc:2|A-8340-OUT,B-1482-OUT,T-5637-OUT;n:type:ShaderForge.SFN_Vector1,id:5637,x:32338,y:32878,varname:node_5637,prsc:2,v1:0;proporder:7773-816-7383;pass:END;sub:END;*/

Shader "Materialiser/Hologram" {
    Properties {
        _HologramAmount ("HologramAmount", Range(0, 1)) = 0.6416152
        _Mask ("Mask", 2D) = "white" {}
        [HDR]_HologramColour ("HologramColour", Color) = (0,0.3921569,0.7843137,0.5)
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            // Dithering function, to use with scene UVs (screen pixel coords)
            // 4x4 Bayer matrix, based on https://en.wikipedia.org/wiki/Ordered_dithering
            float BinaryDither4x4( float value, float2 sceneUVs ) {
                float4x4 mtx = float4x4(
                    float4( 1,  9,  3, 11 )/17.0,
                    float4( 13, 5, 15,  7 )/17.0,
                    float4( 4, 12,  2, 10 )/17.0,
                    float4( 16, 8, 14,  6 )/17.0
                );
                float2 px = floor(_ScreenParams.xy * sceneUVs);
                int xSmp = fmod(px.x,4);
                int ySmp = fmod(px.y,4);
                float4 xVec = 1-saturate(abs(float4(0,1,2,3) - xSmp));
                float4 yVec = 1-saturate(abs(float4(0,1,2,3) - ySmp));
                float4 pxMult = float4( dot(mtx[0],yVec), dot(mtx[1],yVec), dot(mtx[2],yVec), dot(mtx[3],yVec) );
                return round(value + dot(pxMult, xVec));
            }
            uniform float _HologramAmount;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform float4 _HologramColour;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 projPos : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float2 sceneUVs = (i.projPos.xy / i.projPos.w);
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                float mask = floor(saturate((sin((_HologramAmount*1.5808+-0.01))+_Mask_var.r)));
                float node_6659 = mask;
                float node_8340 = ((_HologramColour.a*(1.0 - (1.0-max(0,dot(normalDirection, viewDirection)))))*node_6659);
                clip( BinaryDither4x4(lerp(node_8340,lerp(1.0,node_8340,node_6659),0.0) - 1.5, sceneUVs) );
////// Lighting:
////// Emissive:
                float node_4240 = mask;
                float3 emissive = (node_4240*saturate((node_4240*_HologramColour.rgb)));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
