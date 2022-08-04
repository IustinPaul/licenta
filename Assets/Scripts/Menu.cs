using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private Color m_selectColor = Color.white;
    [SerializeField] private Color m_unselectColor = Color.black;
    [SerializeField] private List<Text> m_buttons;
    [SerializeField] private GameObject m_errorText;

    private int m_selectedIndex = 0;

    private void Awake()
    {
        m_buttons[m_selectedIndex].color = m_selectColor;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 1;
            gameObject.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
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
            Time.timeScale = 1;
            switch (m_selectedIndex)
            {
                case 0:
                    gameObject.SetActive(false);
                    m_errorText.SetActive(false);
                    break;
                case 1:
                    if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
                    {
                        PlayerPrefs.SetInt("IsLoaded", 1);
                        SceneManager.LoadScene(1);
                    }
                    else
                    {
                        m_errorText.SetActive(true);
                        Time.timeScale = 0;
                    }
                    break;
                case 2:
                    SceneManager.LoadScene(0);
                    break;
                default:
                    break;
            }
        }
    }
}
