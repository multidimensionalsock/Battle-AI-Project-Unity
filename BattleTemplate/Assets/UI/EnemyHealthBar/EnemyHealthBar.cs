using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{

	BattleScript m_battleScript;
	Slider m_slider;
	Camera m_camera;
	RectTransform m_rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        m_battleScript = transform.GetComponentInParent<BattleScript>();
		m_slider = GetComponent<Slider>();
		m_rectTransform= GetComponent<RectTransform>();
		m_slider.maxValue= m_battleScript.GetHp();
		m_slider.value = m_slider.maxValue;
		m_battleScript.HPreduce += ReduceHP;
		m_camera = Camera.current;

    }

	private void Update()
	{
		Vector3 lookRot = m_camera.transform.position - transform.position;
		transform.rotation = Quaternion.LookRotation(lookRot);
	}

	void ReduceHP(float hpLost)
	{
		m_slider.value = hpLost;
		if (m_slider.value <= 0 ) 
		{ 
			Destroy(this); 
		}
	}
}
