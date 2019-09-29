using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : Steering {

    public Kinematic target; 

    public override SteeringOutPut GetSteering(Kinematic character)
    {
        SteeringOutPut steering = new SteeringOutPut();
        steering.vector = target.posicion - character.posicion;
        steering.vector.Normalize();
        steering.vector *= character.maxAceleration;
        steering.vector = steering.vector - character.velocidadActual;
        steering.real = 0;
        return steering;
    }

}
