using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Player : Singleton<Player>
{
    private WaitForSeconds useToolAnimationPause;
    private WaitForSeconds liftToolAnimationPause;
    private WaitForSeconds pickAnimationPause;
    private WaitForSeconds afterUseToolAnimationPause;
    private WaitForSeconds afterLiftToolAnimationPause;
    private WaitForSeconds afterPickAnimationPause;

    // Movement parameters
    private float xInput;
    private float yInput;
    private bool isCarrying = false;
    private bool isIdle;
    private bool isLiftingToolDown;
    private bool isLiftingToolLeft;
    private bool isLiftingToolRight;
    private bool isLiftingToolUp;
    private bool isRunning;
    private bool isUsingToolDown;
    private bool isUsingToolLeft;
    private bool isUsingToolRight;
    private bool isUsingToolUp;
    private bool isSwingingToolDown;
    private bool isSwingingToolLeft;
    private bool isSwingingToolRight;
    private bool isSwingingToolUp;
    private bool isWalking;
    private bool isPickingUp;
    private bool isPickingDown;
    private bool isPickingLeft;
    private bool isPickingRight;
    private ToolEffect toolEffect = ToolEffect.none;
    private bool playerToolUseDisabled = false;

    private AnimationOverrides animationOverrides;

    private Camera mainCamera;

    private Rigidbody2D myRigidbody2D;
#pragma warning disable 414
    private Direction playerDirection;

#pragma warning restore 414

    private GridCursor gridCursor;
    private Cursor cursor;
    private Transform parentItem;

    private List<CharacterAttribute> characterAttributeCustomisationList;

    [Tooltip("Should be populated in the prefab with equipped item sprite renderer")]
    [SerializeField] private SpriteRenderer equippedItemSpriteRenderer = null;

    [SerializeField] private GameObject itemPrefab = null;

    // Player attributes that can be swapped
    private CharacterAttribute armsCharacterAttribute;
    private CharacterAttribute toolCharacterAttribute;

    private float movementSpeed;

    private bool _playerInputIsDisabled = false;
    public bool PlayerInputIsDisabled { get { return _playerInputIsDisabled; } }

    protected override void Awake()
    {
        base.Awake();
        myRigidbody2D = GetComponent<Rigidbody2D>();

        animationOverrides = GetComponentInChildren<AnimationOverrides>();

        //Initialise swappable character attributes
        armsCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.arms, PartVariantColor.none, PartVariantType.none);
        toolCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.tool, PartVariantColor.none, PartVariantType.hoe);

        // Initialise character attribute list
        characterAttributeCustomisationList = new List<CharacterAttribute>();

        // get reference to mainCamera;
        mainCamera = Camera.main;
    }

    private void Start()
    {
        gridCursor = FindObjectOfType<GridCursor>().GetComponent<GridCursor>();
        cursor = FindObjectOfType<Cursor>().GetComponent<Cursor>();
        useToolAnimationPause = new WaitForSeconds(Settings.useToolAnimationPause);
        afterUseToolAnimationPause = new WaitForSeconds(Settings.afterUseToolAnimationPause);
        liftToolAnimationPause = new WaitForSeconds(Settings.liftToolAnimationPause);
        afterLiftToolAnimationPause = new WaitForSeconds(Settings.afterLiftToolAnimationPause);
        pickAnimationPause = new WaitForSeconds(Settings.pickAnimationPause);
        afterPickAnimationPause = new WaitForSeconds(Settings.afterPickAnimationPause);
    }

    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadFadeOutEvent += DisablePlayerInputAndResetMovement;
        EventHandler.AfterSceneLoadFadeInEvent += EnablePlayerInput;
        EventHandler.AfterSceneLoadEvent += SceneLoaded;
    }

    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadFadeOutEvent -= DisablePlayerInputAndResetMovement;
        EventHandler.AfterSceneLoadFadeInEvent -= EnablePlayerInput;
        EventHandler.AfterSceneLoadEvent -= SceneLoaded;
    }

    private void Update()
    {
        #region Player Input

        if (!PlayerInputIsDisabled)
        {
            ResetAnimationTriggers();
            PlayerMovementInput();
            PlayerWalkInput();
            PlayerClickInput();

            // Send event to any listeners for player movement input
            EventHandler.CallMovementEvent(xInput, yInput, isWalking, isRunning, isIdle, isCarrying, toolEffect,
                isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
                isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
                isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
                isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
                false, false, false, false);
        }
        #endregion

        PlayerTestInput();
    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        Vector2 move = new Vector2(xInput * movementSpeed * Time.deltaTime, yInput * movementSpeed * Time.deltaTime);
        myRigidbody2D.MovePosition(myRigidbody2D.position + move);
    }

    private void ResetAnimationTriggers()
    {
        isPickingRight = false;
        isPickingLeft = false;
        isPickingUp = false;
        isPickingDown = false;
        isUsingToolRight = false;
        isUsingToolLeft = false;
        isUsingToolUp = false;
        isUsingToolDown = false;
        isLiftingToolRight = false;
        isLiftingToolLeft = false;
        isLiftingToolUp = false;
        isLiftingToolDown = false;
        isSwingingToolRight = false;
        isSwingingToolLeft = false;
        isSwingingToolUp = false;
        isSwingingToolDown = false;
        toolEffect = ToolEffect.none;
    }

    private void PlayerMovementInput()
    {
        yInput = Input.GetAxisRaw("Vertical");
        xInput = Input.GetAxisRaw("Horizontal");

        if (yInput != 0 && xInput != 0)
        {
            xInput = xInput * 0.71f;
            yInput = yInput * 0.71f;
        }

        if (xInput != 0 || yInput != 0)
        {
            isRunning = true;
            isWalking = false;
            isIdle = false;
            movementSpeed = Settings.runningSpeed;

            // Capture player direction for save game
            if (xInput < 0)
            {
                playerDirection = Direction.left;
            }
            else if (xInput > 0)
            {
                playerDirection = Direction.right;
            }
            else if (yInput < 0)
            {
                playerDirection = Direction.down;
            }
            else
            {
                playerDirection = Direction.up;
            }
        }
        else if (xInput == 0 && yInput == 0)
        {
            isRunning = false;
            isWalking = false;
            isIdle = true;
        }
    }

    private void PlayerWalkInput()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            isRunning = false;
            isWalking = true;
            isIdle = false;
            movementSpeed = Settings.walkingSpeed;
        }
        else
        {
            isRunning = true;
            isWalking = false;
            isIdle = false;
            movementSpeed = Settings.runningSpeed;
        }
    }
    private void PlayerClickInput()
    {
        if (!playerToolUseDisabled)
        {
            if (Input.GetMouseButton(0))
            {
                if (gridCursor.CursorIsEnabled || cursor.isCursorEnabled)
                {
                    // Get Cursor grid position
                    Vector3Int cursorGridPosition = gridCursor.GetGridPositionForCursor();
                    // Get player grid position
                    Vector3Int playerGridPosition = gridCursor.GetGridPositionForPlayer();
                    ProcessPlayerClickInput(cursorGridPosition, playerGridPosition);
                }
            }
        }
    }

    private void ProcessPlayerClickInput(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        ResetMovement();

        Vector3Int playerDirection = GetPlayerClickDirection(cursorGridPosition, playerGridPosition);

        // Get gridPropertyDetails at cursor position ( the GridCursor validation routine, ensures that gridPropertyDetails are not null
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);

        // Get selected item details
        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        if (itemDetails != null)
        {
            switch (itemDetails.itemType)
            {
                case ItemType.Seed:
                    if (Input.GetMouseButtonDown(0))
                    {
                        ProcessPlayerClickInputSeed(gridPropertyDetails, itemDetails);
                    }
                    break;
                case ItemType.Commodity:
                    if (Input.GetMouseButtonDown(0))
                    {
                        ProcessPlayerClickInputCommodity(itemDetails);
                    }
                    break;
                case ItemType.HoeingTool:
                case ItemType.WateringTool:
                case ItemType.ReapingTool:
                case ItemType.CollectingTool:
                case ItemType.ChoppingTool:
                case ItemType.BreakingTool:
                case ItemType.DiggingTool:
                    ProcessPlayerClickInputTool(gridPropertyDetails, itemDetails, playerDirection);
                    break;

                case ItemType.none:
                    break;
                case ItemType.count:
                    break;
                default:
                    break;
            }
        }
    }

    private Vector3Int GetPlayerClickDirection(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        if (cursorGridPosition.x > playerGridPosition.x)
        {
            return Vector3Int.right;
        }
        else if (cursorGridPosition.x < playerGridPosition.x)
        {
            return Vector3Int.left;
        }
        else if (cursorGridPosition.y > playerGridPosition.y)
        {
            return Vector3Int.up;
        }
        else
        {
            return Vector3Int.down;
        }
    }

    private Vector3Int GetPlayerDirection(Vector3 cursorPosition, Vector3 playerPosition)
    {
        if (
            cursorPosition.x > playerPosition.x
            &&
            cursorPosition.y < (playerPosition.y + cursor.itemUseRadius / 2f)
            &&
            cursorPosition.y > (playerPosition.y - cursor.itemUseRadius / 2f)
            )
        {
            return Vector3Int.right;
        }
        else if (
            cursorPosition.x < playerPosition.x
            &&
            cursorPosition.y < (playerPosition.y + cursor.itemUseRadius / 2f)
            &&
            cursorPosition.y > (playerPosition.y - cursor.itemUseRadius / 2f)
            )
        {
            return Vector3Int.left;
        }
        else if (cursorPosition.y > playerPosition.y)
        {
            return Vector3Int.up;
        }
        else
        {
            return Vector3Int.down;
        }
    }

    private void ProcessPlayerClickInputSeed(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        if (itemDetails.canBeDropped && gridCursor.CursorPositionIsValid && gridPropertyDetails.daysSinceDug > -1 && gridPropertyDetails.seedItemCode == -1 && gridPropertyDetails.isPlantable)
        {
            PlantSeedAtCursor(gridPropertyDetails, itemDetails);
        }
        else if (itemDetails.canBeDropped && gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }

    private void ProcessPlayerClickInputCommodity(ItemDetails itemDetails)
    {
        if (itemDetails.canBeDropped && gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }

    private void ProcessPlayerClickInputTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        switch (itemDetails.itemType)
        {
            case ItemType.WateringTool:
                if (gridCursor.CursorPositionIsValid)
                {
                    WaterGroundAtCursor(gridPropertyDetails, playerDirection);
                }
                break;
            case ItemType.HoeingTool:
                if (gridCursor.CursorPositionIsValid)
                {
                    HoeGroundAtCursor(gridPropertyDetails, playerDirection);
                }
                break;
            case ItemType.ChoppingTool:
                if (gridCursor.CursorPositionIsValid)
                {
                    ChopInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
                }
                break;
            case ItemType.BreakingTool:
                if (gridCursor.CursorPositionIsValid)
                {
                    BreakInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
                }
                break;
            case ItemType.ReapingTool:
                if (cursor.isCursorPositionValid)
                {
                    playerDirection = GetPlayerDirection(cursor.GetWorldPositionForCursor(), GetPlayerCenterPosition());
                    ReapInPlayerDirectionAtCursor(itemDetails, playerDirection);
                }
                break;
            case ItemType.CollectingTool:
                if (gridCursor.CursorPositionIsValid)
                {
                    CollectInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
                }
                break;
            case ItemType.DiggingTool:
                if (gridCursor.CursorPositionIsValid)
                {
                    TreasureDigInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
                }
                break;
            default:
                break;
        }
    }



    private void WaterGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        // trigger watering animation
        StartCoroutine(WaterGroundAtCursorRoutine(playerDirection, gridPropertyDetails));
    }

    private IEnumerator WaterGroundAtCursorRoutine(Vector3Int playerDirection, GridPropertyDetails gridPropertyDetails)
    {
        _playerInputIsDisabled = true;
        playerToolUseDisabled = true;

        // Set tool animation to wateringCan in overrideAnimation
        toolCharacterAttribute.partVariantType = PartVariantType.wateringCan;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

        // TODO: only use if there is water in the watering can
        toolEffect = ToolEffect.watering;

        if (playerDirection == Vector3Int.right)
        {
            isLiftingToolRight = true;
        }
        else if (playerDirection == Vector3Int.left)
        {
            isLiftingToolLeft = true;
        }
        else if (playerDirection == Vector3Int.up)
        {
            isLiftingToolUp = true;
        }
        else if (playerDirection == Vector3Int.down)
        {
            isLiftingToolDown = true;
        }

        yield return liftToolAnimationPause;

        // Set gridPropertyDetails for dug ground
        if (gridPropertyDetails.daysSinceWatered == -1)
        {
            gridPropertyDetails.daysSinceWatered = 0;
        }

        // Set gridProperty to watered
        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        // Display watered grid tiles
        GridPropertiesManager.Instance.DisplayWateredGround(gridPropertyDetails);

        // After animation pause
        yield return afterLiftToolAnimationPause;

        _playerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }

    private void HoeGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        // Trigger hoeing animation
        StartCoroutine(HoeGroundAtCursorRoutine(playerDirection, gridPropertyDetails));
    }

    private IEnumerator HoeGroundAtCursorRoutine(Vector3Int playerDirection, GridPropertyDetails gridPropertyDetails)
    {
        _playerInputIsDisabled = true;
        playerToolUseDisabled = true;

        // Set tool animation to hoe in overrideAnimation
        toolCharacterAttribute.partVariantType = PartVariantType.hoe;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

        if (playerDirection == Vector3Int.right)
        {
            isUsingToolRight = true;
        }
        else if (playerDirection == Vector3Int.left)
        {
            isUsingToolLeft = true;
        }
        else if (playerDirection == Vector3Int.up)
        {
            isUsingToolUp = true;
        }
        else if (playerDirection == Vector3Int.down)
        {
            isUsingToolDown = true;
        }

        yield return useToolAnimationPause;

        // Set gridPropertyDetails for dug ground
        if (gridPropertyDetails.daysSinceDug == -1)
        {
            gridPropertyDetails.daysSinceDug = 0;
        }

        gridPropertyDetails.isPlantable = true;

        // Set gridProperty to dug
        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        // Display dug grid tiles
        GridPropertiesManager.Instance.DisplayDugGround(gridPropertyDetails);

        // After animation pause
        yield return afterUseToolAnimationPause;

        _playerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }

    private void ReapInPlayerDirectionAtCursor(ItemDetails itemDetails, Vector3Int playerDirection)
    {
        StartCoroutine(ReapInPlayerDirectionAtCursorRoutine(itemDetails, playerDirection));
    }

    private IEnumerator ReapInPlayerDirectionAtCursorRoutine(ItemDetails itemDetails, Vector3Int playerDirection)
    {
        _playerInputIsDisabled = true;
        playerToolUseDisabled = true;

        // Set tool animation to scythe in override animation
        toolCharacterAttribute.partVariantType = PartVariantType.scythe;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

        // Reap in player direction
        UseToolInPlayerDirection(itemDetails, playerDirection);
        yield return useToolAnimationPause;

        _playerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }

    private void ChopInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        // Trigger chop animation
        StartCoroutine(ChopInPlayerDirectionReoutine(gridPropertyDetails, equippedItemDetails, playerDirection));
    }

    private IEnumerator ChopInPlayerDirectionReoutine(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        _playerInputIsDisabled = true;
        playerToolUseDisabled = true;

        // Set tool animation to axe in override animation
        toolCharacterAttribute.partVariantType = PartVariantType.axe;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

        ProcessCropWithEquippedItemInPlayerDIrection(playerDirection, equippedItemDetails, gridPropertyDetails);

        yield return useToolAnimationPause;

        yield return afterUseToolAnimationPause;

        _playerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }

    private void BreakInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        StartCoroutine(BreakInPlayerDirectionRoutine(gridPropertyDetails, itemDetails, playerDirection));
    }

    private IEnumerator BreakInPlayerDirectionRoutine(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        _playerInputIsDisabled = true;
        playerToolUseDisabled = true;

        // Set tool animation to axe in override animation
        toolCharacterAttribute.partVariantType = PartVariantType.pickaxe;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

        ProcessCropWithEquippedItemInPlayerDIrection(playerDirection, equippedItemDetails, gridPropertyDetails);
        yield return useToolAnimationPause;

        yield return afterUseToolAnimationPause;

        _playerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }

    private void UseToolInPlayerDirection(ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        if (Input.GetMouseButton(0))
        {
            switch (equippedItemDetails.itemType)
            {
                case ItemType.ReapingTool:
                    if (playerDirection == Vector3Int.right)
                    {
                        isSwingingToolRight = true;
                    }
                    else if (playerDirection == Vector3Int.left)
                    {
                        isSwingingToolLeft = true;
                    }
                    else if (playerDirection == Vector3Int.up)
                    {
                        isSwingingToolUp = true;
                    }
                    else if (playerDirection == Vector3Int.down)
                    {
                        isSwingingToolDown = true;
                    }
                    break;
            }

            // Define center position of square which will be used for colision testing
            Vector2 point = new Vector2(GetPlayerCenterPosition().x + (playerDirection.x * (equippedItemDetails.itemUseRadius / 2f)),
                                        GetPlayerCenterPosition().y + (playerDirection.y * (equippedItemDetails.itemUseRadius / 2f)));

            // Define size of the square which will be used for collision testing
            Vector2 size = new Vector2(equippedItemDetails.itemUseRadius, equippedItemDetails.itemUseRadius);

            // Get Item components with 2D collider located in the square at the center point defined, limited by maxCollidersToTestReapSwing
            Item[] itemArray = HelperMethods.GetComponentsAtBoxLocationNonAlloc<Item>(Settings.maxCollidersToTestPerReapSwing, point, size);
            int reapableItemCount = 0;

            // Loop through all items retrieved
            for (int i = itemArray.Length - 1; i >= 0; i--)
            {
                if (itemArray[i] != null)
                {
                    // Destroy Item GameObject if reapable
                    if (InventoryManager.Instance.GetItemDetails(itemArray[i].ItemCode).itemType == ItemType.ReapableScenery)
                    {
                        // Efect Position
                        Vector3 effectPosition = new Vector3(itemArray[i].transform.position.x, itemArray[i].transform.position.y + Settings.gridCellSize / 2f,
                            itemArray[i].transform.position.z);
                        // Trigger reaping effect
                        EventHandler.CallHarvestActionEffectEvent(effectPosition, HarvestActionEffect.reaping);

                        Destroy(itemArray[i].gameObject);

                        reapableItemCount++;
                        if (reapableItemCount >= Settings.maxTargetComponentsToDestroyPerReapSwing)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }

    private void PlantSeedAtCursor(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        if (GridPropertiesManager.Instance.GetCropDetails(itemDetails.itemCode) != null)
        {
            // Update grid properties with seed details
            gridPropertyDetails.seedItemCode = itemDetails.itemCode;
            gridPropertyDetails.growthDays = 0;

            // Display planted crop at grid property details
            GridPropertiesManager.Instance.DisplayPlantedCrop(gridPropertyDetails);

            // Remove item from inventory
            EventHandler.CallRemoveSelectedItemFromInventoryEvent();
        }
    }

    private void CollectInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        StartCoroutine(CollectInPlayerDirectionRoutine(gridPropertyDetails, itemDetails, playerDirection));
    }

    private IEnumerator CollectInPlayerDirectionRoutine(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        _playerInputIsDisabled = true;
        playerToolUseDisabled = true;

        ProcessCropWithEquippedItemInPlayerDIrection(playerDirection, equippedItemDetails, gridPropertyDetails);

        yield return pickAnimationPause;

        //After animation pause
        yield return afterPickAnimationPause;

        _playerInputIsDisabled = false;
        playerToolUseDisabled = false;


    }

    private void TreasureDigInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        StartCoroutine(TreasureDigInPlayerDirectionRoutine(gridPropertyDetails, itemDetails, playerDirection));
    }

    private IEnumerator TreasureDigInPlayerDirectionRoutine(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        _playerInputIsDisabled = true;
        playerToolUseDisabled = true;

        // Set tool animation to hoe in overrideAnimation
        toolCharacterAttribute.partVariantType = PartVariantType.shovel;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

        if (playerDirection == Vector3Int.right)
        {
            isUsingToolRight = true;
        }
        else if (playerDirection == Vector3Int.left)
        {
            isUsingToolLeft = true;
        }
        else if (playerDirection == Vector3Int.up)
        {
            isUsingToolUp = true;
        }
        else if (playerDirection == Vector3Int.down)
        {
            isUsingToolDown = true;
        }

        yield return useToolAnimationPause;

        // Set gridPropertyDetails for dug ground
        if (gridPropertyDetails.daysSinceDug == -1)
        {
            gridPropertyDetails.daysSinceDug = 0;
        }

        // Set gridPropertyDetails for treasure dug ground
        gridPropertyDetails.isPlantable = false;

        // Set gridProperty to dug
        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        // Display dug grid tiles
        GridPropertiesManager.Instance.DisplayTreasureDugGround(gridPropertyDetails);

        SpawnDugItems(itemDetails);

        // After animation pause
        yield return afterUseToolAnimationPause;

        _playerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }

    private void SpawnDugItems(ItemDetails equippedItemDetails)
    {
        
        if (equippedItemDetails.lootItemCodeList.Count > 0)
        {
            if (equippedItemDetails.itemBonusPercentage > 0)
            {
                float chanceToHarvest = Random.Range(0f, 1f);
                
                if (chanceToHarvest <= equippedItemDetails.itemBonusPercentage)
                {
                    for (int i = 0; i < equippedItemDetails.lootItemCodeList.Count; i++)
                    {
                        Vector3 worldPosition = new Vector3((gameObject.transform.position.x + Random.Range(1f, 2f)), (gameObject.transform.position.y + Random.Range(1f, 2f)), -mainCamera.transform.position.z);
                        // Create item from prefab at mouse position
                        GameObject itemGameObject = Instantiate(itemPrefab, new Vector3(worldPosition.x, worldPosition.y - Settings.gridCellSize / 2, worldPosition.z), Quaternion.identity, parentItem);
                        Item item = itemGameObject.GetComponent<Item>();
                        item.ItemCode = equippedItemDetails.lootItemCodeList[i];
                    }
                    //TODO: Create a variable for the wellbeing change
                    EventHandler.CallChangeWellBeing(1f);
                }
                else
                {
                    //TODO: Create a variable for the wellbeing change
                    EventHandler.CallChangeWellBeing(-1f);
                }
            }
        }
    }

    /// <summary>
    /// Process crop with equipped item in player direction
    /// </summary>
    private void ProcessCropWithEquippedItemInPlayerDIrection(Vector3Int playerDirection, ItemDetails equippedItemDetails, GridPropertyDetails gridPropertyDetails)
    {
        switch (equippedItemDetails.itemType)
        {
            case ItemType.ChoppingTool:
            case ItemType.BreakingTool:
          
                if (playerDirection == Vector3Int.right)
                {
                    isUsingToolRight = true;
                }
                else if (playerDirection == Vector3Int.left)
                {
                    isUsingToolLeft = true;
                }
                else if (playerDirection == Vector3Int.up)
                {
                    isUsingToolUp = true;
                }
                else if (playerDirection == Vector3Int.down)
                {
                    isUsingToolDown = true;
                }
                break;
            case ItemType.CollectingTool:
                if (playerDirection == Vector3Int.right)
                {
                    isPickingRight = true;
                }
                else if (playerDirection == Vector3Int.left)
                {
                    isPickingLeft = true;
                }
                else if (playerDirection == Vector3Int.up)
                {
                    isPickingUp = true;
                }
                else if (playerDirection == Vector3Int.down)
                {
                    isPickingDown = true;
                }
                break;
            default:
                break;
        }

        // Get crop at cursor grid location
        Crop crop = GridPropertiesManager.Instance.GetCropObjectAtGridLocation(gridPropertyDetails);

        // Execute process tool action for crop
        if (crop != null)
        {
            switch (equippedItemDetails.itemType)
            {
                case ItemType.CollectingTool:
                    crop.ProcessToolAction(equippedItemDetails, isPickingRight, isPickingLeft, isPickingDown, isPickingUp);
                    break;
                case ItemType.ChoppingTool:
                case ItemType.BreakingTool:
               
                    crop.ProcessToolAction(equippedItemDetails, isUsingToolRight, isUsingToolLeft, isUsingToolDown, isUsingToolUp);
                    break;

            }
        }

       
    }

    public void EnablePlayerInput()
    {
        _playerInputIsDisabled = false;
    }

    public void DisablePlayerInput()
    {
        _playerInputIsDisabled = true;
    }

    private void ResetMovement()
    {
        // Reset movement
        xInput = 0f;
        yInput = 0f;
        isRunning = false;
        isWalking = false;
        isIdle = true;
    }

    public void DisablePlayerInputAndResetMovement()
    {
        DisablePlayerInput();
        ResetMovement();

        // Send event to any listeners for player input
        EventHandler.CallMovementEvent(xInput, yInput, isWalking, isRunning, isIdle, isCarrying, toolEffect,
            isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
            isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
            isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
            isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
            false, false, false, false);
    }

    public Vector3 GetPlayerViewportPosition()
    {
        return mainCamera.WorldToViewportPoint(transform.position);
    }

    public Vector3 GetPlayerCenterPosition()
    {
        return new Vector3(transform.position.x, transform.position.y + Settings.playerCenterYOffset, transform.position.z);
    }

    public void ShowCarriedItem(int itemCode)
    {
        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);
        if (itemDetails != null)
        {
            equippedItemSpriteRenderer.sprite = itemDetails.itemSprite;
            equippedItemSpriteRenderer.color = new Color(1f, 1f, 1f, 1f);

            // Apply "carry" character arms customisation
            armsCharacterAttribute.partVariantType = PartVariantType.carry;
            characterAttributeCustomisationList.Clear();
            characterAttributeCustomisationList.Add(armsCharacterAttribute);
            animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

            isCarrying = true;
        }
    }

    public void ClearCarriedItem()
    {
        equippedItemSpriteRenderer.sprite = null;
        equippedItemSpriteRenderer.color = new Color(0f, 0f, 0f, 0f);

        // Apply base character arms customization
        armsCharacterAttribute.partVariantType = PartVariantType.none;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(armsCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

        isCarrying = false;
    }

    //TODO: Remove
    /// <summary>
    /// Temp routine to rapdly advance time
    /// </summary>
    private void PlayerTestInput()
    {
        // advances a minute
        if (Input.GetKey(KeyCode.T))
        {
            TimeManager.Instance.TestAdvanceGameMinute();
        }
        // advance a day
        if (Input.GetKeyDown(KeyCode.Y))
        {
            TimeManager.Instance.TestAdvanceGameDay();
        }
        // advance a season
        if (Input.GetKeyDown(KeyCode.U))
        {
            TimeManager.Instance.TestAdvanceGameSeason();
        }
    }

    private void SceneLoaded()
    {
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }
}
