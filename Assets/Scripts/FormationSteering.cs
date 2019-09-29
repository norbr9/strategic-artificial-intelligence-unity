
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationSteering : BlendedSteering
{

    [SerializeField]
    private FormationManager formation;
    public int slot;

    SlotAssignment slotAs;

    public Arrive arrive;
    public Align align;

    GameObject objetivo;
    Kinematic kinetic;
    void Start()
    {
        slotAs = formation.getSlotByNumber(slot);
        objetivo = new GameObject(); //Creo el objetivo
        objetivo.AddComponent<Kinematic>();

        kinetic = objetivo.GetComponent<Kinematic>();
    }

    public override SteeringOutPut GetSteering(Kinematic character)
    {
        slotAs = formation.getSlotByNumber(slot);

        kinetic.posicion = slotAs.Location.Posicion;
        kinetic.orientacion = slotAs.Location.Orientacion;
        arrive.target = kinetic;
        align.target = kinetic;
        return base.GetSteering(character);
    }
}

