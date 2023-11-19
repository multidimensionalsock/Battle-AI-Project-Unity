using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    Rigidbody m_rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MiniEnemyFSM>().StateChange += SetAnimation;
        m_animator = GetComponent<Animator>();
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Mathf.Abs( m_rigidbody.velocity.x) == 0 || Mathf.Abs(m_rigidbody.velocity.z) == 0)
        {
            m_animator.SetBool("Moving", false);
        }
        else
        {
            m_animator.SetBool("Moving", true);
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
