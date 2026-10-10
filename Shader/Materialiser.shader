// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:False,mssp:True,bkdf:True,hqlp:True,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:3,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.015,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32719,y:32712,varname:node_3138,prsc:2|diff-3929-OUT,spec-6365-R,gloss-2242-OUT,normal-2499-RGB,emission-8884-OUT,clip-4039-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:31152,y:33098,ptovrint:False,ptlb:HologramColor,ptin:_HologramColor,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_Fresnel,id:8887,x:31032,y:33673,varname:node_8887,prsc:2|EXP-5561-OUT;n:type:ShaderForge.SFN_Multiply,id:4818,x:31429,y:33686,varname:node_4818,prsc:2|A-780-OUT,B-952-OUT,C-6476-OUT;n:type:ShaderForge.SFN_OneMinus,id:952,x:31206,y:33673,varname:node_952,prsc:2|IN-8887-OUT;n:type:ShaderForge.SFN_Slider,id:5561,x:30712,y:33700,ptovrint:False,ptlb:Fresnel Power,ptin:_FresnelPower,varname:node_5561,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:4;n:type:ShaderForge.SFN_Slider,id:780,x:31049,y:33598,ptovrint:False,ptlb:Opacity Strength,ptin:_OpacityStrength,varname:node_780,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8,max:1;n:type:ShaderForge.SFN_Set,id:6218,x:31619,y:33686,varname:opacityClip,prsc:2|IN-4818-OUT;n:type:ShaderForge.SFN_Get,id:3466,x:31783,y:33139,varname:node_3466,prsc:2|IN-6218-OUT;n:type:ShaderForge.SFN_Set,id:2560,x:31394,y:33110,varname:hologramColor,prsc:2|IN-7241-RGB;n:type:ShaderForge.SFN_FragmentPosition,id:9491,x:30804,y:33265,varname:node_9491,prsc:2;n:type:ShaderForge.SFN_Multiply,id:7352,x:31163,y:33286,varname:node_7352,prsc:2|A-8911-OUT,B-9211-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8911,x:30989,y:33265,ptovrint:False,ptlb:Scanline Scale,ptin:_ScanlineScale,varname:node_8911,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:512;n:type:ShaderForge.SFN_Add,id:9211,x:30989,y:33324,varname:node_9211,prsc:2|A-9491-Y,B-2808-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8583,x:30620,y:33487,ptovrint:False,ptlb:Animation Strength,ptin:_AnimationStrength,varname:node_8583,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Multiply,id:2808,x:30804,y:33399,varname:node_2808,prsc:2|A-5586-TSL,B-8583-OUT;n:type:ShaderForge.SFN_Time,id:5586,x:30620,y:33347,varname:node_5586,prsc:2;n:type:ShaderForge.SFN_Sin,id:6482,x:31319,y:33286,varname:node_6482,prsc:2|IN-7352-OUT;n:type:ShaderForge.SFN_Slider,id:6213,x:31162,y:33432,ptovrint:False,ptlb:Scanline Strength,ptin:_ScanlineStrength,varname:node_6213,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.05,max:1;n:type:ShaderForge.SFN_Lerp,id:9446,x:31534,y:33286,varname:node_9446,prsc:2|A-969-OUT,B-6482-OUT,T-6213-OUT;n:type:ShaderForge.SFN_Vector1,id:969,x:31319,y:33229,varname:node_969,prsc:2,v1:1;n:type:ShaderForge.SFN_Set,id:9513,x:31701,y:33286,varname:scanLineWorldUV,prsc:2|IN-9446-OUT;n:type:ShaderForge.SFN_Get,id:6476,x:31185,y:33848,varname:node_6476,prsc:2|IN-9513-OUT;n:type:ShaderForge.SFN_Tex2d,id:6365,x:32067,y:32603,ptovrint:False,ptlb:MetallicGlossMap,ptin:_MetallicGlossMap,varname:node_6365,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:2499,x:31980,y:32913,ptovrint:False,ptlb:BumpMap,ptin:_BumpMap,varname:node_2499,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:2457,x:30894,y:32185,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_2457,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:eab6eb41cb3093844aff1af775b7b7ed,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:4721,x:30894,y:32361,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_4721,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:8388,x:31140,y:32230,varname:node_8388,prsc:2|A-2457-RGB,B-4721-RGB;n:type:ShaderForge.SFN_Vector3,id:4493,x:31140,y:32362,varname:node_4493,prsc:2,v1:0,v2:0,v3:0;n:type:ShaderForge.SFN_Set,id:8774,x:31580,y:32344,varname:BaseColor,prsc:2|IN-8388-OUT;n:type:ShaderForge.SFN_Get,id:3929,x:32486,y:32699,varname:node_3929,prsc:2|IN-8774-OUT;n:type:ShaderForge.SFN_Slider,id:7773,x:28989,y:32124,ptovrint:False,ptlb:Dissolve,ptin:_Dissolve,varname:node_7773,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_RemapRange,id:5349,x:29817,y:32084,varname:node_5349,prsc:2,frmn:0,frmx:1,tomn:-0.15,tomx:1|IN-6312-OUT;n:type:ShaderForge.SFN_Tex2d,id:816,x:29817,y:32260,ptovrint:False,ptlb:Noise,ptin:_Noise,varname:node_816,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Add,id:794,x:30015,y:32159,varname:node_794,prsc:2|A-5349-OUT,B-816-R;n:type:ShaderForge.SFN_Add,id:405,x:30213,y:32059,varname:node_405,prsc:2|A-9194-OUT,B-794-OUT;n:type:ShaderForge.SFN_Floor,id:1023,x:30213,y:32198,varname:node_1023,prsc:2|IN-794-OUT;n:type:ShaderForge.SFN_Set,id:5744,x:30379,y:32198,varname:MinBurn,prsc:2|IN-1023-OUT;n:type:ShaderForge.SFN_Set,id:7233,x:30536,y:32058,varname:MaxBurn,prsc:2|IN-5547-OUT;n:type:ShaderForge.SFN_Get,id:1250,x:29311,y:32870,varname:node_1250,prsc:2|IN-5744-OUT;n:type:ShaderForge.SFN_Get,id:7015,x:29311,y:32948,varname:node_7015,prsc:2|IN-7233-OUT;n:type:ShaderForge.SFN_Multiply,id:8566,x:29543,y:32792,varname:node_8566,prsc:2|A-1250-OUT,B-7015-OUT;n:type:ShaderForge.SFN_Clamp01,id:3987,x:29715,y:32792,varname:node_3987,prsc:2|IN-8566-OUT;n:type:ShaderForge.SFN_OneMinus,id:2775,x:29879,y:32792,varname:node_2775,prsc:2|IN-3987-OUT;n:type:ShaderForge.SFN_Set,id:825,x:30036,y:32792,varname:MinMask,prsc:2|IN-2775-OUT;n:type:ShaderForge.SFN_Add,id:9141,x:29543,y:32963,varname:node_9141,prsc:2|A-1250-OUT,B-7015-OUT;n:type:ShaderForge.SFN_Clamp01,id:8654,x:29715,y:32963,varname:node_8654,prsc:2|IN-9141-OUT;n:type:ShaderForge.SFN_Set,id:2755,x:29889,y:32963,varname:MaxMask,prsc:2|IN-8654-OUT;n:type:ShaderForge.SFN_Multiply,id:5527,x:29479,y:33376,varname:node_5527,prsc:2|A-2666-OUT,B-8811-OUT;n:type:ShaderForge.SFN_Get,id:8811,x:29276,y:33412,varname:node_8811,prsc:2|IN-2755-OUT;n:type:ShaderForge.SFN_Get,id:2666,x:29276,y:33352,varname:node_2666,prsc:2|IN-2560-OUT;n:type:ShaderForge.SFN_Color,id:9441,x:29276,y:33495,ptovrint:False,ptlb:EdgeBurn,ptin:_EdgeBurn,varname:node_9441,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.4233347,c2:0.8277428,c3:0.9926471,c4:1;n:type:ShaderForge.SFN_Multiply,id:4273,x:29479,y:33526,varname:node_4273,prsc:2|A-9441-RGB,B-3737-OUT;n:type:ShaderForge.SFN_Get,id:3737,x:29276,y:33656,varname:node_3737,prsc:2|IN-825-OUT;n:type:ShaderForge.SFN_Lerp,id:1780,x:29760,y:33474,varname:node_1780,prsc:2|A-5527-OUT,B-4273-OUT,T-9691-OUT;n:type:ShaderForge.SFN_Get,id:9691,x:29479,y:33656,varname:node_9691,prsc:2|IN-825-OUT;n:type:ShaderForge.SFN_Clamp01,id:1667,x:29936,y:33474,varname:node_1667,prsc:2|IN-1780-OUT;n:type:ShaderForge.SFN_Get,id:4820,x:29915,y:33607,varname:node_4820,prsc:2|IN-2755-OUT;n:type:ShaderForge.SFN_Set,id:2147,x:30297,y:33474,varname:Emission,prsc:2|IN-7515-OUT;n:type:ShaderForge.SFN_Get,id:960,x:32304,y:32944,varname:node_960,prsc:2|IN-2147-OUT;n:type:ShaderForge.SFN_Slider,id:6261,x:31927,y:32806,ptovrint:False,ptlb:GlossMapScale,ptin:_GlossMapScale,varname:node_6261,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Lerp,id:634,x:31358,y:32344,varname:node_634,prsc:2|A-8388-OUT,B-4493-OUT,T-9537-OUT;n:type:ShaderForge.SFN_Floor,id:5547,x:30379,y:32058,varname:node_5547,prsc:2|IN-405-OUT;n:type:ShaderForge.SFN_Lerp,id:4039,x:32409,y:33237,varname:node_4039,prsc:2|A-6249-OUT,B-7225-OUT,T-2276-OUT;n:type:ShaderForge.SFN_Get,id:2541,x:31783,y:33194,varname:node_2541,prsc:2|IN-2755-OUT;n:type:ShaderForge.SFN_Vector1,id:5498,x:31980,y:33298,varname:node_5498,prsc:2,v1:1;n:type:ShaderForge.SFN_ToggleProperty,id:6965,x:32088,y:33511,ptovrint:False,ptlb:ShowModel,ptin:_ShowModel,varname:node_6965,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:True;n:type:ShaderForge.SFN_Multiply,id:6249,x:31980,y:33158,varname:node_6249,prsc:2|A-3466-OUT,B-2541-OUT;n:type:ShaderForge.SFN_Lerp,id:7225,x:32189,y:33264,varname:node_7225,prsc:2|A-5498-OUT,B-6249-OUT,T-2541-OUT;n:type:ShaderForge.SFN_Set,id:7760,x:32273,y:33511,varname:showModel,prsc:2|IN-6965-OUT;n:type:ShaderForge.SFN_Get,id:2276,x:32170,y:33410,varname:node_2276,prsc:2|IN-7760-OUT;n:type:ShaderForge.SFN_Get,id:9537,x:31119,y:32569,varname:node_9537,prsc:2|IN-8404-OUT;n:type:ShaderForge.SFN_Multiply,id:7515,x:30117,y:33486,varname:node_7515,prsc:2|A-1667-OUT,B-4820-OUT,C-3237-OUT;n:type:ShaderForge.SFN_Slider,id:3237,x:29779,y:33765,ptovrint:False,ptlb:EdgeGlow,ptin:_EdgeGlow,varname:node_3237,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:1,cur:1.5,max:5;n:type:ShaderForge.SFN_Slider,id:9194,x:29738,y:31931,ptovrint:False,ptlb:TransitionSize,ptin:_TransitionSize,varname:node_9194,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.01,cur:0.035,max:0.25;n:type:ShaderForge.SFN_Multiply,id:2242,x:32304,y:32740,varname:node_2242,prsc:2|A-6365-A,B-6261-OUT;n:type:ShaderForge.SFN_Set,id:8404,x:29309,y:32196,varname:DisolveAmount,prsc:2|IN-7773-OUT;n:type:ShaderForge.SFN_Get,id:6312,x:29533,y:32191,varname:node_6312,prsc:2|IN-8404-OUT;n:type:ShaderForge.SFN_Slider,id:3384,x:32140,y:33027,ptovrint:False,ptlb:EmissiveStrength,ptin:_EmissiveStrength,varname:node_3384,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:1,cur:1,max:10;n:type:ShaderForge.SFN_Multiply,id:8884,x:32491,y:32965,varname:node_8884,prsc:2|A-960-OUT,B-3384-OUT;proporder:4721-2457-6365-6261-2499-816-7773-7241-9441-3237-9194-6965-5561-780-6213-8911-8583-3384;pass:END;sub:END;*/

