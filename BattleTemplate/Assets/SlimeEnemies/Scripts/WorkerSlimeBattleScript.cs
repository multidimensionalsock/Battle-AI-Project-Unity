using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerSlimeBattleScript : BattleScript
{
    GameObject PlayerReference;
    GameObject KingSlimeReference;
    BattleScript KingSlimeBattleData;
    bool collidingWithPlayer;
    [SerializeField] float playerSeekDistance; //distance from player when you start seeking them
    [SerializeField] float maxDistanceFromKing; //max disntance from king before king defence starts
    [SerializeField] float KingHealthToDefend; //kings health when you start defending
    [SerializeField] float KingHealthToHeal; //kings health when you use heal

    void Start()
    {
        PlayerReference = GameObject.FindGameObjectWithTag("Player");
        KingSlimeReference = GameObject.FindGameObjectWithTag("KingSlime");
        KingSlimeBattleData = KingSlimeReference.GetComponent<BattleScript>();
    }

    // Update is called once per frame
    void Update()
    {
        //check kings health 
        if (KingSlimeBattleData.GetHp() <= KingHealthToHeal)
        {
            Heal();
        }
        else if (KingSlimeBattleData.GetHp() <= KingHealthToDefend)
        {
            DefendKing();
        }
        
        //check distance from player
        ////seek player
        ////if colliding then attack player?
        //check kings distance from player?
        ////defend king
    }

    void AttackOther()
    {
        PlayerReference.GetComponent<BattleScript>().Attack(m_Attack);
    }

    void Seek()
    {
        //go towards player 
    }

    void DefendKing()
    {
        //move towards king
    }

    void Heal()
    {
        //add hp to king
    }

    void Wander()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == PlayerReference)
        {
            collidingWithPlayer = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == PlayerReference)
        {
            collidingWithPlayer = false;
        }
    }
}
