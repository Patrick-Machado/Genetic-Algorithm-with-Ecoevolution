using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public CreatureAI CreatureCandidate;
    public  bool TryCatchThisFood(CreatureAI candidate)
    {
        if(CreatureCandidate == null) { CreatureCandidate = candidate; return true; }
        if (CreatureCandidate != null)
        { if(CreatureCandidate.Size > candidate.Size)
          { return false; }
          else { CreatureCandidate = candidate; return true; }
        }
        Debug.Log("Some Error Happened");
        return false;
    }
}
