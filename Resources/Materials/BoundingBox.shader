// Shader created with Shader Forge v1.41 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Enhanced by Antoine Guillon / Arkham Development - http://www.arkham-development.com/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.41;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33376,y:32693,varname:node_3138,prsc:2|emission-6726-OUT,alpha-1802-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32508,y:32492,ptovrint:False,ptlb:OutsideColor,ptin:_OutsideColor,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_Multiply,id:3999,x:33068,y:32753,varname:node_3999,prsc:2|A-7665-OUT,B-7614-OUT;n:type:ShaderForge.SFN_TexCoord,id:7913,x:31534,y:32704,varname:node_7913,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Blend,id:9701,x:32111,y:32743,varname:node_9701,prsc:2,blmd:5,clmp:True|SRC-7652-OUT,DST-2152-OUT;n:type:ShaderForge.SFN_Length,id:7652,x:31885,y:32683,varname:node_7652,prsc:2|IN-6087-OUT;n:type:ShaderForge.SFN_RemapRange,id:6087,x:31709,y:32683,varname:node_6087,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-7913-U;n:type:ShaderForge.SFN_Length,id:2152,x:31885,y:32840,varname:node_2152,prsc:2|IN-1017-OUT;n:type:ShaderForge.SFN_RemapRange,id:1017,x:31709,y:32840,varname:node_1017,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-7913-V;n:type:ShaderForge.SFN_Subtract,id:7045,x:32350,y:32880,varname:node_7045,prsc:2|A-9701-OUT,B-6706-OUT;n:type:ShaderForge.SFN_Slider,id:4633,x:31834,y:33049,ptovrint:False,ptlb:Thickness,ptin:_Thickness,varname:node_4633,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.1686472,max:1;n:type:ShaderForge.SFN_Multiply,id:1802,x:32716,y:32964,varname:node_1802,prsc:2|A-7045-OUT,B-4298-OUT;n:type:ShaderForge.SFN_Slider,id:4298,x:32350,y:33085,ptovrint:False,ptlb:AlphaBoost,ptin:_AlphaBoost,varname:node_4298,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:1,cur:1,max:50;n:type:ShaderForge.SFN_OneMinus,id:6706,x:32158,y:33012,varname:node_6706,prsc:2|IN-4633-OUT;n:type:ShaderForge.SFN_FaceSign,id:2478,x:32508,y:32727,varname:node_2478,prsc:2,fstp:0;n:type:ShaderForge.SFN_Color,id:3976,x:32508,y:32307,ptovrint:False,ptlb:InsideColor,ptin:_InsideColor,varname:node_3976,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.0511748,c2:0.238074,c3:0.4716981,c4:1;n:type:ShaderForge.SFN_Lerp,id:7643,x:32760,y:32532,varname:node_7643,prsc:2|A-3976-RGB,B-7241-RGB,T-2478-VFACE;n:type:ShaderForge.SFN_Clamp01,id:7614,x:32716,y:32840,varname:node_7614,prsc:2|IN-7045-OUT;n:type:ShaderForge.SFN_Slider,id:4273,x:32351,y:32655,ptovrint:False,ptlb:Emission,ptin:_Emission,varname:node_4273,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:1,cur:17.27234,max:50;n:type:ShaderForge.SFN_Multiply,id:7665,x:32923,y:32600,varname:node_7665,prsc:2|A-7643-OUT,B-4273-OUT;n:type:ShaderForge.SFN_Clamp01,id:6726,x:33219,y:32815,varname:node_6726,prsc:2|IN-3999-OUT;proporder:7241-3976-4273-4633-4298;pass:END;sub:END;*/

Shader "Shader Forge/BoundingBox" {
    Properties {
        _OutsideColor ("OutsideColor", Color) = (0.07843138,0.3921569,0.7843137,1)
        _InsideColor ("InsideColor", Color) = (0.0511748,0.238074,0.4716981,1)
        _Emission ("Emission", Range(1, 50)) = 17.27234
        _Thickness ("Thickness", Range(0, 1)) = 0.1686472
        _AlphaBoost ("AlphaBoost", Range(1, 50)) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #ifndef UNITY_PASS_FORWARDBASE
            #define UNITY_PASS_FORWARDBASE
            #endif //UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _OutsideColor;
            uniform float _Thickness;
            uniform float _AlphaBoost;
            uniform float4 _InsideColor;
            uniform float _Emission;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float node_7045 = (saturate(max(length((i.uv0.r*2.0+-1.0)),length((i.uv0.g*2.0+-1.0))))-(1.0 - _Thickness));
                float3 emissive = saturate(((lerp(_InsideColor.rgb,_OutsideColor.rgb,isFrontFace)*_Emission)*saturate(node_7045)));
                float3 finalColor = emissive;
                return fixed4(finalColor,(node_7045*_AlphaBoost));
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #ifndef UNITY_PASS_SHADOWCASTER
            #define UNITY_PASS_SHADOWCASTER
            #endif //UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
