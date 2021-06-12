using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    [SerializeField] private float m_baseLife = 100.0f;
    [SerializeField] private float m_baseAttack = 5.0f;
    [SerializeField] private float m_dropChance = 25.0f;
    [SerializeField] private int m_level = 1;
    [SerializeField] private List<ItemsPool> m_itemTypes;
    [SerializeField] private GameObject m_droppable;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    [ContextMenu("DropItem")]
    public void DropItem()
    {
        if(m_dropChance >= Random.Range(1, 101))
        {
            Item item = Instantiate(m_droppable, transform.position, transform.rotation).GetComponent<Item>();

            var itemType =  m_itemTypes[Random.Range(0, m_itemTypes.Count)];
            item.Name = itemType.ItemType;
            item.Level = m_level;
            item.Sprite = itemType.Sprites[Random.Range(0, itemType.Sprites.Length)];
            for(int i = 0; i < itemType.BaseStats.Length; i++)
            {
                string applyTo = itemType.BaseStats[i].ApplyTo;
                string amount = itemType.BaseStats[i].Amount;

                if (itemType.BaseStats[i].Amount.Contains("%"))
                {
                    amount = (float.Parse(amount.Remove(amount.Length - 1)) * item.Level).ToString("0.0") + "%";
                }
                else
                {
                    amount = (float.Parse(amount) * item.Level).ToString("0.0");
                }

                item.BaseStats.Add(new Effect(applyTo, amount));
            }

            for(int i = 0; i < 3; i++)
            {
                int stat = Random.Range(0, itemType.Effects.Length);

                string applyTo = itemType.Effects[stat].ApplyTo;
                string amount = itemType.Effects[stat].Amount;

                if (itemType.Effects[stat].Amount.Contains("%"))
                {
                    amount = (float.Parse(amount.Remove(amount.Length - 1)) * item.Level).ToString("0.0") + "%";
                }
                else
                {
                    amount = (float.Parse(amount) * item.Level).ToString("0.0");
                }

                item.Effects.Add(new Effect(applyTo, amount));
            }

            item.transform.GetComponent<SpriteRenderer>().sprite = item.Sprite;
            
        }

    }
}
