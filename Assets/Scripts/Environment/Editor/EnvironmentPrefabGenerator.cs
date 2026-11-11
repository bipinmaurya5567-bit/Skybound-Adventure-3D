using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SkyboundAdventure
{
    public static class EnvironmentPrefabGenerator
    {
        private const string MaterialsFolder = "Assets/Art/Materials";
        private const string MeshesFolder = "Assets/Art/Meshes";
        private const string PrefabsFolder = "Assets/Prefabs/Environment";
        private const string TexturesFolder = "Assets/Art/Textures";

        [MenuItem("Skybound/Generate Master Broadleaf Tree Only")]
        public static string GenerateMasterTreeOnly()
        {
            EnsureDirectories();
            EnsureTextures();
            var mats = CreateOrLoadMaterials();

            CreateLargeLushTreePrefab(mats);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return "Master Broadleaf Tree (Tree_LargeLush) successfully generated!";
        }

        [MenuItem("Skybound/Generate Environment Prefabs & Materials")]
        public static string GenerateAllAssets()
        {
            EnsureDirectories();
            EnsureTextures();
            var mats = CreateOrLoadMaterials();

            // 1. Generate 3 Rock Prefabs
            CreateLargeBoulderPrefab(mats);
            CreateMediumStonePrefab(mats);
            CreateSmallRockClusterPrefab(mats);

            // 2. Generate 6 Tree Prefabs (5 User-Requested Variations + Fantasy Tree)
            CreateLargeLushTreePrefab(mats);
            CreateTallForestTreePrefab(mats);
            CreateTropicalPalmTreePrefab(mats);
            CreateAutumnTreePrefab(mats);
            CreateSmallDecorativeTreePrefab(mats);
            CreateFantasyTreePrefab(mats);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return "Environment prefabs, meshes, and materials successfully generated!";
        }

        private static void EnsureDirectories()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Art")) AssetDatabase.CreateFolder("Assets", "Art");
            if (!AssetDatabase.IsValidFolder(MaterialsFolder)) AssetDatabase.CreateFolder("Assets/Art", "Materials");
            if (!AssetDatabase.IsValidFolder(MeshesFolder)) AssetDatabase.CreateFolder("Assets/Art", "Meshes");
            if (!AssetDatabase.IsValidFolder(TexturesFolder)) AssetDatabase.CreateFolder("Assets/Art", "Textures");
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs")) AssetDatabase.CreateFolder("Assets", "Prefabs");
            if (!AssetDatabase.IsValidFolder(PrefabsFolder)) AssetDatabase.CreateFolder("Assets/Prefabs", "Environment");
        }

        private static void EnsureTextures()
        {
            // 1. Stylized Bark Texture (wood fibers, organic clefts)
            string barkPath = $"{TexturesFolder}/T_StylizedBark.png";
            if (!System.IO.File.Exists(barkPath))
            {
                Texture2D tex = new Texture2D(256, 256, TextureFormat.RGBA32, true);
                Color baseCol = new Color(0.90f, 0.82f, 0.72f);
                Color darkCol = new Color(0.65f, 0.52f, 0.40f);
                Color highCol = new Color(1.0f, 0.95f, 0.88f);

                for (int y = 0; y < 256; y++)
                {
                    for (int x = 0; x < 256; x++)
                    {
                        float u = (float)x / 256f;
                        float v = (float)y / 256f;
                        float n1 = Mathf.PerlinNoise(u * 14f, v * 2.5f);
                        float n2 = Mathf.PerlinNoise(u * 32f, v * 6f) * 0.35f;
                        float val = Mathf.Clamp01(n1 * 0.75f + n2);

                        Color c = Color.Lerp(darkCol, baseCol, val);
                        if (val > 0.72f) c = Color.Lerp(c, highCol, (val - 0.72f) * 3.5f);
                        tex.SetPixel(x, y, c);
                    }
                }
                tex.Apply();
                System.IO.File.WriteAllBytes(barkPath, tex.EncodeToPNG());
                AssetDatabase.ImportAsset(barkPath);
            }

            // 2. Stylized Foliage Texture (soft leaf dapples and cell highlights)
            string folPath = $"{TexturesFolder}/T_StylizedFoliage.png";
            if (!System.IO.File.Exists(folPath))
            {
                Texture2D tex = new Texture2D(256, 256, TextureFormat.RGBA32, true);
                Color baseCol = new Color(0.92f, 0.96f, 0.90f);
                Color darkCol = new Color(0.74f, 0.84f, 0.70f);
                Color highCol = new Color(1.0f, 1.0f, 0.98f);

                for (int y = 0; y < 256; y++)
                {
                    for (int x = 0; x < 256; x++)
                    {
                        float u = (float)x / 256f;
                        float v = (float)y / 256f;
                        float n1 = Mathf.PerlinNoise(u * 10f, v * 10f);
                        float n2 = Mathf.PerlinNoise(u * 24f, v * 24f) * 0.4f;
                        float val = Mathf.Clamp01(n1 * 0.7f + n2);

                        Color c = Color.Lerp(darkCol, baseCol, val);
                        if (val > 0.68f) c = Color.Lerp(c, highCol, (val - 0.68f) * 3.1f);
                        tex.SetPixel(x, y, c);
                    }
                }
                tex.Apply();
                System.IO.File.WriteAllBytes(folPath, tex.EncodeToPNG());
                AssetDatabase.ImportAsset(folPath);
            }

            // 3. Palm Trunk Texture (horizontal fibrous rings)
            string palmTrunkPath = $"{TexturesFolder}/T_PalmTrunk.png";
            if (!System.IO.File.Exists(palmTrunkPath))
            {
                Texture2D tex = new Texture2D(256, 256, TextureFormat.RGBA32, true);
                Color baseCol = new Color(0.88f, 0.78f, 0.65f);
                Color ringCol = new Color(0.58f, 0.45f, 0.32f);
                Color highCol = new Color(1.0f, 0.92f, 0.82f);

                for (int y = 0; y < 256; y++)
                {
                    for (int x = 0; x < 256; x++)
                    {
                        float u = (float)x / 256f;
                        float v = (float)y / 256f;
                        float ring = Mathf.Sin(v * Mathf.PI * 16f) * 0.5f + 0.5f;
                        float grain = Mathf.PerlinNoise(u * 22f, v * 4f) * 0.3f;
                        float val = Mathf.Clamp01(ring * 0.6f + grain);

                        Color c = Color.Lerp(ringCol, baseCol, val);
                        if (val > 0.70f) c = Color.Lerp(c, highCol, (val - 0.70f) * 3.3f);
                        tex.SetPixel(x, y, c);
                    }
                }
                tex.Apply();
                System.IO.File.WriteAllBytes(palmTrunkPath, tex.EncodeToPNG());
                AssetDatabase.ImportAsset(palmTrunkPath);
            }
        }

        #region Material Creation

        public class ArtMaterials
        {
            public Material Grass;
            public Material Soil;
            public Material Rock;
            public Material RockDark;
            public Material RockMoss;
            public Material Bark;
            public Material PalmTrunk;
            public Material Coconut;
            public Material CanopyDark;
            public Material CanopyGreen;
            public Material CanopyLight;
            public Material CanopyAutumn;
            public Material CanopyGold;
            public Material CanopyOrange;
            public Material CanopyFantasy;
            public Material PalmFrond;
            public Material PalmFrondLight;
            public Material Vine;
            public Material FlowerPink;
            public Material FlowerYellow;
            public Material FlowerBlue;
        }

        private static ArtMaterials CreateOrLoadMaterials()
        {
            Shader litShader = Shader.Find("Universal Render Pipeline/Lit");
            if (litShader == null) litShader = Shader.Find("Standard");

            var mats = new ArtMaterials();

            mats.Grass = GetOrCreateMat("M_StylizedGrass", litShader, new Color(0.24f, 0.65f, 0.22f), 0f, 0.25f);
            mats.Soil = GetOrCreateMat("M_StylizedSoil", litShader, new Color(0.38f, 0.24f, 0.16f), 0f, 0.25f);
            mats.Rock = GetOrCreateMat("M_StylizedRock", litShader, new Color(0.48f, 0.52f, 0.58f), 0f, 0.35f);
            mats.RockDark = GetOrCreateMat("M_StylizedRockDark", litShader, new Color(0.30f, 0.33f, 0.38f), 0f, 0.30f);
            mats.RockMoss = GetOrCreateMat("M_StylizedRockMoss", litShader, new Color(0.35f, 0.52f, 0.32f), 0f, 0.30f);

            mats.Bark = GetOrCreateMat("M_StylizedBark", litShader, new Color(0.44f, 0.28f, 0.16f), 0f, 0.20f, "T_StylizedBark");
            mats.PalmTrunk = GetOrCreateMat("M_StylizedPalmTrunk", litShader, new Color(0.52f, 0.36f, 0.22f), 0f, 0.24f, "T_PalmTrunk");
            mats.Coconut = GetOrCreateMat("M_StylizedCoconut", litShader, new Color(0.35f, 0.24f, 0.12f), 0f, 0.22f, "T_StylizedBark");

            mats.CanopyDark = GetOrCreateMat("M_StylizedCanopyDark", litShader, new Color(0.12f, 0.38f, 0.16f), 0f, 0.12f);
            mats.CanopyGreen = GetOrCreateMat("M_StylizedCanopyGreen", litShader, new Color(0.24f, 0.65f, 0.22f), 0f, 0.15f);
            mats.CanopyLight = GetOrCreateMat("M_StylizedCanopyLight", litShader, new Color(0.46f, 0.80f, 0.18f), 0f, 0.18f);

            mats.CanopyAutumn = GetOrCreateMat("M_StylizedCanopyAutumn", litShader, new Color(0.92f, 0.38f, 0.08f), 0f, 0.15f);
            mats.CanopyGold = GetOrCreateMat("M_StylizedCanopyGold", litShader, new Color(0.98f, 0.74f, 0.10f), 0f, 0.18f);
            mats.CanopyOrange = GetOrCreateMat("M_StylizedCanopyOrange", litShader, new Color(0.96f, 0.54f, 0.08f), 0f, 0.15f);

            mats.CanopyFantasy = GetOrCreateMat("M_StylizedCanopyFantasy", litShader, new Color(0.22f, 0.75f, 0.88f), 0.10f, 0.25f, null, new Color(0.04f, 0.22f, 0.28f));

            mats.PalmFrond = GetOrCreateMat("M_StylizedPalmFrond", litShader, new Color(0.22f, 0.68f, 0.16f), 0f, 0.20f);
            mats.PalmFrondLight = GetOrCreateMat("M_StylizedPalmFrondLight", litShader, new Color(0.42f, 0.82f, 0.20f), 0f, 0.20f);

            mats.Vine = GetOrCreateMat("M_StylizedVine", litShader, new Color(0.18f, 0.50f, 0.16f), 0f, 0.25f);
            mats.FlowerPink = GetOrCreateMat("M_StylizedFlowerPink", litShader, new Color(0.98f, 0.32f, 0.65f), 0f, 0.35f);
            mats.FlowerYellow = GetOrCreateMat("M_StylizedFlowerYellow", litShader, new Color(1.0f, 0.92f, 0.15f), 0f, 0.35f);
            mats.FlowerBlue = GetOrCreateMat("M_StylizedFlowerBlue", litShader, new Color(0.25f, 0.65f, 0.98f), 0f, 0.35f);

            return mats;
        }

        private static Material GetOrCreateMat(string name, Shader shader, Color color, float metallic, float smoothness, string textureName = null, Color? emission = null)
        {
            string path = $"{MaterialsFolder}/{name}.mat";
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                mat = new Material(shader) { name = name };
                AssetDatabase.CreateAsset(mat, path);
            }

            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);
            if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);

            if (!string.IsNullOrEmpty(textureName))
            {
                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>($"{TexturesFolder}/{textureName}.png");
                if (tex != null)
                {
                    if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", tex);
                    if (mat.HasProperty("_MainTex")) mat.SetTexture("_MainTex", tex);
                }
            }
            else
            {
                if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", null);
                if (mat.HasProperty("_MainTex")) mat.SetTexture("_MainTex", null);
            }

            if (emission.HasValue)
            {
                mat.EnableKeyword("_EMISSION");
                if (mat.HasProperty("_EmissionColor")) mat.SetColor("_EmissionColor", emission.Value);
            }

            EditorUtility.SetDirty(mat);
            return mat;
        }

        private static Mesh SaveMeshAsset(Mesh mesh, string name)
        {
            string path = $"{MeshesFolder}/{name}.asset";
            Mesh existing = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (existing != null)
            {
                existing.Clear();
                existing.vertices = mesh.vertices;
                existing.triangles = mesh.triangles;
                existing.normals = mesh.normals;
                existing.uv = mesh.uv;
                existing.colors = mesh.colors;
                existing.tangents = mesh.tangents;
                existing.RecalculateBounds();
                EditorUtility.SetDirty(existing);
                return existing;
            }
            AssetDatabase.CreateAsset(mesh, path);
            return mesh;
        }

        #endregion

        #region Rock Prefabs

        private static void CreateLargeBoulderPrefab(ArtMaterials mats)
        {
            GameObject root = new GameObject("Rock_LargeBoulder");
            Mesh m = EnvironmentModelFactory.CreateFacetedBoulderMesh(101, new Vector3(2.4f, 2.0f, 2.2f));
            m = SaveMeshAsset(m, "Mesh_Rock_LargeBoulder");

            var mf = root.AddComponent<MeshFilter>();
            mf.sharedMesh = m;
            var mr = root.AddComponent<MeshRenderer>();
            mr.sharedMaterial = mats.Rock;

            var mc = root.AddComponent<MeshCollider>();
            mc.sharedMesh = m;
            mc.convex = true;

            SavePrefab(root, "Rock_LargeBoulder");
            Object.DestroyImmediate(root);
        }

        private static void CreateMediumStonePrefab(ArtMaterials mats)
        {
            GameObject root = new GameObject("Rock_MediumStone");
            Mesh m = EnvironmentModelFactory.CreateFacetedBoulderMesh(202, new Vector3(1.4f, 1.1f, 1.3f));
            m = SaveMeshAsset(m, "Mesh_Rock_MediumStone");

            var mf = root.AddComponent<MeshFilter>();
            mf.sharedMesh = m;
            var mr = root.AddComponent<MeshRenderer>();
            mr.sharedMaterial = mats.RockDark;

            var bc = root.AddComponent<BoxCollider>();
            bc.center = new Vector3(0f, 0.45f, 0f);
            bc.size = new Vector3(1.3f, 0.9f, 1.2f);

            SavePrefab(root, "Rock_MediumStone");
            Object.DestroyImmediate(root);
        }

        private static void CreateSmallRockClusterPrefab(ArtMaterials mats)
        {
            GameObject root = new GameObject("Rock_SmallCluster");
            int[] seeds = { 301, 302, 303 };
            Vector3[] offsets = {
                new Vector3(-0.35f, 0.15f, -0.15f),
                new Vector3(0.35f, 0.10f, 0.20f),
                new Vector3(0.05f, 0.08f, -0.30f)
            };
            Vector3[] scales = {
                new Vector3(0.55f, 0.45f, 0.50f),
                new Vector3(0.35f, 0.28f, 0.32f),
                new Vector3(0.40f, 0.32f, 0.38f)
            };

            for (int i = 0; i < seeds.Length; i++)
            {
                GameObject rock = new GameObject($"Pebble_{i}");
                rock.transform.SetParent(root.transform, false);
                rock.transform.localPosition = offsets[i];
                rock.transform.localRotation = Quaternion.Euler(Random.Range(-15f, 15f), Random.Range(0f, 360f), Random.Range(-15f, 15f));

                Mesh m = EnvironmentModelFactory.CreateFacetedBoulderMesh(seeds[i], scales[i]);
                m = SaveMeshAsset(m, $"Mesh_Pebble_{i}");

                var mf = rock.AddComponent<MeshFilter>();
                mf.sharedMesh = m;
                var mr = rock.AddComponent<MeshRenderer>();
                mr.sharedMaterial = (i % 2 == 0) ? mats.Rock : mats.RockDark;
            }

            SavePrefab(root, "Rock_SmallCluster");
            Object.DestroyImmediate(root);
        }

        #endregion

        #region Tree Prefab 1: Hero Large Lush Green Tree

        private static void CreateLargeLushTreePrefab(ArtMaterials mats)
        {
            GameObject root = new GameObject("Tree_LargeLush");

            // 1. Organic Tapered Trunk with 4 Flared Buttress Roots and natural S-curve
            GameObject trunk = new GameObject("Trunk");
            trunk.transform.SetParent(root.transform, false);
            Mesh trunkMesh = EnvironmentModelFactory.CreateOrganicTrunkMesh(42, 3.4f, 0.40f, 0.22f, 0.24f, 0.14f, 4);
            trunkMesh = SaveMeshAsset(trunkMesh, "Mesh_Trunk_LargeLush");
            trunk.AddComponent<MeshFilter>().sharedMesh = trunkMesh;
            trunk.AddComponent<MeshRenderer>().sharedMaterial = mats.Bark;

            var cc = trunk.AddComponent<CapsuleCollider>();
            cc.center = new Vector3(0.08f, 1.6f, 0.05f);
            cc.radius = 0.42f;
            cc.height = 3.2f;

            // 2. Visible 3D Branching Architecture (Ascending upward sweep, physically supporting foliage)
            GameObject branches = new GameObject("Branches");
            branches.transform.SetParent(root.transform, false);

            // Primary Branch 1: West / Left arching bough (sweeps upward-west at 42°)
            CreateBranch(branches.transform, "Branch_Left",
                new Vector3(-0.08f, 2.7f, 0.05f),
                Quaternion.Euler(-42f, -60f, 15f),
                EnvironmentModelFactory.CreateBranchTubeMesh(501, 2.1f, 0.18f, 0.08f, 18f, 0.42f),
                mats.Bark, "Mesh_Branch_Lush_L");

            // Primary Branch 2: East / Right arching bough (sweeps upward-east at 45°)
            CreateBranch(branches.transform, "Branch_Right",
                new Vector3(0.10f, 2.8f, 0.06f),
                Quaternion.Euler(-45f, 60f, -15f),
                EnvironmentModelFactory.CreateBranchTubeMesh(502, 2.0f, 0.17f, 0.08f, -16f, 0.38f),
                mats.Bark, "Mesh_Branch_Lush_R");

            // Primary Branch 3: North / Back arching bough (sweeps upward-north at 48°)
            CreateBranch(branches.transform, "Branch_Back",
                new Vector3(0.0f, 2.9f, -0.08f),
                Quaternion.Euler(-48f, 175f, 0f),
                EnvironmentModelFactory.CreateBranchTubeMesh(503, 1.9f, 0.16f, 0.07f, 14f, 0.45f),
                mats.Bark, "Mesh_Branch_Lush_C");

            // Primary Branch 4: Center Leader (ascends steeply upward through crown)
            CreateBranch(branches.transform, "Branch_Center",
                new Vector3(0.04f, 3.1f, 0.03f),
                Quaternion.Euler(-75f, 15f, 0f),
                EnvironmentModelFactory.CreateBranchTubeMesh(504, 2.1f, 0.18f, 0.08f, 12f, 0.52f),
                mats.Bark, "Mesh_Branch_Lush_Leader");

            // Secondary branch spur 1 (off West bough)
            CreateBranch(branches.transform, "Branch_Spur_W",
                new Vector3(-0.85f, 3.5f, 0.4f),
                Quaternion.Euler(-35f, -95f, 18f),
                EnvironmentModelFactory.CreateBranchTubeMesh(505, 1.2f, 0.11f, 0.055f, -14f, 0.38f),
                mats.Bark, "Mesh_Branch_Lush_SpurW");

            // Secondary branch spur 2 (off East bough)
            CreateBranch(branches.transform, "Branch_Spur_E",
                new Vector3(0.85f, 3.5f, 0.4f),
                Quaternion.Euler(-35f, 95f, -18f),
                EnvironmentModelFactory.CreateBranchTubeMesh(506, 1.2f, 0.11f, 0.055f, 12f, 0.36f),
                mats.Bark, "Mesh_Branch_Lush_SpurE");

            // Secondary branch spur 3 (forward reaching, visible from front)
            CreateBranch(branches.transform, "Branch_Spur_F",
                new Vector3(-0.02f, 2.8f, 0.10f),
                Quaternion.Euler(-38f, 10f, -8f),
                EnvironmentModelFactory.CreateBranchTubeMesh(507, 1.3f, 0.12f, 0.06f, 16f, 0.35f),
                mats.Bark, "Mesh_Branch_Lush_SpurF");

            // 3. Hanging Vine wrapping trunk
            CreateHangingVine(root.transform, new Vector3(0.28f, 1.4f, 0.08f), mats.Vine);

            // 4. Volumetric Organic Leaf Clusters (19 scalloped leaf bunches forming an arched broadleaf dome)
            GameObject foliage = new GameObject("Foliage");
            foliage.transform.SetParent(root.transform, false);

            // Left Bough Canopy
            CreateFoliageCluster(foliage.transform, "Foliage_Left_Tip", new Vector3(-2.0f, 4.0f, 0.6f), new Vector3(0.92f, 0.78f, 0.88f), mats.CanopyDark, 101);
            CreateFoliageCluster(foliage.transform, "Foliage_Left_Mid", new Vector3(-1.4f, 4.3f, 0.3f), new Vector3(0.85f, 0.72f, 0.82f), mats.CanopyGreen, 102);
            CreateFoliageCluster(foliage.transform, "Foliage_Left_Spur", new Vector3(-1.7f, 4.5f, -0.1f), new Vector3(0.80f, 0.68f, 0.78f), mats.CanopyGreen, 103);
            CreateFoliageCluster(foliage.transform, "Foliage_Left_Hang", new Vector3(-1.6f, 3.5f, 0.7f), new Vector3(0.72f, 0.62f, 0.70f), mats.CanopyDark, 104);

            // Right Bough Canopy
            CreateFoliageCluster(foliage.transform, "Foliage_Right_Tip", new Vector3(1.9f, 4.1f, 0.7f), new Vector3(0.92f, 0.78f, 0.88f), mats.CanopyDark, 105);
            CreateFoliageCluster(foliage.transform, "Foliage_Right_Mid", new Vector3(1.4f, 4.4f, 0.3f), new Vector3(0.85f, 0.72f, 0.82f), mats.CanopyGreen, 106);
            CreateFoliageCluster(foliage.transform, "Foliage_Right_Spur", new Vector3(1.6f, 4.5f, -0.1f), new Vector3(0.80f, 0.68f, 0.78f), mats.CanopyGreen, 107);
            CreateFoliageCluster(foliage.transform, "Foliage_Right_Hang", new Vector3(1.5f, 3.6f, 0.8f), new Vector3(0.72f, 0.62f, 0.70f), mats.CanopyDark, 108);

            // Back Bough Canopy
            CreateFoliageCluster(foliage.transform, "Foliage_Back_Tip", new Vector3(0.0f, 4.4f, -1.7f), new Vector3(0.92f, 0.78f, 0.88f), mats.CanopyDark, 109);
            CreateFoliageCluster(foliage.transform, "Foliage_Back_Left", new Vector3(-0.8f, 4.7f, -1.2f), new Vector3(0.85f, 0.72f, 0.82f), mats.CanopyGreen, 110);
            CreateFoliageCluster(foliage.transform, "Foliage_Back_Right", new Vector3(0.8f, 4.6f, -1.2f), new Vector3(0.85f, 0.72f, 0.82f), mats.CanopyGreen, 111);
            CreateFoliageCluster(foliage.transform, "Foliage_Back_Hang", new Vector3(0.0f, 3.8f, -1.4f), new Vector3(0.72f, 0.62f, 0.70f), mats.CanopyDark, 112);

            // Forward Bough Canopy
            CreateFoliageCluster(foliage.transform, "Foliage_Front_Tip", new Vector3(-0.05f, 3.9f, 1.3f), new Vector3(0.85f, 0.72f, 0.82f), mats.CanopyGreen, 113);
            CreateFoliageCluster(foliage.transform, "Foliage_Front_Hang", new Vector3(0.45f, 3.5f, 1.1f), new Vector3(0.72f, 0.62f, 0.70f), mats.CanopyDark, 114);

            // Center Crown Tier (Lush volume connecting branches to upper crown)
            CreateFoliageCluster(foliage.transform, "Foliage_Center_Core", new Vector3(0.05f, 4.6f, 0.05f), new Vector3(1.05f, 0.85f, 1.0f), mats.CanopyGreen, 115);
            CreateFoliageCluster(foliage.transform, "Foliage_Center_West", new Vector3(-0.65f, 5.0f, 0.15f), new Vector3(0.90f, 0.78f, 0.88f), mats.CanopyGreen, 116);
            CreateFoliageCluster(foliage.transform, "Foliage_Center_East", new Vector3(0.65f, 4.9f, -0.15f), new Vector3(0.90f, 0.78f, 0.88f), mats.CanopyGreen, 117);

            // Sunlit Upper Crown Peak (Vibrant lime sunlit highlights, asymmetrical silhouette)
            CreateFoliageCluster(foliage.transform, "Foliage_Crown_Main", new Vector3(0.0f, 5.5f, 0.08f), new Vector3(0.95f, 0.82f, 0.92f), mats.CanopyLight, 118);
            CreateFoliageCluster(foliage.transform, "Foliage_Crown_Peak", new Vector3(-0.25f, 6.05f, 0.05f), new Vector3(0.78f, 0.72f, 0.75f), mats.CanopyLight, 119);
            CreateFoliageCluster(foliage.transform, "Foliage_Crown_Sunlit", new Vector3(0.30f, 5.75f, 0.25f), new Vector3(0.68f, 0.62f, 0.68f), mats.CanopyLight, 120);

            // 5. Apply Ghibli/Genshin Canopy-Center Normal Transfer for soft cohesive volume lighting
            ApplyTreeCanopyNormals(root, new Vector3(0f, 4.6f, 0.05f), 0.82f);

            // 6. Ground flora ring
            CreateGroundFloraRing(root.transform, mats);

            SavePrefab(root, "Tree_LargeLush");
            Object.DestroyImmediate(root);
        }

        private static void ApplyTreeCanopyNormals(GameObject treeRoot, Vector3 canopyCenter, float blendWeight = 0.82f)
        {
            Transform foliage = treeRoot.transform.Find("Foliage");
            if (foliage == null) return;

            MeshFilter[] mfs = foliage.GetComponentsInChildren<MeshFilter>();
            foreach (var mf in mfs)
            {
                if (mf.sharedMesh == null) continue;

                Mesh m = Object.Instantiate(mf.sharedMesh);
                Vector3[] verts = m.vertices;
                Vector3[] normals = m.normals;

                for (int i = 0; i < verts.Length; i++)
                {
                    Vector3 rootV = mf.transform.localPosition + mf.transform.localRotation * Vector3.Scale(verts[i], mf.transform.localScale);
                    Vector3 radialNorm = (rootV - canopyCenter + Vector3.up * 0.45f).normalized;
                    Vector3 localRadial = Quaternion.Inverse(mf.transform.localRotation) * radialNorm;

                    normals[i] = Vector3.Lerp(normals[i], localRadial, blendWeight).normalized;
                }

                m.normals = normals;
                m.RecalculateBounds();

                string meshName = $"Mesh_{treeRoot.name}_{mf.gameObject.name}";
                m = SaveMeshAsset(m, meshName);
                mf.sharedMesh = m;
            }
        }

        #endregion

        #region Tree Prefab 2: Tall Forest Tree (Vertical Stately Silhouette)

        private static void CreateTallForestTreePrefab(ArtMaterials mats)
        {
            GameObject root = new GameObject("Tree_TallForest");

            // Taller, slender trunk with vertical taper
            GameObject trunk = new GameObject("Trunk");
            trunk.transform.SetParent(root.transform, false);
            Mesh trunkMesh = EnvironmentModelFactory.CreateOrganicTrunkMesh(77, 5.4f, 0.38f, 0.18f, 0.18f, 0.12f, 4);
            trunkMesh = SaveMeshAsset(trunkMesh, "Mesh_Trunk_TallForest");
            trunk.AddComponent<MeshFilter>().sharedMesh = trunkMesh;
            trunk.AddComponent<MeshRenderer>().sharedMaterial = mats.Bark;

            var cc = trunk.AddComponent<CapsuleCollider>();
            cc.center = new Vector3(0.06f, 2.7f, 0.04f);
            cc.radius = 0.40f;
            cc.height = 5.4f;

            // Ascending branches spiraling upward
            GameObject branches = new GameObject("Branches");
            branches.transform.SetParent(root.transform, false);

            CreateBranch(branches.transform, "Branch_1", new Vector3(0.05f, 3.6f, 0.05f),
                Quaternion.Euler(-20f, -40f, 25f),
                EnvironmentModelFactory.CreateBranchTubeMesh(601, 1.4f, 0.15f, 0.07f, 15f, 0.50f),
                mats.Bark, "Mesh_Branch_Tall_1");

            CreateBranch(branches.transform, "Branch_2", new Vector3(0.07f, 4.3f, 0.06f),
                Quaternion.Euler(15f, 80f, -25f),
                EnvironmentModelFactory.CreateBranchTubeMesh(602, 1.3f, 0.14f, 0.07f, -12f, 0.48f),
                mats.Bark, "Mesh_Branch_Tall_2");

            CreateBranch(branches.transform, "Branch_3", new Vector3(0.08f, 4.9f, 0.06f),
                Quaternion.Euler(-10f, 170f, 20f),
                EnvironmentModelFactory.CreateBranchTubeMesh(603, 1.2f, 0.13f, 0.06f, 10f, 0.52f),
                mats.Bark, "Mesh_Branch_Tall_3");

            // Tiered vertical canopy (9 clusters forming an elongated silhouette)
            GameObject foliage = new GameObject("Foliage");
            foliage.transform.SetParent(root.transform, false);

            CreateFoliageCluster(foliage.transform, "Foliage_Tier1_L", new Vector3(-1.1f, 4.3f, 0.6f), new Vector3(1.5f, 1.2f, 1.4f), mats.CanopyDark, 201);
            CreateFoliageCluster(foliage.transform, "Foliage_Tier1_R", new Vector3(1.0f, 4.8f, -0.6f), new Vector3(1.4f, 1.15f, 1.3f), mats.CanopyDark, 202);
            CreateFoliageCluster(foliage.transform, "Foliage_Tier2_C", new Vector3(0.1f, 5.5f, 0.1f), new Vector3(1.9f, 1.5f, 1.8f), mats.CanopyGreen, 203);
            CreateFoliageCluster(foliage.transform, "Foliage_Tier2_F", new Vector3(-0.2f, 5.3f, 0.8f), new Vector3(1.4f, 1.2f, 1.4f), mats.CanopyGreen, 204);
            CreateFoliageCluster(foliage.transform, "Foliage_Tier2_B", new Vector3(0.2f, 5.2f, -0.8f), new Vector3(1.4f, 1.2f, 1.4f), mats.CanopyGreen, 205);
            CreateFoliageCluster(foliage.transform, "Foliage_Tier3_Main", new Vector3(0.0f, 6.4f, 0.0f), new Vector3(1.6f, 1.4f, 1.6f), mats.CanopyLight, 206);
            CreateFoliageCluster(foliage.transform, "Foliage_Tier3_L", new Vector3(-0.5f, 6.2f, 0.3f), new Vector3(1.2f, 1.0f, 1.2f), mats.CanopyGreen, 207);
            CreateFoliageCluster(foliage.transform, "Foliage_Tier3_R", new Vector3(0.5f, 6.1f, -0.3f), new Vector3(1.2f, 1.0f, 1.2f), mats.CanopyGreen, 208);
            CreateFoliageCluster(foliage.transform, "Foliage_Spire_Top", new Vector3(0.0f, 7.2f, 0.0f), new Vector3(1.3f, 1.3f, 1.3f), mats.CanopyLight, 209);

            ApplyTreeCanopyNormals(root, new Vector3(0f, 5.8f, 0f), 0.82f);

            SavePrefab(root, "Tree_TallForest");
            Object.DestroyImmediate(root);
        }

        #endregion

        #region Tree Prefab 3: Tropical Palm Tree (Feathered Fronds & Coconuts)

        private static void CreateTropicalPalmTreePrefab(ArtMaterials mats)
        {
            GameObject root = new GameObject("Tree_TropicalPalm");

            // Segmented curved palm trunk with flared horizontal rings
            GameObject trunk = new GameObject("Trunk");
            trunk.transform.SetParent(root.transform, false);
            Mesh trunkMesh = EnvironmentModelFactory.CreateStylizedPalmTrunkMesh(14, 5.4f, 26f, 0.38f, 0.20f);
            trunkMesh = SaveMeshAsset(trunkMesh, "Mesh_Trunk_Palm_Segmented");
            trunk.AddComponent<MeshFilter>().sharedMesh = trunkMesh;
            trunk.AddComponent<MeshRenderer>().sharedMaterial = mats.PalmTrunk;

            var cc = trunk.AddComponent<CapsuleCollider>();
            cc.center = new Vector3(0.55f, 2.5f, 0.05f);
            cc.radius = 0.42f;
            cc.height = 5.0f;

            // Crown at trunk tip
            Vector3 crownPos = new Vector3(1.20f, 4.95f, 0.08f);
            GameObject crown = new GameObject("PalmCrown");
            crown.transform.SetParent(root.transform, false);
            crown.transform.localPosition = crownPos;

            // 1. Coconut Cluster at crown center
            GameObject coconuts = new GameObject("Coconuts");
            coconuts.transform.SetParent(crown.transform, false);
            Mesh coconutMesh = EnvironmentModelFactory.CreateCoconutMesh(0.20f);
            coconutMesh = SaveMeshAsset(coconutMesh, "Mesh_Coconut_Prop");

            Vector3[] cocoOffsets = {
                new Vector3(-0.16f, -0.12f, 0.12f),
                new Vector3(0.18f, -0.14f, 0.10f),
                new Vector3(0.02f, -0.16f, -0.16f),
                new Vector3(-0.08f, -0.22f, -0.05f)
            };
            for (int i = 0; i < cocoOffsets.Length; i++)
            {
                GameObject coco = new GameObject($"Coconut_{i}");
                coco.transform.SetParent(coconuts.transform, false);
                coco.transform.localPosition = cocoOffsets[i];
                coco.transform.localRotation = Quaternion.Euler(Random.Range(-20f, 20f), Random.Range(0f, 360f), Random.Range(-20f, 20f));
                coco.AddComponent<MeshFilter>().sharedMesh = coconutMesh;
                coco.AddComponent<MeshRenderer>().sharedMaterial = mats.Coconut;
            }

            // 2. Feathered 3D Palm Fronds (11 fronds arranged in 3 tiers with individual narrow leaflets)
            Mesh frondMesh = EnvironmentModelFactory.CreateFeatheredPalmFrondMesh(3.2f, 0.85f, 36f, 18);
            frondMesh = SaveMeshAsset(frondMesh, "Mesh_PalmFrond_Feathered");

            // Tier 1: Top young fronds (3 fronds, arching upward/outward at +14°)
            for (int i = 0; i < 3; i++)
            {
                float angle = (float)i / 3 * 360f + 15f;
                GameObject frond = new GameObject($"Frond_Top_{i}");
                frond.transform.SetParent(crown.transform, false);
                frond.transform.localPosition = new Vector3(0f, 0.08f, 0f);
                frond.transform.localRotation = Quaternion.Euler(-12f, angle, 0f);

                frond.AddComponent<MeshFilter>().sharedMesh = frondMesh;
                frond.AddComponent<MeshRenderer>().sharedMaterial = mats.PalmFrondLight;
            }

            // Tier 2: Mid mature fronds (4 fronds, reaching broadly outward at -10°)
            for (int i = 0; i < 4; i++)
            {
                float angle = (float)i / 4 * 360f + 45f;
                GameObject frond = new GameObject($"Frond_Mid_{i}");
                frond.transform.SetParent(crown.transform, false);
                frond.transform.localPosition = Vector3.zero;
                frond.transform.localRotation = Quaternion.Euler(-24f, angle, 0f);

                frond.AddComponent<MeshFilter>().sharedMesh = frondMesh;
                frond.AddComponent<MeshRenderer>().sharedMaterial = (i % 2 == 0) ? mats.PalmFrond : mats.PalmFrondLight;
            }

            // Tier 3: Bottom drooping fronds (4 fronds, drooping downward at -38°)
            for (int i = 0; i < 4; i++)
            {
                float angle = (float)i / 4 * 360f + 10f;
                GameObject frond = new GameObject($"Frond_Bot_{i}");
                frond.transform.SetParent(crown.transform, false);
                frond.transform.localPosition = new Vector3(0f, -0.10f, 0f);
                frond.transform.localRotation = Quaternion.Euler(-38f, angle, 0f);

                frond.AddComponent<MeshFilter>().sharedMesh = frondMesh;
                frond.AddComponent<MeshRenderer>().sharedMaterial = mats.PalmFrond;
            }

            SavePrefab(root, "Tree_TropicalPalm");
            Object.DestroyImmediate(root);
        }

        #endregion

        #region Tree Prefab 4: Sprawling Autumn Oak Tree

        private static void CreateAutumnTreePrefab(ArtMaterials mats)
        {
            GameObject root = new GameObject("Tree_AutumnOak");

            // Sturdy gnarled oak trunk with strong buttress root flares
            GameObject trunk = new GameObject("Trunk");
            trunk.transform.SetParent(root.transform, false);
            Mesh trunkMesh = EnvironmentModelFactory.CreateOrganicTrunkMesh(123, 3.6f, 0.48f, 0.28f, 0.25f, 0.22f, 4);
            trunkMesh = SaveMeshAsset(trunkMesh, "Mesh_Trunk_AutumnOak");
            trunk.AddComponent<MeshFilter>().sharedMesh = trunkMesh;
            trunk.AddComponent<MeshRenderer>().sharedMaterial = mats.Bark;

            var cc = trunk.AddComponent<CapsuleCollider>();
            cc.center = new Vector3(0.10f, 1.8f, 0.08f);
            cc.radius = 0.52f;
            cc.height = 3.6f;

            // Broad sprawling horizontal branches reaching in 4 quadrants
            GameObject branches = new GameObject("Branches");
            branches.transform.SetParent(root.transform, false);

            CreateBranch(branches.transform, "Branch_Oak_W", new Vector3(-0.05f, 3.0f, 0.05f),
                Quaternion.Euler(-18f, -75f, 10f),
                EnvironmentModelFactory.CreateBranchTubeMesh(701, 2.1f, 0.20f, 0.09f, 20f, 0.32f),
                mats.Bark, "Mesh_Branch_Oak_W");

            CreateBranch(branches.transform, "Branch_Oak_E", new Vector3(0.12f, 3.1f, 0.05f),
                Quaternion.Euler(14f, 85f, -15f),
                EnvironmentModelFactory.CreateBranchTubeMesh(702, 2.2f, 0.21f, 0.09f, -16f, 0.30f),
                mats.Bark, "Mesh_Branch_Oak_E");

            CreateBranch(branches.transform, "Branch_Oak_N", new Vector3(0.05f, 3.2f, 0.12f),
                Quaternion.Euler(-22f, 15f, 8f),
                EnvironmentModelFactory.CreateBranchTubeMesh(703, 1.9f, 0.18f, 0.08f, 14f, 0.36f),
                mats.Bark, "Mesh_Branch_Oak_N");

            CreateBranch(branches.transform, "Branch_Oak_S", new Vector3(0.08f, 2.9f, -0.08f),
                Quaternion.Euler(18f, -165f, -12f),
                EnvironmentModelFactory.CreateBranchTubeMesh(704, 1.8f, 0.18f, 0.08f, -15f, 0.34f),
                mats.Bark, "Mesh_Branch_Oak_S");

            // Volumetric leaf masses in rich warm autumn tones (13 clusters)
            GameObject foliage = new GameObject("Foliage");
            foliage.transform.SetParent(root.transform, false);

            // Lower perimeter clusters (Autumn Orange & Russet)
            CreateFoliageCluster(foliage.transform, "Foliage_W", new Vector3(-1.8f, 3.7f, 0.6f), new Vector3(1.9f, 1.35f, 1.8f), mats.CanopyAutumn, 301);
            CreateFoliageCluster(foliage.transform, "Foliage_E", new Vector3(1.8f, 3.6f, -0.5f), new Vector3(1.9f, 1.4f, 1.8f), mats.CanopyAutumn, 302);
            CreateFoliageCluster(foliage.transform, "Foliage_N", new Vector3(0.3f, 3.8f, 1.7f), new Vector3(1.8f, 1.3f, 1.7f), mats.CanopyOrange, 303);
            CreateFoliageCluster(foliage.transform, "Foliage_S", new Vector3(-0.2f, 3.5f, -1.6f), new Vector3(1.7f, 1.25f, 1.6f), mats.CanopyOrange, 304);

            // Mid body clusters (Warm Orange & Golden Yellow)
            CreateFoliageCluster(foliage.transform, "Foliage_Mid_C", new Vector3(0.1f, 4.4f, 0.1f), new Vector3(2.5f, 1.7f, 2.4f), mats.CanopyGold, 305);
            CreateFoliageCluster(foliage.transform, "Foliage_Mid_NW", new Vector3(-1.1f, 4.3f, 1.1f), new Vector3(1.8f, 1.35f, 1.7f), mats.CanopyAutumn, 306);
            CreateFoliageCluster(foliage.transform, "Foliage_Mid_SE", new Vector3(1.2f, 4.2f, -1.0f), new Vector3(1.7f, 1.3f, 1.7f), mats.CanopyGold, 307);
            CreateFoliageCluster(foliage.transform, "Foliage_Mid_NE", new Vector3(1.1f, 4.4f, 0.9f), new Vector3(1.7f, 1.3f, 1.6f), mats.CanopyOrange, 308);
            CreateFoliageCluster(foliage.transform, "Foliage_Mid_SW", new Vector3(-1.0f, 4.2f, -0.9f), new Vector3(1.6f, 1.25f, 1.6f), mats.CanopyAutumn, 309);

            // Upper crown clusters (Golden Yellow highlights)
            CreateFoliageCluster(foliage.transform, "Foliage_Top_C", new Vector3(0.0f, 5.2f, 0.0f), new Vector3(2.0f, 1.4f, 1.9f), mats.CanopyGold, 310);
            CreateFoliageCluster(foliage.transform, "Foliage_Top_L", new Vector3(-0.6f, 4.9f, 0.3f), new Vector3(1.4f, 1.1f, 1.4f), mats.CanopyGold, 311);
            CreateFoliageCluster(foliage.transform, "Foliage_Top_R", new Vector3(0.6f, 4.8f, -0.3f), new Vector3(1.4f, 1.1f, 1.4f), mats.CanopyOrange, 312);
            CreateFoliageCluster(foliage.transform, "Foliage_Peak", new Vector3(0.1f, 5.8f, 0.1f), new Vector3(1.3f, 1.1f, 1.3f), mats.CanopyGold, 313);

            ApplyTreeCanopyNormals(root, new Vector3(0f, 4.8f, 0f), 0.82f);

            SavePrefab(root, "Tree_AutumnOak");
            Object.DestroyImmediate(root);
        }

        #endregion

        #region Tree Prefab 5: Small Decorative Tree (Compact & Charming)

        private static void CreateSmallDecorativeTreePrefab(ArtMaterials mats)
        {
            GameObject root = new GameObject("Tree_DecorativeSmall");

            // Slender curving trunk
            GameObject trunk = new GameObject("Trunk");
            trunk.transform.SetParent(root.transform, false);
            Mesh trunkMesh = EnvironmentModelFactory.CreateOrganicTrunkMesh(88, 2.2f, 0.28f, 0.16f, 0.16f, 0.10f, 3);
            trunkMesh = SaveMeshAsset(trunkMesh, "Mesh_Trunk_SmallDeco");
            trunk.AddComponent<MeshFilter>().sharedMesh = trunkMesh;
            trunk.AddComponent<MeshRenderer>().sharedMaterial = mats.Bark;

            var cc = trunk.AddComponent<CapsuleCollider>();
            cc.center = new Vector3(0.06f, 1.1f, 0.04f);
            cc.radius = 0.32f;
            cc.height = 2.2f;

            // Two primary branches
            GameObject branches = new GameObject("Branches");
            branches.transform.SetParent(root.transform, false);

            CreateBranch(branches.transform, "Branch_Deco_L", new Vector3(0.04f, 1.9f, 0.03f),
                Quaternion.Euler(-20f, -50f, 18f),
                EnvironmentModelFactory.CreateBranchTubeMesh(801, 1.1f, 0.12f, 0.06f, 15f, 0.35f),
                mats.Bark, "Mesh_Branch_Deco_L");

            CreateBranch(branches.transform, "Branch_Deco_R", new Vector3(0.06f, 1.95f, 0.04f),
                Quaternion.Euler(15f, 65f, -16f),
                EnvironmentModelFactory.CreateBranchTubeMesh(802, 1.1f, 0.12f, 0.06f, -14f, 0.32f),
                mats.Bark, "Mesh_Branch_Deco_R");

            // Compact volumetric canopy (6 leaf clusters)
            GameObject foliage = new GameObject("Foliage");
            foliage.transform.SetParent(root.transform, false);

            CreateFoliageCluster(foliage.transform, "Foliage_L", new Vector3(-0.85f, 2.5f, 0.45f), new Vector3(1.2f, 0.95f, 1.1f), mats.CanopyGreen, 401);
            CreateFoliageCluster(foliage.transform, "Foliage_R", new Vector3(0.80f, 2.45f, -0.40f), new Vector3(1.15f, 0.90f, 1.1f), mats.CanopyGreen, 402);
            CreateFoliageCluster(foliage.transform, "Foliage_Center", new Vector3(0.05f, 2.85f, 0.05f), new Vector3(1.5f, 1.2f, 1.4f), mats.CanopyGreen, 403);
            CreateFoliageCluster(foliage.transform, "Foliage_F", new Vector3(0.1f, 2.7f, 0.7f), new Vector3(1.05f, 0.85f, 1.0f), mats.CanopyDark, 404);
            CreateFoliageCluster(foliage.transform, "Foliage_B", new Vector3(-0.1f, 2.65f, -0.6f), new Vector3(1.0f, 0.80f, 1.0f), mats.CanopyDark, 405);
            CreateFoliageCluster(foliage.transform, "Foliage_Top", new Vector3(0.0f, 3.4f, 0.0f), new Vector3(1.1f, 0.95f, 1.1f), mats.CanopyLight, 406);

            ApplyTreeCanopyNormals(root, new Vector3(0f, 2.8f, 0f), 0.82f);

            SavePrefab(root, "Tree_DecorativeSmall");
            Object.DestroyImmediate(root);
        }

        #endregion

        #region Tree Prefab 6: Mythic Fantasy Celestial Tree

        private static void CreateFantasyTreePrefab(ArtMaterials mats)
        {
            GameObject root = new GameObject("Tree_FantasySky");

            GameObject trunk = new GameObject("Trunk");
            trunk.transform.SetParent(root.transform, false);
            Mesh trunkMesh = EnvironmentModelFactory.CreateOrganicTrunkMesh(333, 4.0f, 0.42f, 0.22f, 0.28f, 0.20f, 4);
            trunkMesh = SaveMeshAsset(trunkMesh, "Mesh_Trunk_Fantasy_Organic");
            trunk.AddComponent<MeshFilter>().sharedMesh = trunkMesh;
            trunk.AddComponent<MeshRenderer>().sharedMaterial = mats.Bark;

            var cc = trunk.AddComponent<CapsuleCollider>();
            cc.center = new Vector3(0.10f, 2.0f, 0.08f);
            cc.radius = 0.46f;
            cc.height = 4.0f;

            // Celestial Branches
            GameObject branches = new GameObject("Branches");
            branches.transform.SetParent(root.transform, false);

            CreateBranch(branches.transform, "Branch_F1", new Vector3(0.08f, 3.4f, 0.06f),
                Quaternion.Euler(-22f, -60f, 18f),
                EnvironmentModelFactory.CreateBranchTubeMesh(901, 1.8f, 0.17f, 0.08f, 18f, 0.40f),
                mats.Bark, "Mesh_Branch_Fantasy_1");

            CreateBranch(branches.transform, "Branch_F2", new Vector3(0.12f, 3.5f, 0.07f),
                Quaternion.Euler(16f, 75f, -18f),
                EnvironmentModelFactory.CreateBranchTubeMesh(902, 1.9f, 0.18f, 0.08f, -16f, 0.38f),
                mats.Bark, "Mesh_Branch_Fantasy_2");

            // Volumetric Luminescent Azure Foliage
            GameObject foliage = new GameObject("Foliage");
            foliage.transform.SetParent(root.transform, false);

            CreateFoliageCluster(foliage.transform, "Foliage_L", new Vector3(-1.4f, 4.2f, 0.7f), new Vector3(1.7f, 1.3f, 1.6f), mats.CanopyFantasy, 501);
            CreateFoliageCluster(foliage.transform, "Foliage_R", new Vector3(1.4f, 4.1f, -0.6f), new Vector3(1.7f, 1.3f, 1.6f), mats.CanopyFantasy, 502);
            CreateFoliageCluster(foliage.transform, "Foliage_Main", new Vector3(0.1f, 4.8f, 0.1f), new Vector3(2.3f, 1.6f, 2.2f), mats.CanopyFantasy, 503);
            CreateFoliageCluster(foliage.transform, "Foliage_F", new Vector3(-0.1f, 4.6f, 1.2f), new Vector3(1.6f, 1.2f, 1.5f), mats.CanopyFantasy, 504);
            CreateFoliageCluster(foliage.transform, "Foliage_B", new Vector3(0.2f, 4.5f, -1.1f), new Vector3(1.5f, 1.2f, 1.5f), mats.CanopyFantasy, 505);
            CreateFoliageCluster(foliage.transform, "Foliage_Top", new Vector3(0.0f, 5.6f, 0.0f), new Vector3(1.8f, 1.4f, 1.7f), mats.CanopyFantasy, 506);

            ApplyTreeCanopyNormals(root, new Vector3(0f, 4.8f, 0f), 0.82f);

            SavePrefab(root, "Tree_FantasySky");
            Object.DestroyImmediate(root);
        }

        #endregion

        #region Helper Builders

        private static void CreateBranch(Transform parent, string name, Vector3 pos, Quaternion rot, Mesh mesh, Material mat, string meshName)
        {
            GameObject branch = new GameObject(name);
            branch.transform.SetParent(parent, false);
            branch.transform.localPosition = pos;
            branch.transform.localRotation = rot;

            mesh = SaveMeshAsset(mesh, meshName);
            branch.AddComponent<MeshFilter>().sharedMesh = mesh;
            branch.AddComponent<MeshRenderer>().sharedMaterial = mat;
        }

        private static void CreateFoliageCluster(Transform parent, string name, Vector3 localPos, Vector3 scale, Material mat, int seed)
        {
            GameObject clump = new GameObject(name);
            clump.transform.SetParent(parent, false);
            clump.transform.localPosition = localPos;
            clump.transform.localRotation = Quaternion.Euler(Random.Range(-10f, 10f), Random.Range(0f, 360f), Random.Range(-10f, 10f));

            Mesh m = EnvironmentModelFactory.CreateVolumetricFoliageMesh(seed, scale);
            m = SaveMeshAsset(m, $"Mesh_VolFoliage_{seed}");
            clump.AddComponent<MeshFilter>().sharedMesh = m;
            clump.AddComponent<MeshRenderer>().sharedMaterial = mat;
        }

        private static void CreateHangingVine(Transform parent, Vector3 localPos, Material vineMat)
        {
            GameObject vine = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            vine.name = "Vine";
            vine.transform.SetParent(parent, false);
            vine.transform.localPosition = localPos;
            vine.transform.localRotation = Quaternion.Euler(15f, 35f, -22f);
            vine.transform.localScale = new Vector3(0.06f, 0.85f, 0.06f);
            vine.GetComponent<Renderer>().sharedMaterial = vineMat;
            var col = vine.GetComponent<Collider>();
            if (col != null) Object.DestroyImmediate(col);
        }

        private static void CreateGroundFloraRing(Transform parent, ArtMaterials mats)
        {
            GameObject flora = new GameObject("FloraRing");
            flora.transform.SetParent(parent, false);
            flora.transform.localPosition = Vector3.zero;

            Mesh grassMesh = EnvironmentModelFactory.CreateGrassTuftMesh(0.35f, 0.38f);
            grassMesh = SaveMeshAsset(grassMesh, "Mesh_GrassTuft");

            for (int i = 0; i < 6; i++)
            {
                float angle = (float)i / 6 * Mathf.PI * 2f;
                float r = Random.Range(0.75f, 1.15f);
                Vector3 p = new Vector3(Mathf.Cos(angle) * r, 0.05f, Mathf.Sin(angle) * r);

                GameObject tuft = new GameObject($"GrassTuft_{i}");
                tuft.transform.SetParent(flora.transform, false);
                tuft.transform.localPosition = p;
                tuft.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                tuft.AddComponent<MeshFilter>().sharedMesh = grassMesh;
                tuft.AddComponent<MeshRenderer>().sharedMaterial = mats.Grass;
            }

            for (int i = 0; i < 4; i++)
            {
                float angle = (float)i / 4 * Mathf.PI * 2f + 0.4f;
                float r = Random.Range(0.6f, 0.95f);
                Vector3 p = new Vector3(Mathf.Cos(angle) * r, 0.12f, Mathf.Sin(angle) * r);

                GameObject fl = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                fl.name = $"Flower_{i}";
                fl.transform.SetParent(flora.transform, false);
                fl.transform.localPosition = p;
                fl.transform.localScale = new Vector3(0.12f, 0.12f, 0.12f);
                fl.GetComponent<Renderer>().sharedMaterial = (i % 2 == 0) ? mats.FlowerPink : mats.FlowerYellow;
                var col = fl.GetComponent<Collider>();
                if (col != null) Object.DestroyImmediate(col);
            }
        }

        private static void SavePrefab(GameObject go, string name)
        {
            string path = $"{PrefabsFolder}/{name}.prefab";
            PrefabUtility.SaveAsPrefabAsset(go, path);
        }

        #endregion
    }
}
