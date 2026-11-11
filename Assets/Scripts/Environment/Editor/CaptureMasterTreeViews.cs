using System.IO;
using UnityEditor;
using UnityEngine;

public class CaptureMasterTreeViews
{
    private static readonly string ArtifactDir = @"C:\Users\Bipin Maurya\.gemini\antigravity-cli\brain\3805c2c9-ce2d-49a8-ba50-e2a58bbd340f";

    public static string CaptureAllViews()
    {
        var tree = GameObject.Find("HavenTree_Lush");
        if (tree == null) return "Error: HavenTree_Lush not found in scene!";

        Vector3 treeBase = tree.transform.position;
        Vector3 treeCenter = treeBase + new Vector3(0f, 3.8f, 0f);

        // 1. Front View (From South-East looking North-West, clear line of sight)
        CaptureView("master_tree_front.png", new Vector3(-1.5f, 3.0f, 0.2f), treeCenter);

        // 2. Side View (East side looking West)
        CaptureView("master_tree_side.png", new Vector3(0.5f, 3.2f, 2.5f), treeCenter);

        // 3. Behind View (North side looking South)
        CaptureView("master_tree_behind.png", new Vector3(-5.5f, 3.2f, 8.5f), treeCenter);

        // 4. Above View (Looking down at 45 degrees from high vantage)
        CaptureView("master_tree_above.png", new Vector3(-1.5f, 9.5f, -1.0f), treeCenter);

        // 5. Gameplay Camera View (Player perspective approaching from island start)
        CaptureView("master_tree_gameplay.png", new Vector3(0.0f, 2.2f, -1.5f), treeCenter);

        return "Successfully captured all 5 Master Tree views!";
    }

    private static void CaptureView(string filename, Vector3 camPos, Vector3 lookTarget)
    {
        GameObject camGo = new GameObject("TempRenderCam");
        Camera cam = camGo.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.Skybox;
        cam.fieldOfView = 55f;
        cam.nearClipPlane = 0.1f;
        cam.farClipPlane = 1000f;

        camGo.transform.position = camPos;
        camGo.transform.LookAt(lookTarget);

        int width = 1280;
        int height = 720;
        RenderTexture rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        rt.antiAliasing = 4;
        cam.targetTexture = rt;

        RenderTexture.active = rt;
        cam.Render();

        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        RenderTexture.active = null;
        cam.targetTexture = null;
        Object.DestroyImmediate(rt);
        Object.DestroyImmediate(camGo);

        byte[] pngData = tex.EncodeToPNG();
        Object.DestroyImmediate(tex);

        string savePath = Path.Combine(ArtifactDir, filename);
        File.WriteAllBytes(savePath, pngData);
    }
}
