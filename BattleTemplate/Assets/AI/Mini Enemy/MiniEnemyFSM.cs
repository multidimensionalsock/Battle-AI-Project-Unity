using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum MiniEnemyStates 
{ 
    Idle, //default state
    Seek, //Seek towards player
    Wander, //wander randomly when not in players versinity
    Attack, //attack the player (only mellee when in range)
    Attacked,
    Defend, //go around main enemy and block player path to prevent them being attacked
    Death
}


public class MiniEnemyFSM : MonoBehaviour
{
    public event System.Action<MiniEnemyStates> StateChange;
    MiniEnemyStates m_currentState;
    BattleScript m_battleScript;
    Pathfinding m_pathfinder;
    Rigidbody m_rigidbody;
    bool m_HPDecreased;
    public GameObject m_playerRef;
    public GameObject m_bossRef;
    [SerializeField] float m_distanceToSeek;
    [SerializeField] float m_distanceToDefend;
    List<GameObject> m_collidingWith;
    float m_attackDamage;
    bool lockAttack;

    // Start is called before the first frame update
    void Start()
    {
        m_battleScript = GetComponent<BattleScript>();
        m_pathfinder = GetComponent<Pathfinding>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_battleScript.HPreduce += TransitionAny;
        m_collidingWith = new List<GameObject>();
        m_attackDamage = Random.Range(1,5);
    }

    public void SetUp(GameObject bossRef, GameObject playerRef)
    {
        m_playerRef = playerRef;
        m_bossRef = bossRef;
    }

    void TransitionAny(float hpdec)
    {
        if (m_battleScript.GetHp() <= 0) 
        {
            StopAllCoroutines();
            m_currentState = MiniEnemyStates.Death;
            StateChange?.Invoke(m_currentState);
        }
        else
        {
            StopAllCoroutines();
            m_HPDecreased = false;
            StateChange?.Invoke(MiniEnemyStates.Attacked);
        }
    }

    IEnumerable Idle()
    {
        while (m_currentState == MiniEnemyStates.Idle)
        {
            TransitionIdle();
            yield return new WaitForEndOfFrame();
        }
    }

    void TransitionIdle()
    {
        // trnaition to seek if in player versinity 
       if (InPlayerVercinity())
        {
            m_currentState = MiniEnemyStates.Seek; 
            StateChange?.Invoke(m_currentState);
        }
        //transition to wander if in idle if not 
        else
        {
            m_currentState = MiniEnemyStates.Wander;
            StateChange?.Invoke(m_currentState);
        }

    }

    IEnumerable Seek()
    {
        m_pathfinder.SetNewNavigation(pathfindingState.seek, m_playerRef);
        while (m_currentState == MiniEnemyStates.Seek)
        {
            TransitionSeek();
            yield return new WaitForEndOfFrame();
        }
    }

    void TransitionSeek()
    {
        //if colliding with player then attack
        if (m_collidingWith.Any() && lockAttack != true)
        {
            m_currentState = MiniEnemyStates.Attack;
            StateChange?.Invoke(m_currentState);
        }
        //if player is in boss verciitnity, switch to defend
        else if (PlayerInBossVercinity()) 
        {
            m_currentState = MiniEnemyStates.Defend;
            StateChange?.Invoke(m_currentState);
        }
        //if no longer in player vercinity, go to wander 
        else if (InPlayerVercinity())
        {
            m_currentState = MiniEnemyStates.Wander;
            StateChange?.Invoke(m_currentState);
        }
        
    }

    IEnumerable Wander()
    {
        m_pathfinder.SetNewNavigation(pathfindingState.wander);
        while (m_currentState == MiniEnemyStates.Wander)
        {
            TransitionWander();
            yield return new WaitForEndOfFrame();
        }
    }

    void TransitionWander()
    {
        // if in player vercinity, seek 
        if (InPlayerVercinity())
        {
            m_currentState = MiniEnemyStates.Seek;
            StateChange?.Invoke(m_currentState);
        }
        // if player in boss vercinity, defend 
        if (PlayerInBossVercinity())
        {
            m_currentState = MiniEnemyStates.Defend;
            StateChange?.Invoke(m_currentState);
        }
    }

    IEnumerable Attack()
    {
        //take away HP if colliding
        if (m_collidingWith.Any())
        {
            m_collidingWith[0].GetComponent<BattleScript>().Attack(m_attackDamage);
        }
        while (m_currentState == MiniEnemyStates.Attack)
        {
            TransitionAttack();
            yield return new WaitForEndOfFrame();
        }
    }
    void TransitionAttack()
    {
        if (lockAttack) { return; }
        m_currentState = MiniEnemyStates.Idle;
        StateChange?.Invoke(m_currentState);
    }

    IEnumerator lockTimer()
    {
        lockAttack = true;
        yield return new WaitForSeconds(0.1f);
        lockAttack = false;

    }

    IEnumerable Defend()
    {
        //naviagte to player path 
        while (m_currentState == MiniEnemyStates.Defend)
        {
            TransitionDefend();
            yield return new WaitForEndOfFrame();
        }
    }

    void TransitionDefend()
    {
        //if player no longer in vercitinity and is in your vercinity, seek 
        //if colliding with player then attack
        if (m_collidingWith.Any() && lockAttack != true)
        {
            m_currentState = MiniEnemyStates.Attack;
            StateChange?.Invoke(m_currentState);
        }
        else if (!InPlayerVercinity())
        {
            m_currentState = MiniEnemyStates.Seek;
            StateChange?.Invoke(m_currentState);
        }
        //if player in niether vercinity, wander
        if (!InPlayerVercinity() && !PlayerInBossVercinity())
        {
            m_currentState = MiniEnemyStates.Wander;
            StateChange?.Invoke(m_currentState);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            m_collidingWith.Add(collision.gameObject);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            m_collidingWith.Clear();
        }
    }

    bool InPlayerVercinity()
    {
        if (Mathf.Abs(Vector3.Distance(m_playerRef.transform.position, transform.position)) <= m_distanceToSeek)
        {
            return true;
        }
        return false;
    }

    bool PlayerInBossVercinity()
    {
        if (Mathf.Abs(Vector3.Distance(m_playerRef.transform.position, m_bossRef.transform.position)) < m_distanceToSeek)
        {
            return true;
        }
        return false;
    }
}
