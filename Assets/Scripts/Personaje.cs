using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Personaje : Kinematic{

	// Use this for initialization
	void Start () {
       
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnDrawGizmos()
    {
        // Draws a 5 unit long red line in front of the object
        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(Vector3.forward) * 8;
        Gizmos.DrawRay(transform.position, direction);
    }



}
