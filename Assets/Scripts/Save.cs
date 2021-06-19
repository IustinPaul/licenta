using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save
{
    public float TotalLife;
    public float AttackDmg;
    public float TotalStamina;
    public float Invulnerability;
    public float LifeProcRegenPerSec;
    public float StaminaProcRegenPerSec;
    public float RollStaminaCost;
    public float AttackStaminaCost;
    public float BleedDmg;
    public float CritChance;
    public float BleedChance;
    public float Thorns;
    public float Speed;
    public float BlockStaminaCost;
    public float XpNextLevel;
    public float CurrentXp;
    public float Armor;
    public int Level;
    public List<SaveItem> Items = new List<SaveItem>();
}
