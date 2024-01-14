using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BoltAttack : MonoBehaviour
{
    public float AttackDamage;
    public float lengthOfAttack;
    public float opacity;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<BattleScript>().Attack(AttackDamage);
        }
    }

    private void Start()
    {
        StartCoroutine(DestroyObject());
    }

    IEnumerator DestroyObject()
    {
        yield return new WaitForSecondsRealtime(6f);
        Destroy(gameObject);
    }
}
