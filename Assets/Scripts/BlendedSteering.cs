using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendedSteering : Steering {

    public BehaviorAndWeight[] behaviors;

    public override SteeringOutPut GetSteering(Kinematic character)
    {
        SteeringOutPut steering = new SteeringOutPut();
        SteeringOutPut steeringAux = new SteeringOutPut();
        foreach (BehaviorAndWeight behavior in behaviors)
        {
            steeringAux = behavior.behavior.GetSteering(character);
            steering.vector += steeringAux.vector * behavior.weight;
            steering.real += steeringAux.real * behavior.weight;
        }
        return steering;
    }
}
