using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//public enum MiniEnemyStates
//{
//    Idle, //default state
//    Seek, //Seek towards player
//    Wander, //wander randomly when not in players versinity
//    Attack, //attack the player (only mellee when in range)
//    Attacked,
//    Defend, //go around main enemy and block player path to prevent them being attacked
//    Death
//}


public class MiniEnemyAnim : MonoBehaviour
{
    Animator m_animator;
    NavMeshAgent m_agent;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MiniEnemyFinite>().StateChange += SetAnimation;
        m_animator = GetComponent<Animator>();
        m_agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (m_agent.velocity == Vector3.zero) //m agent is null causing this issue 
        {
            m_animator.SetBool("Moving", false);
        }
        else
        {
            m_animator.SetBool("Moving", true); //this var isnt being set to true 
        }
    }

    void SetAnimation(MiniEnemyStates newState)
    {
        switch(newState)
        {
            case MiniEnemyStates.Attack:
                m_animator.SetTrigger("Attack");
                break;
            case MiniEnemyStates.Attacked:
                m_animator.SetTrigger("Hit");
                break;
            case MiniEnemyStates.Death:
                m_animator.SetTrigger("Death");
                break;
        }
    }
}
