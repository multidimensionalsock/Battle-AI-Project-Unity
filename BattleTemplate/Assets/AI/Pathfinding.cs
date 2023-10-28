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
	Rigidbody m_rigidbody;

    private void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
		m_rigidbody = GetComponent<Rigidbody>();
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
				Evade();
                break;
			case pathfindingState.nullptr:
				StopAllCoroutines();
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
            case pathfindingState.nullptr:
                StopAllCoroutines();
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
        while (m_currentState == pathfindingState.flee)
        {
            Vector3 angleToPlayer = (transform.position - m_objectToPathfind.transform.position).normalized; //angle from platey as a vector 3, destination - origin

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

    }

	IEnumerator Wander()
	{
		Vector2 pointInCircle = Random.insideUnitCircle * 7;
        m_targetPosition = new Vector3(pointInCircle.x, -1.51f, pointInCircle.y);
        m_agent.SetDestination(m_targetPosition);
        Debug.Log(m_targetPosition);
        while (m_currentState == pathfindingState.wander)
		{
			if ((transform.position - m_targetPosition).magnitude < 0.5f)
				
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
}
