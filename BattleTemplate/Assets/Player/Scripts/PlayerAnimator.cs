using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerAnimator : MonoBehaviour
{
    Animator m_animator;
    PlayerInput m_input;
    Rigidbody m_rigidbody;
    int CurrentCollisions = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();  
        m_input = GetComponent<PlayerInput>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_input.currentActionMap.FindAction("Movement").performed += MoveStart;
        m_input.currentActionMap.FindAction("Movement").canceled += MoveEnd;
        //m_input.currentActionMap.FindAction("Jump").performed += Jump;
        m_input.currentActionMap.FindAction("Defence").performed += DefenceStart;
        m_input.currentActionMap.FindAction("Defence").canceled += DefenceEnd;
        GetComponent<PlayerBattleScript>().AttackCall += Attack;

        var destroy = new AnimationEvent();
        destroy.functionName = "DestroySelf";
        destroy.time = 2f;

    }

    private void Update()
    {
        m_animator.SetFloat("YMovement", m_rigidbody.velocity.y);
    }


    //void DefenceStart(InputAction.CallbackContext context)

    void MoveStart(InputAction.CallbackContext context)
    {
        m_animator.SetBool("Moving", true);
        m_animator.SetBool("Defence", false);
    }

    void MoveEnd(InputAction.CallbackContext context)
    {
        m_animator.SetBool("Moving", false);
    }

    void Attack()
    {
        m_animator.SetBool("Moving", false);
        m_animator.SetBool("Defence", false);
        m_animator.SetTrigger("Attack");
    }

    void DefenceStart(InputAction.CallbackContext context)
    {
        m_animator.SetBool("Defence", true);
    }
    
    void DefenceEnd(InputAction.CallbackContext context)
    {
        m_animator.SetBool("Defence", false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        m_animator.SetTrigger("Collision");
        if (collision.gameObject.tag == "Floor")
            m_animator.SetBool("Grounded", true);
        if (CurrentCollisions < 0) { CurrentCollisions = 0; }
        CurrentCollisions += 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        m_animator.SetBool("Grounded", true);
    }

    private void OnCollisionExit(Collision collision)
    {
        CurrentCollisions -= 1;
        if (CurrentCollisions <= 0) 
        { 
            CurrentCollisions = 0;
            m_animator.SetBool("Grounded", false);
        }
    }

    public void DestroyPlayer()
    {
        Destroy(gameObject);
    }

}
