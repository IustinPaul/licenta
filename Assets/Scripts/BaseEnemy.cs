using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    [SerializeField] private float m_baseLife = 100.0f;
    [SerializeField] private float m_baseAttack = 5.0f;
    [SerializeField] private float m_baseXpValue = 2.0f;
    [SerializeField] private float m_dropChance = 25.0f;
    [SerializeField] private float m_speed = 4.0f;
    [SerializeField] private float m_lifePerLevel = 5.0f;
    [SerializeField] private float m_attackPerLevel = 1.0f;
    [SerializeField] private float m_bleedDuration = 5.0f;
    [SerializeField] private int m_level = 1;
    [SerializeField] private List<ItemsPool> m_itemTypes;
    [SerializeField] private GameObject m_droppable;

    private float m_currentLife;
    private float m_bleedTimer;
    private float m_bleedDmgPerSecond;
    private PlayerStats m_playerStats;
    private Animator m_animator;
    private BoxCollider2D m_collider;

    void Awake()
    {
        m_collider = GetComponent<BoxCollider2D>();
        m_playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
        m_level = Random.Range(m_playerStats.GetPlayerLevel(), m_playerStats.GetPlayerLevel() + 2);
        m_baseLife += m_level * m_lifePerLevel;
        m_baseAttack += m_level * m_attackPerLevel;
        m_baseXpValue *= m_level; 
        m_currentLife = m_baseLife;
        m_bleedTimer = m_bleedDuration;
        m_animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        m_bleedTimer += dt;
        if (m_bleedTimer <= m_bleedDuration)
            TakeDmg(m_bleedDmgPerSecond * dt, false);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttack"))
        {
            TakeDmg(m_playerStats.GetPlayerDmg());
            if (m_playerStats.GetPlayerBleedChance() >= Random.Range(10,1001) / 10.0f)
            {
                m_bleedDmgPerSecond = m_playerStats.GetPlayerBleedDmg();
                m_bleedTimer = 0.0f;
            }
        }
    }

    public void TakeDmg(float dmg, bool withAnimation = true)
    {
        m_currentLife -= dmg;
        if(m_currentLife <= 0)
        {
            m_collider.enabled = false;
            m_animator.SetTrigger("Death");
            m_playerStats.ReceiveXP(m_baseXpValue);
            //disable movement

        }
        else if(withAnimation)
        {
            m_animator.SetTrigger("Hurt");
        }
    }

    public void DropItem()
    {
        if (m_dropChance >= Random.Range(10, 1001) / 10.0f)
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
        Destroy(gameObject);

    }
}
