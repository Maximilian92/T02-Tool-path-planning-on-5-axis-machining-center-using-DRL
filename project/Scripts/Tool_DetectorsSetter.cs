using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Tool_DetectorsSetter : MonoBehaviour
{
	/* Collection of detectors to be faded */

	GameObject DetectorAll;
	GameObject DetectorAxisY1;
	GameObject DetectorAxisY2;
	GameObject DetectorCutter;

	/* Rendering settings */

	enum RenderingMode
	{
		Opaque,
		Cutout,
		Fade,
		Transparent,
	}

	Color colorDetector = Color.white;
	Color colorDetectorFade;
	float transparency = 0;

	/********************* Template methods *********************/
	void Awake()
	{
		DetectorAll = GameObject.Find("DetectorAll");
		DetectorAxisY1 = GameObject.Find("DetectorAxisY1");
		DetectorAxisY2 = GameObject.Find("DetectorAxisY2");
		DetectorCutter = GameObject.Find("DetectorCutter");

		HandleDetector(DetectorAll, "All");
		HandleDetector(DetectorAxisY1, "AxisY1");
		HandleDetector(DetectorAxisY2, "AxisY2");
		HandleDetector(DetectorCutter, "Cutter");
	}

	/********************* Self defined methods *********************/
	private void HandleDetector(GameObject Detector, string type)
	{
		Detector.tag = "detector";
		
		colorDetectorFade = new Color(colorDetector.r, colorDetector.g, colorDetector.b, transparency);
		Detector.GetComponent<MeshRenderer>().material.SetColor("_Color", colorDetectorFade);
		SetMaterialRenderingMode(Detector.GetComponent<MeshRenderer>().material, RenderingMode.Fade);

		Detector.AddComponent<Rigidbody>();
		Detector.GetComponent<Rigidbody>().useGravity = false;
		Detector.GetComponent<Rigidbody>().isKinematic = true;

		Detector.AddComponent<MeshCollider>();
		Detector.GetComponent<MeshCollider>().convex = true;

		if (type == "All")
		{
			Detector.AddComponent<Coll_DetectorAll>();
		}
		if (type == "AxisY1")
		{
			Detector.AddComponent<Coll_DetectorAxisY1>();
		}
		if (type == "AxisY2")
		{
			Detector.AddComponent<Coll_DetectorAxisY2>();
		}
		if (type == "Cutter")
		{
			Detector.AddComponent<Coll_DetectorCutter>();
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