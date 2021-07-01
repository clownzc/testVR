using System;
using UnityEngine;

namespace UnityEditor
{
class VRSurfaceShaderGUI : ShaderGUI
{
	private static class Styles
	{
        public static string whiteSpaceString = " ";
        public static string commonMode = "Common Mode";
        public static string albedo = "Albedo";
        public static string normal = "Normal";
        public static string metal = "Metal";
        public static string fresnel = "Fresnel";
        public static string emission = "Emission";
        public static string emissionEffect = "Emission Effect";
        public static string light = "Vertex Light";
        public static string emissionFlow = "Emission Effect Flow";
        public static string emissionAnim = "Emission Effect Animation";
        public static string advancedText = "Advanced Options";
        public static string fogMode = "Fog Mode";

        public static GUIContent albedoText = new GUIContent("Albedo", "Albedo(RGB)");
        public static GUIContent normalMapText = new GUIContent("Normal Map", "Normal Map(RGB)");
        public static GUIContent mixText = new GUIContent("Mix Map", "Metallic (R) and Occlusion (G) and Smoothness (B)");
        public static GUIContent cubeText = new GUIContent("Cube Map", "Cube(RGB)");
        public static GUIContent emissionText = new GUIContent("Emission", "Emission(RGB)");
        public static GUIContent flowText = new GUIContent("Flow", "Flow(RGB)");
	}

	MaterialProperty cullMode = null;
    MaterialProperty zwriteMode = null;

	MaterialProperty albedoMap = null;
	MaterialProperty albedoColor = null;

    MaterialProperty bumpMode = null;
    MaterialProperty bumpMap = null;
    MaterialProperty bumpInten = null;

    MaterialProperty metalMode = null;
    MaterialProperty mixmap = null;
    MaterialProperty gloss = null;
    MaterialProperty occlusion = null;
    MaterialProperty cube = null;
    MaterialProperty refColor = null;
    MaterialProperty refInten = null;

    MaterialProperty fresnelMode = null;
    MaterialProperty fresnelDirection = null;
    MaterialProperty fresnelSource = null;
    MaterialProperty fresnelColor = null;
    MaterialProperty fresnelAdd = null;
    MaterialProperty fresnelMul = null;
    MaterialProperty fresnelExp = null;

    MaterialProperty emissionMode = null;
    MaterialProperty emissionUV = null;
    MaterialProperty emissionMap = null;
    MaterialProperty emissionColor = null;
    MaterialProperty emissionMul = null;

    MaterialProperty emissionEffectMode = null;
    MaterialProperty emissionAni = null;
    MaterialProperty flowMap = null;
    MaterialProperty flowColor = null;
    MaterialProperty flowMul = null;
    MaterialProperty flowSpeed = null;

    MaterialProperty lightMode = null;
    MaterialProperty fogMode = null;

    //MaterialProperty transparency = null;

    MaterialEditor m_MaterialEditor;
    ColorPickerHDRConfig m_EmissionColorPickerHDRConfig = new ColorPickerHDRConfig(0f, 99f, 1/99f, 3f);

	bool m_FirstTimeApply = true;

	public void FindProperties (MaterialProperty[] props)
	{
        cullMode = FindProperty("_Cull", props);
        zwriteMode = FindProperty("_ZWrite", props);

        albedoMap = FindProperty("_MainTex", props);
        albedoColor = FindProperty("_Color", props);

        bumpMode = FindProperty("_NormalMode", props);
        bumpMap = FindProperty("_BumpMap", props);
        bumpInten = FindProperty("_NormalInten", props);

        metalMode = FindProperty("_MetalMode", props);
        mixmap = FindProperty("_Mixmap", props);
        gloss = FindProperty("_Gloss", props);
        occlusion = FindProperty("_Occlusion", props);
        cube = FindProperty("_Cube", props);
        refColor = FindProperty("_ReflectColor", props);
        refInten = FindProperty("_RefInten", props);

        fresnelMode = FindProperty("_FresnelMode", props);
        fresnelDirection = FindProperty("_FresnelDirection", props);
        fresnelSource = FindProperty("_FresnelSource", props);
        fresnelColor = FindProperty("_FresnelColor", props);
        fresnelAdd = FindProperty("_FresnelAdd", props);
        fresnelMul = FindProperty("_FresnelMul", props);
        fresnelExp = FindProperty("_FresnelExp", props);

        emissionMode = FindProperty("_EmissionMode", props);
        emissionUV = FindProperty("_EmissionUV", props);
        emissionMap = FindProperty("_EmissionMap", props);
        emissionColor = FindProperty("_EmissionColor", props);
        emissionMul = FindProperty("_EmissionMul", props);

        emissionEffectMode = FindProperty("_EmissionEffectMode", props);
        emissionAni = FindProperty("_EmissionAni", props);
        flowMap = FindProperty("_FlowMap", props);
        flowColor = FindProperty("_FlowColor", props);
        flowMul = FindProperty("_FlowMul", props);
        flowSpeed = FindProperty("_FlowSpeed", props);

        lightMode = FindProperty("_LightMode", props);
        fogMode = FindProperty("_FogMode", props);
    }
	
