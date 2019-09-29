using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kinematic : MonoBehaviour{

    public Vector3 posicion;
    public float orientacion;

    public float maxSpeed;
    public float maxAceleration;
    public float maxAcelerationAngular;
    public float maxRotation;
    public Vector3 velocidadActual; //la componente "y" vale 0
    public float rotacionActual;

    //Esto es para el pathfinding
    public List<Vector3> listPuntos = new List<Vector3>();

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void iniciar(Vector3 posicionPer)
    {
        posicion = posicionPer;
        orientacion = 0;
        maxRotation = maxRotation * Mathf.Deg2Rad;
        
    }

    public void actualizarPosicion(Vector3 posicionPer)
    {
        posicion = posicionPer;
    }

    public void actualizarOrientacion(float orientacionPer)
    {
        orientacion = orientacionPer;
    }

    public float nuevaOrientacion(float orientacion, Vector3 velocidad)
    {
        if (velocidad.magnitude > 0)
            return Mathf.Atan2(velocidad.z, velocidad.x);
        else
            return orientacion;
    }

    public void setSteering(SteeringOutPut str)
    {
        if(str.vector.magnitude > 0)
        {
            velocidadActual = velocidadActual + str.vector * Time.deltaTime;   //str.vector = aceleracion
            posicion = posicion + velocidadActual * Time.deltaTime;
        }
        else
        {
            velocidadActual -= velocidadActual * Time.deltaTime;   //str.vector = aceleracion
            posicion = posicion + velocidadActual * Time.deltaTime;
        }
        if(str.real > 0)
        {
            rotacionActual = rotacionActual + str.real * Time.deltaTime; //str.real = aceleracion angular
            orientacion = orientacion + rotacionActual * Time.deltaTime;
        }
        else
        {
            rotacionActual -= rotacionActual * Time.deltaTime; //str.real = aceleracion angular
            orientacion = orientacion + rotacionActual * Time.deltaTime;
        }
        transform.position = posicion;
        transform.rotation = Quaternion.identity;
        //ajustar con los valores que yo quiera a Unity (de lo que quiero a Unity) 
        float grados = orientacion * Mathf.Rad2Deg;
        transform.Rotate(Vector3.up, grados); //rota con respecto al eje y, trabaja en grados
    }


}
