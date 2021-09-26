using System;
using UnityEngine;


public class Item : MonoBehaviour
{
    [ItemCodeDescription]
    [SerializeField] private int _itemCode;
    public int ItemCode { get { return _itemCode; } set { _itemCode = value; } }
    private SpriteRenderer spriteRenderer;

    [SerializeField] private int itemQuantity;
    public int ItemQuantity { get { return itemQuantity; } }

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        if(_itemCode != 0)
        {
            Init(_itemCode);
        }
        //if(itemQuantity == 0)
        //{
        //    itemQuantity = 1;
        //}
    }

    public void Init(int itemCodeParam)
    {
        if(itemCodeParam != 0)
        {
            ItemCode = itemCodeParam;
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(ItemCode);
            spriteRenderer.sprite = itemDetails.itemSprite;

            // if item type is reapable then add ItemNudge
            if(itemDetails.itemType == ItemType.ReapableScenery)
            {
                gameObject.AddComponent<ItemNudge>();
            }
        }
    }
}