Shader "Shader Forge/Materialiser/Original" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _MetallicGlossMap ("MetallicGlossMap", 2D) = "black" {}
        _GlossMapScale ("GlossMapScale", Range(0, 1)) = 1
        _BumpMap ("BumpMap", 2D) = "bump" {}
        _Noise ("Noise", 2D) = "white" {}
        _Dissolve ("Dissolve", Range(0, 1)) = 0
        _HologramColor ("HologramColor", Color) = (0.07843138,0.3921569,0.7843137,1)
        _EdgeBurn ("EdgeBurn", Color) = (0.4233347,0.8277428,0.9926471,1)
        _EdgeGlow ("EdgeGlow", Range(1, 5)) = 1.5
        _TransitionSize ("TransitionSize", Range(0.01, 0.25)) = 0.035
        [MaterialToggle] _ShowModel ("ShowModel", Float ) = 1
        _FresnelPower ("Fresnel Power", Range(0, 4)) = 1
        _OpacityStrength ("Opacity Strength", Range(0, 1)) = 0.8
        _ScanlineStrength ("Scanline Strength", Range(0, 1)) = 0.05
        _ScanlineScale ("Scanline Scale", Float ) = 512
        _AnimationStrength ("Animation Strength", Float ) = 2
        _EmissiveStrength ("EmissiveStrength", Range(1, 10)) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
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
            uniform float4 _HologramColor;
            uniform float _FresnelPower;
            uniform float _OpacityStrength;
            uniform float _ScanlineScale;
            uniform float _AnimationStrength;
            uniform float _ScanlineStrength;
            uniform sampler2D _MetallicGlossMap; uniform float4 _MetallicGlossMap_ST;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _Color;
            uniform float _Dissolve;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float4 _EdgeBurn;
            uniform float _GlossMapScale;
            uniform fixed _ShowModel;
            uniform float _EdgeGlow;
            uniform float _TransitionSize;
            uniform float _EmissiveStrength;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                float4 projPos : TEXCOORD7;
                LIGHTING_COORDS(8,9)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD10;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(i.uv0, _BumpMap)));
                float3 normalLocal = _BumpMap_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float2 sceneUVs = (i.projPos.xy / i.projPos.w);
                float4 node_5586 = _Time;
                float scanLineWorldUV = lerp(1.0,sin((_ScanlineScale*(i.posWorld.g+(node_5586.r*_AnimationStrength)))),_ScanlineStrength);
                float opacityClip = (_OpacityStrength*(1.0 - pow(1.0-max(0,dot(normalDirection, viewDirection)),_FresnelPower))*scanLineWorldUV);
                float DisolveAmount = _Dissolve;
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(i.uv0, _Noise));
                float node_794 = ((DisolveAmount*1.15+-0.15)+_Noise_var.r);
                float MinBurn = floor(node_794);
                float node_1250 = MinBurn;
                float MaxBurn = floor((_TransitionSize+node_794));
                float node_7015 = MaxBurn;
                float MaxMask = saturate((node_1250+node_7015));
                float node_2541 = MaxMask;
                float node_6249 = (opacityClip*node_2541);
                float showModel = _ShowModel;
                clip( BinaryDither4x4(lerp(node_6249,lerp(1.0,node_6249,node_2541),showModel) - 1.5, sceneUVs) );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                UNITY_LIGHT_ATTENUATION(attenuation,i, i.posWorld.xyz);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float4 _MetallicGlossMap_var = tex2D(_MetallicGlossMap,TRANSFORM_TEX(i.uv0, _MetallicGlossMap));
                float gloss = (_MetallicGlossMap_var.a*_GlossMapScale);
                float perceptualRoughness = 1.0 - (_MetallicGlossMap_var.a*_GlossMapScale);
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMin[0] = unity_SpecCube0_BoxMin;
                    d.boxMin[1] = unity_SpecCube1_BoxMin;
                #endif
                #if UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMax[0] = unity_SpecCube0_BoxMax;
                    d.boxMax[1] = unity_SpecCube1_BoxMax;
                    d.probePosition[0] = unity_SpecCube0_ProbePosition;
                    d.probePosition[1] = unity_SpecCube1_ProbePosition;
                #endif
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = _MetallicGlossMap_var.r;
                float specularMonochrome;
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 node_8388 = (_MainTex_var.rgb*_Color.rgb);
                float3 BaseColor = node_8388;
                float3 diffuseColor = BaseColor; // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                half surfaceReduction;
                #ifdef UNITY_COLORSPACE_GAMMA
                    surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;
                #else
                    surfaceReduction = 1.0/(roughness*roughness + 1.0);
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                indirectSpecular *= surfaceReduction;
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float3 hologramColor = _HologramColor.rgb;
                float MinMask = (1.0 - saturate((node_1250*node_7015)));
                float3 Emission = (saturate(lerp((hologramColor*MaxMask),(_EdgeBurn.rgb*MinMask),MinMask))*MaxMask*_EdgeGlow);
                float3 emissive = (Emission*_EmissiveStrength);
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
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
            uniform float4 _HologramColor;
            uniform float _FresnelPower;
            uniform float _OpacityStrength;
            uniform float _ScanlineScale;
            uniform float _AnimationStrength;
            uniform float _ScanlineStrength;
            uniform sampler2D _MetallicGlossMap; uniform float4 _MetallicGlossMap_ST;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _Color;
            uniform float _Dissolve;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float4 _EdgeBurn;
            uniform float _GlossMapScale;
            uniform fixed _ShowModel;
            uniform float _EdgeGlow;
            uniform float _TransitionSize;
            uniform float _EmissiveStrength;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                float4 projPos : TEXCOORD7;
                LIGHTING_COORDS(8,9)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(i.uv0, _BumpMap)));
                float3 normalLocal = _BumpMap_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float2 sceneUVs = (i.projPos.xy / i.projPos.w);
                float4 node_5586 = _Time;
                float scanLineWorldUV = lerp(1.0,sin((_ScanlineScale*(i.posWorld.g+(node_5586.r*_AnimationStrength)))),_ScanlineStrength);
                float opacityClip = (_OpacityStrength*(1.0 - pow(1.0-max(0,dot(normalDirection, viewDirection)),_FresnelPower))*scanLineWorldUV);
                float DisolveAmount = _Dissolve;
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(i.uv0, _Noise));
                float node_794 = ((DisolveAmount*1.15+-0.15)+_Noise_var.r);
                float MinBurn = floor(node_794);
                float node_1250 = MinBurn;
                float MaxBurn = floor((_TransitionSize+node_794));
                float node_7015 = MaxBurn;
                float MaxMask = saturate((node_1250+node_7015));
                float node_2541 = MaxMask;
                float node_6249 = (opacityClip*node_2541);
                float showModel = _ShowModel;
                clip( BinaryDither4x4(lerp(node_6249,lerp(1.0,node_6249,node_2541),showModel) - 1.5, sceneUVs) );
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                UNITY_LIGHT_ATTENUATION(attenuation,i, i.posWorld.xyz);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float4 _MetallicGlossMap_var = tex2D(_MetallicGlossMap,TRANSFORM_TEX(i.uv0, _MetallicGlossMap));
                float gloss = (_MetallicGlossMap_var.a*_GlossMapScale);
                float perceptualRoughness = 1.0 - (_MetallicGlossMap_var.a*_GlossMapScale);
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = _MetallicGlossMap_var.r;
                float specularMonochrome;
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 node_8388 = (_MainTex_var.rgb*_Color.rgb);
                float3 BaseColor = node_8388;
                float3 diffuseColor = BaseColor; // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Back
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
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
            uniform float _FresnelPower;
            uniform float _OpacityStrength;
            uniform float _ScanlineScale;
            uniform float _AnimationStrength;
            uniform float _ScanlineStrength;
            uniform float _Dissolve;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform fixed _ShowModel;
            uniform float _TransitionSize;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float2 uv1 : TEXCOORD2;
                float2 uv2 : TEXCOORD3;
                float4 posWorld : TEXCOORD4;
                float3 normalDir : TEXCOORD5;
                float4 projPos : TEXCOORD6;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float2 sceneUVs = (i.projPos.xy / i.projPos.w);
                float4 node_5586 = _Time;
                float scanLineWorldUV = lerp(1.0,sin((_ScanlineScale*(i.posWorld.g+(node_5586.r*_AnimationStrength)))),_ScanlineStrength);
                float opacityClip = (_OpacityStrength*(1.0 - pow(1.0-max(0,dot(normalDirection, viewDirection)),_FresnelPower))*scanLineWorldUV);
                float DisolveAmount = _Dissolve;
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(i.uv0, _Noise));
                float node_794 = ((DisolveAmount*1.15+-0.15)+_Noise_var.r);
                float MinBurn = floor(node_794);
                float node_1250 = MinBurn;
                float MaxBurn = floor((_TransitionSize+node_794));
                float node_7015 = MaxBurn;
                float MaxMask = saturate((node_1250+node_7015));
                float node_2541 = MaxMask;
                float node_6249 = (opacityClip*node_2541);
                float showModel = _ShowModel;
                clip( BinaryDither4x4(lerp(node_6249,lerp(1.0,node_6249,node_2541),showModel) - 1.5, sceneUVs) );
                SHADOW_CASTER_FRAGMENT(i)
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
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
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
            uniform float4 _HologramColor;
            uniform sampler2D _MetallicGlossMap; uniform float4 _MetallicGlossMap_ST;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _Color;
            uniform float _Dissolve;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float4 _EdgeBurn;
            uniform float _GlossMapScale;
            uniform float _EdgeGlow;
            uniform float _TransitionSize;
            uniform float _EmissiveStrength;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float4 projPos : TEXCOORD4;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float2 sceneUVs = (i.projPos.xy / i.projPos.w);
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                float3 hologramColor = _HologramColor.rgb;
                float DisolveAmount = _Dissolve;
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(i.uv0, _Noise));
                float node_794 = ((DisolveAmount*1.15+-0.15)+_Noise_var.r);
                float MinBurn = floor(node_794);
                float node_1250 = MinBurn;
                float MaxBurn = floor((_TransitionSize+node_794));
                float node_7015 = MaxBurn;
                float MaxMask = saturate((node_1250+node_7015));
                float MinMask = (1.0 - saturate((node_1250*node_7015)));
                float3 Emission = (saturate(lerp((hologramColor*MaxMask),(_EdgeBurn.rgb*MinMask),MinMask))*MaxMask*_EdgeGlow);
                o.Emission = (Emission*_EmissiveStrength);
                
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 node_8388 = (_MainTex_var.rgb*_Color.rgb);
                float3 BaseColor = node_8388;
                float3 diffColor = BaseColor;
                float specularMonochrome;
                float3 specColor;
                float4 _MetallicGlossMap_var = tex2D(_MetallicGlossMap,TRANSFORM_TEX(i.uv0, _MetallicGlossMap));
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, _MetallicGlossMap_var.r, specColor, specularMonochrome );
                float roughness = 1.0 - (_MetallicGlossMap_var.a*_GlossMapScale);
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
