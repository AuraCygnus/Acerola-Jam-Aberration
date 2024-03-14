#ifndef MTOON_CORE_INCLUDED
#define MTOON_CORE_INCLUDED

#include "Lighting.cginc"
#include "AutoLight.cginc"

//UNITY_INSTANCING_BUFFER_START(Props)
//UNITY_INSTANCING_BUFFER_END(Props)

// const
const float EPS_COL = 0.00001;

half _Cutoff;
fixed4 _Color;
sampler2D _MainTex;
float4 _MainTex_ST;

half _BumpScale;
sampler2D _BumpMap;

sampler2D _ReceiveShadowTexture;
half _ReceiveShadowRate;
half _ShadeShift;
half _ShadeToony;
half _LightColorAttenuation;
half _IndirectLightIntensity;

sampler2D _RimTexture;
half4 _RimColor;
half _RimLightingMix;
half _RimFresnelPower;
half _RimLift;

half4 _EmissionColor;
sampler2D _EmissionMap;
half4 _EmissionSimplePulseRange;
half4 _EmissionSimplePulseTimeWeights;
half _EmissionPositionPulseScale;
half4 _EmissionPositionPulseRange;
half4 _EmissionPositionPulseTimeWeights;

sampler2D _OutlineWidthTexture;
half _OutlineWidth;
half _OutlineScaledMaxDistance;
fixed4 _OutlineColor;
half _OutlineLightingMix;

// NOTE: "tex2d() * _Time.y" returns mediump value if sampler is half precision in Android VR platform
sampler2D_float _UvAnimMaskTexture;
float _UvAnimScrollX;
float _UvAnimScrollY;
float _UvAnimRotation;

// Lerp value for reflectiveness
float _Reflectiveness;
sampler2D _ReflectTex;

// Weight of specular to apply
float _SpecWeight;
// Multiplier for Specular power
float _SpecPowerModifier;
// Weight to apply _SpecIntensityTex to calculated Intensity vs straight intensity value
float _SpecIntensityTexWeight;
sampler2D _SpecTex;
sampler2D _SpecColourTex;
sampler2D _SpecIntensityTex;

// HSV values for balanced shading approach
half4 _MaterialLitHSVScaling;
half4 _MaterialShadeHSVScaling;

struct v2f
{
    float4 pos : SV_POSITION;
    float4 posWorld : TEXCOORD0;
    half3 tspace0 : TEXCOORD1;
    half3 tspace1 : TEXCOORD2;
    half3 tspace2 : TEXCOORD3;
    float2 uv0 : TEXCOORD4;
    float isOutline : TEXCOORD5;
    fixed4 color : TEXCOORD6;
    UNITY_FOG_COORDS(7)
    UNITY_SHADOW_COORDS(8)
    // Selection of single value parameters
    // x = Emission Pulse
    half4 parameters : TEXCOORD9;
#if _REFLECT_ON
    half3 worldRefl : TEXCOORD10;
#endif

    //UNITY_VERTEX_INPUT_INSTANCE_ID // necessary only if any instanced properties are going to be accessed in the fragment Shader.
    UNITY_VERTEX_OUTPUT_STEREO
};

half EmissionPulseSimple(half4 pulseRange, half4 pulseWeights)
{
    // Normalize pulse into range [0,1]
    half4 pulse = (_SinTime + 1.0f) * 0.5f;
    // Apply weights to pulse elements
    half4 pulseTime = pulse * pulseWeights;
    // Combine pulse
    half emissionPulseTime = 0.25f * (pulseTime.x + pulseTime.y + pulseTime.z + pulseTime.w);
    // Clamp pulse between min and max values
    return lerp(pulseRange.x, pulseRange.y, emissionPulseTime) * pulseRange.z;
}

half EmissionPulsePosition(half4 pulseRange, half4 pulseWeights, float4 posWorld)
{
    float posTime = (posWorld.x + posWorld.y + posWorld.z) * _Time.y * _EmissionPositionPulseScale;
    float4 sinTime = float4(sin(posTime), sin(posTime * 0.5f), sin(posTime * 0.25f), sin(posTime * 0.125f));
    // Normalize pulse into range [0,1]
    half4 pulse = (sinTime + 1.0f) * 0.5f;
    // Apply weights to pulse elements
    half4 pulseTime = pulse * pulseWeights;
    // Combine pulse
    half emissionPulseTime = 0.25f * (pulseTime.x + pulseTime.y + pulseTime.z + pulseTime.w);
    // Clamp pulse between min and max values
    return lerp(pulseRange.x, pulseRange.y, emissionPulseTime) * pulseRange.z;
}

