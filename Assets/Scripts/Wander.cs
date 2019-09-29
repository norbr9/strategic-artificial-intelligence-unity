using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : Face {

    public float wanderOffset; // Distancia al centro del circulo 
    public float wanderRadius; // Radio del circulo

    public float wanderRate;

    public float wanderOrientation;

    GameObject objetivo;
    Kinematic kinetic;

    void Start()
    {
        objetivo = new GameObject(); //Creo el objetivo
        objetivo.AddComponent<Kinematic>();

        kinetic = objetivo.GetComponent<Kinematic>();
        wanderRate = wanderRate * Mathf.Deg2Rad;  //Máxima Orientacion
    }


    public override SteeringOutPut GetSteering(Kinematic character)
    {
        SteeringOutPut steering = new SteeringOutPut();
        if (kinetic != null) kinetic.posicion = character.posicion;

        wanderOrientation += randomBinomial() * wanderRate;

        float targetOrientation = wanderOrientation + character.orientacion; //La orientación del objetivo
        kinetic.orientacion = targetOrientation;

        Vector3 target = character.posicion + wanderOffset * asVector(character.orientacion);
        target += wanderRadius * asVector(targetOrientation);

        kinetic.posicion = target;
        targetOrig = kinetic;
        steering = base.GetSteering(character);

        steering.vector = character.maxAceleration * asVector(targetOrientation);
        return steering;
    }

    

}
