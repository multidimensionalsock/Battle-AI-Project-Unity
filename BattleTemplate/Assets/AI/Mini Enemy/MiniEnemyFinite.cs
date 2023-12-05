using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    public static event System.Action Death;
    Pathfinding m_pathfinder;
    [SerializeField] GameObject m_playerRef;
    [SerializeField] GameObject m_bossRef;
    [SerializeField] float m_distanceToSeek;
    [SerializeField] float m_distanceToDefend;
    [SerializeField] float m_minAttackDamage;
    [SerializeField] float m_maxAttackDamage;
    float m_attackDamage;
    Coroutine m_currentStateCoroutine; //use it so theres no memory leaks or other coroutines runnign to delete it specificallt 
    bool m_playerCollision = false;
    bool lockAttack;

    private void Start()
    {
        GetComponent<BattleScript>().HPreduce += TransitionAny;
        m_pathfinder = GetComponent<Pathfinding>();
        m_pathfinder.SetDistanceToFlee(m_distanceToSeek * 1.25f);
        StateChange += CallStateChange;
        m_attackDamage = Random.Range(m_minAttackDamage, m_maxAttackDamage);

        StateChange?.Invoke(MiniEnemyStates.Idle);

    }

    void TransitionAny(float hpDec)
    {
        float HP = GetComponent<BattleScript>().GetHp();
        StopAllCoroutines();
        if (HP <= 0)
        {
            StateChange?.Invoke(MiniEnemyStates.Death);
            Death?.Invoke();
            return;
        }
        StateChange?.Invoke(MiniEnemyStates.Attacked);
    }

    IEnumerator Idle()
    {
        m_pathfinder.SetNewNavigation(pathfindingState.nullptr);
		do
		{
			IdleTransition();
			yield return new WaitForFixedUpdate();
		} while (m_currentState == MiniEnemyStates.Idle);

	}

    void IdleTransition()
    {
        if (InPlayerVercinity())
        {
            StateChange?.Invoke(MiniEnemyStates.Seek);
        }
        else if (!InPlayerVercinity())
        {
            StateChange?.Invoke(MiniEnemyStates.Wander);
        }
    }

    IEnumerator Seek()
    {
        //seek to player
        m_pathfinder.SetNewNavigation(pathfindingState.seek, m_playerRef);
        Debug.Log(m_playerRef.ToString());
        
        do {
            SeekTransition();
            yield return new WaitForFixedUpdate();
        } while (m_currentState == MiniEnemyStates.Seek) ;

	}

    void SeekTransition()
    {
        if (m_playerCollision)
        {
            StateChange?.Invoke(MiniEnemyStates.Attack);
        }
		if (!InPlayerVercinity())
		{
			StateChange?.Invoke(MiniEnemyStates.Wander);
		}
    }

    IEnumerator Wander()
    {
        m_pathfinder.SetNewNavigation(pathfindingState.wander);
        do
        {
            WanderTransition();
            yield return new WaitForFixedUpdate();
        } while (m_currentState == MiniEnemyStates.Wander);
    }

    void WanderTransition()
    {
        if (InPlayerVercinity())
        {
            StateChange.Invoke(MiniEnemyStates.Seek);
        }
        if (PlayerInBossVercinity())
        {
            StateChange?.Invoke(MiniEnemyStates.Defend);
        }
    }

    IEnumerator Defend()
    {
        m_pathfinder.SetNewNavigation(pathfindingState.seek, m_playerRef);
        while (m_currentState == MiniEnemyStates.Defend)
        {
            DefendTransition();
            yield return new WaitForFixedUpdate();
        }
        //FacePlayer();
    }

    void DefendTransition()
    {
        if (m_playerCollision)
        {
            StateChange?.Invoke(MiniEnemyStates.Attack);
        }
        if (!PlayerInBossVercinity())
        {
            StateChange?.Invoke(MiniEnemyStates.Wander);
        }
    }

    IEnumerator Attack()
    {
        if (lockAttack) { yield break; }
        FacePlayer();
        //take away HP if colliding
        if (m_playerCollision)
        {
            m_playerRef.GetComponent<BattleScript>().Attack(m_attackDamage);
            Debug.Log("Attack");
        }
        lockAttack = true;
        yield return new WaitForSeconds(2f);
        lockAttack = false;
        AttackTransition();
    }

    void AttackTransition()
    {
        StateChange?.Invoke(MiniEnemyStates.Idle);
    }

    //hasnt been tested
    IEnumerator Flee()
    {
        m_pathfinder.SetNewNavigation(pathfindingState.flee, m_playerRef);
        while (m_currentState == MiniEnemyStates.Flee)
        {
            FleeTransition();
            yield return new WaitForFixedUpdate();
        }
    }

    void FleeTransition()
    {
        if (PlayerInBossVercinity())
        {
            StateChange?.Invoke(MiniEnemyStates.Defend);
        }
        if (Mathf.Abs(Vector3.Distance(m_playerRef.transform.position, transform.position)) <= m_distanceToSeek * 1.5f) //out of player range and a half
        {
            StateChange?.Invoke(MiniEnemyStates.Wander);
        }

    }

    void CallStateChange(MiniEnemyStates newState)
    {
        if (lockAttack) { return; }
		m_currentState = newState;
        GetComponent<NavMeshAgent>().isStopped = false;
        switch (newState)
        {
            case MiniEnemyStates.Idle:
				StopAllCoroutines();
                GetComponent<NavMeshAgent>().isStopped = true;
                m_currentStateCoroutine = StartCoroutine(Idle());
                break;
            case MiniEnemyStates.Seek:
				//if (m_currentStateCoroutine != null) { StopCoroutine(m_currentStateCoroutine); }
				m_currentStateCoroutine = StartCoroutine(Seek());
                break;
            case MiniEnemyStates.Wander: 
                m_currentState = newState;
				if (m_currentStateCoroutine != null) { StopCoroutine(m_currentStateCoroutine); }
				m_currentStateCoroutine = StartCoroutine("Wander");
                break;
            case MiniEnemyStates.Defend: 
                m_currentState = newState;
				if (m_currentStateCoroutine != null) { StopCoroutine(m_currentStateCoroutine); }
				m_currentStateCoroutine = StartCoroutine("Defend");
                break;
            case MiniEnemyStates.Attack:
                m_currentState = newState;
				if (m_currentStateCoroutine != null) { StopCoroutine(m_currentStateCoroutine); }
				m_currentStateCoroutine = StartCoroutine("Attack");
                break;
            case MiniEnemyStates.Attacked:
                m_currentState = MiniEnemyStates.Flee;
				if (m_currentStateCoroutine != null) { StopCoroutine(m_currentStateCoroutine); }
				m_currentStateCoroutine = StartCoroutine("Flee");
                break; 
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
        if (m_playerRef == null || m_bossRef == null) { return false; }
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == m_playerRef)
        {
            m_playerCollision = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == m_playerRef)
        {
            m_playerCollision = false;
        }
    }
}