half EmissionPulse(float4 posWorld)
{
#if (_EMISSIONPULSE_SIMPLE)
    return saturate(EmissionPulseSimple(_EmissionSimplePulseRange, _EmissionSimplePulseTimeWeights));
#elif (_EMISSIONPULSE_POSITION)
    return saturate(EmissionPulsePosition(_EmissionPositionPulseRange, _EmissionPositionPulseTimeWeights, posWorld));
#elif (_EMISSIONPULSE_BOTH)
    return saturate(EmissionPulseSimple(_EmissionSimplePulseRange, _EmissionSimplePulseTimeWeights) + EmissionPulsePosition(_EmissionPositionPulseRange, _EmissionPositionPulseTimeWeights, posWorld));
#else
    return 1.0f;
#endif
}

inline v2f InitializeV2F(appdata_full v, float4 projectedVertex, float isOutline)
{
    v2f o;
    UNITY_INITIALIZE_OUTPUT(v2f, o);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    //UNITY_TRANSFER_INSTANCE_ID(v, o);

    o.pos = projectedVertex;
    o.posWorld = mul(unity_ObjectToWorld, v.vertex);
    o.uv0 = v.texcoord;
    half3 worldNormal = UnityObjectToWorldNormal(v.normal);
    half3 worldTangent = UnityObjectToWorldDir(v.tangent);
    half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
    half3 worldBitangent = cross(worldNormal, worldTangent) * tangentSign;
    o.tspace0 = half3(worldTangent.x, worldBitangent.x, worldNormal.x);
    o.tspace1 = half3(worldTangent.y, worldBitangent.y, worldNormal.y);
    o.tspace2 = half3(worldTangent.z, worldBitangent.z, worldNormal.z);
    o.isOutline = isOutline;
    o.color = v.color;
    UNITY_TRANSFER_SHADOW(o, o._ShadowCoord);
    UNITY_TRANSFER_FOG(o, o.pos);

#if _EMISSION_ON
    o.parameters.x = EmissionPulse(o.posWorld);
#endif

#ifdef _REFLECT_ON
    // compute world space view direction
    float3 worldViewDir = normalize(UnityWorldSpaceViewDir(o.posWorld.xyz));
    // world space reflection vector
    o.worldRefl = reflect(-worldViewDir, worldNormal);
#endif

    return o;
}

inline float4 CalculateOutlineVertexClipPosition(appdata_full v, float outlineTex, half outlineWidth, half outlineScaledMaxDistance)
{
 #if defined(MTOON_OUTLINE_WIDTH_WORLD)
    float3 worldNormalLength = length(mul((float3x3)transpose(unity_WorldToObject), v.normal));
    float3 outlineOffset = 0.01 * outlineWidth * outlineTex * worldNormalLength * v.normal;
    float4 vertex = UnityObjectToClipPos(v.vertex + outlineOffset);
 #elif defined(MTOON_OUTLINE_WIDTH_SCREEN)
    float4 nearUpperRight = mul(unity_CameraInvProjection, float4(1, 1, UNITY_NEAR_CLIP_VALUE, _ProjectionParams.y));
    float aspect = abs(nearUpperRight.y / nearUpperRight.x);
    float4 vertex = UnityObjectToClipPos(v.vertex);
    float3 viewNormal = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal.xyz);
    float3 clipNormal = TransformViewToProjection(viewNormal.xyz);
    float2 projectedNormal = normalize(clipNormal.xy);
    projectedNormal *= min(vertex.w, outlineScaledMaxDistance);
    projectedNormal.x *= aspect;
    vertex.xy += 0.01 * outlineWidth * outlineTex * projectedNormal.xy * saturate(1 - abs(normalize(viewNormal).z)); // ignore offset when normal toward camera
 #else
    float4 vertex = UnityObjectToClipPos(v.vertex);
 #endif
    return vertex;
}

