using UnityEngine;

[System.Serializable]
public class ItemDetails
{
    public int itemCode; // unique number
    public ItemType itemType;
    public string itemDescription;
    public Sprite itemSprite;
    public string itemLongDescription;
    public short itemUseGridRadius; //if target is positioned on a gridbase
    public float itemUseRadius; //if target is NOT positioned on a gridbase
    public bool isStartingItem;
    public bool canBePickedUp;
    public bool canBeDropped;
    public bool canBeEaten;
    public bool canBeCarried;
}
