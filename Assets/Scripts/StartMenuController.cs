using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuController : MonoBehaviour
{
    [SerializeField] private Color m_selectColor = Color.white;
    [SerializeField] private Color m_unselectColor = Color.black;
    [SerializeField] private List<Text> m_buttons;
    [SerializeField] private GameObject nextScreen;

    private int m_selectedIndex = 0;
    private void Awake()
    {
        Cursor.visible = false;
        m_buttons[m_selectedIndex].color = m_selectColor;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            m_buttons[m_selectedIndex].color = m_unselectColor;
            m_selectedIndex++;
            if (m_selectedIndex >= m_buttons.Count)
                m_selectedIndex = 0;
            m_buttons[m_selectedIndex].color = m_selectColor;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            m_buttons[m_selectedIndex].color = m_unselectColor;
            m_selectedIndex--;
            if (m_selectedIndex < 0)
                m_selectedIndex = m_buttons.Count - 1;
            m_buttons[m_selectedIndex].color = m_selectColor;
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            switch (m_selectedIndex)
            {
                case 0:
                    PlayerPrefs.SetInt("IsLoaded", 0);
                    nextScreen.SetActive(true);
                    gameObject.SetActive(false);
                    break;
                case 1:
                    PlayerPrefs.SetInt("IsLoaded", 1);
                    SceneManager.LoadScene(1);
                    break;
                case 2:
                    Application.Quit();
                    break;
                default:
                    break;
            }
        }
    }
}
