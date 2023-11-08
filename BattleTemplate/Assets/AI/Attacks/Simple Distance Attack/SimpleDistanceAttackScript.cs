using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDistanceAttackScript : AttackTemplate
{
    private void FixedUpdate()
    {
        transform.position += transform.forward * speed * Time.fixedDeltaTime;
        Debug.Log(transform.position);
    }

    protected IEnumerator AutoKill()
    {
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }
}
