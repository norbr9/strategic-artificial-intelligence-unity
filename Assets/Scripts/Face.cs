using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face : Align {

    public Kinematic targetOrig;

    public override SteeringOutPut GetSteering(Kinematic character)
    {
        if (targetOrig == null) return new SteeringOutPut();
        Vector3 direction = targetOrig.posicion - character.posicion;
        float distance = direction.magnitude;
        if (distance == 0)
        {
            SteeringOutPut steering = new SteeringOutPut();
            return steering;
        }
        target = targetOrig;
        target.orientacion = Mathf.Atan2(direction.x, direction.z);
        return base.GetSteering(character);
    }

    void OnDrawGizmos()
    {
        // Draws a 5 unit long red line in front of the object
        Gizmos.color = Color.yellow;
        if(targetOrig != null)
            Gizmos.DrawSphere(targetOrig.posicion, 1);
    }
}
