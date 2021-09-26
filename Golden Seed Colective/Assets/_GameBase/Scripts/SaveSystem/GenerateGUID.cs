using UnityEngine;

[ExecuteAlways]
public class GenerateGUID : MonoBehaviour
{
    [SerializeField] private string gUID = "";
    public string GUID { get => gUID; set => gUID = value; }

    private void Awake()
    {
        // Only populate in the editor
        if(!Application.IsPlaying(gameObject))
        {
            // Ensure the object has a garanteed unique id
            if(gUID == "")
            {
                // Assign GUID
                gUID = System.Guid.NewGuid().ToString();
            }
        }
    }
}
