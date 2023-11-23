using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum pathfindingState
{
	seek,
	flee,
	evade,
	wander,
    seekAttack,
	nullptr
}

public class Pathfinding : MonoBehaviour
{
    private NavMeshAgent m_agent = null;
	pathfindingState m_currentState;
	Vector3 m_targetPosition;
	float m_distanceToFlee;
	[SerializeField] GameObject m_objectToPathfind;
    public event System.Action<Attack> callAttack;


    private void OnEnable()
    {
        m_agent = GetComponent<NavMeshAgent>();
    }

	public void SetDistanceToFlee(float distance)
	{
		m_distanceToFlee = distance;
	}

	public void SetNewNavigation(pathfindingState newState, GameObject objectPos)
	{
		m_objectToPathfind = objectPos;
        StopAllCoroutines();
		switch (newState)
		{
			case pathfindingState.seek:
                StartCoroutine("SeekObject");
                break;
			case pathfindingState.flee:
                StartCoroutine("FleeObject");
                break;
			case pathfindingState.evade:
				Evade();
                break;
			case pathfindingState.nullptr:
				StopAllCoroutines();
				break;

		}
	}

    public void SetNewNavigation(pathfindingState newState, Vector3 targetpos)
    {
        m_targetPosition = targetpos;
        StopAllCoroutines();
        switch (newState)
        {
            case pathfindingState.seek:
                StartCoroutine("SeekLocation");
                break;
            case pathfindingState.flee:
                StartCoroutine("FleeLocation");
                break;
            case pathfindingState.nullptr:
                m_targetPosition = transform.position;
                StopAllCoroutines();
                break;

        }
    }

    public void SetNewNavigation(pathfindingState newState)
    {
        m_currentState = newState;
        StopAllCoroutines();
        switch (newState)
        {
            case pathfindingState.wander:
                StartCoroutine("Wander");
                break;
            case pathfindingState.nullptr:
                StopAllCoroutines();
                break;

        }
    }

    public void SetNewNavigation(Attack attack)
    {
        StopAllCoroutines();
        StartCoroutine(SeekToAttack(attack));
    }

    IEnumerator SeekObject()
	{
		m_currentState = pathfindingState.seek;
		//run towards target location
		while (m_currentState == pathfindingState.seek)
		{
            if (m_objectToPathfind == null) { m_currentState = pathfindingState.nullptr; m_targetPosition = transform.position; break; }
			m_targetPosition = m_objectToPathfind.transform.position;
            m_agent.SetDestination(m_targetPosition);
            if (transform.position == m_targetPosition)
			{
				m_currentState = pathfindingState.nullptr;
			}
			yield return new WaitForFixedUpdate();
		}
        m_currentState = pathfindingState.nullptr;
	}

