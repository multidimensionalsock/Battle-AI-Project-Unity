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
    [SerializeField] float m_movementForce;
    [SerializeField] float m_maxSpeed;
    Rigidbody m_rigidBody;
    Vector3 m_movementDirection;
    int jumpNo;
    [SerializeField] float m_jumpForce;
    [SerializeField] float m_rotationSpeed;
    bool m_movementLock = false;
    Coroutine cameraMovco;
	[SerializeField] GameObject m_SpecialAttackObject;

    public float GetSpeed()
    {
        return m_rigidBody.velocity.magnitude;
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
        m_input.currentActionMap.FindAction("Defence").performed += DefenceStart;
        m_input.currentActionMap.FindAction("Defence").canceled += DefenceEnd;
        //m_distance = Mathf.Abs(Vector3.Distance(transform.position, m_camera.transform.position));

           
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
        m_movementDirection = new Vector3(m_movement.x, 0f, m_movement.y).normalized;
        m_movementCoroutine = StartCoroutine(Move());
    }

    void MoveEnd(InputAction.CallbackContext context)
    {
        m_movementDirection = Vector3.zero;
        StopCoroutine(m_movementCoroutine);
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
        Vector2 rotateCameraBy = context.ReadValue<Vector2>() * Time.deltaTime;
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
            child.rotation = Quaternion.Slerp(child.rotation, child.rotation * Quaternion.LookRotation(cameraMove), m_rotationSpeed / 5f);
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator Move()
    {
        GameObject cameraLook = transform.GetChild(0).gameObject;
        GameObject model = transform.GetChild(1).gameObject;

        
        while (m_movementDirection != Vector3.zero)
        {
            if (m_movementLock)
            {
                yield return new WaitForFixedUpdate();
                continue;
            }

            m_rigidBody.AddForce(cameraLook.transform.forward * m_movementDirection.z * m_movementForce);
            m_rigidBody.AddForce(cameraLook.transform.right * m_movementDirection.x * m_movementForce);
            m_rigidBody.velocity = Vector3.ClampMagnitude(m_rigidBody.velocity, m_maxSpeed);

            if (m_movementDirection.z > 0)
            {
                //rotate to look away from camera
                model.transform.rotation = Quaternion.Slerp(model.transform.rotation, cameraLook.transform.rotation, m_rotationSpeed);
                
                //transform.position += cameraLook.transform.forward * m_movementSpeed * Time.fixedDeltaTime;
            }
            else if (m_movementDirection.z < 0)
            {
                //rotate to look towards camera
                model.transform.rotation = Quaternion.Slerp(model.transform.rotation, cameraLook.transform.rotation * Quaternion.Euler(0, 180f, 0), m_rotationSpeed);
                
                //transform.position -= cameraLook.transform.forward * m_movementSpeed * Time.fixedDeltaTime;
            }

            if (m_movementDirection.x > 0)
            {
                //rotate to camera right 
                model.transform.rotation = Quaternion.Slerp(model.transform.rotation, cameraLook.transform.rotation * Quaternion.Euler(0, 90f, 0), m_rotationSpeed);
                //transform.position += cameraLook.transform.right * m_movementSpeed * Time.fixedDeltaTime;
            }
            else if (m_movementDirection.x < 0)
            {
                //rotate to camerta left 
                model.transform.rotation = Quaternion.Slerp(model.transform.rotation, cameraLook.transform.rotation * Quaternion.Euler(0, -90f, 0), m_rotationSpeed);
                //transform.position -= cameraLook.transform.right * m_movementSpeed * Time.fixedDeltaTime;
            }
            
            yield return new WaitForFixedUpdate();
        }
        //m_rigidBody.velocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        jumpNo = 0;
    }


}
