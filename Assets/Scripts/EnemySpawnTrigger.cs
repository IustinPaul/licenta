using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnTrigger : MonoBehaviour
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
            m_gameC.SpawnTrigger.SetActive(false);
            m_gameC.SpawnEnemies();
        }
    }
}
