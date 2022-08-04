using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private int m_inventoryHeigth;
    [SerializeField] private int m_inventoryWidth;
    [SerializeField] private GameObject m_selector;
    [SerializeField] private PlayerController m_playerController;
    [SerializeField] private PlayerStats m_playerStats;
    private int m_selectorX = 0;
    private int m_selectorY = 0;
    private float m_toDestroy = 0;
    public Item[,] Inventory;
    public Item[] Equiped;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            m_playerController.CanMove = true;
            gameObject.SetActive(false);
        }

        if (Input.GetKey(KeyCode.W) && Inventory[m_selectorY, m_selectorX].Name != "" && 
            !Inventory[m_selectorY, m_selectorX].transform.GetChild(1).gameObject.activeSelf)
        {
            m_toDestroy += Time.deltaTime;
            if (m_toDestroy > 1)
            {
                Item item = Inventory[m_selectorY, m_selectorX];
                Image img = item.transform.GetChild(0).GetComponent<Image>();
                img.sprite = null;
                img.gameObject.SetActive(false);
                item.Name = "";
                item.Level = 0;
                item.Sprite = null;
                item.BaseStats.Clear();
                item.Effects.Clear();
                item.PosInInv = null;
                SetItemInfo(item);
                m_playerStats.Score += 10;
            }
        }
        else
        {
            m_toDestroy = 0;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            m_selectorX++;
            if(m_selectorX>= m_inventoryWidth)
            {
                m_selectorX = 0;
            }
            m_selector.transform.position = Inventory[m_selectorY, m_selectorX].transform.position;
            SetItemInfo(Inventory[m_selectorY, m_selectorX]);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {

            m_selectorX--;
            if (m_selectorX < 0)
            {
                m_selectorX = m_inventoryWidth - 1;
            }
            m_selector.transform.position = Inventory[m_selectorY, m_selectorX].transform.position;
            SetItemInfo(Inventory[m_selectorY, m_selectorX]);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            m_selectorY++;
            if (m_selectorY >= m_inventoryHeigth)
            {
                m_selectorY = 0;
            }
            m_selector.transform.position = Inventory[m_selectorY, m_selectorX].transform.position;
            SetItemInfo(Inventory[m_selectorY, m_selectorX]);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            m_selectorY--;
            if (m_selectorY < 0)
            {
                m_selectorY = m_inventoryHeigth - 1;
            }
            m_selector.transform.position = Inventory[m_selectorY, m_selectorX].transform.position;
            SetItemInfo(Inventory[m_selectorY, m_selectorX]);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            if(Inventory[m_selectorY,m_selectorX].Name != "")
            {
                EquipItem(Inventory[m_selectorY, m_selectorX]);
                m_playerStats.UpdateBonusStats(Equiped);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            var obj = m_selector.transform.GetChild(0).gameObject;
            obj.SetActive(!obj.activeSelf);
        }

    }

    private void OnEnable()
    {
        SetItemInfo(Inventory[m_selectorY, m_selectorX]);
    }

    public void Initialized()
    {
        Inventory = new Item[m_inventoryHeigth, m_inventoryWidth];
        Equiped = new Item[10];
        for (int i = 0; i < m_inventoryHeigth; i++)
        {
            for (int j = 0; j < m_inventoryWidth; j++)
            {
                Inventory[i, j] = transform.GetChild(0).GetChild(i * m_inventoryWidth + j).GetComponent<Item>();
            }
        }
        for (int i = 0; i < 10; i++)
        {
            Equiped[i] = transform.GetChild(1).GetChild(1 + i).GetComponent<Item>();
        }
    }

    public void EquipItem(Item item)
    {
        int slot = 0;

        switch (item.Name)
        {
            case "Helmet":
                slot = 0;
                break;
            case "Necklace":
                slot = 1;
                break;
            case "Gloves":
                slot = 2;
                break;
            case "Chest":
                slot = 3;
                break;
            case "Ring":
                slot = 4;
                break;
            case "Pants":
                slot = 5;
                break;
            case "Boots":
                slot = 6;
                break;
            case "Sword":
                slot = 7;
                break;
            case "Shield":
                slot = 8;
                break;
            case "QuickItem":
                slot = 9;
                break;
            default:
                break;

        }

        Item itemSlot = Equiped[slot];
        Image img = itemSlot.transform.GetChild(0).GetComponent<Image>();
        if (itemSlot.PosInInv == item)
        {
            item.transform.GetChild(1).gameObject.SetActive(false);
            img.sprite = null;
            img.gameObject.SetActive(false);
            itemSlot.Name = "";
            itemSlot.Level = 0;
            itemSlot.Sprite = null;
            itemSlot.BaseStats.Clear();
            itemSlot.Effects.Clear();
            itemSlot.PosInInv = null;
        }
        else
        {
            if(itemSlot.PosInInv != null)
            {
                itemSlot.PosInInv.transform.GetChild(1).gameObject.SetActive(false);
                itemSlot.BaseStats.Clear();
                itemSlot.Effects.Clear();
            }
            item.transform.GetChild(1).gameObject.SetActive(true);
            img.sprite = item.Sprite;
            img.gameObject.SetActive(true);
            itemSlot.Name = item.Name;
            itemSlot.Level = item.Level;
            itemSlot.Sprite = item.Sprite;
            foreach(var v in item.BaseStats)
            {
                itemSlot.BaseStats.Add(new Effect(v.ApplyTo, v.Amount));
            }
            foreach(var v in item.Effects)
            {
                itemSlot.Effects.Add(new Effect(v.ApplyTo, v.Amount));
            }
            itemSlot.PosInInv = item;
        }
    }

    private void SetItemInfo(Item item)
    {
        string txt1 = "Base stats\n";
        string txt2 = "\n";

        foreach(var v in item.BaseStats)
        {
            txt1 += v.ApplyTo + "\n";
            txt2 += v.Amount + "\n";
        }
        txt1 += "\nSpecials\n";
        txt2 += "\n\n";
        foreach(var v in item.Effects)
        {

            txt1 += v.ApplyTo + "\n";
            txt2 += v.Amount + "\n";
        }

        m_selector.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = txt1;
        m_selector.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = txt2;
    }

    public Item AddToInventory(Item item)
    {
        for(int i = 0; i < m_inventoryHeigth; i++)
        {
            for(int  j = 0; j < m_inventoryWidth; j++)
            {
                if (Inventory[i, j].Name.Equals(""))
                {
                    Inventory[i, j].Name = item.Name;
                    Inventory[i, j].Level = item.Level;
                    Inventory[i, j].Sprite = item.Sprite;
                    foreach(var v in item.BaseStats)
                    {
                        Inventory[i, j].BaseStats.Add(new Effect(v.ApplyTo, v.Amount));
                    }
                    foreach(var v in item.Effects)
                    {
                        Inventory[i, j].Effects.Add(new Effect(v.ApplyTo, v.Amount));
                    }
                    Image img = Inventory[i, j].transform.GetChild(0).GetComponent<Image>();
                    img.sprite = item.Sprite;
                    img.gameObject.SetActive(true);
                    return Inventory[i,j];
                }
            }
        }
        return null;
    }

    public int GetHeigth()
    {
        return m_inventoryHeigth;
    }
    public int GetWidth()
    {
        return m_inventoryWidth;
    }
}