	public override void AssignNewShaderToMaterial (Material material, Shader oldShader, Shader newShader)
	{
		base.AssignNewShaderToMaterial(material, oldShader, newShader);

		// Re-run this in case the new shader needs custom setup.
		m_FirstTimeApply = true;
	}

	public override void OnGUI (MaterialEditor materialEditor, MaterialProperty[] props)
	{
		FindProperties (props); // MaterialProperties can be animated so we do not cache them but fetch them every event to ensure animated values are updated correctly
		m_MaterialEditor = materialEditor;
		Material material = materialEditor.target as Material;

		ShaderPropertiesGUI (material);

		// Make sure that needed keywords are set up if we're switching some existing
		// material to a standard shader.
		if (m_FirstTimeApply)
		{
			SetMaterialKeywords (material);
			m_FirstTimeApply = false;

			// Repaint all in case we modified how things render
			SceneView.RepaintAll();
		}
	}

	public void ShaderPropertiesGUI (Material material)
	{
		// Use default labelWidth
		EditorGUIUtility.labelWidth = 0f;

		// Detect any changes to the material

        GUILayout.Label(Styles.commonMode, EditorStyles.boldLabel);
        CommonModePopup(material);
        EditorGUILayout.Space();

        GUILayout.Label(Styles.albedo, EditorStyles.boldLabel);
        DoAlbedoArea(material);
        EditorGUILayout.Space();

        GUILayout.Label(Styles.normal, EditorStyles.boldLabel);
        DoNormalArea(material);
        EditorGUILayout.Space();

        GUILayout.Label(Styles.metal, EditorStyles.boldLabel);
        DoMetalArea(material);
        EditorGUILayout.Space();

        GUILayout.Label(Styles.fresnel, EditorStyles.boldLabel);
        DoFresnelArea(material);
        EditorGUILayout.Space();

        GUILayout.Label(Styles.emission, EditorStyles.boldLabel);
        DoEmissionArea(material);
        EditorGUILayout.Space();

        GUILayout.Label(Styles.emissionEffect, EditorStyles.boldLabel);
        DoEmissionEffectArea(material);
        EditorGUILayout.Space();

        GUILayout.Label(Styles.light, EditorStyles.boldLabel);
        DoLightArea(material);
        EditorGUILayout.Space();

        GUILayout.Label(Styles.fogMode, EditorStyles.boldLabel);
        DoFogMode(material);
        EditorGUILayout.Space();

        GUILayout.Label(Styles.advancedText, EditorStyles.boldLabel);
        m_MaterialEditor.RenderQueueField();
        m_MaterialEditor.EnableInstancingField();
    }

    void CommonModePopup(Material material)
    {
        m_MaterialEditor.ShaderProperty(cullMode, cullMode.displayName);
        m_MaterialEditor.ShaderProperty(zwriteMode, zwriteMode.displayName);
        SetMaterialKeywords(material);
    }

	void DoAlbedoArea(Material material)
	{
		m_MaterialEditor.TexturePropertySingleLine(Styles.albedoText, albedoMap, albedoColor);
        m_MaterialEditor.TextureScaleOffsetProperty(albedoMap);
	}

    void DoNormalArea(Material material)
    {
        m_MaterialEditor.ShaderProperty(bumpMode, bumpMode.displayName);
        if ((int)bumpMode.floatValue == 1)
        {
            m_MaterialEditor.TexturePropertySingleLine(Styles.normalMapText, bumpMap, bumpInten);
        }
        else
        {
            bumpMap.textureValue = null;
        }
    }

    void DoMetalArea(Material material)
    {
        m_MaterialEditor.ShaderProperty(metalMode, metalMode.displayName);
        SetKeyword(material, "_METAL", (int)metalMode.floatValue == 1);
        if ((int)metalMode.floatValue == 1)
        {
            m_MaterialEditor.TexturePropertySingleLine(Styles.mixText, mixmap);
            m_MaterialEditor.ShaderProperty(gloss, gloss.displayName);
            m_MaterialEditor.ShaderProperty(occlusion, occlusion.displayName);
            m_MaterialEditor.TexturePropertySingleLine(Styles.cubeText, cube, refColor, refInten);
        }
        else
        {
            mixmap.textureValue = null;
            cube.textureValue = null;
        }
    }

