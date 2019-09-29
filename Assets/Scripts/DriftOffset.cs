using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriftOffset
{
    public Vector3 Posicion { get; set; }
    public float Orientacion { get; set; }

    public DriftOffset()
    {
        Posicion = new Vector3();
        Orientacion = 0;
    }
}
