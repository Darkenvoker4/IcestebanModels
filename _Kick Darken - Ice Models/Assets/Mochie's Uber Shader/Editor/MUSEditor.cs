using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;

public class MUSEditor : ShaderGUI {

    public enum RenderMode {NONE, TOON, PBR}
    public static void SetRenderMode(Material material, RenderMode mode) {
        EditorGUI.BeginChangeCheck();
        switch (mode) {
            case RenderMode.NONE:
                material.DisableKeyword("TOON");
                material.DisableKeyword("PBR");
                material.DisableKeyword("_DETAIL_MULX2");
                material.DisableKeyword("_PARALLAXMAP");
                material.DisableKeyword("_EMISSION");
                break;
            case RenderMode.TOON:
                material.EnableKeyword("TOON");
                material.DisableKeyword("PBR");
                material.DisableKeyword("_PARALLAXMAP");
                material.DisableKeyword("_EMISSION");
                break;
            case RenderMode.PBR:
                material.EnableKeyword("PBR");
                material.DisableKeyword("TOON");
                break;
            default: break;
        }
    }

    public enum ShatterMode {OFF, ON}
    public static void SetShatterMode(Material material, ShatterMode mode) {
        EditorGUI.BeginChangeCheck();
        switch (mode) {
            case ShatterMode.OFF:
                material.DisableKeyword("PIXELSNAP_ON");
                break;
            case ShatterMode.ON:
                material.EnableKeyword("PIXELSNAP_ON");
                break;
            default: break;
        }
    }

    static GUIContent MainTexLabel = new GUIContent("Main Texture", "");
    static GUIContent EmissTexLabel = new GUIContent("Emission Map", "");
    static GUIContent EmissMaskLabel = new GUIContent("Mask", "");
    static GUIContent NormalTexLabel = new GUIContent("Normal Map", "");
    static GUIContent DetailNormalLabel = new GUIContent("Detail Normal Map", "");
    static GUIContent MetallicTexLabel = new GUIContent("Metallic", "");
    static GUIContent RoughnessTexLabel = new GUIContent("Roughness", "");
    static GUIContent OcclusionMapLabel = new GUIContent("Occlusion", "");
    static GUIContent HeightTexLabel = new GUIContent("Height", "");
    static GUIContent ReflCubeLabel = new GUIContent("Reflection Cubemap", "");
    static GUIContent TranslucencyLabel = new GUIContent("Translucency Map", "");
    static GUIContent ShadowRampLabel = new GUIContent("Shadow Ramp", "");

    static bool showBase = true;
    static bool showShading = false;
    static bool showEmiss = false;
    static bool showRim = false;
    static bool showLighting = false;
    static bool showShadows = false;

    static string header = "Header_Pro";
    static string baseFoldout = "BaseFoldout_Pro";
    static string shadingFoldout = "ShadingFoldout_Pro";
    static string emissFoldout = "EmissFoldout_Pro";
    static string rimFoldout = "RimLightingFoldout_Pro";
    static string discordPic = "Discord_Pro";
    static string discordIconPic = "Discord_Icon_Pro";
    static string versionPic = "Version_Pro";
    static string lightingFoldout = "LightingFoldout_Pro";
    static string shadowsFoldout = "ShadowsFoldout_Pro";
    
