﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {

	public CameraAnchor Anchor;
	public Camera Camera;

	// Use this for initialization
	void Start () {
		Anchor = transform.parent.GetComponent<CameraAnchor>();
		Camera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
