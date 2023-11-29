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
    Pathfinding m_pathfinder;
    [SerializeField] GameObject m_playerRef;
    [SerializeField] GameObject m_bossRef;
    [SerializeField] float m_distanceToSeek;
    [SerializeField] float m_distanceToDefend;
    Coroutine m_currentStateCoroutine; //use it so theres no memory leaks or other coroutines runnign to delete it specificallt 

    private void Start()
    {
        GetComponent<BattleScript>().HPreduce += TransitionAny;
        m_pathfinder = GetComponent<Pathfinding>();
        m_pathfinder.SetDistanceToFlee(m_distanceToSeek * 1.25f);
        StateChange += CallStateChange;

        StateChange?.Invoke(MiniEnemyStates.Idle);

    }

    void TransitionAny(float hpDec)
    {
        float HP = GetComponent<BattleScript>().GetHp();
        StopAllCoroutines();
        if (HP <= 0)
        {
            StateChange?.Invoke(MiniEnemyStates.Death);
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
        //else if (!InPlayerVercinity())
        //{
        //    StateChange?.Invoke(MiniEnemyStates.Wander);
        //}
    }

    IEnumerator Seek()
    {
        //seek to player
        m_pathfinder.SetNewNavigation(pathfindingState.seek, m_playerRef);
        
        do {
			Debug.Log("seeking");
            SeekTransition();
            yield return new WaitForFixedUpdate();
        } while (m_currentState == MiniEnemyStates.Seek) ;

	}

    void SeekTransition()
    {
    //    if colliding with player
    ////attack
            //if not in player range
    ////wander
		if (!InPlayerVercinity())
		{
			StateChange?.Invoke(MiniEnemyStates.Idle);
		}
    }

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
		m_currentState = newState;
        switch(newState)
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
}
