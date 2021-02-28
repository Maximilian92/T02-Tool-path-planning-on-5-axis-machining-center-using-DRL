using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;

public class Cam_CutterTopViewInverse : MonoBehaviour
{
	/* Camera component */

	Camera CamCutterTopViewInverse;

	/* Reference object cutter */

	Transform cutter;

	/* Agent and assistant tools */

	GameObject Agent;
	GameObject AssistantTools;

	/* Coordinate system info */

	float biasY;
	float yLength;
	float cutterPositionStartY;

	float positionX;
	float positionZ;
	float positionDiffY;

	/*  Visual observation details */

	bool hidingTopViewToCut;
	bool hidingTopViewToKeep;

	/* View size */

	float fieldOfViewMin;
	float fieldOfViewMax;

	/********************* Template methods *********************/
	void Awake()
	{
		CamCutterTopViewInverse = gameObject.GetComponent<Camera>();

		cutter = GameObject.Find("Cutter").transform;

		Agent = GameObject.Find("Agent");
		AssistantTools = GameObject.Find("AssistantTools");
	}
	void Start()
    {
		hidingTopViewToCut = Agent.GetComponent<AgentMT>().hidingTopViewToCut;
		hidingTopViewToKeep = Agent.GetComponent<AgentMT>().hidingTopViewToKeep;
		fieldOfViewMin = Agent.GetComponent<AgentMT>().fieldOfViewMin;
		fieldOfViewMax = Agent.GetComponent<AgentMT>().fieldOfViewMax;

		CamCutterTopViewInverse.clearFlags = CameraClearFlags.SolidColor;
		CamCutterTopViewInverse.backgroundColor = Color.black;
		CamCutterTopViewInverse.cullingMask = (1 << LayerMask.NameToLayer("Voxel toCut")) +
											  (1 << LayerMask.NameToLayer("Voxel toKeep"));

		if (hidingTopViewToCut) CamCutterTopViewInverse.cullingMask &= ~(1 << LayerMask.NameToLayer("Voxel toCut"));
		if (hidingTopViewToKeep) CamCutterTopViewInverse.cullingMask &= ~(1 << LayerMask.NameToLayer("Voxel toKeep"));
		CamCutterTopViewInverse.orthographic = false;
		CamCutterTopViewInverse.fieldOfView = fieldOfViewMin;
				
		CamCutterTopViewInverse.nearClipPlane = 0.01f;
		CamCutterTopViewInverse.farClipPlane = 10;
		CamCutterTopViewInverse.depth = -2;

		biasY = -CamCutterTopViewInverse.nearClipPlane;
		yLength = Agent.GetComponent<AgentMT>().yLength;
		cutterPositionStartY = cutter.position.y;

		transform.localPosition = new Vector3(0, biasY, 0);
		transform.localEulerAngles = new Vector3(-90, 0, 0);

		positionX = transform.position.x;
		positionZ = transform.position.z;
		positionDiffY = cutterPositionStartY - transform.position.y;
	}
	void Update()
	{
		transform.position = new Vector3(cutter.position.x, cutter.position.y - positionDiffY, cutter.position.z);
		CamCutterTopViewInverse.fieldOfView = CalculateFieldOfView(cutter.position.y);
	}

	/********************* Self defined methods *********************/
	private float CalculateFieldOfView(float cutterPositionY)
	{
		return fieldOfViewMin + (cutterPositionStartY - cutterPositionY) / yLength * (fieldOfViewMax - fieldOfViewMin);
	}
}