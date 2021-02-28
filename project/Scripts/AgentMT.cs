using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using MLAgents.Sensors;
using UnityEngine.Animations;
using Barracuda;
using MLAgents.Policies;
using UnityEngine.UIElements;
using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Random = UnityEngine.Random;
using System.Runtime.CompilerServices;
using UnityEditor;
using System.Linq;
using TMPro;

/*******************************************************************
 File description:

	This file is the brain of the agent, which integrates the whole decision chain of the agent.

	$ Class nmae: 
	  - AgentMT:Agent
	$ Member variables:
	  - ......
	$ Member methods (template):
	  - void Awake
	  - void Start
	  - public override void OnEpisodeBegin
	  - public override void CollectObservations
	  - public override void CollectDiscreteActionMasks
	  - public override void OnActionReceived
	  - public override float[] Heuristic
	$ Member methods (self defined):
	  - private void HandleAxis
	  - private void SetCameraSensor
	  - private void RenderEmissionComponent
	  - private void SetRewardSystem
	  - private void ResetMachineToolStateFixed
	  - private void ResetMachineToolStateRandom
	  - private float[] GetMachineToolState
	  - private float GetCutterPotential
	  - private float[] DecodeAction
	  - private float[] FilterAction
	  - private void ReportResult
	 
*******************************************************************/

public class AgentMT : Agent
{
	/* Agent components */

	GameObject Base;
	GameObject AxisB1;
	GameObject AxisB2;
	GameObject AxisC;
	GameObject AxisX;
	GameObject AxisY1;
	GameObject AxisY2;
	GameObject AxisZ;
	GameObject Cutter;

	Transform axisB1;
	Transform axisB2;
	Transform axisC;
	Transform axisX;
	Transform axisY1;
	Transform axisY2;
	Transform axisZ;
	Transform cutter;

	GameObject DetectorAll;
	GameObject DetectorAxisY1;
	GameObject DetectorAxisY2;
	GameObject DetectorCutter;
	
	Transform markersAxisY;

	GameObject MarkerAxisYIn;
	GameObject MarkerAxisYOut;
	GameObject MarkerAxisYCollisionFrontIn;
	GameObject MarkerAxisYCollisionFrontOut;
	GameObject MarkerAxisYCollisionTopIn;
	GameObject MarkerAxisYCollisionTopOut;

	Transform markersCutter;

	GameObject MarkerCutterIn;
	GameObject MarkerCutterOut;
	GameObject MarkerCutterCollisionFrontIn;
	GameObject MarkerCutterCollisionFrontOut;
	GameObject MarkerCutterCollisionTopIn;
	GameObject MarkerCutterCollisionTopOut;

	GameObject AssistantTools;

	/* Agent dimension infos */

	Vector3 rotAxisB1_reset;
	Vector3 rotAxisB2_reset;
	Vector3 rotAxisC_reset;

	Vector3 traAxisB1_reset;
	Vector3 traAxisB2_reset;
	Vector3 traAxisC_reset;
	Vector3 traAxisX_reset;
	Vector3 traAxisY1_reset;
	Vector3 traAxisY2_reset;
	Vector3 traAxisZ_reset;

	float bLength;
	float cLength;

	float[] xTravel;
	float[] yTravel;
	float[] zTravel;

	float[] bTravel = new float[] { -180, 180 };
	float[] cTravel = new float[] { -110, 5 };

	internal float xLength = 5f;
	internal float yLength = 4f;
	internal float zLength = 4.5f;
	internal float cutterLength = 0.5f;
	internal float cutterDiameter = 0.1f;

	/* Material infos */
	
	internal string root = "C:\\Users\\yueai\\Desktop\\";
	
	internal int voxelNumLimit = 2200;
	internal int voxelizationResolution = 20;
	internal float voxSafetyFactor = 1f;
	
	float scale;
	float yMaterialHeight;
	float ySafetyDistance;
	float lMaterialLength;

	int toKeepNum;
	int toCutNum;

	/* Visual observation (TopView) */

	enum ProjectionModeTopView
	{
		perspective,
		orthographic
	}
	ProjectionModeTopView ProModeTopView = ProjectionModeTopView.perspective;
	internal string ProModeTopViewStr;

	internal bool hidingTopViewMarkerCutter = false;
	internal bool hidingTopViewToCut = false;
	internal bool hidingTopViewToKeep = false;

	internal float fieldOfViewMin = 90;
	internal float fieldOfViewMax = 160;
	internal float viewSizeMax = 2.6f;
	internal float viewSizeMin = 0.1f;

	internal int sensorCamWidthTopView;
	internal int sensorCamHeightTopView;
	int[] cameraSensorResolutionTopView = new int[] { 84, 84 };
	
	internal int sensorCamWidthTopViewInverse;
	internal int sensorCamHeightTopViewInverse;
	int[] cameraSensorResolutionTopViewInverse = new int[] { 40, 40 };

	/* Auxiliary visual view (FrontView) */

	internal int sensorCamWidthFrontView;
	internal int sensorCamHeightFrontView;
	int[] cameraSensorResolutionFrontView = new int[] { 160, 160 };

