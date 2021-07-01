using System;
using UnityEngine;

namespace UnityEditor
{
class VRMobileStandardShaderGUI : ShaderGUI
{
    private enum WorkflowMode
    {
        Specular,
        Metallic,
        Dielectric
    }

	public enum BlendMode
	{
		Opaque,
		Cutout,
		Fade,		// Old school alpha-blending mode, fresnel does not affect amount of transparency
		Transparent // Physically plausible transparency mode, implemented as alpha pre-multiply
	}

    public enum EmissionMode
    {
        Emission,
        Fresnel,
        EmissionAndFresnel,
        EmissionMask,
        EmissionAdd
    }

	private static class Styles
	{
        //public:关键字是类型和类型成员的访问修饰符
        //override:要扩展或修改继承的方法、属性、索引器或事件的抽象实现或虚实现，必须使用 override 修饰符
        //void:用作方法的返回类型时，void 关键字指定方法不返回值。
		public static GUIStyle optionsButton = "PaneOptions";
		public static GUIContent uvSetLabel = new GUIContent("UV Set");
		public static GUIContent[] uvSetOptions = new GUIContent[] { new GUIContent("UV channel 0"), new GUIContent("UV channel 1") };

		public static string emptyTootip = "";
		public static GUIContent albedoText = new GUIContent("Albedo", "Albedo (RGB) and Transparency (A)");
		public static GUIContent alphaCutoffText = new GUIContent("Alpha Cutoff", "Threshold for alpha cutoff");
		public static GUIContent specularMapText = new GUIContent("Specular", "Specular (RGB) and Smoothness (A)");
        public static GUIContent metallicMapText = new GUIContent("Mix Map", "Metallic (R) and Occlusion (G) and Smoothness (B)");
		public static GUIContent smoothnessText = new GUIContent("Gloss", "");
		public static GUIContent normalMapText = new GUIContent("Normal Map", "Normal Map");
		public static GUIContent orthoNormalizeText = new GUIContent("Orthonormalize", "Orthonormalize tangent base");
		//public static GUIContent heightMapText = new GUIContent("Height Map", "Height Map (G)");
		public static GUIContent occlusionText = new GUIContent("Occlusion", "Occlusion (G)");
		public static GUIContent emissionText = new GUIContent("Emission", "Emission (RGB)");
        public static GUIContent fresnelText = new GUIContent("Fresnel", "Fresnel (RGB)");
		public static GUIContent detailMaskText = new GUIContent("Detail Mask", "Mask for Secondary Maps (A)");
		public static GUIContent detailAlbedoText = new GUIContent("Detail Albedo x2", "Albedo (RGB) multiplied by 2");
		public static GUIContent detailNormalMapText = new GUIContent("Normal Map", "Normal Map");
		public static GUIContent smoothnessInAlbedoText = new GUIContent("Smoothness in Albedo", "Smoothness is stored in Albedo (A); Specular is a single color.");

		public static string whiteSpaceString = " ";
		public static string primaryMapsText = "Main Maps";
		public static string secondaryMapsText = "Secondary Maps";
		public static string renderingMode = "Rendering Mode";
        public static string emissionMode = "Emission Mode";
		public static GUIContent emissiveWarning = new GUIContent ("Emissive value is animated but the material has not been configured to support emissive. Please make sure the material itself has some amount of emissive.");
        public static GUIContent fresnelWarning = new GUIContent("Fresnel value is animated but the material has not been configured to support fresnel. Please make sure the material itself has some amount of fresnel.");
		public static readonly string[] blendNames = Enum.GetNames (typeof (BlendMode));
        public static readonly string[] emissionNames = Enum.GetNames(typeof(EmissionMode));
	}

