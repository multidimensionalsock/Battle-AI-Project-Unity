using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleAreaAttack : AttackTemplate
{
    [SerializeField] int numBolts;
    [SerializeField] GameObject objectToSpawn;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numBolts; i++)
        {
            Vector2 pointInCircle = Random.insideUnitCircle * 10f;
            Vector3 dimpointInCircle = new Vector3(pointInCircle.x, 0, pointInCircle.y);
            GameObject bolt = Instantiate(objectToSpawn);
            bolt.transform.parent = this.transform; 
            bolt.transform.position = dimpointInCircle;
        }
    }

    private void Update()
    {
        if (transform.childCount == 0)
            Destroy(this);
    }

}
