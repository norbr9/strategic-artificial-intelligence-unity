using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarSearch : MonoBehaviour {

    public Grid GridReference;//For referencing the grid class
    public Transform StartPosition;//Starting position to pathfind from
    public Transform TargetPosition;//Starting position to pathfind to

    public NPC[] personajes;

    public bool ocupadoSeleccionando; //Esta variable serivirá para cuando se seleccione un personaje (A) para elegir un nuevo target, los demás personajes
                                      //si se selecciona a uno de ellos (B) con el objetivo de que A vaya a por B, B no se quede seleccionado para buscar un nuevo objetivo
    public Transform[] waypointsRojos;
    public Transform[] waypointsAzules;

    public Transform respawnRojo;
    public Transform respawnAzul;

    public Transform[] zonasSegurasRojo;
    public Transform[] zonasSegurasAzul;

    public Transform[] caminoPatrullaRojo;
    public Transform[] caminoPatrullaAzul;

    List<NPC> equipoRojo;
    List<NPC> equipoAzul;

    public GameObject salidaRojo;
    public GameObject salidaAzul;

    private void Awake()//When the program starts
    {
        GridReference = GetComponent<Grid>();//Get a reference to the game manager
        ocupadoSeleccionando = false;
        equipoRojo = new List<NPC>();
        equipoAzul = new List<NPC>();
        asignarObjetivos();    
    }

    private void Update()//Every frame
    {
        foreach (NPC personaje in personajes)
        {
            GridReference.establecerMapaInfluencias(personaje.transform);
        }   
    }

    public List<Nodo> FindPath(Vector3 a_StartPos, Vector3 a_TargetPos, float[,] mapaCostes)
    {
        Nodo StartNode = GridReference.NodeFromWorldPoint(a_StartPos);//Gets the node closest to the starting position
        Nodo TargetNode = GridReference.NodeFromWorldPoint(a_TargetPos);//Gets the node closest to the target position

        List<Nodo> OpenList = new List<Nodo>();//List of nodes for the open list
        HashSet<Nodo> ClosedList = new HashSet<Nodo>();//Hashset of nodes for the closed list

        OpenList.Add(StartNode);//Add the starting node to the open list to begin the program

        while (OpenList.Count > 0)//Whilst there is something in the open list
        {
            Nodo CurrentNode = OpenList[0];//Create a node and set it to the first item in the open list
            for (int i = 1; i < OpenList.Count; i++)//Loop through the open list starting from the second object
            {
                if (OpenList[i].FCost * mapaCostes[OpenList[i].iGridX, OpenList[i].iGridY] < CurrentNode.FCost * mapaCostes[CurrentNode.iGridX, CurrentNode.iGridY]
                        || OpenList[i].FCost * mapaCostes[OpenList[i].iGridX, OpenList[i].iGridY] == CurrentNode.FCost * mapaCostes[CurrentNode.iGridX, CurrentNode.iGridY] 
                            && OpenList[i].ihCost * mapaCostes[OpenList[i].iGridX, OpenList[i].iGridY] < CurrentNode.ihCost * mapaCostes[CurrentNode.iGridX, CurrentNode.iGridY])//If the f cost of that object is less than or equal to the f cost of the current node
                {
                    CurrentNode = OpenList[i];//Set the current node to that object
                }
            }
            OpenList.Remove(CurrentNode);//Remove that from the open list
            ClosedList.Add(CurrentNode);//And add it to the closed list

            if (CurrentNode == TargetNode)//If the current node is the same as the target node
            {
                return GetFinalPath(StartNode, TargetNode);//Calculate the final path
            }

            foreach (Nodo NeighborNode in GridReference.GetNeighboringNodes(CurrentNode))//Loop through each neighbor of the current node
            {
                if (!NeighborNode.bIsWall || ClosedList.Contains(NeighborNode))//If the neighbor is a wall or has already been checked
                {
                    continue;//Skip it
                }
                int MoveCost = CurrentNode.igCost + GetManhattenDistance(CurrentNode, NeighborNode);//Get the F cost of that neighbor

                if (MoveCost * mapaCostes[CurrentNode.iGridX, CurrentNode.iGridY] < NeighborNode.igCost * mapaCostes[NeighborNode.iGridX, NeighborNode.iGridY] || !OpenList.Contains(NeighborNode))//If the f cost is greater than the g cost or it is not in the open list
                {
                    NeighborNode.igCost = MoveCost;//Set the g cost to the f cost
                    NeighborNode.ihCost = GetManhattenDistance(NeighborNode, TargetNode);//Set the h cost
                    NeighborNode.ParentNode = CurrentNode;//Set the parent of the node for retracing steps

                    if (!OpenList.Contains(NeighborNode))//If the neighbor is not in the openlist
                    {
                        OpenList.Add(NeighborNode);//Add it to the list
                    }
                }
            }
        }
        List<Nodo> vacio = new List<Nodo>();
        return vacio;
    }

    List<Nodo> GetFinalPath(Nodo a_StartingNode, Nodo a_EndNode)
    {
        List<Nodo> FinalPath = new List<Nodo>();//List to hold the path sequentially 
        Nodo CurrentNode = a_EndNode;//Node to store the current node being checked

        while (CurrentNode != a_StartingNode)//While loop to work through each node going through the parents to the beginning of the path
        {
            FinalPath.Add(CurrentNode);//Add that node to the final path
            CurrentNode = CurrentNode.ParentNode;//Move onto its parent node
        }

        FinalPath.Reverse();//Reverse the path to get the correct order

        GridReference.FinalPath = FinalPath;//Set the final path

        return FinalPath;

    }

    int GetManhattenDistance(Nodo a_nodeA, Nodo a_nodeB)
    {
        int ix = Mathf.Abs(a_nodeA.iGridX - a_nodeB.iGridX);//x1-x2
        int iy = Mathf.Abs(a_nodeA.iGridY - a_nodeB.iGridY);//y1-y2

        return ix + iy;//Return the sum
    }


    public NPC devolverEnemigo(Transform enemigo)
    {
        bool encontrado = false;
        int i = 0;
        while(!encontrado && i< personajes.Length)
        {
            if (personajes[i].transform == enemigo)
                return personajes[i];

            i++;
        }
        return null;
    }

    public string getTagNodo(Nodo nodo)
    {
        //if (nodo == null) Debug.Log("ok no");
        return GridReference.getTagNodo(nodo.iGridX,nodo.iGridY);
    }

    public void moverHaciaSpawn(NPC npc)
    {
        if(npc.tag == "SoldadoLigeroRojo" || npc.tag == "SoldadoPesadoRojo" || npc.tag == "ExploradorRojo" || npc.tag == "ArqueroRojo")
        {
            npc.transform.position = new Vector3(respawnRojo.position.x, npc.transform.position.y, respawnRojo.position.z);
        } else
        {
            npc.transform.position = new Vector3(respawnAzul.position.x, npc.transform.position.y, respawnAzul.position.z);
        }
    }

    void asignarObjetivos()
    {
        
        foreach (NPC personaje in personajes)
        {
            if (personaje.tag == "SoldadoLigeroRojo" || personaje.tag == "SoldadoPesadoRojo" || personaje.tag == "ExploradorRojo" || personaje.tag == "ArqueroRojo")
                equipoRojo.Add(personaje);
            else
                equipoAzul.Add(personaje);
        }
        int r = 0;
        int a = 0;
        bool first = true;
        foreach(NPC personaje in equipoRojo) //Asignamos objetivos al equipo rojo
        {
            if(first)  //Si es el primero de la lista, le hacemos que patrulle
            {
                personaje.patrulla = true;
                personaje.actitud = 1; //Estos se inializarán a modo defensivo
                first = false;
            }
            else if (r < waypointsRojos.Length) //Asignamos en los waypoints del mismo equipo
            {
                personaje.target = waypointsRojos[r];
                personaje.patrulla = false;
                personaje.actitud = 1;  //Estos se inializarán a modo defensivo
                r++;
            }
            else //Si ya se han asignado al menos a uno los puntos rojos
            {
                if(a < waypointsAzules.Length)
                {
                    personaje.target = waypointsAzules[a];
                    personaje.patrulla = false;
                    personaje.actitud = 2;//Estos se inicializarán a modo neutro
                    a++;
                }
                else 
                {
                    a = 0;
                    personaje.target = waypointsAzules[a];
                    personaje.patrulla = false;
                    personaje.actitud = 2; //Estos se inicializarán a modo neutro
                    a++;
                }
            }
        }

        r = 0;
        a = 0;
        first = true;
        foreach (NPC personaje in equipoAzul) //Asignamos objetivos al equipo azul
        {
            if (first)  //Si es el primero de la lista, le hacemos que patrulle
            {
                personaje.patrulla = true;
                personaje.actitud = 1; //Estos se inicializarán a modo defensivo
                first = false;
            }
            else if (a < waypointsAzules.Length) //Asignamos en los waypoints del mismo equipo
            {
                personaje.target = waypointsAzules[a];
                personaje.patrulla = false;
                personaje.actitud = 1; //Estos se inicializarán a modo defensivo
                a++;
            }
            else //Si ya se han asignado al menos a uno los puntos rojos
            {
                if (r < waypointsRojos.Length)
                {
                    personaje.target = waypointsRojos[r];
                    personaje.patrulla = false;
                    personaje.actitud = 2;//Estos se inicializarán a modo neutro
                    r++;
                }
                else
                {
                    r = 0;
                    personaje.target = waypointsRojos[r];
                    personaje.patrulla = false;
                    personaje.actitud = 2; //Estos se inicializarán a modo neutro
                    r++;
                }
            }
        }
    }

    public Nodo[] crearCaminoPatrullaRojo()
    {
        Nodo[] camino = new Nodo[caminoPatrullaRojo.Length];
        for(int i = 0; i < caminoPatrullaRojo.Length; i++)
        {
            camino[i] = GridReference.NodeFromWorldPoint(caminoPatrullaRojo[i].position);
        }
        return camino;
    }

    public Nodo[] crearCaminoPatrullaAzul()
    {
        Nodo[] camino = new Nodo[caminoPatrullaAzul.Length];
        for (int i = 0; i < caminoPatrullaAzul.Length; i++)
        {
            camino[i] = GridReference.NodeFromWorldPoint(caminoPatrullaAzul[i].position);
        }
        return camino;
    }

    //Valores negativos = Zona Roja
    //Valores positivos = Zona Azul

    List<Transform> wayPointsRojosSiendoRojo()  //Para actitud defensiva
    {
        List<Transform> wp = new List<Transform>();
        foreach(Transform waypoint in waypointsRojos)
        {
            Nodo n = GridReference.NodeFromWorldPoint(waypoint.position);
            float valor = GridReference.valorInfluenciaNodo(n);
            if (valor > 0) wp.Add(waypoint); //Si el valor del waypoint no es rojo, añadir a posible zona
        }
        return wp;
    }

    List<Transform> wayPointsRojosSiendoAzul() //Para actitud ofensiva
    {
        List<Transform> wp = new List<Transform>();
        foreach (Transform waypoint in waypointsRojos)
        {
            Nodo n = GridReference.NodeFromWorldPoint(waypoint.position);
            float valor = GridReference.valorInfluenciaNodo(n);
            if (valor < 0) wp.Add(waypoint); //Si el valor del waypoint es rojo, añadir a posible zona
        }
        return wp;
    }

    List<Transform> wayPointsAzulesSiendoAzul() //Para actitud defensiva
    {
        List<Transform> wp = new List<Transform>();
        foreach (Transform waypoint in waypointsAzules)
        {
            Nodo n = GridReference.NodeFromWorldPoint(waypoint.position);
            float valor = GridReference.valorInfluenciaNodo(n);
            if (valor < 0) wp.Add(waypoint); //Si el valor del waypoint no es azul, añadir a posible zona
        }
        return wp;
    }

    List<Transform> wayPointsAzulesSiendoRojo() //Para actitud ofensiva
    {
        List<Transform> wp = new List<Transform>();
        foreach (Transform waypoint in waypointsAzules)
        {
            Nodo n = GridReference.NodeFromWorldPoint(waypoint.position);
            float valor = GridReference.valorInfluenciaNodo(n);
            if (valor > 0) wp.Add(waypoint); //Si el valor del waypoint es azul, añadir a posible zona
        }
        return wp;
    }

    public Transform buscarNuevoObjetivo(NPC npc, float[,] mapaCostes)
    {
        int equipo = 0; //1 es Equipo Rojo y 2 es Equipo Azul
        if (npc.tagNPC == "SoldadoLigeroRojo" || npc.tagNPC == "SoldadoPesadoRojo" || npc.tagNPC == "ExploradorRojo" || npc.tag == "ArqueroRojo")
            equipo = 1; 
        else
            equipo = 2;

        if (npc.actitud == 2)//Actitud Neutra, buscará primero si tiene que defender algún Waypoint, y si no, buscar waypoints enemigos
        {
            if(equipo == 1) //Equipo Rojo
            {
                //Busco defender
                List<Transform> wp = wayPointsRojosSiendoRojo();
                if(wp.Count == 0) //Todos los waypoints están defendidos
                {
                    //Busco atacar
                    wp = wayPointsAzulesSiendoRojo();
                    if (wp.Count == 0) //Si todos los waypoints estas a 0 o en rojos, ve a la base enemiga
                        return respawnAzul; 
                    if (wp.Count == 1)
                        return wp[0];
                    Transform escogido = wp[0];
                    List<Nodo> caminoEscogido = FindPath(npc.transform.position, wp[0].position, mapaCostes);
                    for (int i = 1; i < wp.Count; i++)
                    {
                        List<Nodo> camino = FindPath(npc.transform.position, wp[i].position, mapaCostes);
                        if (camino.Count < caminoEscogido.Count)
                        {
                            escogido = wp[i];
                            caminoEscogido = camino;
                        }
                    }
                    return escogido;
                } else //Defiendo
                {
                    if(wp.Count == 1)
                    {
                        return wp[0];
                    }
                    Transform escogido = wp[0];
                    List<Nodo> caminoEscogido = FindPath(npc.transform.position, wp[0].position, mapaCostes);
                    for(int i = 1; i < wp.Count; i++)
                    {
                        List<Nodo> camino = FindPath(npc.transform.position, wp[i].position, mapaCostes);
                        if(camino.Count < caminoEscogido.Count)
                        {
                            escogido = wp[i];
                            caminoEscogido = camino;
                        }
                    }
                    return escogido;
                }
            } else //Equipo Azul
            {
                //Busco defender
                List<Transform> wp = wayPointsAzulesSiendoAzul();
                if (wp.Count == 0) //Todos los waypoints están defendidos
                {
                    //Busco atacar
                    wp = wayPointsRojosSiendoAzul();
                    if (wp.Count == 0)
                        return respawnRojo; //Si todos los waypoints estas a 0 o en azules, ve a la base enemiga
                    if (wp.Count == 1)
                        return wp[0];
                    Transform escogido = wp[0];
                    List<Nodo> caminoEscogido = FindPath(npc.transform.position, wp[0].position, mapaCostes);
                    for (int i = 1; i < wp.Count; i++)
                    {
                        List<Nodo> camino = FindPath(npc.transform.position, wp[i].position, mapaCostes);
                        if (camino.Count < caminoEscogido.Count)
                        {
                            escogido = wp[i];
                            caminoEscogido = camino;
                        }
                    }
                    return escogido;
                }
                else //Defiendo
                {
                    if (wp.Count == 1)
                    {
                        return wp[0];
                    }
                    Transform escogido = wp[0];
                    List<Nodo> caminoEscogido = FindPath(npc.transform.position, wp[0].position, mapaCostes);
                    for (int i = 1; i < wp.Count; i++)
                    {
                        List<Nodo> camino = FindPath(npc.transform.position, wp[i].position, mapaCostes);
                        if (camino.Count < caminoEscogido.Count)
                        {
                            escogido = wp[i];
                            caminoEscogido = camino;
                        }
                    }
                    return escogido;
                }
            }
        } else if (npc.actitud == 1) //Actitud Defensiva
        {
            if (equipo == 1) //Equipo Rojo
            {
                //Busco defender
                List<Transform> wp = wayPointsRojosSiendoRojo();
                if (wp.Count == 0) //Todos los waypoints están defendidos, me voy al más cercano
                {
                    Transform escogido = waypointsRojos[0];
                    List<Nodo> caminoEscogido = FindPath(npc.transform.position, waypointsRojos[0].position, mapaCostes);
                    for (int i = 1; i < waypointsRojos.Length; i++)
                    {
                        List<Nodo> camino = FindPath(npc.transform.position, waypointsRojos[i].position, mapaCostes);
                        if (camino.Count < caminoEscogido.Count)
                        {
                            escogido = waypointsRojos[i];
                            caminoEscogido = camino;
                        }
                    }
                    return escogido;
                }
                else //Defiendo
                {
                    if (wp.Count == 1)
                    {
                        return wp[0];
                    }
                    Transform escogido = wp[0];
                    List<Nodo> caminoEscogido = FindPath(npc.transform.position, wp[0].position, mapaCostes);
                    for (int i = 1; i < wp.Count; i++)
                    {
                        List<Nodo> camino = FindPath(npc.transform.position, wp[i].position, mapaCostes);
                        if (camino.Count < caminoEscogido.Count)
                        {
                            escogido = wp[i];
                            caminoEscogido = camino;
                        }
                    }
                    return escogido;
                }
            }
            else //Equipo Azul
            {
                //Busco defender
                List<Transform> wp = wayPointsAzulesSiendoAzul();
                if (wp.Count == 0) //Todos los waypoints están defendidos, me voy al más cercano
                {
                    Transform escogido = waypointsAzules[0];
                    List<Nodo> caminoEscogido = FindPath(npc.transform.position, waypointsAzules[0].position, mapaCostes);
                    for (int i = 1; i < waypointsAzules.Length; i++)
                    {
                        List<Nodo> camino = FindPath(npc.transform.position, waypointsAzules[i].position, mapaCostes);
                        if (camino.Count < caminoEscogido.Count)
                        {
                            escogido = waypointsAzules[i];
                            caminoEscogido = camino;
                        }
                    }
                    return escogido;
                }
                else //Defiendo
                {
                    if (wp.Count == 1)
                    {
                        return wp[0];
                    }
                    Transform escogido = wp[0];
                    List<Nodo> caminoEscogido = FindPath(npc.transform.position, wp[0].position, mapaCostes);
                    for (int i = 1; i < wp.Count; i++)
                    {
                        List<Nodo> camino = FindPath(npc.transform.position, wp[i].position, mapaCostes);
                        if (camino.Count < caminoEscogido.Count)
                        {
                            escogido = wp[i];
                            caminoEscogido = camino;
                        }
                    }
                    return escogido;
                }
            }
        } else //Actitud Ofensiva
        {
            if (equipo == 1) //Equipo Rojo
            {
                //Busco atacar
                List<Transform> wp = wayPointsAzulesSiendoRojo();
                if (wp.Count == 0)
                     return respawnAzul; //Si todos los waypoints estas a 0 o en rojos, ve a la base enemiga
                if (wp.Count == 1)
                     return wp[0];
                Transform escogido = wp[0];
                List<Nodo> caminoEscogido = FindPath(npc.transform.position, wp[0].position, mapaCostes);
                for (int i = 1; i < wp.Count; i++)
                {
                     List<Nodo> camino = FindPath(npc.transform.position, wp[i].position, mapaCostes);
                     if (camino.Count < caminoEscogido.Count)
                     {
                         escogido = wp[i];
                         caminoEscogido = camino;
                     }
                }
                return escogido;
            }
            else //Equipo Azul
            {
                //Busco atacar
                List<Transform> wp = wayPointsRojosSiendoAzul();
                if (wp.Count == 0)
                    return respawnRojo; //Si todos los waypoints estas a 0 o en azules, ve a la base enemiga
                if (wp.Count == 1)
                    return wp[0];
                Transform escogido = wp[0];
                List<Nodo> caminoEscogido = FindPath(npc.transform.position, wp[0].position, mapaCostes);
                for (int i = 1; i < wp.Count; i++)
                {
                    List<Nodo> camino = FindPath(npc.transform.position, wp[i].position, mapaCostes);
                    if (camino.Count < caminoEscogido.Count)
                    {
                        escogido = wp[i];
                        caminoEscogido = camino;
                    }
                }
                return escogido;
            }
        }
    }

    public Transform buscarZonaSegura(NPC npc, float[,] mapaCostes)
    {
        if (npc.tagNPC == "SoldadoLigeroRojo" || npc.tagNPC == "SoldadoPesadoRojo" || npc.tagNPC == "ExploradorRojo" || npc.tag == "ArqueroRojo")
        {
            Transform escogido = zonasSegurasRojo[0];
            List<Nodo> caminoEscogido = FindPath(npc.transform.position, zonasSegurasRojo[0].position, mapaCostes);
            for (int i = 1; i < zonasSegurasRojo.Length; i++)
            {
                List<Nodo> camino = FindPath(npc.transform.position, zonasSegurasRojo[i].position, mapaCostes);
                if (camino.Count < caminoEscogido.Count)
                {
                    escogido = zonasSegurasRojo[i];
                    caminoEscogido = camino;
                }
            }
            return escogido;
        }
        else
        {
            Transform escogido = zonasSegurasAzul[0];
            List<Nodo> caminoEscogido = FindPath(npc.transform.position, zonasSegurasAzul[0].position, mapaCostes);
            for (int i = 1; i < zonasSegurasAzul.Length; i++)
            {
                List<Nodo> camino = FindPath(npc.transform.position, zonasSegurasAzul[i].position, mapaCostes);
                if (camino.Count < caminoEscogido.Count)
                {
                    escogido = zonasSegurasAzul[i];
                    caminoEscogido = camino;
                }
            }
            return escogido;
        }
    }

    public void modoOfensivoAzul() //A parte del patrulla, habrá uno defensivo, y el resto ofensivo
    {
        bool first = false;
        foreach(NPC npc in equipoAzul)
        {
            if (!first)
            {
                if (!npc.patrulla)
                {
                    npc.actitud = 1;
                    npc.cambioActitud();
                    first = true;
                }
            }
            else
            {
                if (!npc.patrulla)
                {
                    npc.actitud = 3;
                    npc.cambioActitud();
                }
            }
        }
        Debug.Log("MODO OFENSIVO para equipo Azul ACTIVADO");
    }

    public void modoOfensivoRojo()
    {
        bool first = false;
        foreach (NPC npc in equipoRojo)
        {
            if (!first)
            {
                if (!npc.patrulla)
                {
                    npc.actitud = 1;
                    npc.cambioActitud();
                    first = true;
                }
            }
            else
            {
                if (!npc.patrulla)
                {
                    npc.actitud = 3;
                    npc.cambioActitud();
                }
            }
        }
        Debug.Log("MODO OFENSIVO para equipo Rojo ACTIVADO");
    }
    public void modoDefensivoAzul() //TODOS defensivo
    {
        foreach(NPC npc in equipoAzul)
        {
            npc.actitud = 1;
            npc.cambioActitud();
        }
        Debug.Log("MODO DEFENSIVO para equipo Azul ACTIVADO");
    }

    public void modoDefensivoRojo()
    {
        foreach (NPC npc in equipoRojo)
        {
            npc.actitud = 1;
            npc.cambioActitud();
        }
        Debug.Log("MODO DEFENSIVO para equipo Rojo ACTIVADO");
    }

    public void modoBatallaFinal()
    {
        bool first = false;
        foreach (NPC npc in equipoAzul)
        {
            if (!first)
            {
                if (!npc.patrulla)
                {
                    npc.actitud = 1;
                    npc.cambioActitud();
                    first = true;
                }
            }
            else
            {
                if (!npc.patrulla)
                {
                    npc.actitud = 3;
                    npc.target = respawnRojo;
                    npc.cambio = true;
                }
            }
        }
        first = false;
        foreach (NPC npc in equipoRojo)
        {
            if (!first)
            {
                if (!npc.patrulla)
                {
                    npc.actitud = 1;
                    npc.cambioActitud();
                    first = true;
                }
            }
            else
            {
                if (!npc.patrulla)
                {
                    npc.actitud = 3;
                    npc.target = respawnAzul;
                    npc.cambio = true;
                }
            }
        }
        Debug.Log("MODO BATALLA FINAL ACTIVADO");
    }

    public void modoReinicio()
    {
        foreach (NPC npc in personajes)
        {
            npc.actitud = npc.actitudInicial;
            npc.cambioActitud();
        }
        
        Debug.Log("MODO REINICIO ACTIVADO");
    }

    public void ganar(string equipo)
    {
        foreach( NPC npc in personajes)
        {
            npc.quedarseQuietoFinJuego();
        }
        if(equipo == "ROJO")
        {
            salidaRojo.SetActive(true);
        }
        else
        {
            salidaAzul.SetActive(true);
        }
    }

    public void salir() { Application.Quit(); }
}
