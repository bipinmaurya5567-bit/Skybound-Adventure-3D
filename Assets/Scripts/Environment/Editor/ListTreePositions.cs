using UnityEngine;

public class ListTreePositions
{
    public static string Run()
    {
        string s = "Trees in scene:";
        foreach (var go in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            if (go.name.Contains("Tree") && go.transform.parent == null || (go.name.StartsWith("Haven") || go.name.StartsWith("Tree")))
            {
                s += $"\n  {go.name}: pos={go.transform.position}";
            }
        }
        return s;
    }
}
