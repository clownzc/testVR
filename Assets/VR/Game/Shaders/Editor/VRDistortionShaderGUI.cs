using System;
using UnityEngine;

namespace UnityEditor
{
class VRDistortionShaderGUI : ShaderGUI
{


	private static class Styles
	{
        public static string whiteSpaceString = " ";
        public static string distortion = "Common";
        public static string albedo = "Distortion";
	}

    MaterialProperty ZTest = null;
    MaterialProperty lightingMode = null;
    MaterialProperty cullMode = null;
    MaterialProperty fogMode = null;

	MaterialProperty albedo = null;
    MaterialProperty tint = null;

    MaterialProperty normal = null;
    MaterialProperty mixMap = null;
    MaterialProperty dissolveColor = null;
    MaterialProperty transparencyMask = null;
    MaterialProperty dissolveColorMul = null;
    MaterialProperty fresnelColor = null;
    MaterialProperty emissiveColor = null;
    MaterialProperty emissiveInten = null;
    MaterialProperty edgeThickness = null;
    MaterialProperty fresnelAdd = null;
    MaterialProperty fresnelPow = null;
    MaterialProperty fresnelMul = null;
    MaterialProperty mixTexture01 = null;

    MaterialEditor m_MaterialEditor;

	bool m_FirstTimeApply = true;

	public void FindProperties (MaterialProperty[] props)
	{
        lightingMode = FindProperty("_Lighting_Mode", props);
        ZTest = FindProperty("_ZTest", props);
        cullMode = FindProperty("_Cull", props);
        fogMode = FindProperty("_Fog", props);

        albedo = FindProperty("_Albedo", props);
        tint = FindProperty("_Tint_Color", props);

        normal = FindProperty("_Normal", props);
        mixMap = FindProperty("_Mix_Texture", props);
        dissolveColor = FindProperty("_Dissolve_Color", props);
        transparencyMask = FindProperty("_TransparencyMask", props);
        dissolveColorMul = FindProperty("_Dissolve_Color_Mul", props);
        fresnelColor = FindProperty("_Fresnel_Color", props);
        emissiveColor = FindProperty("_Emissive_Color", props);
        emissiveInten = FindProperty("_Emissive_Inten", props);
        edgeThickness = FindProperty("_Edge_Thickness", props);
        fresnelAdd = FindProperty("_Fresnel_Add", props);
        fresnelPow = FindProperty("_Fresnel_Pow", props);
        fresnelMul = FindProperty("_Fresnel_Mul", props);
        mixTexture01 = FindProperty("_Mix_Texture_01", props);
	}
	
    //Switch
	public override void AssignNewShaderToMaterial (Material material, Shader oldShader, Shader newShader)
	{
		base.AssignNewShaderToMaterial(material, oldShader, newShader);

            // Re-run this in case the new shader needs custom setup.

            // Shader switch to prevent the original shader to eliminate custom parameters
            m_FirstTimeApply = true;
	}
    //On GUI
	public override void OnGUI (MaterialEditor materialEditor, MaterialProperty[] props)
	{
		FindProperties (props); // MaterialProperties can be animated so we do not cache them but fetch them every event to ensure animated values are updated correctly
        //Find shader Switch definition
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
    

	public void ShaderPropertiesGUI (Material material) //GUI Shape
	{
		// Use default labelWidth 
		EditorGUIUtility.labelWidth = 0f;

		// Detect any changes to the material

        GUILayout.Label(Styles.distortion, EditorStyles.boldLabel);
        DoCommonArea(material);
        EditorGUILayout.Space();

        GUILayout.Label(Styles.albedo, EditorStyles.boldLabel); // EditorStyles Built-in API Bold
        
        DoDissolveArea(material);
        EditorGUILayout.Space(); //Empty row
        }

    void DoCommonArea(Material material)
    {
        m_MaterialEditor.ShaderProperty(cullMode, cullMode.displayName);
        m_MaterialEditor.ShaderProperty(lightingMode, lightingMode.displayName); //Example: Lighting Mode name use shader Switch name
        if ((int)lightingMode.floatValue == 1)
        {
            material.SetOverrideTag("LightMode", "ForwardBase");
        }
        else
        {
            material.SetOverrideTag("LightMode", "Always");
        }

        m_MaterialEditor.ShaderProperty(ZTest, ZTest.displayName);
        
        m_MaterialEditor.ShaderProperty(fogMode, fogMode.displayName);

        SetMaterialKeywords(material);
    }

    void DoDissolveArea(Material material)
    {
        m_MaterialEditor.ShaderProperty(albedo, albedo.displayName);
        m_MaterialEditor.ShaderProperty(tint, tint.displayName);

        m_MaterialEditor.ShaderProperty(normal, normal.displayName);
        m_MaterialEditor.ShaderProperty(mixMap, mixMap.displayName);
        m_MaterialEditor.ShaderProperty(dissolveColor, dissolveColor.displayName);
        m_MaterialEditor.ShaderProperty(transparencyMask, transparencyMask.displayName);
        m_MaterialEditor.ShaderProperty(dissolveColorMul, dissolveColorMul.displayName);
        m_MaterialEditor.ShaderProperty(fresnelColor, fresnelColor.displayName);
        m_MaterialEditor.ShaderProperty(emissiveColor, emissiveColor.displayName);
        m_MaterialEditor.ShaderProperty(emissiveInten, emissiveInten.displayName);
        m_MaterialEditor.ShaderProperty(edgeThickness, edgeThickness.displayName);
        m_MaterialEditor.ShaderProperty(fresnelAdd, fresnelAdd.displayName);
        m_MaterialEditor.ShaderProperty(fresnelPow, fresnelPow.displayName);
        m_MaterialEditor.ShaderProperty(fresnelMul, fresnelMul.displayName);
        m_MaterialEditor.ShaderProperty(mixTexture01, mixTexture01.displayName);
    
    }

    void SetMaterialKeywords(Material material)
    {
        SetKeyword(material, "_LIGHTING", (int)lightingMode.floatValue == 1);
        SetKeyword(material, "_Fog", (int)fogMode.floatValue == 1);
    }

    static void SetKeyword(Material m, string keyword, bool state)
    {
        if (state)
            m.EnableKeyword(keyword);
        else
            m.DisableKeyword(keyword);
    }
}

} // namespace UnityEditor
