using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path {
    
    public Vector3[] puntos;

    public void establecerCamino(Vector3[] p)
    {
        puntos = new Vector3[p.Length];
        for(int i =0; i < p.Length; i++)
        {
            puntos[i] = p[i];
        }
    }
}
