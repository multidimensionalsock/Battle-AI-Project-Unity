using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MiniEnemyStates
{
    Idle, //default state
    Seek, //Seek towards player
    Wander, //wander randomly when not in players versinity
    Attack, //attack the player (only mellee when in range)
    Attacked,
    Flee,
    Defend, //go around main enemy and block player path to prevent them being attacked
    Death
}

public class MiniEnemyFinite : MonoBehaviour
{
    MiniEnemyStates m_currentState;
    public event System.Action<MiniEnemyStates> StateChange;

    private void Start()
    {
        GetComponent<BattleScript>().HPreduce += TransitionAny;
        StateChange += CallStateChange;
    }

    void TransitionAny(float hpDec)
    {
        float HP = GetComponent<BattleScript>().GetHp();
        //if hp 0 then die
        //if hit then call attacked 
    }

    IEnumerator Idle()
    {
        //set not moving on x and z 
        while (m_currentState == MiniEnemyStates.Idle)
        {
            IdleTransition();
            yield return new WaitForFixedUpdate();
        }
    }

    void IdleTransition()
    {
        //if in player range
        ////seek
        //if not in player range 
        ////wander
    }

    //IEnumerator Seek()
    //{
    //seek to player
    //}

    //void SeekTransition()
    //{
    //if colliding with player
    ////attack
    //if not in player range
    ////wander
    //}

    //IEnumerator Wander()
    //{

    //}

    //void WanderTransition()
    //{
    //if player near boss 
    //// defend
    //if player in range
    ////seek
    //}

    //IEnumerator Defend()
    //{

    //}

    //void DefendTransition()
    //{
    //if colliding with player
    ////attack
    //if player not near boss
    ////wander

    //}

    //IEnumerator Attack()
    //{

    //}

    //void AttackTransition()
    //{
    //auto to idle 
    //}

    //IEnumerator Flee()
    //{

    //}

    //void FleeTransition()
    //{
    //if player in boss range
    //// defend 
    //if no longer in player range
    ////wander 

    //}

    void CallStateChange(MiniEnemyStates newState)
    {
        switch(newState)
        {
            case MiniEnemyStates.Idle:
            case MiniEnemyStates.Seek: break;
            case MiniEnemyStates.Wander: break;
            case MiniEnemyStates.Defend: break;
            case MiniEnemyStates.Attack: break;
            case MiniEnemyStates.Attacked: break; //make them flee
        }
    }
}
