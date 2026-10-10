// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:6,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:33115,y:33076,varname:node_2865,prsc:2|emission-180-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:3994,x:28887,y:32525,varname:node_3994,prsc:2;n:type:ShaderForge.SFN_ObjectPosition,id:3881,x:28887,y:32221,varname:node_3881,prsc:2;n:type:ShaderForge.SFN_Distance,id:2177,x:29623,y:32272,varname:node_2177,prsc:2|A-3210-OUT,B-9169-Y;n:type:ShaderForge.SFN_Slider,id:7276,x:29436,y:32418,ptovrint:False,ptlb:Multiplier,ptin:_Multiplier,varname:node_7276,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:100;n:type:ShaderForge.SFN_Multiply,id:8971,x:29811,y:32272,varname:node_8971,prsc:2|A-2177-OUT,B-7276-OUT;n:type:ShaderForge.SFN_Smoothstep,id:7957,x:31131,y:32906,varname:node_7957,prsc:2|A-3274-OUT,B-3266-OUT,V-3062-OUT;n:type:ShaderForge.SFN_Multiply,id:4483,x:32489,y:33177,varname:node_4483,prsc:2|A-1935-OUT,B-9455-OUT;n:type:ShaderForge.SFN_Color,id:4065,x:31949,y:33235,ptovrint:False,ptlb:HologramColor,ptin:_HologramColor,varname:node_4065,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.710345,c3:1,c4:1;n:type:ShaderForge.SFN_Time,id:5308,x:30701,y:31968,varname:node_5308,prsc:2;n:type:ShaderForge.SFN_RemapRange,id:8507,x:30811,y:33184,varname:node_8507,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-2542-OUT;n:type:ShaderForge.SFN_Slider,id:3266,x:30660,y:32941,ptovrint:False,ptlb:MaxStep,ptin:_MaxStep,varname:node_3266,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8,max:4;n:type:ShaderForge.SFN_Slider,id:3354,x:30379,y:32694,ptovrint:False,ptlb:MinStep,ptin:_MinStep,varname:node_3354,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.25,max:4;n:type:ShaderForge.SFN_Slider,id:1000,x:30554,y:31890,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_1000,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:1,max:5;n:type:ShaderForge.SFN_Multiply,id:9419,x:30891,y:31940,varname:node_9419,prsc:2|A-1000-OUT,B-5308-T;n:type:ShaderForge.SFN_Add,id:1539,x:29119,y:32305,varname:node_1539,prsc:2|A-3881-XYZ,B-3278-XYZ;n:type:ShaderForge.SFN_Vector4Property,id:3278,x:28887,y:32372,ptovrint:False,ptlb:OffsetPulseOrigin,ptin:_OffsetPulseOrigin,varname:node_3278,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_Set,id:249,x:29978,y:32282,varname:PulseValue,prsc:2|IN-8971-OUT;n:type:ShaderForge.SFN_Get,id:1474,x:30093,y:33158,varname:node_1474,prsc:2|IN-249-OUT;n:type:ShaderForge.SFN_Set,id:8924,x:31105,y:31976,varname:Timer,prsc:2|IN-9419-OUT;n:type:ShaderForge.SFN_Get,id:6936,x:30093,y:33102,varname:node_6936,prsc:2|IN-8924-OUT;n:type:ShaderForge.SFN_Get,id:9455,x:31949,y:33388,varname:node_9455,prsc:2|IN-8630-OUT;n:type:ShaderForge.SFN_Lerp,id:1935,x:32180,y:33174,varname:node_1935,prsc:2|A-909-RGB,B-4065-RGB,T-9455-OUT;n:type:ShaderForge.SFN_Color,id:909,x:31949,y:33069,ptovrint:False,ptlb:OffColor,ptin:_OffColor,varname:node_909,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Slider,id:7493,x:32332,y:33104,ptovrint:False,ptlb:Emission,ptin:_Emission,varname:node_7493,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:50;n:type:ShaderForge.SFN_Multiply,id:3531,x:32670,y:33177,varname:node_3531,prsc:2|A-7493-OUT,B-4483-OUT;n:type:ShaderForge.SFN_ObjectPosition,id:2373,x:29065,y:32146,varname:node_2373,prsc:2;n:type:ShaderForge.SFN_FragmentPosition,id:9169,x:29119,y:32449,varname:node_9169,prsc:2;n:type:ShaderForge.SFN_Set,id:9834,x:29790,y:32202,varname:Distance,prsc:2|IN-2177-OUT;n:type:ShaderForge.SFN_Multiply,id:4557,x:30738,y:32745,varname:node_4557,prsc:2|A-3354-OUT,B-8761-OUT;n:type:ShaderForge.SFN_Get,id:8761,x:30488,y:32822,varname:node_8761,prsc:2|IN-9834-OUT;n:type:ShaderForge.SFN_Clamp01,id:3274,x:30908,y:32745,varname:node_3274,prsc:2|IN-4557-OUT;n:type:ShaderForge.SFN_Set,id:8630,x:31345,y:32906,varname:FXSmooth,prsc:2|IN-7957-OUT;n:type:ShaderForge.SFN_Tan,id:2542,x:30618,y:33184,varname:node_2542,prsc:2|IN-2428-OUT;n:type:ShaderForge.SFN_OneMinus,id:3062,x:31005,y:33184,varname:node_3062,prsc:2|IN-8507-OUT;n:type:ShaderForge.SFN_Add,id:2428,x:30349,y:33129,varname:node_2428,prsc:2|A-6936-OUT,B-1474-OUT;n:type:ShaderForge.SFN_ComponentMask,id:3210,x:29288,y:32278,varname:node_3210,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-1539-OUT;n:type:ShaderForge.SFN_Slider,id:5084,x:31866,y:32870,ptovrint:False,ptlb:Fresnel,ptin:_Fresnel,varname:node_5084,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:6,max:7;n:type:ShaderForge.SFN_Fresnel,id:7123,x:32364,y:32717,varname:node_7123,prsc:2|NRM-7162-OUT,EXP-5084-OUT;n:type:ShaderForge.SFN_NormalVector,id:7162,x:32023,y:32704,prsc:2,pt:True;n:type:ShaderForge.SFN_Multiply,id:4498,x:32650,y:32854,varname:node_4498,prsc:2|A-7123-OUT,B-8612-RGB;n:type:ShaderForge.SFN_Color,id:8612,x:32353,y:32875,ptovrint:False,ptlb:FresnelColor,ptin:_FresnelColor,varname:node_8612,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Add,id:180,x:32874,y:33068,varname:node_180,prsc:2|A-4498-OUT,B-3531-OUT;proporder:7493-909-4065-7276-3354-3266-3278-1000-5084-8612;pass:END;sub:END;*/

