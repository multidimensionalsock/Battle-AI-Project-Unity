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

        //ice spike random rotation
        float rotationOffsetY = Random.Range(-0.1f, 0.1f);
        float rotationOffsetZ = Random.Range(-0.1f, 0.1f);
        Transform iceSpikes = transform.GetChild(0).transform;
        iceSpikes.rotation = new Quaternion(iceSpikes.rotation.x, iceSpikes.rotation.y + rotationOffsetY, iceSpikes.rotation.z + rotationOffsetZ, iceSpikes.rotation.w);
        //odnt rouch x but touch y and z, only touch y for rock flat

        //rock flat random rotation
        rotationOffsetY = Random.Range(-0.1f, 0.1f);
        Transform rockFlat = transform.GetChild(2).transform;
        rockFlat.rotation = new Quaternion(rockFlat.rotation.x, rockFlat.rotation.y, rockFlat.rotation.z, rockFlat.rotation.w);

        //randomise scale?

        StartCoroutine(FadeIn());
    }

    //function to fade in 
    IEnumerator FadeIn()
    {
        while (opacity < 1)
        {
            opacity += 1 / (lifeTime / 2) / 50;
            for (int i = 0; i < materials.Length; i++)
            { 
                Color c = materials[i].material.color;
                materials[i].material.color = new Color(c.r, c.g, c.b, opacity);
            }
            yield return new WaitForFixedUpdate();
        }
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        while (opacity > 0)
        {
            opacity -= 1 / (lifeTime / 2) / 50;
            for (int i = 0; i < materials.Length; i++)
            {
                Color c = materials[i].material.color;
                materials[i].material.color = new Color(c.r, c.g, c.b, opacity);
            }
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
    }
    //function to fade out

    //ienum to spawn next one after x seconds
    IEnumerator SpawnNext()
    {
        yield return new WaitForSeconds(timeToSpawnNext);
        if (colliding) { yield break; }
        //make a new object set spawn to across from 
        Vector3 spawnPos = transform.position + (transform.forward) * spawnDistance;
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
        if (other.gameObject.tag == "Floor" || other.gameObject.tag == "Player") { return; }

        colliding = true;
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<PlayerBattleScript>().Attack(damage * opacity);
        }

    }


    //damage is affected by object opacity 

}