	MaterialProperty blendMode = null;
	MaterialProperty cullMode = null;
    MaterialProperty fogMode = null;
    MaterialProperty emissionMode = null;
	MaterialProperty albedoMap = null;
	MaterialProperty albedoColor = null;
    MaterialProperty albedoStrength = null;
	MaterialProperty alphaCutoff = null;
    MaterialProperty transparency = null;
    MaterialProperty transparencyMaskMode = null;
    MaterialProperty transparencyMaskMap = null;
    MaterialProperty transparencyMask = null;
    MaterialProperty scale = null;
	MaterialProperty specularMap = null;
	MaterialProperty specularColor = null;
	MaterialProperty metallicMap = null;
	MaterialProperty metallic = null;
	MaterialProperty smoothness = null;
    MaterialProperty metallicStrength = null;
    MaterialProperty glossStrength = null;
    MaterialProperty glossmin = null;
    MaterialProperty glossmax = null;
	MaterialProperty smoothnessTweak1 = null;
	MaterialProperty smoothnessTweak2 = null;
	MaterialProperty smoothnessTweaks = null;
	MaterialProperty specularMapColorTweak = null;
	MaterialProperty bumpScale = null;
	MaterialProperty bumpMap = null;
	MaterialProperty orthoNormalize = null;
	MaterialProperty occlusionStrength = null;
	MaterialProperty occlusionMap = null;
	MaterialProperty heigtMapScale = null;
	MaterialProperty emissionColorForRendering = null;
	MaterialProperty emissionMap = null;
    MaterialProperty emissionMul = null;
    MaterialProperty emissionMask = null;
    MaterialProperty emissionMaskMin = null;
    MaterialProperty emissionMaskMax = null;
    MaterialProperty emissionMaskMul = null;
    MaterialProperty emissionAddIndex = null;
    MaterialProperty emissionAddMul = null;
    MaterialProperty fresnelColorForRendering = null;
    MaterialProperty fresnelMap = null;
    MaterialProperty fresnelAdd = null;
    MaterialProperty fresnelMul = null;
    MaterialProperty fresnelExp = null;
	MaterialProperty detailMask = null;
	MaterialProperty detailAlbedoMap = null;
	MaterialProperty detailNormalMapScale = null;
	MaterialProperty detailNormalMap = null;
	MaterialProperty uvSetSecondary = null;
	MaterialProperty smoothnessInAlbedo = null;
    MaterialProperty outlineSwitch = null;
    MaterialProperty outlineWidth = null;
    MaterialProperty outlineAdd = null;
    MaterialProperty outlineMul = null;
    MaterialProperty outlineExp = null;
    MaterialProperty outlineColor = null;
    MaterialProperty outlineFactor = null;
    MaterialProperty fresnelAlphaMode = null;
    MaterialProperty fresnelAlphaAdd = null;
    MaterialProperty fresnelAlphaMul = null;
    MaterialProperty fresnelAlphaExp = null;
    MaterialProperty lightMode = null;

    MaterialEditor m_MaterialEditor;
	WorkflowMode m_WorkflowMode = WorkflowMode.Specular;
    ColorPickerHDRConfig m_EmissionColorPickerHDRConfig = new ColorPickerHDRConfig(0f, 99f, 1 / 99f, 3f);
    ColorPickerHDRConfig m_FresnelColorPickerHDRConfig = new ColorPickerHDRConfig(0f, 99f, 1 / 99f, 3f);

	bool m_FirstTimeApply = true;

