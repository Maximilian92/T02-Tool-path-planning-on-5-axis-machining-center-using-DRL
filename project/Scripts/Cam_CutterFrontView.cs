using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;

public class Cam_CutterFrontView : MonoBehaviour
{
	/* Camera component */

	Camera CamCutterFrontView;

	/* Reference object cutter */

	Transform cutter;

	/* Agent and assistant tools */

	GameObject Agent;
	GameObject AssistantTools;

	/* Coordinate system info */

	float biasZ;
	float cutterDiameter;

	float positionX;
	float positionY;
	float positionDiffZ;

	/* Front view markers for collision */

	GameObject MarkerAxisYCollisionFrontIn;
	GameObject MarkerAxisYCollisionFrontOut;
	GameObject MarkerCutterCollisionFrontIn;
	GameObject MarkerCutterCollisionFrontOut;

	/********************* Template methods *********************/
	void Awake()
	{
		CamCutterFrontView = gameObject.GetComponent<Camera>();

		MarkerAxisYCollisionFrontIn = GameObject.Find("MarkerAxisYCollisionFrontIn");
		MarkerAxisYCollisionFrontOut = GameObject.Find("MarkerAxisYCollisionFrontOut");
		MarkerCutterCollisionFrontIn = GameObject.Find("MarkerCutterCollisionFrontIn");
		MarkerCutterCollisionFrontOut = GameObject.Find("MarkerCutterCollisionFrontOut");

		cutter = GameObject.Find("Cutter").transform;

		Agent = GameObject.Find("Agent");
		AssistantTools = GameObject.Find("AssistantTools");
	}
	void Start()
	{
		biasZ = AssistantTools.GetComponent<Tool_Voxelizer>().scale;
		cutterDiameter = Agent.GetComponent<AgentMT>().cutterDiameter;

		CamCutterFrontView.clearFlags = CameraClearFlags.SolidColor;
		CamCutterFrontView.backgroundColor = Color.black;
		CamCutterFrontView.cullingMask = (1 << LayerMask.NameToLayer("MarkerFrontTop")) + 
										 (1 << LayerMask.NameToLayer("MarkerFront")) + 
										 (1 << LayerMask.NameToLayer("Voxel toCut")) + 
										 (1 << LayerMask.NameToLayer("Voxel toKeep"));
		CamCutterFrontView.orthographic = true;
		CamCutterFrontView.orthographicSize = 2.6f;
		CamCutterFrontView.nearClipPlane = 0.01f;
		CamCutterFrontView.farClipPlane = biasZ + cutterDiameter / 2;
		CamCutterFrontView.depth = -2;

		transform.localPosition = new Vector3(0, -2, biasZ);
		transform.localEulerAngles = new Vector3(0, 180, 0);

		positionX = transform.position.x;
		positionY = transform.position.y;
		positionDiffZ = cutter.position.z - transform.position.z;

		MarkerAxisYCollisionFrontIn.transform.Translate(0, 0, biasZ);
		MarkerAxisYCollisionFrontOut.transform.Translate(0, 0, biasZ);
		MarkerCutterCollisionFrontIn.transform.Translate(0, 0, biasZ);
		MarkerCutterCollisionFrontOut.transform.Translate(0, 0, biasZ);
	}
	void Update()
	{
		transform.position = new Vector3(positionX, positionY, cutter.position.z - positionDiffZ);
	}
}