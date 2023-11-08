using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAIAnimation : MonoBehaviour
{
    Rigidbody m_rigidBody;
    Animator m_animator;
    AnimatorOverrideController m_overrideController;
    int currentCollisions = 0;
    Vector3 lastPos;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidBody = GetComponentInParent<Rigidbody>();
        m_animator = GetComponent<Animator>();
        m_overrideController = new AnimatorOverrideController(m_animator.runtimeAnimatorController);
        m_animator.runtimeAnimatorController = m_overrideController;
        lastPos = transform.position;
        gameObject.GetComponentInParent<Pathfinding>().callAttack += AttackAnimation;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 distance = transform.position - lastPos;
        if (Mathf.Abs(distance.x) > 0.01f || Mathf.Abs( distance.z) > 0.01f)//currenrly animatin now working even though it is running the insides
        {
            m_animator.SetBool("Moving", true);
        }
        else
        {
            m_animator.SetBool("Moving", false);
        }
        m_animator.SetFloat("YMovement", m_rigidBody.velocity.y);
        lastPos = transform.position;
    }

    void AttackAnimation(Attack attack)
    {
        m_overrideController["attack"] = attack.associatedAnimation; //set attack aniamtion to current attacks animation 
        m_animator.SetTrigger("Attack");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (currentCollisions < 0) { currentCollisions = 0; }
        currentCollisions++;
        m_animator.SetBool("grounded", true);
    }

    private void OnCollisionExit(Collision collision)
    {
        currentCollisions--;
        if (currentCollisions <= 0) 
        {
            m_animator.SetBool("grounded", false);
            if (currentCollisions < 0 ) { currentCollisions = 0; }
        }
    }

}