	public void FindProperties (MaterialProperty[] props)
	{
		blendMode = FindProperty ("_Mode", props);
		cullMode = FindProperty ("_CullMode", props, false);
        fogMode = FindProperty("_FogMode", props, false);
	    emissionMode = FindProperty("_EmissionMode", props);
		albedoMap = FindProperty ("_MainTex", props);
		albedoColor = FindProperty ("_Color", props);
        albedoStrength = FindProperty("_AlbedoStrength", props, false);
		alphaCutoff = FindProperty ("_Cutoff", props);
        transparency = FindProperty("_Transparency", props);
        transparencyMaskMode = FindProperty("_TransparencyMaskMode", props, false);
        transparencyMaskMap = FindProperty("_TransparencyMaskTex", props, false);
        transparencyMask = FindProperty("_TransparencyMask", props, false);
        scale = FindProperty("_Scale", props, false);
		specularMap = FindProperty ("_SpecGlossMap", props, false);
		specularColor = FindProperty ("_SpecColor", props, false);
		metallicMap = FindProperty ("_MetallicGlossMap", props, false);
		metallic = FindProperty ("_Metallic", props, false);
		if (specularMap != null && specularColor != null)
			m_WorkflowMode = WorkflowMode.Specular;
		else if (metallicMap != null && metallic != null)
			m_WorkflowMode = WorkflowMode.Metallic;
		else
			m_WorkflowMode = WorkflowMode.Dielectric;
		smoothness = FindProperty ("_Glossiness", props);
        metallicStrength = FindProperty("_MetallicStrength", props, false);
        glossStrength = FindProperty("_GlossStrength", props, false);
        glossmin = FindProperty ("_GlossMin", props, false);
        glossmax = FindProperty ("_GlossMax", props, false);
        smoothnessTweak1 = FindProperty ("_SmoothnessTweak1", props, false);
		smoothnessTweak2 = FindProperty ("_SmoothnessTweak2", props, false);
		smoothnessTweaks = FindProperty ("_SmoothnessTweaks", props, false);
		specularMapColorTweak = FindProperty ("_SpecularMapColorTweak", props, false);

		bumpScale = FindProperty ("_BumpScale", props);
		bumpMap = FindProperty ("_BumpMap", props);
		orthoNormalize = FindProperty ("_Orthonormalize", props, false);
		//heigtMapScale = FindProperty ("_Parallax", props);
		//heightMap = FindProperty("_ParallaxMap", props);
		occlusionStrength = FindProperty ("_OcclusionStrength", props);
		occlusionMap = FindProperty ("_OcclusionMap", props);
        emissionColorForRendering = FindProperty("_EmissionColor", props);
		emissionMap = FindProperty ("_EmissionMap", props);
        emissionMul = FindProperty("_EmissionMul", props);
        emissionMask = FindProperty("_EmissionMask", props);
        emissionMaskMin = FindProperty("_EmissionMaskMin", props);
        emissionMaskMax = FindProperty("_EmissionMaskMax", props);
        emissionMaskMul = FindProperty("_EmissionMaskMul", props);
        emissionAddIndex = FindProperty("_EmissionAddIndex", props);
        emissionAddMul = FindProperty("_EmissionAddMul", props);
        fresnelColorForRendering = FindProperty("_FresnelColor", props); ;
        fresnelMap = FindProperty("_FresnelMap", props);
        fresnelAdd = FindProperty("_FresnelAdd", props); ;
        fresnelMul = FindProperty("_FresnelMul", props); ;
        fresnelExp = FindProperty("_FresnelExp", props); ;
		detailMask = FindProperty ("_DetailMask", props);
		detailAlbedoMap = FindProperty ("_DetailAlbedoMap", props);
		detailNormalMapScale = FindProperty ("_DetailNormalMapScale", props);
		detailNormalMap = FindProperty ("_DetailNormalMap", props);
		uvSetSecondary = FindProperty ("_UVSec", props);
		smoothnessInAlbedo = FindProperty ("_SmoothnessInAlbedo", props, false);

        outlineSwitch = FindProperty("_OutLineSwitch", props);
        outlineFactor = FindProperty("_OutLineFactor", props);
        outlineWidth = FindProperty("_OutLineWidth", props);
        outlineAdd = FindProperty("_OutLineAdd", props);
        outlineMul = FindProperty("_OutLineMul", props);
        outlineExp = FindProperty("_OutLineExp", props);
        outlineColor = FindProperty("_OutLineColor", props);

        fresnelAlphaMode = FindProperty("_FresnelAlphaMode", props);
        fresnelAlphaAdd = FindProperty("_FresnelAlphaAdd", props);
        fresnelAlphaMul = FindProperty("_FresnelAlphaMul", props);
        fresnelAlphaExp = FindProperty("_FresnelAlphaExp", props);

        lightMode = FindProperty("_LightMode", props, false);

            //unityLut = FindProperty("unity_Lut", props);

            //unityLut.textureValue = Resources.Load<Texture2D>("Shader/unity_lut");
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
			// Make sure we've updated this packed vector
			if(smoothnessTweak1 != null && smoothnessTweak2 != null && smoothnessTweaks != null) {
				var w = new Vector4(smoothnessTweak1.floatValue, smoothnessTweak2.floatValue);
				if(smoothnessTweaks.vectorValue != w)
					smoothnessTweaks.vectorValue = w;
			}

			SetMaterialKeywords (material, m_WorkflowMode);
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
		EditorGUI.BeginChangeCheck();
		{
            ModePopup(material);
			BlendModePopup();
			OrthoNormalizeToggle();

			// Primary properties   
            // primary排序
			GUILayout.Label (Styles.primaryMapsText, EditorStyles.boldLabel);
			DoAlbedoArea(material);
			DoSpecularMetallicArea();
			m_MaterialEditor.TexturePropertySingleLine(Styles.normalMapText, bumpMap, bumpMap.textureValue != null ? bumpScale : null);
			//m_MaterialEditor.TexturePropertySingleLine(Styles.heightMapText, heightMap, heightMap.textureValue != null ? heigtMapScale : null);
			DoEmissionArea(material);
			m_MaterialEditor.TexturePropertySingleLine(Styles.detailMaskText, detailMask);
            //m_MaterialEditor.TexturePropertySingleLine(Styles.occlusionText, occlusionMap, occlusionMap.textureValue != null ? occlusionStrength : null);
			EditorGUI.BeginChangeCheck();
			m_MaterialEditor.TextureScaleOffsetProperty(albedoMap);
			if (EditorGUI.EndChangeCheck())
				emissionMap.textureScaleAndOffset = albedoMap.textureScaleAndOffset; // Apply the main texture scale and offset to the emission texture as well, for Enlighten's sake
            //m_MaterialEditor.TexturePropertySingleLine(Styles.occlusionText, occlusionMap, occlusionMap.textureValue != null ? occlusionStrength : null);
            DoOutlineArea(material);
            DoFresnelAlphaArea(material);
            EditorGUILayout.Space();

			// Secondary properties
            // Secondary排序
			GUILayout.Label(Styles.secondaryMapsText, EditorStyles.boldLabel);
			m_MaterialEditor.TexturePropertySingleLine(Styles.detailAlbedoText, detailAlbedoMap);
			m_MaterialEditor.TexturePropertySingleLine(Styles.detailNormalMapText, detailNormalMap, detailNormalMapScale);
			m_MaterialEditor.TextureScaleOffsetProperty(detailAlbedoMap);
			m_MaterialEditor.ShaderProperty(uvSetSecondary, Styles.uvSetLabel.text);
		}
		if (EditorGUI.EndChangeCheck())
		{
			foreach (var obj in blendMode.targets)
				MaterialChanged((Material)obj, m_WorkflowMode);
		}
	}