    static MaterialProperty _RenderMode = null; 
    static MaterialProperty _CullingMode = null; 
    static MaterialProperty _MainTex = null; 
    static MaterialProperty _Color = null; 
    static MaterialProperty _SaturationRGB = null; 
    static MaterialProperty _Contrast = null; 
    static MaterialProperty _EmissionMap = null; 
    static MaterialProperty _EmissMask = null; 
    static MaterialProperty _EmissionColor = null; 
    static MaterialProperty _AdvancedEmiss = null; 
    static MaterialProperty _CrossMode = null; 
    static MaterialProperty _XScroll = null; 
    static MaterialProperty _YScroll = null; 
    static MaterialProperty _PulseSpeed = null; 
    static MaterialProperty _Crossfade = null; 
    static MaterialProperty _ReactToggle = null; 
    static MaterialProperty _ReactThresh = null;
    static MaterialProperty _Outline = null; 
    static MaterialProperty _OutlineThicc = null; 
    static MaterialProperty _OutlineCol = null;
    static MaterialProperty _RimCol = null; 
    static MaterialProperty _RimStrength = null; 
    static MaterialProperty _RimWidth = null; 
    static MaterialProperty _RimEdge = null;
    static MaterialProperty _BumpMap = null; 
    static MaterialProperty _DetailNormalMap = null; 
    static MaterialProperty _BumpScale = null; 
    static MaterialProperty _DetailNormalMapScale = null;
    static MaterialProperty _PreserveCol = null; 
    static MaterialProperty _ShadowStr = null; 
    static MaterialProperty _RampWidth = null;
    static MaterialProperty _FakeSpec = null; 
    static MaterialProperty _SpecStr = null; 
    static MaterialProperty _SpecSize = null; 
    static MaterialProperty _SpecCol = null; 
    static MaterialProperty _SpecBias = null;
    static MaterialProperty _Metallic = null; 
    static MaterialProperty _MetallicGlossMap = null;
    static MaterialProperty _Glossiness = null; 
    static MaterialProperty _SpecGlossMap = null;
    static MaterialProperty _OcclusionStrength = null; 
    static MaterialProperty _OcclusionMap = null;
    static MaterialProperty _Parallax = null;
    static MaterialProperty _ParallaxMap = null; 
    static MaterialProperty _March = null; 
    static MaterialProperty _HeightContrast = null;
    static MaterialProperty _UseReflCube = null; 
    static MaterialProperty _ReflCube = null;
    static MaterialProperty _ShatterToggle = null; 
    static MaterialProperty _ShatterMax = null; 
    static MaterialProperty _ShatterMin = null; 
    static MaterialProperty _ShatterSpread = null; 
    static MaterialProperty _ShatterCull = null;
    static MaterialProperty _WFColor = null; 
    static MaterialProperty _WFFill = null; 
    static MaterialProperty _WFSmoothing = null; 
    static MaterialProperty _WFThickness = null;
    static MaterialProperty _ZWrite = null; 
    static MaterialProperty _Opacity = null;
    static MaterialProperty _Cutoff = null; 
    static MaterialProperty _ATM = null;
    static MaterialProperty _Subsurface = null;
    static MaterialProperty _TranslucencyMap = null; 
    static MaterialProperty _SColor = null;
    static MaterialProperty _SStrength = null;
    static MaterialProperty _SSharp = null;
    static MaterialProperty _Reflections = null;
    static MaterialProperty _ReflSmooth = null;
    static MaterialProperty _EnableShadowRamp = null;
    static MaterialProperty _ShadowRamp = null;
    static MaterialProperty _SPen = null;
    static MaterialProperty _Brightness = null;
    static MaterialProperty _RAmt = null;
    static MaterialProperty _GAmt = null;
    static MaterialProperty _BAmt = null;
    static MaterialProperty _Hue = null;
    static MaterialProperty _SaturationHSL = null;
    static MaterialProperty _Luminance = null;
    static MaterialProperty _HSLMin = null;
    static MaterialProperty _HSLMax = null;
    static MaterialProperty _FilterModel = null;
    static MaterialProperty _AutoShift = null;
    static MaterialProperty _AutoShiftSpeed = null;
    static MaterialProperty _InvertRough = null;
    static MaterialProperty _LightMask = null;

    BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    MaterialEditor m_MaterialEditor;
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props) {

        Material material = materialEditor.target as Material;
        bool isTransparent = material.shader.name.Contains("Transparent");
        bool isCutout = material.shader.name.Contains("Cutout");
        if( !materialEditor.isVisible )
            return;
        foreach (var property in GetType().GetFields(bindingFlags)){
            if (property.FieldType == typeof(MaterialProperty)){
                property.SetValue(this, FindProperty(property.Name, props));
            }
        }

        if (!EditorGUIUtility.isProSkin){
                header = "Header";
                baseFoldout = "BaseFoldout";
                shadingFoldout = "ShadingFoldout";
                emissFoldout = "EmissFoldout";
                rimFoldout = "RimLightingFoldout";
                discordPic = "Discord";
                discordIconPic = "Discord_Icon";
                versionPic = "Version";
                lightingFoldout = "LightingFoldout";
                shadowsFoldout = "shadowsFoldout";
        }
        Texture2D headerTex = (Texture2D)Resources.Load(header, typeof(Texture2D));
        ShaderGUIHelper.CenteredTexture(headerTex, 0, 0);
        EditorGUI.BeginChangeCheck(); {

            Texture2D baseTex = (Texture2D)Resources.Load(baseFoldout, typeof(Texture2D));
            showBase = ShaderGUIHelper.Foldout(baseTex, showBase, 22.0f);
            if (showBase){
                // Rendering Style
                GUILayout.Space(4);
                EditorGUI.showMixedValue = _RenderMode.hasMixedValue;
                var rMode = (RenderMode)_RenderMode.floatValue;
                EditorGUI.BeginChangeCheck();
                rMode = (RenderMode)EditorGUILayout.Popup("Shading Style", (int)rMode, Enum.GetNames(typeof(RenderMode)));
                if (EditorGUI.EndChangeCheck()) {
                    materialEditor.RegisterPropertyChangeUndo("Shading Style");
                    _RenderMode.floatValue = (float)rMode;
                    foreach (var obj in _RenderMode.targets){
                        SetRenderMode((Material)obj, (RenderMode)rMode);
                    }
                    EditorGUI.showMixedValue = false;
                }
                if (_RenderMode.floatValue != 2)
                    _March.floatValue = 0;
                    
                // Culling
                materialEditor.ShaderProperty(_CullingMode, "Backface Culling");
                
                // Shatter Culling
                EditorGUI.showMixedValue = _ShatterToggle.hasMixedValue;
                var sMode = (ShatterMode)_ShatterToggle.floatValue;
                EditorGUI.BeginChangeCheck();
                sMode = (ShatterMode)EditorGUILayout.Popup("Shatter Culling", (int)sMode, Enum.GetNames(typeof(ShatterMode)));
                if (EditorGUI.EndChangeCheck()) {
                    materialEditor.RegisterPropertyChangeUndo("Shatter Culling");
                    _ShatterToggle.floatValue = (float)sMode;
                    foreach (var obj in _ShatterToggle.targets){
                        SetShatterMode((Material)obj, (ShatterMode)sMode);
                    }
                    EditorGUI.showMixedValue = false;
                }
                if (_ShatterToggle.floatValue == 1){
                    materialEditor.ShaderProperty(_ShatterMax, "Max Range", 1);
                    materialEditor.ShaderProperty(_ShatterMin, "Min Range", 1);
                    materialEditor.ShaderProperty(_ShatterSpread, "Spread", 1);
                    materialEditor.ShaderProperty(_ShatterCull, "Cull Threshold", 1);
                    GUILayout.Space(8);
                    materialEditor.ShaderProperty(_WFColor, "Wireframe Color", 1);
                    materialEditor.ShaderProperty(_WFFill, "Wireframe Opacity", 1);
                    materialEditor.ShaderProperty(_WFSmoothing, "Wireframe Smoothing", 1);
                    materialEditor.ShaderProperty(_WFThickness, "Wireframe Thickness", 1);
                    GUILayout.Space(8);
                }

                // Outline
                materialEditor.ShaderProperty(_Outline, "Outline");
                if (_Outline.floatValue >= 1){
                    materialEditor.ShaderProperty(_OutlineCol, "Color", 1);
                    materialEditor.ShaderProperty(_OutlineThicc, "Thickness", 1);
                    GUILayout.Space(8);
                }
                
                // Cutout and Transparent specific properties
                if (isCutout){
                    if (_RenderMode.floatValue != 2)
                        materialEditor.ShaderProperty(_ATM, "Alpha To Mask");
                    if (_ATM.floatValue != 1)
                        materialEditor.ShaderProperty(_Cutoff, "Cutout Amount");
                }

                if (isTransparent){
                    materialEditor.ShaderProperty(_ZWrite, _ZWrite.displayName);
                    materialEditor.ShaderProperty(_Opacity, "Opacity");
                }

                GUILayout.Space(15);
                if (_RenderMode.floatValue == 1){
                    materialEditor.TexturePropertySingleLine(MainTexLabel, _MainTex, _Color, _PreserveCol);
                    GUILayout.Space(-20);
                    Rect rm = EditorGUILayout.GetControlRect();
                    rm.x += ShaderGUIHelper.GetInspectorWidth()-170;
                    EditorGUI.LabelField(rm, "Color Preservation");
                }
                else {
                    materialEditor.TexturePropertySingleLine(MainTexLabel, _MainTex, _Color);
                    
                }
                materialEditor.TextureScaleOffsetProperty(_MainTex);
                GUILayout.Space(8);

                // Post filters
                materialEditor.ShaderProperty(_FilterModel, "Filters");
                if (_FilterModel.floatValue == 1){
                    materialEditor.ShaderProperty(_SaturationRGB, "Saturation", 1);
                    materialEditor.ShaderProperty(_Contrast, "Contrast", 1);
                    materialEditor.ShaderProperty(_Brightness, "Brightness", 1);
                    materialEditor.ShaderProperty(_RAmt, "Red", 1);
                    materialEditor.ShaderProperty(_GAmt, "Green", 1);
                    materialEditor.ShaderProperty(_BAmt, "Blue", 1);
                }
                if (_FilterModel.floatValue == 2){
                    materialEditor.ShaderProperty(_AutoShift, "Auto Shift", 1);
                    if (_AutoShift.floatValue == 1)
                        materialEditor.ShaderProperty(_AutoShiftSpeed, "Speed", 1);
                    else
                        materialEditor.ShaderProperty(_Hue, "Hue", 1);
                    materialEditor.ShaderProperty(_SaturationHSL, "Saturation", 1);
                    materialEditor.ShaderProperty(_Luminance, "Luminance", 1);
                    materialEditor.ShaderProperty(_HSLMin, "Min Threshold", 1);
                    materialEditor.ShaderProperty(_HSLMax, "Max Threshold", 1);
                }
                GUILayout.Space(8);
            }

            // Toon Shading
            if (_RenderMode.floatValue == 1){

                bool usingRamp = material.GetTexture("_ShadowRamp");
                bool usingDetail = material.GetTexture("_DetailNormalMap");
                if (usingRamp) _EnableShadowRamp.floatValue = 1;
                else _EnableShadowRamp.floatValue = 0;
                if (usingDetail) ShaderGUIHelper.SetKeyword(material, "_DETAIL_MULX2", true);
                else ShaderGUIHelper.SetKeyword(material, "_DETAIL_MULX2", false); 

                // Shadows
                Texture2D shadowsTex = (Texture2D)Resources.Load(shadowsFoldout, typeof(Texture2D));
                showShadows = ShaderGUIHelper.Foldout(shadowsTex, showShadows, 22.0f);
                if (showShadows){
                    GUILayout.Space(8);
                    materialEditor.TexturePropertySingleLine(ShadowRampLabel, _ShadowRamp);
                    materialEditor.TexturePropertySingleLine(OcclusionMapLabel, _OcclusionMap, _OcclusionMap.textureValue != null ? _OcclusionStrength : null);
                    materialEditor.ShaderProperty(_ShadowStr, "Shadow Intensity");
                    EditorGUI.BeginDisabledGroup(usingRamp);
                    materialEditor.ShaderProperty(_RampWidth, "Gradient Width");
                    EditorGUI.EndDisabledGroup();
                    GUILayout.Space(8);
                }

                // Lighting
                Texture2D lightingTex = (Texture2D)Resources.Load(lightingFoldout, typeof(Texture2D));
                showLighting = ShaderGUIHelper.Foldout(lightingTex, showLighting, 22.0f);
                if (showLighting){
                    GUILayout.Space(8);
                    materialEditor.TexturePropertySingleLine(NormalTexLabel, _BumpMap, _BumpScale);
                    materialEditor.TexturePropertySingleLine(DetailNormalLabel, _DetailNormalMap, usingDetail ? _DetailNormalMapScale : null); 
                    if (usingDetail){
                        materialEditor.TextureScaleOffsetProperty(_DetailNormalMap);
                        GUILayout.Space(10);
                    }
                    materialEditor.TexturePropertySingleLine(new GUIContent("Mask"), _LightMask);
                    ShaderGUIHelper.ToggleSlider(materialEditor, "Reflectivity", _Reflections, _ReflSmooth);
                    materialEditor.ShaderProperty(_FakeSpec, "Specular Highlights");
                    if (_FakeSpec.floatValue == 1){
                        materialEditor.ShaderProperty(_SpecCol, "Color", 1);
                        materialEditor.ShaderProperty(_SpecBias, "Bias", 1);
                        materialEditor.ShaderProperty(_SpecStr, "Strength", 1);
                        materialEditor.ShaderProperty(_SpecSize, "Smoothness", 1);
                    }
                    materialEditor.ShaderProperty(_Subsurface, "Subsurface Scattering");
                    if (_Subsurface.floatValue == 1){
                        materialEditor.TexturePropertySingleLine(TranslucencyLabel, _TranslucencyMap);
                        materialEditor.ShaderProperty(_SColor, "Color", 1);
                        materialEditor.ShaderProperty(_SStrength, "Strength", 1);
                        materialEditor.ShaderProperty(_SPen, "Penetration", 1);
                        materialEditor.ShaderProperty(_SSharp, "Sharpness", 1);
                    }
                    GUILayout.Space(8);
                }
            }

            // PBR
            if (_RenderMode.floatValue == 2){    

                bool usingHeight = material.GetTexture("_ParallaxMap"); 
                bool usingDetail = material.GetTexture("_DetailNormalMap");
                bool usingMetallic = material.GetTexture("_MetallicGlossMap");
                bool usingRoughness = material.GetTexture("_SpecGlossMap");   
                if (usingMetallic) _Metallic.floatValue = 1;
                if (usingRoughness) _Glossiness.floatValue = 1;
                if (!usingRoughness) _InvertRough.floatValue = 0;
                if (usingDetail) ShaderGUIHelper.SetKeyword(material, "_DETAIL_MULX2", true);
                else ShaderGUIHelper.SetKeyword(material, "_DETAIL_MULX2", false);
                if (usingHeight) ShaderGUIHelper.SetKeyword(material, "_PARALLAXMAP", true);
                else {
                    ShaderGUIHelper.SetKeyword(material, "_PARALLAXMAP", false);
                    ShaderGUIHelper.SetKeyword(material, "_EMISSION", false);
                    _March.floatValue = 0;
                }

                Texture2D shadingTex = (Texture2D)Resources.Load(shadingFoldout, typeof(Texture2D));
                showShading = ShaderGUIHelper.Foldout(shadingTex, showShading, 22.0f);
                if (showShading){
                    GUILayout.Space(8);
                    materialEditor.TexturePropertySingleLine(NormalTexLabel, _BumpMap, _BumpScale);
                    materialEditor.TexturePropertySingleLine(DetailNormalLabel, _DetailNormalMap, _DetailNormalMapScale);
                    if (usingDetail){
                        materialEditor.TextureScaleOffsetProperty(_DetailNormalMap);
                        GUILayout.Space(10);
                    }
                    materialEditor.TexturePropertySingleLine(MetallicTexLabel, _MetallicGlossMap, !usingMetallic ? _Metallic : null);
                    materialEditor.TexturePropertySingleLine(RoughnessTexLabel, _SpecGlossMap, !usingRoughness ? _Glossiness : null, usingRoughness ? _InvertRough : null);
                    if (usingRoughness){
                        GUILayout.Space(-20);
                        Rect r = EditorGUILayout.GetControlRect();
                        r.x += ShaderGUIHelper.GetInspectorWidth()-180;
                        EditorGUI.LabelField(r, "Read as Smoothness");
                    }
                    materialEditor.TexturePropertySingleLine(OcclusionMapLabel, _OcclusionMap, _OcclusionMap.textureValue != null ? _OcclusionStrength : null);
                    materialEditor.TexturePropertySingleLine(HeightTexLabel, _ParallaxMap, usingHeight ? _Parallax : null);
                    if (usingHeight){
                        materialEditor.ShaderProperty(_HeightContrast, "Height Contrast", 2);
                        materialEditor.ShaderProperty(_March, "High Quality", 2);
                    }
                    materialEditor.TexturePropertySingleLine(ReflCubeLabel, _ReflCube, _UseReflCube);
                    GUILayout.Space(8);
                }
            }

            // Emission
            Texture2D emissTex = (Texture2D)Resources.Load(emissFoldout, typeof(Texture2D));
            showEmiss = ShaderGUIHelper.Foldout(emissTex, showEmiss, 22.0f);
            if (showEmiss){
                GUILayout.Space(8);
                materialEditor.TexturePropertySingleLine(EmissTexLabel, _EmissionMap, _EmissionColor, _AdvancedEmiss);
                GUILayout.Space(-20);
                Rect r = EditorGUILayout.GetControlRect();
                r.x += ShaderGUIHelper.GetInspectorWidth()-120;
                EditorGUI.LabelField(r, "Advanced");
                GUILayout.Space(2);
                materialEditor.TextureScaleOffsetProperty(_EmissionMap);
                GUILayout.Space(4);
                if (_AdvancedEmiss.floatValue == 1){
                    GUILayout.Space(4);
                    materialEditor.TexturePropertySingleLine(EmissMaskLabel, _EmissMask);
                    materialEditor.TextureScaleOffsetProperty(_EmissMask);
                    GUILayout.Space(8);
                    materialEditor.ShaderProperty(_XScroll, "X Scrolling");
                    materialEditor.ShaderProperty(_YScroll, "Y Scrolling");
                    materialEditor.ShaderProperty(_PulseSpeed, "Pulse Speed");
                    materialEditor.ShaderProperty(_ReactToggle, "Light Reactivity");
                    EditorGUI.BeginDisabledGroup(_ReactToggle.floatValue == 0);
                    materialEditor.ShaderProperty(_CrossMode, "Crossfade Mode");
                    EditorGUI.EndDisabledGroup();
                    if (_CrossMode.floatValue == 1){
                        materialEditor.ShaderProperty(_ReactThresh, "Threshold", 1);
                        materialEditor.ShaderProperty(_Crossfade, "Strength", 1);
                    }
                    GUILayout.Space(8);
                }
                else
                    GUILayout.Space(8);
            }

            // Rim Lighting
            if (_RenderMode.floatValue > 0){
                Texture2D rimTex = (Texture2D)Resources.Load(rimFoldout, typeof(Texture2D));
                showRim = ShaderGUIHelper.Foldout(rimTex, showRim, 22.0f);
                if (showRim){
                    GUILayout.Space(4);
                    materialEditor.ShaderProperty(_RimCol, "Color");
                    materialEditor.ShaderProperty(_RimStrength, "Strength");
                    materialEditor.ShaderProperty(_RimWidth, "Width");
                    materialEditor.ShaderProperty(_RimEdge, "Sharpness");
                    GUILayout.Space(15);
                }
            }
            
            Texture2D discord = (Texture2D)Resources.Load(discordPic, typeof(Texture2D));
            Texture2D discordIcon = (Texture2D)Resources.Load(discordIconPic, typeof(Texture2D));
            Texture2D version = (Texture2D)Resources.Load(versionPic, typeof(Texture2D));
            ShaderGUIHelper.CenteredTexture(discordIcon, discord, -20, 0, 0);
            ShaderGUIHelper.CenteredTexture(version, -10, 0);
        }    
    }
}
