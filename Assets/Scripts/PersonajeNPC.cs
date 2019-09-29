using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonajeNPC : Personaje {

    Steering[] listSteerings;
    public int distancePathfinding;
    public Pathfinding controlador;

    List<GameObject> camino;
    // Use this for initialization
    void Start () {
        iniciar(transform.position);
        listSteerings = GetComponents<Steering>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.A))
            distancePathfinding = 1;
        if (Input.GetKey(KeyCode.S))
            distancePathfinding = 2;
        if (Input.GetKey(KeyCode.D))
            distancePathfinding = 3;
        if (Input.GetMouseButtonDown(0) && distancePathfinding != 0)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000.0f))
            {
                if (hit.transform != null && hit.transform.tag != "Muro" && hit.transform.tag != "Agua")
                {
                    listPuntos = controlador.EstablecerNodoFinal(this);
                    pintarCamino();
                }
            }
        }
        for (int i = 0; i < listSteerings.Length; i++)
        {
            SteeringOutPut str = listSteerings[i].GetSteering(this);
            setSteering(str);
        }
	}

    void pintarCamino()
    {
        if (listPuntos.Count != 0)
        {
            if (camino != null)
            {
                foreach (GameObject g in camino)
                {
                      Destroy(g);
                }
            }
            
            camino = new List<GameObject>();
            GameObject aux;
            foreach(Vector3 v in listPuntos)
            {
                aux = GameObject.CreatePrimitive(PrimitiveType.Cube);
                aux.transform.localScale = new Vector3(5, 2, 5);
                aux.transform.position = v;
                aux.GetComponent<Renderer>().material.color = new Color(0, 0, 0);
                camino.Add(aux);
            }
            
        }
    }


}
