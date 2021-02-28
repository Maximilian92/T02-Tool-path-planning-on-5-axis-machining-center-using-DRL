using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Coll_DetectorAll : MonoBehaviour
{
    /* Detected collision information */

    List<GameObject> detectedVoxels;

    /********************* Template methods *********************/
    void Start()
    {
        detectedVoxels = new List<GameObject>() { };
    }
    private void OnTriggerEnter(Collider coll)
    {
        if (!coll.gameObject.CompareTag("rotation"))
        {
            coll.gameObject.GetComponent<BoxCollider>().isTrigger = false;
            detectedVoxels.Add(coll.gameObject);
        }
    }
    private void OnCollisionExit(Collision coll)
    {
        if (!coll.gameObject.CompareTag("rotation"))
        {
            coll.gameObject.GetComponent<BoxCollider>().isTrigger = true;
            detectedVoxels.Remove(coll.gameObject);
        }
    }

    /********************* Self defined methods *********************/
    internal void _Reset()
    {
        foreach (GameObject detectedVoxel in detectedVoxels)
        {
            detectedVoxel.GetComponent<BoxCollider>().isTrigger = true;
        }
        detectedVoxels = new List<GameObject>() { };
    }
}