using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] PlayerBattleScript playerBattleScript;
    [SerializeField] Slider HPbar;
    [SerializeField] Slider TPbar;
    [SerializeField] TextMeshProUGUI HPnumber;
    [SerializeField] TextMeshProUGUI TPnumber;

    // Start is called before the first frame update
    void Start()
    {
        HPbar = transform.GetChild(0).GetComponent<Slider>();
        TPbar = transform.GetChild(1).GetComponent<Slider>();

        //set nmax and current value
        HPbar.maxValue = playerBattleScript.m_maxHP;
        HPbar.value = HPbar.maxValue;
        HPnumber.text = HPbar.value.ToString();

        TPbar.maxValue = playerBattleScript.m_MaxTP;
        TPbar.value = TPbar.maxValue;
        TPnumber.text = TPbar.value.ToString();
    }

    private void Update()
    {
        HPbar.value = playerBattleScript.GetHp();
        TPbar.value = playerBattleScript.GetTP();
        HPnumber.text = Mathf.RoundToInt(HPbar.value).ToString();
        TPnumber.text = TPbar.value.ToString();
    }
}
