using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackTemplate : MonoBehaviour
{
    float hpDecrease;
    public float speed;
    public Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateAttack(Attack attack, BattleScript creatorObject)
    {
        if (attack.attackType == AttackType.special)
        {
            hpDecrease = attack.attackDamage + creatorObject.m_SpecialAttack;
        }
        else
        {
            hpDecrease = attack.attackDamage + creatorObject.m_Attack;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<BattleScript>() != null)
        {
            collision.gameObject.GetComponent<BattleScript>().Attack(hpDecrease);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
