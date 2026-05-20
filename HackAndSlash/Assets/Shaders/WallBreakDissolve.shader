Shader "Shader Graphs/WallBreakDissolve"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1, 1, 1, 1)
        _BreakAmount ("Break Amount", Range(0, 1)) = 0
        _EdgeColor ("Break Edge Color", Color) = (1, 0.55, 0.12, 1)
        _EdgeWidth ("Break Edge Width", Range(0.01, 0.25)) = 0.08
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "WallBreakDissolve"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _BreakAmount;
                float4 _EdgeColor;
                float _EdgeWidth;
            CBUFFER_END

            float Noise(float2 uv)
            {
                float2 p = floor(uv * 18.0);
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.color = input.color * _Color;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 spriteColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) * input.color;
                float noise = Noise(input.uv);
                float dissolveMask = smoothstep(_BreakAmount, _BreakAmount + _EdgeWidth, noise);
                float edgeMask = 1.0 - smoothstep(_BreakAmount + _EdgeWidth, _BreakAmount + (_EdgeWidth * 2.0), noise);

                spriteColor.rgb = lerp(spriteColor.rgb, _EdgeColor.rgb, saturate(edgeMask) * _BreakAmount);
                spriteColor.a *= dissolveMask;
                return spriteColor;
            }
            ENDHLSL
        }
    }
}
