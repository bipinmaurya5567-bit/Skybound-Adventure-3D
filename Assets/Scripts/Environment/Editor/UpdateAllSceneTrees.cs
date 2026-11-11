using System.Collections.Generic;
using SkyboundAdventure;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class UpdateAllSceneTrees
{
    public static string Run()
    {
        // 1. Generate All Tree and Rock Prefabs with new geometry and materials
        string genLog = EnvironmentPrefabGenerator.GenerateAllAssets();

        // 2. Map existing tree names to target prefab names
        Dictionary<string, string> treePrefabMap = new Dictionary<string, string>
        {
            { "HavenTree_Lush", "Tree_LargeLush" },
            { "HavenTree_Palm", "Tree_TropicalPalm" },
            { "HavenTree_Small", "Tree_DecorativeSmall" },
            { "OutlookTree", "Tree_TallForest" },
            { "BridgeTree", "Tree_TropicalPalm" },
            { "MidTree", "Tree_AutumnOak" },
            { "LandingPalm", "Tree_TropicalPalm" },
            { "RidgeTree", "Tree_LargeLush" },
            { "CrystalTree_Fantasy", "Tree_FantasySky" },
            { "LedgeTree_Fantasy", "Tree_FantasySky" },
            { "TempleTree_L", "Tree_FantasySky" },
            { "TempleTree_R", "Tree_FantasySky" }
        };

        int replacedCount = 0;

        // Collect trees to replace
        var allGos = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        List<GameObject> treesToReplace = new List<GameObject>();

        foreach (var go in allGos)
        {
            if (treePrefabMap.ContainsKey(go.name) || (go.name == "Tree" && go.transform.parent != null && go.transform.parent.name.StartsWith("Distant")))
            {
                treesToReplace.Add(go);
            }
        }

        foreach (var oldTree in treesToReplace)
        {
            string prefabName = "Tree_LargeLush";
            if (treePrefabMap.TryGetValue(oldTree.name, out string mapped))
            {
                prefabName = mapped;
            }
            else if (oldTree.name == "Tree")
            {
                // Alternate distant island trees across all 5 styles
                int hash = Mathf.Abs(oldTree.transform.position.GetHashCode());
                string[] styles = { "Tree_LargeLush", "Tree_TallForest", "Tree_TropicalPalm", "Tree_AutumnOak", "Tree_FantasySky" };
                prefabName = styles[hash % styles.Length];
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Prefabs/Environment/{prefabName}.prefab");
            if (prefab == null) continue;

            string originalName = oldTree.name;
            Transform parent = oldTree.transform.parent;
            Vector3 pos = oldTree.transform.position;
            Quaternion rot = oldTree.transform.rotation;
            Vector3 scale = oldTree.transform.localScale;

            Object.DestroyImmediate(oldTree);

            GameObject newTree = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            newTree.name = originalName;
            if (parent != null) newTree.transform.SetParent(parent, true);
            newTree.transform.position = pos;
            newTree.transform.rotation = rot;
            newTree.transform.localScale = scale;

            replacedCount++;
        }

        EditorSceneManager.SaveOpenScenes();

        return $"Successfully generated all assets and replaced {replacedCount} trees across the world!\nLog: {genLog}";
    }
}
