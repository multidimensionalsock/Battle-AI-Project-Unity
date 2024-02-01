using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBattleScript : BattleScript
{
    PlayerInput m_input;
    BattleScript currentCollision = null;
	[SerializeField] protected GameObject m_battleObject;
    bool canAttack;
    [SerializeField] float attackCooldown;
    [SerializeField] float damageCooldown;

    // Start is called before the first frame update
    void Start()
    {
		m_input = GetComponent<PlayerInput>();
		m_input.currentActionMap.FindAction("Attack").performed += AttackOther;
        m_input.currentActionMap.FindAction("SpecialAttack").performed += SpecialAttackOther;
        m_input.currentActionMap.FindAction("Defence").performed += DefenceStart;
        m_input.currentActionMap.FindAction("Defence").canceled += DefenceEnd;
		this.GetComponent<BattleScript>().HPreduce += Attacked;
    }

    void AttackOther(InputAction.CallbackContext context)
    {
        if (currentCollision == null) { return; }
        m_TP += 1;
        currentCollision.Attack(m_Attack);
    }

    void SpecialAttackOther(InputAction.CallbackContext context)
    {
        //currentCollision.SpecialAttack(m_SpecialAttack);
        //create distance attack
        m_TP -= 4;
        if (m_TP < 4)
        {
            return;
        }
		GameObject m_attackObj = Instantiate(m_battleObject);
		m_attackObj.transform.position = transform.position;
		m_attackObj.GetComponent<SpecialSlashAttack>().CreateAttack(m_Attack, gameObject.transform.GetChild(1).gameObject.transform.rotation);
    }

    void DefenceStart(InputAction.CallbackContext context)
    {
        defenseActivated = true;
    }
    void DefenceEnd(InputAction.CallbackContext context)
    {
        defenseActivated = false;
	}

	void Attacked(float hpLost)
	{
        Debug.Log("ATTACKED" + gameObject.name);
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
