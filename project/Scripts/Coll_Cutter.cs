using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coll_Cutter : MonoBehaviour
{
	/* Agent */

	GameObject Agent;
	
	/* Variables during program running */
	
	internal int rightCut;
	internal int wrongCut;

	float stepReward;
	float episodeReward;

	/********************* Template methods *********************/
	void Awake()
	{
		Agent = GameObject.Find("Agent");
	}
	void Start()
	{
		rightCut = 0;
		wrongCut = 0;
	}
	void OnCollisionEnter(Collision coll)
	{
		if (coll.gameObject.CompareTag("toCut"))
		{
			rightCut += 1;
			Agent.GetComponent<AgentMT>().stepReward += Agent.GetComponent<AgentMT>().rewardRight;
			Agent.GetComponent<AgentMT>().episodeReward += Agent.GetComponent<AgentMT>().rewardRight;
			Agent.GetComponent<AgentMT>().AddReward(Agent.GetComponent<AgentMT>().rewardRight);
		}
		if (coll.gameObject.CompareTag("toKeep"))
		{
			wrongCut += 1;
			Agent.GetComponent<AgentMT>().stepReward += Agent.GetComponent<AgentMT>().rewardWrong;
			Agent.GetComponent<AgentMT>().episodeReward += Agent.GetComponent<AgentMT>().rewardWrong;
			Agent.GetComponent<AgentMT>().AddReward(Agent.GetComponent<AgentMT>().rewardWrong);
		}
	}
	
	/********************* Self defined methods *********************/
	internal void _Reset()
	{
		rightCut = 0;
		wrongCut = 0;
	}
}