	/* Vector observations */

	internal int vectorObsDigit = 4;

	float cutterPotential;

	float rotC;
	float rotB;
	float traX;
	float traY;
	float traZ;

	Vector3 collVectorAxisY;
	Vector3 collVectorCutter;
	Vector3 toKeepVectorCutter;

	/* UI display */
	
	GameObject Controller_UI;
	
	internal string uiCutterPotential;

	internal string uiRotC;
	internal string uiRotB;
	internal string uiTraX;
	internal string uiTraZ;
	internal string uiTraY;

	internal string uiCollVectorAxisY;
	internal string uiCollVectorCutter;
	internal string uiToKeepVectorCutter;

	internal string uiChosenAction;
	
	/* Variables for action limits */
	
	float cutterPotentialSafetyFactor = 1.2f;
	
	float cutterPotentialLimitHigh;
	float cutterPotentialLimitLow;
	
	/* Variables during program running */

	internal int numEpisode = 0;
	internal int numEpisodeStep;
	internal int numTotalStep;

	int stepRightCut;
	int stepWrongCut;
	internal float stepDiscount;
	internal float stepRewardStatic;
	internal float stepReward;

	int episodeRightCut;
	int episodeWrongCut;
	int episodeSteps;
	internal float episodeReward;

	int allSteps = 0;

	bool still;
	bool collision;
	bool end;
	bool done;
	bool restart;

	/* Step length */

	int numStepLengthIncrement = 5;

	float rStepLengthTotal = 0.5f;
	float tStepLengthTotal = 0.05f;
	
	float rStepLength;
	float tStepLength;

	/* Configurable parameters for heuristic behavior */

	float coefficientRSpeed = 10;
	float coefficientTSpeed = 10;
	float rSpeed;
	float tSpeed;

	/* Reward system */
	
	public enum RewardSystem
	{
		lowWrongCutPenalty,
		highWrongCutPenalty
	}
	
	[Space(10)]
	[Header("Reward system configuration")]
	public RewardSystem ReSys;
	
	float rewardBaseline = 100;
	float rewardDiscountBaseline;
	float rewardStill;
	internal float rewardRight;
	internal float rewardWrong;
	internal float rewardCollision;

	/* Rendering details */
	
	internal bool turnOnLightNatrual = true;
	internal bool turnOnLightWork = true;
	
	enum EnvironmentMode
	{
		normal,
		darkTorchLight,
		darkEmission1,
		darkEmission2,
		darkEmission3,
		darkLight1,
		darkLight2,
		darkLight3,
		darkLight4
	}
	EnvironmentMode EnvMode = EnvironmentMode.darkEmission1;
	internal string EnvModeStr;

	float[] renderingDetails = new float[] { 1, 0.2f, 4, 0 };
	internal float emissionIntensity;
	internal float lightIntensity;
	internal float lightRange;
	internal float transparency;

	Color colorCutterLight = Color.white;

	bool emissionAgentComponent = false;
	internal bool showCollMarker = false;

	/* Simulation details */
	
	internal bool heuristic;
	bool inference;
	
	int numMaterialReset = 0;
	int materialResetPeriode = 50000;
	
	int decisionPeriode = 5;
	int numEpisodeStepMax = 15000;

	bool randomStartPosition = true;
	
	[Space(10)]
	[Header("Restriction configuration")]
	[Tooltip("This restriction should always be set to true")]
	public bool restrictionCollision;
	[Tooltip("This restriction is set or removed when training (default mode)")]
	public bool restrictionMovingRange;
	[Tooltip("This restriction is set or removed when testing (inference mode)")]
	public bool restrictionWrongCut;
	
	/* Reports */

	string dataName = "YV0";

