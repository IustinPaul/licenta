using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveCollider : MonoBehaviour
{
    private GameController m_gameC;
    private bool m_isColliding = false;
    void Awake()
    {
        m_gameC = transform.parent.GetComponent<GameController>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && m_isColliding)
        {
            m_gameC.SaveGame();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            m_isColliding = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            m_isColliding = false;
        }
    }

}
