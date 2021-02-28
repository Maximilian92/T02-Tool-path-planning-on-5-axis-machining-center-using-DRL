using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coll_Axis : MonoBehaviour
{
	/* Agent */

	GameObject Agent;

	/* Variables during program running */

	internal bool collision;
	bool heuristic;

	float stepReward;
	float episodeReward;

	/********************* Template methods *********************/
	void Awake()
	{
		Agent = GameObject.Find("Agent");
	}
	void Start()
	{
		collision = false;
		heuristic = Agent.GetComponent<AgentMT>().heuristic;

		stepReward = Agent.GetComponent<AgentMT>().stepReward;
		episodeReward = Agent.GetComponent<AgentMT>().episodeReward;
	}
	void OnCollisionEnter(Collision coll)
	{
		if (!coll.gameObject.CompareTag("detector"))
		{
			collision = true;
			stepReward += Agent.GetComponent<AgentMT>().rewardCollision / 2;
			episodeReward += Agent.GetComponent<AgentMT>().rewardCollision / 2;
			Agent.GetComponent<AgentMT>().AddReward(Agent.GetComponent<AgentMT>().rewardCollision / 2);
			if (heuristic)
			{
				print(string.Format("{0} hits {1}", gameObject.name, coll.gameObject.name));
			}
		}
	}
	
	/********************* Self defined methods *********************/
	internal void _Reset()
	{
		collision = false;
	}
}