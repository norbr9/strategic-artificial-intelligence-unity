using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrive : Steering {

    public Kinematic target;
  
    public float targetRadius; //Radio para llegar al objetivo
    public float slowRadius; //Radio para reducir la velocidad
    float timeToTarget = 0.1f;

    public override SteeringOutPut GetSteering(Kinematic character)
    {
        if (target == null) return new SteeringOutPut();
        SteeringOutPut steering = new SteeringOutPut();
        Vector3 direction = target.posicion - character.posicion;
        float distancia = direction.magnitude;
        
        if (distancia < targetRadius)
        {
            steering.vector = -character.velocidadActual;  //Sería -VelocidadActual para anular la velocidad y que se quede quieto
            return steering;
        }
        float targetSpeed;
        if (distancia > slowRadius)
        {
            targetSpeed = character.maxSpeed;
        }
        else
        {
            targetSpeed = character.maxSpeed * distancia / slowRadius;
        }
        Vector3 targetVelocity = direction;
        targetVelocity.Normalize();
        targetVelocity *= targetSpeed;

        steering.vector = targetVelocity - character.velocidadActual;
        steering.vector = new Vector3(steering.vector.x/timeToTarget, steering.vector.y/timeToTarget,steering.vector.z/timeToTarget);  

        if(steering.vector.magnitude > character.maxAceleration)
        {
            steering.vector.Normalize();
            steering.vector *= character.maxAceleration;
        }

        steering.real = 0;
        
        return steering;
    }
}
