using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    private Canvas canvas;
    private Camera mainCamera;
    private GridCursor gridCursor = null;
    [SerializeField] private Image cursorImage = null;
    [SerializeField] private RectTransform cursorRectTransform = null;
    [SerializeField] private Sprite greenCursorSprite = null;
    [SerializeField] private Sprite transparentCursorSprite = null;
    

    [HideInInspector] public bool isCursorEnabled = false;
    [HideInInspector] public bool isCursorPositionValid = false;
    [HideInInspector] public ItemType selectedItemType;
    [HideInInspector] public float itemUseRadius = 0f;

    void Start()
    {
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
        gridCursor = GetComponent<GridCursor>();
    }

    void Update()
    {
        if(isCursorEnabled)
        {
            DisplayCursor();
        }
    }

    private void DisplayCursor()
    {
        // Get position for cursor
        Vector3 cursorWorldPosition = GetWorldPositionForCursor();

        // Set cursor sprite
        SetCursorValidity(cursorWorldPosition, Player.Instance.GetPlayerCenterPosition());

        // Get rectTransform position for cursor
        cursorRectTransform.position = GetRectTransformPositionForCursor();
    }

    private void SetCursorValidity(Vector3 cursorPosition, Vector3 playerPosition)
    {
        SetCursorToValid();
        // Check use radius corners
        {
            if (
                cursorPosition.x > (playerPosition.x + itemUseRadius / 2f) && cursorPosition.y > (playerPosition.y + itemUseRadius / 2f)
                ||
                cursorPosition.x > (playerPosition.x + itemUseRadius / 2f) && cursorPosition.y < (playerPosition.y - itemUseRadius / 2f)
                ||
                cursorPosition.x < (playerPosition.x - itemUseRadius / 2f) && cursorPosition.y > (playerPosition.y + itemUseRadius / 2f)
                ||
                cursorPosition.x < (playerPosition.x - itemUseRadius / 2f) && cursorPosition.y < (playerPosition.y - itemUseRadius / 2f)
                )
            {
                SetCursorToInvalid();
                return;
            }

            //Check item use radius is valid
            if (Mathf.Abs(cursorPosition.x - playerPosition.x) > itemUseRadius
                || Mathf.Abs(cursorPosition.y - playerPosition.y) > itemUseRadius)
            {
                SetCursorToInvalid();
                return;
            }

            // Get selected item details
            ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

            if (itemDetails == null)
            {
                SetCursorToInvalid();
                return;
            }

            // Determine cursor validity based on InventoryItem selected and what object the cursor is over
            switch (itemDetails.itemType)
            {
                case ItemType.WateringTool:
                case ItemType.HoeingTool:
                case ItemType.ChoppingTool:
                case ItemType.BreakingTool:
                case ItemType.ReapingTool:
                case ItemType.CollectingTool:
                    if (!SetCursorValidityTool(cursorPosition, playerPosition, itemDetails))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;

                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Sets the cursor as either valid or invalid for the tool for the target. Returns true if valid or false if invalid
    /// </summary>
       private bool SetCursorValidityTool(Vector3 cursorPosition, Vector3 playerPosition, ItemDetails itemDetails)
    {
        // Switch on tool
        switch (itemDetails.itemType)
        {
            case ItemType.ReapingTool:
                return SetCursorValidityReapingTool(cursorPosition, playerPosition, itemDetails);
            default:
                return false;
        }
    }

    private bool SetCursorValidityReapingTool(Vector3 cursorPosition, Vector3 playerPosition, ItemDetails itemDetails)
    {
        List<Item> itemList = new List<Item>();
        if(HelperMethods.GetComponentAtCursorLocation<Item>(out itemList, cursorPosition))
        {
            if(itemList.Count > 0)
            {
                foreach (Item item in itemList)
                {
                    if(InventoryManager.Instance.GetItemDetails(item.ItemCode).itemType == ItemType.ReapableScenery)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Set the cursor to be valid
    /// </summary>
    private void SetCursorToValid()
    {
        cursorImage.sprite = greenCursorSprite;
        isCursorPositionValid = true;
        gridCursor.DisableCursor();
    }

    /// <summary>
    /// Sets the cursor to be invalid
    /// </summary>
    private void SetCursorToInvalid()
    {
        cursorImage.sprite = transparentCursorSprite;
        isCursorPositionValid = false;
        gridCursor.EnableCursor();
    }


    private Vector2 GetRectTransformPositionForCursor()
    {
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        return RectTransformUtility.PixelAdjustPoint(screenPosition, cursorRectTransform, canvas);
    }

    public Vector3 GetWorldPositionForCursor()
    {
        Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }

    public void EnableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 1f);
        isCursorEnabled = true;
    }

    public void DisableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 0f);
        isCursorEnabled = false;
    }
}
