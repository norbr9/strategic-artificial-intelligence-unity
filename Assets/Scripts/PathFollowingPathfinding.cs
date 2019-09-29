using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollowingPathfinding : Seek
{
    public Vector3[] puntos;
    public List<Vector3> listPuntos = new List<Vector3>();
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
        SteeringOutPut steering = new SteeringOutPut();
        if (listPuntos != character.listPuntos)
        {
            listPuntos = character.listPuntos;
            nodoActual = 0;
        }
        if(listPuntos.Count != 0 && nodoActual < listPuntos.Count)
        {
            puntos = new Vector3[listPuntos.Count];
            for(int i = 0; i < listPuntos.Count; i++)
            {
                puntos[i] = listPuntos[i];
            }
            pathTarget.posicion = puntos[nodoActual];
            Vector3 distance = pathTarget.posicion - character.posicion;
            float distancia = distance.magnitude;

            if (distancia <= radius) nodoActual++;

            if (nodoActual >= puntos.Length)
            {
                return steering;
            }
            target = pathTarget;

            return base.GetSteering(character);
        }
        return steering;
    }
}
