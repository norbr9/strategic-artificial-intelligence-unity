using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollowing : Seek {

    public Vector3[] puntos;
    public int radius;

    public int nodoActual;
    
    GameObject objetivo;
    Kinematic pathTarget;


    void Start()
    {
        objetivo = new GameObject(); //Creo el objetivo
        objetivo.AddComponent<Kinematic>();
        pathTarget = objetivo.GetComponent<Kinematic>();
        nodoActual = 0;
    }

    public override SteeringOutPut GetSteering(Kinematic character)
    {

        pathTarget.posicion = puntos[nodoActual];
        Vector3 distance = pathTarget.posicion - character.posicion;
        float distancia = distance.magnitude;
        
        if (distancia <= radius) nodoActual++;

        if (nodoActual >= puntos.Length) nodoActual = 0;
        target = pathTarget;

        return base.GetSteering(character);
    }
}
