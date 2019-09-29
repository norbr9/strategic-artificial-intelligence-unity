using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerFivePattern : Pattern
{

    DriftOffset[] PosicionesPattern = new DriftOffset[4];

    public FingerFivePattern(Kinematic lider, List<Kinematic> personajes) : base (lider, personajes)
    {

        PosicionesPattern[0] = new DriftOffset();
        PosicionesPattern[0].Posicion = new Vector3(-2f, 0f, -2f);
        PosicionesPattern[0].Orientacion = 0;

        PosicionesPattern[1] = new DriftOffset();
        PosicionesPattern[1].Posicion = new Vector3(2f, 0f, -2f);
        PosicionesPattern[1].Orientacion = 0;

        PosicionesPattern[2] = new DriftOffset();
        PosicionesPattern[2].Posicion = new Vector3(-4f, 0f, -4f);
        PosicionesPattern[2].Orientacion = 0;

        PosicionesPattern[3] = new DriftOffset();
        PosicionesPattern[3].Posicion = new Vector3(4f, 0f, -4f);
        PosicionesPattern[3].Orientacion = 0;

    }

    public override int calculateNumberOfSlots(List<SlotAssignment> assignments)
    {
        int filledSlots = 0;
        int maxSlotNumber = 0;

        foreach (SlotAssignment assignment in assignments)
        {
            if (assignment.SlotNumber >= maxSlotNumber)
            {
                maxSlotNumber = assignment.SlotNumber;
                filledSlots = assignment.SlotNumber;
            }
        }

        NumberOfSlots = filledSlots + 1;

        return NumberOfSlots;
    }

    public override DriftOffset getSlotLocation(int slotNumber)
    {
        DriftOffset location = new DriftOffset();

        Vector3 newPosition = new Vector3(PosicionesPattern[slotNumber].Posicion.x, PosicionesPattern[slotNumber].Posicion.y, PosicionesPattern[slotNumber].Posicion.z);
        location.Posicion = new Vector3(Lider.posicion.x + newPosition.x, Lider.posicion.y, Lider.posicion.z + newPosition.z);

        location.Orientacion = Lider.orientacion;
        
        return location;
    }

    public override bool supportsSlots(int slotCount)
    {
        if (5 >= slotCount)
            return true;
        else return false;

    }
}
