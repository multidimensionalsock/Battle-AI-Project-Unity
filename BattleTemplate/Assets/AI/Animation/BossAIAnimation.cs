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
    }

    // Update is called once per frame
    void Update()
    {
        if (lastPos != transform.position)
        {
            Debug.Log("move");
            lastPos = transform.position;
            m_animator.SetBool("Moving", true);
        }
        else
        {
            Debug.Log("stop");
            m_animator.SetBool("Moving", false);
        }
        m_animator.SetFloat("YMovement", m_rigidBody.velocity.y);
    }

    void AttackAnimation(Attack attack)
    {
        m_overrideController["Attack"] = attack.associatedAnimation; //set attack aniamtion to current attacks animation 
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
