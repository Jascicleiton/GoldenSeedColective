using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    [Tooltip("This should be populated with the child GameObject")]
    [SerializeField] private SpriteRenderer cropHarvestedSpriteRenderer = null;
    [Tooltip("This should be populated child transform gameObject showing harvest effect spawn point")]
    [SerializeField] private Transform harvestActionEffectTansform = null;

    private int harvestActionCount = 0;
    [HideInInspector] public Vector2Int cropGridPosition;

    public void ProcessToolAction(ItemDetails equippedItemDetails, bool isToolRight, bool isToolLeft, bool isToolDown, bool isToolUp)
    {
        // Get grid property details
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y);
                if(gridPropertyDetails == null)
        {
            return;
        }

        // Get seed item details
        ItemDetails seedItemDetails = InventoryManager.Instance.GetItemDetails(gridPropertyDetails.seedItemCode);
        if(seedItemDetails == null)
        {
            return;
        }

        // Get crop details
        CropDetails cropDetails = GridPropertiesManager.Instance.GetCropDetails(seedItemDetails.itemCode);
        if(cropDetails == null)
        {
            return;
        }

        // Try to get animator for crop
        Animator animator = GetComponentInChildren<Animator>();
        // Trigger tool animation
        if(animator != null)
        {
            if(isToolRight || isToolUp)
            {
                animator.SetTrigger("usetoolright");
            }
            else if(isToolLeft || isToolDown)
            {
                animator.SetTrigger("usetoolleft");
            }
        }

        if(cropDetails.isHaverstActionEffect)
        {
            EventHandler.CallHarvestActionEffectEvent(harvestActionEffectTansform.position, cropDetails.harvestActionEffect);
        }

        // Get required harvest action for tool
        int requiredHarvestActions = cropDetails.RequiredHarvestActionsForTool(equippedItemDetails.itemCode);
        if(requiredHarvestActions == -1)
        {
            return; // This tool can't be used to harvest this crop
        }

        harvestActionCount++;

        // Check if required harvest actions made
        if (harvestActionCount >= requiredHarvestActions)
        {
            HarvestCrop(isToolRight, isToolUp, cropDetails, gridPropertyDetails, animator);
        }
    }

    private void HarvestCrop(bool isUsingToolRight, bool isUsingToolUp, CropDetails cropDetails, GridPropertyDetails gridPropertyDetails, Animator animator)
    {
        // Is there a harvested animation
        if(cropDetails.isHarvestedAnimation && animator != null)
        {
            if(cropDetails.harvestedSprite != null)
            {
                cropHarvestedSpriteRenderer.sprite = cropDetails.harvestedSprite;
            }
        }

        if (isUsingToolRight || isUsingToolUp)
        {
            animator.SetTrigger("harvestright");
        }
        else
        {
            animator.SetTrigger("harvestleft");
        }

        // Delete crop from grid properties
        gridPropertyDetails.seedItemCode = -1;
        gridPropertyDetails.growthDays = -1;
        gridPropertyDetails.daysSinceLastharvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;

        // Should the crop be hidden before the harvested animation
        if (cropDetails.hideCropBeforeHarvestedAnimation)
        {
            GetComponentInChildren<SpriteRenderer>().enabled = false;
        }

        // Should box colliders be disabled before harvest
        if(cropDetails.disableCropCollidersBeforeHarvestedAnimation)
        {
            Collider2D[] colliders2D = GetComponentsInChildren<Collider2D>();
            foreach (Collider2D collider2D in colliders2D)
            {
                collider2D.enabled = false;
            }
        }

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        //Is there a harvested animation - Destroy this crop gameObject after animation is completed
        if(cropDetails.isHarvestedAnimation && animator != null)
        {
            StartCoroutine(ProcessHarvestActionsAfterAnimation(cropDetails, gridPropertyDetails, animator));
        }
        else
        {
             HarvestActions(cropDetails, gridPropertyDetails);
        }
    }

    private void HarvestActions(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        SpawnHarvestedItems(cropDetails);
        // Does this crop transforms into another crop
        if(cropDetails.harvestedTansformItemCode > 0)
        {
            CreateHarvestedTransformCrop(cropDetails, gridPropertyDetails);
        }

        Destroy(gameObject);
    }

    private void CreateHarvestedTransformCrop(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        // Update crop in grid properties
        gridPropertyDetails.seedItemCode = cropDetails.harvestedTansformItemCode;
        gridPropertyDetails.growthDays = 0;
        gridPropertyDetails.daysSinceLastharvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        GridPropertiesManager.Instance.DisplayPlantedCrop(gridPropertyDetails);
    }

    private IEnumerator ProcessHarvestActionsAfterAnimation(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails, Animator animator)
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Harvested"))
        {
            yield return null;
        }

        HarvestActions(cropDetails, gridPropertyDetails);
    }

    private void SpawnHarvestedItems(CropDetails cropDetails)
    {
        // Spawn the item(s) to be produced
        for (int i = 0; i < cropDetails.cropProducedItemCode.Length; i++)
        {
            int cropsToProduce;

            // Calculate how many crops to produce
            if(cropDetails.cropProducedMaxQuantitty[i] <= cropDetails.cropProducedMinQuantity[i])
            {
                cropsToProduce = cropDetails.cropProducedMinQuantity[i];
            }
            else
            {
                cropsToProduce = Random.Range(cropDetails.cropProducedMinQuantity[i], cropDetails.cropProducedMaxQuantitty[i] + 1);
            }
            for (int j = 0; j < cropsToProduce; j++)
            {
                Vector3 spawnPosition;
                if(cropDetails.spawnCropProduceAtPlayerPosition)
                {
                    InventoryManager.Instance.AddItem(InventoryLocation.player, cropDetails.cropProducedItemCode[i]);
                }
                else
                {
                    // Random Position
                    spawnPosition = new Vector3(transform.position.x + Random.Range(-1f, 1f), transform.position.y + Random.Range(-1f, 1f), 0f);
                    SceneItemsManager.Instance.InstantiateSceneItem(cropDetails.cropProducedItemCode[i], spawnPosition);
                }
            }
        }
    }
}
