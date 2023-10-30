using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMoving : MonoBehaviour
{
    float speed = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position -= new Vector3(Time.deltaTime * speed, 0);

        if(this.transform.position.x <= -9.0f)
        {
            this.transform.position = new Vector3(6.58f, 3.37f, -3.0f);
        }
    }
}
