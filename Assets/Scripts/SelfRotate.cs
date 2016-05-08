﻿using UnityEngine;
using System.Collections;

public class SelfRotate : MonoBehaviour {

	public float rotateSpeed = 10;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
	}
}
