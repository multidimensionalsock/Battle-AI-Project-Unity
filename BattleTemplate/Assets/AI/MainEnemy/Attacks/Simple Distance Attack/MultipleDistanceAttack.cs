using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleDistanceAttack : AttackTemplate
{
    [SerializeField] GameObject attackObject;
    [SerializeField] int numAttacks;
    // Start is called before the first frame update
    void Start()
    {
        float degreeEach = 360 / numAttacks;
        for (int i = 0; i < numAttacks; i++)
        {
            GameObject newAttack = Instantiate(attackObject);
            newAttack.transform.position = transform.position;
            newAttack.transform.parent = this.transform;
            newAttack.transform.rotation = Quaternion.Euler(0f, degreeEach * i, 0f);
        }
    }

}
