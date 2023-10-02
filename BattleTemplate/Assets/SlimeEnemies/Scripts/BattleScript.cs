using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleScript : MonoBehaviour
{
    [SerializeField] protected float m_HP;
    [SerializeField] protected int m_TP;
    [SerializeField] protected int m_Attack;
    [SerializeField] protected int m_Defence;
    [SerializeField] protected int m_SpecialAttack;
    [SerializeField] protected int m_SpecialDefence;
    protected bool defenseActivated;

    public void Attack(float hpDecrease)
    {
        if (defenseActivated)
        {
            hpDecrease = hpDecrease * 0.5f * (m_Defence / 100);
        }
        else
        {
            hpDecrease = hpDecrease * (m_Defence / 100);
        }
        m_HP -= hpDecrease;
    }

    public void SpecialAttack(float hpDecrease)
    {
        if (defenseActivated)
        {
            hpDecrease = hpDecrease * 0.5f * (m_Defence / 100);
        }
        else
        {
            hpDecrease = hpDecrease * (m_Defence / 100);
        }
        m_HP -= hpDecrease;
    }

    public float GetHp()
    {
        return m_HP;
    }
}
