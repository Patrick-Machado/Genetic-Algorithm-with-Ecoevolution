using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Movement : MonoBehaviour
{
    public Ecosystem gridpoints;
    NavMeshAgent agent;
    Rigidbody rb;
    public bool imOn = false;
    enum State
    {
        random, persue, stoped
    }
    State myState = State.random;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }
    public void Move(Transform t)
    {
        agent.destination = t.position;
    }
    public void StopAgent()
    {
        agent.isStopped=true; ChangeState(2);
    }
    public void SetSpeed(float value)
    {
        agent.speed = value;
    }
    public void ChangeState(int state)
    {
        if (state == 0) myState = State.random;
        if (state == 1) myState = State.persue;
        if (state == 2) myState = State.stoped;
    }
    void FixedUpdate()
    {
        if(imOn && myState == State.random && rb.velocity == Vector3.zero)
        {
            int aim = Random.Range(0, gridpoints.GridList.Count - 1);
            Move(gridpoints.GridList[aim]);
        }
        
    }

}
