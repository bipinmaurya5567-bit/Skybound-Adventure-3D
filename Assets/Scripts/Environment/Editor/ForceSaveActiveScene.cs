using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class ForceSaveActiveScene
{
    public static string Run()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        bool saved = EditorSceneManager.SaveScene(activeScene);
        return $"Active Scene: '{activeScene.name}' ({activeScene.path}), Saved: {saved}";
    }
}
