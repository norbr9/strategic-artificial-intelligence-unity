using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Separation : Steering {

    public Kinematic[] targets;

    public float threshold;
    public float decayCoefficient;


    public override SteeringOutPut GetSteering(Kinematic character)
    {
        SteeringOutPut steering = new SteeringOutPut();
        foreach (Kinematic target in targets)
        {
            Vector3 direction = character.posicion - target.posicion;
            float distance = direction.magnitude;
            if (distance < threshold)
            {
                float strength = Mathf.Min(decayCoefficient / (distance * distance), character.maxAceleration);

                direction.Normalize();

                steering.vector += strength * direction;
            }
        }
        return steering;
    }
}
