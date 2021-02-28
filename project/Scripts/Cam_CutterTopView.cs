using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;

public class Cam_CutterTopView : MonoBehaviour
{
	/* Camera component */

	Camera CamCutterTopView;

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

	/* Top view markers for collision */

	GameObject MarkerAxisYCollisionTopIn;
	GameObject MarkerAxisYCollisionTopOut;
	GameObject MarkerCutterCollisionTopIn;
	GameObject MarkerCutterCollisionTopOut;

	/*  Visual observation details */

	bool PerModeTopView;
	bool hidingTopViewMarkerCutter;
	bool hidingTopViewToCut;
	bool hidingTopViewToKeep;

	/* View size */

	float fieldOfViewMin;
	float fieldOfViewMax;
	float viewSizeMax;
	float viewSizeMin;

	/********************* Template methods *********************/
	void Awake()
	{
		CamCutterTopView = gameObject.GetComponent<Camera>();

		cutter = GameObject.Find("Cutter").transform;

		Agent = GameObject.Find("Agent");
		AssistantTools = GameObject.Find("AssistantTools");

		MarkerAxisYCollisionTopIn = GameObject.Find("MarkerAxisYCollisionTopIn");
		MarkerAxisYCollisionTopOut = GameObject.Find("MarkerAxisYCollisionTopOut");
		MarkerCutterCollisionTopIn = GameObject.Find("MarkerCutterCollisionTopIn");
		MarkerCutterCollisionTopOut = GameObject.Find("MarkerCutterCollisionTopOut");
	}
	void Start()
	{
		PerModeTopView = Agent.GetComponent<AgentMT>().ProModeTopViewStr == "perspective";
		hidingTopViewMarkerCutter = Agent.GetComponent<AgentMT>().hidingTopViewMarkerCutter;
		hidingTopViewToCut = Agent.GetComponent<AgentMT>().hidingTopViewToCut;
		hidingTopViewToKeep = Agent.GetComponent<AgentMT>().hidingTopViewToKeep;
		fieldOfViewMin = Agent.GetComponent<AgentMT>().fieldOfViewMin;
		fieldOfViewMax = Agent.GetComponent<AgentMT>().fieldOfViewMax;
		viewSizeMax = Agent.GetComponent<AgentMT>().viewSizeMax;
		viewSizeMin = Agent.GetComponent<AgentMT>().viewSizeMin;

		CamCutterTopView.clearFlags = CameraClearFlags.SolidColor;
		CamCutterTopView.backgroundColor = Color.white;
		CamCutterTopView.cullingMask = (1 << LayerMask.NameToLayer("MarkerFrontTop")) +
									   (1 << LayerMask.NameToLayer("MarkerTop")) +
									   (1 << LayerMask.NameToLayer("Voxel toCut")) +
									   (1 << LayerMask.NameToLayer("Voxel toKeep"));
		if (PerModeTopView)
		{
			CamCutterTopView.cullingMask &= ~(1 << LayerMask.NameToLayer("MarkerFrontTop"));
			if (hidingTopViewToCut) CamCutterTopView.cullingMask &= ~(1 << LayerMask.NameToLayer("Voxel toCut"));
			if (hidingTopViewToKeep) CamCutterTopView.cullingMask &= ~(1 << LayerMask.NameToLayer("Voxel toKeep"));
			CamCutterTopView.orthographic = false;
			CamCutterTopView.fieldOfView = fieldOfViewMin;
		}
		else
		{
			if (hidingTopViewMarkerCutter) CamCutterTopView.cullingMask &= ~(1 << LayerMask.NameToLayer("MarkerFrontTop"));
			if (hidingTopViewToCut) CamCutterTopView.cullingMask &= ~(1 << LayerMask.NameToLayer("Voxel toCut"));
			if (hidingTopViewToKeep) CamCutterTopView.cullingMask &= ~(1 << LayerMask.NameToLayer("Voxel toKeep"));
			CamCutterTopView.orthographic = true;
			CamCutterTopView.orthographicSize = viewSizeMax;
		}		
		CamCutterTopView.nearClipPlane = 0.01f;
		CamCutterTopView.farClipPlane = 10;
		CamCutterTopView.depth = -2;

		biasY = PerModeTopView ? CamCutterTopView.nearClipPlane : AssistantTools.GetComponent<Tool_Voxelizer>().scale;
		yLength = Agent.GetComponent<AgentMT>().yLength;
		cutterPositionStartY = cutter.position.y;

		transform.localPosition = new Vector3(0, biasY, 0);
		transform.localEulerAngles = new Vector3(90, 0, 180);

		positionX = transform.position.x;
		positionZ = transform.position.z;
		positionDiffY = cutterPositionStartY - transform.position.y;

		MarkerAxisYCollisionTopIn.transform.Translate(0, biasY, 0);
		MarkerAxisYCollisionTopOut.transform.Translate(0, biasY, 0);
		MarkerCutterCollisionTopIn.transform.Translate(0, biasY, 0);
		MarkerCutterCollisionTopOut.transform.Translate(0, biasY, 0);
	}
	void Update()
	{
		transform.position = new Vector3(cutter.position.x, cutter.position.y - positionDiffY, cutter.position.z);
		if (PerModeTopView) CamCutterTopView.fieldOfView = CalculateFieldOfView(cutter.position.y);
		else CamCutterTopView.orthographicSize = CalculateOrthographicSize(cutter.position.y);
	}

	/********************* Self defined methods *********************/
	private float CalculateFieldOfView(float cutterPositionY)
	{
		return fieldOfViewMin + (cutterPositionStartY - cutterPositionY) / yLength * (fieldOfViewMax - fieldOfViewMin);
	}
	private float CalculateOrthographicSize(float cutterPositionY)
	{
		return viewSizeMax + (cutterPositionStartY - cutterPositionY) / yLength * (viewSizeMin - viewSizeMax);
	}
}