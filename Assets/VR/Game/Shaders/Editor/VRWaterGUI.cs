using System;
using UnityEngine;

namespace UnityEditor
{
class VRWaterGUI : ShaderGUI
{


	private static class Styles
	{
        public static string whiteSpaceString = " ";
        public static string distortion = "Distortion";
        public static string albedo = "Beam";
        public static string falloff = "FallOff";
	}

	//MaterialProperty cullMode = null;

	MaterialProperty normalmap_01 = null;
    MaterialProperty uspeed_01 = null;
    MaterialProperty vspeed_01 = null;

	MaterialProperty normalmap_02 = null;
    MaterialProperty uspeed_02 = null;
    MaterialProperty vspeed_02 = null;
    MaterialProperty normal_inten = null;

    MaterialProperty maskmap = null;
    MaterialProperty alpha_inten = null;

    MaterialProperty specularmap = null;
    MaterialProperty specularcolor = null;
    MaterialProperty specularinten = null;
    MaterialProperty specularedge = null;

    MaterialProperty muddycolor = null;
    MaterialProperty muddyalpha = null;

    MaterialProperty diffusecolor = null;
    MaterialProperty diffusealpha = null;
    MaterialProperty camerarange = null;
    MaterialProperty rangeconstrast = null;

    MaterialProperty riverbedmap = null;
    MaterialProperty riverbedcolor = null;

    MaterialProperty beamMode = null;
    MaterialEditor m_MaterialEditor;

	bool m_FirstTimeApply = true;

	public void FindProperties (MaterialProperty[] props)
	{
        //cullMode = FindProperty("_Cull", props);

        normalmap_01 = FindProperty("_Normal_01", props);
        uspeed_01 = FindProperty("_Uspeed_01", props);
        vspeed_01 = FindProperty("_Vspeed_01", props);

        normalmap_02 = FindProperty("Normal_02", props);
        uspeed_02 = FindProperty("_Uspeed_02", props);
        vspeed_02 = FindProperty("_Vspeed_02", props);
        normal_inten = FindProperty("_Normal_Inten", props);

        maskmap = FindProperty("_Mask_Texture", props);
        alpha_inten = FindProperty("_Distortion_U_Speed", props);

        specularmap = FindProperty("_Specular_Tex", props);
        specularcolor = FindProperty("_Specular_Color", props);
        specularinten = FindProperty("_Specular_Inten", props);
        specularedge = FindProperty("_Specular_Edge", props);

        diffusecolor = FindProperty("_DiffuseColor", props);
        diffusealpha = FindProperty("_Diffuse_Alpha", props);

        camerarange = FindProperty("_Camera_Range", props);
        rangeconstrast = FindProperty("_range_constrast", props);
        
        beamMode = FindProperty("_Beam_Mode", props);
        riverbedcolor = FindProperty("_River_Bed_Map", props);
        riverbedmap = FindProperty("_River_bed_Color", props);

        //fogMode = FindProperty("_Fog_Mode", props);
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

        GUILayout.Label(Styles.distortion, EditorStyles.boldLabel);
        DoMainArea(material);
        EditorGUILayout.Space();

        GUILayout.Label(Styles.albedo, EditorStyles.boldLabel);
        DoRiverBedArea(material);
        EditorGUILayout.Space();

        //GUILayout.Label(Styles.falloff, EditorStyles.boldLabel);
        //DoFallOffArea(material);
        //EditorGUILayout.Space();
	}

    void DoMainArea(Material material)
    {
        //m_MaterialEditor.ShaderProperty(cullMode, cullMode.displayName);
        //m_MaterialEditor.ShaderProperty(fogMode, fogMode.displayName);
        //SetKeyword(material, "_FOG", (int)fogMode.floatValue == 1);
        m_MaterialEditor.ShaderProperty(diffusecolor, diffusecolor.displayName);
        m_MaterialEditor.ShaderProperty(normalmap_01, normalmap_01.displayName);
        m_MaterialEditor.ShaderProperty(uspeed_01, uspeed_01.displayName);
        m_MaterialEditor.ShaderProperty(vspeed_01, vspeed_01.displayName);
        m_MaterialEditor.ShaderProperty(normalmap_02, normalmap_02.displayName);
        m_MaterialEditor.ShaderProperty(uspeed_02, uspeed_02.displayName);
        m_MaterialEditor.ShaderProperty(vspeed_02, vspeed_02.displayName);
        m_MaterialEditor.ShaderProperty(normal_inten, normal_inten.displayName);
        //m_MaterialEditor.ShaderProperty(mapBleach, mapBleach.displayName);
    }

    void DoRiverBedArea(Material material)
	{
        m_MaterialEditor.ShaderProperty(beamMode, beamMode.displayName);
        SetKeyword(material, "_BEAM", (int)beamMode.floatValue == 1);
        if ((int)beamMode.floatValue == 1)
        {
            m_MaterialEditor.ShaderProperty(maskmap, maskmap.displayName);
            m_MaterialEditor.ShaderProperty(alpha_inten, alpha_inten.displayName);
            m_MaterialEditor.ShaderProperty(specularmap, specularmap.displayName);
        }
	}


	void SetMaterialKeywords(Material material)
	{
        SetKeyword(material, "_BEAM", (int)beamMode.floatValue == 1);
        //SetKeyword(material, "_FOG", (int)beamMode.floatValue == 1);
        //SetKeyword(material, "_FALLOFF", (int)falloffMode.floatValue == 1);
	}

    //void DoFallOffArea(Material material)
    //{
    //    m_MaterialEditor.ShaderProperty(falloffMode, falloffMode.displayName);
    //    SetKeyword(material, "_FALLOFF", (int)falloffMode.floatValue == 1);
    //    if ((int)falloffMode.floatValue == 1)
    //    {
    //        m_MaterialEditor.ShaderProperty(falloff, falloff.displayName);
    //    }
    //}

	static void SetKeyword(Material m, string keyword, bool state)
	{
		if (state)
			m.EnableKeyword (keyword);
		else
			m.DisableKeyword (keyword);
	}

}

} // namespace UnityEditor
