using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evade : Flee {

    public Kinematic fleeTarget;
    public float maxPrediction;

    public override SteeringOutPut GetSteering(Kinematic character)
    {

        Vector3 direction = fleeTarget.posicion - character.posicion;
        float distance = direction.magnitude;

        float speed = character.velocidadActual.magnitude;

        float prediction;
        if (speed <= distance / maxPrediction)
        {
            prediction = maxPrediction;
        }
        else
        {
            prediction = distance / speed;
        }

        target = fleeTarget;
        target.posicion += target.velocidadActual * prediction;
        return base.GetSteering(character);
    }
}
