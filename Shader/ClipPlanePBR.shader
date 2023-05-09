// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:True,aust:False,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:32988,y:32698,varname:node_2865,prsc:2|diff-4987-OUT,spec-358-OUT,gloss-1813-OUT,normal-5964-RGB,emission-9691-OUT,alpha-1852-OUT;n:type:ShaderForge.SFN_Multiply,id:6343,x:31880,y:32505,varname:node_6343,prsc:2|A-7736-RGB,B-6665-RGB;n:type:ShaderForge.SFN_Color,id:6665,x:31687,y:32598,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:7736,x:31687,y:32413,ptovrint:True,ptlb:Base Color,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:5964,x:32407,y:32978,ptovrint:True,ptlb:Normal Map,ptin:_BumpMap,varname:_BumpMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Slider,id:358,x:32250,y:32780,ptovrint:False,ptlb:Metallic,ptin:_Metallic,varname:node_358,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:1813,x:32250,y:32882,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:_Metallic_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8,max:1;n:type:ShaderForge.SFN_FragmentPosition,id:9467,x:30925,y:33541,varname:node_9467,prsc:2;n:type:ShaderForge.SFN_Code,id:6153,x:31270,y:34211,varname:node_6153,prsc:2,code:UABsAGEAbgBlAE4AbwByAG0AYQBsACAAPQAgAG4AbwByAG0AYQBsAGkAegBlACgAUABsAGEAbgBlAE4AbwByAG0AYQBsACkAOwAKAAoAaABhAGwAZgAgAGQAaQBzAHQAIAA9ACAAKABXAG8AcgBsAGQAUABvAHMAaQB0AGkAbwBuAC4AeAAgACoAIABQAGwAYQBuAGUATgBvAHIAbQBhAGwALgB4ACkAIAArACAAKABXAG8AcgBsAGQAUABvAHMAaQB0AGkAbwBuAC4AeQAgACoAIABQAGwAYQBuAGUATgBvAHIAbQBhAGwALgB5ACkAIAArAAoACQAoAFcAbwByAGwAZABQAG8AcwBpAHQAaQBvAG4ALgB6ACAAKgAgAFAAbABhAG4AZQBOAG8AcgBtAGEAbAAuAHoAKQAgAC0AIAAKAAkAKABQAGwAYQBuAGUAUABvAGkAbgB0AC4AeAAgACoAIABQAGwAYQBuAGUATgBvAHIAbQBhAGwALgB4ACkAIAAtACAAKABQAGwAYQBuAGUAUABvAGkAbgB0AC4AeQAgACoAIABQAGwAYQBuAGUATgBvAHIAbQBhAGwALgB5ACkAIAAtACAAKABQAGwAYQBuAGUAUABvAGkAbgB0AC4AegAgAC0AIABQAGwAYQBuAGUATgBvAHIAbQBhAGwALgB6ACkAIAAvAAoACQBzAHEAcgB0ACgAcABvAHcAKABQAGwAYQBuAGUATgBvAHIAbQBhAGwALgB4ACwAIAAyAC4AMAApACAAKwAgAHAAbwB3ACgAUABsAGEAbgBlAE4AbwByAG0AYQBsAC4AeQAsACAAMgAuADAAKQAgACsAIABwAG8AdwAoAFAAbABhAG4AZQBOAG8AcgBtAGEAbAAuAHoALAAgADIALgAwACkAKQA7AAoACgBjAGwAaQBwACgAZABpAHMAdAApADsACgAKAGYAbABvAGEAdAAgAG0AYQBzAGsAIAA9ACAAZABpAHMAdAAgAC8AIABDAG8AbgB2AGUAcgB0AEQAaQBzAHQAYQBuAGMAZQA7AAoACgByAGUAdAB1AHIAbgAgAG0AYQBzAGsAOwA=,output:0,fname:CalculateClipPlane,width:1007,height:390,input:2,input:2,input:2,input:0,input_1_label:WorldPosition,input_2_label:PlanePoint,input_3_label:PlaneNormal,input_4_label:ConvertDistance|A-9467-XYZ,B-1923-XYZ,C-6470-XYZ,D-1133-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1133,x:30925,y:34042,ptovrint:False,ptlb:ConvertDistance,ptin:_ConvertDistance,varname:node_1133,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Clamp01,id:8087,x:32473,y:33747,varname:node_8087,prsc:2|IN-678-OUT;n:type:ShaderForge.SFN_Multiply,id:9234,x:32204,y:32495,varname:node_9234,prsc:2|A-2104-OUT,B-6343-OUT;n:type:ShaderForge.SFN_Vector4Property,id:1923,x:30925,y:33691,ptovrint:False,ptlb:PlanePosition,ptin:_PlanePosition,varname:node_1923,prsc:2,glob:True,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5,v2:0.5,v3:0.5,v4:1;n:type:ShaderForge.SFN_Vector4Property,id:6470,x:30925,y:33860,ptovrint:False,ptlb:PlaneNormal,ptin:_PlaneNormal,varname:_PlanePosition_copy,prsc:2,glob:True,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5,v2:0.5,v3:0.5,v4:1;n:type:ShaderForge.SFN_Set,id:237,x:32704,y:33659,varname:ClipAmount,prsc:2|IN-8087-OUT;n:type:ShaderForge.SFN_Get,id:5043,x:31052,y:31995,varname:node_5043,prsc:2|IN-237-OUT;n:type:ShaderForge.SFN_Lerp,id:4209,x:31596,y:31812,varname:node_4209,prsc:2|A-8630-OUT,B-2871-RGB,T-9503-OUT;n:type:ShaderForge.SFN_Color,id:2871,x:31052,y:31848,ptovrint:False,ptlb:PhaseColor,ptin:_PhaseColor,varname:node_2871,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.6519271,c3:0.02941179,c4:1;n:type:ShaderForge.SFN_Vector3,id:8630,x:31052,y:31739,varname:node_8630,prsc:2,v1:1,v2:1,v3:1;n:type:ShaderForge.SFN_OneMinus,id:9503,x:31393,y:31865,varname:node_9503,prsc:2|IN-5043-OUT;n:type:ShaderForge.SFN_Lerp,id:5686,x:31434,y:32064,varname:node_5686,prsc:2|A-2871-RGB,B-1778-OUT,T-5043-OUT;n:type:ShaderForge.SFN_Vector3,id:1778,x:31181,y:32092,varname:node_1778,prsc:2,v1:0,v2:0,v3:0;n:type:ShaderForge.SFN_Set,id:726,x:31650,y:32064,cmnt:The output emission color applied on top of the base color,varname:Emission,prsc:2|IN-5686-OUT;n:type:ShaderForge.SFN_Get,id:9691,x:32770,y:32863,varname:node_9691,prsc:2|IN-726-OUT;n:type:ShaderForge.SFN_Set,id:3828,x:31787,y:31812,varname:PhaseColor,prsc:2|IN-4209-OUT;n:type:ShaderForge.SFN_Get,id:2104,x:32027,y:32461,varname:node_2104,prsc:2|IN-3828-OUT;n:type:ShaderForge.SFN_FaceSign,id:9210,x:32359,y:32532,varname:node_9210,prsc:2,fstp:0;n:type:ShaderForge.SFN_Lerp,id:4987,x:32573,y:32389,cmnt:Combine outside phased mix and inside color,varname:node_4987,prsc:2|A-7561-RGB,B-9234-OUT,T-9210-VFACE;n:type:ShaderForge.SFN_Color,id:7561,x:32282,y:32271,ptovrint:False,ptlb:InsideColor,ptin:_InsideColor,varname:node_7561,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0.4344826,c4:1;n:type:ShaderForge.SFN_FaceSign,id:4525,x:32367,y:33319,varname:node_4525,prsc:2,fstp:0;n:type:ShaderForge.SFN_Lerp,id:1852,x:32588,y:33194,varname:node_1852,prsc:2|A-7519-OUT,B-2250-OUT,T-4525-VFACE;n:type:ShaderForge.SFN_Slider,id:2250,x:32210,y:33247,ptovrint:False,ptlb:OutsideAlpha,ptin:_OutsideAlpha,varname:node_2250,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Slider,id:7519,x:32210,y:33151,ptovrint:False,ptlb:InsideAlpha,ptin:_InsideAlpha,varname:_OutsideAlpha_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.4223226,max:1;n:type:ShaderForge.SFN_Code,id:678,x:31440,y:33566,varname:node_678,prsc:2,code:aABhAGwAZgAgAGQAaQBzAHQAIAA9ACAAbABlAG4AZwB0AGgAKABXAG8AcgBsAGQAUABvAHMAaQB0AGkAbwBuACAALQAgAFAAbABhAG4AZQBQAG8AcwBpAHQAaQBvAG4AKQA7AAoACgBmAGwAbwBhAHQAIABjAGwAaQBwAEQAaQBzAHQAIAA9ACAAZABpAHMAdAAgAC0AIABSAGEAZABpAHUAcwA7AAoACgBjAGwAaQBwACgAYwBsAGkAcABEAGkAcwB0ACkAOwAKAAoAcgBlAHQAdQByAG4AIAAoAGMAbABpAHAARABpAHMAdAAgAC8AIABDAG8AbgB2AGUAcgB0AGUAZABEAGkAcwB0AGEAbgBjAGUAKQA7AA==,output:0,fname:RadiusClip,width:648,height:383,input:2,input:2,input:0,input:0,input_1_label:WorldPosition,input_2_label:PlanePosition,input_3_label:Radius,input_4_label:ConvertedDistance|A-9467-XYZ,B-1923-XYZ,C-6050-OUT,D-1133-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6050,x:31022,y:33427,ptovrint:False,ptlb:Radius,ptin:_Radius,varname:node_6050,prsc:2,glob:True,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;proporder:5964-6665-7736-358-1813-1133-2871-7561-2250-7519;pass:END;sub:END;*/

