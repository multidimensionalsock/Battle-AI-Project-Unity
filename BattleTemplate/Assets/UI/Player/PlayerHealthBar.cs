using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] PlayerBattleScript playerBattleScript;
    Slider HPbar;
    Slider TPbar;
    // Start is called before the first frame update
    void Start()
    {
        HPbar = transform.GetChild(0).GetComponent<Slider>();
        TPbar = transform.GetChild(1).GetComponent<Slider>();

        //set nmax and current value
        HPbar.maxValue = playerBattleScript.m_maxHP;
        HPbar.value = HPbar.maxValue;

        TPbar.maxValue = playerBattleScript.m_MaxTP;
        TPbar.value = TPbar.maxValue;
    }

    private void Update()
    {
        HPbar.value = playerBattleScript.GetHp();
        TPbar.value = playerBattleScript.GetTP();
    }
}
