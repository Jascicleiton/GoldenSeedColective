using System.Collections.Generic;

[System.Serializable]
public class SceneSave
{
    public Dictionary<string, int> intDictionary;
    public Dictionary<string, bool> boolDictionary; // Used to check if the scene is being loaded for the first time
    public Dictionary<string, string> stringDictionary; // used to save scene and player's direction
    public Dictionary<string, Vector3Serializable> vector3Dictionary; // used to save player's position
    public Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary;
    public Dictionary<string, int[]> intArrayDictionary;
    public List<SceneItem> listSceneItem;
    public List<InventoryItem>[] listInvItemArray;
    public List<string> completedQuests;
    public List<string> dialogueOpationsSelected;
    public int day;

}