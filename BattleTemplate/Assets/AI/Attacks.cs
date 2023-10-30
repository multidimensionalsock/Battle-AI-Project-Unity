using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Attack 
{
    public string attackName;
    public float attackDamage;
    public GameObject attackObject;
    public Animation associatedAnimation;
    public int attackStage;

}


[CreateAssetMenu(fileName = "Attacks", menuName = "ScriptableObjects/Attacks", order = 2)]
public class Attacks : ScriptableObject
{
    public Attack[] attackDetails;
}