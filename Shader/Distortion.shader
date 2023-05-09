// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33211,y:32715,varname:node_3138,prsc:2|alpha-4077-OUT,refract-532-OUT;n:type:ShaderForge.SFN_Tex2d,id:5559,x:31878,y:32984,ptovrint:False,ptlb:Normal,ptin:_Normal,varname:node_5559,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:bbab0a6f7bae9cf42bf057d8ee2755f6,ntxv:3,isnm:True|UVIN-2910-OUT;n:type:ShaderForge.SFN_Multiply,id:532,x:32777,y:33025,varname:node_532,prsc:2|A-1608-OUT,B-375-OUT,C-7609-OUT;n:type:ShaderForge.SFN_Slider,id:375,x:31878,y:33156,ptovrint:False,ptlb:Refraction,ptin:_Refraction,varname:node_375,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.1538462,max:1;n:type:ShaderForge.SFN_ComponentMask,id:1608,x:32049,y:32984,varname:node_1608,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-5559-RGB;n:type:ShaderForge.SFN_Slider,id:4077,x:32828,y:32874,ptovrint:False,ptlb:Opacity,ptin:_Opacity,varname:node_4077,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Time,id:9527,x:32179,y:33359,varname:node_9527,prsc:2;n:type:ShaderForge.SFN_Slider,id:9864,x:32022,y:33506,ptovrint:False,ptlb:TimeSpeed,ptin:_TimeSpeed,varname:node_9864,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:1523,x:32369,y:33415,varname:node_1523,prsc:2|A-9527-T,B-9864-OUT;n:type:ShaderForge.SFN_Multiply,id:8027,x:32538,y:33382,varname:node_8027,prsc:2|A-9890-OUT,B-1523-OUT;n:type:ShaderForge.SFN_Vector1,id:9890,x:32340,y:33329,varname:node_9890,prsc:2,v1:0.25;n:type:ShaderForge.SFN_Add,id:919,x:32742,y:33357,varname:node_919,prsc:2|A-5715-OUT,B-8027-OUT;n:type:ShaderForge.SFN_Frac,id:3644,x:32742,y:33509,varname:node_3644,prsc:2|IN-8027-OUT;n:type:ShaderForge.SFN_Frac,id:2909,x:32909,y:33357,varname:node_2909,prsc:2|IN-919-OUT;n:type:ShaderForge.SFN_Vector1,id:5715,x:32538,y:33310,varname:node_5715,prsc:2,v1:1;n:type:ShaderForge.SFN_Set,id:6811,x:33092,y:33357,varname:Diff1,prsc:2|IN-2909-OUT;n:type:ShaderForge.SFN_Set,id:1720,x:32934,y:33509,varname:Diff2,prsc:2|IN-3644-OUT;n:type:ShaderForge.SFN_Get,id:3135,x:32214,y:33750,varname:node_3135,prsc:2|IN-6811-OUT;n:type:ShaderForge.SFN_Subtract,id:5748,x:32435,y:33694,varname:node_5748,prsc:2|A-3135-OUT,B-1812-OUT;n:type:ShaderForge.SFN_Vector1,id:1812,x:32214,y:33809,varname:node_1812,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Divide,id:8976,x:32639,y:33765,varname:node_8976,prsc:2|A-5748-OUT,B-1812-OUT;n:type:ShaderForge.SFN_Abs,id:9873,x:32817,y:33765,varname:node_9873,prsc:2|IN-8976-OUT;n:type:ShaderForge.SFN_Set,id:4692,x:32987,y:33765,varname:LerpValue,prsc:2|IN-9873-OUT;n:type:ShaderForge.SFN_Get,id:8471,x:31175,y:32883,varname:node_8471,prsc:2|IN-6811-OUT;n:type:ShaderForge.SFN_TexCoord,id:7738,x:31175,y:32949,varname:node_7738,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Get,id:6507,x:31175,y:33100,varname:node_6507,prsc:2|IN-1720-OUT;n:type:ShaderForge.SFN_Lerp,id:2910,x:31656,y:33001,varname:node_2910,prsc:2|A-2986-OUT,B-2598-OUT,T-4827-OUT;n:type:ShaderForge.SFN_Get,id:4827,x:31368,y:33175,varname:node_4827,prsc:2|IN-4692-OUT;n:type:ShaderForge.SFN_Add,id:2986,x:31389,y:32877,varname:node_2986,prsc:2|A-8471-OUT,B-7738-UVOUT;n:type:ShaderForge.SFN_Add,id:2598,x:31389,y:33022,varname:node_2598,prsc:2|A-7738-UVOUT,B-6507-OUT;n:type:ShaderForge.SFN_TexCoord,id:9412,x:31878,y:32801,varname:node_9412,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_RemapRange,id:9959,x:32158,y:32799,varname:node_9959,prsc:2,frmn:0,frmx:1,tomn:0,tomx:1.2|IN-9412-V;n:type:ShaderForge.SFN_OneMinus,id:7609,x:32341,y:32799,varname:node_7609,prsc:2|IN-9959-OUT;proporder:5559-375-4077-9864;pass:END;sub:END;*/

Shader "Shader Forge/Distortion" {
    Properties {
        _Normal ("Normal", 2D) = "bump" {}
        _Refraction ("Refraction", Range(0, 1)) = 0.1538462
        _Opacity ("Opacity", Range(0, 1)) = 0
        _TimeSpeed ("TimeSpeed", Range(-1, 1)) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _Refraction;
            uniform float _Opacity;
            uniform float _TimeSpeed;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 projPos : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 node_9527 = _Time;
                float node_8027 = (0.25*(node_9527.g*_TimeSpeed));
                float Diff1 = frac((1.0+node_8027));
                float Diff2 = frac(node_8027);
                float node_1812 = 0.5;
                float LerpValue = abs(((Diff1-node_1812)/node_1812));
                float2 node_2910 = lerp((Diff1+i.uv0),(i.uv0+Diff2),LerpValue);
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_2910, _Normal)));
                float2 sceneUVs = (i.projPos.xy / i.projPos.w) + (_Normal_var.rgb.rg*_Refraction*(1.0 - (i.uv0.g*1.2+0.0)));
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
////// Lighting:
                float3 finalColor = 0;
                return fixed4(lerp(sceneColor.rgb, finalColor,_Opacity),1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    
}
