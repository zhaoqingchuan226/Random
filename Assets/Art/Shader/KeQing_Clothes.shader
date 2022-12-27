Shader "KeQing"
{
    Properties
    {
        [Header(Base)]
        _Color("Color",Color) = (1,1,1,1)
        _MainTex("MainTex",2D) = "white"{}
        _Ref("Ref",int)=1
        _RimColor("RimColor",Color) = (1,1,1,1)

        [Header(OutLine)]

        _OutLineColor("OutLineColor",Color)=(0,0,0,0)
        _OutLineWidth("_OutLineWidth",Range(0,0.2))=0.1
        _WidthControl("WidthControl",Range(0,1))=0

        [Header(Shadow)]
        _ShadowStep("ShadowStep",float)=0.3
        [IntRange]_StepAmount("StepAmount",Range(1,5))=2

        [Header(Ramp)]
        _Ramp("Ramp",2D)="white"

        [Header(Rim)]
        _RimStep("RimStep",float)=0.5
        _RimIntensity("_RimIntensity",float)=1
        _RimContrast("RimContrast",float)=1

        [Header(MatCap)]
        [Toggle]_MatCap("MatCap",int)=0
        _MatCapTex("MatCap",2D)="black"{}
        _MatCapColor("MatCapColor",Color)=(1,1,1,1)

        [Header(Specular)]
        [Toggle]_Specular("Specular",int)=0
        _SpecularIntensity("SpecularIntensity",float)=1
        _SpecularStep("SpecularStep",Range(0,1))=0.3
        _Grossness("Grossness",float)=40
        _SpecularColor("SpecularColor",Color)=(1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            //告诉引擎此SubShader是用于URP渲染管线下的
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
            "Queue"="Geometry+0"
        }
        
        Pass
        {
            
            Tags 
            { 
                "LightMode" = "UniversalForward"
            }
            
            // Render State
            Blend One Zero, One Zero
            Cull off
            ZTest LEqual
            ZWrite On
            // ColorMask: <None>
            Stencil
            {
                Ref [_Ref]
                Comp Always
                Pass Replace
            }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            // Pragmas
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 4.0
            #pragma multi_compile_instancing
            #pragma shader_feature _MATCAP_ON
            #pragma shader_feature _SPECULAR_ON
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            

            

            CBUFFER_START(UnityPerMaterial)
            half4 _Color;
            float _ShadowStep;
            float _StepAmount;
            half4 _RimColor;
            float _RimIntensity;
            float _RimContrast;
            float _RimStep;
            float _SpecularIntensity;
            float _Grossness;
            float _SpecularStep;
            half4 _SpecularColor;
            half4 _MatCapColor;
            CBUFFER_END

            // sampler2D _MainTex;
            TEXTURE2D(_MainTex);    //纹理的定义，如果是编绎到GLES2.0平台，则相当于sampler2D _MainTex;否则就相当于Texture2D _MainTex;
            float4 _MainTex_ST;
            // SAMPLER(sampler_MainTex);   //采样器的定义，如果是编绎到GLES2.0平台，就相当于空，否则就相当于SamplerState sampler_MainTex;
            #define smp _linear_clampU_mirrorV
            SAMPLER(smp);
            TEXTURE2D(_Ramp);
            SAMPLER(sampler_Ramp);
            TEXTURE2D(_MatCapTex);
            

            //顶点着色器的输入(模型的数据信息)
            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv:TEXCOORD;
                float3 normal:NORMAL;
            };
            
            //顶点着色器的输出
            struct Varyings
            {
                float4 positionCS : SV_Position;
                float2 uv:TEXCOORD;
                float3 normalWS:TEXCOORD1;
                float3 viewWS:TEXCOORD2;
                float3 vertexColor:TEXCOORD3;
                float4 shadowCoord: TEXCOORD4;
                
            };

            // v2f vert(appdata v)
            // 顶点着色器
            Varyings vert(Attributes v)
            {
                Varyings o = (Varyings)0;
                float3 positionWS = TransformObjectToWorld(v.positionOS);
                o.positionCS = TransformWorldToHClip(positionWS);
                o.uv = TRANSFORM_TEX(v.uv,_MainTex);
                o.normalWS=TransformObjectToWorldNormal(v.normal);
                o.viewWS=normalize(_WorldSpaceCameraPos-positionWS);
                o.vertexColor=SAMPLE_TEXTURE2D_LOD(_MainTex, smp, o.uv,6);
                o.shadowCoord = TransformWorldToShadowCoord(positionWS);
                return o;
            }

            //fixed4 frag(v2f i):SV_TARGET
            //片断着色器
            half4 frag(Varyings i) : SV_TARGET 
            {    
                half4 c;
                
                //normalize
                i.normalWS=normalize(i.normalWS);
                i.viewWS=normalize(i.viewWS);

                //albedo
                half4 mainTex = SAMPLE_TEXTURE2D(_MainTex,smp,i.uv);
                c = mainTex * _Color;
                
                //lightatten
                half3 N=i.normalWS;
                Light mainLight=GetMainLight(i.shadowCoord);
                half3 L=mainLight.direction;
                half halfLambert=(0.5*dot(N,L)*+0.5);
                halfLambert*=mainLight.shadowAttenuation*mainLight.color.r;
          
                
                
                
                
                //算法分级 half atten=ceil(_StepAmount*halfLambert)/_StepAmount;
                // 最暗限制atten=max(0.65,atten);
                half3 atten=SAMPLE_TEXTURE2D(_Ramp,sampler_Ramp,float2(0.3,halfLambert)).xyz;
                
                c.rgb*=atten;
                //rim
                half3 V=i.viewWS;
                half NdotV=dot(N,V);
                half3 rim=pow(smoothstep(_RimStep,_RimStep+0.04,saturate(1-NdotV)),_RimContrast)*_RimIntensity*_RimColor.rgb;
                rim*=i.vertexColor;
                
                #ifdef _MATCAP_ON
                    //matcap
                    {
                        half3 normalVS=mul(i.normalWS,unity_WorldToCamera);
                        half3 matCap=SAMPLE_TEXTURE2D(_MatCapTex,smp,normalVS.xy*0.5+0.5).xyz;
                        
                        c.rgb+=matCap.rgb*_MatCapColor*3;
                    }
                #endif

                #ifdef _SPECULAR_ON
                    half3 H=normalize(L+V);
                    
                    N=normalize(N);
                    half NdotH=dot(N,H);
                    NdotH=smoothstep(_SpecularStep,_SpecularStep,NdotH);
                    half3 specular=pow(NdotH,_Grossness)*_SpecularIntensity*_SpecularColor;
                    
                    c.rgb+=specular;
                #endif

                c.rgb+=rim;
                return c;
            }

            ENDHLSL
        }   
        Pass
        {
            Name "OutLine"
            Tags 
            { 
                "LightMode" = "OutLine"
            }
            
            // Render State
            Blend One Zero, One Zero
            Cull back
            ZTest LEqual
            ZWrite On
            // ColorMask: <None>
            Stencil
            {
                Ref [_Ref]
                Comp NotEqual
                
            }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            // Pragmas
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma multi_compile_instancing
            
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            

            CBUFFER_START(UnityPerMaterial)
            half4 _OutLineColor;
            float _OutLineWidth;
            float _WidthControl;
            CBUFFER_END

            TEXTURE2D(_MainTex);  
            float4 _MainTex_ST;
            
            #define smp _linear_clampU_mirrorV
            SAMPLER(smp);

            

            //顶点着色器的输入(模型的数据信息)
            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normal:NORMAL;
                float4 tangent:TANGENT;
                float2 uv:TEXCOORD;
            };
            
            //顶点着色器的输出
            struct Varyings
            {
                float4 positionCS : SV_Position;
                float3 vertexColor:TEXCOORD;
            };

            // v2f vert(appdata v)
            // 顶点着色器
            Varyings vert(Attributes v)
            {
                Varyings o = (Varyings)0;
                float3 positionOS=v.positionOS;
                float3 positionWS = TransformObjectToWorld(positionOS);
                float distance=length(_WorldSpaceCameraPos-positionWS);
                distance=lerp(1,distance*0.1,_WidthControl);
                
                
                
                positionOS+=normalize(v.tangent.xyz)*_OutLineWidth*distance;
                o.positionCS = TransformObjectToHClip(positionOS);
                o.vertexColor=SAMPLE_TEXTURE2D_LOD(_MainTex,smp,v.uv,4);
                return o;
            }

            //fixed4 frag(v2f i):SV_TARGET
            //片断着色器
            half4 frag(Varyings i) : SV_TARGET 
            {    
                // return _OutLineColor;
                return half4(pow(i.vertexColor,2)*_OutLineColor.rgb,1);
            }

            ENDHLSL
        } 
        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _GLOSSINESS_FROM_BASE_ALPHA

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }
    }
    
    FallBack "Hidden/Shader Graph/FallbackError"
}
