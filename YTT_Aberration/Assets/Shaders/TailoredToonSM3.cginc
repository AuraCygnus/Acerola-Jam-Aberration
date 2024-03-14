#include "./TailoredToonCore.cginc"

v2f vert_forward_base(appdata_full v)
{
    UNITY_SETUP_INSTANCE_ID(v);
    v.normal = normalize(v.normal);
    return InitializeV2F(v, UnityObjectToClipPos(v.vertex), 0);
}

v2f vert_forward_base_outline(appdata_full v)
{
    UNITY_SETUP_INSTANCE_ID(v);
    v.normal = normalize(v.normal);
    float outlineTex = tex2Dlod(_OutlineWidthTexture, float4(TRANSFORM_TEX(v.texcoord, _MainTex), 0, 0)).r;
    return InitializeV2F(v, CalculateOutlineVertexClipPosition(v, outlineTex, _OutlineWidth, _OutlineScaledMaxDistance), 1);
}

v2f vert_forward_add(appdata_full v)
{
    UNITY_SETUP_INSTANCE_ID(v);
    v.normal = normalize(v.normal);
    return InitializeV2F(v, UnityObjectToClipPos(v.vertex), 0);
}

//
// Customised and Optimised Fragment shader
// No Matcap
//
float4 tailored_frag_forward(v2f i) : SV_TARGET
{
#ifdef MTOON_CLIP_IF_OUTLINE_IS_NONE
    #ifdef MTOON_OUTLINE_WIDTH_WORLD
    #elif MTOON_OUTLINE_WIDTH_SCREEN
    #else
        clip(-1);
    #endif
#endif

    //UNITY_TRANSFER_INSTANCE_ID(v, o);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

    // uv
    float2 mainUv = TRANSFORM_TEX(i.uv0, _MainTex);

#ifdef _UV_ANIMATE
    float uvAnim = tex2D(_UvAnimMaskTexture, mainUv).r * _Time.y;
    mainUv = AnimatedUVs(mainUv, uvAnim, _UvAnimScrollX, _UvAnimScrollY, _UvAnimRotation);
#endif

    // main tex
    half4 mainTex = tex2D(_MainTex, mainUv);

    // alpha
    half alpha = 1;
#ifdef _ALPHATEST_ON
    alpha = _Color.a * mainTex.a;
    alpha = (alpha - _Cutoff) / max(fwidth(alpha), EPS_COL) + 0.5; // Alpha to Coverage
    clip(alpha - _Cutoff);
    alpha = 1.0; // Discarded, otherwise it should be assumed to have full opacity
#endif
#ifdef _ALPHABLEND_ON
    alpha = _Color.a * mainTex.a;
#if !_ALPHATEST_ON && SHADER_API_D3D11 // Only enable this on D3D11, where I tested it
    clip(alpha - 0.0001);              // Slightly improves rendering with layered transparency
#endif
#endif

    // World View Direction
    float3 worldView = normalize(lerp(_WorldSpaceCameraPos.xyz - i.posWorld.xyz, UNITY_MATRIX_V[2].xyz, unity_OrthoParams.w));
    // normal
    float3 worldNormal = WorldNormal(_BumpMap, mainUv, i.tspace0, i.tspace1, i.tspace2, worldView, i.isOutline, _BumpScale);

    // Unity lighting
    UNITY_LIGHT_ATTENUATION(shadowAttenuation, i, i.posWorld.xyz);
    half3 lightDir = lerp(_WorldSpaceLightPos0.xyz, normalize(_WorldSpaceLightPos0.xyz - i.posWorld.xyz), _WorldSpaceLightPos0.w);
    half3 lightColor = _LightColor0.rgb * step(0.5, length(lightDir)); // length(lightDir) is zero if directional light is disabled.
    half dotNL = dot(lightDir, worldNormal);
#ifdef MTOON_FORWARD_ADD
    half lightAttenuation = 1;
#elif _SHADOWTEX_ON
    half lightAttenuation = shadowAttenuation * lerp(1, shadowAttenuation, _ReceiveShadowRate * tex2D(_ReceiveShadowTexture, mainUv).r);
#else
    half lightAttenuation = shadowAttenuation * lerp(1, shadowAttenuation, _ReceiveShadowRate);
#endif

    // Diffuse light term
    half diffuseTerm = TailoredDiffuse(dotNL, lightAttenuation, mainUv, EPS_COL, _ShadeShift, _ShadeToony);

    // Albedo color
    half4 lit = _Color * mainTex;
    half3 col = lit;

#ifdef _REFLECT_ON
    col = ApplyReflectColor(_ReflectTex, col, mainUv, i.worldRefl, _Reflectiveness);
#endif

    float3 hsv = rgb2hsv(col);
    hsv.rgb *= lerp(_MaterialShadeHSVScaling.rgb, _MaterialLitHSVScaling.rgb, diffuseTerm);
    col = hsv2rgb(hsv);

    // Direct Light
    half3 lighting = lightColor;
    lighting = lerp(lighting, max(EPS_COL, max(lighting.x, max(lighting.y, lighting.z))), _LightColorAttenuation); // color atten

    half directLightAttenuation = DirectLightAttenuation(dotNL, shadowAttenuation);
    col *= (lighting * directLightAttenuation);

    // Compute specular light if it's checked
    // Add specular after so that it doesn't get obliterated by Indirect Light
#if _SPECULAR_ON
    // Sample specular map
    half4 specMapTex = tex2D(_SpecTex, mainUv);
    // Calculate specular term
    float specTerm = StyledBlinnPhong(worldView, lightDir, worldNormal, specMapTex, dotNL, shadowAttenuation, _SpecPowerModifier, _SpecIntensityTex, _SpecIntensityTexWeight);
    // Add specular colour
    col += SpecColor(_SpecColourTex, specTerm, lightColor, mainUv, _SpecWeight, specMapTex);
#endif

    // Indirect Light
#ifdef MTOON_FORWARD_ADD
#else
    half3 indirectLighting = IndirectLighting(worldNormal, EPS_COL, _IndirectLightIntensity, _LightColorAttenuation);
    col += indirectLighting * lit;

    col = min(col, lit); // comment out if you want to PBR absolutely.
#endif

    // parametric rim lighting
#ifdef MTOON_FORWARD_ADD
    half3 staticRimLighting = 0;
    half3 mixedRimLighting = lighting;
#else
    half3 staticRimLighting = 1;
    half3 mixedRimLighting = lighting + indirectLighting;
#endif
    // Calculate Rim Term
    half rimTerm = RimLight(worldView, worldNormal, _RimLift, _RimFresnelPower);
    // Add rim light
    col += RimLightColor(rimTerm, staticRimLighting, mixedRimLighting, mainUv, i.isOutline, _RimLightingMix, _RimColor, _RimTexture);

    // Emission
#ifdef MTOON_FORWARD_ADD
#elif _EMISSION_ON
#if _EMISSIONPULSE_SIMPLE || _EMISSIONPULSE_POSITION || _EMISSIONPULSE_BOTH
    half3 emission = tex2D(_EmissionMap, mainUv).rgb * _EmissionColor.rgb * i.parameters.x;
#else
    half3 emission = tex2D(_EmissionMap, mainUv).rgb * _EmissionColor.rgb;
#endif
    col += lerp(emission, half3(0, 0, 0), i.isOutline);
#endif

    // outline
#ifdef MTOON_OUTLINE_COLOR_FIXED
    col = lerp(col, _OutlineColor, i.isOutline);
#elif MTOON_OUTLINE_COLOR_MIXED
    col = lerp(col, _OutlineColor * lerp(half3(1, 1, 1), col, _OutlineLightingMix), i.isOutline);
#else
#endif

    // debug
#ifdef MTOON_DEBUG_NORMAL
    #ifdef MTOON_FORWARD_ADD
        return float4(0, 0, 0, 0);
    #else
        return float4(worldNormal * 0.5 + 0.5, alpha);
    #endif
#elif MTOON_DEBUG_LITSHADERATE
    #ifdef MTOON_FORWARD_ADD
        return float4(0, 0, 0, 0);
    #else
        return float4(diffuseTerm * lighting, alpha);
    #endif
#endif

    half4 result = half4(col, alpha);
    UNITY_APPLY_FOG(i.fogCoord, result);
    return result;
}