    void ModePopup(Material material)
	{
	    if (cullMode != null)
	    {
	        m_MaterialEditor.ShaderProperty(cullMode, cullMode.displayName);
	    }

	    if (fogMode != null)
	    {
	        m_MaterialEditor.ShaderProperty(fogMode, fogMode.displayName);
            SetKeyword(material, "_FOG", (int)fogMode.floatValue == 1);
	    }

        if (lightMode != null)
        {
            m_MaterialEditor.ShaderProperty(lightMode, lightMode.displayName);
            SetKeyword(material, "_VERTEX_LIGHT", (int)lightMode.floatValue == 1);
        }
    }
	
	void OrthoNormalizeToggle()
	{
		if(orthoNormalize == null)
			return;
		
		EditorGUI.showMixedValue = orthoNormalize.hasMixedValue;
		var on = Mathf.RoundToInt(orthoNormalize.floatValue);
		
		EditorGUI.BeginChangeCheck();
			on = EditorGUILayout.Toggle(Styles.orthoNormalizeText, on == 1) ? 1 : 0;
		if (EditorGUI.EndChangeCheck())
		{
			m_MaterialEditor.RegisterPropertyChangeUndo("Orthonormalize");
			orthoNormalize.floatValue = (float)on;
		}
		
		EditorGUI.showMixedValue = false;
	}

	bool SmoothnessInAlbedoToggle()
	{
		if(smoothnessInAlbedo == null)
			return false;
		
		EditorGUI.showMixedValue = smoothnessInAlbedo.hasMixedValue;
		var on = Mathf.RoundToInt(smoothnessInAlbedo.floatValue);
		
		EditorGUI.BeginChangeCheck();
			on = EditorGUILayout.Toggle(Styles.smoothnessInAlbedoText, on == 1) ? 1 : 0;
		if (EditorGUI.EndChangeCheck())
		{
			m_MaterialEditor.RegisterPropertyChangeUndo("SmoothnessInAlbedo");
			smoothnessInAlbedo.floatValue = (float)on;
		}
		
		EditorGUI.showMixedValue = false;
		return (on == 1);
	}

