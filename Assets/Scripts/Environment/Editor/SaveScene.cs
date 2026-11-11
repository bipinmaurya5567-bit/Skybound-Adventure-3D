using UnityEditor;
using UnityEditor.SceneManagement;

public class SaveScene
{
    public static string Run()
    {
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
        return "Saved successfully";
    }
}
