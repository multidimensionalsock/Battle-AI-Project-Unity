using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsougiController : MonoBehaviour
{
    private Animator animator;

    public float Speed = 1.0f;
    public float Grabity = 50.0f;
    public float Rotas = 10.0f;

    float walkFinishTime;
    bool walkFinish;
    float rotateTime = 0.3f;

    float idleTime;
    float idleChangeSpan = 10.0f;
    float blinkTime;
    float blinkSpan = 7.0f;
    bool allowBlink;

    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;

    private bool isDownDownButton;
    private bool isDownLeftButton;
    private bool isDownRightButton;
    private bool isDownUpButton;

    bool allowWalk = true;

    public AttackEffect attackEffect;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        attackEffect = GameObject.Find("Trail").GetComponent<AttackEffect>();
    }

    void Update()
    {
        if (controller.isGrounded)
        {
            if (Input.GetKeyDown("space"))
            {
                ChangeAnimation("attack");
                allowWalk = false;

                Invoke("AttackEffectOn", 0.6f);
                Invoke("AttackEffectOff", 0.8f);
                Invoke("AllowWalk", 2.0f);
            }

            if (Input.GetKey("down") || Input.GetKey("s"))
            {
                isDownDownButton = true;
            } else 
            {
                isDownDownButton = false;
                walkFinish = true;
            }

            if (Input.GetKey("left") || Input.GetKey("a"))
            {
                isDownLeftButton = true;
            } else 
            {
                isDownLeftButton = false;
                walkFinish = true;
            }

            if (Input.GetKey("up") || Input.GetKey("w")) 
            {
                isDownUpButton = true;
            } else 
            {
                isDownUpButton = false;
                walkFinish = true;
            }         

            if (Input.GetKey("right") || Input.GetKey("d")) 
            {
                isDownRightButton = true;
            } else 
            {
                isDownRightButton = false;
                walkFinish = true; 
            }
        }

    }

    void FixedUpdate()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("IdleA"))
        {
            idleTime += Time.deltaTime;
            blinkTime += Time.deltaTime; 

            if (idleTime > idleChangeSpan)
            {
                allowBlink = false;
                ChangeAnimation("idleB");
                idleTime = 0;

                Invoke("AllowBlink", 5.0f);
            }
        }

        if (allowBlink)
        {
            if (blinkTime > blinkSpan)
            {
                ChangeAnimation("blink");
                blinkTime = 0;
            }
        }

        if (walkFinish)
        {
            walkFinishTime += Time.deltaTime;

            if (walkFinishTime > rotateTime)
            {
                ChangeAnimation("idle");
                walkFinishTime = 0;
            }
        }

        if (allowWalk)
        {
            if (isDownDownButton)
            {
                ChangeAnimation("walk");

                float angleDiff = Mathf.DeltaAngle(transform.localEulerAngles.y, 180);
                if (angleDiff == 0)
                {
                    controller.Move (this.gameObject.transform.forward * Speed * Time.deltaTime);
                } else if (angleDiff < -1f)
                {
                    transform.Rotate(0, Rotas * -1, 0);
                } else if (angleDiff > 1f)
                {
                    transform.Rotate(0, Rotas * 1, 0);
                } else {
                    transform.rotation = Quaternion.Euler(0.0f, 180, 0.0f);
                }
            }

            if (isDownLeftButton)
            {
                ChangeAnimation("walk");

                float angleDiff = Mathf.DeltaAngle(transform.localEulerAngles.y, -90);
                if (angleDiff == 0)
                {
                    controller.Move (this.gameObject.transform.forward * Speed * Time.deltaTime);
                } else if (angleDiff < -1f) 
                {
                    transform.Rotate( 0,Rotas * -1, 0);
                } else if (angleDiff > 1f) 
                {
                    transform.Rotate( 0,Rotas * 1, 0);
                } else 
                {
                    transform.rotation = Quaternion.Euler(0.0f, -90, 0.0f);
                }
            }

            if (isDownUpButton) 
            {
                ChangeAnimation("walk");

                float angleDiff = Mathf.DeltaAngle(transform.localEulerAngles.y, 0);
                if (angleDiff == 0) 
                {
                    controller.Move (this.gameObject.transform.forward * Speed * Time.deltaTime);
                } else if (angleDiff < -1f) 
                {
                    transform.Rotate( 0,Rotas * -1, 0);
                } else if (angleDiff > 1f) 
                {
                    transform.Rotate( 0,Rotas * 1, 0);
                } else 
                {
                    transform.rotation = Quaternion.identity;
                }
            }

            if (isDownRightButton) 
            {
                ChangeAnimation("walk");

                float angleDiff = Mathf.DeltaAngle(transform.localEulerAngles.y, 90);
                //Debug.Log($"left: {angleDiff}");
                if (angleDiff == 0) 
                {
                    controller.Move (this.gameObject.transform.forward * Speed * Time.deltaTime);
                } else if (angleDiff < -1f) 
                {
                    transform.Rotate( 0,Rotas * -1, 0);
                } else if (angleDiff > 1f) 
                {
                transform.Rotate( 0,Rotas * 1, 0);
                } else 
                {
                    transform.rotation = Quaternion.Euler(0.0f, 90, 0.0f);
                }
            }
        }

        moveDirection.y -= Grabity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

    void ChangeAnimation(string currentAnimation)
    {
        if (currentAnimation == "idle")
        {
            animator.SetBool("walkFlag", false);
            animator.SetBool("idleFlag", true);
        }

        if (currentAnimation == "idleB")
        {
            animator.SetTrigger("idleBFlag");
        }       

        if (currentAnimation == "walk")
        {
            animator.SetBool("walkFlag", true);
            animator.SetBool("idleFlag", false);
        }

        if (currentAnimation == "attack")
        {
            animator.SetTrigger("attackFlag");
        }

        if (currentAnimation == "blink")
        {
            animator.SetTrigger("blinkFlag");
        }
    }

    void AttackEffectOn()
    {
        attackEffect.AttackEffectOn();
    }

    void AttackEffectOff()
    {
        attackEffect.AttackEffectOff();
    }

    void AllowWalk()
    {
        allowWalk = true;
    }

    void AllowBlink()
    {
        allowBlink = true;
    }
}
