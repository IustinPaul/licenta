using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string Name;
    public int Level;
    public Sprite Sprite;
    public List<Effect> BaseStats = new List<Effect>();
    public List<Effect> Effects = new List<Effect>();
    public Item PosInInv = null;

}
