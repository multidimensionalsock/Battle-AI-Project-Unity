using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerMovement : MonoBehaviour
{
    PlayerInput m_input;
    Coroutine m_movementCoroutine;
    [SerializeField] float m_movementSpeed;
    Rigidbody m_rigidBody;
    Vector3 m_movementDirection;
    int jumpNo;
    [SerializeField] float m_jumpForce;
    
    void Start()
    {
        m_input = GetComponent<PlayerInput>();
        m_rigidBody = GetComponent<Rigidbody>();
        m_input.currentActionMap.FindAction("Movement").performed += MoveStart;
        m_input.currentActionMap.FindAction("Movement").canceled += MoveEnd;
        m_input.currentActionMap.FindAction("Jump").performed += Jump;
        m_input.currentActionMap.FindAction("Camera").performed += MoveCamera;
    }

    void MoveStart(InputAction.CallbackContext context)
    {
        Debug.Log("outpted");
        Vector2 m_movement = context.ReadValue<Vector2>();
        m_movementDirection = new Vector3(m_movement.x, 0, m_movement.y).normalized;
        m_movementCoroutine = StartCoroutine(Move());
    }

    void MoveEnd(InputAction.CallbackContext context)
    {
        m_movementDirection = Vector3.zero;
        StopCoroutine(m_movementCoroutine);
    }

    void Jump(InputAction.CallbackContext context)
    {
        if (jumpNo < 2)
        {
            m_rigidBody.AddForce(new Vector3(0f, m_jumpForce - m_rigidBody.velocity.y, 0f), ForceMode.Impulse);
            jumpNo++;
        }
    }


    IEnumerator Move()
    {
        while (m_movementDirection != Vector3.zero)
        {
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_movementDirection), 0.1f);
            if (m_movementDirection.z > 0)
            {
                transform.position += transform.forward * m_movementSpeed * Time.fixedDeltaTime;
            }
            else if (m_movementDirection.z < 0)
            {
                transform.position -= transform.forward * m_movementSpeed * Time.fixedDeltaTime;
            }

            if (m_movementDirection.x > 0)
            {
                transform.position += transform.right * m_movementSpeed * Time.fixedDeltaTime;
            }
            else if (m_movementDirection.x < 0)
            {
                transform.position -= transform.right * m_movementSpeed * Time.fixedDeltaTime;
            }


            //transform.position += m_movementDirection * m_movementSpeed * Time.fixedDeltaTime;
            //m_rigidBody.velocity = new Vector3(m_movementDirection.x * m_movementSpeed * Time.fixedDeltaTime, m_rigidBody.velocity.y, m_movementDirection.z * m_movementSpeed * Time.fixedDeltaTime);

            yield return new WaitForFixedUpdate();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        jumpNo = 0;
    }

    void MoveCamera(InputAction.CallbackContext context)
    {
        Vector2 move = context.ReadValue<Vector2>();
        Vector3 moveRot = new Vector3(move.x, 0f, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveRot), 0.05f);
    }

}