    void DoFresnelArea(Material material)
    {
        m_MaterialEditor.ShaderProperty(fresnelMode, fresnelMode.displayName);
        SetKeyword(material, "_FRESNEL", (int)fresnelMode.floatValue == 1);
        if ((int)fresnelMode.floatValue == 1)
        {
            m_MaterialEditor.ShaderProperty(fresnelDirection, fresnelDirection.displayName);
            m_MaterialEditor.ShaderProperty(fresnelSource, fresnelSource.displayName);
            m_MaterialEditor.ShaderProperty(fresnelColor, fresnelColor.displayName);
            m_MaterialEditor.ShaderProperty(fresnelAdd, fresnelAdd.displayName);
            m_MaterialEditor.ShaderProperty(fresnelMul, fresnelMul.displayName);
            m_MaterialEditor.ShaderProperty(fresnelExp, fresnelExp.displayName);
        }
    }

    void DoEmissionArea(Material material)
    {
        float brightness = emissionColor.colorValue.maxColorComponent;
        m_MaterialEditor.ShaderProperty(emissionMode, emissionMode.displayName);
        SetKeyword(material, "_EMISSION", (int)emissionMode.floatValue == 1);

        bool hadEmissionTexture = emissionMap.textureValue != null;

        if ((int)emissionMode.floatValue == 1)
        {
            //m_MaterialEditor.TexturePropertySingleLine(Styles.emissionText, emissionMap, emissionColor, emissionMul); //another GUI properties
            // Texture and HDR color controls
            m_MaterialEditor.TexturePropertyWithHDRColor(Styles.emissionText, emissionMap, emissionColor,
            m_EmissionColorPickerHDRConfig, false);

            // If texture was assigned and color was black set color to white
            if (emissionMap.textureValue != null && !hadEmissionTexture && brightness <= 0f)
            emissionColor.colorValue = Color.white;

            m_MaterialEditor.ShaderProperty(emissionUV, emissionUV.displayName);

            EditorGUI.BeginDisabledGroup(!(emissionColor.colorValue.maxColorComponent > 0f));

            m_MaterialEditor.LightmapEmissionProperty(MaterialEditor.kMiniTextureFieldLabelIndentLevel + 1);
            EditorGUI.EndDisabledGroup();
        }
        else
        {
            emissionMap.textureValue = null;
            emissionColor.colorValue = Color.black;
        }
    }

    void DoEmissionEffectArea(Material material)
    {
        m_MaterialEditor.ShaderProperty(emissionEffectMode, emissionEffectMode.displayName);

        SetKeyword(material, "_FLOW", (int)emissionEffectMode.floatValue == 1);
        if ((int)emissionEffectMode.floatValue == 1)
        {
            m_MaterialEditor.TexturePropertySingleLine(Styles.flowText, flowMap, flowColor, flowMul);
            m_MaterialEditor.ShaderProperty(flowSpeed, flowSpeed.displayName);
        }
        else
        {
            flowMap.textureValue = null;
        }

        SetKeyword(material, "_ANIM", (int)emissionEffectMode.floatValue == 2);
        if ((int)emissionEffectMode.floatValue == 2)
        {
            m_MaterialEditor.ShaderProperty(emissionAni, emissionAni.displayName);
        }
    }

    void DoLightArea(Material material)
    {
        m_MaterialEditor.ShaderProperty(lightMode, lightMode.displayName);

        SetKeyword(material, "_VERTOR_LIGHT", (int)lightMode.floatValue == 1);
    }

    void DoFogMode(Material material)
    {
        m_MaterialEditor.ShaderProperty(fogMode, fogMode.displayName);

        SetKeyword(material, "_DAYDREAM_FOG", (int)fogMode.floatValue == 1);
        SetKeyword(material, "_DAYDREAM_HEIGHT_FOG", (int)fogMode.floatValue == 2);
    }

    void SetMaterialKeywords(Material material)
	{
        SetKeyword(material, "_METAL", (int)metalMode.floatValue == 1);
        SetKeyword(material, "_EMISSION", (int)emissionMode.floatValue == 1);
        SetKeyword(material, "_FRESNEL", (int)fresnelMode.floatValue == 1);
        SetKeyword(material, "_FLOW", (int)emissionEffectMode.floatValue == 1);
        SetKeyword(material, "_ANIM", (int)emissionEffectMode.floatValue == 2);
        SetKeyword(material, "_VERTEX_LIGHT", (int)lightMode.floatValue == 1);
        SetKeyword(material, "_DAYDREAM_FOG", (int)fogMode.floatValue == 1);
        SetKeyword(material, "_DAYDREAM_HEIGHT_FOG", (int)fogMode.floatValue == 2);
    }

	static void SetKeyword(Material m, string keyword, bool state)
	{
		if (state)
			m.EnableKeyword (keyword);
		else
			m.DisableKeyword (keyword);
	}

}

} // namespace UnityEditor