	void BlendModePopup()
	{
		EditorGUI.showMixedValue = blendMode.hasMixedValue;
		var mode = (BlendMode)blendMode.floatValue;

		EditorGUI.BeginChangeCheck();
		mode = (BlendMode)EditorGUILayout.Popup(Styles.renderingMode, (int)mode, Styles.blendNames);
		if (EditorGUI.EndChangeCheck())
		{
			m_MaterialEditor.RegisterPropertyChangeUndo("Rendering Mode");
			blendMode.floatValue = (float)mode;
		}

		EditorGUI.showMixedValue = false;
	}

	void DoAlbedoArea(Material material)
	{
		m_MaterialEditor.TexturePropertySingleLine(Styles.albedoText, albedoMap, albedoColor);
        m_MaterialEditor.ShaderProperty(albedoStrength, albedoStrength.displayName);

		if (((BlendMode)material.GetFloat("_Mode") == BlendMode.Cutout))
		{
			m_MaterialEditor.ShaderProperty(alphaCutoff, Styles.alphaCutoffText.text, MaterialEditor.kMiniTextureFieldLabelIndentLevel+1);
		}
        if (((BlendMode)material.GetFloat("_Mode") == BlendMode.Transparent) || ((BlendMode)material.GetFloat("_Mode") == BlendMode.Fade))
	    {
            m_MaterialEditor.ShaderProperty(transparency, transparency.displayName);

	        if (transparencyMaskMode != null)
	        {
	            m_MaterialEditor.ShaderProperty(transparencyMaskMode, transparencyMaskMode.displayName);
                SetKeyword(material, "_DRAGON", (int)transparencyMaskMode.floatValue == 1);
                if ((int)transparencyMaskMode.floatValue == 1)
	            {
                    m_MaterialEditor.ShaderProperty(transparencyMaskMap, transparencyMaskMap.displayName);
                    m_MaterialEditor.ShaderProperty(transparencyMask, transparencyMask.displayName);
                    m_MaterialEditor.ShaderProperty(scale, scale.displayName);
	            }
	        }
	        else
	        {
                SetKeyword(material, "_DRAGON", false);
	        }
	    }
	}

