using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotAssignment
{
    public int SlotNumber { get; set; }
    public Kinematic Character { get; set; }
    public DriftOffset Location { get; set; }

    public SlotAssignment(Kinematic personaje, int slot)
    {
        SlotNumber = slot;
        Character = personaje;
        Location = new DriftOffset();
    }
}
