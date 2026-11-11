using System.IO;
using UnityEngine;

public class CaptureAllTreeStyles
{
    private static readonly string ArtifactDir = @"C:\Users\Bipin Maurya\.gemini\antigravity-cli\brain\3805c2c9-ce2d-49a8-ba50-e2a58bbd340f";

    public static string Run()
    {
        // 1. Tropical Palm Tree (HavenTree_Palm at (5.5, 0.7, 1.5))
        var palm = GameObject.Find("HavenTree_Palm");
        if (palm != null)
        {
            Vector3 center = palm.transform.position + new Vector3(0f, 3.2f, 0f);
            Capture("tree_palm_redesigned.png", palm.transform.position + new Vector3(-4.5f, 2.8f, -4.5f), center);
            Capture("tree_palm_closeup_redesigned.png", palm.transform.position + new Vector3(-2.2f, 4.2f, -2.2f), palm.transform.position + new Vector3(0.8f, 4.8f, 0f));
        }

        // 2. Tall Forest Tree (OutlookTree)
        var tall = GameObject.Find("OutlookTree");
        if (tall != null)
        {
            Vector3 center = tall.transform.position + new Vector3(0f, 3.8f, 0f);
            Capture("tree_tallforest_redesigned.png", tall.transform.position + new Vector3(5.0f, 3.2f, -5.0f), center);
        }

        // 3. Autumn Oak Tree (MidTree)
        var oak = GameObject.Find("MidTree");
        if (oak != null)
        {
            Vector3 center = oak.transform.position + new Vector3(0f, 3.5f, 0f);
            Capture("tree_autumnoak_redesigned.png", oak.transform.position + new Vector3(5.5f, 3.0f, -5.0f), center);
        }

        // 4. Fantasy Celestial Tree (CrystalTree_Fantasy)
        var fantasy = GameObject.Find("CrystalTree_Fantasy");
        if (fantasy != null)
        {
            Vector3 center = fantasy.transform.position + new Vector3(0f, 3.6f, 0f);
            Capture("tree_fantasy_redesigned.png", fantasy.transform.position + new Vector3(5.2f, 3.0f, -4.5f), center);
        }

        // 5. Small Decorative Tree (HavenTree_Small)
        var small = GameObject.Find("HavenTree_Small");
        if (small != null)
        {
            Vector3 center = small.transform.position + new Vector3(0f, 2.0f, 0f);
            Capture("tree_deco_redesigned.png", small.transform.position + new Vector3(3.8f, 2.0f, -3.5f), center);
        }

        return "All tree variation screenshots captured successfully!";
    }

    private static void Capture(string filename, Vector3 camPos, Vector3 target)
    {
        GameObject camGo = new GameObject("TempCam");
        Camera cam = camGo.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.Skybox;
        cam.fieldOfView = 55f;
        cam.nearClipPlane = 0.1f;
        cam.farClipPlane = 1000f;

        camGo.transform.position = camPos;
        camGo.transform.LookAt(target);

        int w = 1280;
        int h = 720;
        RenderTexture rt = new RenderTexture(w, h, 24, RenderTextureFormat.ARGB32);
        rt.antiAliasing = 4;
        cam.targetTexture = rt;

        RenderTexture.active = rt;
        cam.Render();

        Texture2D tex = new Texture2D(w, h, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        tex.Apply();

        RenderTexture.active = null;
        cam.targetTexture = null;
        Object.DestroyImmediate(rt);
        Object.DestroyImmediate(camGo);

        byte[] png = tex.EncodeToPNG();
        Object.DestroyImmediate(tex);

        File.WriteAllBytes(Path.Combine(ArtifactDir, filename), png);
    }
}