	/********************* Template methods *********************/
	void Awake()
	{		
		Base = GameObject.Find("Base");
		AxisB1 = GameObject.Find("AxisB1");
		AxisB2 = GameObject.Find("AxisB2");
		AxisC = GameObject.Find("AxisC");
		AxisX = GameObject.Find("AxisX");
		AxisY1 = GameObject.Find("AxisY1");
		AxisY2 = GameObject.Find("AxisY2");
		AxisZ = GameObject.Find("AxisZ");
		Cutter = GameObject.Find("Cutter");
		
		Base.layer = LayerMask.NameToLayer("Agent Entity");
		AxisB1.layer = LayerMask.NameToLayer("Agent Entity");
		AxisB2.layer = LayerMask.NameToLayer("Agent Entity");
		AxisC.layer = LayerMask.NameToLayer("Agent Entity");
		AxisX.layer = LayerMask.NameToLayer("Agent Entity");
		AxisY1.layer = LayerMask.NameToLayer("Agent Entity");
		AxisY2.layer = LayerMask.NameToLayer("Agent Entity");
		AxisZ.layer = LayerMask.NameToLayer("Agent Entity");
		Cutter.layer = LayerMask.NameToLayer("Agent Entity");
				
		AxisB1.tag = "rotation";
		AxisB2.tag = "rotation";
		AxisC.tag = "rotation";
		AxisX.tag = "translation";
		AxisY1.tag = "translation";
		AxisY2.tag = "translation";
		AxisZ.tag = "translation";
		Cutter.tag = "translation";

		axisB1 = AxisB1.transform;
		axisB2 = AxisB2.transform;
		axisC = AxisC.transform;
		axisX = AxisX.transform;
		axisY1 = AxisY1.transform;
		axisY2 = AxisY2.transform;
		axisZ = AxisZ.transform;
		cutter = Cutter.transform;

		HandleAxis(AxisB2);
		HandleAxis(AxisC);
		HandleAxis(AxisY1);
		HandleAxis(AxisY2);
		HandleAxis(Cutter);

		sensorCamWidthTopView = cameraSensorResolutionTopView[0];
		sensorCamHeightTopView = cameraSensorResolutionTopView[1];
		sensorCamWidthTopViewInverse = cameraSensorResolutionTopViewInverse[0];
		sensorCamHeightTopViewInverse = cameraSensorResolutionTopViewInverse[1];
		sensorCamWidthFrontView = cameraSensorResolutionFrontView[0];
		sensorCamHeightFrontView = cameraSensorResolutionFrontView[1];
		SetCameraSensor();

		DetectorAll = GameObject.Find("DetectorAll");
		DetectorAxisY1 = GameObject.Find("DetectorAxisY1");
		DetectorAxisY2 = GameObject.Find("DetectorAxisY2");
		DetectorCutter = GameObject.Find("DetectorCutter");

		DetectorAxisY1.layer = LayerMask.NameToLayer("Detector");
		DetectorAxisY2.layer = LayerMask.NameToLayer("Detector");
		DetectorCutter.layer = LayerMask.NameToLayer("Detector");

		markersAxisY = GameObject.Find("MarkersAxisY").transform;
		
		MarkerAxisYIn = GameObject.Find("MarkerAxisYIn");
		MarkerAxisYOut = GameObject.Find("MarkerAxisYOut");
		MarkerAxisYCollisionFrontIn = GameObject.Find("MarkerAxisYCollisionFrontIn");
		MarkerAxisYCollisionFrontOut = GameObject.Find("MarkerAxisYCollisionFrontOut");
		MarkerAxisYCollisionTopIn = GameObject.Find("MarkerAxisYCollisionTopIn");
		MarkerAxisYCollisionTopOut = GameObject.Find("MarkerAxisYCollisionTopOut");

		MarkerAxisYIn.layer = LayerMask.NameToLayer("MarkerFront");
		MarkerAxisYOut.layer = LayerMask.NameToLayer("MarkerFront");
		MarkerAxisYCollisionFrontIn.layer = LayerMask.NameToLayer("MarkerFront");
		MarkerAxisYCollisionFrontOut.layer = LayerMask.NameToLayer("MarkerFront");
		MarkerAxisYCollisionTopIn.layer = LayerMask.NameToLayer("MarkerTop");
		MarkerAxisYCollisionTopOut.layer = LayerMask.NameToLayer("MarkerTop");

		markersCutter = GameObject.Find("MarkersCutter").transform;

		MarkerCutterIn = GameObject.Find("MarkerCutterIn");
		MarkerCutterOut = GameObject.Find("MarkerCutterOut");
		MarkerCutterCollisionFrontIn = GameObject.Find("MarkerCutterCollisionFrontIn");
		MarkerCutterCollisionFrontOut = GameObject.Find("MarkerCutterCollisionFrontOut");
		MarkerCutterCollisionTopIn = GameObject.Find("MarkerCutterCollisionTopIn");
		MarkerCutterCollisionTopOut = GameObject.Find("MarkerCutterCollisionTopOut");
		
		MarkerCutterIn.layer = LayerMask.NameToLayer("MarkerFrontTop");
		MarkerCutterOut.layer = LayerMask.NameToLayer("MarkerFrontTop");
		MarkerCutterCollisionFrontIn.layer = LayerMask.NameToLayer("MarkerFront");
		MarkerCutterCollisionFrontOut.layer = LayerMask.NameToLayer("MarkerFront");
		MarkerCutterCollisionTopIn.layer = LayerMask.NameToLayer("MarkerTop");
		MarkerCutterCollisionTopOut.layer = LayerMask.NameToLayer("MarkerTop");

		foreach (Transform marker in markersAxisY)
		{
			RenderEmissionComponent(marker.gameObject);
		}
		foreach (Transform marker in markersCutter)
		{
			RenderEmissionComponent(marker.gameObject);
		}

		AssistantTools = GameObject.Find("AssistantTools");

		EnvModeStr = EnvMode.ToString();

		ProModeTopViewStr = ProModeTopView.ToString();

		emissionIntensity = renderingDetails[0];
		lightIntensity = renderingDetails[1];
		lightRange = renderingDetails[2];
		transparency = renderingDetails[3];

		if (emissionAgentComponent)
		{
			RenderEmissionComponent(Base);
			RenderEmissionComponent(AxisB1);
			RenderEmissionComponent(AxisB2);
			RenderEmissionComponent(AxisC);
			RenderEmissionComponent(AxisX);
			RenderEmissionComponent(AxisY1);
			RenderEmissionComponent(AxisY2);
			RenderEmissionComponent(AxisZ);
		}
		
		Controller_UI = GameObject.Find("Controller_UI");
		
		heuristic = gameObject.GetComponent<BehaviorParameters>().behaviorType == BehaviorType.HeuristicOnly;
		inference = gameObject.GetComponent<BehaviorParameters>().behaviorType == BehaviorType.InferenceOnly;
		
		this.maxStep = numEpisodeStepMax;
	}
	void Start()
	{
		rotAxisB1_reset = axisB1.localEulerAngles;
		rotAxisB2_reset = axisB2.localEulerAngles;
		rotAxisC_reset = axisC.localEulerAngles;

		traAxisB1_reset = axisB1.localPosition;
		traAxisB2_reset = axisB2.localPosition;
		traAxisC_reset = axisC.localPosition;
		traAxisX_reset = axisX.localPosition;
		traAxisY1_reset = axisY1.localPosition;
		traAxisY2_reset = axisY2.localPosition;
		traAxisZ_reset = axisZ.localPosition;
		
		bLength = bTravel[1] - bTravel[0];
		cLength = cTravel[1] - cTravel[0];
		
		xTravel = new float[] { traAxisX_reset[0] - xLength / 2, traAxisX_reset[0] + xLength / 2 };
		yTravel = new float[] { traAxisY1_reset[1] - yLength, traAxisY1_reset[1] };
		zTravel = new float[] { traAxisZ_reset[2] - zLength / 2, traAxisZ_reset[2] + zLength / 2 };
		
		scale = AssistantTools.GetComponent<Tool_Voxelizer>().scale;
		yMaterialHeight = AssistantTools.GetComponent<Tool_Voxelizer>().yMaterialHeight;
		ySafetyDistance = yMaterialHeight + cutterLength;
		lMaterialLength = AssistantTools.GetComponent<Tool_Voxelizer>().lMaterialLength;

		toKeepNum = AssistantTools.GetComponent<Tool_Voxelizer>().toKeepNum;
		toCutNum = AssistantTools.GetComponent<Tool_Voxelizer>().toCutNum;
		
		cutterPotentialLimitHigh = (float)Math.Round(1 - lMaterialLength / (2 * yLength * (float)Math.Tan(Math.PI * fieldOfViewMin / 360)), vectorObsDigit);
		cutterPotentialLimitLow = (float)Math.Round(tStepLengthTotal * cutterPotentialSafetyFactor / yLength - 1, vectorObsDigit);

		rStepLength = rStepLengthTotal / decisionPeriode;
		tStepLength = tStepLengthTotal / decisionPeriode;
		
		rSpeed = coefficientRSpeed * decisionPeriode;
		tSpeed = coefficientTSpeed * decisionPeriode;
		
		SetRewardSystem();

		StreamWriter sw = new StreamWriter($"{root}{dataName}.txt", false);
		sw.WriteLine(dataName);
		sw.Close();
	}
	public override void OnEpisodeBegin()
	{
		numEpisode++;
		restart = false;

		DetectorAll.GetComponent<Coll_DetectorAll>()._Reset();

		if (numMaterialReset != numTotalStep / materialResetPeriode)
		{
			numMaterialReset = numTotalStep / materialResetPeriode;
			ResetMachineToolStateFixed();
			AssistantTools.GetComponent<Tool_Voxelizer>().SettleVoxels();
			AssistantTools.GetComponent<Tool_VoxelsRenderer>().RenderVoxels();
			print("--------------------- Material Reset ---------------------");
		}
		
		// Mechanical reset order: c -> b1 -> b2, x -> z -> y1 -> y2 (-> cutter)
		if (randomStartPosition) ResetMachineToolStateRandom();
		else ResetMachineToolStateFixed();

		cutterPotential = GetCutterPotential();

		AssistantTools.GetComponent<Tool_Voxelizer>()._Reset();
		DetectorAxisY1.GetComponent<Coll_DetectorAxisY1>()._Reset();
		DetectorAxisY2.GetComponent<Coll_DetectorAxisY2>()._Reset();
		DetectorCutter.GetComponent<Coll_DetectorCutter>()._Reset();

		episodeRightCut = 0;
		episodeWrongCut = 0;
		episodeSteps = 0;
		episodeReward = 0;
	}
	public override void CollectObservations(VectorSensor vectorSensor)
	{		
		float[] machineToolState = GetMachineToolState();

		rotC = machineToolState[0];
		rotB = machineToolState[1];
		traX = machineToolState[2];
		traZ = machineToolState[3];
		traY = machineToolState[4];

		vectorSensor.AddObservation(rotC);
		vectorSensor.AddObservation(rotB);

		uiRotC = rotC.ToString();
		uiRotB = rotB.ToString();
		uiTraX = traX.ToString();
		uiTraZ = traZ.ToString();
		uiTraY = traY.ToString();

		collVectorAxisY = DetectorAxisY2.GetComponent<Coll_DetectorAxisY2>().GetCollVector();
		collVectorCutter = DetectorCutter.GetComponent<Coll_DetectorCutter>().GetCollVector();
		toKeepVectorCutter = DetectorCutter.GetComponent<Coll_DetectorCutter>().GetToKeepVector();

		uiCollVectorAxisY = $"({collVectorAxisY[0]}, {collVectorAxisY[1]} ,{collVectorAxisY[2]})";
		uiCollVectorCutter = $"({collVectorCutter[0]}, {collVectorCutter[1]} , {collVectorCutter[2]})";
		uiToKeepVectorCutter = $"({toKeepVectorCutter[0]}, {toKeepVectorCutter[1]}, {toKeepVectorCutter[2]})";

		vectorSensor.AddObservation(cutterPotential);
		uiCutterPotential = cutterPotential.ToString();
	}
	public override void CollectDiscreteActionMasks(DiscreteActionMasker actionMasker)
	{
		// action order: b, c, x, y, z
		if (restrictionCollision && !(collVectorAxisY == Vector3.zero && collVectorCutter == Vector3.zero))
		{
			actionMasker.SetMask(0, new int[2] { 0, 2 });
			actionMasker.SetMask(1, new int[2] { 0, 2 });
			actionMasker.SetMask(2, new int[2] { 0, 2 });
			actionMasker.SetMask(3, new int[2] { 0, 1 });
			actionMasker.SetMask(4, new int[2] { 0, 2 });
		}
		else if (restrictionMovingRange)
		{
			if (cutterPotential == 0)
			{
				if (toKeepVectorCutter == Vector3.zero) 
				{
					if (traX > 0.5f) actionMasker.SetMask(2, new int[2] { 1, 2 });
					if (traX < 0.5f) actionMasker.SetMask(2, new int[2] { 0, 1 });
					if (traZ > 0.5f) actionMasker.SetMask(4, new int[2] { 1, 2 });
					if (traZ < 0.5f) actionMasker.SetMask(4, new int[2] { 0, 1 });
					actionMasker.SetMask(3, new int[2] { 0, 2 });
					
					actionMasker.SetMask(7, new int[4] { 0, 1, 2, 3 });
					actionMasker.SetMask(9, new int[4] { 0, 1, 2, 3 });
				}
				else
				{
					actionMasker.SetMask(0, new int[2] { 0, 2 });
					actionMasker.SetMask(1, new int[2] { 0, 2 });
					actionMasker.SetMask(2, new int[2] { 0, 2 });
					actionMasker.SetMask(3, new int[2] { 0, 1 });
					actionMasker.SetMask(4, new int[2] { 0, 2 });
				}
			}
			else if (Math.Abs(cutterPotential) <= cutterPotentialLimitHigh)
			{
				actionMasker.SetMask(3, new int[2] { 1, 2 });
				
				actionMasker.SetMask(8, new int[4] { 0, 1, 2, 3 });
			}
		}
	}
	public override void OnActionReceived(float[] actionRaw)
	{
		// Result of last action
		
		if (!still)
		{
			collision = AxisB2.GetComponent<Coll_Axis>().collision ||
						AxisC.GetComponent<Coll_Axis>().collision ||
						AxisY1.GetComponent<Coll_Axis>().collision ||
						AxisY2.GetComponent<Coll_Axis>().collision;
			AxisB2.GetComponent<Coll_Axis>()._Reset();
			AxisC.GetComponent<Coll_Axis>()._Reset();
			AxisY1.GetComponent<Coll_Axis>()._Reset();
			AxisY2.GetComponent<Coll_Axis>()._Reset();

			stepRightCut = Cutter.GetComponent<Coll_Cutter>().rightCut;
			stepWrongCut = Cutter.GetComponent<Coll_Cutter>().wrongCut;
			Cutter.GetComponent<Coll_Cutter>()._Reset();

			episodeRightCut += stepRightCut;
			episodeWrongCut += stepWrongCut;
		}
		
		allSteps++;
		numTotalStep = allSteps / decisionPeriode;

		episodeSteps ++;
		numEpisodeStep = episodeSteps / decisionPeriode;
		
		Controller_UI.GetComponent<Controller_UI>().UpdateUI();

		done = episodeRightCut == toCutNum;
		end = episodeSteps == maxStep;

		if (collision || done || end || restart)
		{
			ReportResult();
			EndEpisode();
		}
		
		// Begin of current action
		
		else
		{
			float[] action = new float[actionRaw.Length];
			for (int i = 0; i < action.Length; i++)
			{
				action[i] = actionRaw[i];
			}
			
			if (heuristic)
			{
				action = FilterAction(action, DetectorAxisY2.GetComponent<Coll_DetectorAxisY2>().collVector, DetectorCutter.GetComponent<Coll_DetectorCutter>().collVector);
			}
			else
			{
				action = DecodeAction(action);
			}

			float aB = action[0] * action[5] / numStepLengthIncrement;
			float aC = action[1] * action[6] / numStepLengthIncrement;
			float aX = action[2] * action[7] / numStepLengthIncrement;
			float aY = action[3] * action[8] / numStepLengthIncrement;
			float aZ = action[4] * action[9] / numStepLengthIncrement;
			
			if (aB == 0 && aC == 0 && aX == 0 && aY == 0 && aZ == 0)
			{
				still = true;
				collision = false;
				stepRightCut = 0;
				stepWrongCut = 0;
			}
			else
			{
				still = false;
				
				var new_trans = new float[] { axisB1.localEulerAngles.y + aB * rStepLength,
											  axisC.localEulerAngles.z + aC * rStepLength,
											  axisX.localPosition.x + aX * tStepLength,
											  axisY1.localPosition.y + aY * tStepLength,
											  axisZ.localPosition.z + aZ * tStepLength
											};

				if (new_trans[0] > 180) new_trans[0] -= 360;
				if (new_trans[1] > 180) new_trans[1] -= 360;
				
				bool move_b = true;
				bool move_c = new_trans[1] >= cTravel[0] && new_trans[1] <= cTravel[1];
				bool move_x = new_trans[2] >= xTravel[0] && new_trans[2] <= xTravel[1];
				bool move_y = new_trans[3] >= yTravel[0] && new_trans[3] <= yTravel[1];
				bool move_z = new_trans[4] >= zTravel[0] && new_trans[4] <= zTravel[1];
				
				if (!move_b) aB = 0;
				if (!move_c) aC = 0;
				if (!move_x) aX = 0;
				if (!move_y) aY = 0;
				if (!move_z) aZ = 0;
				
				if (inference && restrictionWrongCut && toKeepVectorCutter != Vector3.zero)
				{
					aB = 0;
					aC = 0;
					Vector3 sweepDirection = new Vector3(aX, aY, aZ).normalized;
					float sweepDistance = (float)Math.Sqrt(aX * aX + aY * aY + aZ * aZ) * tStepLength;
					RaycastHit[] sweepDetections = Cutter.GetComponent<Rigidbody>().SweepTestAll(sweepDirection, sweepDistance);
					foreach (RaycastHit sweepDetection in sweepDetections)
					{
						if (sweepDetection.collider.CompareTag("toKeep"))
						{
							aX = 0;
							aY = 0.2f;
							aZ = 0;
							break;
						}
					}
				}
				
				// Mechanical travel order: c -> b1, x -> z -> y1 (-> cutter)
				axisC.Rotate(new Vector3(0, 0, aC * rStepLength));
				axisB1.Rotate(new Vector3(0, aB * rStepLength, 0));
				axisX.Translate(aX * tStepLength, 0, 0);
				axisZ.Translate(0, 0, aZ * tStepLength);
				axisY1.Translate(0, aY * tStepLength, 0);

				cutterPotential = GetCutterPotential();
			}
			
			uiChosenAction = $"({aB}, {aC}, {aX}, {aY}, {aZ})";
			
			if (Math.Abs(cutterPotential) > cutterPotentialLimitHigh) stepDiscount = 0.5f;
			else stepDiscount = 2;
			
			stepRewardStatic = rewardStill * (still == true ? 1 : 0) + rewardDiscountBaseline * stepDiscount;
			AddReward(stepRewardStatic);
			
			stepReward = stepRewardStatic;
			episodeReward += stepRewardStatic;
		}
	}
	public override float[] Heuristic()
	{	
		var action = new float[10];

		if (Input.GetKey(KeyCode.N)) restart = true;

		if (Input.GetKey(KeyCode.J)) action[0] = Time.deltaTime * rSpeed;
		if (Input.GetKey(KeyCode.L)) action[0] = -1 * Time.deltaTime * rSpeed;
		if (Input.GetKey(KeyCode.I)) action[1] = Time.deltaTime * rSpeed;
		if (Input.GetKey(KeyCode.K)) action[1] = -1 * Time.deltaTime * rSpeed;
		if (Input.GetKey(KeyCode.A)) action[2] = Time.deltaTime * tSpeed;
		if (Input.GetKey(KeyCode.D)) action[2] = -1 * Time.deltaTime * tSpeed;
		if (Input.GetKey(KeyCode.E)) action[3] = Time.deltaTime * tSpeed;
		if (Input.GetKey(KeyCode.Q)) action[3] = -1 * Time.deltaTime * tSpeed;
		if (Input.GetKey(KeyCode.S)) action[4] = Time.deltaTime * tSpeed;
		if (Input.GetKey(KeyCode.W)) action[4] = -1 * Time.deltaTime * tSpeed;

		action[5] = numStepLengthIncrement;
		action[6] = numStepLengthIncrement;
		action[7] = numStepLengthIncrement;
		action[8] = numStepLengthIncrement;
		action[9] = numStepLengthIncrement;
		
		return action;
	}
	
