using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

    Nodo nodoActual;
    Nodo nodoFinal;
    GameObject nodoEnd; //Objeto visual
    LRTAStar lrtaStar = new LRTAStar();
    private MyGrid grid;
    float[,] mapaCostes;
    private void Awake()
    {
        grid = GetComponent<MyGrid>();
        
    }
    private void Start()
    {
        mapaCostes = grid.ObtenerMatrizCostes("SoldadoLigeroAzul");
    }


    // Update is called once per frame
    void Update () {
       
    }

    public List<Vector3> EstablecerNodoFinal(PersonajeNPC npc)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        List<Nodo> nodos;
        nodoActual = grid.NodeFromWorldPoint(npc.posicion);
        if (Physics.Raycast(ray, out hit, 1000.0f))
        {
            if (hit.transform != null && hit.transform.tag != "Muro" && hit.transform.tag != "Agua")
            {
                if (nodoEnd != null) Destroy(nodoEnd);
                nodoEnd = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                nodoEnd.transform.localScale = new Vector3(10, 10, 10);
                nodoEnd.transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y + 1, hit.transform.position.z);
                nodoEnd.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
                nodoFinal = grid.NodeFromWorldPoint(nodoEnd.transform.position);
                nodos = lrtaStar.FindPath(nodoActual, nodoFinal, npc.distancePathfinding, grid, mapaCostes);
                List<Vector3> aux = new List<Vector3>(nodos.Count);
                for (int i=0; i < nodos.Count; i++)
                {
                    aux.Add(nodos[i].vPosition);
                    
                }
                return aux;

            }
        }

        return null;
    }

}
