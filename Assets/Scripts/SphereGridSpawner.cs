using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class SphereGridSpawner : MonoBehaviour
{
    [Header("Grid Layout Settings")]
    [SerializeField] private int rows = 2;
    [SerializeField] private int columns = 5;
    [SerializeField] private float spacingX = 2.0f;
    [SerializeField] private float spacingY = 2.0f;

    [System.Serializable]
    public struct SphereColorDefinition
    {
        public string name;
        public Color color;

        public SphereColorDefinition(string name, Color color)
        {
            this.name = name;
            this.color = color;
        }
    }

    public static readonly SphereColorDefinition[] DefaultColors = new SphereColorDefinition[10]
    {
        new SphereColorDefinition("Red", new Color(0.92f, 0.18f, 0.18f)),
        new SphereColorDefinition("Orange", new Color(1.00f, 0.52f, 0.05f)),
        new SphereColorDefinition("Yellow", new Color(0.98f, 0.88f, 0.10f)),
        new SphereColorDefinition("Green", new Color(0.18f, 0.80f, 0.25f)),
        new SphereColorDefinition("Teal", new Color(0.05f, 0.80f, 0.75f)),
        new SphereColorDefinition("SkyBlue", new Color(0.15f, 0.65f, 1.00f)),
        new SphereColorDefinition("RoyalBlue", new Color(0.20f, 0.25f, 0.90f)),
        new SphereColorDefinition("Purple", new Color(0.60f, 0.18f, 0.90f)),
        new SphereColorDefinition("Pink", new Color(0.95f, 0.25f, 0.65f)),
        new SphereColorDefinition("White", new Color(0.95f, 0.95f, 0.95f))
    };

    private void Awake()
    {
        EnsureGrid();
    }

    private void Start()
    {
        EnsureGrid();
    }

    [ContextMenu("Rebuild 10 Spheres")]
    public void RebuildGrid()
    {
        ClearSpheres();
        CreateSpheres();
    }

    public void EnsureGrid()
    {
        if (transform.childCount == rows * columns)
        {
            UpdateExistingSpheres();
        }
        else if (transform.childCount == 0)
        {
            CreateSpheres();
        }
    }

    private void ClearSpheres()
    {
        while (transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                DestroyImmediate(child.gameObject);
                continue;
            }
#endif
            Destroy(child.gameObject);
        }
    }

    public void CreateSpheres()
    {
        float startX = -((columns - 1) * spacingX) / 2f;
        float startY = ((rows - 1) * spacingY) / 2f;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                int index = r * columns + c;
                if (index >= DefaultColors.Length) break;

                var colorDef = DefaultColors[index];
                Vector3 localPos = new Vector3(
                    startX + c * spacingX,
                    startY - r * spacingY,
                    0f
                );

                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.name = $"Sphere_{r}_{c}_{colorDef.name}";
                sphere.transform.SetParent(transform, false);
                sphere.transform.localPosition = localPos;
                sphere.transform.localRotation = Quaternion.identity;
                sphere.transform.localScale = Vector3.one;

                ApplyColorToSphere(sphere, colorDef, index);
            }
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            EditorUtility.SetDirty(gameObject);
            if (gameObject.scene.IsValid())
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
            }
        }
#endif
    }

    private void UpdateExistingSpheres()
    {
        float startX = -((columns - 1) * spacingX) / 2f;
        float startY = ((rows - 1) * spacingY) / 2f;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                int index = r * columns + c;
                if (index >= transform.childCount || index >= DefaultColors.Length) break;

                Transform child = transform.GetChild(index);
                var colorDef = DefaultColors[index];
                Vector3 localPos = new Vector3(
                    startX + c * spacingX,
                    startY - r * spacingY,
                    0f
                );

                child.localPosition = localPos;
                child.localRotation = Quaternion.identity;
                child.localScale = Vector3.one;
                child.name = $"Sphere_{r}_{c}_{colorDef.name}";

                ApplyColorToSphere(child.gameObject, colorDef, index);
            }
        }
    }

    private void ApplyColorToSphere(GameObject sphere, SphereColorDefinition colorDef, int index)
    {
        Renderer rend = sphere.GetComponent<Renderer>();
        if (rend == null) return;

        Material mat = null;
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            mat = GetOrCreateMaterialAsset(colorDef, index);
            if (mat != null)
            {
                rend.sharedMaterial = mat;
            }
        }
#endif
        if (mat == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) shader = Shader.Find("Diffuse");

            mat = new Material(shader != null ? shader : Shader.Find("Sprites/Default"));
            mat.name = $"M_Sphere_{colorDef.name}";
            SetMaterialColor(mat, colorDef.color);
            rend.material = mat;
        }

        // Apply MaterialPropertyBlock as extra reliability for URP and Built-in
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        rend.GetPropertyBlock(mpb);
        mpb.SetColor("_BaseColor", colorDef.color);
        mpb.SetColor("_Color", colorDef.color);
        rend.SetPropertyBlock(mpb);
    }

    private static void SetMaterialColor(Material mat, Color color)
    {
        if (mat.HasProperty("_BaseColor"))
        {
            mat.SetColor("_BaseColor", color);
        }
        if (mat.HasProperty("_Color"))
        {
            mat.SetColor("_Color", color);
        }
        mat.color = color;
    }

#if UNITY_EDITOR
    private static Material GetOrCreateMaterialAsset(SphereColorDefinition colorDef, int index)
    {
        string folderPath = "Assets/Materials";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }

        string assetPath = $"{folderPath}/M_Sphere_{index}_{colorDef.name}.mat";
        Material existing = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
        if (existing != null)
        {
            SetMaterialColor(existing, colorDef.color);
            EditorUtility.SetDirty(existing);
            return existing;
        }

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        if (shader == null) shader = Shader.Find("Diffuse");

        Material mat = new Material(shader != null ? shader : Shader.Find("Sprites/Default"));
        mat.name = $"M_Sphere_{colorDef.name}";
        SetMaterialColor(mat, colorDef.color);
        AssetDatabase.CreateAsset(mat, assetPath);
        return mat;
    }

    [MenuItem("Tools/Rebuild 10 Spheres In Scene")]
    public static void GenerateInActiveScene()
    {
        SphereGridSpawner spawner = Object.FindFirstObjectByType<SphereGridSpawner>();
        if (spawner == null)
        {
            GameObject gridGo = new GameObject("SphereGrid");
            gridGo.transform.position = new Vector3(0f, 1f, 0f);
            spawner = gridGo.AddComponent<SphereGridSpawner>();
            Undo.RegisterCreatedObjectUndo(gridGo, "Create SphereGrid");
        }
        spawner.RebuildGrid();
    }
#endif
}
