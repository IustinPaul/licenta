using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float m_totalLife = 100.0f;
    [SerializeField] private float m_attackDmg = 10.0f;
    [SerializeField] private float m_stamina = 100.0f;
    [SerializeField] private float m_delayDmgTaken = 1.0f;

    private Animator m_animator;
    private float m_currentLife = 100.0f;
    private float m_armor = 1.0f;
    private float m_timeSinceDmg;
    void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_timeSinceDmg = m_delayDmgTaken;
    }

    void Update()
    {
        m_timeSinceDmg += Time.deltaTime;
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
}
