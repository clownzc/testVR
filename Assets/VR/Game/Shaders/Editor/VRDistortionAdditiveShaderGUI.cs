using System;
using UnityEngine;

namespace UnityEditor
{
class VRDistortionAdditiveShaderGUI : ShaderGUI
{


	private static class Styles
	{
        public static string whiteSpaceString = " ";
        public static string distortion = "Distortion";
        public static string albedo = "Beam";
        public static string falloff = "FallOff";
	}

	MaterialProperty cullMode = null;

	MaterialProperty mainMap = null;
    MaterialProperty tint = null;

	MaterialProperty distortionMap = null;
    MaterialProperty distortionVector = null;
    MaterialProperty distortionUSpeed = null;
    MaterialProperty distortionVSpeed = null;

    MaterialProperty maskMap = null;
    MaterialProperty mapBleach = null;

    MaterialProperty beamMode = null;
    MaterialProperty plannerGSpeed = null;
    MaterialProperty plannerBSpeed = null;
    MaterialProperty beamEdgeAlpha = null;

    MaterialProperty falloffMode = null;
    MaterialProperty falloff = null;
    MaterialProperty edgehardness = null;

    MaterialProperty fogMode = null;

    MaterialEditor m_MaterialEditor;

	bool m_FirstTimeApply = true;

	public void FindProperties (MaterialProperty[] props)
	{
        cullMode = FindProperty("_Cull", props);

        mainMap = FindProperty("_Main_Texture", props);
        tint = FindProperty("_Tint", props);

        distortionMap = FindProperty("_Distortion_Tex_01", props);
        distortionVector = FindProperty("_ColorDistort_Inten", props);
        distortionUSpeed = FindProperty("_Distortion_U_Speed", props);
        distortionVSpeed = FindProperty("_Distortion_V_Speed", props);

        maskMap = FindProperty("_Mask_Texture", props);
        mapBleach = FindProperty("_tex_bleach", props);

        beamMode = FindProperty("_Beam_Mode", props);
        plannerGSpeed = FindProperty("_Planner_G_Speed", props);
        plannerBSpeed = FindProperty("_Planner_B_Speed", props);
        beamEdgeAlpha = FindProperty("_Beam_edge_Alpha", props);

        falloffMode = FindProperty("_FallOff_Mode", props);
        falloff = FindProperty("_FallOff", props);
        edgehardness = FindProperty("_EdgeHardness", props);
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
        DoDistortionArea(material);
        EditorGUILayout.Space();

        GUILayout.Label(Styles.albedo, EditorStyles.boldLabel);
        DoBeamArea(material);
        EditorGUILayout.Space();

        GUILayout.Label(Styles.falloff, EditorStyles.boldLabel);
        DoFallOffArea(material);
        EditorGUILayout.Space();
	}

    void DoDistortionArea(Material material)
    {
        m_MaterialEditor.ShaderProperty(cullMode, cullMode.displayName);
        //m_MaterialEditor.ShaderProperty(fogMode, fogMode.displayName);
        //SetKeyword(material, "_FOG", (int)fogMode.floatValue == 1);

        m_MaterialEditor.ShaderProperty(mainMap, mainMap.displayName);
        m_MaterialEditor.ShaderProperty(tint, tint.displayName);
        m_MaterialEditor.ShaderProperty(distortionMap, distortionMap.displayName);
        m_MaterialEditor.ShaderProperty(distortionVector, distortionVector.displayName);
        m_MaterialEditor.ShaderProperty(distortionUSpeed, distortionUSpeed.displayName);
        m_MaterialEditor.ShaderProperty(distortionVSpeed, distortionVSpeed.displayName);
        m_MaterialEditor.ShaderProperty(maskMap, maskMap.displayName);
        m_MaterialEditor.ShaderProperty(mapBleach, mapBleach.displayName);
    }

    void DoBeamArea(Material material)
	{
        m_MaterialEditor.ShaderProperty(beamMode, beamMode.displayName);
        SetKeyword(material, "_BEAM", (int)beamMode.floatValue == 1);
        if ((int)beamMode.floatValue == 1)
        {
            m_MaterialEditor.ShaderProperty(plannerGSpeed, plannerGSpeed.displayName);
            m_MaterialEditor.ShaderProperty(plannerBSpeed, plannerBSpeed.displayName);
            m_MaterialEditor.ShaderProperty(beamEdgeAlpha, beamEdgeAlpha.displayName);
        }
	}


	void SetMaterialKeywords(Material material)
	{
        SetKeyword(material, "_BEAM", (int)beamMode.floatValue == 1);
        SetKeyword(material, "_FOG", (int)beamMode.floatValue == 1);
        SetKeyword(material, "_FALLOFF", (int)falloffMode.floatValue == 1);
	}

    void DoFallOffArea(Material material)
    {
        m_MaterialEditor.ShaderProperty(falloffMode, falloffMode.displayName);
        SetKeyword(material, "_FALLOFF", (int)falloffMode.floatValue == 1);
        if ((int)falloffMode.floatValue == 1)
        {
            m_MaterialEditor.ShaderProperty(falloff, falloff.displayName);
            m_MaterialEditor.ShaderProperty(edgehardness, edgehardness.displayName);
        }
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
