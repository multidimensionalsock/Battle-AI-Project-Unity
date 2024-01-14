using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDistanceAttackScript : AttackTemplate
{
    public float AttackDamage;

    private void Start()
    {
        transform.position = new Vector3 (transform.position.x, transform.position.y + 1, transform.position.z);
        StartCoroutine(AutoKill());
    }

    private void FixedUpdate()
    {
        transform.position += transform.forward * speed * Time.fixedDeltaTime;
    }

    protected IEnumerator AutoKill()
    {
        yield return new WaitForSeconds(8);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<BattleScript>().Attack(AttackDamage);
            Destroy(gameObject);
        }
    }
}
