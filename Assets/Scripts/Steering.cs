using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Steering : MonoBehaviour{

    public abstract SteeringOutPut GetSteering(Kinematic character);

    public float randomBinomial()
    {
        return Random.value - Random.value;
    }

    public Vector3 asVector(float orientacion)
    {
        Vector3 v = new Vector3(Mathf.Sin(orientacion), 0, Mathf.Cos(orientacion));
        return v;
    }
}
