using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller_UI : MonoBehaviour
{
    /* UI elements */

    Text uiTitle1;
    Text uiRotC;
    Text uiRotB;
    Text uiTraX;
    Text uiTraZ;
    Text uiTraY;
    Text uiTitle2;
    Text uiCollVectorAxisY;
    Text uiCollVectorCutter;
    Text uiToKeepVectorCutter;
    Text uiTitle3;
    Text uiCutterPotential;
    Text uiTitle4;
    Text uiChosenAction;
    Text uiTitle5;
    Text uiRewardDiscount;
    Text uiRewardStep;
    Text uiRewardEpisode;

    Text uiNumEpisode;
    Text uiNumEpisodeStep;
    Text uiNumTotalStep;

    RenderTexture uiTopViewRenderTexture;
    RenderTexture uiFrontViewRenderTexture;

    /* Agent */

    GameObject Agent;

    /********************* Template methods *********************/
    void Awake()
    {
        uiTitle1 = GameObject.Find("UI/UITitle1").GetComponent<Text>();
        uiRotC = GameObject.Find("UI/RotC").GetComponent<Text>();
        uiRotB = GameObject.Find("UI/RotB").GetComponent<Text>();
        uiTraX = GameObject.Find("UI/TraX").GetComponent<Text>();
        uiTraZ = GameObject.Find("UI/TraZ").GetComponent<Text>();
        uiTraY = GameObject.Find("UI/TraY").GetComponent<Text>();
        uiTitle2 = GameObject.Find("UI/UITitle2").GetComponent<Text>();
        uiCollVectorAxisY = GameObject.Find("UI/CollVectorAxisY").GetComponent<Text>();
        uiCollVectorCutter = GameObject.Find("UI/CollVectorCutter").GetComponent<Text>();
        uiToKeepVectorCutter = GameObject.Find("UI/ToKeepVectorCutter").GetComponent<Text>();
        uiTitle3 = GameObject.Find("UI/UITitle3").GetComponent<Text>();
        uiCutterPotential = GameObject.Find("UI/CutterPotential").GetComponent<Text>();
        uiTitle4 = GameObject.Find("UI/UITitle4").GetComponent<Text>();
        uiChosenAction = GameObject.Find("UI/ChosenAction").GetComponent<Text>();
        uiTitle5 = GameObject.Find("UI/UITitle5").GetComponent<Text>();
        uiRewardDiscount = GameObject.Find("UI/RewardDiscount").GetComponent<Text>();
        uiRewardStep = GameObject.Find("UI/RewardStep").GetComponent<Text>();
        uiRewardEpisode = GameObject.Find("UI/RewardEpisode").GetComponent<Text>();

        uiNumEpisode = GameObject.Find("UI/NumEpisode").GetComponent<Text>();
        uiNumEpisodeStep = GameObject.Find("UI/NumEpisodeStep").GetComponent<Text>();
        uiNumTotalStep = GameObject.Find("UI/NumTotalStep").GetComponent<Text>();

        uiTopViewRenderTexture = GameObject.Find("Cam_CutterTopView").GetComponent<Camera>().targetTexture;
        uiFrontViewRenderTexture = GameObject.Find("Cam_CutterFrontView").GetComponent<Camera>().targetTexture;

        Agent = GameObject.Find("Agent");
    }
    void Start()
    {
        uiTitle1.text = "Machine Tool State";
        uiRotC.text = "RotC: ";
        uiRotB.text = "RotB: ";
        uiTraX.text = "TraX: ";
        uiTraZ.text = "TraZ: ";
        uiTraY.text = "TraY: ";
        uiTitle2.text = "Collision Information";
        uiCollVectorAxisY.text = "CollVectorAxisY: ";
        uiCollVectorCutter.text = "CollVectorCutter: ";
        uiToKeepVectorCutter.text = "ToKeepVectorCutter: ";
        uiTitle3.text = "Cutter Relative Position";
        uiCutterPotential.text = "CutterPotential: ";
        uiTitle4.text = "Action (B, C, X, Y, Z)";
        uiChosenAction.text = "ChosenAction: ";
        uiTitle5.text = "Reward";
        uiRewardDiscount.text = "RewardDiscount: ";
        uiRewardStep.text = "RewardStep: ";
        uiRewardEpisode.text = "RewardEpisode: ";

        uiNumEpisode.text = "NumEpisode: ";
        uiNumEpisodeStep.text = "NumEpisodeStep: ";
        uiNumTotalStep.text = "NumTotalStep: ";

        uiTopViewRenderTexture.width = Agent.GetComponent<AgentMT>().sensorCamWidthTopView;
        uiTopViewRenderTexture.height = Agent.GetComponent<AgentMT>().sensorCamHeightTopView;

        uiFrontViewRenderTexture.width = Agent.GetComponent<AgentMT>().sensorCamWidthFrontView;
        uiFrontViewRenderTexture.height = Agent.GetComponent<AgentMT>().sensorCamHeightFrontView;
    }
    
	/********************* Self defined methods *********************/
	internal void UpdateUI()
    {
        uiRotC.text = $"RotC: {Agent.GetComponent<AgentMT>().uiRotC}";
        uiRotB.text = $"RotB: {Agent.GetComponent<AgentMT>().uiRotB}";
        uiTraX.text = $"TraX: {Agent.GetComponent<AgentMT>().uiTraX}";
        uiTraZ.text = $"TraZ: { Agent.GetComponent<AgentMT>().uiTraZ}";
        uiTraY.text = $"TraY: { Agent.GetComponent<AgentMT>().uiTraY}";
        uiCollVectorAxisY.text = $"CollVectorAxisY: {Agent.GetComponent<AgentMT>().uiCollVectorAxisY}";
        uiCollVectorCutter.text = $"CollVectorCutter: {Agent.GetComponent<AgentMT>().uiCollVectorCutter}";
        uiToKeepVectorCutter.text = $"ToKeepVectorCutter: {Agent.GetComponent<AgentMT>().uiToKeepVectorCutter}";
        uiCutterPotential.text = $"CutterPotential: {Agent.GetComponent<AgentMT>().uiCutterPotential}";
        uiChosenAction.text = $"ChosenAction: {Agent.GetComponent<AgentMT>().uiChosenAction}";
        uiRewardDiscount.text = $"RewardDiscount: {Agent.GetComponent<AgentMT>().stepDiscount}";
        uiRewardStep.text = $"RewardStep: {Agent.GetComponent<AgentMT>().stepReward}";
        uiRewardEpisode.text = $"RewardEpisode: {Agent.GetComponent<AgentMT>().episodeReward}";

        uiNumEpisode.text = $"NumEpisode: {Agent.GetComponent<AgentMT>().numEpisode}";
        uiNumEpisodeStep.text = $"NumEpisodeStep: {Agent.GetComponent<AgentMT>().numEpisodeStep}";
        uiNumTotalStep.text = $"NumTotalStep: {Agent.GetComponent<AgentMT>().numTotalStep}";
    }
}