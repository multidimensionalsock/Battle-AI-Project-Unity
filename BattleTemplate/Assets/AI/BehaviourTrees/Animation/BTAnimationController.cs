using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BTAnimationController : MonoBehaviour
{
    Animator m_animator;
    AnimatorOverrideController m_controller;
    public event System.Action AttackAnimFinished;
    NavMeshAgent m_agent;
    // Start is called before the first frame update
    private void OnEnable()
    {
        m_animator = GetComponent<Animator>();
        m_agent = transform.parent.GetComponent<NavMeshAgent>(); 
        m_controller = new AnimatorOverrideController(m_animator.runtimeAnimatorController);
        m_animator.GetComponent<Animator>().runtimeAnimatorController = m_controller;
        transform.parent.GetComponent<CheckConditions>().AttackImplem += AttackAnimation;
    }

    // Update is called once per frame
    void Update()
    {
        //m_animator.SetFloat("YVelocity", m_agent.velocity.y);
        if (m_agent.velocity.x != 0 || m_agent.velocity.z != 0 ) 
        {
            m_animator.SetBool("Moving", true);
        }
        else
        {
            m_animator.SetBool("Moving", false);
        }
    }

    void AttackAnimation(Attack attackData)
    {
        //code to switch in attack anim once this anim is finisghed anim attack finished needs calling 
        m_controller["Attack"] = attackData.associatedAnimation;
        m_animator.SetTrigger("Attack");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            Debug.Log("grounded");
            m_animator.SetBool("Grounded", true);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            Debug.Log("ungrounded");
            m_animator.SetBool("Grounded", false);
        }
    }


}
