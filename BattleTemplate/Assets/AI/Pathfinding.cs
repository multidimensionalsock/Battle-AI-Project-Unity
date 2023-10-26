using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
	[SerializeField] float AISpeed;

    void Seek(Vector3 position)
	{
		Vector3.MoveTowards(gameObject.transform.position, position, AISpeed * Time.deltaTime);
	}

	void Flee(Vector3 position)
	{
		Vector3 dir = gameObject.transform.position - position;
		transform.Translate(dir * AISpeed * Time.deltaTime);
	}

	void Arrive(Vector3 position)
	{

	}

	void Evade(Vector3 position)
	{

	}

	void Wander()
	{
		//wander around the scene
	}

	void repel()
	{
		//check if anything is in trigger collision distance, if so repel from it 
	}
}
