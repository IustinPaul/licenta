using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private BoxCollider2D m_townExitColl;
    [SerializeField] private BoxCollider2D m_enemyMapExitColl;
    [SerializeField] private BoxCollider2D m_spawnTrigger;
    [SerializeField] private GameObject m_townMap;
    [SerializeField] private GameObject m_enemyMap;
    [SerializeField] private Transform m_enemiesOnMap;
    [SerializeField] private Transform m_playerStartPosEnemyMap;
    [SerializeField] private Transform m_playerStartPosTown;
    [SerializeField] private PlayerStats m_player;
    [SerializeField] private List<GameObject> m_enemiesTypes;
    [SerializeField] private List<Transform> m_spawnLocations;

    private int m_gameStage = 8;

    void Update()
    {
        if(m_enemiesOnMap.childCount == 0 && m_gameStage < 8)
        {
            m_enemyMapExitColl.enabled = true;
        }
        else
        {
            m_enemyMapExitColl.enabled = false;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (m_townExitColl.enabled)
            {
                m_gameStage = 0;
                m_player.transform.position = m_playerStartPosEnemyMap.position;
                m_townExitColl.enabled = false;
                m_spawnTrigger.enabled = true;
                m_townMap.SetActive(false);
                m_enemyMap.SetActive(true);
            }
            else if (m_spawnTrigger.enabled)
            {
                m_spawnTrigger.enabled = false;
                SpawnEnemies();
            }
            else if (m_enemyMapExitColl.enabled)
            {
                m_gameStage++;
                if (m_gameStage > 7)
                {
                    m_enemyMapExitColl.enabled = false;
                    m_player.transform.position = m_playerStartPosTown.position;
                    m_player.Restore();
                    m_townExitColl.enabled = true;
                    m_spawnTrigger.enabled = false;
                    m_enemyMap.SetActive(false);
                    m_townMap.SetActive(true);
                }
                else
                {
                    m_player.transform.position = m_playerStartPosEnemyMap.position;
                    m_spawnTrigger.enabled = true;
                }
            }
        }
    }

    private void SpawnEnemies()
    {
        switch (m_gameStage)
        {
            case 0:
                for (int i = 0; i < 3 + m_player.GetPlayerLevel(); i++)
                {
                    int pos = Random.Range(0, m_spawnLocations.Count);
                    if (pos % 2 == 0)
                    {
                        Instantiate(m_enemiesTypes[0], new Vector3(m_spawnLocations[pos].position.x, Random.Range(-20, 17), 0), transform.rotation, m_enemiesOnMap);
                    }
                    else
                    {
                        Instantiate(m_enemiesTypes[0], new Vector3(Random.Range(-16, 21), m_spawnLocations[pos].position.y, 0), transform.rotation, m_enemiesOnMap);
                    }
                }
                break;
            case 1:
                for (int i = 0; i < 5 + m_player.GetPlayerLevel(); i++)
                {
                    int enemy = Random.Range(0, 2);
                    int pos = Random.Range(0, m_spawnLocations.Count);
                    if (pos % 2 == 0)
                    {
                        Instantiate(m_enemiesTypes[enemy], new Vector3(m_spawnLocations[pos].position.x, Random.Range(-20, 17), 0), transform.rotation, m_enemiesOnMap);
                    }
                    else
                    {
                        Instantiate(m_enemiesTypes[enemy], new Vector3(Random.Range(-16, 21), m_spawnLocations[pos].position.y, 0), transform.rotation, m_enemiesOnMap);
                    }
                }
                break;
            case 2:
                for (int i = 0; i < 3 + m_player.GetPlayerLevel(); i++)
                {
                    int pos = Random.Range(0, m_spawnLocations.Count);
                    if (pos % 2 == 0)
                    {
                        Instantiate(m_enemiesTypes[2], new Vector3(m_spawnLocations[pos].position.x, Random.Range(-20, 17), 0), transform.rotation, m_enemiesOnMap);
                    }
                    else
                    {
                        Instantiate(m_enemiesTypes[2], new Vector3(Random.Range(-16, 21), m_spawnLocations[pos].position.y, 0), transform.rotation, m_enemiesOnMap);
                    }
                }
                break;
            case 3:
                for (int i = 0; i < 5 + m_player.GetPlayerLevel(); i++)
                {
                    int enemy = Random.Range(3, 5);
                    int pos = Random.Range(0, m_spawnLocations.Count);
                    if (pos % 2 == 0)
                    {
                        Instantiate(m_enemiesTypes[enemy], new Vector3(m_spawnLocations[pos].position.x, Random.Range(-20, 17), 0), transform.rotation, m_enemiesOnMap);
                    }
                    else
                    {
                        Instantiate(m_enemiesTypes[enemy], new Vector3(Random.Range(-16, 21), m_spawnLocations[pos].position.y, 0), transform.rotation, m_enemiesOnMap);
                    }
                }
                break;
            case 4:
                for (int i = 0; i < 7 + m_player.GetPlayerLevel(); i++)
                {
                    int enemy = Random.Range(4,6);
                    int pos = Random.Range(0, m_spawnLocations.Count);
                    if (pos % 2 == 0)
                    {
                        Instantiate(m_enemiesTypes[enemy], new Vector3(m_spawnLocations[pos].position.x, Random.Range(-20, 17), 0), transform.rotation, m_enemiesOnMap);
                    }
                    else
                    {
                        Instantiate(m_enemiesTypes[enemy], new Vector3(Random.Range(-16, 21), m_spawnLocations[pos].position.y, 0), transform.rotation, m_enemiesOnMap);
                    }
                }
                break;
            case 5:
                for (int i = 0; i < 1 + m_player.GetPlayerLevel()/5; i++)
                {
                    int pos = Random.Range(0, m_spawnLocations.Count);
                    if (pos % 2 == 0)
                    {
                        Instantiate(m_enemiesTypes[6], new Vector3(m_spawnLocations[pos].position.x, Random.Range(-20, 17), 0), transform.rotation, m_enemiesOnMap);
                    }
                    else
                    {
                        Instantiate(m_enemiesTypes[6], new Vector3(Random.Range(-16, 21), m_spawnLocations[pos].position.y, 0), transform.rotation, m_enemiesOnMap);
                    }
                }
                break;
            case 6:
                for (int i = 0; i < 7 + m_player.GetPlayerLevel(); i++)
                {
                    int enemy = Random.Range(0,5);
                    int pos = Random.Range(0, m_spawnLocations.Count);
                    if (pos % 2 == 0)
                    {
                        Instantiate(m_enemiesTypes[enemy], new Vector3(m_spawnLocations[pos].position.x, Random.Range(-20, 17), 0), transform.rotation, m_enemiesOnMap);
                    }
                    else
                    {
                        Instantiate(m_enemiesTypes[enemy], new Vector3(Random.Range(-16, 21), m_spawnLocations[pos].position.y, 0), transform.rotation, m_enemiesOnMap);
                    }
                }
                break;
            case 7:
                for (int i = 0; i < 1 + m_player.GetPlayerLevel() / 10; i++)
                {
                    int pos = Random.Range(0, m_spawnLocations.Count);
                    if (pos % 2 == 0)
                    {
                        Instantiate(m_enemiesTypes[7], new Vector3(m_spawnLocations[pos].position.x, Random.Range(-20, 17), 0), transform.rotation, m_enemiesOnMap);
                    }
                    else
                    {
                        Instantiate(m_enemiesTypes[7], new Vector3(Random.Range(-16, 21), m_spawnLocations[pos].position.y, 0), transform.rotation, m_enemiesOnMap);
                    }
                }
                break;
        }
    }
}
