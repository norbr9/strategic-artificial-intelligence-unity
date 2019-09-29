using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector
{
    Collision colision;

    void Start()
    {
        colision = new Collision();
        colision.normal = Vector3.zero;
        colision.posicion = Vector3.zero;
    }

    public Collision getCollision(Vector3 origen,Vector3 direccion, float maxDistacia)
    {
        RaycastHit[] hit = Physics.RaycastAll(origen, direccion, maxDistacia);
        colision = new Collision();
        colision.normal = Vector3.zero;
        colision.posicion = Vector3.zero;
        bool fin = false;
        for (int i=0; i< hit.Length && !fin; i++)
        {
            if (hit[i].transform.CompareTag("Muro")|| hit[i].transform.CompareTag("Agua"))
            {
                colision.posicion = hit[i].point;
                colision.normal += hit[i].normal;
                fin = true;
            }
        }
        if (fin)
            return colision;
        return null;
    }
}
