using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Linq;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEditor;
using TMPro;
using UnityEngine.Rendering;
using System;
using System.ComponentModel;

public class Tool_Voxelizer : MonoBehaviour
{
    /* Agent */
    
    GameObject Agent;

    /* Prefabs and models */

    public GameObject PrefabMaterial;
    public GameObject PrefabProduct;

    GameObject ModelMaterial;
    GameObject ModelProduct;

    /* Collection of voxels */

    GameObject[] VoxelsToKeep;
    GameObject[] VoxelsToCut;
    GameObject[] VoxelsToDetermine;

    Transform voxelsToCut;
    Transform voxelsToKeep;

    /* Voxelization parameters */

    float sizeX;
    float sizeY;
    float sizeZ;

    int voxelizationResolution;
    float voxSafetyFactor;

    internal float l_max;
    internal float scale;
    Vector3 correctPosition;

    internal float yMaterialHeight;
    internal float lMaterialLength;

    Vector3 voxelizeStart;

    /* Number of voxels */

    int numVoxelsX;
    int numVoxelsY;
    int numVoxelsZ;

    internal int toKeepNum;
    internal int toCutNum;

    /********************* Template methods *********************/
    void Awake()
    {
        Agent = GameObject.Find("Agent");

        ModelMaterial = Instantiate(PrefabMaterial, Vector3.zero, Quaternion.identity);
        ModelProduct = Instantiate(PrefabProduct, Vector3.zero, Quaternion.identity);

        ModelMaterial.layer = LayerMask.NameToLayer("Model material");
        ModelProduct.layer = LayerMask.NameToLayer("Model product");

        voxelsToKeep = GameObject.Find("VoxelsToKeep").transform;
        voxelsToCut = GameObject.Find("VoxelsToCut").transform;

        sizeX = ModelMaterial.GetComponent<Renderer>().bounds.size.x;
        sizeY = ModelMaterial.GetComponent<Renderer>().bounds.size.y;
        sizeZ = ModelMaterial.GetComponent<Renderer>().bounds.size.z;

        voxelizationResolution = Agent.GetComponent<AgentMT>().voxelizationResolution;
        voxSafetyFactor = Agent.GetComponent<AgentMT>().voxSafetyFactor;

        l_max = new float[] { sizeX, sizeY, sizeZ }.Max();
        scale = l_max / voxelizationResolution;
        correctPosition = new Vector3(0, scale / 2, 0);

        yMaterialHeight = sizeY;
        lMaterialLength = l_max;

        numVoxelsX = (int)(sizeX / scale + 1);
        numVoxelsY = (int)(sizeY / scale + 1);
        numVoxelsZ = (int)(sizeZ / scale + 1);

        if (numVoxelsX * numVoxelsY * numVoxelsZ >= Agent.GetComponent<AgentMT>().voxelNumLimit)
        {
            print("WARNING: Too many voxels required, the performance can degradate!");
            EditorApplication.isPlaying = false;
        }
        else
        {
            HandleModel(ModelMaterial);
            HandleModel(ModelProduct);

            voxelizeStart = new Vector3(ModelMaterial.GetComponent<Renderer>().bounds.center.x - scale * numVoxelsX / 2 + scale / 2,
                                        ModelMaterial.GetComponent<Renderer>().bounds.center.y - scale * numVoxelsY / 2 + scale / 2,
                                        ModelMaterial.GetComponent<Renderer>().bounds.center.z - scale * numVoxelsZ / 2 + scale / 2);

            BuildVoxelizedModel();
        }
    }

