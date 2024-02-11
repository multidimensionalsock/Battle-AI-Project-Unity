using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
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
    bool lockAttack = false;
    [SerializeField] float maxTimeInState;
    [SerializeField] float attackCooldown;

    public static event System.Action MiniEnemyDead;

    public void OnInstantiation(GameObject playerReference, GameObject bossReference)
    {
        m_playerRef = playerReference;
        m_bossRef = bossReference;
        Physics.IgnoreLayerCollision(9, 8);
    }

    private void Start()
    {
        GetComponent<BattleScript>().HPreduce += TransitionAny;
        m_pathfinder = GetComponent<Pathfinding>();
        m_pathfinder.SetObjectToNaviagte(m_playerRef);
        m_pathfinder.SetDistanceToFlee(m_distanceToSeek * 1.25f);
        StateChange += CallStateChange;
        m_attackDamage = Random.Range(m_minAttackDamage, m_maxAttackDamage);
        m_distanceToDefend = Random.Range(m_distanceToDefend * 0.5f, m_distanceToDefend * 1.5f);
        m_distanceToSeek = Random.Range(m_distanceToSeek * 0.5f, m_distanceToSeek * 1.5f);


        StateChange?.Invoke(MiniEnemyStates.Seek);
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
        float idleTime = 0;
        m_pathfinder.SetNewNavigation(pathfindingState.nullptr);
		do
		{
            idleTime += 0.02f;
			IdleTransition(idleTime);
			yield return new WaitForFixedUpdate();
		} while (m_currentState == MiniEnemyStates.Idle);

	}

    void IdleTransition(float timeInIdle)
    {
        if (InPlayerVercinity()) // works 
        {
            StateChange?.Invoke(MiniEnemyStates.Seek);
        }
        if (timeInIdle >= maxTimeInState) //works
        {
            StateChange?.Invoke(MiniEnemyStates.Wander);
        }
    }

    IEnumerator Seek()
    {
        //seek to player
        m_pathfinder.SetNewNavigation(pathfindingState.seek, m_playerRef);
        
        do {
            SeekTransition();
            yield return new WaitForFixedUpdate();
        } while (m_currentState == MiniEnemyStates.Seek) ;

	}

    void SeekTransition()
    {
        ////attack when colliding with player 
        if (m_playerCollision) //working
        {
            StateChange?.Invoke(MiniEnemyStates.Attack);
        }
        ////idle if not in players vicinity
        if (!InPlayerVercinity()) 
        {
            StateChange?.Invoke(MiniEnemyStates.Idle);
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
        //seek if in players vicinity
        if (InPlayerVercinity()) //works
        {
            StateChange?.Invoke(MiniEnemyStates.Seek);
        }
        ////defend if player in boss vicnity
        if (PlayerInBossVercinity()) //works
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
        ////seek if player not in boss vicinity
        if (!PlayerInBossVercinity()) //works
        {
            StateChange?.Invoke(MiniEnemyStates.Seek);
        }
        ////attack if colliding with player 
        if (m_playerCollision)
        {
            StateChange?.Invoke(MiniEnemyStates.Attack);
        }
    }

    IEnumerator Attack()
    {
        if (lockAttack) { yield break; }
        m_pathfinder.SetNewNavigation(pathfindingState.nullptr);
        FacePlayer();
        //take away HP if colliding
        if (m_playerCollision)
        {
            m_playerRef.GetComponent<BattleScript>().Attack(m_attackDamage);
            Debug.Log("Attack");
        }
        lockAttack = true;
        yield return new WaitForSeconds(attackCooldown);
        lockAttack = false;
        AttackTransition();
    }

    void AttackTransition()
    {
        //idle after done 
        StateChange?.Invoke(MiniEnemyStates.Idle);
    }

    //hasnt been tested
    IEnumerator Flee()
    {
        float fleeTime = 0;
        m_pathfinder.SetNewNavigation(pathfindingState.flee, m_playerRef);
        while (m_currentState == MiniEnemyStates.Flee)
        {
            fleeTime += 0.02f;
            FleeTransition(fleeTime);
            yield return new WaitForFixedUpdate();
        }
    }

    void FleeTransition(float fleeTime)
    {
        //idle if out of players vicinity or time limit reached
        if (!InPlayerVercinity() || fleeTime >= maxTimeInState)
        {
            StateChange?.Invoke(MiniEnemyStates.Idle);
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
                if (m_currentStateCoroutine != null) { StopCoroutine(m_currentStateCoroutine); }
                int random = Random.Range(0, 1);
                if (random == 0)
                {
                    if (m_playerCollision)
                    {
                        m_currentState = MiniEnemyStates.Attack;
                        StateChange?.Invoke(MiniEnemyStates.Attack);
                        break;
                    }
                }
                m_currentState = MiniEnemyStates.Flee;
                StateChange?.Invoke(MiniEnemyStates.Flee);
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            m_playerCollision = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
        if (other.gameObject.tag == "Player")
        {
            m_playerCollision = false;
        }
    }

}
