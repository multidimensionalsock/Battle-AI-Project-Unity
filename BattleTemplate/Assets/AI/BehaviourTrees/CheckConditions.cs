using System.Collections;
using System.Collections.Generic;
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

    private void OnEnable()
    {
        //LoadAttacks();
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

    //virtual public void LoadAttacks()
    //{
    //    meleeAttacks = new List<Attack>();
    //    rangeAttacks = new List<Attack>();
    //    specialAttacks = new List<Attack>();
    //    for (int i = 0; i < AttackList.attackDetails.Length; i++)
    //    {
    //        switch (AttackList.attackDetails[i].attackType)
    //        {
    //            case AttackType.melee:
    //                meleeAttacks.Add(AttackList.attackDetails[i]);
    //                break;
    //            case AttackType.range:
    //                rangeAttacks.Add(AttackList.attackDetails[i]);
    //                break;
    //            case AttackType.special:
    //                specialAttacks.Add(AttackList.attackDetails[i]);
    //                break;
    //        }
    //    }
    //}
}
