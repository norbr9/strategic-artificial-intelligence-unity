using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cohesion : Seek {

    public Kinematic[] targets;
    public float threshold;

    GameObject objetivo;
    Kinematic kinetic;

    void Start()
    {
        objetivo = new GameObject(); //Creo el objetivo
        objetivo.AddComponent<Kinematic>();

        kinetic = objetivo.GetComponent<Kinematic>();
    }

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
                steering.vector += target.posicion;
                count++;
            }
        }

        if (count == 0)
        {
            return steering;
        }
        steering.vector /= count;
        kinetic.posicion = steering.vector;
        target = kinetic;
        return base.GetSteering(character);
    }
}
