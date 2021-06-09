using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float m_totalLife = 100.0f;
    [SerializeField] private float m_attackDmg = 10.0f;
    [SerializeField] private float m_totalStamina = 100.0f;
    [SerializeField] private float m_delayDmgTaken = 1.0f;
    [SerializeField] private float m_lifeRegenPerSec = 0.0f;
    [SerializeField] private float m_staminaRegenPerSec = 5.0f;
    [SerializeField] private float m_rollStaminaCost = 20.0f;
    [SerializeField] private float m_attackStaminaCost = 10.0f;
    [SerializeField] private Color m_highLife;
    [SerializeField] private Color m_mediumLife;
    [SerializeField] private Color m_lowLife;

    private Animator m_animator;
    private RectTransform m_lifeBar;
    private RectTransform m_staminaBar;
    private Image m_lifeBarImage;
    private float m_maxSizeLifeBar;
    private float m_maxSizeStaminaBar;
    private float m_currentLife = 100.0f;
    private float m_currentStamina = 100.0f;
    private float m_armor = 1.0f;
    private float m_timeSinceDmg;
    void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_lifeBar = transform.GetChild(1).GetChild(0).GetChild(0) as RectTransform;
        m_staminaBar = transform.GetChild(1).GetChild(1).GetChild(0) as RectTransform;
        m_lifeBarImage = m_lifeBar.GetComponent<Image>();
        m_maxSizeLifeBar = m_lifeBar.rect.width;
        m_maxSizeStaminaBar = m_staminaBar.rect.width;
        m_timeSinceDmg = m_delayDmgTaken;
    }

    void Update()
    {
        float dt = Time.deltaTime;
        m_timeSinceDmg += Time.deltaTime;

        m_currentLife += m_lifeRegenPerSec * dt;
        if(m_currentLife > m_totalLife)
        {
            m_currentLife = m_totalLife;
        }

        m_currentStamina += m_staminaRegenPerSec * dt;
        if(m_currentStamina > m_totalStamina)
        {
            m_currentStamina = m_totalStamina;
        }

        UpdateLifeBar();
        UpdateStaminaBar();
    }

    public void TakeDmg(float dmg)
    {
        if(m_timeSinceDmg > m_delayDmgTaken)
        {
            m_currentLife -= dmg / m_armor;
            UpdateLifeBar();
            m_timeSinceDmg = 0;
            if(m_currentLife <= 0)
            {
                m_animator.SetTrigger("Death");

                //Problem: Sliding after death if it was moving
                GetComponent<PlayerController>().enabled = false;
            }
            else
            {
                m_animator.SetTrigger("Hurt");
            }
        }
    }

    public bool CanUseRoll()
    {
        if (m_currentStamina >= m_rollStaminaCost)
        {
            m_currentStamina -= m_rollStaminaCost;
            UpdateStaminaBar();
            return true;
        }

        return false;
    }

    public bool CanAttack()
    {

        if (m_currentStamina >= m_attackStaminaCost)
        {
            m_currentStamina -= m_attackStaminaCost;
            UpdateStaminaBar();
            return true;
        }

        return false;
    }

    private void UpdateLifeBar()
    {
        m_lifeBar.sizeDelta= new Vector2(m_maxSizeLifeBar / m_totalLife * m_currentLife, m_lifeBar.rect.height);
        if(m_maxSizeLifeBar/2 < m_lifeBar.rect.width)
        {
            m_lifeBarImage.color = m_highLife;
        }
        else if(m_maxSizeLifeBar / 4 < m_lifeBar.rect.width)
        {

            m_lifeBarImage.color = m_mediumLife;
        }
        else
        {

            m_lifeBarImage.color = m_lowLife;
        }
    }

    private void UpdateStaminaBar()
    {
        m_staminaBar.sizeDelta = new Vector2(m_maxSizeStaminaBar / m_totalStamina * m_currentStamina, m_staminaBar.rect.height);
    }
}
