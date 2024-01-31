using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckConditions : MonoBehaviour
{
    public bool collidingWithPlayer;
    public bool triggerWithPlayer;
    public GameObject playerRef;
    [SerializeField] Attacks AttackList;
    public List<Attack> meleeAttacks;
    public List<Attack> rangeAttacks;
    public List<Attack> specialAttacks;
    public float distanceToFlee;
    public bool ableToAttack = true;
    public bool ableToSpecialAttack = false;
    protected Attack nextAttack;
    public bool MovementLocked;
    [SerializeField] float attackCoolDownTime;
    [SerializeField] float specialAttackCoolDownTime;
    float SpecialAttackCoolDownTimeRemaining;
	[SerializeField] float lockMovementTime;
    private int MiniEnemiesInScene;

    public int attacksInTheLastMinute;
    public float damageInTheLastMinute;
    private List<float[,]> LastMinuteStatList;
    private float[,] lastAttackStat = { { 0, 0 } }; //hp lost, attack no
    bool attacked = false;
    [SerializeField] float damageInLastMinuteToUnlockSpecialAttack;
    [SerializeField] float attacksInLastMinuteToUnlockSpecialAttack;

    public event System.Action<Attack> AttackImplem;
    public event System.Action<AnimationClip> NextAttackAnimChange;
    public event System.Action<bool> waitModeOnOff;
    
    private void OnEnable()
    {
        LoadAttacks();
        GetComponent<BattleScript>().HPreduce += Attacked;
        ableToAttack = true;
        ableToSpecialAttack = false;
        StartCoroutine(StartSpecialAttackCooldown());
        
        //transform.GetChild(0).GetComponent<BTAnimationController>().AttackAnimFinished += EndMovementLock;

        LastMinuteStatList = new List<float[,]>();
        float[,] empty = { { 0, 0 } };
        for (int i = 0; i < 60; i++)
        {
            LastMinuteStatList.Add(empty);
        }
        StartCoroutine(lastMinuteStats());

        MiniEnemyFinite.Death += MiniEnemyDied;
    }
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


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            triggerWithPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            triggerWithPlayer = false;
        }
    }

    virtual public void LoadAttacks()
    {
        meleeAttacks = new List<Attack>();
        rangeAttacks = new List<Attack>();
        specialAttacks = new List<Attack>();
        for (int i = 0; i < AttackList.attackDetails.Length; i++)
        {
            switch (AttackList.attackDetails[i].attackType)
            {
                case AttackType.melee:
                    meleeAttacks.Add(AttackList.attackDetails[i]);
                    break;
                case AttackType.range:
                    rangeAttacks.Add(AttackList.attackDetails[i]);
                    break;
                case AttackType.special:
                    specialAttacks.Add(AttackList.attackDetails[i]);
                    break;
            }
        }
    }

    public void StartAttackCooldown()
    {
       //make a coroutine for this 
    }

    public void WaitModeEventCaller(bool state)
    {
        waitModeOnOff?.Invoke(state); 
    }

    IEnumerator AttackCooldown(Attack attackdata)
    {
        ableToAttack = false;
        yield return new WaitForSeconds(attackCoolDownTime + attackdata.freezeTime);
        ableToAttack = true;
    }

    IEnumerator SpecialAttackCooldown(Attack attackdata)
    {
        ableToSpecialAttack = false;
        SpecialAttackCoolDownTimeRemaining = specialAttackCoolDownTime; 
        while (SpecialAttackCoolDownTimeRemaining > 0)
        {
            if (attacksInTheLastMinute > attacksInLastMinuteToUnlockSpecialAttack)
            {
                ableToSpecialAttack = true;
                yield break;
            }
            if (damageInTheLastMinute > damageInLastMinuteToUnlockSpecialAttack)
            {
                ableToSpecialAttack = true;
                yield break;
            }
            SpecialAttackCoolDownTimeRemaining -= 1f * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        ableToSpecialAttack = true;
    }

    IEnumerator StartSpecialAttackCooldown()
    {
        ableToSpecialAttack = false;
        SpecialAttackCoolDownTimeRemaining = specialAttackCoolDownTime; 
        while (SpecialAttackCoolDownTimeRemaining > 0)
        {
            if (attacksInTheLastMinute > attacksInLastMinuteToUnlockSpecialAttack)
            {
                ableToSpecialAttack = true;
                yield break;
            }
            if (damageInTheLastMinute > damageInLastMinuteToUnlockSpecialAttack)
            {
                ableToSpecialAttack = true;
                yield break;
            }
            SpecialAttackCoolDownTimeRemaining -= 1f * Time.fixedDeltaTime;
            Debug.Log(SpecialAttackCoolDownTimeRemaining);
            yield return new WaitForFixedUpdate();
        }
        ableToSpecialAttack = true;
    }

    private void Attacked(float hpLost)
    {
        float[,] temp = { { hpLost / 10, 1 } };
        lastAttackStat = temp;
        attacked = true;
    }

    IEnumerator lastMinuteStats() //needs testing
    {
        while (LastMinuteStatList.Any())
        {
            if (lastAttackStat[0, 0] > 0 || lastAttackStat[0, 1] > 0)
            {
                damageInTheLastMinute += lastAttackStat[0, 0];
                attacksInTheLastMinute += (int)lastAttackStat[0, 1];
                LastMinuteStatList.Add(lastAttackStat);

                float[,] temp = { { 0, 0 } };
                lastAttackStat = temp;
            }
            else
            {
                float[,] empty = { { 0, 0 } };
                LastMinuteStatList.Add(empty);
            }
            damageInTheLastMinute -= LastMinuteStatList[0][0, 0];
            LastMinuteStatList.RemoveAt(0);
            
            yield return new WaitForSeconds(1.0f);
        }
    }

    public void CallAttackEvent(Attack attackData)
    {
        StartCoroutine(MovementLockCoroutine(attackData.freezeTime));
        AttackImplem?.Invoke(attackData);
        if (attackData.attackType == AttackType.special)
        {
            StartCoroutine(SpecialAttackCooldown(attackData));
            return;
        }
        StartCoroutine(AttackCooldown(attackData));
        if (attackData.attackType == AttackType.melee)
        {
            GetComponent<BattleScript>().SetTP(-1);
        }
    }

    IEnumerator MovementLockCoroutine(float time)
    {
        MovementLocked = true;
        yield return new WaitForSeconds(lockMovementTime + time);
        MovementLocked = false;
    }

    public void SetNextAttack(Attack newAttack)
    {
        NextAttackAnimChange?.Invoke(newAttack.associatedAnimation);
        nextAttack = newAttack;
    }

    public Attack GetNextAttack()
    {
        return nextAttack;
    }

    public void AddMiniEnemys(int numberAdded)
    {
        MiniEnemiesInScene += numberAdded;
    }

    public void MiniEnemyDied()
    {
        MiniEnemiesInScene -= 1;
    }

    public int GetNumberMiniEnemies()
    {
        return MiniEnemiesInScene;
    }

    public bool GetWasAttacked()
    {
        if (attacked == false) return false;
        if (attacked == true) attacked = false; return true;
    }
}
