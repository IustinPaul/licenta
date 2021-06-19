using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveItem
{
    public string Name;
    public int Level;
    public int Sprite;
    public List<Effect> BaseStats = new List<Effect>();
    public List<Effect> Effects = new List<Effect>();
    public bool isEquiped;
}