    private void DoEmissionArea(Material material)
    {
        EditorGUI.showMixedValue = emissionMode.hasMixedValue;
        var mode = (EmissionMode)emissionMode.floatValue;
        EditorGUI.BeginChangeCheck();
        mode = (EmissionMode)EditorGUILayout.Popup(Styles.emissionMode, (int)mode, Styles.emissionNames);
        if (EditorGUI.EndChangeCheck())
        {
            m_MaterialEditor.RegisterPropertyChangeUndo("Emission Mode");
            emissionMode.floatValue = (float)mode;
        }
        EditorGUI.showMixedValue = false;

        SetKeyword(material, "_EMISSION", mode == EmissionMode.Emission || mode == EmissionMode.EmissionMask || mode == EmissionMode.EmissionAdd || mode == EmissionMode.EmissionAndFresnel);
        SetKeyword(material, "_EMISSION_ADD", mode == EmissionMode.EmissionAdd);
        SetKeyword(material, "_EMISSION_MASK", mode == EmissionMode.EmissionMask);
        SetKeyword(material, "_FRESNEL", mode == EmissionMode.Fresnel || mode == EmissionMode.EmissionAndFresnel);

        if (mode == EmissionMode.Emission || mode == EmissionMode.EmissionMask || mode == EmissionMode.EmissionAdd || mode == EmissionMode.EmissionAndFresnel)
        {
            float brightness = emissionColorForRendering.colorValue.maxColorComponent;
            bool showHelpBox = !HasValidEmissiveKeyword(material);
            bool showEmissionColorAndGIControls = brightness > 0.0f;

            bool hadEmissionTexture = emissionMap.textureValue != null;

            // Texture and HDR color controls
            m_MaterialEditor.TexturePropertyWithHDRColor(Styles.emissionText, emissionMap, emissionColorForRendering,
                m_EmissionColorPickerHDRConfig, false);

            // If texture was assigned and color was black set color to white
            if (emissionMap.textureValue != null && !hadEmissionTexture && brightness <= 0f)
                emissionColorForRendering.colorValue = Color.white;

            // Dynamic Lightmapping mode
            if (showEmissionColorAndGIControls)
            {
                bool shouldEmissionBeEnabled = ShouldBeEnabledByColor(emissionColorForRendering.colorValue);
                EditorGUI.BeginDisabledGroup(!shouldEmissionBeEnabled);

                m_MaterialEditor.LightmapEmissionProperty(MaterialEditor.kMiniTextureFieldLabelIndentLevel + 1);
                m_MaterialEditor.ShaderProperty(emissionMul, emissionMul.displayName);

                if (mode == EmissionMode.EmissionMask)
                {
                    m_MaterialEditor.ShaderProperty(emissionMask, emissionMask.displayName);
                    m_MaterialEditor.ShaderProperty(emissionMaskMin, emissionMaskMin.displayName);
                    m_MaterialEditor.ShaderProperty(emissionMaskMax, emissionMaskMax.displayName);
                    m_MaterialEditor.ShaderProperty(emissionMaskMul, emissionMaskMul.displayName);
                }
                else if(mode == EmissionMode.EmissionAdd)
                {
                    m_MaterialEditor.ShaderProperty(emissionAddIndex, emissionAddIndex.displayName);
                    m_MaterialEditor.ShaderProperty(emissionAddMul, emissionAddMul.displayName);
                }

                EditorGUI.EndDisabledGroup();
            }

            if (showHelpBox)
            {
                EditorGUILayout.HelpBox(Styles.emissiveWarning.text, MessageType.Warning);
            }
        }

        if (mode == EmissionMode.Fresnel || mode == EmissionMode.EmissionAndFresnel)
        {
            float brightness = fresnelColorForRendering.colorValue.maxColorComponent;
            bool showHelpBox = !HasValidFresnelKeyword(material);

            bool hadFresnelTexture = fresnelMap.textureValue != null;

            // Texture and HDR color controls
            m_MaterialEditor.TexturePropertyWithHDRColor(Styles.fresnelText, fresnelMap, fresnelColorForRendering,
                m_FresnelColorPickerHDRConfig, false);

            // If texture was assigned and color was black set color to white
            if (fresnelMap.textureValue != null && !hadFresnelTexture && brightness <= 0f)
                fresnelColorForRendering.colorValue = Color.white;

            m_MaterialEditor.ShaderProperty(fresnelAdd, fresnelAdd.displayName);
            m_MaterialEditor.ShaderProperty(fresnelMul, fresnelMul.displayName);
            m_MaterialEditor.ShaderProperty(fresnelExp, fresnelExp.displayName);

            if (showHelpBox)
            {
                EditorGUILayout.HelpBox(Styles.fresnelWarning.text, MessageType.Warning);
            }
        }
    }

    private void DoOutlineArea(Material material)
    {
        m_MaterialEditor.ShaderProperty(outlineSwitch, outlineSwitch.displayName);
        if ((int)outlineSwitch.floatValue == 1)
        {
            m_MaterialEditor.ShaderProperty(outlineColor, outlineColor.displayName);
            m_MaterialEditor.ShaderProperty(outlineAdd, outlineAdd.displayName);
            m_MaterialEditor.ShaderProperty(outlineMul, outlineMul.displayName);
            m_MaterialEditor.ShaderProperty(outlineExp, outlineExp.displayName);
            m_MaterialEditor.ShaderProperty(outlineWidth, outlineWidth.displayName);
            m_MaterialEditor.ShaderProperty(outlineFactor, outlineFactor.displayName);
        }
    }

