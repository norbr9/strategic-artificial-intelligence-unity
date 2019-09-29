using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrioritySteering : Steering {

    public BlendedSteering[] groups;

    public float epsilon; //Que sea un valor pequeño

    public override SteeringOutPut GetSteering(Kinematic character)
    {
        SteeringOutPut steering = new SteeringOutPut();

        foreach (BlendedSteering group in groups)
        {
            steering = group.GetSteering(character);

            if((steering.vector.magnitude > epsilon)||(Mathf.Abs(steering.real) > epsilon)) //Si el valor de este grupo es muy pequeño, se pasa al siguiente grupo
            {
                return steering;
            }
        }

        return steering; //Si se llega aqui, entonces se pasa el steering del último grupo, aunque sea el del menor valor
    }
}
