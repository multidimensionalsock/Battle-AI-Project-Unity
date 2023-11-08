using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackTemplate : MonoBehaviour
{
    protected float hpDecrease;
    [SerializeField] protected float speed;
    protected Vector3 direction;
    protected Attack attackData;

    public void CreateAttack(Attack attack, BattleScript creatorObject, Quaternion rotation)
    {
        attackData = attack;
        transform.rotation = rotation;
        if (attack.attackType == AttackType.special)
        {
            hpDecrease = attack.attackDamage + creatorObject.m_SpecialAttack;
        }
        else
        {
            hpDecrease = attack.attackDamage + creatorObject.m_Attack;
        }
        //StartCoroutine(AutoKill());
    }

    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<PlayerAnimator>() != null)
        {
            collision.gameObject.GetComponent<BattleScript>().Attack(hpDecrease);
            Destroy(gameObject);
        }
        //else
        //{
        //    Destroy(gameObject);
        //}
    }

    protected IEnumerator AutoKill()
    {
        yield return new WaitForSeconds(attackData.freezeTime);
        Destroy(gameObject);
    }
}