    private void DoFresnelAlphaArea(Material material)
    {
        m_MaterialEditor.ShaderProperty(fresnelAlphaMode, fresnelAlphaMode.displayName);
        SetKeyword(material, "_FRESNEL_ALPHA", (int)fresnelAlphaMode.floatValue == 1);
        if ((int)fresnelAlphaMode.floatValue == 1) {
            blendMode.floatValue = (float)BlendMode.Transparent;
            m_MaterialEditor.ShaderProperty(fresnelAlphaAdd, fresnelAlphaAdd.displayName);
            m_MaterialEditor.ShaderProperty(fresnelAlphaMul, fresnelAlphaMul.displayName);
            m_MaterialEditor.ShaderProperty(fresnelAlphaExp, fresnelAlphaExp.displayName);
		} 
    }

    void DoSpecularMetallicArea()
	{
		if (m_WorkflowMode == WorkflowMode.Specular)
		{
			if (specularMap.textureValue == null) {
				if(smoothnessInAlbedo == null) {
					m_MaterialEditor.TexturePropertyTwoLines(Styles.specularMapText, specularMap, specularColor, Styles.smoothnessText, smoothness);
				} else {
					m_MaterialEditor.TexturePropertySingleLine(Styles.specularMapText, specularMap, specularColor);
					int indent = 3;
					EditorGUI.indentLevel += indent;
					if (!SmoothnessInAlbedoToggle())
						m_MaterialEditor.ShaderProperty(smoothness, Styles.smoothnessText.text);
					EditorGUI.indentLevel -= indent;
				}
			} else {
				m_MaterialEditor.TexturePropertySingleLine(Styles.specularMapText, specularMap);
				if(specularMapColorTweak != null)
					m_MaterialEditor.ColorProperty(specularMapColorTweak, specularMapColorTweak.displayName);

				if(smoothnessTweak1 != null && smoothnessTweak2 != null) {
					m_MaterialEditor.ShaderProperty(smoothnessTweak1, smoothnessTweak1.displayName);
					m_MaterialEditor.ShaderProperty(smoothnessTweak2, smoothnessTweak2.displayName);
					
					if(GUI.changed && smoothnessTweaks != null)
						smoothnessTweaks.vectorValue = new Vector4(smoothnessTweak1.floatValue, smoothnessTweak2.floatValue);

                m_MaterialEditor.TexturePropertySingleLine(Styles.occlusionText, occlusionMap, occlusionMap.textureValue != null ? occlusionStrength : null);
				}
			}
		}
		else if (m_WorkflowMode == WorkflowMode.Metallic)
		{
		    m_MaterialEditor.TexturePropertySingleLine(Styles.metallicMapText, metallicMap);

		    if (metallicMap.textureValue == null)
		    {
                m_MaterialEditor.ShaderProperty(metallic, metallic.displayName);
                m_MaterialEditor.ShaderProperty(smoothness, smoothness.displayName);
		    }
            else if (metallicStrength != null && glossStrength != null)
		    {
                m_MaterialEditor.ShaderProperty(metallicStrength, metallicStrength.displayName);
                m_MaterialEditor.ShaderProperty(glossStrength, glossStrength.displayName);
		    }

            if (glossmin != null && glossmax != null)
            {
                m_MaterialEditor.ShaderProperty(glossmin, glossmin.displayName);
                m_MaterialEditor.ShaderProperty(glossmax, glossmax.displayName);
            }

             m_MaterialEditor.ShaderProperty(occlusionStrength, occlusionStrength.displayName);
         }
	}

    public static void SetupMaterialWithBlendMode(Material material, BlendMode blendMode)
	{
		switch (blendMode)
		{
			case BlendMode.Opaque:
				material.SetOverrideTag("RenderType", "");
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
				material.SetInt("_ZWrite", 1);
				material.DisableKeyword("_ALPHATEST_ON");
				material.DisableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = -1;
				break;
			case BlendMode.Cutout:
				material.SetOverrideTag("RenderType", "TransparentCutout");
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
				material.SetInt("_ZWrite", 1);
				material.EnableKeyword("_ALPHATEST_ON");
				material.DisableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = 2450;
				break;
			case BlendMode.Fade:
				material.SetOverrideTag("RenderType", "Transparent");
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				material.SetInt("_ZWrite", 0);
				material.DisableKeyword("_ALPHATEST_ON");
				material.EnableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = 3000;
				break;
			case BlendMode.Transparent:
				material.SetOverrideTag("RenderType", "Transparent");
				//material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 1);
				material.DisableKeyword("_ALPHATEST_ON");
				material.DisableKeyword("_ALPHABLEND_ON");
				material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = 3000;
				break;
		}
	}
	
