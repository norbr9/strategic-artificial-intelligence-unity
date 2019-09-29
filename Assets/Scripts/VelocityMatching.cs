using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityMatching : Steering {

    public Kinematic target;
    float timeToTarget = 0.1f;

    public override SteeringOutPut GetSteering(Kinematic character)
    {
        SteeringOutPut steering = new SteeringOutPut();

        steering.vector = target.velocidadActual - character.velocidadActual;
        steering.vector /= timeToTarget;

        if(steering.vector.magnitude > character.maxAceleration)
        {
            steering.vector.Normalize();
            steering.vector *= character.maxAceleration;
        }
        steering.real = 0;
        return steering;
    }
}
