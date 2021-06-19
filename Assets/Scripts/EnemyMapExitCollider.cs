using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMapExitCollider : MonoBehaviour
{
    private GameController m_gameC;
    void Awake()
    {
        m_gameC = transform.parent.GetComponent<GameController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            m_gameC.GameStage++;
            if(m_gameC.GameStage > 7)
            {
                m_gameC.EnemyMapExitColl.SetActive(false);
                m_gameC.Player.transform.position = m_gameC.PlayerStartPosTown.position;
                m_gameC.Player.Restore();
                m_gameC.TownExitColl.SetActive(true);
                m_gameC.SpawnTrigger.SetActive(false);
                m_gameC.EnemyMap.SetActive(false);
                m_gameC.TownMap.SetActive(true);
                m_gameC.SaveCollider.SetActive(true);
            }
            else
            {
                m_gameC.Player.transform.position = m_gameC.PlayerStartPosEnemyMap.position;
                m_gameC.SpawnTrigger.SetActive(true);
            }
        }
    }
}
