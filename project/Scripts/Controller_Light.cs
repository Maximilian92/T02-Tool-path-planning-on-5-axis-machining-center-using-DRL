using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Light : MonoBehaviour
{
    /* Agent */

    GameObject Agent;

    /* Light objects */

    GameObject ObjectLightNatrual;
    GameObject ObjectLightWork;

    /* Lights */

    Light LightNatrual;
    Light LightWork;

    /********************* Template methods *********************/
    void Awake()
    {
        Agent = GameObject.Find("Agent");

        ObjectLightNatrual = GameObject.Find("LightNatrual");
        ObjectLightWork = GameObject.Find("LightWork");

        LightNatrual = ObjectLightNatrual.GetComponent<Light>();
        LightWork = ObjectLightWork.GetComponent<Light>();
    }
    void Start()
    {
        LightNatrual.type = LightType.Directional;
        LightNatrual.color = Color.white;
		LightNatrual.intensity = 1;
        LightNatrual.shadows = LightShadows.Soft;
        LightNatrual.cullingMask &= ~(1 << LayerMask.NameToLayer("Voxel toCut"));
        LightNatrual.cullingMask &= ~(1 << LayerMask.NameToLayer("Voxel toKeep"));

        LightWork.type = LightType.Directional;
        LightWork.color = Color.white;
		LightWork.intensity = 0.4f;
        LightWork.shadows = LightShadows.None;
        LightWork.cullingMask = (1 << LayerMask.NameToLayer("Voxel toCut")) + (1 << LayerMask.NameToLayer("Voxel toKeep"));

        ObjectLightNatrual.transform.localEulerAngles = new Vector3(150, 0, 0);
        ObjectLightWork.transform.localEulerAngles = new Vector3(90, 0, 0);

        ObjectLightNatrual.SetActive(Agent.GetComponent<AgentMT>().EnvModeStr == "normal" || Agent.GetComponent<AgentMT>().turnOnLightNatrual);
        ObjectLightWork.SetActive(Agent.GetComponent<AgentMT>().turnOnLightWork);
    }
}