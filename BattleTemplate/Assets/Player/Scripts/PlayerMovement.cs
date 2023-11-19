using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;
using UnityEngine.Windows;
using static UnityEngine.GraphicsBuffer;

public class PlayerMovement : MonoBehaviour
{
    PlayerInput m_input;
    Coroutine m_movementCoroutine;
    Animator m_animator;
    [SerializeField] float m_movementSpeed;
    Rigidbody m_rigidBody;
    Vector3 m_movementDirection;
    int jumpNo;
    [SerializeField] float m_jumpForce;
    bool m_movementLock = false;
    Coroutine cameraMovco;

    public float GetSpeed()
    {
        return m_movementSpeed;
    }
    
    void Start()
    {
        m_input = GetComponent<PlayerInput>();
        m_rigidBody = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
        m_input.currentActionMap.FindAction("Movement").performed += MoveStart;
        m_input.currentActionMap.FindAction("Movement").canceled += MoveEnd;
        m_input.currentActionMap.FindAction("Jump").performed += Jump;
        m_input.currentActionMap.FindAction("Camera").performed += MoveCamera;
        m_input.currentActionMap.FindAction("Camera").canceled += StopCamera;
        m_input.currentActionMap.FindAction("Attack").performed += Attack;
        m_input.currentActionMap.FindAction("SpecialAttack").performed += SpecialAttack;
        m_input.currentActionMap.FindAction("Defence").performed += DefenceStart;
        m_input.currentActionMap.FindAction("Defence").canceled += DefenceEnd;
        //m_distance = Mathf.Abs(Vector3.Distance(transform.position, m_camera.transform.position));

           
    }

    void SpecialAttack(InputAction.CallbackContext context)
    {
        if (m_movementLock)
            return;
    }

    void DefenceStart(InputAction.CallbackContext context)
    {
        m_movementLock = true;
    }

    void DefenceEnd(InputAction.CallbackContext context)
    {
        m_movementLock = false;
    }

    void MoveStart(InputAction.CallbackContext context)
    {
        Vector2 m_movement = context.ReadValue<Vector2>();
        m_movementDirection = new Vector3(m_movement.x, 0, m_movement.y).normalized;
        m_movementCoroutine = StartCoroutine(Move());
    }

    void MoveEnd(InputAction.CallbackContext context)
    {
        m_movementDirection = Vector3.zero;
        StopCoroutine(m_movementCoroutine);
    }

    void Attack(InputAction.CallbackContext context)
    {
        StartCoroutine(LockMovement());
    }

    IEnumerator LockMovement()
    {
        m_movementLock = true;
        yield return new WaitForSeconds(m_animator.GetCurrentAnimatorClipInfo(0).Length * 1.5f);
        m_movementLock = false;
    }

    void Jump(InputAction.CallbackContext context)
    {
        if (jumpNo < 2)
        {
            m_rigidBody.AddForce(new Vector3(0f, m_jumpForce - m_rigidBody.velocity.y, 0f), ForceMode.Impulse);
            jumpNo++;
        }
    }

    void MoveCamera(InputAction.CallbackContext context)
    {
        Vector2 rotateCameraBy = context.ReadValue<Vector2>();
        if (cameraMovco != null) { StopCoroutine(cameraMovco); }
        cameraMovco = StartCoroutine(CameraMoveCoroutine(new Vector3(rotateCameraBy.x, 0, rotateCameraBy.y)));
        
    }

    void StopCamera(InputAction.CallbackContext context)
    {
        StopCoroutine(cameraMovco);
    }

    IEnumerator CameraMoveCoroutine(Vector3 cameraMove)
    {
        Transform child = transform.GetChild(0).gameObject.transform;
        while (cameraMove != Vector3.zero)
        {
            child.rotation = Quaternion.Slerp(child.rotation, child.rotation * Quaternion.LookRotation(cameraMove), 0.01f);
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator Move()
    {
        while (m_movementDirection != Vector3.zero)
        {
            if (m_movementLock)
            {
                yield return new WaitForFixedUpdate();
                continue;
            }
            if (m_movementDirection.z > 0)
            {
                //transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * Quaternion.LookRotation(m_movementDirection), 0.01f);
                transform.position += transform.forward * m_movementSpeed * Time.fixedDeltaTime;
            }
            else if (m_movementDirection.z < 0)
            {
                
                transform.position -= transform.forward * m_movementSpeed * Time.fixedDeltaTime;
            }

            if (m_movementDirection.x > 0)
            {
                //transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * Quaternion.LookRotation(m_movementDirection), 0.01f);
                transform.position += transform.right * m_movementSpeed * Time.fixedDeltaTime;
            }
            else if (m_movementDirection.x < 0)
            {
                //transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * Quaternion.LookRotation(m_movementDirection), 0.01f);
                transform.position -= transform.right * m_movementSpeed * Time.fixedDeltaTime;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        jumpNo = 0;
    }


}
