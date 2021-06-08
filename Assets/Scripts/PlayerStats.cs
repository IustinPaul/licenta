using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private Animator m_animator;
    private float m_currentLife = 100.0f;
    private float m_currentStamina = 100.0f;
    private float m_armor = 1.0f;
    private float m_timeSinceDmg;
    private float m_nrSeconds = 0.0f;
    void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_timeSinceDmg = m_delayDmgTaken;
    }

    void Update()
    {
        m_timeSinceDmg += Time.deltaTime;
        m_nrSeconds += Time.deltaTime;

        if(m_nrSeconds > 1.0f)
        {
            m_nrSeconds -= 1.0f;

            m_currentLife += m_lifeRegenPerSec;
            if(m_currentLife > m_totalLife)
            {
                m_currentLife = m_totalLife;
            }

            m_currentStamina += m_staminaRegenPerSec;
            if(m_currentStamina > m_totalStamina)
            {
                m_currentStamina = m_totalStamina;
            }
        }
    }

    public void TakeDmg(float dmg)
    {
        if(m_timeSinceDmg > m_delayDmgTaken)
        {
            m_currentLife -= dmg / m_armor;
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
            return true;
        }

        return false;
    }

    public bool CanAttack()
    {

        if (m_currentStamina >= m_attackStaminaCost)
        {
            m_currentStamina -= m_attackStaminaCost;
            return true;
        }

        return false;
    }
}
