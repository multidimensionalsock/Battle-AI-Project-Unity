using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FoxAIcontroller : MonoBehaviour
{
    Pathfinding m_pathfinder;
    [SerializeField] GameObject m_playerReference;
    [SerializeField] BattlePhaseTemplate[] battlePhases;
    BattlePhaseTemplate m_currentBattlePhase;
    BattleScript m_battleInformation;
    
    // Start is called before the first frame update
    void Start()
    {
        m_pathfinder = GetComponent<Pathfinding>();
        m_battleInformation = GetComponent<BattleScript>();
        m_currentBattlePhase = battlePhases[0];
        m_currentBattlePhase.Enable(m_playerReference, 1);
        m_battleInformation.HPreduce += checkPhaseUpdate;
    }

    // Update is called once per frame
    void Update()
    {
        //m_currentBattlePhase.AttackStrategy();
        
    }

    private void FixedUpdate()
    {
        
            m_currentBattlePhase.MovementStrategy();
        
    }

    //make an event whemn hp falls.
    void checkPhaseUpdate(float hp)
    {
        //current values are placeholders;
        if (hp < 5 && m_currentBattlePhase == battlePhases[0])
        {
            m_currentBattlePhase = battlePhases[1];
            m_currentBattlePhase.Enable(m_playerReference, 2);

            //any other behaviours on the switch
        }
        else if (hp < 2.5 && m_currentBattlePhase == battlePhases[1])
        {
            m_currentBattlePhase= battlePhases[2];
            m_currentBattlePhase.Enable(m_playerReference, 3);
            //any others behaviours on the switch
        }
        m_currentBattlePhase.SetPlayerReference(m_playerReference);
        m_currentBattlePhase.enabled = true;
        m_currentBattlePhase.SetPathfinding(m_pathfinder);
    }
}
