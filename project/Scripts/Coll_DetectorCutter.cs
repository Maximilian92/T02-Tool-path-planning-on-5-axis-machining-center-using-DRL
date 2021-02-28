using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Coll_DetectorCutter : MonoBehaviour
{
	/* Related gameobjects */

	GameObject Agent;

	Transform cutter;

	/* Collision marker */

	GameObject MarkerCutterCollisionFrontIn;
	GameObject MarkerCutterCollisionFrontOut;
	GameObject MarkerCutterCollisionTopIn;
	GameObject MarkerCutterCollisionTopOut;

	/* Detected collision information */

	List<GameObject> CollObjs;
	List<GameObject> CollObjsDeactive;

	List<Vector3> ContactsCollision;
	int numContactsCollision = 1;

	/* Detected toKeep information */

	List<GameObject> ToKeepVoxels;
	List<GameObject> ToKeepVoxelsDeactive;

	List<Vector3> ContactsToKeep;
	int numContactsToKeep = 1;

	/* Observations and other information */

	int vectorObsDigit;

	internal List<Vector3> obsDetectorCollContacts;
	internal Vector3 collVector;
	internal int numCollision;

	internal List<Vector3> obsDetectorToKeepContacts;
	internal Vector3 toKeepVector;
	internal int numToKeep;

	/* Rendering details */

	bool showCollMarker;

	/* Simulation details */

	int numEpisode;

	/********************* Template methods *********************/
	void Awake()
	{
		Agent = GameObject.Find("Agent");

		cutter = GameObject.Find("Cutter").transform;

		MarkerCutterCollisionFrontIn = GameObject.Find("MarkerCutterCollisionFrontIn");
		MarkerCutterCollisionFrontOut = GameObject.Find("MarkerCutterCollisionFrontOut");
		MarkerCutterCollisionTopIn = GameObject.Find("MarkerCutterCollisionTopIn");
		MarkerCutterCollisionTopOut = GameObject.Find("MarkerCutterCollisionTopOut");
	}
	void Start()
	{
		CollObjs = new List<GameObject>() { };
		CollObjsDeactive = new List<GameObject> { };
		numCollision = 0;

		ToKeepVoxels = new List<GameObject>() { };
		ToKeepVoxelsDeactive = new List<GameObject>() { };
		numToKeep = 0;

		ContactsCollision = new List<Vector3> { };
		ContactsToKeep = new List<Vector3> { };

		obsDetectorCollContacts = new List<Vector3> { };
		for (int i = 0; i < numContactsCollision; i++)
		{
			obsDetectorCollContacts.Add(Vector3.zero);
		}
		collVector = Vector3.zero;

		obsDetectorToKeepContacts = new List<Vector3> { };
		for (int i = 0; i < numContactsToKeep; i++)
		{
			obsDetectorToKeepContacts.Add(Vector3.zero);
		}
		toKeepVector = Vector3.zero;

		vectorObsDigit = Agent.GetComponent<AgentMT>().vectorObsDigit;

		showCollMarker = Agent.GetComponent<AgentMT>().showCollMarker;

		MarkerCutterCollisionFrontIn.SetActive(false);
		MarkerCutterCollisionFrontOut.SetActive(false);
		MarkerCutterCollisionTopIn.SetActive(false);
		MarkerCutterCollisionTopOut.SetActive(false);
	}
	private void OnCollisionEnter(Collision coll)
	{
		if (coll.gameObject.CompareTag("rotation"))
		{
			ContactsCollision.Add(coll.GetContact(coll.contactCount - 1).point);
			CollObjs.Add(coll.gameObject);
		}
		if (coll.gameObject.CompareTag("toKeep"))
		{
			ContactsToKeep.Add(coll.GetContact(coll.contactCount - 1).point);
			ToKeepVoxels.Add(coll.gameObject);
		}
	}
	private void OnCollisionStay(Collision coll)
	{
		if (numEpisode == Agent.GetComponent<AgentMT>().numEpisode)
		{
			if (coll.gameObject.CompareTag("rotation"))
			{
				ContactsCollision[ContactsCollision.Count - 1] = coll.GetContact(coll.contactCount - 1).point;
			}
			if (coll.gameObject.CompareTag("toKeep"))
			{
				ContactsToKeep[ContactsToKeep.Count - 1] = coll.GetContact(coll.contactCount - 1).point;
			}
		}
	}
	void OnCollisionExit(Collision coll)
	{
		if (numEpisode == Agent.GetComponent<AgentMT>().numEpisode)
		{
			if (coll.gameObject.CompareTag("rotation"))
			{
				ContactsCollision.RemoveAt(CollObjs.IndexOf(coll.gameObject));
				CollObjs.Remove(coll.gameObject);
			}
			if (coll.gameObject.CompareTag("toKeep"))
			{
				ContactsToKeep.RemoveAt(ToKeepVoxels.IndexOf(coll.gameObject));
				ToKeepVoxels.Remove(coll.gameObject);
			}
		}
	}

	/********************* Self defined methods *********************/
	internal Vector3 GetCollVector()
	{
		numEpisode = Agent.GetComponent<AgentMT>().numEpisode;

		foreach (GameObject CollObj in CollObjs)
		{
			if (!CollObj.activeSelf)
			{
				CollObjsDeactive.Add(CollObj);
			}
		}
		foreach (GameObject CollObjDeactive in CollObjsDeactive)
		{
			ContactsCollision.RemoveAt(CollObjs.IndexOf(CollObjDeactive));
			CollObjs.Remove(CollObjDeactive);
		}
		CollObjsDeactive.Clear();

		numCollision = CollObjs.Count;

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

		if (obsDetectorCollContacts[numContactsCollision - 1] != Vector3.zero)
		{
			collVector = (obsDetectorCollContacts[0] - cutter.position).normalized;
			collVector[0] = (float)Math.Round(collVector[0], vectorObsDigit);
			collVector[1] = (float)Math.Round(collVector[1], vectorObsDigit);
			collVector[2] = (float)Math.Round(collVector[2], vectorObsDigit);

			if (showCollMarker)
			{
				float collContactX = obsDetectorCollContacts[0][0];
				float collContactY = obsDetectorCollContacts[0][1];
				float collContactZ = obsDetectorCollContacts[0][2];

				MarkerCutterCollisionFrontIn.transform.position = new Vector3(collContactX, collContactY, MarkerCutterCollisionFrontIn.transform.position.z);
				MarkerCutterCollisionFrontOut.transform.position = new Vector3(collContactX, collContactY, MarkerCutterCollisionFrontOut.transform.position.z);
				MarkerCutterCollisionTopIn.transform.position = new Vector3(collContactX, MarkerCutterCollisionTopIn.transform.position.y, collContactZ);
				MarkerCutterCollisionTopOut.transform.position = new Vector3(collContactX, MarkerCutterCollisionTopOut.transform.position.y, collContactZ);
				MarkerCutterCollisionFrontIn.SetActive(true);
				MarkerCutterCollisionFrontOut.SetActive(true);
				MarkerCutterCollisionTopIn.SetActive(true);
				MarkerCutterCollisionTopOut.SetActive(true);
			}
		}
		else
		{
			collVector = Vector3.zero;

			if (showCollMarker)
			{
				MarkerCutterCollisionFrontIn.SetActive(false);
				MarkerCutterCollisionFrontOut.SetActive(false);
				MarkerCutterCollisionTopIn.SetActive(false);
				MarkerCutterCollisionTopOut.SetActive(false);
			}
		}
		
		return collVector;
	}
	internal Vector3 GetToKeepVector()
	{
		numEpisode = Agent.GetComponent<AgentMT>().numEpisode;

		foreach (GameObject ToKeepVoxel in ToKeepVoxels)
		{
			if (!ToKeepVoxel.activeSelf)
			{
				ToKeepVoxelsDeactive.Add(ToKeepVoxel);
			}
		}
		foreach (GameObject ToKeepVoxelDeactive in ToKeepVoxelsDeactive)
		{
			ContactsToKeep.RemoveAt(ToKeepVoxels.IndexOf(ToKeepVoxelDeactive));
			ToKeepVoxels.Remove(ToKeepVoxelDeactive);
		}
		ToKeepVoxelsDeactive.Clear();

		numToKeep = ToKeepVoxels.Count;

		obsDetectorToKeepContacts.Clear();
		foreach (Vector3 contact in ContactsToKeep)
		{
			obsDetectorToKeepContacts.Add(contact);
		}
		for (int i = 0; i < numContactsToKeep; i++)
		{
			obsDetectorToKeepContacts.Insert(0, Vector3.zero);
		}
		if (obsDetectorToKeepContacts.Count > numContactsToKeep)
		{
			obsDetectorToKeepContacts.RemoveRange(0, obsDetectorToKeepContacts.Count - numContactsToKeep);
		}

		if (obsDetectorToKeepContacts[numContactsToKeep - 1] != Vector3.zero)
		{
			toKeepVector = (obsDetectorToKeepContacts[0] - cutter.position).normalized;
			toKeepVector[0] = (float)Math.Round(toKeepVector[0], vectorObsDigit);
			toKeepVector[1] = (float)Math.Round(toKeepVector[1], vectorObsDigit);
			toKeepVector[2] = (float)Math.Round(toKeepVector[2], vectorObsDigit);
		}
		else
		{
			toKeepVector = Vector3.zero;
		}

		return toKeepVector;
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

		ToKeepVoxels.Clear();
		ToKeepVoxelsDeactive.Clear();
		numToKeep = 0;
		ContactsToKeep.Clear();

		obsDetectorToKeepContacts.Clear();
		for (int i = 0; i < numContactsToKeep; i++)
		{
			obsDetectorToKeepContacts.Add(Vector3.zero);
		}
		toKeepVector = Vector3.zero;

		if (showCollMarker)
		{
			MarkerCutterCollisionFrontIn.SetActive(false);
			MarkerCutterCollisionFrontOut.SetActive(false);
			MarkerCutterCollisionTopIn.SetActive(false);
			MarkerCutterCollisionTopOut.SetActive(false);
		}
	}
}