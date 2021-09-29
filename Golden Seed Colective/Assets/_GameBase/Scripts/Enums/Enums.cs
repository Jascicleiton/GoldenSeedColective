public enum AnimationName
{
    idleDown,
    idleUp,
    idleRight,
    idleLeft,
    walkUp,
    walkDown,
    walkRight,
    walkLeft,
    runUp,
    runDown,
    runRight,
    runLeft,
    useToolUp,
    useToolDown,
    useToolRight,
    useToolLeft,
    swingToolUp,
    swingToolDown,
    swingToolRight,
    swingToolLeft,
    liftToolUp,
    liftToolDown,
    liftToolRight,
    liftToolLeft,
    holdToolUp,
    holdToolDown,
    holdToolRight,
    holdToolLeft,
    pickDown,
    pickUp,
    pickRight,
    pickLeft,
    count
}

public enum CharacterPartAnimator
{
    body,
    arms,
    hair,
    tool,
    hat,
    count
}

public enum PartVariantColor
{
    none,
    count
}

public enum PartVariantType
{
    none,
    carry,
    hoe,
    pickaxe,
    axe,
    scythe,
    wateringCan,
    shovel,
    count
}

public enum GridBoolProperty
{
    diggable,
    canDropItem,
    canPlaceFurniture,
    isPath,
    isNPCObstacle
}

public enum InventoryLocation
{
    player,
    chest,
    count
}

public enum SceneName
{
    Scene1_Farm,
    Scene2_Field,
    Scene3_Cabin
}

public enum Season
{
    Spring,
    Summer,
    Autumn,
    Winter,
    none,
    count
}

public enum DayOfWeek
{
    Sun,
    Mon,
    Tue,
    Wed,
    Thu,
    Fri,
    Sat,
    count
}

public enum ToolEffect 
{
   none,
   watering
}

public enum HarvestActionEffect
{
    deciduousLeavesFalling,
    pineConesFalling,
    choppingTreeTrunk,
    breakingStone,
    reaping,
    none
}

public enum Direction
{
    right,
    left,
    up,
    down,
    none
}

public enum ItemType
{
    Seed,
    Commodity,
    WateringTool,
    HoeingTool,
    ChoppingTool,
    BreakingTool,
    ReapingTool,
    CollectingTool,
    ReapableScenery,
    Furniture,
    DiggingTool,
    none,
    count
}


