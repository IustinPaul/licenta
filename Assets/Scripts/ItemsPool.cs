using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemPool", menuName = "ScriptableObject/ItemsPool", order = 1)]
public class ItemsPool : ScriptableObject
{
    public string ItemType;
    public Effect[] BaseStats;
    public Sprite[] Sprites;
    public Effect[] Effects;
}

[System.Serializable]
public class Effect
{
    public string ApplyTo;
    public string Amount;
    public Effect(string applyTo, string amount)
    {
        ApplyTo = applyTo;
        Amount = amount;
    }
}