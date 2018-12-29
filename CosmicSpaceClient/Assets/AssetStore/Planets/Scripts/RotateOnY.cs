using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is only to rotate the stars corona around the Y axis to make it feel animated.
public class RotateOnY : MonoBehaviour
{

    public float Speed;

    private Vector3 rotationAxis;

	void Start () {
        rotationAxis = transform.up;
	}
	
	void Update () {
        transform.RotateAround(transform.position,rotationAxis,Speed*Time.deltaTime);
	}
}
