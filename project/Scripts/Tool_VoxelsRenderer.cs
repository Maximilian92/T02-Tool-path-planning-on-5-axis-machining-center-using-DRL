using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using MLAgents;

public class Tool_VoxelsRenderer : MonoBehaviour
{   
    /* Collection of voxels */

    Transform voxelsToCut;
    Transform voxelsToKeep;

    /* Agent */

    GameObject Agent;

    /* Rendering settings */

    bool fadeCut = false;
    bool fadeKeep = false;
    bool emissionCut = false;
    bool emissionKeep = false;
    bool lightCut = false;
	bool halo = false;
    bool blackKeep = false;

    enum RenderingMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent,
    }

    float emissionIntensity;
	float lightIntensity;
    float lightRange;
    float transparency;

    Color colorCut = Color.green;
    Color colorKeep = Color.grey;
    Color colorCutFade;
    Color colorKeepFade;
    Color colorCutEmission;
    Color colorKeepEmission;

    /********************* Template methods *********************/
    void Awake()
    {
        voxelsToCut = GameObject.Find("VoxelsToCut").transform;
        voxelsToKeep = GameObject.Find("VoxelsToKeep").transform;

        Agent = GameObject.Find("Agent");
    }
    void Start()
    {
        string EnvModeStr = Agent.GetComponent<AgentMT>().EnvModeStr;
        SetRenderingDetails(EnvModeStr);

        emissionIntensity = Agent.GetComponent<AgentMT>().emissionIntensity;
		lightIntensity = Agent.GetComponent<AgentMT>().lightIntensity;
        lightRange = Agent.GetComponent<AgentMT>().lightRange;
        transparency = Agent.GetComponent<AgentMT>().transparency;

        colorCutFade = new Color(colorCut.r, colorCut.g, colorCut.b, transparency);
        colorKeepFade = new Color(colorKeep.r, colorKeep.g, colorKeep.b, transparency);
        colorCutEmission = colorCut * Mathf.Pow(2f, emissionIntensity - (0.4169f));
        colorKeepEmission = colorKeep * Mathf.Pow(2f, emissionIntensity - (0.4169f));

        RenderVoxels();
    }

    /********************* Self defined methods *********************/
    private void SetRenderingDetails(string EnvModeStr)
    {
        if (EnvModeStr == "normal" || EnvModeStr == "darkTorchLight")
        {
        }
        if (EnvModeStr == "darkEmission1")
        {
            emissionCut = true;
            emissionKeep = true;
        }
        if (EnvModeStr == "darkEmission2")
        {
            emissionCut = true;
            fadeKeep = true;
        }
		if (EnvModeStr == "darkEmission3")
        {
            emissionCut = true;
            emissionKeep = true;
            fadeKeep = true;
        }
        if (EnvModeStr == "darkLight1")
        {
            lightCut = true;
        }
        if (EnvModeStr == "darkLight2")
        {
            lightCut = true;
            blackKeep = true;
        }
        if (EnvModeStr == "darkLight3")
        {
            fadeCut = true;
            lightCut = true;
			halo = true;
        }
        if (EnvModeStr == "darkLight3")
        {
            fadeCut = true;
            fadeKeep = true;
            lightCut = true;
			halo = true;
        }
    }
    internal void RenderVoxels()
    {
        foreach (Transform voxel in voxelsToCut)
        {
            GameObject Voxel = voxel.gameObject;
            if (fadeCut)
            {
                Voxel.GetComponent<MeshRenderer>().material.SetColor("_Color", colorCutFade);
                SetMaterialRenderingMode(Voxel.GetComponent<MeshRenderer>().material, RenderingMode.Fade);
            }
            else
            {
                Voxel.GetComponent<MeshRenderer>().material.SetColor("_Color", colorCut);
            }
            if (emissionCut)
            {
                Voxel.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", colorCutEmission);
                Voxel.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
            }
            if (lightCut)
            {
                Light voxToCutLight = Voxel.AddComponent<Light>();
                voxToCutLight.range = lightRange;
                voxToCutLight.color = colorCut;
				voxToCutLight.intensity = lightIntensity;
				if (halo)
				{
					SerializedObject voxToCutHalo = new SerializedObject(voxToCutLight);
					voxToCutHalo.FindProperty("m_DrawHalo").boolValue = true;
					voxToCutHalo.ApplyModifiedProperties();
				}
            }
        }
        foreach (Transform voxel in voxelsToKeep)
        {
            GameObject Voxel = voxel.gameObject;
            if (fadeKeep)
            {
                Voxel.GetComponent<MeshRenderer>().material.SetColor("_Color", colorKeepFade);
                SetMaterialRenderingMode(Voxel.GetComponent<MeshRenderer>().material, RenderingMode.Fade);
            }
            else if (blackKeep)
            {
                Voxel.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.black);               
            }
            else
            {
                Voxel.GetComponent<MeshRenderer>().material.SetColor("_Color", colorKeep);
            }
            if (emissionKeep)
            {
                Voxel.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", colorKeepEmission);
                Voxel.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
            }
        }
    }
    private void SetMaterialRenderingMode(Material material, RenderingMode renderingMode)
    {
        switch (renderingMode)
        {
            case RenderingMode.Opaque:
                material.SetFloat("_Mode", 0);
                material.SetInt("_SrcBlend", (int)BlendMode.One);
                material.SetInt("_DstBlend", (int)BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = -1;
                break;
            case RenderingMode.Cutout:
                material.SetFloat("_Mode", 1);
                material.SetInt("_SrcBlend", (int)BlendMode.One);
                material.SetInt("_DstBlend", (int)BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.EnableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 2450;
                break;
            case RenderingMode.Fade:
                material.SetFloat("_Mode", 2);
                material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;

            case RenderingMode.Transparent:
                material.SetFloat("_Mode", 3);
                material.SetInt("_SrcBlend", (int)BlendMode.One);
                material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
        }
    }
}