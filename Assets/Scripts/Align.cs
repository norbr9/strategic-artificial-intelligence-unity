using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Align : Steering
{
    public Kinematic target;

    public float targetRadius; 
    public float slowRadius; 
    //float timeToTarget = 0.1f;

    public override SteeringOutPut GetSteering(Kinematic character)
    {
        float targetRadiusLoc = targetRadius * Mathf.Deg2Rad;
        float slowRadiusLoc = slowRadius * Mathf.Deg2Rad;
        SteeringOutPut steering = new SteeringOutPut();
        float rotation = target.orientacion - character.orientacion;
        rotation = mapToRange(rotation);
        rotation = rotation * Mathf.Deg2Rad;
        
        float rotationSize = Mathf.Abs(rotation);
        if (rotationSize < targetRadiusLoc)
        {
            steering.real = -character.rotacionActual;
            return steering;
        }
        float targetRotation;
        if (rotationSize > slowRadiusLoc)
        {
            targetRotation = character.maxRotation;
        }
        else
        {
            targetRotation = character.maxRotation * rotationSize / slowRadiusLoc;
        }
        targetRotation *= rotation / rotationSize;

        steering.real = targetRotation - character.rotacionActual;
        
        float angularAcceleration = Mathf.Abs(steering.real);
        if (angularAcceleration > character.maxAcelerationAngular)
        {
            steering.real /= angularAcceleration;
            steering.real *= character.maxAcelerationAngular;
        }
        steering.vector = new Vector3(0,0,0);
        return steering;
    }

    float mapToRange(float rotation)
    {
        float grados = rotation * Mathf.Rad2Deg;
        if(grados > 180) //Si es mayor de 180 (p.e 270), pasarlo a la rotación más corta por el otro sentido (-90)
        {
            grados = grados - 360;
        }
        else if(grados < -180)
        {
            grados = 360 + grados;
        }
        return grados;
    }

}
