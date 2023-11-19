using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDistanceAttackScript : AttackTemplate
{

    private void Start()
    {
        transform.position = new Vector3 (transform.position.x, transform.position.y + 1, transform.position.z);
    }
    private void FixedUpdate()
    {
        transform.position += transform.forward * speed * Time.fixedDeltaTime;
    }

    protected IEnumerator AutoKill()
    {
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }
}
