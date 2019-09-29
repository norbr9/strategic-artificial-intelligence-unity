using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursue : Seek {

    public Kinematic pursueTarget;
    public float maxPrediction;

    public override SteeringOutPut GetSteering(Kinematic character)
    {
        Vector3 direction = pursueTarget.posicion - character.posicion;
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
        
        target = pursueTarget;
        target.posicion += target.velocidadActual * prediction;
        return base.GetSteering(character);  //Delegamos en la clase padre
    }
}
