using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections.Generic;

public class UIInventoryBar : MonoBehaviour
{
    [SerializeField] private Sprite blank16x16Sprite = null;
    [SerializeField] private UIInventorySlot[] inventorySlots = null;
    public GameObject inventoryBarDraggedItem;

    [HideInInspector] public GameObject inventoryTextBoxGameObject;

    private RectTransform rectTransform;
    private bool isInventoryBarPositionBottom = true;
    public bool IsInventoryBarPositionBottom { get => isInventoryBarPositionBottom; set => isInventoryBarPositionBottom = value; }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        // Switch inventory bar position depending on player position
        SwitchInventoryBarPosition();
    }

    private void OnEnable()
    {
        EventHandler.InventoryUpdatedEvent += InventoryUpdated;
    }

    private void OnDisable()
    {
        EventHandler.InventoryUpdatedEvent -= InventoryUpdated;
    }

    private void SwitchInventoryBarPosition()
    {
        Vector3 playerViewPortPosition = Player.Instance.GetPlayerViewportPosition();

        if (playerViewPortPosition.y > 0.3f && isInventoryBarPositionBottom == false)
        {
            rectTransform.pivot = new Vector2(0.5f, 0f);
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.anchoredPosition = new Vector2(0f, 2.5f);

            isInventoryBarPositionBottom = true;
        }
        else if (playerViewPortPosition.y <= 0.3f && isInventoryBarPositionBottom == true)
        {
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = new Vector2(0f, -2.5f);

            isInventoryBarPositionBottom = false;
        }
    }

    private void InventoryUpdated(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        if (inventoryLocation == InventoryLocation.player)
        {
            ClearInventorySlots();

            if (inventorySlots.Length > 0 && inventoryList.Count > 0)
            {
                //loop through inventory slots and update with corresponding inventory list item
                for (int i = 0; i < inventorySlots.Length; i++)
                {
                    if (i < inventoryList.Count)
                    {
                        int itemCode = inventoryList[i].itemCode;

                        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);

                        if (itemDetails != null)
                        {
                            // add images and details to inventory item slot
                            inventorySlots[i].inventorySlotImage.sprite = itemDetails.itemSprite;
                            inventorySlots[i].quantityTxt.text = inventoryList[i].itemQuantity.ToString();
                            inventorySlots[i].itemDetails = itemDetails;
                            inventorySlots[i].itemQuantity = inventoryList[i].itemQuantity;
                            SetHighlightedInventorySlots(i);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }

    private void ClearInventorySlots()
    {
        if (inventorySlots.Length > 0)
        {
            // loop through inventory slots and update with blank sprite
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                inventorySlots[i].inventorySlotImage.sprite = blank16x16Sprite;
                inventorySlots[i].quantityTxt.text = "";
                inventorySlots[i].itemDetails = null;
                inventorySlots[i].itemQuantity = 0;

                SetHighlightedInventorySlots(i);
            }
        }
    }

    public void ClearHighLightOnInventorySlots()
    {
        if (inventorySlots.Length > 0)
        {
            // loop through inventory slots for highlight sprites
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].isSelected)
                {
                    inventorySlots[i].isSelected = false;
                    inventorySlots[i].inventorySlotHighlight.color = new Color(0f, 0f, 0f, 0f);
                    // Update inventory to show item as not selected
                    InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.player);
                }
            }
        }
    }

    /// <summary>
    /// Set the selected highlight if set on all inventory item position
    /// </summary>
    public void SetHighlightedInventorySlots()
    {
        if (inventorySlots.Length > 0)
        {
            // loop through inventory slots and clear highlight sprites
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                SetHighlightedInventorySlots(i);
            }
        }
    }

    /// <summary>
    /// Set the selected highlight, if set on an inventory item for a given slotItem position
    /// </summary>
    public void SetHighlightedInventorySlots(int itemPosition)
    {
        if (inventorySlots.Length > 0 && inventorySlots[itemPosition].itemDetails != null)
        {
            if (inventorySlots[itemPosition].isSelected)
            {
                inventorySlots[itemPosition].inventorySlotHighlight.color = new Color(1f, 1f, 1f, 1f);

                // Update inventory to show item as selected
                InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.player, inventorySlots[itemPosition].itemDetails.itemCode);
            }
        }
    }
}
