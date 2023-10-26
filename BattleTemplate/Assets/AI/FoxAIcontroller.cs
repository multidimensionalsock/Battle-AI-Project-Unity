using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxAIcontroller : MonoBehaviour
{
    Pathfinding m_pathfinder;
    [SerializeField] GameObject m_playerReference;
    // Start is called before the first frame update
    void Start()
    {
        m_pathfinder = GetComponent<Pathfinding>();
        m_pathfinder.SetNewNavigation(pathfindingState.flee, m_playerReference);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
