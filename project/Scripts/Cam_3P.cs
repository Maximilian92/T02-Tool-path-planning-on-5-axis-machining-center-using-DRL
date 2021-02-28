using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Animations;

public class Cam_3P : MonoBehaviour
{
	/* Camera component */

	Camera Cam3P;

	/* Important infos */

	Transform target;
	Vector3 offset;

	/* Changable details */

	bool heuristic;
	List<float> scaleRange = new List<float> { 2f, 20 };
	float scaleSensity = 2;
	float rotationSensity = 2;
	float translateSensity = 5;
	float translateRapid = 10;

	/********************* Template methods *********************/
	void Awake()
	{
		Cam3P = gameObject.GetComponent<Camera>();
	}
	void Start()
	{
		Cam3P.clearFlags = CameraClearFlags.SolidColor;
		Cam3P.cullingMask &= ~(1 << LayerMask.NameToLayer("Detector"));
		Cam3P.cullingMask &= ~(1 << LayerMask.NameToLayer("MarkerFrontTop"));
		Cam3P.cullingMask &= ~(1 << LayerMask.NameToLayer("MarkerFront"));
		Cam3P.cullingMask &= ~(1 << LayerMask.NameToLayer("MarkerTop"));
		Cam3P.fieldOfView = 60;
		Cam3P.nearClipPlane = 0.01f;
		Cam3P.farClipPlane = 1000;
		Cam3P.depth = -1;
	
		target = GameObject.Find("Cutter").transform;
		offset = transform.position - target.position;
		
		heuristic = GameObject.Find("Agent").GetComponent<AgentMT>().heuristic;
	}
	void Update()
	{
		if (heuristic)
		{
			transform.position = target.position + offset;
			Scale();
			HeuristicRotate();
		}
		else
		{
			DefaultRotate();
			Translate();
		}
	}
	
	/********************* Self defined methods *********************/
	private void Scale()
	{
		float distance = offset.magnitude;
		distance -= Input.GetAxis("Mouse ScrollWheel") * scaleSensity;
		if (distance < scaleRange[0] || distance > scaleRange[1])
		{
			return;
		}
		offset = offset.normalized * distance;
	}
	private void HeuristicRotate()
	{
		if (Input.GetMouseButton(1))
		{
			transform.RotateAround(target.position, Vector3.up, Input.GetAxis("Mouse X") * rotationSensity);
			transform.RotateAround(target.position, Vector3.right, Input.GetAxis("Mouse Y") * rotationSensity);
			if (transform.localEulerAngles.z != 0)
			{
				transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
			}
			offset = transform.position - target.position;
		}
	}
	private void DefaultRotate()
	{
		if (Input.GetMouseButton(1))
		{
			transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * rotationSensity);
			transform.Rotate(Vector3.left, Input.GetAxis("Mouse Y") * rotationSensity);
			if (transform.localEulerAngles.z != 0)
			{
				transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
			}
		}
	}
	private void Translate()
	{
		if (Input.GetMouseButton(1))
		{
			if (Input.GetKey(KeyCode.D)) transform.Translate(Time.deltaTime * translateSensity, 0, 0);
			if (Input.GetKey(KeyCode.A)) transform.Translate(-1 * Time.deltaTime * translateSensity, 0, 0);
			if (Input.GetKey(KeyCode.E)) transform.Translate(0, Time.deltaTime * translateSensity, 0);
			if (Input.GetKey(KeyCode.Q)) transform.Translate(0, -1 * Time.deltaTime * translateSensity, 0);
			if (Input.GetKey(KeyCode.W)) transform.Translate(0, 0, Time.deltaTime * translateSensity);
			if (Input.GetKey(KeyCode.S)) transform.Translate(0, 0, -1 * Time.deltaTime * translateSensity);
		}
		if (Input.GetMouseButton(1) && Input.GetKey(KeyCode.LeftShift))
		{
			if (Input.GetKey(KeyCode.D)) transform.Translate(Time.deltaTime * translateRapid, 0, 0);
			if (Input.GetKey(KeyCode.A)) transform.Translate(-1 * Time.deltaTime * translateRapid, 0, 0);
			if (Input.GetKey(KeyCode.E)) transform.Translate(0, Time.deltaTime * translateRapid, 0);
			if (Input.GetKey(KeyCode.Q)) transform.Translate(0, -1 * Time.deltaTime * translateRapid, 0);
			if (Input.GetKey(KeyCode.W)) transform.Translate(0, 0, Time.deltaTime * translateRapid);
			if (Input.GetKey(KeyCode.S)) transform.Translate(0, 0, -1 * Time.deltaTime * translateRapid);
		}
	}
}