	static bool ShouldBeEnabledByColor (Color color)
	{
		return color.maxColorComponent > (0.1f / 255.0f);
	}

	static void SetMaterialKeywords(Material material, WorkflowMode workflowMode)
	{
		// Note: keywords must be based on Material value not on MaterialProperty due to multi-edit & material animation
		// (MaterialProperty value might come from renderer material property block)
		SetKeyword (material, "_NORMALMAP", material.GetTexture ("_BumpMap") || material.GetTexture ("_DetailNormalMap"));
		SetKeyword (material, "ORTHONORMALIZE_TANGENT_BASE", material.HasProperty("__orthonormalize") && material.GetFloat("__orthonormalize") > 0.5f);
		SetKeyword (material, "SMOOTHNESS_IN_ALBEDO", material.HasProperty("__smoothnessinalbedo") && material.GetFloat("__smoothnessinalbedo") > 0.5f && !material.GetTexture ("_SpecGlossMap"));
		if (workflowMode == WorkflowMode.Specular)
			SetKeyword (material, "_SPECGLOSSMAP", material.GetTexture ("_SpecGlossMap"));
		else if (workflowMode == WorkflowMode.Metallic)
			SetKeyword (material, "_METALLICGLOSSMAP", material.GetTexture ("_MetallicGlossMap"));
		//SetKeyword (material, "_PARALLAXMAP", material.GetTexture ("_ParallaxMap"));
		SetKeyword (material, "_DETAIL_MULX2", material.GetTexture ("_DetailAlbedoMap") || material.GetTexture ("_DetailNormalMap"));

        //bool shouldEmissionBeEnabled = ShouldBeEnabledByColor(material.GetColor("_EmissionColor"));
		//SetKeyword (material, "_EMISSION", shouldEmissionBeEnabled);
        //bool shouldFresnelBeEnabled = ShouldBeEnabledByColor(material.GetColor("_FresnelColor"));
        //SetKeyword(material, "_FRESNEL", shouldFresnelBeEnabled);

		// Setup lightmap emissive flags
		MaterialGlobalIlluminationFlags flags = material.globalIlluminationFlags;
		if ((flags & (MaterialGlobalIlluminationFlags.BakedEmissive | MaterialGlobalIlluminationFlags.RealtimeEmissive)) != 0)
		{
			flags &= ~MaterialGlobalIlluminationFlags.EmissiveIsBlack;
			if (!ShouldBeEnabledByColor(material.GetColor("_EmissionColor")))
				flags |= MaterialGlobalIlluminationFlags.EmissiveIsBlack;

			material.globalIlluminationFlags = flags;
		}
	}

	bool HasValidEmissiveKeyword (Material material)
	{
		// Material animation might be out of sync with the material keyword.
		// So if the emission support is disabled on the material, but the property blocks have a value that requires it, then we need to show a warning.
		// (note: (Renderer MaterialPropertyBlock applies its values to emissionColorForRendering))
		bool hasEmissionKeyword = material.IsKeywordEnabled ("_EMISSION");

        if (!hasEmissionKeyword && ShouldBeEnabledByColor(emissionColorForRendering.colorValue))
			return false;
		else
			return true;
	}

    bool HasValidFresnelKeyword(Material material)
    {
        // Material animation might be out of sync with the material keyword.
        // So if the emission support is disabled on the material, but the property blocks have a value that requires it, then we need to show a warning.
        // (note: (Renderer MaterialPropertyBlock applies its values to emissionColorForRendering))
        bool hasEmissionKeyword = material.IsKeywordEnabled("_FRESNEL");

        if (!hasEmissionKeyword && ShouldBeEnabledByColor(fresnelColorForRendering.colorValue))
            return false;
        else
            return true;
    }

	static void MaterialChanged(Material material, WorkflowMode workflowMode)
	{
		SetupMaterialWithBlendMode(material, (BlendMode)material.GetFloat("_Mode"));

		SetMaterialKeywords(material, workflowMode);
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