    /********************* Self defined methods *********************/
    private void HandleModel(GameObject Model)
    {
        Model.transform.parent = GameObject.Find("AxisB2").transform;
        Model.transform.localPosition = correctPosition;

        Model.AddComponent<Rigidbody>();
        Model.GetComponent<Rigidbody>().isKinematic = true;
        Model.AddComponent<MeshCollider>();
    }
    private void BuildVoxelizedModel()
    {
        ModelMaterial.SetActive(true);
        ModelProduct.SetActive(true);

        float voxelPositionX = voxelizeStart.x;
        float voxelPositionY = voxelizeStart.y;
        float voxelPositionZ = voxelizeStart.z;

        for (int y = 0; y < numVoxelsY; y++)
        {
            for (int z = 0; z < numVoxelsZ; z++)
            {
                for (int x = 0; x < numVoxelsX; x++)
                {
                    Vector3 voxelPosition = new Vector3(voxelPositionX, voxelPositionY, voxelPositionZ);
                    voxelPositionX += scale;
                    AddVoxel(voxelPosition);
                }
                voxelPositionZ += scale;
                voxelPositionX = voxelizeStart.x;
            }
            voxelPositionY += scale;
            voxelPositionX = voxelizeStart.x;
            voxelPositionZ = voxelizeStart.z;
        }

        VoxelsToDetermine = GameObject.FindGameObjectsWithTag("toDetermine");

        foreach (GameObject VoxelToDetermine in VoxelsToDetermine)
        {
            DetermineVoxel(VoxelToDetermine);
        }

        VoxelsToKeep = GameObject.FindGameObjectsWithTag("toKeep");
        VoxelsToCut = GameObject.FindGameObjectsWithTag("toCut");

        foreach (GameObject VoxelToKeep in VoxelsToKeep)
        {
            VoxelToKeep.transform.parent = voxelsToKeep;
        }
        foreach (GameObject VoxelToCut in VoxelsToCut)
        {
            VoxelToCut.transform.parent = voxelsToCut;
        }

        toKeepNum = voxelsToKeep.childCount;
        toCutNum = voxelsToCut.childCount;

        ModelMaterial.SetActive(false);
        ModelProduct.SetActive(false);
    }
    private void AddVoxel(Vector3 voxelPosition)
    {
        var Voxel = GameObject.CreatePrimitive(PrimitiveType.Cube);

        Voxel.transform.localScale = new Vector3(scale, scale, scale);
        Voxel.transform.localPosition = voxelPosition;

        Voxel.GetComponent<BoxCollider>().size = new Vector3(voxSafetyFactor, voxSafetyFactor, voxSafetyFactor);
        Voxel.GetComponent<BoxCollider>().isTrigger = true;

        Voxel.AddComponent<Rigidbody>();
        Voxel.GetComponent<Rigidbody>().useGravity = false;
        Voxel.GetComponent<Rigidbody>().isKinematic = false;
        Voxel.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        Voxel.AddComponent<Coll_Vox>();

        if (Physics.CheckBox(voxelPosition, new Vector3(scale / 2, scale / 2, scale / 2), Quaternion.identity, LayerMask.GetMask("Model product")))
        {
            Voxel.tag = "toKeep";
            Voxel.layer = LayerMask.NameToLayer("Voxel toKeep");
        }
        else if (Physics.CheckBox(voxelPosition, new Vector3(scale / 2, scale / 2, scale / 2), Quaternion.identity, LayerMask.GetMask("Model material")))
        {
            Voxel.tag = "toCut";
            Voxel.layer = LayerMask.NameToLayer("Voxel toCut");
        }
        else
        {
            Voxel.tag = "toDetermine";
        }
    }
    private void DetermineVoxel(GameObject Voxel)
    {
        int layerMask = (1 << LayerMask.NameToLayer("Voxel toCut")) + (1 << LayerMask.NameToLayer("Voxel toKeep"));
        
        RaycastHit raycastHit1 = new RaycastHit();
        Physics.Raycast(Voxel.transform.position, new Vector3(0, 1, 0), out raycastHit1, l_max, layerMask);

        if (raycastHit1.collider == null)
        {
            Destroy(Voxel);
        }
        else
        {
            RaycastHit raycastHit2 = new RaycastHit();
            RaycastHit raycastHit3 = new RaycastHit();
            RaycastHit raycastHit4 = new RaycastHit();
            RaycastHit raycastHit5 = new RaycastHit();

            Physics.Raycast(Voxel.transform.position, new Vector3(1, 0, 0), out raycastHit2, l_max, layerMask);
            Physics.Raycast(Voxel.transform.position, new Vector3(-1, 0, 0), out raycastHit3, l_max, layerMask);
            Physics.Raycast(Voxel.transform.position, new Vector3(0, 0, 1), out raycastHit4, l_max, layerMask);
            Physics.Raycast(Voxel.transform.position, new Vector3(0, 0, -1), out raycastHit5, l_max, layerMask);

            if (raycastHit1.collider.CompareTag("toCut") ||
				raycastHit2.collider.CompareTag("toCut") || 
                raycastHit3.collider.CompareTag("toCut") ||
                raycastHit4.collider.CompareTag("toCut") ||
                raycastHit5.collider.CompareTag("toCut"))
            {
                Voxel.tag = "toCut";
                Voxel.layer = LayerMask.NameToLayer("Voxel toCut");
            }
            else
            {
                Voxel.tag = "toKeep";
                Voxel.layer = LayerMask.NameToLayer("Voxel toKeep");
            }
        }
    }
    private void DestroyVoxelizedModel()
    {
        foreach (Transform voxel in voxelsToKeep)
        {
			voxel.gameObject.SetActive(false);
            Destroy(voxel.gameObject);
        }
		foreach (Transform voxel in voxelsToCut)
        {
			voxel.gameObject.SetActive(false);
            Destroy(voxel.gameObject);
        }
    }
    internal void SettleVoxels()
    {
        DestroyVoxelizedModel();
        BuildVoxelizedModel();
    }
    internal void _Reset()
    {
        foreach (Transform voxel in voxelsToCut)
        {
            voxel.gameObject.SetActive(true);
        }
        foreach (Transform voxel in voxelsToKeep)
        {
            voxel.gameObject.SetActive(true);
        }
    }
}