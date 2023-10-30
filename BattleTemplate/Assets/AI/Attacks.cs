using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    melee,
    range,
    special
}

[System.Serializable]
public struct Attack 
{
    public string attackName;
    public float attackDamage;
    public GameObject attackObject;
    public Animation associatedAnimation;
    public AttackType attackType;
    public float distanceFromPlayerToPerform;
}

[CreateAssetMenu(fileName = "Attacks", menuName = "ScriptableObjects/Attacks", order = 2)]
public class Attacks : ScriptableObject
{
    public Attack[] attackDetails;
}