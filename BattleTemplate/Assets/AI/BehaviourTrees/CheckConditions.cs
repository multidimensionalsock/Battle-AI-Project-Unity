using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckConditions : MonoBehaviour
{
    public bool collidingWithPlayer;
    public GameObject playerRef;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collidingWithPlayer = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collidingWithPlayer = false;
        }
    }
}
