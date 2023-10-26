using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum pathfindingState
{
	seek,
	flee,
	arrive,
	evade,
	wander,
	repel,
	nullptr
}

public class Pathfinding : MonoBehaviour
{
    private NavMeshAgent m_agent = null;
	pathfindingState m_currentState;
	Vector3 m_targetPosition;
	[SerializeField] float m_distanceToFlee;
	GameObject m_objectToPathfind;

    private void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
    }

	public void SetNewNavigation(pathfindingState newState, GameObject objectPos)
	{
		m_objectToPathfind = objectPos;
		switch (newState)
		{
			case pathfindingState.seek:
                StartCoroutine("Seek");
                break;
			case pathfindingState.flee:
                StartCoroutine("Flee");
                break;
			case pathfindingState.arrive:
                StartCoroutine("Arrive");
                break;
			case pathfindingState.evade:
                StartCoroutine("Evade");
                break;

		}
	}

    public void SetNewNavigation(pathfindingState newState)
    {
        m_currentState = newState;
        switch (newState)
        {
            case pathfindingState.wander:
                StartCoroutine("Wander");
                break;
            case pathfindingState.repel:
                StartCoroutine("Repel");
                break;

        }
    }

    IEnumerator Seek()
	{
		m_currentState = pathfindingState.seek;
		//run towards target location
		while (m_currentState == pathfindingState.seek)
		{
			m_targetPosition = m_objectToPathfind.transform.position;
            m_agent.SetDestination(m_targetPosition);
            if (gameObject.transform.position == m_targetPosition)
			{
				m_currentState = pathfindingState.nullptr;
			}
			yield return new WaitForFixedUpdate();
		}
	}

	IEnumerator Flee()
	{
		m_currentState = pathfindingState.flee;

		Vector3 angleToPlayer = m_objectToPathfind.transform.position - transform.position; //angle from platey as a vector 3, destination - origin

        if (Physics.Raycast(transform.position, angleToPlayer, m_distanceToFlee))
		{
			RaycastHit hit;
			Ray ray = new Ray(transform.position, angleToPlayer);
			if (Physics.Raycast(ray, out hit))
			{
				m_targetPosition = m_targetPosition - (angleToPlayer * hit.distance);
			}
		}
		//if less than min distance then flee to max
		else
		{
			m_targetPosition = m_targetPosition - (angleToPlayer * m_distanceToFlee);
		}

        m_agent.SetDestination(m_targetPosition);

        while (m_currentState == pathfindingState.flee)
        {
            if (gameObject.transform.position == m_targetPosition)
            {
                m_currentState = pathfindingState.nullptr;
            }
            yield return new WaitForFixedUpdate();
        }
    }

 //   IEnumerator Arrive()
	//{

	//}

	//IEnumerator Evade()
	//{

	//}

	//IEnumerator Wander()
	//{
	//	//wander around the scene
	//}

	//IEnumerator Repel()
	//{
	//	//check if anything is in trigger collision distance, if so repel from it 
	//}
}
