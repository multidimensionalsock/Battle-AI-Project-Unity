using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialSlashAttack : MonoBehaviour
{
	float m_attackDamage;
	Vector3 m_movementDirection;
	[SerializeField] float m_attackSpeed;

	public void CreateAttack(float attackDamage, Quaternion direction)
	{
		m_attackDamage= attackDamage;
		direction.y = direction.y;
		transform.rotation = direction;
		
	}
	private void FixedUpdate()
	{
		transform.position += transform.forward * m_attackSpeed;
	}


	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Enemy")
		{
			Debug.Log("attakckede");
			other.gameObject.GetComponent<BattleScript>().Attack(m_attackDamage);
		}

		//activate end particle effect
		//timer
		//destory object 
	}
}
