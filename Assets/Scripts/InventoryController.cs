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
    private int m_selectorX = 0;
    private int m_selectorY = 0;
    private Item[,] m_inventory;
    private Item[] m_equiped;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            m_playerController.CanMove = true;
            gameObject.SetActive(false);
        }


        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            m_selectorX++;
            if(m_selectorX>= m_inventoryWidth)
            {
                m_selectorX = 0;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {

            m_selectorX--;
            if (m_selectorX < 0)
            {
                m_selectorX = m_inventoryWidth - 1;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            m_selectorY++;
            if (m_selectorY >= m_inventoryHeigth)
            {
                m_selectorY = 0;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            m_selectorY--;
            if (m_selectorY < 0)
            {
                m_selectorY = m_inventoryHeigth - 1;
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            if(m_inventory[m_selectorY,m_selectorX].Name != "")
            {
                EquipItem(m_inventory[m_selectorY, m_selectorX]);
            }
        }

        m_selector.transform.position = m_inventory[m_selectorY, m_selectorX].transform.position;

    }

    public void Initialized()
    {
        m_inventory = new Item[m_inventoryHeigth, m_inventoryWidth];
        m_equiped = new Item[10];
        for (int i = 0; i < m_inventoryHeigth; i++)
        {
            for (int j = 0; j < m_inventoryWidth; j++)
            {
                m_inventory[i, j] = transform.GetChild(0).GetChild(i * m_inventoryWidth + j).GetComponent<Item>();
            }
        }
        for (int i = 0; i < 10; i++)
        {
            m_equiped[i] = transform.GetChild(1).GetChild(1 + i).GetComponent<Item>();
        }
    }

    private void EquipItem(Item item)
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

        Item itemSlot = m_equiped[slot];
        Image img = itemSlot.transform.GetChild(0).GetComponent<Image>();
        if (itemSlot.PosInInv == item)
        {
            //Scoate E ul de la itemul echipat
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
            if(itemSlot != null)
            {
                //Scoate E ul de la itemul echipat
                itemSlot.BaseStats.Clear();
                itemSlot.Effects.Clear();
            }
            //Pune E ul pe noul item
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

    public bool AddToInventory(Item item)
    {
        for(int i = 0; i < m_inventoryHeigth; i++)
        {
            for(int  j = 0; j < m_inventoryWidth; j++)
            {
                if (m_inventory[i, j].Name.Equals(""))
                {
                    m_inventory[i, j].Name = item.Name;
                    m_inventory[i, j].Level = item.Level;
                    m_inventory[i, j].Sprite = item.Sprite;
                    foreach(var v in item.BaseStats)
                    {
                        m_inventory[i, j].BaseStats.Add(new Effect(v.ApplyTo, v.Amount));
                    }
                    foreach(var v in item.Effects)
                    {
                        m_inventory[i, j].Effects.Add(new Effect(v.ApplyTo, v.Amount));
                    }
                    Image img = m_inventory[i, j].transform.GetChild(0).GetComponent<Image>();
                    img.sprite = item.Sprite;
                    img.gameObject.SetActive(true);
                    return true;
                }
            }
        }
        return false;
    }
}