float2 AnimatedUVs(float2 mainUv, float uvAnim, float uvAnimScrollX, float uvAnimScrollY, float uvAnimRotation)
{
    // uv anim
    const float PI_2 = 6.28318530718;

    // translate uv in bottom-left origin coordinates.
    mainUv += float2(uvAnimScrollX, uvAnimScrollY) * uvAnim;
    // rotate uv counter-clockwise around (0.5, 0.5) in bottom-left origin coordinates.
    float rotateRad = uvAnimRotation * PI_2 * uvAnim;
    const float2 rotatePivot = float2(0.5, 0.5);
    return mul(float2x2(cos(rotateRad), -sin(rotateRad), sin(rotateRad), cos(rotateRad)), mainUv - rotatePivot) + rotatePivot;
}

half3 WorldNormal(sampler2D bumpMap, float2 mainUv, half3 tspace0, half3 tspace1, half3 tspace2, float3 worldView, float isOutline, half bumpScale)
{
#ifdef _NORMALMAP
    half3 tangentNormal = UnpackScaleNormal(tex2D(bumpMap, mainUv), bumpScale);
    half3 worldNormal;
    worldNormal.x = dot(tspace0, tangentNormal);
    worldNormal.y = dot(tspace1, tangentNormal);
    worldNormal.z = dot(tspace2, tangentNormal);
#else
    half3 worldNormal = half3(tspace0.z, tspace1.z, tspace2.z);
#endif

    worldNormal *= step(0, dot(worldView, worldNormal)) * 2 - 1; // flip if projection matrix is flipped
    worldNormal *= lerp(+1.0, -1.0, isOutline);
    return normalize(worldNormal);
}

half MToonDiffuse(half dotNL, half lightAttenuation, float2 mainUv, float EPS_COL, half shadingGradeRate, sampler2D shadingGradeTexture, half shadeShift, half shadeToony)
{
    // Decide albedo color rate from Direct Light
    half shadingGrade = 1.0 - shadingGradeRate * (1.0 - tex2D(shadingGradeTexture, mainUv).r);
    // Half Lambert
    half diffuse = dotNL; // [-1, +1]
    diffuse = diffuse * 0.5 + 0.5; // from [-1, +1] to [0, 1]
    diffuse = diffuse * lightAttenuation; // receive shadow
    // Apply shadingGrade
    diffuse = diffuse * shadingGrade; // darker
    // Reverse Half Lambert?
    diffuse = diffuse * 2.0 - 1.0; // from [0, 1] to [-1, +1]
    // tooned. mapping from [minIntensityThreshold, maxIntensityThreshold] to [0, 1]
    half maxIntensityThreshold = lerp(1, shadeShift, shadeToony);
    half minIntensityThreshold = shadeShift;
    return saturate((diffuse - minIntensityThreshold) / max(EPS_COL, (maxIntensityThreshold - minIntensityThreshold)));
}

///
/// Optimised Diffuse
/// Removed shading grade
/// 
half TailoredDiffuse(half dotNL, half lightAttenuation, float2 mainUv, float EPS_COL, half shadeShift, half shadeToony)
{
    // Half Lambert
    half diffuse = dotNL; // [-1, +1]
    diffuse = diffuse * 0.5 + 0.5; // from [-1, +1] to [0, 1]
    diffuse = diffuse * lightAttenuation; // receive shadow
    // Reverse Half Lambert?
    diffuse = diffuse * 2.0 - 1.0; // from [0, 1] to [-1, +1]
    // tooned. mapping from [minIntensityThreshold, maxIntensityThreshold] to [0, 1]
    half maxIntensityThreshold = lerp(1, shadeShift, shadeToony);
    half minIntensityThreshold = shadeShift;
    return saturate((diffuse - minIntensityThreshold) / max(EPS_COL, (maxIntensityThreshold - minIntensityThreshold)));
}

float BlinnPhong(half3 worldView, half3 worldLightDir, half3 worldNormal, half4 specMapTex, float dotNL, float shadowAttenuation, float specPowerModifier)
{
    // Half Vector
    float3 halfVector = normalize(worldLightDir + worldView);
    float NdotH = saturate(dot(worldNormal, halfVector));

    float selfShadowTerm = saturate(4 * dotNL);

    return saturate(selfShadowTerm * shadowAttenuation * pow(NdotH, specPowerModifier) * specMapTex.g);
}

