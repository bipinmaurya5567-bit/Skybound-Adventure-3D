using UnityEditor;
using UnityEngine;

public class InstantiateMasterTreeHelper
{
    public static string Run()
    {
        var existing = GameObject.Find("HavenTree_Lush");
        Vector3 pos = new Vector3(-5.5f, 0.70f, 2.5f);
        Transform parent = null;
        if (existing != null)
        {
            pos = existing.transform.position;
            parent = existing.transform.parent;
            Object.DestroyImmediate(existing);
        }

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Environment/Tree_LargeLush.prefab");
        if (prefab == null) return "Error: Tree_LargeLush.prefab not found!";

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.name = "HavenTree_Lush";
        if (parent != null) instance.transform.SetParent(parent, true);
        instance.transform.position = pos;
        instance.transform.rotation = Quaternion.Euler(0f, 25f, 0f);

        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();

        return $"Successfully instantiated Master Broadleaf Tree at {instance.transform.position}";
    }
}
