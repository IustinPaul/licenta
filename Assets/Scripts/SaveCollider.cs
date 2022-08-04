using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveCollider : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> m_runes;
    [SerializeField] private Text m_saveText;
    private GameController m_gameC;
    private bool m_isColliding = false;
    private float m_visibilitySpeed = 0.01f;
    void Awake()
    {
        m_gameC = transform.parent.GetComponent<GameController>();
    }
    private void Update()
    {
        if (m_isColliding)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                m_gameC.SaveGame();
                m_saveText.text = "Game Saved!";
            }
            if(m_runes[0].color.a < 1)
            {
                foreach(var v in m_runes)
                {
                    Color tempColor = v.color;
                    tempColor.a += m_visibilitySpeed;
                    v.color = tempColor;
                }
            }
        }
        else if(m_runes[0].color.a > 0)
        {
            foreach (var v in m_runes)
            {
                Color tempColor = v.color;
                tempColor.a -= m_visibilitySpeed;
                v.color = tempColor;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            m_isColliding = true;
            m_saveText.transform.parent.gameObject.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            m_isColliding = false;
            m_saveText.transform.parent.gameObject.SetActive(false);
            m_saveText.text = "Savepoint!";
        }
    }

}
