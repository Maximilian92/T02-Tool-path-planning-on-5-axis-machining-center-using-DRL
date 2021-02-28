using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coll_Vox : MonoBehaviour
{
    /********************* Template methods *********************/
	private void OnCollisionEnter(Collision coll)
    {
        if(coll.gameObject.CompareTag("translation"))
		{
			gameObject.SetActive(false);
		}
    }
}