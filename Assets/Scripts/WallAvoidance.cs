using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallAvoidance : Seek
{
    CollisionDetector collisionDetector;

    public float avoidDistance;

    public float lookahead;

    public Vector3 rayVector;

    GameObject objetivo;
    Kinematic kinetic;

    void Start()
    {
        objetivo = new GameObject(); //Creo el objetivo
        objetivo.AddComponent<Kinematic>();

        kinetic = objetivo.GetComponent<Kinematic>();
        collisionDetector = new CollisionDetector();
    }


    public override SteeringOutPut GetSteering(Kinematic character)
    {
        SteeringOutPut steering = new SteeringOutPut();
        rayVector = character.velocidadActual;
        rayVector.Normalize();
        rayVector *= lookahead;

        Collision collision = collisionDetector.getCollision(character.posicion, rayVector, lookahead);

        if (collision != null)
        {
            kinetic.posicion = collision.posicion + collision.normal * avoidDistance;
            target = kinetic;
            return base.GetSteering(character);
        }
        return steering;
    }
}
