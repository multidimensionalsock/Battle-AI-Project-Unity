using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBattleScript : BattleScript
{
    PlayerInput m_input;
    BattleScript currentCollision = null;
    // Start is called before the first frame update
    void Start()
    {
        m_input.currentActionMap.FindAction("Attack").performed += AttackOther;
        m_input.currentActionMap.FindAction("SpecialAttack").performed += SpecialAttackOther;
        m_input.currentActionMap.FindAction("Defence").performed += DefenceStart;
        m_input.currentActionMap.FindAction("Defence").canceled += DefenceEnd;
    }

    void AttackOther(InputAction.CallbackContext context)
    {
        currentCollision.Attack(m_Attack);
    }

    void SpecialAttackOther(InputAction.CallbackContext context)
    {
        currentCollision.SpecialAttack(m_SpecialAttack);
    }

    void DefenceStart(InputAction.CallbackContext context)
    {
        defenseActivated = true;
    }
    void DefenceEnd(InputAction.CallbackContext context)
    {
        defenseActivated = false;
    }

    private void OnTriggerEnter(Collider other) ///enemies and player need a secondary collision thats a trigger thats the attack radius
    {
        if (other.gameObject.tag == "Enemy")
        {
            currentCollision = other.gameObject.GetComponent<BattleScript>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            currentCollision = null;
        }
    }
}