	/********************* Self defined methods *********************/
	private void HandleAxis(GameObject Axis)
	{
		Axis.AddComponent<Rigidbody>();
		Axis.GetComponent<Rigidbody>().useGravity = false;
		Axis.AddComponent<MeshCollider>();
		Axis.GetComponent<MeshCollider>().convex = true;
		if (Axis.CompareTag("rotation"))
		{
			Axis.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		}
		if (Axis.CompareTag("translation"))
		{
			Axis.GetComponent<Rigidbody>().isKinematic = true;
		}
		if (Axis.name != "Cutter")
		{
			Axis.AddComponent<Coll_Axis>();
		}
		if (Axis.name == "Cutter")
		{
			Axis.AddComponent<Coll_Cutter>();
			GameObject.Find("Cam_CutterFrontView").AddComponent<Cam_CutterFrontView>();
			GameObject.Find("Cam_CutterTopView").AddComponent<Cam_CutterTopView>();
			GameObject.Find("Cam_CutterTopViewInverse").AddComponent<Cam_CutterTopViewInverse>();
			if (EnvMode == EnvironmentMode.darkTorchLight)
			{
				Light cutterLight = Axis.AddComponent<Light>();
				cutterLight.range = 100;
				cutterLight.color = colorCutterLight;
			}
		}
	}
	private void SetCameraSensor()
	{
		CameraSensorComponent sensorCamTopView = Cutter.AddComponent<CameraSensorComponent>();
		sensorCamTopView.camera = GameObject.Find("Cam_CutterTopView").GetComponent<Camera>();
		sensorCamTopView.sensorName = "CameraTopView";
		sensorCamTopView.width = sensorCamWidthTopView;
		sensorCamTopView.height = sensorCamHeightTopView;
		sensorCamTopView.grayscale = false;
		sensorCamTopView.compression = SensorCompressionType.PNG;
		
		CameraSensorComponent sensorCamTopViewInverse = Cutter.AddComponent<CameraSensorComponent>();
		sensorCamTopViewInverse.camera = GameObject.Find("Cam_CutterTopViewInverse").GetComponent<Camera>();
		sensorCamTopViewInverse.sensorName = "CameraTopViewInverse";
		sensorCamTopViewInverse.width = sensorCamWidthTopViewInverse;
		sensorCamTopViewInverse.height = sensorCamHeightTopViewInverse;
		sensorCamTopViewInverse.grayscale = false;
		sensorCamTopViewInverse.compression = SensorCompressionType.PNG;
	}
	private void RenderEmissionComponent(GameObject Component)
	{
		Color colorEmission = Component.GetComponent<Renderer>().material.color * Mathf.Pow(2f, emissionIntensity - (0.4169f));
		Component.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", colorEmission);
		Component.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
	}
	private void SetRewardSystem()
	{
		if (ReSys == RewardSystem.lowWrongCutPenalty)
		{
			rewardRight = rewardBaseline / toCutNum;
			rewardWrong = -rewardBaseline / toKeepNum;
			rewardCollision = rewardWrong * 20f;
			rewardStill = rewardWrong * 2f;
			rewardDiscountBaseline = -rewardBaseline / numEpisodeStepMax;
		}
		else if (ReSys == RewardSystem.highWrongCutPenalty)
		{
			rewardRight = rewardBaseline / toCutNum;
			rewardWrong = rewardRight * (-5f);
			rewardCollision = rewardWrong * 20f;
			rewardStill = rewardWrong * 2f;
			rewardDiscountBaseline = -rewardBaseline / numEpisodeStepMax;
		}
	}
	private void ResetMachineToolStateFixed()
	{
		axisC.localEulerAngles = rotAxisC_reset;
		axisC.localPosition = traAxisC_reset;
		axisB1.localEulerAngles =rotAxisB1_reset;
		axisB1.localPosition = traAxisB1_reset;
		axisB2.localEulerAngles = rotAxisB2_reset;
		axisB2.localPosition = traAxisB2_reset;
		axisX.localPosition = traAxisX_reset;
		axisZ.localPosition = traAxisZ_reset;
		axisY1.localPosition = traAxisY1_reset;
		axisY2.localPosition = traAxisY2_reset;
	}
	private void ResetMachineToolStateRandom()
	{
		axisC.localEulerAngles = rotAxisC_reset;
		axisC.localPosition = traAxisC_reset;
		axisB1.localEulerAngles = new Vector3(rotAxisB1_reset[0], Random.Range(bTravel[0], bTravel[1]), rotAxisB1_reset[2]);
		axisB1.localPosition = traAxisB1_reset;
		axisB2.localEulerAngles = rotAxisB2_reset;
		axisB2.localPosition = traAxisB2_reset;
		axisX.localPosition = new Vector3(Random.Range(xTravel[0], xTravel[1]), traAxisX_reset[1], traAxisX_reset[2]);
		axisZ.localPosition = new Vector3(traAxisZ_reset[0], traAxisZ_reset[1], Random.Range(zTravel[0], zTravel[1]));
		axisY1.localPosition = new Vector3(traAxisY1_reset[0], Random.Range(yTravel[0] + ySafetyDistance, yTravel[1]), traAxisY1_reset[2]);
		axisY2.localPosition = traAxisY2_reset;
	}
	private float[] GetMachineToolState()
	{
		float rotC;
		float rotB;
		float traX;
		float traZ;
		float traY;

		rotC = (float)Math.Round((axisC.localEulerAngles.z - cTravel[0]) / cLength, vectorObsDigit);
		if (axisC.localEulerAngles.z > 180)
		{
			rotC = (float)Math.Round((axisC.localEulerAngles.z + cLength - 360) / cLength, vectorObsDigit);
		}
		rotB = (float)Math.Round(axisB1.localEulerAngles.y / bLength, vectorObsDigit);
		if (axisB1.localEulerAngles.y < 0)
		{
			rotB = (float)Math.Round(axisB1.localEulerAngles.y + 360 / bLength, vectorObsDigit);
		}
		traX = (float)Math.Round((axisX.localPosition.x - xTravel[0]) / xLength, vectorObsDigit);
		traZ = (float)Math.Round((axisZ.localPosition.z - zTravel[0]) / zLength, vectorObsDigit);
		traY = (float)Math.Round((axisY1.localPosition.y - yTravel[0]) / yLength, vectorObsDigit);

		return new float[] { rotC, rotB, traX, traZ, traY };
	}
	private float GetCutterPotential()
	{
		RaycastHit singleRaycast = new RaycastHit();
		cutter.GetComponent<Rigidbody>().SweepTest(new Vector3(0, -1, 0), out singleRaycast, 10);
		
		if (singleRaycast.collider != null)
		{
			float sweepDistance = singleRaycast.distance + scale / 2;
			RaycastHit[] sweepRaycasts = cutter.GetComponent<Rigidbody>().SweepTestAll(new Vector3(0, -1, 0), sweepDistance);
			int sweepNum = sweepRaycasts.Length;

			int[] sweepTypes = new int[sweepNum];
			float[] sweepDistances = new float[sweepNum];

			float cutterAltitudeSum = 0;
			int sweepNumEffective = 0;

			for (int i = 0; i < sweepNum; i++)
			{
				if (sweepRaycasts[i].collider.CompareTag("toKeep")) sweepTypes[i] = -1;
				else if (sweepRaycasts[i].collider.CompareTag("toCut")) sweepTypes[i] = 1;
				else sweepTypes[i] = 0;
				sweepDistances[i] = sweepRaycasts[i].distance;

				cutterAltitudeSum += Math.Abs(sweepTypes[i]) * sweepDistances[i] / yLength;
				sweepNumEffective += Math.Abs(sweepTypes[i]);
			}

			if (sweepTypes.Contains(-1)) return (float)Math.Round(cutterAltitudeSum / sweepNumEffective - 1, vectorObsDigit);
			else if (sweepTypes.Contains(1)) return (float)Math.Round(1 - cutterAltitudeSum / sweepNumEffective, vectorObsDigit);
			else return 0;
		}
		else
		{
			return 0;
		}
	}
	private float[] DecodeAction(float[] action)
	{
		// direction-action map: 0 -> -1, 1 -> 0, 2 -> 1
		for (int i = 0; i < action.Length / 2; i++)
		{
			action[i] -= 1;
		}
		// increment-action map: 0 -> 1, 1 -> 2, 2 -> 3, 3 -> 4, 4 -> 5
		for (int i = action.Length / 2; i < action.Length; i++)
		{
			action[i] += 1;
		}
		
		return action;		
	}
	private float[] FilterAction(float[] action, Vector3 collVectorAxisY, Vector3 collVectorCutter)
	{
		// action order: b, c, x, y, z
		if (restrictionCollision && !(collVectorAxisY == Vector3.zero && collVectorCutter == Vector3.zero))
		{
			action[0] = 0;
			action[1] = 0;
			action[2] = 0;
			if (action[3] < 0) action[3] = 0;
			action[4] = 0;
		}
		else if (restrictionMovingRange)
		{
			if (cutterPotential == 0)
			{
				if (toKeepVectorCutter == Vector3.zero)
				{
					if ((traX - 0.5f) * action[2] > 0) action[2] = 0;
					if ((traZ - 0.5f) * action[4] > 0) action[4] = 0;
					action[3] = 0;
				}
				else
				{
					action[0] = 0;
					action[1] = 0;
					action[2] = 0;
					if (action[3] < 0) action[3] = 0;
					action[4] = 0;
				}
			}
			else if (Math.Abs(cutterPotential) <= cutterPotentialLimitHigh && action[3] > 0) 
			{
				action[3] = 0;
			}
		}
		return action;
	}
	private void ReportResult()
	{
		float rightRateSW = (float)Math.Round(episodeRightCut * 1f / toCutNum, 4);
		float wrongRateSW = (float)Math.Round(episodeWrongCut * 1f / toKeepNum, 4);
		StreamWriter sw = new StreamWriter(root + dataName + ".txt", true);
		sw.WriteLine((rightRateSW == 0 || rightRateSW == 1 ? rightRateSW.ToString() + ".0000" : rightRateSW.ToString().PadRight(6, '0')) + ", " +
					 (wrongRateSW == 0 || wrongRateSW == 1 ? wrongRateSW.ToString() + ".0000" : wrongRateSW.ToString().PadRight(6, '0')) + (collision ? ", coll" : ""));
		sw.Close();

		float rightRatePrint = Mathf.Round(rightRateSW * 100);
		float wrongRatePrint = Mathf.Round(wrongRateSW * 100);
		print(string.Format("Collision: {0}, Episode steps: {1} Episode reward: {2}, Right: {3}%, Wrong: {4}% ",
							collision, episodeSteps, episodeReward, rightRatePrint, wrongRatePrint));
	}
}