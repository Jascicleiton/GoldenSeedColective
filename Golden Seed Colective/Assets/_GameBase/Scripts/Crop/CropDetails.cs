using UnityEngine;

[System.Serializable]
public class CropDetails
{
    [ItemCodeDescription]
    public int seedItemCode; // this is the item code for the corresponding seed
    public int[] growthDays; // days of growth for each stage
    public GameObject[] growthPrefabs; // prefab to use when instantiating growth stages
    public GameObject whiteredPrefab; // prefab to use when instantiating whitered crop
    public Sprite[] growthSprites; // growth sprite for each stages
    public Sprite whiteredSprite;
    public Season[] seasons; // each season that the crop grows - TODO: Implement on the game! or not!
    public Sprite harvestedSprite; // sprite used once harvested

    [ItemCodeDescription]
    public int harvestedTransformItemCode; // if the item transforms into another item when harvested this item code will be populated
    public bool hideCropBeforeHarvestedAnimation; // if the crop shoul be disabled before the harvested animation
    public bool disableCropCollidersBeforeHarvestedAnimation; // if colliders on crop shoul be disabled to avoid the harvested animation affecting any other game objects
    public bool isHarvestedAnimation; // true if harvested animation is to be played on final growth stage prefab
    public bool isHaverstActionEffect = false; // flag to determine whether there is a harvest action effect or not
    public bool spawnCropProduceAtPlayerPosition; 
    public HarvestActionEffect harvestActionEffect; // the harvest action effect for the crop

    [ItemCodeDescription]
    public int[] harvestToolItemCodes; // array of item codes for the tools that can harvest or 0 array elements if no tool required
    public int[] requiredHarvestActions; // number of harvest actions required for corresponding tool in harvestToolItemCode array

    [ItemCodeDescription]
    public int[] cropProducedItemCode; // array of itemCodes produced for the harvestedcrop
    public int[] cropProducedMinQuantity; // array of minimu quantities produced for the harvested crop
    public int[] cropProducedMaxQuantitty; // if max quantity is > minQuantity, then a random number between them is produced
    public int daysToRegrow; // days to regrow next crop or -1 if it is a single crop TODO: need to implement this
   public int daysWithoutBeingTended = 3;
   // public int daysSinceTended; // days since last tended the crop. If not tended for daysWithoutBeingTended it whiters.

    /// <summary>
    /// Returns true if the toolItemCode can be used to harvest this crop, else returns false
    /// </summary>
    public bool CanUseToolToHarvestCrop (int toolItemCode)
    {
        if (RequiredHarvestActionsForTool(toolItemCode) == -1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Returns -1 if the tool can't be used to harvest this crop, else returns the numbers of actions required by this tool to harvest this crop
    /// </summary>
    public int RequiredHarvestActionsForTool(int toolItemCode)
    {
        for (int i = 0; i < harvestToolItemCodes.Length; i++)
        {
            if(harvestToolItemCodes[i] == toolItemCode)
            {
                return requiredHarvestActions[i];
            }
        }
        return -1;
    }
}
