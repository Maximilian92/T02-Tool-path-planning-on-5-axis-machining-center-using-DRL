using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Animations;

public class Cam_UI : MonoBehaviour
{
	/* Camera component */

	Camera CamUI;

	/********************* Template methods *********************/
	void Awake()
	{
		CamUI = gameObject.GetComponent<Camera>();
	}
	void Start()
	{
		CamUI.clearFlags = CameraClearFlags.Depth;
		CamUI.cullingMask = 1 << LayerMask.NameToLayer("UI");
		CamUI.fieldOfView = 60;
		CamUI.nearClipPlane = 0.01f;
		CamUI.farClipPlane = 1000;
		CamUI.depth = 0;
	}
}