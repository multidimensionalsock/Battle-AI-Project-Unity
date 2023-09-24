using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerAnimator : MonoBehaviour
{

    Animator m_animator;
    PlayerInput m_input;
    Rigidbody m_rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();  
        m_input = GetComponent<PlayerInput>();
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
        if (m_rigidbody.velocity.y != 0)
        {
            m_animator.SetBool("Grounded", false);
        }
    }

    void Attack(InputAction.CallbackContext context)
    {
        m_animator.SetTrigger("Attack");
    }

    void DefenceStart(InputAction.CallbackContext context)
    {
        m_animator.SetTrigger("Defence");
    }

    void DefenceEnd(InputAction.CallbackContext context)
    {
        m_animator.SetTrigger("Idle");

    }

    private void OnCollisionEnter(Collision collision)
    {
        m_animator.SetBool("Grounded", true);
    }

    void Jump(InputAction.CallbackContext context)
    {
        m_animator.SetTrigger("Jump");
    }

    void MoveStart(InputAction.CallbackContext context)
    {
        m_animator.SetTrigger("Walk");
    }

    void MoveEnd(InputAction.CallbackContext context)
    {
        m_animator.SetTrigger("Idle");
    }

}