Shader "Shader Forge/HologramFX2" {
    Properties {
        _Emission ("Emission", Range(0, 50)) = 1
        _OffColor ("OffColor", Color) = (0.5,0.5,0.5,1)
        _HologramColor ("HologramColor", Color) = (0,0.710345,1,1)
        _Multiplier ("Multiplier", Range(0, 100)) = 1
        _MinStep ("MinStep", Range(0, 4)) = 0.25
        _MaxStep ("MaxStep", Range(0, 4)) = 0.8
        _OffsetPulseOrigin ("OffsetPulseOrigin", Vector) = (0,0,0,0)
        _Speed ("Speed", Range(-5, 5)) = 1
        _Fresnel ("Fresnel", Range(0, 7)) = 6
        _FresnelColor ("FresnelColor", Color) = (0.5,0.5,0.5,1)
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
            Blend One OneMinusSrcColor
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float _Multiplier;
            uniform float4 _HologramColor;
            uniform float _MaxStep;
            uniform float _MinStep;
            uniform float _Speed;
            uniform float4 _OffsetPulseOrigin;
            uniform float4 _OffColor;
            uniform float _Emission;
            uniform float _Fresnel;
            uniform float4 _FresnelColor;
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
                float node_2177 = distance((objPos.rgb+_OffsetPulseOrigin.rgb).g,i.posWorld.g);
                float Distance = node_2177;
                float4 node_5308 = _Time;
                float Timer = (_Speed*node_5308.g);
                float PulseValue = (node_2177*_Multiplier);
                float FXSmooth = smoothstep( saturate((_MinStep*Distance)), _MaxStep, (1.0 - (tan((Timer+PulseValue))*0.5+0.5)) );
                float node_9455 = FXSmooth;
                float3 emissive = ((pow(1.0-max(0,dot(normalDirection, viewDirection)),_Fresnel)*_FresnelColor.rgb)+(_Emission*(lerp(_OffColor.rgb,_HologramColor.rgb,node_9455)*node_9455)));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float _Multiplier;
            uniform float4 _HologramColor;
            uniform float _MaxStep;
            uniform float _MinStep;
            uniform float _Speed;
            uniform float4 _OffsetPulseOrigin;
            uniform float4 _OffColor;
            uniform float _Emission;
            uniform float _Fresnel;
            uniform float4 _FresnelColor;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
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
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                float node_2177 = distance((objPos.rgb+_OffsetPulseOrigin.rgb).g,i.posWorld.g);
                float Distance = node_2177;
                float4 node_5308 = _Time;
                float Timer = (_Speed*node_5308.g);
                float PulseValue = (node_2177*_Multiplier);
                float FXSmooth = smoothstep( saturate((_MinStep*Distance)), _MaxStep, (1.0 - (tan((Timer+PulseValue))*0.5+0.5)) );
                float node_9455 = FXSmooth;
                o.Emission = ((pow(1.0-max(0,dot(normalDirection, viewDirection)),_Fresnel)*_FresnelColor.rgb)+(_Emission*(lerp(_OffColor.rgb,_HologramColor.rgb,node_9455)*node_9455)));
                
                float3 diffColor = float3(0,0,0);
                o.Albedo = diffColor;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
