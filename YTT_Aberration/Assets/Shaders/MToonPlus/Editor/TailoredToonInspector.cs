using MToon;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MToonPlus
{
	public class TailoredToonInspector : ShaderGUI
    {
        private const float RoundsToDegree = 360f;
        private const float RoundsToRadian = (float) Math.PI * 2f;

        private static bool isAdvancedLightingPanelFoldout = false;
        private static EditorRotationUnit editorRotationUnit = EditorRotationUnit.Rounds;
        
        private MaterialProperty _version;
        private MaterialProperty _blendMode;
        private MaterialProperty _bumpMap;
        private MaterialProperty _bumpScale;
        private MaterialProperty _color;
        private MaterialProperty _cullMode;
//        private MaterialProperty _outlineCullMode;
        private MaterialProperty _cutoff;

        private MaterialProperty _debugMode;

        private MaterialProperty _emission;
        private MaterialProperty _emissionColor;
        private MaterialProperty _emissionMap;
        private MaterialProperty _emissionPulse;
        private MaterialProperty _emissionSimplePulseMinMax;
        private MaterialProperty _emissionSimplePulseTimeWeights;
        private MaterialProperty _emissionPositionPulseScale;
        private MaterialProperty _emissionPositionPulseMinMax;
        private MaterialProperty _emissionPositionPulseTimeWeights;

        private MaterialProperty _lightColorAttenuation;
        private MaterialProperty _indirectLightIntensity;
        private MaterialProperty _mainTex;
        private MaterialProperty _outlineColor;
        private MaterialProperty _outlineColorMode;
        private MaterialProperty _outlineLightingMix;
        private MaterialProperty _outlineWidth;
        private MaterialProperty _outlineScaledMaxDistance;
        private MaterialProperty _outlineWidthMode;
        private MaterialProperty _outlineWidthTexture;

        private MaterialProperty _receiveShadowRate;
        private MaterialProperty _shadowTex;
        private MaterialProperty _receiveShadowTexture;

        private MaterialProperty _shadeShift;
        private MaterialProperty _shadeToony;
        private MaterialProperty _rimColor;
        private MaterialProperty _rimTex;
        private MaterialProperty _rimTexture;
        private MaterialProperty _rimLightingMix;
        private MaterialProperty _rimFresnelPower;
        private MaterialProperty _rimLift;

        private MaterialProperty _uvAnimate;
        private MaterialProperty _uvAnimMaskTexture;
        private MaterialProperty _uvAnimScrollX;
        private MaterialProperty _uvAnimScrollY;
        private MaterialProperty _uvAnimRotation;

        private MaterialProperty _reflect;
        private MaterialProperty _reflectiveness;
        private MaterialProperty _reflectTex;

        private MaterialProperty _spec;
        private MaterialProperty _specWeight;
        private MaterialProperty _specPower;
        private MaterialProperty _specIntensity;
        private MaterialProperty _specTex;
        private MaterialProperty _specColorTex;
        private MaterialProperty _specIntensityTex;

        private MaterialProperty _materialLitHSVScaling;
        private MaterialProperty _materialShadeHSVScaling;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            _version = FindProperty(MToon.Utils.PropVersion, properties);
            _debugMode = FindProperty(Utils.PropDebugMode, properties);
            _outlineWidthMode = FindProperty(Utils.PropOutlineWidthMode, properties);
            _outlineColorMode = FindProperty(Utils.PropOutlineColorMode, properties);
            _blendMode = FindProperty(Utils.PropBlendMode, properties);
            _cullMode = FindProperty(Utils.PropCullMode, properties);
//            _outlineCullMode = FindProperty(Utils.PropOutlineCullMode, properties);
            _cutoff = FindProperty(Utils.PropCutoff, properties);
            _color = FindProperty(Utils.PropColor, properties);
            _mainTex = FindProperty(Utils.PropMainTex, properties);
            _bumpScale = FindProperty(Utils.PropBumpScale, properties);
            _bumpMap = FindProperty(Utils.PropBumpMap, properties);

            _receiveShadowRate = FindProperty(Utils.PropReceiveShadowRate, properties);
            _shadowTex = FindProperty(MToonPlusUtils.PropShadowTex, properties);
            _receiveShadowTexture = FindProperty(Utils.PropReceiveShadowTexture, properties);

            _shadeShift = FindProperty(Utils.PropShadeShift, properties);
            _shadeToony = FindProperty(Utils.PropShadeToony, properties);
            _lightColorAttenuation = FindProperty(Utils.PropLightColorAttenuation, properties);
            _indirectLightIntensity = FindProperty(Utils.PropIndirectLightIntensity, properties);
            _rimColor = FindProperty(Utils.PropRimColor, properties);
            _rimTex = FindProperty(MToonPlusUtils.PropRimTex, properties);
            _rimTexture = FindProperty(Utils.PropRimTexture, properties);
            _rimLightingMix = FindProperty(Utils.PropRimLightingMix, properties);
            _rimFresnelPower = FindProperty(Utils.PropRimFresnelPower, properties);
            _rimLift = FindProperty(Utils.PropRimLift, properties);

            _emission = FindProperty(MToonPlusUtils.PropEmission, properties);
            _emissionColor = FindProperty(Utils.PropEmissionColor, properties);
            _emissionMap = FindProperty(Utils.PropEmissionMap, properties);
            _emissionPulse = FindProperty(MToonPlusUtils.PropEmissionPulse, properties);
            _emissionSimplePulseMinMax = FindProperty(MToonPlusUtils.PropEmissionSimplePulseMinMax, properties);
            _emissionSimplePulseTimeWeights = FindProperty(MToonPlusUtils.PropEmissionSimplePulseTimeWeights, properties);
            _emissionPositionPulseScale = FindProperty(MToonPlusUtils.PropEmissionPositionPulseRate, properties);
            _emissionPositionPulseMinMax = FindProperty(MToonPlusUtils.PropEmissionPositionPulseMinMax, properties);
            _emissionPositionPulseTimeWeights = FindProperty(MToonPlusUtils.PropEmissionPositionPulseTimeWeights, properties);

            _outlineWidthTexture = FindProperty(Utils.PropOutlineWidthTexture, properties);
            _outlineWidth = FindProperty(Utils.PropOutlineWidth, properties);
            _outlineScaledMaxDistance = FindProperty(Utils.PropOutlineScaledMaxDistance, properties);
            _outlineColor = FindProperty(Utils.PropOutlineColor, properties);
            _outlineLightingMix = FindProperty(Utils.PropOutlineLightingMix, properties);

            _uvAnimate = FindProperty(MToonPlusUtils.PropUVAnimate, properties);
            _uvAnimMaskTexture = FindProperty(Utils.PropUvAnimMaskTexture, properties);
            _uvAnimScrollX = FindProperty(Utils.PropUvAnimScrollX, properties);
            _uvAnimScrollY = FindProperty(Utils.PropUvAnimScrollY, properties);
            _uvAnimRotation = FindProperty(Utils.PropUvAnimRotation, properties);

            _reflect = FindProperty(MToonPlusUtils.PropReflect, properties);
            _reflectiveness = FindProperty(MToonPlusUtils.PropReflectiveness, properties);
            _reflectTex = FindProperty(MToonPlusUtils.PropReflectTex, properties);

            _spec = FindProperty(MToonPlusUtils.PropSpec, properties);
            _specWeight = FindProperty(MToonPlusUtils.PropSpecWeight, properties);
            _specPower = FindProperty(MToonPlusUtils.PropSpecPower, properties);
            _specIntensity = FindProperty(MToonPlusUtils.PropSpecIntensity, properties);
            _specTex = FindProperty(MToonPlusUtils.PropSpecTex, properties);
            _specColorTex = FindProperty(MToonPlusUtils.PropSpecColorTex, properties);
            _specIntensityTex = FindProperty(MToonPlusUtils.PropSpecIntensityTex, properties);

            _materialLitHSVScaling = FindProperty(MToonPlusUtils.PropMaterialLitHSVScaling, properties);
            _materialShadeHSVScaling = FindProperty(MToonPlusUtils.PropMaterialShadeHSVScaling, properties);

            var materials = materialEditor.targets.Select(x => x as Material).ToArray();
            Draw(materialEditor, materials);
        }

        private void Draw(MaterialEditor materialEditor, Material[] materials)
        {
            EditorGUI.BeginChangeCheck();
            {
                _version.floatValue = Utils.VersionNumber;
                
                EditorGUILayout.LabelField("Rendering", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    EditorGUILayout.LabelField("Mode", EditorStyles.boldLabel);
                    if (PopupEnum<MToon.RenderMode>("Rendering Type", _blendMode, materialEditor))
                    {
                        ModeChanged(materials, isBlendModeChangedByUser: true);
                    }

                    if ((MToon.RenderMode) _blendMode.floatValue == MToon.RenderMode.TransparentWithZWrite)
                    {
                        EditorGUILayout.HelpBox("TransparentWithZWrite mode can cause problems with rendering.", MessageType.Warning);
                    }

                    if (PopupEnum<MToon.CullMode>("Cull Mode", _cullMode, materialEditor))
                    {
                        ModeChanged(materials);
                    }
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Color", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    EditorGUILayout.LabelField("Texture", EditorStyles.boldLabel);
                    {
                        materialEditor.TexturePropertySingleLine(new GUIContent("Lit Color, Alpha", "Lit (RGB), Alpha (A)"),
                            _mainTex, _color);
                    }
                    var bm = (MToon.RenderMode) _blendMode.floatValue;
                    if (bm == MToon.RenderMode.Cutout)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("Alpha", EditorStyles.boldLabel);
                        {
                            materialEditor.ShaderProperty(_cutoff, "Cutoff");
                        }
                    }
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Lighting", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    {
                        materialEditor.ShaderProperty(_shadeToony,
                            new GUIContent("Shading Toony",
                                "0.0 is Lambert. Higher value get toony shading."));

                        materialEditor.ShaderProperty(_materialLitHSVScaling, "Material Lit HSV Scaling");
                        materialEditor.ShaderProperty(_materialShadeHSVScaling, "Material Shade HSV Scaling");

                        // Normal
                        EditorGUI.BeginChangeCheck();
                        materialEditor.TexturePropertySingleLine(new GUIContent("Normal Map [Normal]", "Normal Map (RGB)"),
                            _bumpMap,
                            _bumpScale);
                        if (EditorGUI.EndChangeCheck())
                        {
                            materialEditor.RegisterPropertyChangeUndo("BumpEnabledDisabled");
                            ModeChanged(materials);
                        }
                    }
                    EditorGUILayout.Space();

                    EditorGUI.indentLevel++;
                    {
                        isAdvancedLightingPanelFoldout = EditorGUILayout.Foldout(isAdvancedLightingPanelFoldout, "Advanced Settings", EditorStyles.boldFont);

                        if (isAdvancedLightingPanelFoldout)
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.HelpBox(
                                "The default settings are suitable for Advanced Settings if you want to toony result.",
                                MessageType.Info);
                            if (GUILayout.Button("Use Default"))
                            {
                                _shadeShift.floatValue = 0;
                                _receiveShadowTexture.textureValue = null;
                                _receiveShadowRate.floatValue = 1;
                                _lightColorAttenuation.floatValue = 0;
                                _indirectLightIntensity.floatValue = 0.1f;
                            }
                            EditorGUILayout.EndHorizontal();
                            
                            materialEditor.ShaderProperty(_shadeShift,
                                new GUIContent("Shading Shift",
                                    "Zero is Default. Negative value increase lit area. Positive value increase shade area."));
                            DrawShadowOptions(materialEditor);
                            materialEditor.ShaderProperty(_lightColorAttenuation, "LightColor Attenuation");
                            materialEditor.ShaderProperty(_indirectLightIntensity, "GI Intensity");
                        }
                    }
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();

                DrawEmissionOptions(materialEditor);

                DrawRimOptions(materialEditor);

                EditorGUILayout.LabelField("Outline", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    // Outline
                    EditorGUILayout.LabelField("Width", EditorStyles.boldLabel);
                    {
                        if (PopupEnum<OutlineWidthMode>("Mode", _outlineWidthMode, materialEditor))
                        {
                            ModeChanged(materials);
                        }
                        
                        if ((MToon.RenderMode) _blendMode.floatValue == MToon.RenderMode.Transparent &&
                            (OutlineWidthMode) _outlineWidthMode.floatValue != OutlineWidthMode.None)
                        {
                            EditorGUILayout.HelpBox("Outline with Transparent material cause problem with rendering.", MessageType.Warning);
                        }

                        var widthMode = (OutlineWidthMode) _outlineWidthMode.floatValue;
                        if (widthMode != OutlineWidthMode.None)
                        {
                            materialEditor.TexturePropertySingleLine(
                                new GUIContent("Width", "Outline Width Texture (RGB)"),
                                _outlineWidthTexture, _outlineWidth);
                        }

                        if (widthMode == OutlineWidthMode.ScreenCoordinates)
                        {
                            materialEditor.ShaderProperty(_outlineScaledMaxDistance, "Width Scaled Max Distance");
                        }
                    }
                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Color", EditorStyles.boldLabel);
                    {
                        var widthMode = (OutlineWidthMode) _outlineWidthMode.floatValue;
                        if (widthMode != OutlineWidthMode.None)
                        {
                            EditorGUI.BeginChangeCheck();

                            if (PopupEnum<OutlineColorMode>("Mode", _outlineColorMode, materialEditor))
                            {
                                ModeChanged(materials);
                            }

                            var colorMode = (OutlineColorMode) _outlineColorMode.floatValue;

                            materialEditor.ShaderProperty(_outlineColor, "Color");
                            if (colorMode == OutlineColorMode.MixedLighting)
                                materialEditor.DefaultShaderProperty(_outlineLightingMix, "Lighting Mix");
                        }
                    }
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();

                
                EditorGUILayout.LabelField("UV Coordinates", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    // UV
                    EditorGUILayout.LabelField("Scale & Offset", EditorStyles.boldLabel);
                    {
                        materialEditor.TextureScaleOffsetProperty(_mainTex);
                    }
                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Auto Animation", EditorStyles.boldLabel);
                    {
                        DrawAnimateUVOptions(materialEditor);
                    }
                    EditorGUILayout.Space();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();

                DrawReflectOptions(materialEditor);
                EditorGUILayout.Space();
                DrawSpecularOptions(materialEditor);

                EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    EditorGUILayout.LabelField("Debugging Options", EditorStyles.boldLabel);
                    {
                        if (PopupEnum<DebugMode>("Visualize", _debugMode, materialEditor))
                        {
                            ModeChanged(materials);
                        }
                    }
                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Advanced Options", EditorStyles.boldLabel);
                    {
#if UNITY_5_6_OR_NEWER
//                    materialEditor.EnableInstancingField();
                        materialEditor.DoubleSidedGIField();
#endif
                        EditorGUI.BeginChangeCheck();
                        materialEditor.RenderQueueField();
                        if (EditorGUI.EndChangeCheck())
                        {
                            ModeChanged(materials);
                        }
                    }
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }
            EditorGUI.EndChangeCheck();
        }

        private void DrawShadowOptions(MaterialEditor materialEditor)
        {
            materialEditor.ShaderProperty(_shadowTex, "Receive Shadow Texture Enabled");

            if (Mathf.Approximately(_shadowTex.floatValue, 1f))
            {
                materialEditor.TexturePropertySingleLine(
                new GUIContent("Shadow Receive Multiplier",
                    "Texture (R) * Rate. White is Default. Black attenuates shadows."),
                _receiveShadowTexture,
                _receiveShadowRate);
            }
            else
            {
                materialEditor.ShaderProperty(_receiveShadowRate, "Shadow Receive Multiplier");
            }
        }

        private void DrawEmissionOptions(MaterialEditor materialEditor)
		{
            EditorGUILayout.LabelField("Emission", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                materialEditor.ShaderProperty(_emission, "Emission Enabled");

                if (Mathf.Approximately(_emission.floatValue, 1f))
                {
                    TextureWithHdrColor(materialEditor, "Emission", "Emission (RGB)",
                    _emissionMap, _emissionColor);

                    materialEditor.ShaderProperty(_emissionPulse, "Emission Pulse");

                    if (Mathf.Approximately(_emissionPulse.floatValue, 1f))
                    {
                        DrawSimplePulseProperties(materialEditor);
                    }
                    else if (Mathf.Approximately(_emissionPulse.floatValue, 2f))
					{
                        DrawPositionPulseProperties(materialEditor);

                    }
                    else if (Mathf.Approximately(_emissionPulse.floatValue, 3f))
					{
                        DrawSimplePulseProperties(materialEditor);
                        DrawPositionPulseProperties(materialEditor);
                    }
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        private void DrawSimplePulseProperties(MaterialEditor materialEditor)
		{
            EditorGUILayout.LabelField("Simple");
            materialEditor.VectorProperty(_emissionSimplePulseMinMax, "Pulse Min & Max & Weight");
            materialEditor.VectorProperty(_emissionSimplePulseTimeWeights, "Pulse Time Weights");
        }

        private void DrawPositionPulseProperties(MaterialEditor materialEditor)
        {
            EditorGUILayout.LabelField("Position");
            materialEditor.ShaderProperty(_emissionPositionPulseScale, "Pulse Scale");
            materialEditor.VectorProperty(_emissionPositionPulseMinMax, "Pulse Min & Max & Weight");
            materialEditor.VectorProperty(_emissionPositionPulseTimeWeights, "Pulse Time Weights");
        }

        private void DrawRimOptions(MaterialEditor materialEditor)
		{
            EditorGUILayout.LabelField("Rim", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                materialEditor.ShaderProperty(_rimTex, "Rim Texture Enabled");

                if (Mathf.Approximately(_rimTex.floatValue, 1f))
				{
                    TextureWithHdrColor(materialEditor, "Color", "Rim Color (RGB)",
                        _rimTexture, _rimColor);
                }
                else
				{
                    materialEditor.ShaderProperty(_rimColor, new GUIContent("Color", "Rim Color(RGB)"));
                }

                materialEditor.DefaultShaderProperty(_rimLightingMix, "Lighting Mix");

                materialEditor.ShaderProperty(_rimFresnelPower,
                    new GUIContent("Fresnel Power",
                        "If you increase this value, you get sharpness rim light."));

                materialEditor.ShaderProperty(_rimLift,
                    new GUIContent("Lift",
                        "If you increase this value, you can lift rim light."));
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        private void DrawAnimateUVOptions(MaterialEditor materialEditor)
        {
            materialEditor.ShaderProperty(_uvAnimate, "Animate UV Enabled");

            if (Mathf.Approximately(_uvAnimate.floatValue, 1f))
            {
                materialEditor.TexturePropertySingleLine(new GUIContent("Mask", "Auto Animation Mask Texture (R)"), _uvAnimMaskTexture);
                materialEditor.ShaderProperty(_uvAnimScrollX, "Scroll X (per second)");
                materialEditor.ShaderProperty(_uvAnimScrollY, "Scroll Y (per second)");

                {
                    var control = EditorGUILayout.GetControlRect(hasLabel: true);
                    const int popupMargin = 5;
                    const int popupWidth = 80;

                    var floatControl = new Rect(control);
                    floatControl.width -= popupMargin + popupWidth;
                    var popupControl = new Rect(control);
                    popupControl.x = floatControl.x + floatControl.width + popupMargin;
                    popupControl.width = popupWidth;

                    EditorGUI.BeginChangeCheck();
                    var inspectorRotationValue = GetInspectorRotationValue(editorRotationUnit, _uvAnimRotation.floatValue);
                    inspectorRotationValue = EditorGUI.FloatField(floatControl, "Rotation value (per second)", inspectorRotationValue);
                    if (EditorGUI.EndChangeCheck())
                    {
                        materialEditor.RegisterPropertyChangeUndo("UvAnimRotationValueChanged");
                        _uvAnimRotation.floatValue = GetRawRotationValue(editorRotationUnit, inspectorRotationValue);
                    }
                    editorRotationUnit = (EditorRotationUnit)EditorGUI.EnumPopup(popupControl, editorRotationUnit);
                }
            }
        }

        private void DrawReflectOptions(MaterialEditor materialEditor)
		{
            EditorGUILayout.LabelField("Reflect", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                materialEditor.ShaderProperty(_reflect, "Reflect Enabled");

                if (Mathf.Approximately(_reflect.floatValue, 1f))
				{
                    materialEditor.ShaderProperty(_reflectiveness, "Reflectiveness (aka Metallicness)");
                    materialEditor.TexturePropertySingleLine(new GUIContent("Reflect Map", "R Reflectiveness"), _reflectTex);
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        private void DrawSpecularOptions(MaterialEditor materialEditor)
		{
            EditorGUILayout.LabelField("Specular", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                materialEditor.ShaderProperty(_spec, "Specular Enabled");

                if (Mathf.Approximately(_spec.floatValue, 1f))
                {
                    materialEditor.ShaderProperty(_specWeight, "Weight (for Specular only)");
                    materialEditor.ShaderProperty(_specPower, "Glossiness (aka Specular Power)");
                    materialEditor.ShaderProperty(_specIntensity, "Lerp between calculated intensity and texture intensity");
                    materialEditor.TexturePropertySingleLine(new GUIContent("Specular Map", "R Specular weight, G Specular Glossiness"), _specTex);
                    materialEditor.TexturePropertySingleLine(new GUIContent("Specular Color", "Tint of specular"), _specColorTex);
                    materialEditor.TexturePropertySingleLine(new GUIContent("Specular Intensity", "Custom intensity of specular"), _specIntensityTex);
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            base.AssignNewShaderToMaterial(material, oldShader, newShader);

            Utils.ValidateProperties(material, isBlendModeChangedByUser: true);
        }

        private static void ModeChanged(Material[] materials, bool isBlendModeChangedByUser = false)
        {
            foreach (var material in materials)
            {
                Utils.ValidateProperties(material, isBlendModeChangedByUser);
            }
        }

        private static bool PopupEnum<T>(string name, MaterialProperty property, MaterialEditor editor) where T : struct
        {
            EditorGUI.showMixedValue = property.hasMixedValue;
            EditorGUI.BeginChangeCheck();
            var ret = EditorGUILayout.Popup(name, (int) property.floatValue, Enum.GetNames(typeof(T)));
            var changed = EditorGUI.EndChangeCheck();
            if (changed)
            {
                editor.RegisterPropertyChangeUndo("EnumPopUp");
                property.floatValue = ret;
            }

            EditorGUI.showMixedValue = false;
            return changed;
        }

        private static void TextureWithHdrColor(MaterialEditor materialEditor, string label, string description,
            MaterialProperty texProp, MaterialProperty colorProp)
        {
            materialEditor.TexturePropertyWithHDRColor(new GUIContent(label, description),
                texProp,
                colorProp,
#if UNITY_2018_1_OR_NEWER
#else
                new ColorPickerHDRConfig(minBrightness: 0, maxBrightness: 10, minExposureValue: -10,
                    maxExposureValue: 10),
#endif
                showAlpha: false);
            
        }

        private static float GetRawRotationValue(EditorRotationUnit unit, float inspectorValue)
        {
            switch (unit)
            {
                case EditorRotationUnit.Rounds:
                    return inspectorValue;
                case EditorRotationUnit.Degrees:
                    return inspectorValue / RoundsToDegree;
                case EditorRotationUnit.Radians:
                    return inspectorValue / RoundsToRadian;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static float GetInspectorRotationValue(EditorRotationUnit unit, float rawValue)
        {
            switch (unit)
            {
                case EditorRotationUnit.Rounds:
                    return rawValue;
                case EditorRotationUnit.Degrees:
                    return rawValue * RoundsToDegree;
                case EditorRotationUnit.Radians:
                    return rawValue * RoundsToRadian;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}