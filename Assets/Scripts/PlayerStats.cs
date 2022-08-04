using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private Text m_statsValueTxt;
    [SerializeField] private Text m_scoreText;
    [SerializeField] private Color m_highLife;
    [SerializeField] private Color m_mediumLife;
    [SerializeField] private Color m_lowLife;
    [SerializeField] private AudioClip m_blockSound;
    [SerializeField] private AudioClip m_hurtSound;
    [SerializeField] private AudioClip m_deathSound;

    public float TotalLife = 100.0f;
    public float AttackDmg = 10.0f;
    public float TotalStamina = 100.0f;
    public float Invulnerability = 1.0f;
    public float LifeProcRegenPerSec = 0.0f;
    public float StaminaProcRegenPerSec = 5.0f;
    public float RollStaminaCost = 20.0f;
    public float AttackStaminaCost = 10.0f;
    public float BleedDmg = 0.0f;
    public float CritChance = 0.0f;
    public float BleedChance = 0.0f;
    public float Thorns = 0.0f;
    public float Speed = 4.0f;
    public float BlockStaminaCost = 40.0f;
    public float XpNextLevel = 10.0f;
    public float Armor = 1.0f;
    public float CurrentXp = 0.0f;
    public int Level = 1;
    public int Score = 0;

    private Animator m_animator;
    private RectTransform m_lifeBar;
    private RectTransform m_staminaBar;
    private RectTransform m_xpBar;
    private Image m_lifeBarImage;
    private AudioSource m_audioSource;

    private float m_maxSizeLifeBar;
    private float m_maxSizeStaminaBar;
    private float m_maxSizeXpBar;
    private float m_currentLife = 100.0f;
    private float m_currentStamina = 100.0f;
    private float m_timeSinceDmg;

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
        m_audioSource = GetComponent<AudioSource>();
        m_lifeBar = transform.GetChild(1).GetChild(0).GetChild(0) as RectTransform;
        m_staminaBar = transform.GetChild(1).GetChild(1).GetChild(0) as RectTransform;
        m_xpBar = transform.GetChild(1).GetChild(3).GetChild(0) as RectTransform;
        m_lifeBarImage = m_lifeBar.GetComponent<Image>();
        m_maxSizeLifeBar = m_lifeBar.rect.width;
        m_maxSizeStaminaBar = m_staminaBar.rect.width;
        m_maxSizeXpBar = m_xpBar.rect.width;
        m_timeSinceDmg = Invulnerability;
        UpdateXpBar();
        UpdateStatsValueText();
    }

    void Update()
    {
        float dt = Time.deltaTime;
        m_timeSinceDmg += dt;
        m_scoreText.text = Score.ToString();

        m_currentLife += (TotalLife + m_bonusLife)*(LifeProcRegenPerSec + m_bonusLifeProcRegen) / 100.0f * dt;
        if(m_currentLife > TotalLife + m_bonusLife)
        {
            m_currentLife = TotalLife + m_bonusLife;
        }

        m_currentStamina += (TotalStamina + m_bonusStamina) * (StaminaProcRegenPerSec + m_bonusStaminaProcRegen) /100.0f * dt;
        if(m_currentStamina > TotalStamina + m_bonusStamina)
        {
            m_currentStamina = TotalStamina + m_bonusStamina;
        }

        UpdateLifeBar();
        UpdateStaminaBar();
    }
    public int GetPlayerLevel()
    {
        return Level;
    }

    public float GetPlayerDmg()
    {
        return AttackDmg + m_bonusAttackDmg;
    }

    public float GetPlayerCritChance()
    {
        return CritChance + m_bonusCritChance;
    }

    public float GetPlayerSpeed()
    {
        return Speed + Speed * m_bonusSpeedProc / 100.0f;
    }

    public float GetPlayerBleedDmg()
    {
        return BleedDmg + m_bonusBleedDmg;
    }
    public float GetPlayerBleedChance()
    {
        return BleedChance + m_bonusBleedChance;
    }
    public float GetPlayerThorns()
    {
        return Thorns + m_bonusThornsProc;
    }

    public void Restore()
    {
        m_currentLife = TotalLife + m_bonusLife;
        m_currentStamina = TotalStamina + m_bonusStamina;
    }

    public void ReceiveXP(float xp)
    {
        CurrentXp += xp;
        while (CurrentXp / XpNextLevel >= 1)
        {
            Level++;
            CurrentXp -= XpNextLevel;
            XpNextLevel *= 2;
            TotalLife += 10.0f;
            m_currentLife = TotalLife + m_bonusLife;
            TotalStamina += 10.0f;
            m_currentStamina = TotalStamina + m_bonusStamina;
            AttackDmg++;
            Score += 5 * Level;
            UpdateStatsValueText();
        }
        UpdateXpBar();
    }

    public void TakeDmg(float dmg)
    {
        if(m_timeSinceDmg > Invulnerability && m_currentLife > 0)
        {
            if (m_animator.GetBool("IdleBlock"))
            {
                m_audioSource.PlayOneShot(m_blockSound);
                //m_animator.SetTrigger("AttackBlocked");
                m_currentStamina -= BlockStaminaCost - BlockStaminaCost * m_bonusBlockCostProc / 100.0f;
                UpdateStaminaBar();
            }
            else
            {
                if(Armor+m_bonusArmor < dmg/2)
                    m_currentLife -= (Armor + m_bonusArmor < dmg / 2 ? dmg - (Armor + m_bonusArmor) : dmg * dmg / (3* (Armor + m_bonusArmor)));
                UpdateLifeBar();
                m_timeSinceDmg = 0;
                if (m_currentLife <= 0)
                {
                    m_audioSource.PlayOneShot(m_deathSound);
                    m_animator.SetBool("IsDead", true);
                    m_animator.SetTrigger("Death");
                    File.Delete(Application.persistentDataPath + "/gamesave.save");

                    GetComponent<PlayerController>().CanMove = false;
                }
                else
                {
                    m_audioSource.PlayOneShot(m_hurtSound);
                    m_animator.SetTrigger("Hurt");
                }
            }
        }
    }
    public bool CanUseRoll()
    {
        float cost = RollStaminaCost - RollStaminaCost * m_bonusRollCostProc / 100.0f;
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
        float cost = AttackStaminaCost - AttackStaminaCost * m_bonusAttackCostProc / 100.0f;
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
        float cost = BlockStaminaCost - BlockStaminaCost * m_bonusBlockCostProc / 100.0f;
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

        m_bonusLife += (TotalLife + m_bonusLife) * lifeProcent / 100.0f;
        m_bonusStamina += (TotalStamina + m_bonusStamina) * staminaProcent / 100.0f;
        m_bonusArmor += (m_bonusArmor + Armor) * armorProcent / 100.0f;
        m_bonusBleedDmg += (m_bonusBleedDmg + BleedDmg) * bleedDmgProcent / 100.0f;
        m_bonusAttackDmg += (m_bonusAttackDmg + AttackDmg) * attackDmgProcent / 100.0f;
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

    public void UpdateStatsValueText()
    {
        string level = Level.ToString()+"\n";
        string life = (TotalLife + m_bonusLife).ToString("0") + "\n";
        string lifeRegen = (LifeProcRegenPerSec + m_bonusLifeProcRegen).ToString("0.0") + " hp/s\n";
        string stamina = (TotalStamina + m_bonusStamina).ToString("0") + "\n";
        string staminaRegen = (StaminaProcRegenPerSec + m_bonusStaminaProcRegen).ToString("0.0") + " sp/s\n";
        string armor = (Armor + m_bonusArmor).ToString("0") + "\n";
        string attackDmg = (AttackDmg + m_bonusAttackDmg).ToString("0.0") + "\n";
        string critChance = (CritChance + m_bonusCritChance).ToString("0.0") + "%\n";
        string bleedDmg = (BleedDmg + m_bonusBleedDmg).ToString("0.0") + "\n";
        string bleedChance = (BleedChance + m_bonusBleedChance).ToString("0.0") + "%\n";
        string attackCost = m_bonusAttackCostProc.ToString("0.0") + "%\n";
        string rollCost = m_bonusRollCostProc.ToString("0.0") + "%\n";
        string blockCost = m_bonusBlockCostProc.ToString("0.0") + "%\n";
        string invulnerability = m_bonusInvulnerabilityProc.ToString("0.0") + "%\n";
        string thornsDmg = (Thorns + m_bonusThornsProc).ToString("0.0") + "%";

        m_statsValueTxt.text = level + life + lifeRegen + stamina + staminaRegen + armor + attackDmg + critChance + bleedDmg + bleedChance + attackCost + rollCost + blockCost + invulnerability + thornsDmg;
    }

    public void UpdateXpBar()
    {
        m_xpBar.sizeDelta = new Vector2(m_maxSizeXpBar / XpNextLevel * CurrentXp, m_xpBar.rect.height);
    }

    public void ActivateDeathScreen()
    {
        transform.GetChild(1).GetChild(6).gameObject.SetActive(true);
        transform.GetChild(1).GetChild(6).GetComponent<DeathMenuScript>().SetScore(Score);
    }

    private void UpdateLifeBar()
    {
        m_lifeBar.sizeDelta= new Vector2(m_maxSizeLifeBar / (TotalLife + m_bonusLife) * m_currentLife, m_lifeBar.rect.height);
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
        m_staminaBar.sizeDelta = new Vector2(m_maxSizeStaminaBar / (TotalStamina + m_bonusStamina) * m_currentStamina, m_staminaBar.rect.height);
    }
}
