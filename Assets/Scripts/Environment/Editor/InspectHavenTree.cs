using UnityEngine;

public class InspectHavenTree
{
    public static string Run()
    {
        var t = GameObject.Find("HavenTree_Lush");
        if (t == null) return "HavenTree_Lush is null!";
        var mfs = t.GetComponentsInChildren<MeshFilter>();
        string s = $"Tree found: {t.name}, children MFs: {mfs.Length}";
        foreach (var mf in mfs)
        {
            s += $"\n  {mf.gameObject.name}: mesh={(mf.sharedMesh != null ? mf.sharedMesh.name : "null")}, verts={(mf.sharedMesh != null ? mf.sharedMesh.vertexCount : 0)}";
        }
        return s;
    }
}
