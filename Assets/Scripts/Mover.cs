using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : Personaje {
    [System.Serializable]
    public class Entradas2
    {
        public string horizontal = "Horizontal";
        public string vertical = "Vertical";
    }

    [System.Serializable]
    public class Ajustes2
    {
        public float v;
    }

    [SerializeField]
    Entradas2 entradas;
    [SerializeField]
    Ajustes2 ajustes;

    public float orientacionPerGrados;
    float hor, ver;

    void Start()
    {
        iniciar(transform.position);
    }


    void Update()
    {
        getInput();
    }

    private void FixedUpdate()
    {
        mueve();
        cambiarOrientacion();
        
    }

    void getInput()
    {
        hor = Input.GetAxis(entradas.horizontal);
        ver = Input.GetAxis(entradas.vertical);
    }
    
    void mueve()
    {
        transform.position += hor * ajustes.v * Vector3.right + ver * ajustes.v * Vector3.forward;
        actualizarPosicion(transform.position);
    }

    void cambiarOrientacion()
    {
        transform.rotation = Quaternion.identity;
        transform.Rotate(Vector3.up, orientacionPerGrados);
        float orientacionPerRad = orientacionPerGrados * Mathf.Deg2Rad;
        actualizarOrientacion(orientacionPerRad);
    }
}
