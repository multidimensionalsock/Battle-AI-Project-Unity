using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceAttackHandler : MonoBehaviour
{
    [SerializeField] GameObject iceAttack;
    [SerializeField] float damage;
    public bool colliding = false;
    float opacity = 0;
    [SerializeField] float timeToSpawnNext;
    [SerializeField] float spawnDistance;
    [SerializeField] float lifeTime;
    Renderer[] materials;
    //Vector3 spawnPos = playerPos + playerDirection*spawnDistance;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnNext());
        
        materials = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < materials.Length; i++)
        {
            Color c = materials[i].material.color;
            materials[i].material.color = new Color(c.r, c.g, c.b, opacity);
        }
        StartCoroutine(FadeIn());
    }

    //function to fade in 
    IEnumerator FadeIn()
    {
        while (opacity <= 1)
        {
            opacity += 1 / (lifeTime / 2) / 50;
            for (int i = 0; i < materials.Length; i++)
            { 
                Color c = materials[i].material.color;
                materials[i].material.color = new Color(c.r, c.g, c.b, opacity);
            }
            yield return new WaitForFixedUpdate();
        }
        yield break;
    }

    IEnumerator FadeOut()
    {
        yield break;
    }
    //function to fade out

    //ienum to spawn next one after x seconds
    IEnumerator SpawnNext()
    {
        yield return new WaitForSeconds(timeToSpawnNext);
        if (colliding) { yield break; }
        //make a new object set spawn to across from 
        Vector3 spawnPos = transform.position + (transform.right * -1) * spawnDistance;
        GameObject ice = Instantiate(iceAttack, spawnPos, transform.rotation);
        //set rotation
        //change rotation of child objects mayeb different fo rthe two 

        //random rot
        //randomise scale
        //set new trandofmr it it and aaaaaaaa work out where would be across by x
    }

    //on collisoion, if collide then dont make new object and 
    private void OnTriggerEnter(Collider other)
    {
        //might cause issue when it colliders with the floor
        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Floor") { return; }

        colliding = true;
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<PlayerBattleScript>().Attack(damage * opacity);
        }

    }


    //damage is affected by object opacity 

}
