using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private Text m_statsValueTxt;

    [SerializeField] private float m_totalLife = 100.0f;
    [SerializeField] private float m_attackDmg = 10.0f;
    [SerializeField] private float m_totalStamina = 100.0f;
    [SerializeField] private float m_invulnerability = 1.0f;
    [SerializeField] private float m_lifeProcRegenPerSec = 0.0f;
    [SerializeField] private float m_staminaProcRegenPerSec = 5.0f;
    [SerializeField] private float m_rollStaminaCost = 20.0f;
    [SerializeField] private float m_attackStaminaCost = 10.0f;
    [SerializeField] private float m_bleedDmg = 0.0f;
    [SerializeField] private float m_critChance = 0.0f;
    [SerializeField] private float m_bleedChance = 0.0f;
    [SerializeField] private float m_thorns = 0.0f;
    [SerializeField] private float m_blockStaminaCost = 40.0f;

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
    private int m_level = 1;

    private float m_bonusLife;
    private float m_bonusStamina;
    private float m_bonusArmor;
    private float m_bonusCritChance;
    private float m_bonusAttackDmg;
    private float m_bonusBleedDmg;
    private float m_bonusBleedChance;

    private float m_bonusRollCostProc;
    private float m_bonusStaminaProcRegen;
    private float m_bonusSpeedProc;
    private float m_bonusInvulnerabilityProc;
    private float m_bonusLifeProcRegen;
    private float m_bonusAttackCostProc;
    private float m_bonusBlockCostProc;
    private float m_bonusThornsProc;

    void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_lifeBar = transform.GetChild(1).GetChild(0).GetChild(0) as RectTransform;
        m_staminaBar = transform.GetChild(1).GetChild(1).GetChild(0) as RectTransform;
        m_lifeBarImage = m_lifeBar.GetComponent<Image>();
        m_maxSizeLifeBar = m_lifeBar.rect.width;
        m_maxSizeStaminaBar = m_staminaBar.rect.width;
        m_timeSinceDmg = m_invulnerability;
        UpdateStatsValueText();
    }

    void Update()
    {
        float dt = Time.deltaTime;
        m_timeSinceDmg += Time.deltaTime;

        m_currentLife += (m_totalLife + m_bonusLife)*(m_lifeProcRegenPerSec + m_bonusLifeProcRegen) / 100.0f * dt;
        if(m_currentLife > m_totalLife + m_bonusLife)
        {
            m_currentLife = m_totalLife + m_bonusLife;
        }

        m_currentStamina += (m_totalStamina + m_bonusStamina) * (m_staminaProcRegenPerSec + m_bonusStaminaProcRegen) /100.0f * dt;
        if(m_currentStamina > m_totalStamina + m_bonusStamina)
        {
            m_currentStamina = m_totalStamina + m_bonusStamina;
        }

        UpdateLifeBar();
        UpdateStaminaBar();
    }

    public void TakeDmg(float dmg)
    {
        if(m_timeSinceDmg > m_invulnerability)
        {
            if (m_animator.GetBool("IdleBlock"))
            {
                m_animator.SetTrigger("AttackBlocked");
                m_currentStamina -= m_blockStaminaCost - m_blockStaminaCost * m_bonusBlockCostProc / 100.0f;
                UpdateStaminaBar();
            }
            else
            {
                m_currentLife -= dmg / (m_armor + m_bonusArmor);
                UpdateLifeBar();
                m_timeSinceDmg = 0;
                if (m_currentLife <= 0)
                {
                    m_animator.SetTrigger("Death");

                    GetComponent<PlayerController>().CanMove = false;
                }
                else
                {
                    m_animator.SetTrigger("Hurt");
                }
            }
        }
    }
    public bool CanUseRoll()
    {
        float cost = m_rollStaminaCost - m_rollStaminaCost * m_bonusRollCostProc / 100.0f;
        if (m_currentStamina >= cost)
        {
            m_currentStamina -= cost;
            UpdateStaminaBar();
            return true;
        }

        return false;
    }
    public bool CanAttack()
    {
        float cost = m_attackStaminaCost - m_attackStaminaCost * m_bonusAttackCostProc / 100.0f;
        if (m_currentStamina >= cost)
        {
            m_currentStamina -= cost;
            UpdateStaminaBar();
            return true;
        }

        return false;
    }

    public bool CanBlock()
    {
        float cost = m_blockStaminaCost - m_blockStaminaCost * m_bonusBlockCostProc / 100.0f;
        if (m_currentStamina >= cost)
        {
            return true;
        }
        return false;
    }

    public void UpdateBonusStats(Item[] items)
    {
        float lifeProcent = 0;
        float staminaProcent = 0;
        float armorProcent = 0;
        float bleedDmgProcent = 0;
        float attackDmgProcent = 0;

        m_bonusLife = 0;
        m_bonusStamina = 0;
        m_bonusArmor = 0;
        m_bonusRollCostProc = 0;
        m_bonusStaminaProcRegen = 0;
        m_bonusSpeedProc = 0;
        m_bonusInvulnerabilityProc = 0;
        m_bonusLifeProcRegen = 0;
        m_bonusAttackCostProc = 0;
        m_bonusCritChance = 0;
        m_bonusBlockCostProc = 0;
        m_bonusThornsProc = 0;
        m_bonusAttackDmg = 0;
        m_bonusBleedDmg = 0;
        m_bonusBleedChance = 0;

        foreach(var v in items)
        {
            foreach(var effect in v.BaseStats)
            {
                AddToStats(effect);
            }
            foreach(var effect in v.Effects)
            {
                AddToStats(effect);
            }
        }

        m_bonusLife += (m_totalLife + m_bonusLife) * lifeProcent / 100.0f;
        m_bonusStamina += (m_totalStamina + m_bonusStamina) * staminaProcent / 100.0f;
        m_bonusArmor += (m_bonusArmor + m_armor) * armorProcent / 100.0f;
        m_bonusBleedDmg += (m_bonusBleedDmg + m_bleedDmg) * bleedDmgProcent / 100.0f;
        m_bonusAttackDmg += (m_bonusAttackDmg + m_attackDmg) * attackDmgProcent / 100.0f;
        UpdateStatsValueText();

        void AddToStats(Effect effect)
        {
            switch (effect.ApplyTo)
            {
                case "Armor":
                    if (effect.Amount.Contains("%"))
                    {
                        armorProcent += float.Parse(effect.Amount.Remove(effect.Amount.Length - 1));
                    }
                    else
                    {
                        m_bonusArmor += float.Parse(effect.Amount);
                    }
                    break;
                case "Life":
                    if (effect.Amount.Contains("%"))
                    {
                        lifeProcent += float.Parse(effect.Amount.Remove(effect.Amount.Length - 1));
                    }
                    else
                    {
                        m_bonusLife += float.Parse(effect.Amount);
                    }
                    break;
                case "Life Regen":
                    m_bonusLifeProcRegen += float.Parse(effect.Amount.Remove(effect.Amount.Length - 1));
                    break;
                case "Stamina":
                    if (effect.Amount.Contains("%"))
                    {
                        staminaProcent += float.Parse(effect.Amount.Remove(effect.Amount.Length - 1));
                    }
                    else
                    {
                        m_bonusStamina += float.Parse(effect.Amount);
                    }
                    break;
                case "Stamina Regen":
                    m_bonusStaminaProcRegen += float.Parse(effect.Amount.Remove(effect.Amount.Length - 1));
                    break;
                case "Roll Cost":
                    m_bonusRollCostProc += float.Parse(effect.Amount.Remove(effect.Amount.Length - 1));
                    break;
                case "Attack Cost":
                    m_bonusAttackCostProc += float.Parse(effect.Amount.Remove(effect.Amount.Length - 1));
                    break;
                case "Block Cost":
                    m_bonusBlockCostProc += float.Parse(effect.Amount.Remove(effect.Amount.Length - 1));
                    break;
                case "Crit. Chance":
                    m_bonusCritChance += float.Parse(effect.Amount.Remove(effect.Amount.Length - 1));
                    break;
                case "Bleed Chance":
                    m_bonusBleedChance += float.Parse(effect.Amount.Remove(effect.Amount.Length - 1));
                    break;
                case "Attack Dmg.":
                    if (effect.Amount.Contains("%"))
                    {
                        attackDmgProcent += float.Parse(effect.Amount.Remove(effect.Amount.Length - 1));
                    }
                    else
                    {
                        m_bonusAttackDmg += float.Parse(effect.Amount);
                    }
                    break;
                case "Bleed Dmg.":
                    if (effect.Amount.Contains("%"))
                    {
                        bleedDmgProcent += float.Parse(effect.Amount.Remove(effect.Amount.Length - 1));
                    }
                    else
                    {
                        m_bonusBleedDmg += float.Parse(effect.Amount);
                    }
                    break;
                case "Speed":
                    m_bonusSpeedProc += float.Parse(effect.Amount.Remove(effect.Amount.Length - 1));
                    break;
                case "Invulnerability":
                    m_bonusInvulnerabilityProc += float.Parse(effect.Amount.Remove(effect.Amount.Length - 1));
                    break;
                case "Thorns":
                    m_bonusThornsProc += float.Parse(effect.Amount.Remove(effect.Amount.Length - 1));
                    break;
                default:
                    break;
            }
        }
    }

    private void UpdateStatsValueText()
    {
        string level = m_level.ToString()+"\n";
        string life = (m_totalLife + m_bonusLife).ToString("0") + "\n";
        string lifeRegen = (m_lifeProcRegenPerSec + m_bonusLifeProcRegen).ToString("0.0") + " hp/s\n";
        string stamina = (m_totalStamina + m_bonusStamina).ToString("0") + "\n";
        string staminaRegen = (m_staminaProcRegenPerSec + m_bonusStaminaProcRegen).ToString("0.0") + " sp/s\n";
        string armor = (m_armor + m_bonusArmor).ToString("0") + "\n";
        string attackDmg = (m_attackDmg + m_bonusAttackDmg).ToString("0.0") + "\n";
        string critChance = (m_critChance + m_bonusCritChance).ToString("0.0") + "%\n";
        string bleedDmg = (m_bleedDmg + m_bonusBleedDmg).ToString("0.0") + "\n";
        string bleedChance = (m_bleedChance + m_bonusBleedChance).ToString("0.0") + "%\n";
        string attackCost = m_bonusAttackCostProc.ToString("0.0") + "%\n";
        string rollCost = m_bonusRollCostProc.ToString("0.0") + "%\n";
        string blockCost = m_bonusBlockCostProc.ToString("0.0") + "%\n";
        string invulnerability = m_bonusInvulnerabilityProc.ToString("0.0") + "%\n";
        string thornsDmg = (m_thorns + m_bonusThornsProc).ToString("0.0") + "%";

        m_statsValueTxt.text = level + life + lifeRegen + stamina + staminaRegen + armor + attackDmg + critChance + bleedDmg + bleedChance + attackCost + rollCost + blockCost + invulnerability + thornsDmg;
    }

    private void UpdateLifeBar()
    {
        m_lifeBar.sizeDelta= new Vector2(m_maxSizeLifeBar / (m_totalLife + m_bonusLife) * m_currentLife, m_lifeBar.rect.height);
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
        m_staminaBar.sizeDelta = new Vector2(m_maxSizeStaminaBar / (m_totalStamina + m_bonusStamina) * m_currentStamina, m_staminaBar.rect.height);
    }
}
