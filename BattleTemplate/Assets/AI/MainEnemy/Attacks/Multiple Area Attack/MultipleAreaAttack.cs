using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleAreaAttack : MonoBehaviour
{
    [SerializeField] int numBolts;
    [SerializeField] GameObject objectToSpawn;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numBolts; i++)
        {
            Vector2 pointInCircle = Random.insideUnitCircle * 10f;
            GameObject bolt = Instantiate(objectToSpawn);
            bolt.transform.parent = this.transform; 
            bolt.transform.position = pointInCircle;
        }
    }

    private void Update()
    {
        if (transform.childCount == 0)
            Destroy(this);
    }

}
