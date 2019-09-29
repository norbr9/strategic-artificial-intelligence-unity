using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationManager : MonoBehaviour
{
    [SerializeField]
    private List<Kinematic> personajes;

    [SerializeField]
    private Kinematic lider;

    public int formation;
    public Pattern pattern;
    public List<SlotAssignment> SlotAssignments;

    public void Awake()
    {
        SlotAssignments = new List<SlotAssignment>();
        if (formation == 0)
            pattern = new DiamondPattern(GetLider(), GetPersonajes());
        else if (formation == 1) pattern = new FingerFivePattern(GetLider(), GetPersonajes());
        updateSlotAssignments();
    }

    void Update()
    {
        updateSlots();
    }

    public List<Kinematic> GetPersonajes()
    {
        return personajes;
    }

    public void SetPersonajes(List<Kinematic> value)
    {
        personajes = value;
    }

    public Kinematic GetLider()
    {
        return lider;
    }

    public void SetLider(Kinematic value)
    {
        lider = value;
    }

    public SlotAssignment getSlotByNumber(int slot)
    {
        return SlotAssignments[slot];
    }


    public void updateSlotAssignments()
    {
        for (int i = 0; i < GetPersonajes().Count; i++)
        {
            SlotAssignment slot = new SlotAssignment(GetPersonajes()[i], i);

            SlotAssignments.Add(slot);
        }
    }

    public DriftOffset getSlotByCharacter(Kinematic character)
    {
        for (int i = 0; i < SlotAssignments.Count; i++)
        {
            if (SlotAssignments[i].Character.Equals(character))
                return SlotAssignments[i].Location;
        }
        return null;
    }

    public void updateSlots()
    {
        if (GetPersonajes().Count == 0 || GetLider() == null) return;

        for (int i = 0; i < SlotAssignments.Count; i++)
        {
            SlotAssignments[i].Location = pattern.getSlotLocation(i);
        }
    }


}
