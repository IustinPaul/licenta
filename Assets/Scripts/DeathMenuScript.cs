using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathMenuScript : MonoBehaviour
{
    [SerializeField] private Color m_selectColor;
    [SerializeField] private Color m_unselectColor;
    [SerializeField] private Image m_bkg;
    [SerializeField] private GameObject m_title;
    [SerializeField] private Text m_score;
    [SerializeField] private GameObject m_eYNTitle;
    [SerializeField] private Text m_typeField;
    [SerializeField] private GameObject m_error;
    [SerializeField] private GameObject m_continue;
    [SerializeField] private List<Text> m_buttonsMenu;
    [SerializeField] private GameController m_gameC;

    private float m_alpha;
    private int m_selectedIndex = 0;
    private int m_scoreValue;
    private bool m_isTyping = true;

    void Update()
    {
        m_alpha += Time.deltaTime;
        var tempColor = m_bkg.color;
        tempColor.a = m_alpha/2.0f;
        m_bkg.color = tempColor;

        if (m_isTyping)
        {
            if (m_alpha > 1)
            {
                m_title.SetActive(true);
                m_score.gameObject.SetActive(true);
                m_eYNTitle.SetActive(true);
                m_typeField.gameObject.SetActive(true);
                m_continue.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (m_typeField.text.Length > 0)
                {
                    m_gameC.SaveCurrentScore(m_typeField.text, m_scoreValue);
                    m_eYNTitle.SetActive(false);
                    m_typeField.gameObject.SetActive(false);
                    m_continue.SetActive(false);
                    m_error.SetActive(false);
                    m_isTyping = false;
                    foreach(var v in m_buttonsMenu)
                    {
                        v.gameObject.SetActive(true);
                    }
                }
                else
                {
                    m_error.SetActive(true);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if(m_typeField.text.Length > 0)
                {
                    m_typeField.text = m_typeField.text.Remove(m_typeField.text.Length - 1);
                }
            }
            else if(m_typeField.text.Length < 6)
            {
                if(Input.GetKeyDown(KeyCode.A))
                {
                    m_typeField.text += "A";
                }
                else if(Input.GetKeyDown(KeyCode.B))
                {
                    m_typeField.text += "B";
                }
                else if (Input.GetKeyDown(KeyCode.C))
                {
                    m_typeField.text += "C";
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    m_typeField.text += "D";
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    m_typeField.text += "E";
                }
                else if (Input.GetKeyDown(KeyCode.F))
                {
                    m_typeField.text += "F";
                }
                else if (Input.GetKeyDown(KeyCode.G))
                {
                    m_typeField.text += "G";
                }
                else if (Input.GetKeyDown(KeyCode.H))
                {
                    m_typeField.text += "H";
                }
                else if (Input.GetKeyDown(KeyCode.I))
                {
                    m_typeField.text += "I";
                }
                else if (Input.GetKeyDown(KeyCode.J))
                {
                    m_typeField.text += "J";
                }
                else if (Input.GetKeyDown(KeyCode.K))
                {
                    m_typeField.text += "K";
                }
                else if (Input.GetKeyDown(KeyCode.L))
                {
                    m_typeField.text += "L";
                }
                else if (Input.GetKeyDown(KeyCode.M))
                {
                    m_typeField.text += "M";
                }
                else if (Input.GetKeyDown(KeyCode.N))
                {
                    m_typeField.text += "N";
                }
                else if (Input.GetKeyDown(KeyCode.O))
                {
                    m_typeField.text += "O";
                }
                else if (Input.GetKeyDown(KeyCode.P))
                {
                    m_typeField.text += "P";
                }
                else if (Input.GetKeyDown(KeyCode.Q))
                {
                    m_typeField.text += "Q";
                }
                else if (Input.GetKeyDown(KeyCode.R))
                {
                    m_typeField.text += "R";
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    m_typeField.text += "S";
                }
                else if (Input.GetKeyDown(KeyCode.T))
                {
                    m_typeField.text += "T";
                }
                else if (Input.GetKeyDown(KeyCode.U))
                {
                    m_typeField.text += "U";
                }
                else if (Input.GetKeyDown(KeyCode.V))
                {
                    m_typeField.text += "V";
                }
                else if (Input.GetKeyDown(KeyCode.W))
                {
                    m_typeField.text += "W";
                }
                else if (Input.GetKeyDown(KeyCode.X))
                {
                    m_typeField.text += "X";
                }
                else if (Input.GetKeyDown(KeyCode.Y))
                {
                    m_typeField.text += "Y";
                }
                else if (Input.GetKeyDown(KeyCode.Z))
                {
                    m_typeField.text += "Z";
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                m_buttonsMenu[m_selectedIndex].color = m_unselectColor;
                m_selectedIndex++;
                if (m_selectedIndex >= m_buttonsMenu.Count)
                    m_selectedIndex = 0;
                m_buttonsMenu[m_selectedIndex].color = m_selectColor;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                m_buttonsMenu[m_selectedIndex].color = m_unselectColor;
                m_selectedIndex--;
                if (m_selectedIndex < 0)
                    m_selectedIndex = m_buttonsMenu.Count - 1;
                m_buttonsMenu[m_selectedIndex].color = m_selectColor;
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                switch (m_selectedIndex)
                {
                    case 0:
                        PlayerPrefs.SetInt("IsLoaded", 0);
                        SceneManager.LoadScene(1);
                        break;
                    case 1:
                        SceneManager.LoadScene(0);
                        break;
                    default:
                        break;
                }
            }
        }

    }

    public void SetScore(int score)
    {
        m_score.text += score.ToString();
        m_scoreValue = score;
    }
}
