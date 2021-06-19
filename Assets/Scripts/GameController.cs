using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private List<GameObject> m_enemiesTypes;
    [SerializeField] private List<Transform> m_spawnLocations;
    [SerializeField] private List<Sprite> m_itemSprites;
    [SerializeField] private Transform m_enemiesOnMap;
    public GameObject TownExitColl;
    public GameObject EnemyMapExitColl;
    public GameObject SpawnTrigger;
    public GameObject SaveCollider;
    public GameObject TownMap;
    public GameObject EnemyMap;
    public Transform PlayerStartPosEnemyMap;
    public Transform PlayerStartPosTown;
    public PlayerStats Player;

    [HideInInspector] public int GameStage = 8;

    private void Start()
    {
        if(PlayerPrefs.GetInt("IsLoaded") == 1)
        {
            LoadGame();
        }
    }

    void Update()
    {
        if(m_enemiesOnMap.childCount == 0 && GameStage < 8)
        {
            EnemyMapExitColl.SetActive(true);
        }
        else
        {
            EnemyMapExitColl.SetActive(false);
        }

    }

    public Save CreateSaveGameObject()
    {
        Save save = new Save();
        save.TotalLife = Player.TotalLife;
        save.AttackDmg = Player.AttackDmg;
        save.TotalStamina = Player.TotalStamina;
        save.Invulnerability = Player.Invulnerability;
        save.LifeProcRegenPerSec = Player.LifeProcRegenPerSec;
        save.StaminaProcRegenPerSec = Player.StaminaProcRegenPerSec;
        save.RollStaminaCost = Player.RollStaminaCost;
        save.AttackStaminaCost = Player.AttackStaminaCost;
        save.BleedDmg = Player.BleedDmg;
        save.CritChance = Player.CritChance;
        save.BleedChance = Player.BleedChance;
        save.Thorns = Player.Thorns;
        save.Speed = Player.Speed;
        save.BlockStaminaCost = Player.BlockStaminaCost;
        save.XpNextLevel = Player.XpNextLevel;
        save.CurrentXp = Player.CurrentXp;
        save.Armor = Player.Armor;
        save.Level = Player.Level;

        InventoryController ic = Player.transform.GetChild(1).GetChild(2).GetComponent<InventoryController>();
        for(int i =0; i< ic.GetHeigth(); i++)
        {
            for(int j = 0; j < ic.GetWidth(); j++)
            {
                if(!ic.Inventory[i,j].Name.Equals(""))
                {
                    SaveItem saveItem = new SaveItem();
                    saveItem.Name = ic.Inventory[i, j].Name;
                    saveItem.Level = ic.Inventory[i, j].Level;
                    for(int k = 0; k<m_itemSprites.Count; k++)
                    {
                        if(m_itemSprites[k] == ic.Inventory[i, j].Sprite)
                        {
                            saveItem.Sprite = k;
                            break;
                        }
                    }
                    saveItem.BaseStats = ic.Inventory[i, j].BaseStats;
                    saveItem.Effects = ic.Inventory[i, j].Effects;
                    saveItem.isEquiped = false;
                    for(int k = 0; k < ic.Equiped.Length; k++)
                    {
                        if(ic.Equiped[k].PosInInv == ic.Inventory[i, j])
                        {
                            saveItem.isEquiped = true;
                            break;
                        }
                    }
                    save.Items.Add(saveItem);
                }
            }
        }

        return save;
    }

    public void SaveGame()
    {
        Save save = CreateSaveGameObject();

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, save);
        file.Close();

        Debug.Log("Game Saved");
    }

    public void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            file.Close();

            Player.transform.position = PlayerStartPosTown.position;

            Player.TotalLife = save.TotalLife;
            Player.AttackDmg = save.AttackDmg;
            Player.TotalStamina = save.TotalStamina;
            Player.Invulnerability = save.Invulnerability;
            Player.LifeProcRegenPerSec = save.LifeProcRegenPerSec;
            Player.StaminaProcRegenPerSec = save.StaminaProcRegenPerSec;
            Player.RollStaminaCost = save.RollStaminaCost;
            Player.AttackStaminaCost = save.AttackStaminaCost;
            Player.BleedDmg = save.BleedDmg;
            Player.CritChance = save.CritChance;
            Player.BleedChance = save.BleedChance;
            Player.Thorns = save.Thorns;
            Player.Speed = save.Speed;
            Player.BlockStaminaCost = save.BlockStaminaCost;
            Player.XpNextLevel = save.XpNextLevel;
            Player.CurrentXp = save.CurrentXp;
            Player.Armor = save.Armor;
            Player.Level = save.Level;

            InventoryController ic = Player.transform.GetChild(1).GetChild(2).GetComponent<InventoryController>();
            foreach(var v in save.Items)
            {
                Item item = gameObject.AddComponent<Item>();
                item.Name = v.Name;
                item.Level = v.Level;
                item.Sprite = m_itemSprites[v.Sprite];
                foreach(var v1 in v.BaseStats)
                {
                    item.BaseStats.Add(v1);
                }
                foreach(var v1 in v.Effects)
                {
                    item.Effects.Add(v1);
                }
                var itemInInv = ic.AddToInventory(item);
                if (v.isEquiped)
                {
                    ic.EquipItem(itemInInv);
                }
                Destroy(item);
            }

            Player.UpdateStatsValueText();
            Player.UpdateXpBar();

            Debug.Log("Game Loaded");
        }
        else
        {
            Debug.Log("No game saved!");
        }
    }

    public void SpawnEnemies()
    {
        switch (GameStage)
        {
            case 0:
                for (int i = 0; i < 3 + Player.GetPlayerLevel(); i++)
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
                for (int i = 0; i < 5 + Player.GetPlayerLevel(); i++)
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
                for (int i = 0; i < 3 + Player.GetPlayerLevel(); i++)
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
                for (int i = 0; i < 5 + Player.GetPlayerLevel(); i++)
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
                for (int i = 0; i < 7 + Player.GetPlayerLevel(); i++)
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
                for (int i = 0; i < 1 + Player.GetPlayerLevel()/5; i++)
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
                for (int i = 0; i < 7 + Player.GetPlayerLevel(); i++)
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
                for (int i = 0; i < 1 + Player.GetPlayerLevel() / 10; i++)
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
