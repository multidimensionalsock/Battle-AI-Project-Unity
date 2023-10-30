using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

enum AnimationStates 
{ 
    idle, 
    walk, 
    jump,
    attack,
    specialAttack,
    defence
}


public class PlayerAnimator : MonoBehaviour
{

    Animator m_animator;
    PlayerInput m_input;
    Rigidbody m_rigidbody;
    bool lockState = false;
    AnimationStates m_queued;

    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();  
        m_input = GetComponent<PlayerInput>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_input.currentActionMap.FindAction("Movement").performed += MoveStart;
        m_input.currentActionMap.FindAction("Movement").canceled += MoveEnd;
        m_input.currentActionMap.FindAction("Jump").performed += Jump;
        m_input.currentActionMap.FindAction("Attack").performed += Attack;
        m_input.currentActionMap.FindAction("Defence").performed += DefenceStart;
        m_input.currentActionMap.FindAction("Defence").canceled += DefenceEnd;
    }

    private void Update()
    {
        m_animator.SetFloat("Ymovement", m_rigidbody.velocity.y);
        Debug.Log(m_animator.GetFloat("Ymovement"));
    }

    void Attack(InputAction.CallbackContext context)
    {
        if (lockState)
            return;

        m_animator.SetInteger("State", (int)AnimationStates.attack);
        StartCoroutine(LockCurrentState());
    }

    void DefenceStart(InputAction.CallbackContext context)
    {
        if (lockState)
            return;

        m_animator.SetInteger("State", (int)AnimationStates.defence);
        lockState = true;
    }

    void DefenceEnd(InputAction.CallbackContext context)
    {
        if (lockState && m_animator.GetInteger("State") != 5)
            return;

        lockState = false;
        if (m_queued == AnimationStates.walk)
        {
            Debug.Log("run run");
            m_animator.SetInteger("State", (int)AnimationStates.walk);
        }
        else
        {
            Debug.Log("run end");
            m_animator.SetInteger("State", (int)AnimationStates.idle);
        }
    }

    private void OnTriggerEnter(Collider other)
    { 
        m_animator.SetBool("Grounded", true);
        if (m_queued == AnimationStates.walk)
        {
            Debug.Log("run run");
            m_animator.SetInteger("State", (int)AnimationStates.walk);
        }
        else
        {
            Debug.Log("run end");
            m_animator.SetInteger("State", (int)AnimationStates.idle);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        m_animator.SetBool("Grounded", false);
    }

    void Jump(InputAction.CallbackContext context)
    {
        if (lockState)
            return;

        m_animator.SetInteger("State", (int)AnimationStates.jump);
    }

    void MoveStart(InputAction.CallbackContext context)
    {
        m_queued = AnimationStates.walk;
        if (lockState)
            return;
        
        Debug.Log("walk");
        m_animator.SetInteger("State", (int)AnimationStates.walk);
    }

    void MoveEnd(InputAction.CallbackContext context)
    {
        m_queued -= AnimationStates.walk;
        if (lockState)
            return;
        
        m_animator.SetInteger("State", (int)AnimationStates.idle);
    }

    IEnumerator LockCurrentState()
    {
        Debug.Log("codfidg");
        lockState = true;
        yield return new WaitForSeconds(m_animator.GetCurrentAnimatorClipInfo(0).Length);
        lockState = false;
        Debug.Log("runninignirngilrgj");
        if (m_queued == AnimationStates.walk)
        {
            Debug.Log("run run");
            m_animator.SetInteger("State", (int)AnimationStates.walk);
        }
        else
        {Debug.Log("run end");
            m_animator.SetInteger("State", (int)AnimationStates.idle);
        }
    }

    

}
