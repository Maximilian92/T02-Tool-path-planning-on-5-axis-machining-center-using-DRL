using MLAgents;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Coll_DetectorAxisY2 : MonoBehaviour
{
	/* Related gameobjects */

	GameObject Agent;
	GameObject DetectorAxisY1;

	/* Collision marker */

	GameObject MarkerAxisYCollisionFrontIn;
	GameObject MarkerAxisYCollisionFrontOut;
	GameObject MarkerAxisYCollisionTopIn;
	GameObject MarkerAxisYCollisionTopOut;

	/* Detected collision information */

	List<GameObject> CollObjs;
	List<GameObject> CollObjsDeactive;

	List<Vector3> ContactsCollision;
	int numContactsCollision = 1;

	/* Observations and other information */

	int vectorObsDigit;

	internal List<Vector3> obsDetectorCollContacts;
	internal Vector3 collVector;
	internal int numCollision;

	/* Rendering details */

	bool showCollMarker;

	/* Simulation details */

	int numEpisode;

	/********************* Template methods *********************/
	void Awake()
	{
		Agent = GameObject.Find("Agent");
		DetectorAxisY1 = GameObject.Find("DetectorAxisY1");

		MarkerAxisYCollisionFrontIn = GameObject.Find("MarkerAxisYCollisionFrontIn");
		MarkerAxisYCollisionFrontOut = GameObject.Find("MarkerAxisYCollisionFrontOut");
		MarkerAxisYCollisionTopIn = GameObject.Find("MarkerAxisYCollisionTopIn");
		MarkerAxisYCollisionTopOut = GameObject.Find("MarkerAxisYCollisionTopOut");
	}
	void Start()
	{
		CollObjs = new List<GameObject>() { };
		CollObjsDeactive = new List<GameObject> { };
		numCollision = 0;

		ContactsCollision = new List<Vector3> { };

		obsDetectorCollContacts = new List<Vector3> { };
		for (int i = 0; i < numContactsCollision; i++)
		{
			obsDetectorCollContacts.Add(Vector3.zero);
		}
		collVector = Vector3.zero;

		vectorObsDigit = Agent.GetComponent<AgentMT>().vectorObsDigit;

		showCollMarker = Agent.GetComponent<AgentMT>().showCollMarker;

		MarkerAxisYCollisionFrontIn.SetActive(false);
		MarkerAxisYCollisionFrontOut.SetActive(false);
		MarkerAxisYCollisionTopIn.SetActive(false);
		MarkerAxisYCollisionTopOut.SetActive(false);
	}
	private void OnCollisionEnter(Collision coll)
	{
		ContactsCollision.Add(coll.GetContact(coll.contactCount - 1).point);
		CollObjs.Add(coll.gameObject);
	}
	private void OnCollisionStay(Collision coll)
	{
		if (numEpisode == Agent.GetComponent<AgentMT>().numEpisode)
		{
			ContactsCollision[ContactsCollision.Count - 1] = coll.GetContact(coll.contactCount - 1).point;
		}
	}
	private void OnCollisionExit(Collision coll)
	{
		if (numEpisode == Agent.GetComponent<AgentMT>().numEpisode)
		{
			ContactsCollision.RemoveAt(CollObjs.IndexOf(coll.gameObject));
			CollObjs.Remove(coll.gameObject);
		}
	}

	/********************* Self defined methods *********************/
	internal Vector3 GetCollVector()
	{
		numEpisode = Agent.GetComponent<AgentMT>().numEpisode;

		foreach (GameObject CollObjCut in CollObjs)
		{
			if (!CollObjCut.activeSelf)
			{
				CollObjsDeactive.Add(CollObjCut);
			}
		}
		foreach (GameObject CollObjDeactive in CollObjsDeactive)
		{
			ContactsCollision.RemoveAt(CollObjs.IndexOf(CollObjDeactive));
			CollObjs.Remove(CollObjDeactive);
		}
		CollObjsDeactive.Clear();

		obsDetectorCollContacts.Clear();
		foreach (Vector3 contact in ContactsCollision)
		{
			obsDetectorCollContacts.Add(contact);
		}
		foreach (Vector3 contact in DetectorAxisY1.GetComponent<Coll_DetectorAxisY1>().GetContacts())
		{
			obsDetectorCollContacts.Add(contact);
		}
		for (int i = 0; i < numContactsCollision; i++)
		{
			obsDetectorCollContacts.Insert(0, Vector3.zero);
		}
		if (obsDetectorCollContacts.Count > numContactsCollision)
		{
			obsDetectorCollContacts.RemoveRange(0, obsDetectorCollContacts.Count - numContactsCollision);
		}
		
		numCollision = CollObjs.Count + DetectorAxisY1.GetComponent<Coll_DetectorAxisY1>().numCollision;
		
		if (obsDetectorCollContacts[numContactsCollision - 1] != Vector3.zero)
		{
			collVector = (obsDetectorCollContacts[0] - transform.position).normalized;
			collVector[0] = (float)Math.Round(collVector[0], vectorObsDigit);
			collVector[1] = (float)Math.Round(collVector[1], vectorObsDigit);
			collVector[2] = (float)Math.Round(collVector[2], vectorObsDigit);
			
			if (showCollMarker)
			{
				float collContactX = obsDetectorCollContacts[0][0];
				float collContactY = obsDetectorCollContacts[0][1];
				float collContactZ = obsDetectorCollContacts[0][2];

				MarkerAxisYCollisionFrontIn.transform.position = new Vector3(collContactX, collContactY, MarkerAxisYCollisionFrontIn.transform.position.z);
				MarkerAxisYCollisionFrontOut.transform.position = new Vector3(collContactX, collContactY, MarkerAxisYCollisionFrontOut.transform.position.z);
				MarkerAxisYCollisionTopIn.transform.position = new Vector3(collContactX, MarkerAxisYCollisionTopIn.transform.position.y, collContactZ);
				MarkerAxisYCollisionTopOut.transform.position = new Vector3(collContactX, MarkerAxisYCollisionTopOut.transform.position.y, collContactZ);
				MarkerAxisYCollisionFrontIn.SetActive(true);
				MarkerAxisYCollisionFrontOut.SetActive(true);
				MarkerAxisYCollisionTopIn.SetActive(true);
				MarkerAxisYCollisionTopOut.SetActive(true);
			} 
		}
		else
		{
			collVector = Vector2.zero;

			if (showCollMarker)
			{
				MarkerAxisYCollisionFrontIn.SetActive(false);
				MarkerAxisYCollisionFrontOut.SetActive(false);
				MarkerAxisYCollisionTopIn.SetActive(false);
				MarkerAxisYCollisionTopOut.SetActive(false);
			}
		}
		
		return collVector;
	}
	internal void _Reset()
	{
		CollObjs.Clear();
		CollObjsDeactive.Clear();
		numCollision = 0;

		ContactsCollision.Clear();

		obsDetectorCollContacts.Clear();
		for (int i = 0; i < numContactsCollision; i++)
		{
			obsDetectorCollContacts.Add(Vector3.zero);
		}
		collVector = Vector3.zero;

		if (showCollMarker)
		{
			MarkerAxisYCollisionFrontIn.SetActive(false);
			MarkerAxisYCollisionFrontOut.SetActive(false);
			MarkerAxisYCollisionTopIn.SetActive(false);
			MarkerAxisYCollisionTopOut.SetActive(false);
		}
	}
}