    IEnumerator SeekLocation()
    {
        m_currentState = pathfindingState.seek;
        m_agent.SetDestination(m_targetPosition);
        //run towards target location
        while (m_currentState == pathfindingState.seek)
        {
            if (gameObject.transform.position == m_targetPosition)
            {
                m_currentState = pathfindingState.nullptr;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator SeekToAttack(Attack attack)
    {
        float distanceFromPlayer = Mathf.Abs(Vector3.Distance(m_objectToPathfind.transform.position, transform.position));
        while (distanceFromPlayer > attack.minDistanceToPerform && distanceFromPlayer < attack.maxDistanceToPerform)
        {
            Vector3 anglefromPlayer = (m_objectToPathfind.transform.position - transform.position).normalized;
            Vector3 pos = m_objectToPathfind.transform.position + (anglefromPlayer * (attack.maxDistanceToPerform - attack.minDistanceToPerform));
            SetNewNavigation(pathfindingState.seek, pos);
            yield return new WaitForFixedUpdate();
        }
        callAttack.Invoke(attack);
    }

    IEnumerator FleeObject()
	{
		m_currentState = pathfindingState.flee;
        while (m_currentState == pathfindingState.flee)
        {
            Vector3 angleToPlayer = (transform.position - m_objectToPathfind.transform.position).normalized; //angle from platey as a vector 3, destination - origin
            //issue, if angle to player is angle to wall then they keep moving at the wall so the angle needs to be changed more in that circumstance
            //maybe if distance is less than x then change angle 

			if (Physics.Raycast(transform.position, angleToPlayer, m_distanceToFlee))
			{
				RaycastHit hit;
				Ray ray = new Ray(transform.position, angleToPlayer);
				if (Physics.Raycast(ray, out hit))
				{
					m_targetPosition = transform.position + (angleToPlayer * (hit.distance - 0.1f));
				}
			}
			//if less than min distance then flee to max
			else
			{
				m_targetPosition = transform.position + (angleToPlayer * m_distanceToFlee);
			}

			m_agent.SetDestination(m_targetPosition);

			yield return new WaitForFixedUpdate();
        }
		//turn to face player
    }

    IEnumerator FleePosition()
    {
        m_currentState = pathfindingState.flee;
        
        Vector3 angleToTarget = (transform.position - m_targetPosition).normalized; //angle from platey as a vector 3, destination - origin

        if (Physics.Raycast(transform.position, angleToTarget, m_distanceToFlee))
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, angleToTarget);
            if (Physics.Raycast(ray, out hit))
            {
                m_targetPosition = transform.position + (angleToTarget * (hit.distance - 0.1f));
            }
        }
        //if less than min distance then flee to max
        else
        {
            m_targetPosition = transform.position + (angleToTarget * m_distanceToFlee);
        }

            m_agent.SetDestination(m_targetPosition);
        while (transform.position == m_targetPosition)
        {
            yield return new WaitForFixedUpdate();
        }
        //turn to face player
    }

    void Evade()
	{
		//thius isnt working but im not sure why, it might be ebcause im testing while the player isnt movign 
		float predictionTime = Vector3.Distance(transform.position, m_objectToPathfind.transform.position)/ (m_agent.speed + m_objectToPathfind.GetComponent<PlayerMovement>().GetSpeed());
		Vector3 fleePosition = m_objectToPathfind.transform.position + (m_objectToPathfind.GetComponent<Rigidbody>().velocity * predictionTime);
        Vector3 angleToFlee = (transform.position - fleePosition).normalized; 

        if (Physics.Raycast(transform.position, angleToFlee, m_distanceToFlee))
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, angleToFlee);
            if (Physics.Raycast(ray, out hit))
            {
                m_targetPosition = transform.position + (angleToFlee * (hit.distance - 0.1f));
            }
        }
        //if less than min distance then flee to max
        else
        {
            m_targetPosition = transform.position + (angleToFlee * m_distanceToFlee);
        }

        m_agent.SetDestination(m_targetPosition);
		m_currentState = pathfindingState.nullptr;
		//turn to face player

    }

	IEnumerator Wander()
	{
        Debug.Log(gameObject.name + "wnadering");
		Vector2 pointInCircle = Random.insideUnitCircle * 10f;
        m_targetPosition = new Vector3(pointInCircle.x, -1.51f, pointInCircle.y);
        Debug.Log(gameObject.name +  m_targetPosition);
        m_agent.SetDestination(m_targetPosition);
        Debug.Log(m_targetPosition);
        while (m_currentState == pathfindingState.wander)
		{
			if (Mathf.Abs(transform.position.x - m_targetPosition.x) <= 0.5f || Mathf.Abs(transform.position.z - m_targetPosition.z) <= 0.5f)
			{
                pointInCircle = Random.insideUnitCircle * 10;
                m_targetPosition = new Vector3(pointInCircle.x, -1.51f, pointInCircle.y);
                m_agent.SetDestination(m_targetPosition);
				Debug.Log("resrt pos" + m_targetPosition);
            }
			yield return new WaitForFixedUpdate();
		}
	}

	//IEnumerator Repel()
	//{
	//	//check if anything is in trigger collision distance, if so repel from it 
	//}

    public void CallAttackAnimation(Attack attack)
    {
        callAttack(attack);
    }
}
