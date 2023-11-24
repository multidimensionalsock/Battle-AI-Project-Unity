using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;




public class MiniEnemyFSM : MonoBehaviour
{
    public event System.Action<MiniEnemyStates> StateChange;
    MiniEnemyStates m_currentState;
    BattleScript m_battleScript;
    Pathfinding m_pathfinder;
    public GameObject m_playerRef;
    public GameObject m_bossRef;
    [SerializeField] float m_distanceToSeek;
    [SerializeField] float m_distanceToDefend;
    List<GameObject> m_collidingWith;
    float m_attackDamage;
    bool lockAttack;
    NavMeshAgent m_agent;

    // Start is called before the first frame update
    void Start()
    {
        m_battleScript = GetComponent<BattleScript>();
        m_pathfinder = GetComponent<Pathfinding>();
        m_agent = GetComponent<NavMeshAgent>();
        m_battleScript.HPreduce += TransitionAny;
        m_collidingWith = new List<GameObject>();
        m_attackDamage = Random.Range(1,5);
        StateChange += CallStateChange;
        m_currentState = MiniEnemyStates.Idle;
        CallStateChange(MiniEnemyStates.Idle);
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
            StateChange?.Invoke(MiniEnemyStates.Attacked);
        }
    }

    IEnumerator Idle()
    {
        while (m_currentState == MiniEnemyStates.Idle)
        {
            TransitionIdle();
            yield return new WaitForFixedUpdate();
        }
    }

    void TransitionIdle()
    {
        Debug.Log("transidle");
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

    IEnumerator Seek()
    {
        m_pathfinder.SetNewNavigation(pathfindingState.seek, m_playerRef);
        while (m_currentState == MiniEnemyStates.Seek)
        {
            TransitionSeek();
            yield return new WaitForFixedUpdate();
        }
        FacePlayer();
    }

    void TransitionSeek()
    {
        Debug.Log("transseek");
        //if colliding with player then attack
        if (m_collidingWith.Any() && lockAttack != true)
        {
            m_currentState = MiniEnemyStates.Attack;
            StateChange?.Invoke(m_currentState);
        }
        //if player is in boss verciitnity, switch to defend

        //if no longer in player vercinity, go to wander
        else if (!InPlayerVercinity())
        {
            m_currentState = MiniEnemyStates.Wander;
            StateChange?.Invoke(m_currentState);
        }
    }

    IEnumerator Wander()
    {
        m_pathfinder.SetNewNavigation(pathfindingState.wander);
        while (m_currentState == MiniEnemyStates.Wander)
        {
            TransitionWander();
            yield return new WaitForFixedUpdate();
            
        }
        m_pathfinder.SetNewNavigation(pathfindingState.nullptr);
    }

    void TransitionWander()
    {
        Debug.Log("transwander");
        // if in player vercinity, seek 
        if (InPlayerVercinity())
        {
            m_currentState = MiniEnemyStates.Seek;
            StopAllCoroutines();
            StateChange?.Invoke(m_currentState);
        }
        // if player in boss vercinity, defend 
        if (PlayerInBossVercinity())
        {
            m_currentState = MiniEnemyStates.Defend;
            StopAllCoroutines();
            StateChange?.Invoke(m_currentState);
        }
    }

    IEnumerator Attack()
    {
        
        if (lockAttack) { yield break; }
        FacePlayer();
        //take away HP if colliding
        if (m_collidingWith.Any())
        {
            m_collidingWith[0].GetComponent<BattleScript>().Attack(m_attackDamage);
            Debug.Log("Attack");
        }
        lockAttack = true;
        yield return new WaitForSeconds(0.5f);
        lockAttack = false;
        TransitionAttack();
    }
    void TransitionAttack()
    {
        Debug.Log("transattack");
        if (lockAttack || m_playerRef == null) { return; }
        m_currentState = MiniEnemyStates.Idle;
        StateChange?.Invoke(m_currentState);
    }

    IEnumerator Defend()
    {
        //naviagte to player path 
        //evade kinda but seek to evade 
        //seeking instead because thats easier maybe change later
        m_pathfinder.SetNewNavigation(pathfindingState.seek, m_playerRef);
        while (m_currentState == MiniEnemyStates.Defend)
        {
            TransitionDefend();
            yield return new WaitForFixedUpdate();
        }
        FacePlayer();
    }

    void TransitionDefend()
    {
        Debug.Log("trsansdef");
        //if player no longer in vercitinity and is in your vercinity, seek 
        //if colliding with player then attack
        if (m_collidingWith.Any() && lockAttack != true)
        {
            m_currentState = MiniEnemyStates.Attack;
            StateChange?.Invoke(m_currentState);
        }
        else if (InPlayerVercinity() && !PlayerInBossVercinity())
        {
            m_currentState = MiniEnemyStates.Seek;
            StateChange?.Invoke(m_currentState);
        }
        //if player in niether vercinity, wander
        //if (!InPlayerVercinity() && !PlayerInBossVercinity())
        //{
        //    m_currentState = MiniEnemyStates.Wander;
        //    StateChange?.Invoke(m_currentState);
        //}
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
        if (m_playerRef == null) { return false; }
        if (Mathf.Abs(Vector3.Distance(m_playerRef.transform.position, transform.position)) <= m_distanceToSeek)
        {
            return true;
        }
        return false;
    }

    bool PlayerInBossVercinity()
    {
        if (m_playerRef == null || m_bossRef == null) { return false;  }
        if (Mathf.Abs(Vector3.Distance(m_playerRef.transform.position, m_bossRef.transform.position)) < m_distanceToDefend)
        {
            return true;
        }
        return false;
    }

    void FacePlayer()
    {
        if (m_playerRef == null) return;
        Vector3 lookRot = m_playerRef.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(lookRot);
    }

    void CallStateChange(MiniEnemyStates state)
    {
        switch (state)
        {
            case MiniEnemyStates.Idle:
                StartCoroutine("Idle");
                break;
            case MiniEnemyStates.Seek:
                StartCoroutine("Seek");
                break;
            case MiniEnemyStates.Wander:
                StartCoroutine(Wander());
                break;
            case MiniEnemyStates.Attack:
                StartCoroutine("Attack");
                break;
            case MiniEnemyStates.Defend:
                StartCoroutine("Defend");
                break;
        }
    }
}
