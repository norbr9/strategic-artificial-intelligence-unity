using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alignment : Steering {

    public Kinematic[] targets;
    public float threshold;

    public override SteeringOutPut GetSteering(Kinematic character)
    {
        SteeringOutPut steering = new SteeringOutPut();
        int count = 0;
        foreach (Kinematic target in targets)
        {
            Vector3 direction = target.posicion - character.posicion;
            float distance = direction.magnitude;

            if (distance < threshold)
            {
                steering.real += target.orientacion;
                count++;
            }
        }

        if (count > 0)
        {
            steering.real = steering.real / count;
            steering.real -= character.orientacion;
        }

        return steering;
    }
}
