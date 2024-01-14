using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BTAnimationController : MonoBehaviour
{
    public event System.Action AttackAnimFinished;
    // Start is called before the first frame update
    private void OnEnable()
    {
        GetComponent<CheckConditions>().AttackImplem += AttackAnimation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AttackAnimation(Attack attackData)
    {
        //code to switch in attack anim once this anim is finisghed anim attack finished needs calling 
    }

}
