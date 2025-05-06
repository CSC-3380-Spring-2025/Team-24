using System.ComponentModel;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public Sprite image;
    public ItemType itemType;
}

public enum ItemType
{
    Hat,
    Shirt,
    Pants,
    Shoes,
    Fish
}