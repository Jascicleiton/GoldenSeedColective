using System;
using System.Collections.Generic;
using UnityEngine;

#region Delegates for non Action events
// Movement is not an Action, because it has more than 16 parameters
public delegate void MovementDelegate(float inputX, float inputY, bool isWalking, bool isRunning, bool isIdle, bool isCarrying, 
    ToolEffect tooleffect,
    bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown, 
    bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown,
    bool isPickingRight, bool isPickingLeft, bool isPickingUp, bool isPickingDown,
    bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown,
    bool idleRight, bool idleLeft, bool idleUp, bool idleDown );
#endregion

public static class EventHandler
{
    #region Movement
    //Movement Event
    public static event MovementDelegate MovementEvent;

    // Movement Event call for Publishers
    public static void CallMovementEvent(float inputX, float inputY, bool isWalking, bool isRunning, bool isIdle, bool isCarrying,
    ToolEffect tooleffect,
    bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown,
    bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown,
    bool isPickingRight, bool isPickingLeft, bool isPickingUp, bool isPickingDown,
    bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown,
    bool idleRight, bool idleLeft, bool idleUp, bool idleDown)
    {
        if(MovementEvent != null)
        {
            MovementEvent(inputX, inputY, isWalking, isRunning, isIdle, isCarrying,
     tooleffect,
     isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
     isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
     isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
     isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
     idleRight, idleLeft, idleUp, idleDown);
        }
    }
    #endregion

    #region Inventory
    // Inventory updated event
    public static event Action<InventoryLocation, List<InventoryItem>> InventoryUpdatedEvent;

    public static void CallInventoryUpdatedEvent(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        if(InventoryUpdatedEvent != null)
        {
            InventoryUpdatedEvent(inventoryLocation, inventoryList);
        }
    }

    // Drop selected item event
    public static event Action DropSelectedItemEvent;

    public static void CallDropSelectedItemEvent()
    {
        if(DropSelectedItemEvent != null)
        {
            DropSelectedItemEvent();
        }
    }

    // Remove selected item from inventory
    public static event Action RemoveSelectedItemFromInventoryEvent;

    public static void CallRemoveSelectedItemFromInventoryEvent()
    {
        if (RemoveSelectedItemFromInventoryEvent != null)
        {
            RemoveSelectedItemFromInventoryEvent();
        }
    }
    #endregion

    #region Time

    /// <summary>
    /// Advance game minute - gameYear, gameSeason, gameDay, gameDayOfWeek, gamehour, gameMinute, gameSecond
    /// </summary>
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameMinuteEvent;

    public static void CallAdvanceGameMinuteEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gamehour, int gameMinute, int gameSecond)
    {
        if(AdvanceGameMinuteEvent != null)
        {
            AdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gamehour, gameMinute, gameSecond);
        }
    }

    /// <summary>
    /// Advance game hour - gameYear, gameSeason, gameDay, gameDayOfWeek, gamehour, gameMinute, gameSecond
    /// </summary>
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameHourEvent;

    public static void CallAdvanceGameHourEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gamehour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameHourEvent != null)
        {
            AdvanceGameHourEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gamehour, gameMinute, gameSecond);
        }
    }

    /// <summary>
    /// Advance game day - gameYear, gameSeason, gameDay, gameDayOfWeek, gamehour, gameMinute, gameSecond
    /// </summary>
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameDayEvent;

    public static void CallAdvanceGameDayEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gamehour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameDayEvent != null)
        {
            AdvanceGameDayEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gamehour, gameMinute, gameSecond);
        }
    }

    /// <summary>
    /// Advance game season - gameYear, gameSeason, gameDay, gameDayOfWeek, gamehour, gameMinute, gameSecond
    /// </summary>
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameSeasonEvent;

    public static void CallAdvanceGameSeasonEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gamehour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameSeasonEvent != null)
        {
            AdvanceGameSeasonEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gamehour, gameMinute, gameSecond);
        
        
        }
    }

    /// <summary>
    /// Advance game year - gameYear, gameSeason, gameDay, gameDayOfWeek, gamehour, gameMinute, gameSecond
    /// </summary>
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameYearEvent;

    public static void CallAdvanceGameYearEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gamehour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameYearEvent != null)
        {
            AdvanceGameYearEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gamehour, gameMinute, gameSecond);
        }
    }
    #endregion

    #region SceneLoadEvents
    // Scene load events in the order that they happen

    //Before scene unload fade out event
    public static event Action BeforeSceneUnloadFadeOutEvent;

    public static void CallBeforeSceneUnloadFadeOutEvent()
    {
        if(BeforeSceneUnloadFadeOutEvent != null)
        {
            BeforeSceneUnloadFadeOutEvent();
        }
    }

    // Before scene unload event
    public static event Action BeforeSceneUnloadEvent;

    public static void CallBeforeSceneUnloadEvent()
    {
        if(BeforeSceneUnloadEvent != null)
        {
            BeforeSceneUnloadEvent();
        }
    }

    // After scene loaded event
    public static event Action AfterSceneLoadEvent;

    public static void CallAfterSceneLoadEvent()
    {
        if(AfterSceneLoadEvent != null)
        {
            AfterSceneLoadEvent();
        }
    }

    // After scene load fade in event
    public static event Action AfterSceneLoadFadeInEvent;
    public static void CallAfterSceneLoadFadeInEvent()
    {
        if(AfterSceneLoadFadeInEvent != null)
        {
            AfterSceneLoadFadeInEvent();
        }
    }
    #endregion SceneLoadEvents

    #region Harvest
    // Harvest action effect Event
    public static event Action<Vector3, HarvestActionEffect> HarvestActionEffectEvent;

    public static void CallHarvestActionEffectEvent(Vector3 effectPosition, HarvestActionEffect harvestActionEffect)
    {
        if(HarvestActionEffectEvent != null)
        {
            HarvestActionEffectEvent(effectPosition, harvestActionEffect);
        }
    }

    #endregion Harvest

    #region Crops
    // Instantiate crop prefabs
    public static event Action InstantiateCropPrefabsEvent;

    public static void CallInstantiateCropPrefabsEvent()
    {
        if(InstantiateCropPrefabsEvent != null)
        {
            InstantiateCropPrefabsEvent();
        }
    }
    #endregion Crops

    #region Player
    // Change Wellbeing
    public static event Action <float> ChangeWellBeing;
    
    public static void CallChangeWellBeing(float wellbeingValueToChange)
    {
        if(ChangeWellBeing != null)
        {
            ChangeWellBeing(wellbeingValueToChange);
        }
    }
    #endregion
}
