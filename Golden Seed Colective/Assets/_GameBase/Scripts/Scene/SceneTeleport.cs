using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SceneTeleport : MonoBehaviour
{
    [SerializeField] private SceneName sceneNameToGo = SceneName.Scene1_Farm;
    [SerializeField] private Vector3 scenePositionToGO = new Vector3();

    private void OnTriggerStay2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if(player != null)
        {
            // Calculate player's new position - condition check (before ?), if true its first statement, if false, second
            float xPosition = Mathf.Approximately(scenePositionToGO.x, 0f) ? player.transform.position.x : scenePositionToGO.x;
            float yPosition = Mathf.Approximately(scenePositionToGO.y, 0f) ? player.transform.position.y : scenePositionToGO.y;
            float zPosition = 0f;

            // Teleport to new scene
            SceneControllerManager.Instance.FadeAndLoadScene(sceneNameToGo.ToString(), new Vector3(xPosition, yPosition, zPosition));
        }
    }
}
