using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class NPC : MonoBehaviour {

    public Material materialOriginal;
    public Material materialElegido;
    Renderer renderer;

    public float vidaInicial;
    public float vida;
    public int ataque;
    int ataqueMin;
    public int defensa;
    public string tagNPC;
    public AStarSearch controlador;
    public Transform target;
    Vector3 targetPosicion; //Variable para si el objetivo se ha movido, cambiar el pathfinding

    public int vel;

    float[,] mapaCostes;

    List<Nodo> path;
    Nodo nodoActual; //Nodo actual del camino al que sigo
    public bool cambio;  //Esta variable me servirá para saber si se ha producido cambio de objetivo/recorrido, cambiar el camino (path)
    int numeroCamino; //Llevará la cuenta del camino por el que voy
    bool llego; //Variable para que se quede quieto
    bool camino;

    bool seleccionado;
    public LayerMask hitLayers;

    bool quieto; //Si el NPC está atacando, se queda quieto
    bool finJuego;
    bool esperando;
    bool muerto;  //Esta variable nos serivirá para si estamos muertos y nos habian detectados antes de morir, no quite vida
    bool curando; //Esta variable nos servirá para saber cuando nos hemos curado y tenemos que espera un tiempo para volver a curarnos
    string tagZona; //Esta variable la usaremos cuando estemos quietos para saber donde estamos
    int CTEataqueSuertudo = 3;
    int CTEdefensaSuertudo = 2;

    public int actitudInicial;
    public int actitud; //1 es defensivo, 2 es neutro y 3 es ofensivo

    public Image healthBar;

    public bool patrulla;
    bool pausarPatrulla;
    public Nodo[] caminoPatrulla;
    int numCaminoPatrulla;

    bool disparoLejano;  //Variable para ver quien puede disparar de lejos y quien no

    // Use this for initialization
    void Start () {
        renderer = GetComponent<Renderer>();
        renderer.enabled = true;
        renderer.sharedMaterial = materialOriginal;
        asignarVidaYAtaque();
        tagNPC = transform.tag;
        mapaCostes = controlador.GridReference.obtenerMatrizCostes(tagNPC);
        path = new List<Nodo>();
        cambio = false;
        llego = false;
        camino = false;
        seleccionado = false;
        quieto = false;
        finJuego = false;
        esperando = false;
        muerto = false;
        curando = false;
        tagZona = null;
        numCaminoPatrulla = 0;
        actitudInicial = actitud;
        
        if (patrulla)
        {
            if (transform.tag == "SoldadoLigeroRojo" || transform.tag == "SoldadoPesadoRojo" || transform.tag == "ExploradorRojo" || transform.tag == "ArqueroRojo")
            {
                caminoPatrulla = controlador.crearCaminoPatrullaRojo();
            }
            else
            {
                caminoPatrulla = controlador.crearCaminoPatrullaAzul();
            }
        }
        pausarPatrulla = false;

        if (transform.tag == "ArqueroRojo" || transform.tag == "ArqueroAzul")
            disparoLejano = true;
        else disparoLejano = false;
    }
	
	// Update is called once per frame
	void Update () {
        if ((!esperando) && (!finJuego))
        {
            comprobarVictoria();
            StartCoroutine(comprobarVida());
            if (seleccionado) buscarNuevoTarget();
            StartCoroutine(detectarObjetivo());
            if (!quieto)
            {
                if (!cambio)
                {
                    if (!llego)
                    {
                        if (patrulla)
                        {
                            detectarEnemigoPatrulla();
                            if (pausarPatrulla)  //Ha detectado a alguien, va a por él
                            {
                                if ((nodoActual == null) || (target.position != targetPosicion))
                                {
                                    path = controlador.FindPath(transform.position, target.position, mapaCostes);//Find a path to the target
                                    if (path.Count > 0)
                                    {
                                        nodoActual = path[0];
                                        numeroCamino = 0;
                                        camino = true;
                                        targetPosicion = target.position;
                                    }
                                    else
                                    {
                                        camino = false;
                                    }
                                }
                                if (camino)
                                {
                                    //Ahora nos movemos por el camino
                                    Nodo actual = controlador.GridReference.NodeFromWorldPoint(transform.position);
                                    if (actual == nodoActual) //Avanzo al siguiente nodo del camino
                                    {
                                        numeroCamino++;
                                        if (numeroCamino == path.Count)
                                        {
                                            //Quedate quieto porque has llegado al final del camino
                                            transform.position = transform.position;
                                            llego = true;
                                            //Debug.Log(actitud);
                                            StartCoroutine(buscarNuevoObjetivo());
                                        }
                                        else
                                        {
                                            nodoActual = path[numeroCamino];
                                        }
                                    }
                                    if (numeroCamino < path.Count)
                                    {
                                        //Muevete al siguiente nodo
                                        Vector3 direccion = nodoActual.vPosition - transform.position;
                                        direccion.Normalize();
                                        //Debug.Log(transform.position + " " + direccion + " " + mapaCostes[nodoActual.iGridX, nodoActual.iGridY]);
                                        transform.position = transform.position + direccion * vel * Time.deltaTime / mapaCostes[nodoActual.iGridX, nodoActual.iGridY];
                                    }
                                }
                            }
                            else  //No ha detectado a nadie, patrulla
                            {
                                if (nodoActual == null) //Inicio de la patrulla
                                {
                                    //Debug.Log("kwd " + name);
                                    path = controlador.FindPath(transform.position, caminoPatrulla[numCaminoPatrulla].vPosition, mapaCostes);//Find a path to the target
                                    if (path.Count > 0)
                                    {
                                        nodoActual = path[0];
                                        numeroCamino = 0;
                                        camino = true;
                                        targetPosicion = caminoPatrulla[numCaminoPatrulla].vPosition;
                                    }
                                    else
                                    {
                                        camino = false;
                                    }
                                }
                                if (camino)
                                {
                                    //Ahora nos movemos por el camino
                                    Nodo actual = controlador.GridReference.NodeFromWorldPoint(transform.position);
                                    if (actual == nodoActual) //Avanzo al siguiente nodo del camino
                                    {
                                        numeroCamino++;
                                        if (numeroCamino == path.Count)
                                        {
                                            //Quedate quieto porque has llegado al final del camino
                                            transform.position = transform.position;
                                            nodoActual = null;
                                            numCaminoPatrulla++;
                                            if (numCaminoPatrulla == caminoPatrulla.Length) numCaminoPatrulla = 0; //Si he llegado al final, repetimos
                                        }
                                        else
                                        {
                                            nodoActual = path[numeroCamino];
                                        }
                                    }
                                    if (numeroCamino < path.Count)
                                    {
                                        //Muevete al siguiente nodo
                                        Vector3 direccion = nodoActual.vPosition - transform.position;
                                        direccion.Normalize();
                                        //Debug.Log(transform.position + " " + direccion + " " + mapaCostes[nodoActual.iGridX, nodoActual.iGridY]);
                                        transform.position = transform.position + direccion * vel * Time.deltaTime / mapaCostes[nodoActual.iGridX, nodoActual.iGridY];
                                    }
                                }
                            }
                        }
                        else
                        {
                            if ((nodoActual == null) || (target.position != targetPosicion))
                            {
                                path = controlador.FindPath(transform.position, target.position, mapaCostes);//Find a path to the target
                                if (path.Count > 0)
                                {
                                    nodoActual = path[0];
                                    numeroCamino = 0;
                                    camino = true;
                                    targetPosicion = target.position;
                                }
                                else
                                {
                                    camino = false;
                                }
                            }
                            if (camino)
                            {
                                //Ahora nos movemos por el camino
                                Nodo actual = controlador.GridReference.NodeFromWorldPoint(transform.position);
                                if (actual == nodoActual) //Avanzo al siguiente nodo del camino
                                {
                                    numeroCamino++;
                                    if (numeroCamino == path.Count)
                                    {
                                        //Quedate quieto porque has llegado al final del camino
                                        transform.position = transform.position;
                                        llego = true;
                                        //Debug.Log(actitud);
                                        StartCoroutine(buscarNuevoObjetivo());
                                    }
                                    else
                                    {
                                        nodoActual = path[numeroCamino];
                                    }
                                }
                                if (numeroCamino < path.Count)
                                {
                                    //Muevete al siguiente nodo
                                    Vector3 direccion = nodoActual.vPosition - transform.position;
                                    direccion.Normalize();
                                    //Debug.Log(transform.position + " " + direccion + " " + mapaCostes[nodoActual.iGridX, nodoActual.iGridY]);
                                    transform.position = transform.position + direccion * vel * Time.deltaTime / mapaCostes[nodoActual.iGridX, nodoActual.iGridY];
                                }
                            }
                        }
                    }
                }
                else
                {
                    nodoActual = null;
                    Nodo n = controlador.GridReference.NodeFromWorldPoint(transform.position);
                    tagZona = controlador.getTagNodo(n);
                    cambio = false;
                    llego = false;
                    camino = false;
                }
            }
        }
    }

    IEnumerator comprobarVida()
    {
        if (!finJuego)
        {
            if (vida <= 0)
            {
                Transform m = new GameObject().transform;
                m.position = transform.position;
                target = m; //Su objetivo será ir a donde a muerto
                controlador.moverHaciaSpawn(this);
                cambio = false;
                llego = false;
                camino = true;
                seleccionado = false;
                quieto = false;
                muerto = true;
                esperando = true;
                vida = vidaInicial;
                healthBar.fillAmount = 1;
                yield return new WaitForSeconds(4);
                esperando = false;
                muerto = false;
            }
            else
            {
                if (vida / vidaInicial <= 0.15f) //Que su objetivo sea una zona segura para curarse
                {
                    if ((actitud == 2) && (!quieto) && ((target.tag != "ZonaSeguraAzul") || (target.tag != "ZonaSeguraRojo")))
                    {
                        Debug.Log("Soy " + name + " y voy a una zona segura");
                        target = controlador.buscarZonaSegura(this, mapaCostes);
                    }
                }
                if (!curando)
                {
                    if (transform.tag == "SoldadoLigeroRojo" || transform.tag == "SoldadoPesadoRojo" || transform.tag == "ExploradorRojo" || transform.tag == "ArqueroRojo")
                    {
                        if (nodoActual != null)
                        {
                            if (controlador.getTagNodo(nodoActual) == "ZonaSeguraRojo")
                            {
                                vida += 20;
                                if (vida > vidaInicial) vida = vidaInicial;
                                healthBar.fillAmount = vida / vidaInicial;
                                curando = true;
                                yield return new WaitForSeconds(2);
                                curando = false;
                            }
                        }
                        else
                        {
                            if (tagZona == "ZonaSeguraRojo")
                            {
                                vida += 20;
                                if (vida > vidaInicial) vida = vidaInicial;
                                healthBar.fillAmount = vida / vidaInicial;
                                curando = true;
                                yield return new WaitForSeconds(2);
                                curando = false;
                            }
                        }

                    }
                    else
                    {
                        if (nodoActual != null)
                        {
                            if (controlador.getTagNodo(nodoActual) == "ZonaSeguraAzul")
                            {
                                vida += 20;
                                if (vida > vidaInicial) vida = vidaInicial;
                                healthBar.fillAmount = vida / vidaInicial;
                                curando = true;
                                yield return new WaitForSeconds(2);
                                curando = false;
                            }
                        }
                        else
                        {
                            if (tagZona == "ZonaSeguraAzul")
                            {
                                vida += 20;
                                if (vida > vidaInicial) vida = vidaInicial;
                                healthBar.fillAmount = vida / vidaInicial;
                                curando = true;
                                yield return new WaitForSeconds(2);
                                curando = false;
                            }
                        }
                    }
                }
            }
        }
        
    }

    void asignarVidaYAtaque()
    {
        switch (transform.tag)
        {
            case "SoldadoLigeroAzul":
                vidaInicial = 300;
                vida = vidaInicial;
                ataque = 25;
                ataqueMin = 10;
                defensa = 25;
                break;
            case "SoldadoLigeroRojo":
                vidaInicial = 300;
                vida = vidaInicial;
                ataque = 25;
                ataqueMin = 10;
                defensa = 25;
                break;
            case "SoldadoPesadoAzul":
                vidaInicial = 600;
                vida = vidaInicial;
                ataque = 50;
                ataqueMin = 20;
                defensa = 65;
                break;
            case "SoldadoPesadoRojo":
                vidaInicial = 600;
                vida = vidaInicial;
                ataque = 50;
                ataqueMin = 20;
                defensa = 65;
                break;
            case "ExploradorAzul":
                vidaInicial = 400;
                vida = vidaInicial;
                ataque = 35;
                ataqueMin = 15;
                defensa = 25;
                break;
            case "ExploradorRojo":
                vidaInicial = 400;
                vida = vidaInicial;
                ataque = 35;
                ataqueMin = 15;
                defensa = 25;
                break;
            case "ArqueroAzul":
                vidaInicial = 400;
                vida = vidaInicial;
                ataque = 20;
                ataqueMin = 15;
                defensa = 25;
                break;
            case "ArqueroRojo":
                vidaInicial = 400;
                vida = vidaInicial;
                ataque = 20;
                ataqueMin = 15;
                defensa = 25;
                break;
        }
    }

    void detectarEnemigoPatrulla()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position,40);
        int i = 0;
        bool detectado = false;
        while (i < hitColliders.Length)
        {
            if (transform.tag == "SoldadoLigeroRojo" || transform.tag == "SoldadoPesadoRojo" || transform.tag == "ExploradorRojo" || transform.tag == "ArqueroRojo")
            {
                if (hitColliders[i].tag == "SoldadoLigeroAzul" || hitColliders[i].tag == "SoldadoPesadoAzul" || hitColliders[i].tag == "ExploradorAzul" || transform.tag == "ArqueroAzul")
                {
                    detectado = true;
                    NPC enemigo = controlador.devolverEnemigo(hitColliders[i].transform);
                    if (target != enemigo.transform)
                    {
                        target = enemigo.transform;
                        targetPosicion = enemigo.transform.position;
                        nodoActual = null;
                    }
                    pausarPatrulla = true;
                    break;    //El primero que encuentre será su objetivo
                }
                i++;
            }
            else
            {
                if (hitColliders[i].tag == "SoldadoLigeroRojo" || hitColliders[i].tag == "SoldadoPesadoRojo" || hitColliders[i].tag == "ExploradorRojo" || transform.tag == "ArqueroRojo")
                {
                    detectado = true;
                    NPC enemigo = controlador.devolverEnemigo(hitColliders[i].transform);
                    if(target != enemigo.transform)
                    {
                        target = enemigo.transform;
                        targetPosicion = enemigo.transform.position;
                        nodoActual = null;
                    }
                    pausarPatrulla = true;
                    break;    //El primero que encuentre será su objetivo
                }
                i++;
            }
        }
        if (!detectado)
        {
            if (pausarPatrulla)
            {
                pausarPatrulla = false; //Si no se ha detectado a nadie, entonces por si se estaba quieto, que ya no lo este
                nodoActual = null;
                numCaminoPatrulla = 0;
            }
        }
}

    IEnumerator detectarObjetivo()
    {
        if (!finJuego)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 8);
            int i = 0;
            bool detectado = false;
            while (i < hitColliders.Length)
            {
                if (transform.tag == "SoldadoLigeroRojo" || transform.tag == "SoldadoPesadoRojo" || transform.tag == "ExploradorRojo" || transform.tag == "ArqueroRojo")
                {
                    if (hitColliders[i].tag == "SoldadoLigeroAzul" || hitColliders[i].tag == "SoldadoPesadoAzul" || hitColliders[i].tag == "ExploradorAzul" || transform.tag == "ArqueroAzul")
                    {
                        NPC enemigo = controlador.devolverEnemigo(hitColliders[i].transform);
                        if (enemigo.vida > 0)
                        {
                            quieto = true;
                            atacar(enemigo);
                            esperando = true;
                            detectado = true;
                            if (seleccionado)
                            {
                                renderer.sharedMaterial = materialOriginal;
                                controlador.ocupadoSeleccionando = false;
                                seleccionado = false;

                            }
                            yield return new WaitForSeconds(2);
                            esperando = false;

                        }
                    }

                    i++;
                }
                else
                {
                    if (hitColliders[i].tag == "SoldadoLigeroRojo" || hitColliders[i].tag == "SoldadoPesadoRojo" || hitColliders[i].tag == "ExploradorRojo" || transform.tag == "ArqueroRojo")
                    {
                        NPC enemigo = controlador.devolverEnemigo(hitColliders[i].transform);
                        if (enemigo.vida > 0)
                        {
                            quieto = true;
                            atacar(enemigo);
                            esperando = true;
                            detectado = true;
                            if (seleccionado)
                            {
                                renderer.sharedMaterial = materialOriginal;
                                controlador.ocupadoSeleccionando = false;
                                seleccionado = false;

                            }
                            yield return new WaitForSeconds(2);
                            esperando = false;
                        }
                    }
                    i++;
                }
            }
            if (!detectado && disparoLejano) //Si no ha detectado a nadie y puede disparar de lejos, busca objetivos
            {
                hitColliders = Physics.OverlapSphere(transform.position, 40);
                i = 0;
                while (i < hitColliders.Length)
                {
                    if (transform.tag == "ArqueroRojo")
                    {
                        if (hitColliders[i].tag == "SoldadoLigeroAzul" || hitColliders[i].tag == "SoldadoPesadoAzul" || hitColliders[i].tag == "ExploradorAzul" || transform.tag == "ArqueroAzul")
                        {
                            NPC enemigo = controlador.devolverEnemigo(hitColliders[i].transform);
                            if (enemigo.vida > 0)
                            {
                                quieto = true;
                                atacar(enemigo);
                                esperando = true;
                                detectado = true;
                                if (seleccionado)
                                {
                                    controlador.ocupadoSeleccionando = false;
                                    seleccionado = false;
                                }
                                renderer.sharedMaterial = materialElegido;
                                yield return new WaitForSeconds(2);
                                renderer.sharedMaterial = materialOriginal;
                                esperando = false;

                            }
                        }

                        i++;
                    }
                    else   //entonces será un ArqueroAzul
                    {
                        if (hitColliders[i].tag == "SoldadoLigeroRojo" || hitColliders[i].tag == "SoldadoPesadoRojo" || hitColliders[i].tag == "ExploradorRojo" || transform.tag == "ArqueroRojo")
                        {
                            NPC enemigo = controlador.devolverEnemigo(hitColliders[i].transform);
                            if (enemigo.vida > 0)
                            {
                                quieto = true;
                                atacar(enemigo);
                                esperando = true;
                                detectado = true;
                                if (seleccionado)
                                {
                                    renderer.sharedMaterial = materialOriginal;
                                    controlador.ocupadoSeleccionando = false;
                                    seleccionado = false;
                                }
                                renderer.sharedMaterial = materialElegido;
                                yield return new WaitForSeconds(2);
                                renderer.sharedMaterial = materialOriginal;
                                esperando = false;
                            }
                        }
                        i++;
                    }
                }
            }
            if (!detectado) quieto = false; //Si no se ha detectado a nadie, entonces por si se estaba quieto, que ya no lo este
        }
    }

    

    void atacar(NPC enemigo)
    {
        //Calculo la fuerza para atacar
        float ataqueNPC = 0;
        int rand = Random.Range(0, 100);
        if(rand > 90) 
        {
            //Aplicar ataque suertudo
            ataqueNPC = ataque * CTEataqueSuertudo;
        }
        else
        {
            //Fuerza del atacante= Ataque*FAD*FTA
            ataqueNPC = ataque * calcularFAD(enemigo) * calcularFTA();
        }
        //Le ataco
        enemigo.serAtacado(ataqueNPC, ataqueMin); 
    }

    void atacarLejano(NPC enemigo)
    {
        //Calculo la fuerza para atacar
        float ataqueNPC = 0;
        float fuerzaTiro = 35;
        int rand = Random.Range(0, 100);
        if (rand > 90)
        {
            //Aplicar ataque suertudo
            ataqueNPC = fuerzaTiro * CTEataqueSuertudo;
        }
        else
        {
            //Fuerza del atacante= Ataque*FAD*FTA
            ataqueNPC = fuerzaTiro * calcularFADTiroLejano(enemigo) * calcularFTA();
        }
        //Le ataco
        enemigo.serAtacado(ataqueNPC, ataqueMin);
    }

    void serAtacado(float ataqueEnemigo, int ataqueMinimo)
    {
        if (!muerto)
        {
            //Calculo la fuerza para defender
            float defensaNPC = 0;
            int rand = Random.Range(0, 100);
            if (rand > 90)
            {
                //Aplicar defensa suertudo
                defensaNPC = defensa * CTEdefensaSuertudo;
            }
            else
            {
                //Fuerza del defensor = Calidad * FTD
                defensaNPC = defensa * calcularFTD();
            }
            //Recibo el golpe
            float resultado = defensaNPC - ataqueEnemigo;
            
            if (resultado < 0) //Si resulta que el ataque ha sido superior vida, quitarle vida, sino, no hacer nada
            {
                vida += resultado;
                healthBar.fillAmount = vida / vidaInicial;
            }
            else //Sino, aplicar ataque minimo
            {
                vida -= ataqueMinimo;
                healthBar.fillAmount = vida / vidaInicial;
            }
        }
    }

    float calcularFADTiroLejano(NPC enemigo) //Factor de tipo de atacante/defensor para el tiro lejano (FAD)
    {
        float fad = 0;
        if (transform.tag == "ArqueroRojo" || transform.tag == "ArqueroAzul")
        {
            if (enemigo.tag == "ArqueroRojo" || enemigo.tag == "ArqueroAzul")
            {
                fad = 1;
            }
            else fad = 1.5f;
        }
        return fad;
    }

        float calcularFAD(NPC enemigo) //Factor de tipo de atacante/defensor (FAD)
    {
        float fad = 0;
        if(transform.tag == "SoldadoLigeroRojo" || transform.tag == "SoldadoLigeroAzul")
        {
            if (enemigo.tag == "SoldadoLigeroRojo" || enemigo.tag == "SoldadoLigeroAzul")
            {
                fad = 1;
            }
            else if (enemigo.tag == "SoldadoPesadoRojo" || enemigo.tag == "SoldadoPesadoAzul")
            {
                fad = 0.75f;
            }
            else if (enemigo.tag == "ArqueroRojo" || enemigo.tag == "ArqueroAzul")
            {
                fad = 1.75f;
            }
            else //Será entonces explorador
            {
                fad = 1.5f;
            }


        } else if(transform.tag == "SoldadoPesadoRojo" || transform.tag == "SoldadoPesadoAzul")
        {
            if (enemigo.tag == "SoldadoLigeroRojo" || enemigo.tag == "SoldadoLigeroAzul")
            {
                fad = 1.5f;
            }
            else if (enemigo.tag == "SoldadoPesadoRojo" || enemigo.tag == "SoldadoPesadoAzul")
            {
                fad = 1;
            }
            else if (enemigo.tag == "ArqueroRojo" || enemigo.tag == "ArqueroAzul")
            {
                fad = 1.75f;
            }
            else //Será entonces explorador
            {
                fad = 0.75f;
            }
        }
        else if (transform.tag == "ArqueroRojo" || transform.tag == "ArqueroAzul")
        {
            if (enemigo.tag == "SoldadoLigeroRojo" || enemigo.tag == "SoldadoLigeroAzul")
            {
                fad = 1;
            }
            else if (enemigo.tag == "SoldadoPesadoRojo" || enemigo.tag == "SoldadoPesadoAzul")
            {
                fad = 0.75f;
            }
            else if (enemigo.tag == "ArqueroRojo" || enemigo.tag == "ArqueroAzul")
            {
                fad = 1;
            }
            else //Será entonces explorador
            {
                fad = 1;
            }
        }
        else   //Será entonces explorador
        {
            if (enemigo.tag == "SoldadoLigeroRojo" || enemigo.tag == "SoldadoLigeroAzul")
            {
                fad = 0.75f;
            }
            else if (enemigo.tag == "SoldadoPesadoRojo" || enemigo.tag == "SoldadoPesadoAzul")
            {
                fad = 1.5f;
            }
            else if (enemigo.tag == "ArqueroRojo" || enemigo.tag == "ArqueroAzul")
            {
                fad = 1.5f;
            }
            else //Será entonces explorador
            {
                fad = 1;
            }
        }

        return fad;
    }

    float calcularFTA() //Factor por terreno del atacante (FTA)
    {
        float fta = 0;
        string terreno;
        if (nodoActual == null) terreno = tagZona;
        else terreno = controlador.getTagNodo(nodoActual);
        if (transform.tag == "SoldadoLigeroRojo" || transform.tag == "SoldadoLigeroAzul")
        {
            switch (terreno)
            {
                case "Carretera":
                    fta = 2;
                    break;
                case "Puente":
                    fta = 2;
                    break;
                case "Hierba":
                    fta = 1;
                    break;
                case "Bosque":
                    fta = 0.75f;
                    break;
                case "BaseAzul":
                    fta = 1.5f;
                    break;
                case "BaseRoja":
                    fta = 1.5f;
                    break;
                case "ZonaSeguraAzul":
                    fta = 1.5f;
                    break;
                case "ZonaSeguraRoja":
                    fta = 1.5f;
                    break;
            }
        }
        else if (transform.tag == "SoldadoPesadoRojo" || transform.tag == "SoldadoPesadoAzul")
        {
            switch (terreno)
            {
                case "Carretera":
                    fta = 1.25f;
                    break;
                case "Puente":
                    fta = 1.25f;
                    break;
                case "Hierba":
                    fta = 1.25f;
                    break;
                case "Bosque":
                    fta = 0.75f;
                    break;
                case "BaseAzul":
                    fta = 1.5f;
                    break;
                case "BaseRoja":
                    fta = 1.5f;
                    break;
                case "ZonaSeguraAzul":
                    fta = 1.5f;
                    break;
                case "ZonaSeguraRoja":
                    fta = 1.5f;
                    break;
            }
        }
        else if (transform.tag == "ArqueroRojo" || transform.tag == "ArqueroAzul")
        {
            switch (terreno)
            {
                case "Carretera":
                    fta = 2;
                    break;
                case "Puente":
                    fta = 2;
                    break;
                case "Hierba":
                    fta = 1.5f;
                    break;
                case "Bosque":
                    fta = 0.75f;
                    break;
                case "BaseAzul":
                    fta = 1.5f;
                    break;
                case "BaseRoja":
                    fta = 1.5f;
                    break;
                case "ZonaSeguraAzul":
                    fta = 1.5f;
                    break;
                case "ZonaSeguraRoja":
                    fta = 1.5f;
                    break;
            }
        }
        else   //Será entonces explorador
        {
            switch (terreno)
            {
                case "Carretera":
                    fta = 0.75f;
                    break;
                case "Puente":
                    fta = 0.75f;
                    break;
                case "Hierba":
                    fta = 1;
                    break;
                case "Bosque":
                    fta = 2;
                    break;
                case "BaseAzul":
                    fta = 1.5f;
                    break;
                case "BaseRoja":
                    fta = 1.5f;
                    break;
                case "ZonaSeguraAzul":
                    fta = 1.5f;
                    break;
                case "ZonaSeguraRoja":
                    fta = 1.5f;
                    break;
            }
        }
        return fta;    
    }

    float calcularFTD()  //Factor por terreno del defensor (FTD)
    {
        float ftd = 0;
        string terreno;
        if (nodoActual == null) terreno = tagZona;
        else terreno = controlador.getTagNodo(nodoActual);
        if (transform.tag == "SoldadoLigeroRojo" || transform.tag == "SoldadoLigeroAzul")
        {
            switch (terreno)
            {
                case "Carretera":
                    ftd = 1;
                    break;
                case "Puente":
                    ftd = 1;
                    break;
                case "Hierba":
                    ftd = 0.75f;
                    break;
                case "Bosque":
                    ftd = 0.5f;
                    break;
                case "BaseAzul":
                    ftd = 1;
                    break;
                case "BaseRoja":
                    ftd = 1;
                    break;
                case "ZonaSeguraAzul":
                    ftd = 1;
                    break;
                case "ZonaSeguraRoja":
                    ftd = 1;
                    break;
            }
        }
        else if (transform.tag == "SoldadoPesadoRojo" || transform.tag == "SoldadoPesadoAzul")
        {
            switch (terreno)
            {
                case "Carretera":
                    ftd = 1;
                    break;
                case "Puente":
                    ftd = 1;
                    break;
                case "Hierba":
                    ftd = 1;
                    break;
                case "Bosque":
                    ftd = 0.5f;
                    break;
                case "BaseAzul":
                    ftd = 1;
                    break;
                case "BaseRoja":
                    ftd = 1;
                    break;
                case "ZonaSeguraAzul":
                    ftd = 1;
                    break;
                case "ZonaSeguraRoja":
                    ftd = 1;
                    break;
            }
        }
        else if (transform.tag == "ArqueroRojo" || transform.tag == "ArqueroAzul")
        {
            switch (terreno)
            {
                case "Carretera":
                    ftd = 1;
                    break;
                case "Puente":
                    ftd = 1;
                    break;
                case "Hierba":
                    ftd = 0.75f;
                    break;
                case "Bosque":
                    ftd = 0.5f;
                    break;
                case "BaseAzul":
                    ftd = 1;
                    break;
                case "BaseRoja":
                    ftd = 1;
                    break;
                case "ZonaSeguraAzul":
                    ftd = 1;
                    break;
                case "ZonaSeguraRoja":
                    ftd = 1;
                    break;
            }
        }
        else   //Será entonces explorador
        {
            switch (terreno)
            {
                case "Carretera":
                    ftd = 0.5f;
                    break;
                case "Puente":
                    ftd = 0.5f;
                    break;
                case "Hierba":
                    ftd = 0.75f;
                    break;
                case "Bosque":
                    ftd = 1;
                    break;
                case "BaseAzul":
                    ftd = 1;
                    break;
                case "BaseRoja":
                    ftd = 1;
                    break;
                case "ZonaSeguraAzul":
                    ftd = 1;
                    break;
                case "ZonaSeguraRoja":
                    ftd = 1;
                    break;
            }
        }
        return ftd;
    }

    private void OnMouseOver()
    {
        if ((!controlador.ocupadoSeleccionando) && (!finJuego))//Con esto evitamos que más de un NPC esté seleccionado
        {
            if (!esperando)
            {
                if (!patrulla)  //Si es una patrulla, no podrá ser seleccionado
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        renderer.sharedMaterial = materialElegido;
                        controlador.ocupadoSeleccionando = true;
                        seleccionado = true;
                    }
                }
            }
        }
    }

    void buscarNuevoTarget()
    {
        if (!finJuego)
        {
            if (Input.GetMouseButtonDown(0))//Con esta acción seleccionamos un nuevo objetivo para el target
            {
                Vector3 mouse = Input.mousePosition;
                Ray castPoint = Camera.main.ScreenPointToRay(mouse);
                RaycastHit hit;
                if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, hitLayers)) //Si el raycast no golpea en una pared o en el agua
                {
                    if (hit.transform != this.transform) //Para evitar ser autoseleccionado
                    {
                        target = hit.transform;
                        cambio = true;
                        renderer.sharedMaterial = materialOriginal;
                        controlador.ocupadoSeleccionando = false;
                        seleccionado = false;
                    }
                }
            }

            if (Input.GetMouseButtonDown(1)) //Con esta acción salimos sin seleccionar ningún objetivo nuevo
            {
                renderer.sharedMaterial = materialOriginal;
                controlador.ocupadoSeleccionando = false;
                seleccionado = false;
            }
        }
    }

    IEnumerator buscarNuevoObjetivo()
    {
        yield return new WaitForSeconds(4);
        //Buscar nuevo objetivo
        target = controlador.buscarNuevoObjetivo(this, mapaCostes);
        cambio = true;
    }
    public void cambioActitud()
    {
        target = controlador.buscarNuevoObjetivo(this, mapaCostes);
        cambio = true;
    }

    public void quedarseQuietoFinJuego()
    {
        quieto = true;
        finJuego = true;
    }

    void comprobarVictoria()
    {
        if (transform.tag == "SoldadoLigeroRojo" || transform.tag == "SoldadoPesadoRojo" || transform.tag == "ExploradorRojo" || transform.tag == "ArqueroRojo")
        {
            if(nodoActual == null)
            {
                if (tagZona == "BaseAzul")
                    controlador.ganar("ROJO");
            } else
            {
                if (controlador.getTagNodo(nodoActual) == "BaseAzul")
                    controlador.ganar("ROJO");
            }
        } else
        {
            if (nodoActual == null)
            {
                if (tagZona == "BaseRoja")
                    controlador.ganar("AZUL");
            }
            else
            {
                if (controlador.getTagNodo(nodoActual) == "BaseRoja")
                    controlador.ganar("AZUL");
            }
        }
    }
    
    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        /*if (disparoLejano)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 40);
        }*/
        
    }
}
