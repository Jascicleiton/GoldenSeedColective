using System.Collections;
using UnityEngine;

public class VFXManager : Singleton<VFXManager>
{
    private WaitForSeconds twoSeconds;
    [SerializeField] private GameObject reapingPrefab = null;
    [SerializeField] private GameObject deciduousLeavesFallingPrefab = null;
    [SerializeField] private GameObject choppingTreeTrunkPrefab = null;
    [SerializeField] private GameObject pineConesFallingPrefab = null;
    [SerializeField] private GameObject breakingStonesPrefab = null;

    protected override void Awake()
    {
        base.Awake();
        twoSeconds = new WaitForSeconds(2f);
    }

    private void OnEnable()
    {
        EventHandler.HarvestActionEffectEvent += DisplayHarvestActionEffect;
    }

    private void OnDisable()
    {
        EventHandler.HarvestActionEffectEvent -= DisplayHarvestActionEffect;
    }

    private void DisplayHarvestActionEffect(Vector3 effectPosition, HarvestActionEffect harvestActionEffect)
    {
        switch (harvestActionEffect)
        {
            case HarvestActionEffect.deciduousLeavesFalling:
                GameObject deciduousLeavesFalling = PoolManager.Instance.ReuseObject(deciduousLeavesFallingPrefab, effectPosition, Quaternion.identity);
                deciduousLeavesFalling.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(deciduousLeavesFalling, twoSeconds));
                break;
            case HarvestActionEffect.pineConesFalling:
                GameObject pineConesFalling = PoolManager.Instance.ReuseObject(pineConesFallingPrefab, effectPosition, Quaternion.identity);
                pineConesFalling.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(pineConesFalling, twoSeconds));
                break;
            case HarvestActionEffect.choppingTreeTrunk:
                GameObject choppingTreeTrunk = PoolManager.Instance.ReuseObject(choppingTreeTrunkPrefab, effectPosition, Quaternion.identity);
                choppingTreeTrunk.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(choppingTreeTrunk, twoSeconds));
                break;
            case HarvestActionEffect.breakingStone:
                GameObject breakingStone = PoolManager.Instance.ReuseObject(breakingStonesPrefab, effectPosition, Quaternion.identity);
                breakingStone.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(breakingStone, twoSeconds));
                break;
            case HarvestActionEffect.reaping:
                GameObject reaping = PoolManager.Instance.ReuseObject(reapingPrefab, effectPosition, Quaternion.identity);
                reaping.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(reaping, twoSeconds));
                break;
            case HarvestActionEffect.none:
                break;
            default:
                break;
        }
    }

    private IEnumerator DisableHarvestActionEffect(GameObject effectGameObject, WaitForSeconds secondsToWait)
    {
        yield return secondsToWait;
        effectGameObject.SetActive(false);
    }
}