Shader "Shader Forge/ClipPlanePBR" {
    Properties {
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Base Color", 2D) = "white" {}
        _Metallic ("Metallic", Range(0, 1)) = 0
        _Gloss ("Gloss", Range(0, 1)) = 0.8
        _ConvertDistance ("ConvertDistance", Float ) = 1
        _PhaseColor ("PhaseColor", Color) = (1,0.6519271,0.02941179,1)
        _InsideColor ("InsideColor", Color) = (1,0,0.4344826,1)
        _OutsideAlpha ("OutsideAlpha", Range(0, 1)) = 1
        _InsideAlpha ("InsideAlpha", Range(0, 1)) = 0.4223226
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
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform float _Metallic;
            uniform float _Gloss;
            uniform float _ConvertDistance;
            uniform float4 _PlanePosition;
            uniform float4 _PhaseColor;
            uniform float4 _InsideColor;
            uniform float _OutsideAlpha;
            uniform float _InsideAlpha;
            float RadiusClip( float3 WorldPosition , float3 PlanePosition , float Radius , float ConvertedDistance ){
            half dist = length(WorldPosition - PlanePosition);
            
            float clipDist = dist - Radius;
            
            clip(clipDist);
            
            return (clipDist / ConvertedDistance);
            }
            
            uniform float _Radius;
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
                UNITY_FOG_COORDS(7)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD8;
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
                #elif UNITY_SHOULD_SAMPLE_SH
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
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(i.uv0, _BumpMap)));
                float3 normalLocal = _BumpMap_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Gloss;
                float perceptualRoughness = 1.0 - _Gloss;
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
                float3 specularColor = _Metallic;
                float specularMonochrome;
                float ClipAmount = saturate(RadiusClip( i.posWorld.rgb , _PlanePosition.rgb , _Radius , _ConvertDistance ));
                float node_5043 = ClipAmount;
                float3 PhaseColor = lerp(float3(1,1,1),_PhaseColor.rgb,(1.0 - node_5043));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 diffuseColor = lerp(_InsideColor.rgb,(PhaseColor*(_MainTex_var.rgb*_Color.rgb)),isFrontFace); // Need this for specular when using metallic
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
                float3 Emission = lerp(_PhaseColor.rgb,float3(0,0,0),node_5043); // The output emission color applied on top of the base color
                float3 emissive = Emission;
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                fixed4 finalRGBA = fixed4(finalColor,lerp(_InsideAlpha,_OutsideAlpha,isFrontFace));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform float _Metallic;
            uniform float _Gloss;
            uniform float _ConvertDistance;
            uniform float4 _PlanePosition;
            uniform float4 _PhaseColor;
            uniform float4 _InsideColor;
            uniform float _OutsideAlpha;
            uniform float _InsideAlpha;
            float RadiusClip( float3 WorldPosition , float3 PlanePosition , float Radius , float ConvertedDistance ){
            half dist = length(WorldPosition - PlanePosition);
            
            float clipDist = dist - Radius;
            
            clip(clipDist);
            
            return (clipDist / ConvertedDistance);
            }
            
            uniform float _Radius;
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
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
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
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(i.uv0, _BumpMap)));
                float3 normalLocal = _BumpMap_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                UNITY_LIGHT_ATTENUATION(attenuation, i, i.posWorld.xyz);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Gloss;
                float perceptualRoughness = 1.0 - _Gloss;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = _Metallic;
                float specularMonochrome;
                float ClipAmount = saturate(RadiusClip( i.posWorld.rgb , _PlanePosition.rgb , _Radius , _ConvertDistance ));
                float node_5043 = ClipAmount;
                float3 PhaseColor = lerp(float3(1,1,1),_PhaseColor.rgb,(1.0 - node_5043));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 diffuseColor = lerp(_InsideColor.rgb,(PhaseColor*(_MainTex_var.rgb*_Color.rgb)),isFrontFace); // Need this for specular when using metallic
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
                fixed4 finalRGBA = fixed4(finalColor * lerp(_InsideAlpha,_OutsideAlpha,isFrontFace),0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
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
            #define UNITY_PASS_META 1
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _Metallic;
            uniform float _Gloss;
            uniform float _ConvertDistance;
            uniform float4 _PlanePosition;
            uniform float4 _PhaseColor;
            uniform float4 _InsideColor;
            float RadiusClip( float3 WorldPosition , float3 PlanePosition , float Radius , float ConvertedDistance ){
            half dist = length(WorldPosition - PlanePosition);
            
            float clipDist = dist - Radius;
            
            clip(clipDist);
            
            return (clipDist / ConvertedDistance);
            }
            
            uniform float _Radius;
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
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : SV_Target {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                float ClipAmount = saturate(RadiusClip( i.posWorld.rgb , _PlanePosition.rgb , _Radius , _ConvertDistance ));
                float node_5043 = ClipAmount;
                float3 Emission = lerp(_PhaseColor.rgb,float3(0,0,0),node_5043); // The output emission color applied on top of the base color
                o.Emission = Emission;
                
                float3 PhaseColor = lerp(float3(1,1,1),_PhaseColor.rgb,(1.0 - node_5043));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 diffColor = lerp(_InsideColor.rgb,(PhaseColor*(_MainTex_var.rgb*_Color.rgb)),isFrontFace);
                float specularMonochrome;
                float3 specColor;
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, _Metallic, specColor, specularMonochrome );
                float roughness = 1.0 - _Gloss;
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    
}
