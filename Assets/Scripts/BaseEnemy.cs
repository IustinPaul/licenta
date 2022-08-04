using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseEnemy : MonoBehaviour
{
    [SerializeField] private float m_baseLife = 100.0f;
    [SerializeField] private float m_baseAttack = 5.0f;
    [SerializeField] private float m_baseXpValue = 2.0f;
    [SerializeField] private float m_dropChance = 25.0f;
    [SerializeField] private float m_speed = 0.5f;
    [SerializeField] private float m_lifePerLevel = 5.0f;
    [SerializeField] private float m_attackPerLevel = 1.0f;
    [SerializeField] private float m_bleedDuration = 5.0f;
    [SerializeField] private int m_level = 1;
    [SerializeField] private int m_score = 10;
    [SerializeField] private List<ItemsPool> m_itemTypes;
    [SerializeField] private GameObject m_droppable;

    [SerializeField] private Color m_highLife = Color.green;
    [SerializeField] private Color m_mediumLife = Color.yellow;
    [SerializeField] private Color m_lowLife = Color.red;

    private float m_maxSizeLifeBar;
    private float m_currentLife;
    private float m_bleedTimer;
    private float m_bleedDmgPerSecond;
    private PlayerStats m_playerStats;
    private Animator m_animator;
    private BoxCollider2D m_collider;
    private RectTransform m_lifeBar;
    private Image m_lifeBarContent;

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
        m_lifeBar = transform.GetChild(0).GetChild(0).GetChild(0) as RectTransform;
        m_lifeBarContent = m_lifeBar.GetComponent<Image>();
        m_maxSizeLifeBar = m_lifeBar.rect.width;
        m_speed = Random.Range(0.9f, m_speed);
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        m_bleedTimer += dt;
        if (m_bleedTimer <= m_bleedDuration)
            TakeDmg(m_bleedDmgPerSecond * dt, 0, false);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttack"))
        {
            TakeDmg(m_playerStats.GetPlayerDmg(), m_playerStats.GetPlayerCritChance());
            if (m_playerStats.GetPlayerBleedChance() >= Random.Range(10,1001) / 10.0f)
            {
                m_bleedDmgPerSecond = m_playerStats.GetPlayerBleedDmg();
                m_bleedTimer = 0.0f;
            }
        }
    }

    public float GetEnemySpeed()
    {
        return m_speed;
    }
    public float GetEnemyDmg()
    {
        return m_baseAttack;
    }

    public void TakeDmg(float dmg, float critChance, bool withAnimation = true)
    {
        if (Random.Range(10, 1001) < critChance * 10)
            dmg *= 2;
        m_currentLife -= dmg;
        UpdateLifeBar();
        if(m_currentLife <= 0)
        {
            m_collider.enabled = false;
            m_animator.SetBool("IsDead", true);
            m_animator.SetTrigger("Death");
            m_playerStats.ReceiveXP(m_baseXpValue);
            m_playerStats.Score += m_score;
        }
        else if(withAnimation && dmg > 0)
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
    private void UpdateLifeBar()
    {
        m_lifeBar.sizeDelta = new Vector2(m_maxSizeLifeBar / m_baseLife * m_currentLife, m_lifeBar.rect.height);
        if (m_maxSizeLifeBar / 2 < m_lifeBar.rect.width)
        {
            m_lifeBarContent.color = m_highLife;
        }
        else if (m_maxSizeLifeBar / 4 < m_lifeBar.rect.width)
        {

            m_lifeBarContent.color = m_mediumLife;
        }
        else
        {

            m_lifeBarContent.color = m_lowLife;
        }
    }
}
