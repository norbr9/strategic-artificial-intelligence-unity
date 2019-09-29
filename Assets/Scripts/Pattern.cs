using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pattern
{
    public int NumberOfSlots { get; set; }
    public Kinematic Lider { get; set; }
    public List<Kinematic> Personajes { get; set; }

    public Pattern(Kinematic lider, List<Kinematic> personajes)
    {
        Lider = lider;
        Personajes = personajes;

    }

    // Obtiene la ubicacion de la ranura dada
    public abstract DriftOffset getSlotLocation(int slotNumber);

    public abstract int calculateNumberOfSlots(List<SlotAssignment> assignments);

    public abstract bool supportsSlots(int slotCount);

   

}