float StyledBlinnPhong(half3 worldView, half3 worldLightDir, half3 worldNormal, half4 specMapTex, float dotNL, float shadowAttenuation, float specPowerModifier, sampler2D specIntensityTex, float specIntensityTexWeight)
{
    float specIntensity = BlinnPhong(worldView, worldLightDir, worldNormal, specMapTex, dotNL, shadowAttenuation, specPowerModifier);
    // Threshold control texture to allow toon styling of specular
    float specIntensityTexVal = tex2D(specIntensityTex, float2(specIntensity, specIntensity)).x;
    float scaledSpecIntensity = saturate(specIntensityTexVal * specIntensity);
    return lerp(scaledSpecIntensity, specIntensity, specIntensityTexWeight);
}

half3 SpecColor(sampler2D specColourTex, float spec, half3 lightColor, float2 mainUv, float specWeight, half4 specMapTexVal)
{
    // Sample specular colour
    half4 specColTex = tex2D(specColourTex, mainUv);

    half3 specCol = spec * lightColor * specColTex.rgb;
    return specCol * specWeight * specMapTexVal.r;
}

half3 ApplyReflectColor(sampler2D reflectTex, half3 col, float2 mainUv, half3 worldRefl, float reflectiveness)
{
    // Sample the reflectivity map
    half4 reflectMap = tex2D(reflectTex, mainUv);
    // sample the default reflection cubemap, using the reflection vector
    half4 skyData = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, worldRefl);
    // decode cubemap data into actual color
    half3 skyColor = DecodeHDR(skyData, unity_SpecCube0_HDR);

    float reflect = reflectMap.r * reflectiveness;
    return lerp(col, skyColor.rgb, reflect);
}

half DirectLightAttenuation(half dotNL, float shadowAttenuation)
{
#ifdef MTOON_FORWARD_ADD
#ifdef _ALPHABLEND_ON
    return step(0, dotNL); // darken if transparent. Because Unity's transparent material can't receive shadowAttenuation.
#endif
    half attenuation = 0.5; // darken if additional light.
    attenuation *= min(0, dotNL) + 1; // darken dotNL < 0 area by using half lambert
    attenuation *= shadowAttenuation; // darken if receiving shadow
    return attenuation;
#else
    // base light does not darken.
    return 1.0f;
#endif
}

// Convert RGB to HSV
float3 rgb2hsv(float3 c)
{
    float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
    float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

// Convert HSV to RGB
float3 hsv2rgb(float3 c)
{
    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

half3 IndirectLighting(half3 worldNormal, float EPS_COL, half indirectLightIntensity, half lightColorAttenuation)
{
    half3 toonedGI = 0.5 * (ShadeSH9(half4(0, 1, 0, 1)) + ShadeSH9(half4(0, -1, 0, 1)));
    half3 indirectLighting = lerp(toonedGI, ShadeSH9(half4(worldNormal, 1)), indirectLightIntensity);
    return lerp(indirectLighting, max(EPS_COL, max(indirectLighting.x, max(indirectLighting.y, indirectLighting.z))), lightColorAttenuation); // color atten
}

half3 MatcapLighting(float3 worldView, half3 worldNormal, float isOutline, sampler2D sphereAdd)
{
    half3 worldCameraUp = normalize(UNITY_MATRIX_V[1].xyz);
    half3 worldViewUp = normalize(worldCameraUp - worldView * dot(worldView, worldCameraUp));
    half3 worldViewRight = normalize(cross(worldView, worldViewUp));
    half2 matcapUv = half2(dot(worldViewRight, worldNormal), dot(worldViewUp, worldNormal)) * 0.5 + 0.5;
    half3 matcapLighting = tex2D(sphereAdd, matcapUv);
    return lerp(matcapLighting, half3(0, 0, 0), isOutline);
}

half RimLight(float3 worldView, half3 worldNormal, half rimLift, half rimFresnelPower)
{
    return pow(saturate(1.0 - dot(worldNormal, worldView) + rimLift), rimFresnelPower);
}

half3 RimLightColor(half rimTerm, half3 staticRimLighting, half3 mixedRimLighting, float2 mainUv, float isOutline, half rimLightingMix, half4 rimColor, sampler2D rimTexture)
{
    half3 rimLighting = lerp(staticRimLighting, mixedRimLighting, rimLightingMix);
#if _RIMTEX_ON
    half3 rim = rimTerm * rimColor.rgb * tex2D(rimTexture, mainUv).rgb;
#else
    half3 rim = rimTerm * rimColor.rgb;
#endif
    return lerp(rim * rimLighting, half3(0, 0, 0), isOutline);
}

#endif // MTOON_CORE_INCLUDED
