using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownExitCollider : MonoBehaviour
{
    private GameController m_gameC;
    void Awake()
    {
        m_gameC = transform.parent.GetComponent<GameController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            m_gameC.GameStage = 0;
            m_gameC.Player.transform.position = m_gameC.PlayerStartPosEnemyMap.position;
            m_gameC.TownExitColl.SetActive(false);
            m_gameC.SpawnTrigger.SetActive(true);
            m_gameC.TownMap.SetActive(false);
            m_gameC.EnemyMap.SetActive(true);
            m_gameC.SaveCollider.SetActive(false);
        }
    }
}
