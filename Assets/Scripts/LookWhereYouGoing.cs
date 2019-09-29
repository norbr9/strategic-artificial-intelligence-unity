using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookWhereYouGoing : Align {


    public override SteeringOutPut GetSteering(Kinematic character)
    {
        float distance = character.velocidadActual.magnitude;

        if (distance == 0)
        {
            SteeringOutPut steering = new SteeringOutPut();
            return steering;
        }
        
        target.orientacion = Mathf.Atan2(character.velocidadActual.z, character.velocidadActual.x);
        return base.GetSteering(character);
    }
}
