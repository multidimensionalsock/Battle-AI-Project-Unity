using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckConditions : MonoBehaviour
{
    public bool collidingWithPlayer;
    public bool triggerWithPlayer;
    public GameObject playerRef;
    [SerializeField] Attacks AttackList;
    public List<Attack> meleeAttacks;
    public List<Attack> rangeAttacks;
    public List<Attack> specialAttacks;
    public float distanceToFlee;
    public bool ableToAttack = true;
    public bool ableToSpecialAttack = false;
    public Attack nextAttack;

    public int attacksInTheLastMinute;
    public float damageInTheLastMinute;
    private List<float[,]> LastMinuteStatList;
    private float[,] lastAttackStat; //hp lost, attack no
    bool attacked = false;

    public bool GetWasAttacked()
    {
        if (attacked == false) return false;
        if (attacked == true) attacked = false; return true;
    }

    private void OnEnable()
    {
        LoadAttacks();
        GetComponent<BattleScript>().HPreduce += Attacked;
        ableToAttack = true;
        ableToSpecialAttack = false;

        //make the list and add 60 empty 2d arrays
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collidingWithPlayer = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collidingWithPlayer = false;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            triggerWithPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            triggerWithPlayer = false;
        }
    }

    virtual public void LoadAttacks()
    {
        meleeAttacks = new List<Attack>();
        rangeAttacks = new List<Attack>();
        specialAttacks = new List<Attack>();
        for (int i = 0; i < AttackList.attackDetails.Length; i++)
        {
            switch (AttackList.attackDetails[i].attackType)
            {
                case AttackType.melee:
                    meleeAttacks.Add(AttackList.attackDetails[i]);
                    break;
                case AttackType.range:
                    rangeAttacks.Add(AttackList.attackDetails[i]);
                    break;
                case AttackType.special:
                    specialAttacks.Add(AttackList.attackDetails[i]);
                    break;
            }
        }
    }

    public void StartAttackCooldown()
    {
       //make a coroutine for this 
    }

    private void Attacked(float hpLost)
    {
        float[,] temp = { { hpLost, 1 } };
        lastAttackStat = temp;
        attacked = true;
    }

    IEnumerator lastMinuteStats() //needs testing
    {
        while (LastMinuteStatList.Any())
        {
            if (lastAttackStat[0, 0] > 0 || lastAttackStat[0, 1] > 0)
            {
                damageInTheLastMinute += lastAttackStat[0, 0];
                attacksInTheLastMinute += (int)lastAttackStat[0, 1];
                LastMinuteStatList.Add(lastAttackStat);

                float[,] temp = { { 0, 0 } };
                lastAttackStat = temp;
            }
            damageInTheLastMinute -= LastMinuteStatList[0][0, 0];
            attacksInTheLastMinute -= (int)LastMinuteStatList[0][0, 1];
            LastMinuteStatList.RemoveAt(0);
            yield return new WaitForSeconds(1.0f);
        }
    }
}
