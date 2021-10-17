using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>, ISaveable
{
    private UIInventoryBar inventoryBar;

    private Dictionary<int, ItemDetails> itemDetailsDictionary;
    private int[] selectedInventoryItem; // the index of the array is the inventory list, and the value is the item code

    [SerializeField] private SO_ItemList itemList = null;

    // Index 0 of the array is the player's InventoryItem list, index 1 of the array is the chest's InventoryItem list. The same as in the InventoryLocation enum
    public List<InventoryItem>[] inventoryLists;

    // the index of the array is the inventory list (from the InventoryLocation enum), and the value is the capacity of that inventory list
    [HideInInspector] public int[] inventoryListCapacityIntArray;

    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    protected override void Awake()
    {
        base.Awake();

        // Create inventory lists
        CreateInventoryLists();

        // Create item detaillsDictionary
        CreatesItemDetailsDictionary();

        //Initialise selected inventory item array
        selectedInventoryItem = new int[(int)InventoryLocation.count];
        for (int i = 0; i < selectedInventoryItem.Length; i++)
        {
            selectedInventoryItem[i] = -1;
        }

        // Get unique ID for gameObject and create save data object;
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void Start()
    {
        inventoryBar = FindObjectOfType<UIInventoryBar>();
    }

    private void OnEnable()
    {
        ISaveableRegister();
    }

    private void OnDisable()
    {
        ISaveableDeregister();
    }

    private void CreateInventoryLists()
    {
        inventoryLists = new List<InventoryItem>[(int)InventoryLocation.count];
        for (int i = 0; i < (int)InventoryLocation.count; i++)
        {
            inventoryLists[i] = new List<InventoryItem>();
        }

        // Initialise inventory list capacity array
        inventoryListCapacityIntArray = new int[(int)InventoryLocation.count];

        // Initialise player inventory list capacity
        inventoryListCapacityIntArray[(int)InventoryLocation.player] = Settings.playerInitialInventoryCapacity;
        
       
    }

    private void RemoveItemAtPostition(List<InventoryItem> inventoryList, int itemCode, int position)
    {
        InventoryItem inventoryItem = new InventoryItem();
        int quantity = inventoryList[position].itemQuantity - 1;

        if (quantity > 0)
        {
            inventoryItem.itemQuantity = quantity;
            inventoryItem.itemCode = itemCode;
            inventoryList[position] = inventoryItem;
        }
        else
        {
            inventoryList.RemoveAt(position);
        }
    }

    /// <summary>
    /// Add a new item to the end of the inventory
    /// </summary>
    private void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int itemQuantityToAdd)
    {
        InventoryItem inventoryItem = new InventoryItem();

        inventoryItem.itemCode = itemCode;
        inventoryItem.itemQuantity = itemQuantityToAdd;
        inventoryList.Add(inventoryItem);

        //   DebugPrintInventoryList(inventoryList);
    }

    /// <summary>
    /// Add an item to a already posessed item in the inventory
    /// </summary>
    private void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int position, int itemQuantityToAdd)
    {
        InventoryItem inventoryItem = new InventoryItem();

        int quantity = inventoryList[position].itemQuantity + itemQuantityToAdd;
        inventoryItem.itemQuantity = quantity;
        inventoryItem.itemCode = itemCode;
        inventoryList[position] = inventoryItem;


        //        DebugPrintInventoryList(inventoryList);
    }

    /// <summary>
    /// Populates the itemDetailsDictionary from the scriptable object items list
    /// </summary>
    private void CreatesItemDetailsDictionary()
    {
        itemDetailsDictionary = new Dictionary<int, ItemDetails>();
        if (itemList != null)
        {
            foreach (ItemDetails item in itemList.itemDetails)
            {
                itemDetailsDictionary.Add(item.itemCode, item);
            }
        }
        else
        {
            Debug.LogWarning("INVENTORY MANAGER: SO_ItemList not set");
        }
    }

    /// <summary>
    /// Get the selected item for inventory location. Return itemCode or -1 if nothing is selected
    /// </summary>
    private int GetSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        return selectedInventoryItem[(int)inventoryLocation];
    }

    /// <summary>
    /// Returns itemDetails (from the SO_ItemList) for the currently selected item in the inventoryLocation, or null ir an item isn't selected
    /// </summary>
    /// <param name="inventoryLocation"> Location of inventory </param>
    /// <returns></returns>
    public ItemDetails GetSelectedInventoryItemDetails(InventoryLocation inventoryLocation)
    {
        int itemCode = GetSelectedInventoryItem(inventoryLocation);

        if(itemCode == -1)
        {
            return null;
        }
        else
        {
            return GetItemDetails(itemCode);
        }
    }

    ///<summary>
    /// Add an item to the inventory list for the InventoryLocation and then destroy the gameobject containing the item
    /// </summary>
    public void AddItem(InventoryLocation inventoryLocation, Item item, GameObject gameObjectToDelete, int itemQuantityToAdd = 1)
    {
        AddItem(inventoryLocation, item, itemQuantityToAdd);
        Destroy(gameObjectToDelete);
    }

    ///<summary>
    /// Add an item to the inventory list for the InventoryLocation
    /// </summary>
    public void AddItem(InventoryLocation inventoryLocation, Item item, int itemQuantityToAdd = 1)
    {
        int itemCode = item.ItemCode;
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        // Check if intentory already contains the item
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);
        if (itemPosition != -1)
        {
            AddItemAtPosition(inventoryList, itemCode, itemPosition, itemQuantityToAdd);
        }
        else
        {
            AddItemAtPosition(inventoryList, itemCode, itemQuantityToAdd);
        }

        //Send event that Inventory has been updated
        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }

    /// <summary>
    /// Add an item of item code to the inventorylist for the inventoryLocation
    /// </summary>
    public void AddItem(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventorylist = inventoryLists[(int)inventoryLocation];

        // Check if inventory already contains the item
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);
        if (itemPosition != -1)
        {
            AddItemAtPosition(inventorylist, itemCode, itemPosition, 1);
        }
        else
        {
            AddItemAtPosition(inventorylist, itemCode, 1);
        }

        // Send event that inventory has been updated
        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }

    /// <summary>
    /// Swap item at fromItem index  with item at toItem index in inventoryLocation inventory list
    /// </summary>
    public void SwapInventoryItems(InventoryLocation inventoryLocation, int fromItem, int toItem)
    {
        // if fromItem index and toItemIndex are within the bounds of the list, not the same, and >= 0
        if(fromItem < inventoryLists[(int)inventoryLocation].Count && toItem < inventoryLists[(int)inventoryLocation].Count && 
            fromItem != toItem && fromItem >= 0 && toItem >= 0)
        {
            InventoryItem fromInventoryItem = inventoryLists[(int)inventoryLocation][fromItem];
            InventoryItem toInventoryItem = inventoryLists[(int)inventoryLocation][toItem];

            inventoryLists[(int)inventoryLocation][toItem] = fromInventoryItem;
            inventoryLists[(int)inventoryLocation][fromItem] = toInventoryItem;

            // Send event that inventory has been updated
            EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
        }
    }

    /// <summary>
    /// Find if an itemCode is already in the inventory. Return the item position
    /// in the inventory list, or -1 if the item is not in the inventory
    /// </summary>
    public int FindItemInInventory(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventorylist = inventoryLists[(int)inventoryLocation];
        for (int i = 0; i < inventorylist.Count; i++)
        {
            if (inventorylist[i].itemCode == itemCode)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Remove an item from the inventory, and create a game object at the position it was dropped
    /// </summary>
    public void RemoveItem(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        // Check if inventory already contains item
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);

        if(itemPosition != -1)
        {
            RemoveItemAtPostition(inventoryList, itemCode, itemPosition);
        }

        // Send event that inventory has been updated
        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }

   

    /// <summary>
    /// Returns the itemDetails (from the SO_ItemList) for the itemCode, or null if the item code doesn´t exist
    /// </summary>
    public ItemDetails GetItemDetails(int itemCode)
    {
        ItemDetails itemDetails;
        if (itemDetailsDictionary.TryGetValue(itemCode, out itemDetails))
        {
            return itemDetails;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Get the item type description for an item type. Returns the item type description as a string for a given ItemType
    /// </summary>
    public string GetItemTypeDescription(ItemType itemType)
    {
        string itemTypeDescription;
        switch (itemType)
        {
            case ItemType.WateringTool:
                itemTypeDescription = Settings.WateringTool;
                break;
            case ItemType.HoeingTool:
                itemTypeDescription = Settings.HoeingTool;
                break;
            case ItemType.ChoppingTool:
                itemTypeDescription = Settings.ChoppingTool;
                break;
            case ItemType.BreakingTool:
                itemTypeDescription = Settings.BreakingTool;
                break;
            case ItemType.ReapingTool:
                itemTypeDescription = Settings.ReapingTool;
                break;
            case ItemType.CollectingTool:
                itemTypeDescription = Settings.CollectingTool;
                break;
            default:
                itemTypeDescription = itemType.ToString();
                break;
        }
        return itemTypeDescription;
    }

    /// <summary>
    /// Set the selected inventory item for InventoryLocation to itemCode
    /// </summary>
    public void SetSelectedInventoryItem(InventoryLocation inventoryLocation, int itemCode)
    {
        selectedInventoryItem[(int)inventoryLocation] = itemCode;
    }

    /// <summary>
    /// Clear the selected inventory item for inventoryLocation
    /// </summary>
    public void ClearSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        selectedInventoryItem[(int)inventoryLocation] = -1;
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public GameObjectSave ISaveableSave()
    {
        // Crete new scene save
        SceneSave sceneSave = new SceneSave();

        // Remove any existing scene save for persistent scene for this gameObject
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);

        // Add inventory lists array to persistent scene save
        sceneSave.listInvItemArray = inventoryLists;

        // Add inventory list capacity array to persistent scene save
        sceneSave.intArrayDictionary = new Dictionary<string, int[]>();
        sceneSave.intArrayDictionary.Add("inventoryListCapacityArray", inventoryListCapacityIntArray);

        // Add scene save for gameObject
        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);

        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if(gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            // Need to find inventory lists - start by trying to locate saveScene for game object
            if(gameObjectSave.sceneData.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
            {
                // list inventory items array exists for persistent scene
                if(sceneSave.listInvItemArray != null)
                {
                    inventoryLists = sceneSave.listInvItemArray;

                    // Send events that inventory has been updated
                    for (int i = 0; i < (int)InventoryLocation.count; i++)
                    {
                        EventHandler.CallInventoryUpdatedEvent((InventoryLocation)i, inventoryLists[i]);
                    }

                    // Clear any items player was carrying
                    Player.Instance.ClearCarriedItem();

                    // Clear any highlights on inventory bar
                    inventoryBar.ClearHighLightOnInventorySlots();
                }

                // int array dictionary exists for scene
                if(sceneSave.intArrayDictionary != null && sceneSave.intArrayDictionary.TryGetValue("inventoryListCapacityArray", out int[] inventoryCapacityArray))
                {
                    inventoryListCapacityIntArray = inventoryCapacityArray;
                }
            }
        }
    }

    public void ISaveableStoreScene(string sceneName)
    {
        // Nothing required here since the player is on a persistent scene;
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        // Nothing required here since the player is on a persistent scene;
    }

    //private void DebugPrintInventoryList(List<InventoryItem> inventoryList)
    //{
    //    foreach (InventoryItem item in inventoryList)
    //    {
    //        Debug.Log("Item description: " + Instance.GetItemDetails(item.itemCode).itemDescription + "   - Item quantity: " + item.itemQuantity);
    //    }
    //    Debug.Log("*********************************************************************");
    //}
}
