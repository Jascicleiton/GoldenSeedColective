using System.Collections.Generic;

[System.Serializable]
public class SceneSave
{
    // string key is an identifier name we choose for this list
    public List<SceneItem> listSceneItem;
    public Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary;
    // Used to check if the scene is being loaded for the first time
    public Dictionary<string, bool> boolDictionary;
}