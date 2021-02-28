using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coll_DetectorAxisY1 : MonoBehaviour
{
	/* Related gameobjects */

	GameObject Agent;

	/* Detected collision information */

	List<GameObject> CollObjs;
	List<GameObject> CollObjsDeactive;

	internal List<Vector3> ContactsCollision;
	int numContactsCollision = 1;

	/* Observations and other information */

	internal List<Vector3> obsDetectorCollContacts;
	internal int numCollision = 1;

	/* Episode number */

	int numEpisode;

	/********************* Template methods *********************/
	void Awake()
	{
		Agent = GameObject.Find("Agent");
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
	internal List<Vector3> GetContacts()
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
		for (int i = 0; i < numContactsCollision; i++)
		{
			obsDetectorCollContacts.Insert(0, Vector3.zero);
		}
		if (obsDetectorCollContacts.Count > numContactsCollision)
		{
			obsDetectorCollContacts.RemoveRange(0, obsDetectorCollContacts.Count - numContactsCollision); 
		}
		
		numCollision = CollObjs.Count;
		
		return ContactsCollision;
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
	}
}