Shader "Tailored Toon/Main"
{
    Properties
    {
        _Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
        _Color ("Lit Color + Alpha", Color) = (1,1,1,1)
        [NoScaleOffset] _MainTex ("Lit Texture + Alpha", 2D) = "white" {}
        _BumpScale ("Normal Scale", Float) = 1.0
        [Normal] _BumpMap ("Normal Texture", 2D) = "bump" {}

        _ReceiveShadowRate ("Receive Shadow", Range(0, 1)) = 1
        [Toggle] _ShadowTex("ShadowTex", Float) = 0.0
        [NoScaleOffset] _ReceiveShadowTexture ("Receive Shadow Texture", 2D) = "white" {}

        _ShadeShift ("Shade Shift", Range(-1, 1)) = 0
        _ShadeToony ("Shade Toony", Range(0, 1)) = 0.9
        _LightColorAttenuation ("Light Color Attenuation", Range(0, 1)) = 0
        _IndirectLightIntensity ("Indirect Light Intensity", Range(0, 1)) = 0.1

        [HDR] _RimColor ("Rim Color", Color) = (0,0,0)
        [Toggle] _RimTex("RimTex", Float) = 0.0
        [NoScaleOffset] _RimTexture ("Rim Texture", 2D) = "white" {}
        _RimLightingMix ("Rim Lighting Mix", Range(0, 1)) = 0
        [PowerSlider(4.0)] _RimFresnelPower ("Rim Fresnel Power", Range(0, 100)) = 1
        _RimLift ("Rim Lift", Range(0, 1)) = 0

        [Toggle] _Emission("Emission", Float) = 0.0
        [HDR] _EmissionColor ("Color", Color) = (0,0,0)
        [NoScaleOffset] _EmissionMap ("Emission", 2D) = "white" {}
        [KeywordEnum(None, Simple, Position, Both)] _EmissionPulse("Emission Pulse Type", Float) = 0.0
        _EmissionSimplePulseRange("Emission Simple Pulse Min & Max", Vector) = (0, 1, 0, 0)
        _EmissionSimplePulseTimeWeights("Emission Simple Pulse Time Weights", Vector) = (1, 1, 1, 1)
        _EmissionPositionPulseScale("Emission Position Pulse Scale", Range(0.001, 50)) = 8
        _EmissionPositionPulseRange("Emission Position Pulse Min & Max", Vector) = (0, 1, 0, 0)
        _EmissionPositionPulseTimeWeights("Emission Position Pulse Time Weights", Vector) = (1, 1, 1, 1)

        [NoScaleOffset] _OutlineWidthTexture ("Outline Width Tex", 2D) = "white" {}
        _OutlineWidth ("Outline Width", Range(0.01, 1)) = 0.5
        _OutlineScaledMaxDistance ("Outline Scaled Max Distance", Range(1, 10)) = 1
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineLightingMix ("Outline Lighting Mix", Range(0, 1)) = 1

        [Toggle] _UVAnimate("UV Animation", Float) = 0.0
        [NoScaleOffset] _UvAnimMaskTexture ("UV Animation Mask", 2D) = "white" {}
        _UvAnimScrollX ("UV Animation Scroll X", Float) = 0
        _UvAnimScrollY ("UV Animation Scroll Y", Float) = 0
        _UvAnimRotation ("UV Animation Rotation", Float) = 0

        [Toggle] _Reflect("Reflect", Float) = 0.0
        _Reflectiveness("Reflectiveness", Range(0, 1)) = 1
        [NoScaleOffset] _ReflectTex("Reflectance Texture", 2D) = "white" {}

        [Toggle] _Specular("Specular", Float) = 0.0
        _SpecWeight("Specular Weight", Range(0, 10)) = 1
        _SpecPowerModifier("Glossiness", Range(0.1, 100)) = 1
        _SpecIntensityTexWeight("Specular Texture Weight", Range(0, 1)) = 0.5
        [NoScaleOffset] _SpecTex("Specular Map Texture", 2D) = "white" {}
        [NoScaleOffset] _SpecColourTex("Specular Color Texture", 2D) = "white" {}
        [NoScaleOffset] _SpecIntensityTex("Specular Intensity Texture", 2D) = "white" {}

        _MaterialLitHSVScaling("Lit HSV", Vector) = (1, 1, 1, 1)
        _MaterialShadeHSVScaling("Shade HSV", Vector) = (1, 1.1, 0.4, 1)

        [HideInInspector] _MToonVersion ("_MToonVersion", Float) = 38
        [HideInInspector] _DebugMode ("_DebugMode", Float) = 0.0
        [HideInInspector] _BlendMode ("_BlendMode", Float) = 0.0
        [HideInInspector] _OutlineWidthMode ("_OutlineWidthMode", Float) = 0.0
        [HideInInspector] _OutlineColorMode ("_OutlineColorMode", Float) = 0.0
        [HideInInspector] _CullMode ("_CullMode", Float) = 2.0
        [HideInInspector] _OutlineCullMode ("_OutlineCullMode", Float) = 1.0
        [HideInInspector] _SrcBlend ("_SrcBlend", Float) = 1.0
        [HideInInspector] _DstBlend ("_DstBlend", Float) = 0.0
        [HideInInspector] _ZWrite ("_ZWrite", Float) = 1.0
        [HideInInspector] _AlphaToMask ("_AlphaToMask", Float) = 0.0
    }

    // for SM 3.0
    SubShader
    {
        Tags { "RenderType" = "Opaque"  "Queue" = "Geometry" }

        // Forward Base
        Pass
        {
            Name "FORWARD_BASE"
            Tags { "LightMode" = "ForwardBase" }

            Cull [_CullMode]
            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]
            ZTest LEqual
            BlendOp Add, Max
            AlphaToMask [_AlphaToMask]

            CGPROGRAM
            #pragma target 3.0
            #pragma shader_feature _ MTOON_DEBUG_NORMAL MTOON_DEBUG_LITSHADERATE
            #pragma multi_compile _ _NORMALMAP
            #pragma multi_compile _ _ALPHATEST_ON _ALPHABLEND_ON
            #pragma shader_feature __ _EMISSION_ON
            #pragma shader_feature __ _EMISSIONPULSE_SIMPLE _EMISSIONPULSE_POSITION _EMISSIONPULSE_BOTH
            #pragma shader_feature __ _SHADOWTEX_ON
            #pragma shader_feature __ _RIMTEX_ON
            #pragma shader_feature __ _UV_ANIMATE
            #pragma shader_feature __ _REFLECT_ON
            #pragma shader_feature __ _SPECULAR_ON
            #include "./TailoredToonSM3.cginc"
            #pragma vertex vert_forward_base
            #pragma fragment tailored_frag_forward
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
//            #pragma multi_compile_instancing
            ENDCG
        }

        // Forward Base Outline Pass
        Pass
        {
            Name "FORWARD_BASE_ONLY_OUTLINE"
            Tags { "LightMode" = "ForwardBase" }

            Cull [_OutlineCullMode]
            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]
            ZTest LEqual
            Offset 1, 1
            BlendOp Add, Max
            AlphaToMask [_AlphaToMask]

            CGPROGRAM
            #pragma target 3.0
            #pragma shader_feature _ MTOON_DEBUG_NORMAL MTOON_DEBUG_LITSHADERATE
            #pragma multi_compile _ MTOON_OUTLINE_WIDTH_WORLD MTOON_OUTLINE_WIDTH_SCREEN
            #pragma multi_compile _ MTOON_OUTLINE_COLOR_FIXED MTOON_OUTLINE_COLOR_MIXED
            #pragma multi_compile _ _NORMALMAP
            #pragma multi_compile _ _ALPHATEST_ON _ALPHABLEND_ON
            #pragma shader_feature __ _EMISSION_ON
            #pragma shader_feature __ _EMISSIONPULSE_SIMPLE _EMISSIONPULSE_POSITION _EMISSIONPULSE_BOTH
            #pragma shader_feature __ _UV_ANIMATE
            #pragma shader_feature __ _SHADOWTEX_ON
            #pragma shader_feature __ _RIMTEX_ON
            #define MTOON_CLIP_IF_OUTLINE_IS_NONE
            #include "./TailoredToonSM3.cginc"
            #pragma vertex vert_forward_base_outline
            #pragma fragment tailored_frag_forward
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
//            #pragma multi_compile_instancing
            ENDCG
        }

        
        // Forward Add
        Pass
        {
            Name "FORWARD_ADD"
            Tags { "LightMode" = "ForwardAdd" }

            Cull [_CullMode]
            Blend [_SrcBlend] One
            ZWrite Off
            ZTest LEqual
            BlendOp Add, Max
            AlphaToMask [_AlphaToMask]

            CGPROGRAM
            #pragma target 3.0
            #pragma shader_feature _ MTOON_DEBUG_NORMAL MTOON_DEBUG_LITSHADERATE
            #pragma multi_compile _ _UV_ANIMATE
            #pragma multi_compile _ _NORMALMAP
            #pragma multi_compile _ _ALPHATEST_ON _ALPHABLEND_ON
            #pragma shader_feature __ _UV_ANIMATE
            #pragma shader_feature __ _SHADOWTEX_ON
            #pragma shader_feature __ _RIMTEX_ON
            #pragma shader_feature __ _REFLECT_ON
            #pragma shader_feature __ _SPECULAR_ON
            #define MTOON_FORWARD_ADD
            #include "./TailoredToonSM3.cginc"
            #pragma vertex vert_forward_add
            #pragma fragment tailored_frag_forward
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            ENDCG
        }

        //  Shadow rendering pass
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            Cull [_CullMode]
            ZWrite On
            ZTest LEqual

            CGPROGRAM
            #pragma target 3.0
            #pragma multi_compile _ _ALPHATEST_ON _ALPHABLEND_ON
            #pragma multi_compile_shadowcaster
            #pragma vertex vertShadowCaster
            #pragma fragment fragShadowCaster
            #include "UnityStandardShadow.cginc"
            ENDCG
        }
    }

    Fallback "Unlit/Texture"
    CustomEditor "MToonPlus.TailoredToonInspector"
}
