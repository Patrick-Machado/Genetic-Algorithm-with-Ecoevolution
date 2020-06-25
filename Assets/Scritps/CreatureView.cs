using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureView : MonoBehaviour
{
    public CreatureAI brain;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Food"))
        {
            Food f = other.gameObject.GetComponent<Food>();
            if (f.TryCatchThisFood(brain))
            {
                if (brain.movement != null)
                    brain.movement.Move(f.transform);
                brain.movement.ChangeState(1);
                //eat method on CreatureAI
            }
        }
        if (other.gameObject.CompareTag("Creature"))
        {
            if (brain.movement == null) return;
            if (brain.movement.imOn == false) return;
            CreatureAI f = other.gameObject.GetComponent<CreatureAI>();

                if (brain.movement != null)
                    brain.movement.Move(f.transform);
                brain.movement.ChangeState(1);
                //eat method on CreatureAI
            
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (brain.movement == null) return;
        if (brain.movement.imOn == false) return;
        if (other.gameObject.CompareTag("Food"))
        {
            Food f = other.gameObject.GetComponent<Food>();
            if (f.TryCatchThisFood(brain))
            {
                if (brain.movement != null)
                    brain.movement.Move(f.transform);
                brain.movement.ChangeState(1);
                //eat method on CreatureAI
            }
            if (other.gameObject.CompareTag("Creature"))
            {
                if (brain.movement == null) return;
                if (brain.movement.imOn == false) return;
                CreatureAI c = other.gameObject.GetComponent<CreatureAI>();

                if (brain.movement != null)
                    brain.movement.Move(c.transform);
                brain.movement.ChangeState(1);
                //eat method on CreatureAI

            }
        }
    }
    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, transform.lossyScale.x/2);
    }
}
