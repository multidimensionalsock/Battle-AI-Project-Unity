using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BTAnimationController : MonoBehaviour
{
    Animator m_animator;
    public event System.Action AttackAnimFinished;
    NavMeshAgent m_agent;
    // Start is called before the first frame update
    private void OnEnable()
    {
		m_agent = transform.parent.GetComponent<NavMeshAgent>();
		m_animator = GetComponent<Animator>();
        transform.parent.GetComponent<CheckConditions>().AttackImplem += AttackAnimation;
        transform.parent.GetComponent<CheckConditions>().waitModeOnOff += WaitMode;
    }

    // Update is called once per frame
    void Update()
    {
		transform.rotation = transform.parent.transform.rotation;
        transform.position = transform.parent.transform.position;

        if (m_agent.velocity != Vector3.zero) 
        {
            m_animator.SetBool("Moving", true);
        }
        else
        {
            m_animator.SetBool("Moving", false);
        }
        if (m_agent.velocity.y == 0)
        {
            m_animator.SetBool("Grounded", true);
        }
    }

    void AttackAnimation(Attack attackData)
    {
        m_animator.SetFloat("AttackNumber", attackData.AnimationNumber);
        m_animator.SetTrigger("Attack");
        
    }

    void WaitMode(bool mode)
    {
        m_animator.SetBool("Wait", mode);
    }

    

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.tag == "Floor")
        {
            m_animator.SetBool("Grounded", true);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            m_animator.SetBool("Grounded", false);
        }
    }


}
