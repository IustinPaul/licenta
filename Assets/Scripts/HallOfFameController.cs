using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HallOfFameController : MonoBehaviour
{
    [SerializeField] private GameObject m_startMenu;
    [SerializeField] private Text m_names;
    [SerializeField] private Text m_separators;
    [SerializeField] private Text m_values;
    private void Update()
    {
        if (Input.anyKeyDown)
        {
            m_startMenu.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (File.Exists(Application.persistentDataPath + "/scores.save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/scores.save", FileMode.Open);
            SavedScores savedScores = (SavedScores)bf.Deserialize(file);
            file.Close();

            string names = "";
            string values = "";
            string separators = "";

            for(int i = 0; i < savedScores.Names.Count; i++)
            {
                names += savedScores.Names[i] + "\n";
                values += savedScores.Scores[i].ToString() + "\n";
                separators += "-\n";
            }

            m_names.text = names;
            m_values.text = values;
            m_separators.text = separators;
        }
    }
}
