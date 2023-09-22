using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimeName : MonoBehaviour
{
    Animator m_Animator;
    AnimatorClipInfo[] m_AnimatorClipInfo;

    public Text TextFrame;

    float time;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    void Update()
    {
        time += Time.deltaTime;

        if (time <= 20)
        {
            m_AnimatorClipInfo = m_Animator.GetCurrentAnimatorClipInfo(0);
            TextFrame.text = string.Format(m_AnimatorClipInfo[0].clip.name);
        } else 
        {
            m_AnimatorClipInfo = m_Animator.GetCurrentAnimatorClipInfo(1);
            TextFrame.text = string.Format(m_AnimatorClipInfo[0].clip.name);            
        }
    }
}
