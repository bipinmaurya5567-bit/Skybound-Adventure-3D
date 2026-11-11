using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace SkyboundAdventure
{
    public class WorldBuilder : MonoBehaviour
    {
#if UNITY_EDITOR
        private const string MaterialsFolder = "Assets/Materials/Skybound";

        [MenuItem("Tools/Skybound Adventure/Build Complete Game World 6.0", false, 0)]
        public static void BuildWorldMenu()
        {
            BuildCompleteGame();
        }

        public static void BuildCompleteGame()
        {
            Undo.SetCurrentGroupName("Build Skybound Adventure World 6.0");
            int undoGroup = Undo.GetCurrentGroup();

            EnsureDirectories();
            var mats = CreateOrLoadMaterials();

            // Clear old scene elements
            CleanupOldElements();

            // Root Environment
            GameObject worldRoot = new GameObject("--- ENVIRONMENT ---");
            Transform envTransform = worldRoot.transform;

            // 5 Distinct, Progressive, Beautifully-Designed World Zones
            BuildZone1_EmeraldGarden(envTransform, mats);
            BuildZone2_SkyBridgesAndFerry(envTransform, mats);
            BuildZone3_ChallengeValley(envTransform, mats);
            BuildZone4_CrystalHeights(envTransform, mats);
            BuildZone5_SkyTempleSanctuary(envTransform, mats);

            // Rich Atmosphere, Layered Cloud Banks & Distant Fantasy Sky Realm
            BuildLayeredCloudBanks(envTransform, mats);
            BuildDistantFantasyRealm(envTransform, mats);

            // Managers & DeathZone
            GameObject managersRoot = new GameObject("--- MANAGERS ---");
            var audioMgr = managersRoot.AddComponent<AudioManager>();
            var gameMgr = managersRoot.AddComponent<GameManager>();
            var deathZone = managersRoot.AddComponent<DeathZone>();

            // Player Hero Character - spawn precisely on the clean grass plateau surface (Y = 0.70m)
            GameObject player = BuildHeroCharacter(new Vector3(0f, 0.70f, -3.5f), mats);

            // Fixed Third-Person Tracking Camera
            SetupThirdPersonCamera(player.transform);

            // Faculty-Ready Modern UI System
            GameObject uiRoot = BuildFacultyGameUI();
            var uiMgr = uiRoot.GetComponent<UIManager>();

            // Atmospheric Lighting, Skybox & Fog
            ConfigureAtmosphereAndLighting(mats);

            // Register Coins and Checkpoints
            gameMgr.RegisterCoins();
            gameMgr.RegisterCheckpoints();

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Undo.CollapseUndoOperations(undoGroup);

            Debug.Log("<color=green>★ Skybound Adventure 6.0 successfully built! Clean island geometry, optimized moving ferry, proportional framing trees, and rich sky atmosphere.</color>");
        }

        private static void EnsureDirectories()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Materials"))
            {
                AssetDatabase.CreateFolder("Assets", "Materials");
            }
            if (!AssetDatabase.IsValidFolder(MaterialsFolder))
            {
                AssetDatabase.CreateFolder("Assets/Materials", "Skybound");
            }
        }

        private static MaterialDictionary CreateOrLoadMaterials()
        {
            Shader litShader = Shader.Find("Universal Render Pipeline/Lit");
            if (litShader == null) litShader = Shader.Find("Standard");

            Shader skyShader = Shader.Find("Skybox/Procedural");

            var dict = new MaterialDictionary();
            dict.Grass = GetOrCreateMat("M_Grass", litShader, new Color(0.18f, 0.68f, 0.22f), 0f, 0.2f);
            dict.GrassDark = GetOrCreateMat("M_GrassDark", litShader, new Color(0.12f, 0.48f, 0.16f), 0f, 0.2f);
            dict.Rock = GetOrCreateMat("M_Rock", litShader, new Color(0.34f, 0.38f, 0.44f), 0f, 0.25f);
            dict.RockDark = GetOrCreateMat("M_RockDark", litShader, new Color(0.20f, 0.22f, 0.27f), 0f, 0.2f);
            dict.RockBrown = GetOrCreateMat("M_RockBrown", litShader, new Color(0.42f, 0.32f, 0.24f), 0f, 0.2f);
            dict.StonePath = GetOrCreateMat("M_StonePath", litShader, new Color(0.55f, 0.58f, 0.62f), 0f, 0.35f);
            dict.Wood = GetOrCreateMat("M_Wood", litShader, new Color(0.50f, 0.30f, 0.16f), 0f, 0.35f);
            dict.WoodDark = GetOrCreateMat("M_WoodDark", litShader, new Color(0.30f, 0.18f, 0.10f), 0f, 0.3f);
            dict.WoodBridge = GetOrCreateMat("M_WoodBridge", litShader, new Color(0.58f, 0.38f, 0.22f), 0f, 0.35f);
            dict.Soil = GetOrCreateMat("M_StylizedSoil", litShader, new Color(0.38f, 0.24f, 0.16f), 0f, 0.25f);
            dict.Gold = GetOrCreateMat("M_Gold", litShader, new Color(1.0f, 0.82f, 0.08f), 0.95f, 0.90f, new Color(0.4f, 0.3f, 0.05f));
            dict.GoldBody = GetOrCreateMat("M_Gold_CoinBody", litShader, new Color(1.0f, 0.84f, 0.12f), 0.96f, 0.92f, new Color(0.25f, 0.20f, 0.04f));
            dict.GoldRim = GetOrCreateMat("M_Gold_CoinRim", litShader, new Color(0.78f, 0.52f, 0.08f), 0.92f, 0.80f, new Color(0.16f, 0.10f, 0.02f));
            dict.GoldSymbol = GetOrCreateMat("M_Gold_DollarSymbol", litShader, new Color(1.0f, 0.95f, 0.45f), 0.98f, 0.96f, new Color(0.40f, 0.35f, 0.12f));
            dict.Cloud = GetOrCreateMat("M_Cloud", litShader, new Color(0.96f, 0.98f, 1.0f), 0f, 0.1f);
            dict.CanopyGreen = GetOrCreateMat("M_CanopyGreen", litShader, new Color(0.16f, 0.58f, 0.20f), 0f, 0.25f);
            dict.CanopyAutumn = GetOrCreateMat("M_CanopyAutumn", litShader, new Color(0.95f, 0.48f, 0.08f), 0f, 0.25f);
            dict.CanopyPink = GetOrCreateMat("M_CanopyPink", litShader, new Color(0.98f, 0.52f, 0.70f), 0f, 0.25f);
            dict.Trunk = GetOrCreateMat("M_Trunk", litShader, new Color(0.32f, 0.18f, 0.10f), 0f, 0.25f);
            dict.FlowerPink = GetOrCreateMat("M_FlowerPink", litShader, new Color(1.0f, 0.25f, 0.65f), 0f, 0.35f);
            dict.FlowerYellow = GetOrCreateMat("M_FlowerYellow", litShader, new Color(1.0f, 0.92f, 0.12f), 0f, 0.35f);
            dict.MushroomCap = GetOrCreateMat("M_MushroomCap", litShader, new Color(0.95f, 0.18f, 0.18f), 0.1f, 0.5f);
            dict.MushroomStem = GetOrCreateMat("M_MushroomStem", litShader, new Color(0.95f, 0.92f, 0.85f), 0f, 0.3f);
            dict.Portal = GetOrCreateMat("M_Portal", litShader, new Color(0.05f, 0.88f, 1.0f), 0.2f, 0.95f, new Color(0.15f, 0.9f, 1.0f));
            dict.RuneGlow = GetOrCreateMat("M_RuneGlow", litShader, new Color(1.0f, 0.80f, 0.15f), 0.15f, 0.6f, new Color(0.35f, 0.25f, 0.05f));
            Shader particleShader = Shader.Find("Universal Render Pipeline/Particles/Unlit") ?? Shader.Find("Particles/Standard Unlit");
            dict.ParticleSparkle = GetOrCreateMat("M_CoinSparkleParticle", particleShader, new Color(1.0f, 0.88f, 0.25f, 0.75f));
            dict.PlayerSuit = GetOrCreateMat("M_PlayerSuit", litShader, new Color(0.10f, 0.45f, 0.92f), 0.1f, 0.4f);
            dict.PlayerVisor = GetOrCreateMat("M_PlayerVisor", litShader, new Color(1.0f, 0.88f, 0.12f), 0.8f, 0.95f, new Color(0.9f, 0.75f, 0.1f));
            dict.PlayerWhite = GetOrCreateMat("M_PlayerWhite", litShader, new Color(0.96f, 0.96f, 0.96f), 0.1f, 0.5f);

            dict.CrystalBlue = GetOrCreateMat("M_CrystalBlue", litShader, new Color(0.12f, 0.75f, 1.0f), 0.15f, 0.95f, new Color(0.15f, 0.85f, 1.0f));
            dict.CrystalPurple = GetOrCreateMat("M_CrystalPurple", litShader, new Color(0.75f, 0.22f, 1.0f), 0.15f, 0.95f, new Color(0.70f, 0.18f, 0.95f));
            dict.Water = GetOrCreateMat("M_Water", litShader, new Color(0.20f, 0.85f, 0.95f), 0.10f, 0.98f, new Color(0.05f, 0.30f, 0.40f));
            dict.ObstacleSweeper = GetOrCreateMat("M_ObstacleSweeper", litShader, new Color(0.98f, 0.45f, 0.08f), 0.60f, 0.75f, new Color(0.85f, 0.30f, 0.05f));
            dict.DisappearingWarn = GetOrCreateMat("M_DisappearingWarn", litShader, new Color(1.0f, 0.55f, 0.12f), 0.10f, 0.80f, new Color(0.95f, 0.40f, 0.05f));
            dict.BouncePad = GetOrCreateMat("M_BouncePad", litShader, new Color(0.15f, 0.90f, 0.45f), 0.20f, 0.85f, new Color(0.20f, 0.85f, 0.35f));
            dict.MarbleWhite = GetOrCreateMat("M_MarbleWhite", litShader, new Color(0.94f, 0.95f, 0.98f), 0.05f, 0.85f);
            dict.LandingPad = GetOrCreateMat("M_LandingPad", litShader, new Color(0.25f, 0.75f, 0.95f), 0.30f, 0.90f, new Color(0.15f, 0.60f, 0.85f));

            // Rich azure fantasy skybox
            if (skyShader != null)
            {
                dict.Skybox = GetOrCreateMat("M_Skybox_Fantasy", skyShader, Color.white);
                dict.Skybox.SetColor("_SkyTint", new Color(0.20f, 0.52f, 0.95f));
                dict.Skybox.SetColor("_GroundColor", new Color(0.72f, 0.84f, 0.98f));
                dict.Skybox.SetFloat("_AtmosphereThickness", 0.95f);
                dict.Skybox.SetFloat("_Exposure", 1.05f);
                dict.Skybox.SetFloat("_SunSize", 0.04f);
            }

            return dict;
        }

        private static Material GetOrCreateMat(string name, Shader shader, Color color, float metallic = 0f, float smoothness = 0.5f, Color? emission = null)
        {
            string path = $"{MaterialsFolder}/{name}.mat";
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                mat = new Material(shader);
                mat.name = name;
                AssetDatabase.CreateAsset(mat, path);
            }

            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);
            if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);

            if (emission.HasValue && mat.HasProperty("_EmissionColor"))
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", emission.Value);
            }

            mat.enableInstancing = true;
            EditorUtility.SetDirty(mat);
            return mat;
        }

        private static void CleanupOldElements()
        {
            string[] toDelete = new string[]
            {
                "--- ENVIRONMENT ---",
                "--- MANAGERS ---",
                "Player",
                "Canvas_UI",
                "EventSystem",
                "PlayerSpawnPoint",
                "SharedCoinSparkleEmitter"
            };

            foreach (var name in toDelete)
            {
                GameObject go = GameObject.Find(name);
                if (go != null)
                {
                    Undo.DestroyObjectImmediate(go);
                }
            }
        }

        #region Natural Island Constructors (Clean Grass & Sculpted Rock Underside)

        private static GameObject CreateSculptedIsland(string name, Transform parent, Vector3 pos, Vector3 size, MaterialDictionary mats)
        {
            GameObject island = new GameObject(name);
            island.transform.SetParent(parent, false);
            island.transform.localPosition = pos;

            float w = size.x;
            float h = size.y;
            float d = size.z;

            // 1. Clean, seamless grass plateau with flat BoxCollider (PRESERVES EXACT GAMEPLAY SURFACE & COLLIDER)
            CreateBox("GrassPlateau", island.transform, Vector3.zero, new Vector3(w, h, d), mats.Grass, true);

            // 2. Bevelled Grass Fringe Overhang Rim (Softens hard box edge into natural terrain)
            CreateBox("GrassFringe_Top", island.transform, new Vector3(0f, -0.06f, 0f), new Vector3(w + 0.22f, h * 0.28f, d + 0.22f), mats.GrassDark, false);

            // 3. Earthy Soil Sub-layer
            CreateBox("SoilStrata", island.transform, new Vector3(0f, -h * 0.40f, 0f), new Vector3(w * 0.96f, h * 0.42f, d * 0.96f), mats.Soil, false);

            // 4. Faceted Bedrock Strata (Layered cliff formation)
            CreateBox("Bedrock_Tier1", island.transform, new Vector3(0f, -h * 0.95f, 0f), new Vector3(w * 0.88f, h * 0.95f, d * 0.88f), mats.Rock, false);
            CreateBox("Bedrock_Tier2", island.transform, new Vector3(0.15f, -h * 1.85f, -0.1f), new Vector3(w * 0.68f, h * 1.1f, d * 0.68f), mats.RockDark, false);
            CreateBox("Bedrock_Tier3", island.transform, new Vector3(-0.1f, -h * 2.75f, 0.15f), new Vector3(w * 0.46f, h * 1.2f, d * 0.46f), mats.RockBrown, false);
            CreateBox("StalactiteSpire", island.transform, new Vector3(0f, -h * 3.8f, 0f), new Vector3(w * 0.24f, h * 1.4f, d * 0.24f), mats.RockDark, false);

            return island;
        }

        private static GameObject CreateSteppingIsland(string name, Transform parent, Vector3 pos, float width, float depth, float height, MaterialDictionary mats)
        {
            GameObject island = new GameObject(name);
            island.transform.SetParent(parent, false);
            island.transform.localPosition = pos;

            // 1. Clean, stable stepping platform with solid flat BoxCollider (PRESERVES EXACT GAMEPLAY COLLIDER)
            CreateBox("SteppingSurface", island.transform, Vector3.zero, new Vector3(width, height, depth), mats.Grass, true);

            // 2. Embedded stone landing paver on top
            CreateBox("StonePad", island.transform, new Vector3(0f, height * 0.51f, 0f), new Vector3(width * 0.70f, 0.04f, depth * 0.70f), mats.StonePath, false);

            // 3. Bevelled grass fringe
            CreateBox("GrassFringe", island.transform, new Vector3(0f, -0.04f, 0f), new Vector3(width + 0.14f, height * 0.25f, depth + 0.14f), mats.GrassDark, false);

            // 4. Soil stratum
            CreateBox("SoilBase", island.transform, new Vector3(0f, -height * 0.35f, 0f), new Vector3(width * 0.94f, height * 0.45f, depth * 0.94f), mats.Soil, false);

            // 5. Natural, tapering rocky underside
            CreateBox("RockBase_1", island.transform, new Vector3(0f, -height * 0.85f, 0f), new Vector3(width * 0.84f, height * 0.9f, depth * 0.84f), mats.Rock, false);
            CreateBox("RockBase_2", island.transform, new Vector3(0.06f, -height * 1.65f, -0.05f), new Vector3(width * 0.60f, height * 1.0f, depth * 0.60f), mats.RockDark, false);
            CreateBox("RockRoot", island.transform, new Vector3(0f, -height * 2.45f, 0f), new Vector3(width * 0.32f, height * 1.1f, depth * 0.32f), mats.RockBrown, false);

            return island;
        }

        #endregion

        #region Zone 1 — Emerald Garden (Z: 0 - 26m)

        private static void BuildZone1_EmeraldGarden(Transform parent, MaterialDictionary mats)
        {
            GameObject zone = new GameObject("Zone1_EmeraldGarden");
            zone.transform.SetParent(parent, false);

            // 1. Starting Haven Island
            GameObject island = CreateSculptedIsland("StartingHaven", zone.transform, Vector3.zero, new Vector3(14.0f, 1.4f, 12.0f), mats);

            // Stone Paved Pathway guiding forward
            CreateStonePath(island.transform, new Vector3(0f, 0.71f, -2.8f), new Vector3(0f, 0.71f, 4.5f), mats.StonePath);

            // 3 Distinct Stylized 3D Trees placed on outer flanks to frame the route (NO camera obstruction!)
            CreateStylizedTree("HavenTree_Lush", island.transform, new Vector3(-5.5f, 0.70f, 2.5f), mats, TreeType.LargeLush);
            CreateStylizedTree("HavenTree_Palm", island.transform, new Vector3(5.5f, 0.70f, 1.5f), mats, TreeType.TropicalPalm);
            CreateStylizedTree("HavenTree_Small", island.transform, new Vector3(-5.2f, 0.70f, -2.8f), mats, TreeType.DecorativeSmall);

            // 3 Distinct Stylized 3D Rocks on flanks (Boulders, Medium Stone, Small Clusters)
            CreateStylizedRock("HavenRock_Boulder", island.transform, new Vector3(-5.6f, 0.65f, -0.6f), RockType.LargeBoulder);
            CreateStylizedRock("HavenRock_Stone", island.transform, new Vector3(5.5f, 0.65f, -1.5f), RockType.MediumStone);
            CreateStylizedRock("HavenRock_Cluster", island.transform, new Vector3(4.8f, 0.68f, 1.2f), RockType.SmallCluster);

            // Low-Profile Bushes & Floral Details
            CreateStylizedBush("Bush_1", island.transform, new Vector3(-3.8f, 0.75f, 3.5f), 0.9f, mats.CanopyGreen);
            CreateStylizedBush("Bush_2", island.transform, new Vector3(3.8f, 0.75f, 3.2f), 0.85f, mats.CanopyAutumn);
            CreateFlowerPatch(island.transform, new Vector3(-2.2f, 0.75f, 2.0f), mats);
            CreateFlowerPatch(island.transform, new Vector3(2.5f, 0.75f, -2.2f), mats);
            CreateMushroomCluster(island.transform, new Vector3(-3.4f, 0.75f, 1.2f), mats);

            // Stylized Waterfall cascading off the left cliff edge into clouds
            BuildWaterfallFeature(island.transform, new Vector3(-6.5f, 0.72f, 0.8f), mats);

            // Stepping Isles leading smoothly toward Zone 2 (Comfortable, predictable 2.2m jump spacing)
            Vector3 pos1 = new Vector3(1.4f, 0.2f, 10.5f);
            GameObject isle1 = CreateSteppingIsland("SteppingIsle_1", zone.transform, pos1, 4.2f, 4.0f, 0.8f, mats);
            CreateFlowerPatch(isle1.transform, new Vector3(0.5f, 0.45f, 0.5f), mats);

            Vector3 pos2 = new Vector3(-1.2f, 0.4f, 16.8f);
            GameObject isle2 = CreateSteppingIsland("SteppingIsle_2", zone.transform, pos2, 4.2f, 4.0f, 0.8f, mats);
            CreateStylizedRock("MiniRock", isle2.transform, new Vector3(-0.8f, 0.45f, -0.4f), RockType.MediumStone);

            Vector3 pos3 = new Vector3(0.2f, 0.6f, 23.0f);
            GameObject outlook = CreateSculptedIsland("GardenOutlook", zone.transform, pos3, new Vector3(8.0f, 1.2f, 6.5f), mats);
            CreateStylizedTree("OutlookTree", outlook.transform, new Vector3(-3.2f, 0.65f, 1.2f), mats, TreeType.TallForest);
            CreateStylizedRock("OutlookRock", outlook.transform, new Vector3(3.2f, 0.65f, -0.8f), RockType.MediumStone);
            CreateStylizedRock("OutlookCluster", outlook.transform, new Vector3(2.8f, 0.65f, 1.2f), RockType.SmallCluster);

            // Collectible Coins 1 - 4
            CreateCollectibleCoin("Coin_01", island.transform, new Vector3(0f, 1.9f, -0.5f), mats.GoldBody, mats.GoldRim, mats.GoldSymbol, mats.ParticleSparkle);
            CreateCollectibleCoin("Coin_02", island.transform, new Vector3(0f, 1.9f, 4.0f), mats.GoldBody, mats.GoldRim, mats.GoldSymbol, mats.ParticleSparkle);
            CreateCollectibleCoin("Coin_03", zone.transform, pos1 + Vector3.up * 1.6f, mats.GoldBody, mats.GoldRim, mats.GoldSymbol, mats.ParticleSparkle);
            CreateCollectibleCoin("Coin_04", zone.transform, pos2 + Vector3.up * 1.8f, mats.GoldBody, mats.GoldRim, mats.GoldSymbol, mats.ParticleSparkle);
        }

        private static void BuildWaterfallFeature(Transform parent, Vector3 localPos, MaterialDictionary mats)
        {
            GameObject wf = new GameObject("StylizedWaterfall");
            wf.transform.SetParent(parent, false);
            wf.transform.localPosition = localPos;

            // Pool on plateau surface
            CreateBox("WaterPool", wf.transform, Vector3.zero, new Vector3(2.2f, 0.08f, 2.4f), mats.Water);

            // Cascading water tiers descending into the sky
            CreateBox("Cascade_Tier1", wf.transform, new Vector3(-0.9f, -0.8f, 0f), new Vector3(0.8f, 1.6f, 2.2f), mats.Water);
            CreateBox("Cascade_Tier2", wf.transform, new Vector3(-1.6f, -2.6f, 0f), new Vector3(0.8f, 2.2f, 2.4f), mats.Water);
            CreateBox("Cascade_Tier3", wf.transform, new Vector3(-2.4f, -4.8f, 0f), new Vector3(0.8f, 2.5f, 2.6f), mats.Water);

            // Foam & Mist billows at cascade bottom
            CreateSphere("MistPuff_1", wf.transform, new Vector3(-2.6f, -6.0f, -0.6f), new Vector3(2.4f, 1.6f, 2.2f), mats.Cloud);
            CreateSphere("MistPuff_2", wf.transform, new Vector3(-2.7f, -6.2f, 0.6f), new Vector3(2.6f, 1.7f, 2.4f), mats.Cloud);
        }

        #endregion

        #region Zone 2 — Sky Bridges & The Great Ferry (Z: 28 - 72m)

        private static void BuildZone2_SkyBridgesAndFerry(Transform parent, MaterialDictionary mats)
        {
            GameObject zone = new GameObject("Zone2_SkyBridgesAndFerry");
            zone.transform.SetParent(parent, false);

            // 1. Bridgehead Isle
            Vector3 bhPos = new Vector3(0f, 0.8f, 30.5f);
            GameObject bridgehead = CreateSculptedIsland("BridgeheadIsle", zone.transform, bhPos, new Vector3(8.5f, 1.2f, 6.5f), mats);
            CreateStylizedTree("BridgeTree", bridgehead.transform, new Vector3(-3.2f, 0.65f, -1.2f), mats, TreeType.TropicalPalm);
            CreateStylizedRock("BridgeRock", bridgehead.transform, new Vector3(3.2f, 0.65f, -1.2f), RockType.MediumStone);

            // 2. Wooden Suspension Bridge (Z = 34.5 to 45.5, span 11m, solid walkable planks)
            BuildWoodenSuspensionBridge(zone.transform, new Vector3(0f, 0.9f, 34.5f), new Vector3(0f, 1.1f, 45.5f), mats);

            // 3. Mid-Bridge Haven Island (Ferry Launch Station)
            Vector3 midPos = new Vector3(0f, 1.1f, 50.5f);
            GameObject midHaven = CreateSculptedIsland("MidBridgeHaven", zone.transform, midPos, new Vector3(9.0f, 1.2f, 7.0f), mats);
            CreateStylizedTree("MidTree", midHaven.transform, new Vector3(-3.5f, 0.65f, 1.2f), mats, TreeType.AutumnOak);
            CreateStylizedRock("MidBoulder", midHaven.transform, new Vector3(3.4f, 0.7f, -1.5f), RockType.LargeBoulder);
            CreateStylizedRock("MidCluster", midHaven.transform, new Vector3(3.0f, 0.68f, 1.2f), RockType.SmallCluster);

            // Ferry Launch Pier Marker
            CreateBox("FerryPierPad", midHaven.transform, new Vector3(0f, 0.62f, 2.8f), new Vector3(4.2f, 0.05f, 1.4f), mats.WoodBridge);

            // 4. THE GREAT SKY FERRY (Redesigned Moving Platform: Large 5.6m x 5.2m deck with Dock Pauses)
            Vector3 ferryStart = new Vector3(0f, 1.4f, 56.5f);
            BuildTheGreatSkyFerry(zone.transform, ferryStart, new Vector3(0f, 0f, 5.5f), 3.2f, 0.9f, mats);

            // 5. Dedicated Arrival Dock & Landing Island (Comfortable, predictable landing)
            Vector3 landPos = new Vector3(0f, 1.6f, 68.0f);
            GameObject landIsle = CreateSculptedIsland("FerryLandingIsle", zone.transform, landPos, new Vector3(9.0f, 1.2f, 7.5f), mats);
            CreateStylizedTree("LandingPalm", landIsle.transform, new Vector3(3.4f, 0.65f, -1.8f), mats, TreeType.TropicalPalm);
            CreateStylizedRock("LandingRock", landIsle.transform, new Vector3(-3.4f, 0.65f, -1.8f), RockType.MediumStone);

            // Visual Landing Target Medallion (Clear visual target where the player lands from the ferry!)
            GameObject landingPad = CreateCylinder("LandingMedallion", landIsle.transform, new Vector3(0f, 0.62f, -2.2f), new Vector3(3.4f, 0.06f, 3.4f), mats.LandingPad);
            CreateCylinder("LandingInnerRing", landingPad.transform, new Vector3(0f, 0.06f, 0f), new Vector3(2.2f, 0.08f, 2.2f), mats.RuneGlow);

            // 6. Bounce Pad 1 (Obstacle 2: Launches player high onto Zone 3 Entrance)
            CreateBouncePad("BouncePad_Zone2", landIsle.transform, new Vector3(0f, 0.65f, 2.0f), 6.5f, mats);

            // Collectible Coins 5 - 8
            CreateCollectibleCoin("Coin_05", zone.transform, new Vector3(0f, 2.0f, 40.0f), mats.GoldBody, mats.GoldRim, mats.GoldSymbol, mats.ParticleSparkle); // On suspension bridge
            CreateCollectibleCoin("Coin_06", midHaven.transform, new Vector3(0f, 1.8f, 0f), mats.GoldBody, mats.GoldRim, mats.GoldSymbol, mats.ParticleSparkle);
            CreateCollectibleCoin("Coin_07", zone.transform, ferryStart + new Vector3(0f, 1.8f, 2.75f), mats.GoldBody, mats.GoldRim, mats.GoldSymbol, mats.ParticleSparkle); // Mid-ferry transit coin
            CreateCollectibleCoin("Coin_08", landIsle.transform, new Vector3(0f, 4.8f, 2.0f), mats.GoldBody, mats.GoldRim, mats.GoldSymbol, mats.ParticleSparkle); // High in air over bounce pad
        }

        private static void BuildWoodenSuspensionBridge(Transform parent, Vector3 start, Vector3 end, MaterialDictionary mats)
        {
            GameObject bridge = new GameObject("WoodenSuspensionBridge");
            bridge.transform.SetParent(parent, false);

            int plankCount = 10;
            for (int i = 0; i <= plankCount; i++)
            {
                float t = (float)i / plankCount;
                Vector3 pos = Vector3.Lerp(start, end, t);

                // Subtle catenary sag
                float sag = Mathf.Sin(t * Mathf.PI) * 0.28f;
                pos.y -= sag;

                // Wide individual wooden bridge plank with solid box collider
                GameObject plank = CreateBox($"BridgePlank_{i}", bridge.transform, pos, new Vector3(3.2f, 0.18f, 0.85f), mats.WoodBridge, true);
                plank.transform.localRotation = Quaternion.Euler(Random.Range(-1.5f, 1.5f), 0f, 0f);

                // Side rope handrail posts
                if (i % 2 == 0)
                {
                    CreateCylinder($"Post_L_{i}", bridge.transform, pos + new Vector3(-1.5f, 0.5f, 0f), new Vector3(0.12f, 1.0f, 0.12f), mats.WoodDark, false);
                    CreateCylinder($"Post_R_{i}", bridge.transform, pos + new Vector3(1.5f, 0.5f, 0f), new Vector3(0.12f, 1.0f, 0.12f), mats.WoodDark, false);
                }
            }
        }

        private static GameObject BuildTheGreatSkyFerry(Transform parent, Vector3 startPos, Vector3 delta, float travelTime, float pauseTime, MaterialDictionary mats)
        {
            GameObject ferry = new GameObject("TheGreatSkyFerry");
            ferry.transform.SetParent(parent, false);
            ferry.transform.position = startPos;

            // Spacious 5.6m x 5.2m wooden deck
            Vector3 size = new Vector3(5.6f, 0.6f, 5.2f);
            CreateBox("FerryDeck", ferry.transform, Vector3.zero, size, mats.Wood, true);

            // Inlaid wooden plank deck
            CreateBox("FerryPlankSurface", ferry.transform, new Vector3(0f, 0.05f, 0f), new Vector3(5.0f, 0.55f, 4.6f), mats.WoodBridge);

            // Central rune landing circle
            CreateCylinder("CenterRuneRing", ferry.transform, new Vector3(0f, 0.06f, 0f), new Vector3(2.6f, 0.1f, 2.6f), mats.RuneGlow);

            // Side protective bumpers
            CreateBox("Bumper_L", ferry.transform, new Vector3(-2.65f, 0.22f, 0f), new Vector3(0.25f, 0.35f, 5.0f), mats.WoodDark);
            CreateBox("Bumper_R", ferry.transform, new Vector3(2.65f, 0.22f, 0f), new Vector3(0.25f, 0.35f, 5.0f), mats.WoodDark);

            // 4 Corner lantern posts
            Vector3[] cornerOffsets = new Vector3[]
            {
                new Vector3(-2.4f, 0.45f, -2.2f),
                new Vector3(2.4f, 0.45f, -2.2f),
                new Vector3(-2.4f, 0.45f, 2.2f),
                new Vector3(2.4f, 0.45f, 2.2f)
            };

            for (int i = 0; i < cornerOffsets.Length; i++)
            {
                CreateCylinder($"FerryPost_{i}", ferry.transform, cornerOffsets[i], new Vector3(0.18f, 0.9f, 0.18f), mats.WoodDark);
                CreateSphere($"FerryLantern_{i}", ferry.transform, cornerOffsets[i] + Vector3.up * 0.55f, new Vector3(0.35f, 0.35f, 0.35f), mats.RuneGlow);
            }

            // Configure moving platform with dock pauses
            var mp = ferry.AddComponent<MovingPlatform>();
            mp.Configure(delta, travelTime, pauseTime);

            // Generous trigger volume (height 4.0m) to ensure jumping player remains attached!
            BoxCollider trig = ferry.AddComponent<BoxCollider>();
            trig.isTrigger = true;
            trig.center = new Vector3(0f, 2.0f, 0f);
            trig.size = new Vector3(size.x * 0.98f, 4.0f, size.z * 0.98f);

            return ferry;
        }

        #endregion

        #region Zone 3 — Challenge Valley (Z: 74 - 124m)

        private static void BuildZone3_ChallengeValley(Transform parent, MaterialDictionary mats)
        {
            GameObject zone = new GameObject("Zone3_ChallengeValley");
            zone.transform.SetParent(parent, false);

            // 1. Arrival Plateau with Checkpoint 1
            Vector3 arrPos = new Vector3(0f, 3.5f, 78.0f);
            GameObject arrival = CreateSculptedIsland("ArrivalHaven", zone.transform, arrPos, new Vector3(10.5f, 1.4f, 8.5f), mats);
            CreateCheckpointBeacon("Checkpoint_Valley", arrival.transform, new Vector3(0f, 0.75f, -2.6f), mats);
            CreateStylizedTree("ArrivalTree", arrival.transform, new Vector3(-4.0f, 0.70f, 1.5f), mats, TreeType.TallForest);
            CreateStylizedRock("ArrivalBoulder", arrival.transform, new Vector3(4.0f, 0.70f, 1.5f), RockType.LargeBoulder);
            CreateStylizedRock("ArrivalCluster", arrival.transform, new Vector3(-3.2f, 0.70f, -1.5f), RockType.SmallCluster);
            CreateFlowerPatch(arrival.transform, new Vector3(-2.8f, 0.75f, 1.5f), mats);

            // 2. Sweeper Arena Island (Obstacle 3: Rotating Sweeper Hazard)
            Vector3 arenaPos = new Vector3(0f, 3.6f, 90.0f);
            GameObject arena = CreateSculptedIsland("SweeperArena", zone.transform, arenaPos, new Vector3(12.0f, 1.2f, 12.0f), mats);
            CreateRotatingSweeper("SweeperHazard", arena.transform, new Vector3(0f, 0.65f, 0f), 4.6f, 42f, true, mats);

            // 3. Disappearing / Crumbling Stepping Tiles (Obstacles 4 & 5: Generous 3.8m x 3.8m landing tiles)
            Vector3 disPos1 = new Vector3(0f, 3.8f, 100.0f);
            CreateDisappearingPlatform("DisappearingIsle_1", zone.transform, disPos1, new Vector3(3.8f, 0.5f, 3.8f), mats);

            Vector3 disPos2 = new Vector3(0f, 4.0f, 106.5f);
            CreateDisappearingPlatform("DisappearingIsle_2", zone.transform, disPos2, new Vector3(3.8f, 0.5f, 3.8f), mats);

            // 4. Ridge Rest Haven
            Vector3 ridgePos = new Vector3(0f, 4.2f, 113.5f);
            GameObject ridge = CreateSculptedIsland("ValleyRidgeHaven", zone.transform, ridgePos, new Vector3(8.0f, 1.2f, 6.5f), mats);
            CreateStylizedTree("RidgeTree", ridge.transform, new Vector3(-3.0f, 0.65f, 1.0f), mats, TreeType.LargeLush);
            CreateStylizedRock("RidgeRock", ridge.transform, new Vector3(3.0f, 0.65f, 1.0f), RockType.MediumStone);

            // 5. Forward Moving Glider (Obstacle 6: Longitudinal Glider)
            Vector3 gliderStart = new Vector3(0f, 4.6f, 120.5f);
            CreateMovingPlatform("GliderFerry", zone.transform, gliderStart, new Vector3(0f, 0f, 4.8f), 3.0f, 0.8f, new Vector3(4.8f, 0.5f, 4.5f), mats);

            // Collectible Coins 9 - 13
            CreateCollectibleCoin("Coin_09", arrival.transform, new Vector3(0f, 1.8f, 1.5f), mats.GoldBody, mats.GoldRim, mats.GoldSymbol, mats.ParticleSparkle);
            CreateCollectibleCoin("Coin_10", arena.transform, new Vector3(-3.8f, 1.8f, 0f), mats.GoldBody, mats.GoldRim, mats.GoldSymbol, mats.ParticleSparkle); // Sweeper arena flank
            CreateCollectibleCoin("Coin_11", arena.transform, new Vector3(3.8f, 1.8f, 0f), mats.GoldBody, mats.GoldRim, mats.GoldSymbol, mats.ParticleSparkle);  // Sweeper arena flank
            CreateCollectibleCoin("Coin_12", zone.transform, disPos1 + Vector3.up * 1.5f, mats.GoldBody, mats.GoldRim, mats.GoldSymbol, mats.ParticleSparkle);
            CreateCollectibleCoin("Coin_13", zone.transform, disPos2 + Vector3.up * 1.5f, mats.GoldBody, mats.GoldRim, mats.GoldSymbol, mats.ParticleSparkle);
        }

        #endregion

        #region Zone 4 — Crystal Heights (Z: 126 - 172m)

        private static void BuildZone4_CrystalHeights(Transform parent, MaterialDictionary mats)
        {
            GameObject zone = new GameObject("Zone4_CrystalHeights");
            zone.transform.SetParent(parent, false);

            // 1. Crystal Haven Island with Checkpoint 2
            Vector3 havenPos = new Vector3(0f, 5.0f, 130.0f);
            GameObject haven = CreateSculptedIsland("CrystalHaven", zone.transform, havenPos, new Vector3(10.0f, 1.4f, 9.0f), mats);
            CreateCheckpointBeacon("Checkpoint_Crystal", haven.transform, new Vector3(0f, 0.75f, -2.8f), mats);
            CreateStylizedTree("CrystalTree_Fantasy", haven.transform, new Vector3(-3.8f, 0.75f, -1.2f), mats, TreeType.FantasySky);
            CreateStylizedRock("CrystalBoulder", haven.transform, new Vector3(3.8f, 0.75f, -1.2f), RockType.LargeBoulder);

            // Glowing Crystal Spire Formations flanking the entrance
            BuildCrystalSpireGroup(haven.transform, new Vector3(-3.5f, 0.75f, 1.5f), mats);
            BuildCrystalSpireGroup(haven.transform, new Vector3(3.5f, 0.75f, 1.5f), mats);

            // 2. Floating Crystal Stepping Stones (Faceted natural stepping rocks with embedded crystals)
            Vector3 stone1 = new Vector3(-2.2f, 5.3f, 138.5f);
            GameObject cs1 = CreateSteppingIsland("CrystalStone_1", zone.transform, stone1, 3.8f, 3.6f, 0.7f, mats);
            BuildCrystalCluster(cs1.transform, new Vector3(0.6f, 0.4f, 0.4f), mats);

            Vector3 stone2 = new Vector3(2.2f, 5.7f, 144.5f);
            GameObject cs2 = CreateSteppingIsland("CrystalStone_2", zone.transform, stone2, 3.8f, 3.6f, 0.7f, mats);
            BuildCrystalCluster(cs2.transform, new Vector3(-0.6f, 0.4f, -0.4f), mats);

            Vector3 stone3 = new Vector3(0.0f, 6.1f, 150.5f);
            GameObject cs3 = CreateSteppingIsland("CrystalStone_3", zone.transform, stone3, 4.0f, 3.8f, 0.7f, mats);
            BuildCrystalCluster(cs3.transform, new Vector3(0.8f, 0.4f, 0.0f), mats);

            // 3. Vertical Moving Crystal Lift (Obstacle 7: Ascending Vertical Lift with Dock Pause)
            Vector3 liftStart = new Vector3(0f, 6.5f, 158.0f);
            CreateMovingPlatform("CrystalLift", zone.transform, liftStart, new Vector3(0f, 4.0f, 0f), 3.5f, 0.9f, new Vector3(4.8f, 0.5f, 4.8f), mats, mats.CrystalBlue);

            // 4. Crystal Upper Ledge
            Vector3 ledgePos = new Vector3(0f, 10.6f, 166.0f);
            GameObject ledge = CreateSculptedIsland("CrystalUpperLedge", zone.transform, ledgePos, new Vector3(8.5f, 1.2f, 7.0f), mats);
            BuildCrystalSpireGroup(ledge.transform, new Vector3(-3.0f, 0.65f, 1.2f), mats);
            BuildCrystalSpireGroup(ledge.transform, new Vector3(3.0f, 0.65f, 1.2f), mats);
            CreateStylizedTree("LedgeTree_Fantasy", ledge.transform, new Vector3(3.2f, 0.65f, 1.2f), mats, TreeType.FantasySky);
            CreateStylizedRock("LedgeRock", ledge.transform, new Vector3(-3.2f, 0.65f, -1.2f), RockType.MediumStone);

            // 5. Bounce Pad 2 (Celestial Super Launch into Sky Temple)
            CreateBouncePad("BouncePad_SuperLaunch", ledge.transform, new Vector3(0f, 0.65f, 1.8f), 7.6f, mats);

            // Collectible Coins 14 - 18
            CreateCollectibleCoin("Coin_14", haven.transform, new Vector3(0f, 2.0f, 1.5f), mats.GoldBody, mats.GoldRim, mats.GoldSymbol, mats.ParticleSparkle);
            CreateCollectibleCoin("Coin_15", zone.transform, stone1 + Vector3.up * 1.6f, mats.GoldBody, mats.GoldRim, mats.GoldSymbol, mats.ParticleSparkle);
            CreateCollectibleCoin("Coin_16", zone.transform, stone2 + Vector3.up * 1.6f, mats.GoldBody, mats.GoldRim, mats.GoldSymbol, mats.ParticleSparkle);
            CreateCollectibleCoin("Coin_17", zone.transform, liftStart + Vector3.up * 2.5f, mats.GoldBody, mats.GoldRim, mats.GoldSymbol, mats.ParticleSparkle);
            CreateCollectibleCoin("Coin_18", ledge.transform, new Vector3(0f, 5.2f, 1.8f), mats.GoldBody, mats.GoldRim, mats.GoldSymbol, mats.ParticleSparkle); // High in air over super launch
        }

        private static void BuildCrystalSpireGroup(Transform parent, Vector3 localPos, MaterialDictionary mats)
        {
            GameObject group = new GameObject("CrystalSpires");
            group.transform.SetParent(parent, false);
            group.transform.localPosition = localPos;

            // Main central crystal spire
            GameObject c1 = CreateBox("Spire_Main", group.transform, new Vector3(0f, 1.5f, 0f), new Vector3(0.55f, 3.0f, 0.55f), mats.CrystalBlue);
            c1.transform.localRotation = Quaternion.Euler(15f, 35f, -10f);

            // Flanking secondary crystal
            GameObject c2 = CreateBox("Spire_Sub1", group.transform, new Vector3(0.45f, 1.0f, 0.35f), new Vector3(0.45f, 2.0f, 0.45f), mats.CrystalPurple);
            c2.transform.localRotation = Quaternion.Euler(-20f, 60f, 15f);

            // Small crystal shard
            GameObject c3 = CreateBox("Spire_Sub2", group.transform, new Vector3(-0.4f, 0.65f, -0.3f), new Vector3(0.35f, 1.3f, 0.35f), mats.CrystalBlue);
            c3.transform.localRotation = Quaternion.Euler(25f, -40f, -18f);
        }

        private static void BuildCrystalCluster(Transform parent, Vector3 localPos, MaterialDictionary mats)
        {
            GameObject cluster = new GameObject("CrystalCluster");
            cluster.transform.SetParent(parent, false);
            cluster.transform.localPosition = localPos;

            GameObject c1 = CreateBox("Shard1", cluster.transform, new Vector3(0f, 0.45f, 0f), new Vector3(0.35f, 0.9f, 0.35f), mats.CrystalPurple);
            c1.transform.localRotation = Quaternion.Euler(18f, 45f, 12f);

            GameObject c2 = CreateBox("Shard2", cluster.transform, new Vector3(0.25f, 0.35f, -0.2f), new Vector3(0.28f, 0.7f, 0.28f), mats.CrystalBlue);
            c2.transform.localRotation = Quaternion.Euler(-15f, 20f, -25f);
        }

        #endregion

        #region Zone 5 — The Sky Temple Sanctuary (Z: 176 - 218m)

        private static void BuildZone5_SkyTempleSanctuary(Transform parent, MaterialDictionary mats)
        {
            GameObject zone = new GameObject("Zone5_SkyTempleSanctuary");
            zone.transform.SetParent(parent, false);

            // Grand Floating Temple Island (18x18m, elevated to Y = 13.0m)
            Vector3 templePos = new Vector3(0f, 13.0f, 194.0f);
            GameObject temple = CreateSculptedIsland("SkyTempleIsland", zone.transform, templePos, new Vector3(18.0f, 1.8f, 18.0f), mats);

            // Grand Marble Staircase leading up from Z = 176m to Temple Plaza surface at Z = 184m
            BuildGrandTempleStaircase(zone.transform, new Vector3(0f, 11.5f, 176.0f), new Vector3(0f, 13.9f, 184.5f), mats);

            // Ornate Temple Plaza Border (Marble & Gold)
            CreateBox("PlazaBorder_N", temple.transform, new Vector3(0f, 0.95f, 8.2f), new Vector3(17f, 0.35f, 0.7f), mats.MarbleWhite);
            CreateBox("PlazaBorder_S", temple.transform, new Vector3(0f, 0.95f, -8.2f), new Vector3(17f, 0.35f, 0.7f), mats.MarbleWhite);
            CreateBox("PlazaBorder_W", temple.transform, new Vector3(-8.2f, 0.95f, 0f), new Vector3(0.7f, 0.35f, 17f), mats.MarbleWhite);
            CreateBox("PlazaBorder_E", temple.transform, new Vector3(8.2f, 0.95f, 0f), new Vector3(0.7f, 0.35f, 17f), mats.MarbleWhite);

            // Sacred Sanctuary Fantasy Trees & Monolith Boulders framing the terrace
            CreateStylizedTree("TempleTree_L", temple.transform, new Vector3(-7.2f, 0.95f, -6.0f), mats, TreeType.FantasySky);
            CreateStylizedTree("TempleTree_R", temple.transform, new Vector3(7.2f, 0.95f, -6.0f), mats, TreeType.FantasySky);
            CreateStylizedRock("TempleBoulder_L", temple.transform, new Vector3(-7.4f, 0.90f, 6.0f), RockType.LargeBoulder);
            CreateStylizedRock("TempleBoulder_R", temple.transform, new Vector3(7.4f, 0.90f, 6.0f), RockType.LargeBoulder);

            // 4 Giant Corner Temple Columns with Golden Caps & Flame Urns
            Vector3[] columnOffsets = new Vector3[]
            {
                new Vector3(-6.8f, 0.9f, -6.8f),
                new Vector3(6.8f, 0.9f, -6.8f),
                new Vector3(-6.8f, 0.9f, 6.8f),
                new Vector3(6.8f, 0.9f, 6.8f)
            };

            for (int i = 0; i < columnOffsets.Length; i++)
            {
                Vector3 off = columnOffsets[i];
                CreateCylinder($"TempleCol_{i}", temple.transform, off + Vector3.up * 2.5f, new Vector3(1.2f, 5.0f, 1.2f), mats.MarbleWhite);
                CreateCylinder($"ColCap_{i}", temple.transform, off + Vector3.up * 5.1f, new Vector3(1.6f, 0.45f, 1.6f), mats.Gold);
                CreateSphere($"BrazierFlame_{i}", temple.transform, off + Vector3.up * 5.7f, new Vector3(0.75f, 0.95f, 0.75f), mats.RuneGlow);
            }

            // Central Celestial Altar Sanctuary Shrine
            GameObject shrine = new GameObject("CelestialAltarShrine");
            shrine.transform.SetParent(temple.transform, false);
            shrine.transform.localPosition = new Vector3(0f, 0.9f, 2.5f);

            CreateCylinder("Step_Base", shrine.transform, new Vector3(0f, 0.2f, 0f), new Vector3(5.8f, 0.4f, 5.8f), mats.MarbleWhite);
            CreateCylinder("Step_Mid", shrine.transform, new Vector3(0f, 0.6f, 0f), new Vector3(4.4f, 0.4f, 4.4f), mats.StonePath);
            CreateCylinder("Altar", shrine.transform, new Vector3(0f, 1.0f, 0f), new Vector3(3.0f, 0.4f, 3.0f), mats.Gold);

            // Rotating Concentric Victory Rings
            CreateCylinder("OuterRing", shrine.transform, new Vector3(0f, 2.8f, 0f), new Vector3(3.2f, 0.2f, 3.2f), mats.Gold);
            GameObject innerRing = CreateCylinder("InnerRing", shrine.transform, new Vector3(0f, 2.8f, 0f), new Vector3(2.2f, 0.18f, 2.2f), mats.RuneGlow);
            innerRing.transform.localRotation = Quaternion.Euler(45f, 0f, 0f);

            // Glowing Floating Divine Crystal
            GameObject crystal = CreateBox("DivineCrystal", shrine.transform, new Vector3(0f, 2.8f, 0f), new Vector3(1.1f, 1.8f, 1.1f), mats.CrystalBlue);
            crystal.transform.localRotation = Quaternion.Euler(45f, 45f, 0f);

            // Celebration Particles
            GameObject fxGo = new GameObject("CelebrationParticles");
            fxGo.transform.SetParent(shrine.transform, false);
            fxGo.transform.localPosition = new Vector3(0f, 1.2f, 0f);
            var ps = fxGo.AddComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            var main = ps.main;
            main.playOnAwake = false;
            main.loop = true;
            main.startLifetime = 1.8f;
            main.startSpeed = 5.5f;
            main.startSize = 0.35f;
            main.startColor = new Color(0.2f, 0.9f, 1.0f, 1f);

            var emission = ps.emission;
            emission.rateOverTime = 45f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 30f;
            shape.radius = 1.8f;

            // Goal trigger
            var goal = shrine.AddComponent<LevelGoal>();
            var trig = shrine.AddComponent<BoxCollider>();
            trig.isTrigger = true;
            trig.center = new Vector3(0f, 2.0f, 0f);
            trig.size = new Vector3(4.5f, 4.0f, 4.5f);

            // Collectible Coins 19 & 20
            CreateCollectibleCoin("Coin_19", zone.transform, new Vector3(0f, 13.8f, 180.5f), mats.GoldBody, mats.GoldRim, mats.GoldSymbol, mats.ParticleSparkle); // Grand staircase center
            CreateCollectibleCoin("Coin_20", temple.transform, new Vector3(0f, 2.2f, -1.8f), mats.GoldBody, mats.GoldRim, mats.GoldSymbol, mats.ParticleSparkle); // Altar crown coin
        }

        private static void BuildGrandTempleStaircase(Transform parent, Vector3 start, Vector3 end, MaterialDictionary mats)
        {
            GameObject stairs = new GameObject("GrandTempleStaircase");
            stairs.transform.SetParent(parent, false);

            int stepCount = 6;
            for (int i = 0; i <= stepCount; i++)
            {
                float t = (float)i / stepCount;
                Vector3 pos = Vector3.Lerp(start, end, t);

                // Wide marble slab with box collider
                CreateBox($"Step_{i}", stairs.transform, pos, new Vector3(4.8f, 0.4f, 1.5f), mats.MarbleWhite, true);

                // Golden banister posts on flanks
                if (i % 2 == 0)
                {
                    CreateCylinder($"Banister_L_{i}", stairs.transform, pos + new Vector3(-2.2f, 0.6f, 0f), new Vector3(0.18f, 1.2f, 0.18f), mats.Gold, false);
                    CreateCylinder($"Banister_R_{i}", stairs.transform, pos + new Vector3(2.2f, 0.6f, 0f), new Vector3(0.18f, 1.2f, 0.18f), mats.Gold, false);
                }
            }
        }

        #endregion

        #region Atmosphere, Layered Cloud Banks & Distant Fantasy Sky Realm

        private static void BuildLayeredCloudBanks(Transform parent, MaterialDictionary mats)
        {
            GameObject cloudSea = new GameObject("LayeredCloudBanks");
            cloudSea.transform.SetParent(parent, false);

            // 1. Deep Ocean of Clouds (Y = -10m to -14m)
            Vector3[] deepCloudPositions = new Vector3[]
            {
                new Vector3(-22f, -10.5f, 10f),
                new Vector3(24f, -11.0f, 25f),
                new Vector3(-26f, -10.0f, 45f),
                new Vector3(22f, -10.5f, 65f),
                new Vector3(-20f, -11.5f, 85f),
                new Vector3(24f, -11.0f, 105f),
                new Vector3(-22f, -10.0f, 125f),
                new Vector3(26f, -10.5f, 145f),
                new Vector3(-20f, -11.0f, 165f),
                new Vector3(22f, -10.5f, 185f),
                new Vector3(0f, -12.0f, 35f),
                new Vector3(0f, -12.5f, 110f),
                new Vector3(0f, -13.0f, 185f)
            };

            for (int i = 0; i < deepCloudPositions.Length; i++)
            {
                GameObject cluster = new GameObject($"DeepCloudBank_{i}");
                cluster.transform.SetParent(cloudSea.transform, false);
                cluster.transform.position = deepCloudPositions[i];

                CreateSphere("Puff_A", cluster.transform, Vector3.zero, new Vector3(10.0f, 4.2f, 9.0f), mats.Cloud);
                CreateSphere("Puff_B", cluster.transform, new Vector3(4.0f, -0.4f, 2.0f), new Vector3(8.0f, 3.4f, 7.0f), mats.Cloud);
                CreateSphere("Puff_C", cluster.transform, new Vector3(-3.8f, -0.3f, -1.8f), new Vector3(8.5f, 3.6f, 7.5f), mats.Cloud);
                CreateSphere("Puff_D", cluster.transform, new Vector3(1.0f, 0.8f, -3.2f), new Vector3(6.5f, 3.0f, 5.8f), mats.Cloud);
            }

            // 2. Mid-Level Atmospheric Clouds (Y = 1m to 6m, flanking the level for depth)
            Vector3[] midCloudPositions = new Vector3[]
            {
                new Vector3(-16f, 1.5f, 20f),
                new Vector3(17f, 2.0f, 45f),
                new Vector3(-18f, 3.5f, 75f),
                new Vector3(18f, 4.0f, 110f),
                new Vector3(-16f, 6.5f, 145f),
                new Vector3(17f, 7.0f, 175f)
            };

            for (int i = 0; i < midCloudPositions.Length; i++)
            {
                GameObject cluster = new GameObject($"MidCloud_{i}");
                cluster.transform.SetParent(cloudSea.transform, false);
                cluster.transform.position = midCloudPositions[i];

                CreateSphere("Puff_1", cluster.transform, Vector3.zero, new Vector3(6.5f, 2.4f, 5.5f), mats.Cloud);
                CreateSphere("Puff_2", cluster.transform, new Vector3(2.5f, -0.2f, 1.2f), new Vector3(5.0f, 1.8f, 4.2f), mats.Cloud);
                CreateSphere("Puff_3", cluster.transform, new Vector3(-2.2f, 0.2f, -1.0f), new Vector3(4.5f, 1.6f, 3.8f), mats.Cloud);
            }
        }

        private static void BuildDistantFantasyRealm(Transform parent, MaterialDictionary mats)
        {
            GameObject distant = new GameObject("DistantFantasyRealm");
            distant.transform.SetParent(parent, false);

            // Floating Fantasy Isles & Mountain Silhouettes
            Vector3[] spots = new Vector3[]
            {
                new Vector3(-55f, 6.0f, 30f),
                new Vector3(58f, 5.0f, 45f),
                new Vector3(-50f, 12.0f, 90f),
                new Vector3(52f, 10.0f, 115f),
                new Vector3(-45f, 16.0f, 160f),
                new Vector3(48f, 15.0f, 185f),
                new Vector3(-40f, 20.0f, 215f),
                new Vector3(42f, 18.0f, 230f)
            };

            for (int i = 0; i < spots.Length; i++)
            {
                Vector3 pt = spots[i];
                GameObject di = new GameObject($"DistantIsland_{i}");
                di.transform.SetParent(distant.transform, false);
                di.transform.position = pt;

                CreateBox("Plateau", di.transform, Vector3.zero, new Vector3(12f, 2.0f, 11f), mats.Grass);
                CreateBox("RockBase", di.transform, new Vector3(0f, -2.0f, 0f), new Vector3(9.5f, 2.8f, 8.5f), mats.Rock);
                CreateBox("RockRoot", di.transform, new Vector3(0f, -4.5f, 0f), new Vector3(5.0f, 3.5f, 4.5f), mats.RockDark);

                // Distant Architecture (Tower / Castle / Spire)
                if (i % 2 == 0)
                {
                    // Watchtower
                    CreateCylinder("Tower", di.transform, new Vector3(-2.0f, 3.0f, -1.5f), new Vector3(2.0f, 6.0f, 2.0f), mats.StonePath);
                    CreateCylinder("TowerRoof", di.transform, new Vector3(-2.0f, 6.4f, -1.5f), new Vector3(2.6f, 0.8f, 2.6f), mats.WoodDark);
                    CreateSphere("Beacon", di.transform, new Vector3(-2.0f, 7.2f, -1.5f), new Vector3(0.8f, 0.8f, 0.8f), mats.RuneGlow);
                }
                else
                {
                    // Crystal Peak
                    CreateBox("CrystalPeak", di.transform, new Vector3(2.0f, 3.2f, 1.5f), new Vector3(1.2f, 5.0f, 1.2f), mats.CrystalBlue);
                }

                TreeType tt = (i % 5 == 0) ? TreeType.LargeLush : (i % 5 == 1 ? TreeType.TallForest : (i % 5 == 2 ? TreeType.TropicalPalm : (i % 5 == 3 ? TreeType.AutumnOak : TreeType.FantasySky)));
                CreateStylizedTree("Tree", di.transform, new Vector3(2.5f, 1.0f, -1.8f), mats, tt);
            }
        }

        #endregion

        #region Obstacle Builders (Bounce Pads, Sweeper, Disappearing, Moving)

        private static GameObject CreateBouncePad(string name, Transform parent, Vector3 localPos, float bounceHeight, MaterialDictionary mats)
        {
            GameObject padGo = new GameObject(name);
            padGo.transform.SetParent(parent, false);
            padGo.transform.localPosition = localPos;

            // Outer stone border rim
            CreateCylinder("PadRim", padGo.transform, new Vector3(0f, 0.12f, 0f), new Vector3(2.6f, 0.24f, 2.6f), mats.Rock, false);

            // Spring pad mound (animated by BouncePad.cs)
            GameObject mound = CreateCylinder("PadMound", padGo.transform, new Vector3(0f, 0.22f, 0f), new Vector3(2.2f, 0.35f, 2.2f), mats.BouncePad, false);

            // Glowing energy ring on pad
            CreateCylinder("PadEnergyRing", padGo.transform, new Vector3(0f, 0.26f, 0f), new Vector3(1.6f, 0.08f, 1.6f), mats.RuneGlow, false);

            // BouncePad logic
            var pad = padGo.AddComponent<BouncePad>();
            pad.Configure(bounceHeight);

            // Generous trigger collider for responsive launch
            var col = padGo.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.center = new Vector3(0f, 0.35f, 0f);
            col.size = new Vector3(2.4f, 0.7f, 2.4f);

            return padGo;
        }

        private static GameObject CreateDisappearingPlatform(string name, Transform parent, Vector3 localPos, Vector3 size, MaterialDictionary mats)
        {
            GameObject plat = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plat.name = name;
            plat.transform.SetParent(parent, false);
            plat.transform.localPosition = localPos;
            plat.transform.localScale = size;
            plat.GetComponent<Renderer>().sharedMaterial = mats.GrassDark;

            // DisappearingPlatform component handles warning shake & disappearance
            var dp = plat.AddComponent<DisappearingPlatform>();
            dp.SetupMaterials(mats.GrassDark, mats.DisappearingWarn);

            // Inlaid stone target border
            CreateBox("StoneTarget", plat.transform, new Vector3(0f, 0.51f, 0f), new Vector3(0.75f, 0.04f, 0.75f), mats.StonePath, false);

            // Decorative underside crystal prongs
            GameObject crystalProng = CreateBox("UndersideCrystal", plat.transform, new Vector3(0f, -0.6f, 0f), new Vector3(0.4f, 0.8f, 0.4f), mats.CrystalPurple, false);
            crystalProng.transform.localRotation = Quaternion.Euler(45f, 45f, 0f);

            return plat;
        }

        private static GameObject CreateRotatingSweeper(string name, Transform parent, Vector3 localPos, float armLength, float speed, bool clockwise, MaterialDictionary mats)
        {
            GameObject sweeper = new GameObject(name);
            sweeper.transform.SetParent(parent, false);
            sweeper.transform.localPosition = localPos;

            // Center pillar pivot
            CreateCylinder("SweeperHub", sweeper.transform, new Vector3(0f, 0.7f, 0f), new Vector3(1.1f, 1.4f, 1.1f), mats.RockDark, true);
            CreateSphere("HubTopGlow", sweeper.transform, new Vector3(0f, 1.45f, 0f), new Vector3(0.8f, 0.8f, 0.8f), mats.RuneGlow);

            // Rotating Arm GameObject
            GameObject arm = new GameObject("SweeperArm");
            arm.transform.SetParent(sweeper.transform, false);
            arm.transform.localPosition = new Vector3(0f, 0.55f, 0f);

            // Dual sweeping beam with box collider
            GameObject beam = CreateBox("BeamMesh", arm.transform, Vector3.zero, new Vector3(armLength * 2f, 0.45f, 0.45f), mats.ObstacleSweeper, true);

            // Hazard warning light spheres at beam ends
            CreateSphere("HazardLight_L", arm.transform, new Vector3(-armLength, 0f, 0f), new Vector3(0.6f, 0.6f, 0.6f), mats.RuneGlow);
            CreateSphere("HazardLight_R", arm.transform, new Vector3(armLength, 0f, 0f), new Vector3(0.6f, 0.6f, 0.6f), mats.RuneGlow);

            var rs = sweeper.AddComponent<RotatingSweeper>();
            rs.Configure(speed, clockwise);

            return sweeper;
        }

        private static GameObject CreateMovingPlatform(string name, Transform parent, Vector3 startPos, Vector3 delta, float travelTime, float pauseTime, Vector3 size, MaterialDictionary mats, Material coreMat = null)
        {
            GameObject platform = new GameObject(name);
            platform.transform.SetParent(parent, false);
            platform.transform.position = startPos;

            CreateBox("PlatformDeck", platform.transform, Vector3.zero, size, mats.Wood, true);
            CreateBox("CoreSurface", platform.transform, new Vector3(0f, 0.05f, 0f), new Vector3(size.x * 0.85f, size.y * 1.05f, size.z * 0.85f), coreMat ?? mats.WoodBridge);
            CreateCylinder("RuneRing", platform.transform, new Vector3(0f, 0.06f, 0f), new Vector3(size.x * 0.55f, size.y * 0.1f, size.z * 0.55f), mats.RuneGlow);

            var mp = platform.AddComponent<MovingPlatform>();
            mp.Configure(delta, travelTime, pauseTime);

            // Platform top trigger for seamless player carrying (height 4.0m)
            BoxCollider trig = platform.AddComponent<BoxCollider>();
            trig.isTrigger = true;
            trig.center = new Vector3(0f, 2.0f, 0f);
            trig.size = new Vector3(size.x * 0.95f, 4.0f, size.z * 0.95f);

            return platform;
        }

        #endregion

        #region Proportionate Stylized Foliage & Props (Framing Route, Never Blocking View)

        private enum TreeType { LargeLush, DecorativeSmall, TropicalPalm, AutumnOak, FantasySky, TallForest }
        private enum RockType { LargeBoulder, MediumStone, SmallCluster }

        private static GameObject CreateStylizedTree(string name, Transform parent, Vector3 localPos, MaterialDictionary mats, TreeType type)
        {
            string prefabName = type switch
            {
                TreeType.LargeLush => "Tree_LargeLush",
                TreeType.TallForest => "Tree_TallForest",
                TreeType.DecorativeSmall => "Tree_DecorativeSmall",
                TreeType.TropicalPalm => "Tree_TropicalPalm",
                TreeType.AutumnOak => "Tree_AutumnOak",
                TreeType.FantasySky => "Tree_FantasySky",
                _ => "Tree_LargeLush"
            };

#if UNITY_EDITOR
            GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Prefabs/Environment/{prefabName}.prefab");
            if (prefab != null)
            {
                GameObject instance = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab);
                instance.name = name;
                instance.transform.SetParent(parent, false);
                instance.transform.localPosition = localPos;
                instance.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                return instance;
            }
#endif
            GameObject tree = new GameObject(name);
            tree.transform.SetParent(parent, false);
            tree.transform.localPosition = localPos;
            return tree;
        }

        private static GameObject CreateStylizedRock(string name, Transform parent, Vector3 localPos, RockType type)
        {
            string prefabName = type switch
            {
                RockType.LargeBoulder => "Rock_LargeBoulder",
                RockType.MediumStone => "Rock_MediumStone",
                RockType.SmallCluster => "Rock_SmallCluster",
                _ => "Rock_LargeBoulder"
            };

#if UNITY_EDITOR
            GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Prefabs/Environment/{prefabName}.prefab");
            if (prefab != null)
            {
                GameObject instance = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab);
                instance.name = name;
                instance.transform.SetParent(parent, false);
                instance.transform.localPosition = localPos;
                instance.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                return instance;
            }
#endif
            GameObject rock = new GameObject(name);
            rock.transform.SetParent(parent, false);
            rock.transform.localPosition = localPos;
            return rock;
        }

        private static void CreateStylizedBush(string name, Transform parent, Vector3 localPos, float scale, Material mat)
        {
            GameObject bush = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bush.name = name;
            bush.transform.SetParent(parent, false);
            bush.transform.localPosition = localPos;
            bush.transform.localScale = new Vector3(scale * 1.2f, scale * 0.8f, scale * 1.1f);
            bush.GetComponent<Renderer>().sharedMaterial = mat;
            var col = bush.GetComponent<Collider>();
            if (col != null) DestroyImmediate(col);
        }

        private static void CreateBoulderCluster(string name, Transform parent, Vector3 localPos, MaterialDictionary mats)
        {
            CreateStylizedRock(name, parent, localPos, RockType.SmallCluster);
        }

        private static void CreateBoulder(string name, Transform parent, Vector3 localPos, Vector3 scale, Material mat)
        {
            GameObject boulder = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            boulder.name = name;
            boulder.transform.SetParent(parent, false);
            boulder.transform.localPosition = localPos;
            boulder.transform.localRotation = Quaternion.Euler(Random.Range(15, 60), Random.Range(0, 360), Random.Range(15, 60));
            boulder.transform.localScale = scale;
            boulder.GetComponent<Renderer>().sharedMaterial = mat;
            var col = boulder.GetComponent<Collider>();
            if (col != null) DestroyImmediate(col);
        }

        private static void CreateFlowerPatch(Transform parent, Vector3 localPos, MaterialDictionary mats)
        {
            GameObject patch = new GameObject("FlowerPatch");
            patch.transform.SetParent(parent, false);
            patch.transform.localPosition = localPos;

            for (int i = 0; i < 4; i++)
            {
                float ang = i * (Mathf.PI * 0.5f);
                Vector3 off = new Vector3(Mathf.Cos(ang) * 0.35f, 0.08f, Mathf.Sin(ang) * 0.35f);
                GameObject petal = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                petal.name = $"Petal_{i}";
                petal.transform.SetParent(patch.transform, false);
                petal.transform.localPosition = off;
                petal.transform.localScale = new Vector3(0.25f, 0.2f, 0.25f);
                petal.GetComponent<Renderer>().sharedMaterial = (i % 2 == 0) ? mats.FlowerPink : mats.FlowerYellow;
                DestroyImmediate(petal.GetComponent<Collider>());
            }
        }

        private static void CreateMushroomCluster(Transform parent, Vector3 localPos, MaterialDictionary mats)
        {
            GameObject shrooms = new GameObject("MushroomCluster");
            shrooms.transform.SetParent(parent, false);
            shrooms.transform.localPosition = localPos;

            CreateCylinder("Stem_1", shrooms.transform, new Vector3(0f, 0.2f, 0f), new Vector3(0.18f, 0.4f, 0.18f), mats.MushroomStem);
            CreateSphere("Cap_1", shrooms.transform, new Vector3(0f, 0.45f, 0f), new Vector3(0.55f, 0.35f, 0.55f), mats.MushroomCap);

            CreateCylinder("Stem_2", shrooms.transform, new Vector3(0.35f, 0.15f, 0.15f), new Vector3(0.14f, 0.3f, 0.14f), mats.MushroomStem);
            CreateSphere("Cap_2", shrooms.transform, new Vector3(0.35f, 0.32f, 0.15f), new Vector3(0.42f, 0.28f, 0.42f), mats.MushroomCap);
        }

        private static void CreateStonePath(Transform parent, Vector3 start, Vector3 end, Material mat)
        {
            GameObject pathGroup = new GameObject("StonePath");
            pathGroup.transform.SetParent(parent, false);

            int slabs = 6;
            for (int i = 0; i <= slabs; i++)
            {
                float t = (float)i / slabs;
                Vector3 pos = Vector3.Lerp(start, end, t);
                pos.x += Mathf.Sin(t * Mathf.PI * 2f) * 0.20f;

                GameObject slab = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                slab.name = $"Slab_{i}";
                slab.transform.SetParent(pathGroup.transform, false);
                slab.transform.localPosition = pos;
                slab.transform.localScale = new Vector3(1.1f, 0.05f, 0.85f);
                slab.transform.localRotation = Quaternion.Euler(0f, Random.Range(-20f, 20f), 0f);
                slab.GetComponent<Renderer>().sharedMaterial = mat;
                var col = slab.GetComponent<Collider>();
                if (col != null) DestroyImmediate(col);
            }
        }

        private static void CreateCheckpointBeacon(string name, Transform parent, Vector3 localPos, MaterialDictionary mats)
        {
            GameObject cpGo = new GameObject(name);
            cpGo.transform.SetParent(parent, false);
            cpGo.transform.localPosition = localPos;

            CreateCylinder("Plinth_Base", cpGo.transform, new Vector3(0f, 0.15f, 0f), new Vector3(1.6f, 0.3f, 1.6f), mats.StonePath);
            CreateCylinder("Plinth_Col", cpGo.transform, new Vector3(0f, 0.8f, 0f), new Vector3(0.6f, 1.0f, 0.6f), mats.Rock);

            GameObject beacon = CreateBox("BeaconCrystal", cpGo.transform, new Vector3(0f, 1.6f, 0f), new Vector3(0.45f, 0.7f, 0.45f), mats.RuneGlow);
            beacon.transform.localRotation = Quaternion.Euler(45f, 45f, 0f);

            var cp = cpGo.AddComponent<Checkpoint>();
            cp.Configure(beacon.GetComponent<Renderer>());

            var col = cpGo.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.center = new Vector3(0f, 1.0f, 0f);
            col.size = new Vector3(2.5f, 2.5f, 2.5f);
        }

        private static GameObject CreateCollectibleCoin(string name, Transform parent, Vector3 localPos, Material bodyMat, Material rimMat, Material symbolMat, Material sparkleMat = null)
        {
            // Clean root GameObject with standard unit scale
            GameObject coin = new GameObject(name);
            coin.transform.SetParent(parent, false);
            coin.transform.localPosition = localPos;
            coin.transform.localRotation = Quaternion.identity;
            coin.transform.localScale = Vector3.one;

            // Trigger SphereCollider for responsive collection detection
            var sphereCol = coin.AddComponent<SphereCollider>();
            sphereCol.isTrigger = true;
            sphereCol.radius = 0.85f;

            // Visual Model container
            GameObject model = new GameObject("Visuals");
            model.transform.SetParent(coin.transform, false);
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = Vector3.one;

            // 1. Darker Gold Outer Rim (Upright circular coin edge, diameter 0.96m, thickness 0.16m)
            GameObject rim = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            rim.name = "CoinRim";
            rim.transform.SetParent(model.transform, false);
            rim.transform.localPosition = Vector3.zero;
            rim.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            rim.transform.localScale = new Vector3(0.96f, 0.08f, 0.96f);
            rim.GetComponent<Renderer>().sharedMaterial = rimMat;
            var rimCol = rim.GetComponent<Collider>();
            if (rimCol != null) DestroyImmediate(rimCol);

            // 2. Raised Bevel Border
            GameObject bevel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            bevel.name = "CoinBevel";
            bevel.transform.SetParent(model.transform, false);
            bevel.transform.localPosition = Vector3.zero;
            bevel.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            bevel.transform.localScale = new Vector3(0.88f, 0.083f, 0.88f);
            bevel.GetComponent<Renderer>().sharedMaterial = rimMat;
            var bevelCol = bevel.GetComponent<Collider>();
            if (bevelCol != null) DestroyImmediate(bevelCol);

            // 3. Polished Metallic Golden Body Core
            GameObject core = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            core.name = "CoinCore";
            core.transform.SetParent(model.transform, false);
            core.transform.localPosition = Vector3.zero;
            core.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            core.transform.localScale = new Vector3(0.82f, 0.085f, 0.82f);
            core.GetComponent<Renderer>().sharedMaterial = bodyMat;
            var coreCol = core.GetComponent<Collider>();
            if (coreCol != null) DestroyImmediate(coreCol);

            // 4. 3D Embossed Dollar Symbol on FRONT Face (+Z)
            Create3DDollarSymbol(model.transform, new Vector3(0f, 0f, 0.087f), Quaternion.identity, symbolMat);

            // 5. 3D Embossed Dollar Symbol on BACK Face (-Z)
            Create3DDollarSymbol(model.transform, new Vector3(0f, 0f, -0.087f), Quaternion.Euler(0f, 180f, 0f), symbolMat);

            // 6. Ambient Sparkles
            CreateAmbientCoinSparkles(coin.transform, sparkleMat);

            // Attach Coin component
            coin.AddComponent<Coin>();
            return coin;
        }

        private static void Create3DDollarSymbol(Transform parent, Vector3 localPos, Quaternion localRot, Material symbolMat)
        {
            GameObject symRoot = new GameObject("DollarSymbol");
            symRoot.transform.SetParent(parent, false);
            symRoot.transform.localPosition = localPos;
            symRoot.transform.localRotation = localRot;
            symRoot.transform.localScale = Vector3.one;

            float depth = 0.032f;
            float strokeW = 0.065f;

            // Center vertical strike-through bar
            CreateSymbolBar(symRoot.transform, new Vector3(0f, 0f, 0f), new Vector3(0.055f, 0.52f, depth), symbolMat);

            // Top horizontal bar of 'S'
            CreateSymbolBar(symRoot.transform, new Vector3(0f, 0.17f, 0f), new Vector3(0.28f, strokeW, depth), symbolMat);

            // Top left vertical segment of 'S'
            CreateSymbolBar(symRoot.transform, new Vector3(-0.11f, 0.085f, 0f), new Vector3(strokeW, 0.12f, depth), symbolMat);

            // Middle horizontal cross bar of 'S'
            CreateSymbolBar(symRoot.transform, new Vector3(0f, 0f, 0f), new Vector3(0.28f, strokeW, depth), symbolMat);

            // Bottom right vertical segment of 'S'
            CreateSymbolBar(symRoot.transform, new Vector3(0.11f, -0.085f, 0f), new Vector3(strokeW, 0.12f, depth), symbolMat);

            // Bottom horizontal bar of 'S'
            CreateSymbolBar(symRoot.transform, new Vector3(0f, -0.17f, 0f), new Vector3(0.28f, strokeW, depth), symbolMat);

            // Top-right serif (pointing down)
            CreateSymbolBar(symRoot.transform, new Vector3(0.11f, 0.14f, 0f), new Vector3(strokeW, 0.06f, depth), symbolMat);

            // Bottom-left serif (pointing up)
            CreateSymbolBar(symRoot.transform, new Vector3(-0.11f, -0.14f, 0f), new Vector3(strokeW, 0.06f, depth), symbolMat);
        }

        private static void CreateSymbolBar(Transform parent, Vector3 pos, Vector3 scale, Material mat)
        {
            GameObject bar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bar.name = "SymBar";
            bar.transform.SetParent(parent, false);
            bar.transform.localPosition = pos;
            bar.transform.localRotation = Quaternion.identity;
            bar.transform.localScale = scale;
            bar.GetComponent<Renderer>().sharedMaterial = mat;
            var col = bar.GetComponent<Collider>();
            if (col != null) DestroyImmediate(col);
        }

        private static void CreateAmbientCoinSparkles(Transform parent, Material sparkleMat)
        {
            GameObject fxGo = new GameObject("AmbientSparkles");
            fxGo.transform.SetParent(parent, false);
            fxGo.transform.localPosition = Vector3.zero;

            var ps = fxGo.AddComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var psRend = fxGo.GetComponent<ParticleSystemRenderer>();
            if (psRend != null && sparkleMat != null)
            {
                psRend.sharedMaterial = sparkleMat;
            }

            var main = ps.main;
            main.playOnAwake = true;
            main.duration = 1.0f;
            main.loop = true;
            main.startLifetime = 0.8f;
            main.startSpeed = 0.35f;
            main.startSize = 0.08f;
            main.startColor = new Color(1.0f, 0.88f, 0.30f, 0.70f);
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = 16;

            var emission = ps.emission;
            emission.rateOverTime = 2.5f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.55f;

            var colorLife = ps.colorOverLifetime;
            colorLife.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] { new GradientColorKey(new Color(1f, 0.95f, 0.5f), 0f), new GradientColorKey(new Color(1f, 0.75f, 0.15f), 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(0.9f, 0.3f), new GradientAlphaKey(0f, 1f) }
            );
            colorLife.color = grad;

            var sizeLife = ps.sizeOverLifetime;
            sizeLife.enabled = true;
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0f, 0.2f);
            curve.AddKey(0.5f, 1.0f);
            curve.AddKey(1f, 0f);
            sizeLife.size = new ParticleSystem.MinMaxCurve(1f, curve);

            ps.Play();
        }

        private static GameObject CreateBox(string name, Transform parent, Vector3 localPos, Vector3 localScale, Material mat, bool addCollider = false)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = localScale;
            go.GetComponent<Renderer>().sharedMaterial = mat;
            if (!addCollider)
            {
                var col = go.GetComponent<Collider>();
                if (col != null) DestroyImmediate(col);
            }
            return go;
        }

        private static GameObject CreateCylinder(string name, Transform parent, Vector3 localPos, Vector3 localScale, Material mat, bool addCollider = false)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = new Vector3(localScale.x, localScale.y * 0.5f, localScale.z);
            go.GetComponent<Renderer>().sharedMaterial = mat;
            if (!addCollider)
            {
                var col = go.GetComponent<Collider>();
                if (col != null) DestroyImmediate(col);
            }
            return go;
        }

        private static GameObject CreateSphere(string name, Transform parent, Vector3 localPos, Vector3 localScale, Material mat)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = localScale;
            go.GetComponent<Renderer>().sharedMaterial = mat;
            var col = go.GetComponent<Collider>();
            if (col != null) DestroyImmediate(col);
            return go;
        }

        #endregion

        #region Hero Character Builder

        private static GameObject BuildHeroCharacter(Vector3 spawnPos, MaterialDictionary mats)
        {
            GameObject player = new GameObject("Player");
            player.tag = "Player";
            player.transform.position = spawnPos;

            var cc = player.AddComponent<CharacterController>();
            cc.height = 1.60f;
            cc.radius = 0.35f;
            cc.center = new Vector3(0f, 0.80f, 0f);
            cc.skinWidth = 0.04f;
            cc.stepOffset = 0.35f;
            cc.slopeLimit = 45f;

            player.AddComponent<AudioSource>();

            GameObject model = new GameObject("Model");
            model.transform.SetParent(player.transform, false);

            // Torso
            GameObject torso = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            torso.name = "Torso";
            torso.transform.SetParent(model.transform, false);
            torso.transform.localPosition = new Vector3(0f, 0.88f, 0f);
            torso.transform.localScale = new Vector3(0.66f, 0.52f, 0.56f);
            torso.GetComponent<Renderer>().sharedMaterial = mats.PlayerSuit;
            DestroyImmediate(torso.GetComponent<Collider>());

            // Jetpack
            GameObject pack = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pack.name = "Jetpack";
            pack.transform.SetParent(torso.transform, false);
            pack.transform.localPosition = new Vector3(0f, 0.05f, -0.65f);
            pack.transform.localScale = new Vector3(0.68f, 0.72f, 0.46f);
            pack.GetComponent<Renderer>().sharedMaterial = mats.PlayerWhite;
            DestroyImmediate(pack.GetComponent<Collider>());

            // Head
            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "Head";
            head.transform.SetParent(model.transform, false);
            head.transform.localPosition = new Vector3(0f, 1.50f, 0f);
            head.transform.localScale = new Vector3(0.62f, 0.58f, 0.60f);
            head.GetComponent<Renderer>().sharedMaterial = mats.PlayerSuit;
            DestroyImmediate(head.GetComponent<Collider>());

            // Visor
            GameObject visor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            visor.name = "Visor";
            visor.transform.SetParent(head.transform, false);
            visor.transform.localPosition = new Vector3(0f, 0.06f, 0.46f);
            visor.transform.localScale = new Vector3(0.66f, 0.28f, 0.25f);
            visor.GetComponent<Renderer>().sharedMaterial = mats.PlayerVisor;
            DestroyImmediate(visor.GetComponent<Collider>());

            // Left Arm
            GameObject lArmPivot = new GameObject("LeftArmPivot");
            lArmPivot.transform.SetParent(model.transform, false);
            lArmPivot.transform.localPosition = new Vector3(-0.44f, 1.15f, 0f);
            GameObject lArm = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            lArm.name = "LeftArm";
            lArm.transform.SetParent(lArmPivot.transform, false);
            lArm.transform.localPosition = new Vector3(0f, -0.24f, 0f);
            lArm.transform.localScale = new Vector3(0.22f, 0.24f, 0.22f);
            lArm.GetComponent<Renderer>().sharedMaterial = mats.PlayerWhite;
            DestroyImmediate(lArm.GetComponent<Collider>());

            // Right Arm
            GameObject rArmPivot = new GameObject("RightArmPivot");
            rArmPivot.transform.SetParent(model.transform, false);
            rArmPivot.transform.localPosition = new Vector3(0.44f, 1.15f, 0f);
            GameObject rArm = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            rArm.name = "RightArm";
            rArm.transform.SetParent(rArmPivot.transform, false);
            rArm.transform.localPosition = new Vector3(0f, -0.24f, 0f);
            rArm.transform.localScale = new Vector3(0.22f, 0.24f, 0.22f);
            rArm.GetComponent<Renderer>().sharedMaterial = mats.PlayerWhite;
            DestroyImmediate(rArm.GetComponent<Collider>());

            // Left Leg - Pivot at Y = 0.45, Child at -0.225, Scale 0.225 -> Feet touch EXACT local Y = 0.0
            GameObject lLegPivot = new GameObject("LeftLegPivot");
            lLegPivot.transform.SetParent(model.transform, false);
            lLegPivot.transform.localPosition = new Vector3(-0.21f, 0.45f, 0f);
            GameObject lLeg = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            lLeg.name = "LeftLeg";
            lLeg.transform.SetParent(lLegPivot.transform, false);
            lLeg.transform.localPosition = new Vector3(0f, -0.225f, 0f);
            lLeg.transform.localScale = new Vector3(0.24f, 0.225f, 0.24f);
            lLeg.GetComponent<Renderer>().sharedMaterial = mats.PlayerWhite;
            DestroyImmediate(lLeg.GetComponent<Collider>());

            // Right Leg - Pivot at Y = 0.45, Child at -0.225, Scale 0.225 -> Feet touch EXACT local Y = 0.0
            GameObject rLegPivot = new GameObject("RightLegPivot");
            rLegPivot.transform.SetParent(model.transform, false);
            rLegPivot.transform.localPosition = new Vector3(0.21f, 0.45f, 0f);
            GameObject rLeg = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            rLeg.name = "RightLeg";
            rLeg.transform.SetParent(rLegPivot.transform, false);
            rLeg.transform.localPosition = new Vector3(0f, -0.225f, 0f);
            rLeg.transform.localScale = new Vector3(0.24f, 0.225f, 0.24f);
            rLeg.GetComponent<Renderer>().sharedMaterial = mats.PlayerWhite;
            DestroyImmediate(rLeg.GetComponent<Collider>());

            var pc = player.AddComponent<PlayerController>();
            pc.SetupLimbReferences(model.transform, lLegPivot.transform, rLegPivot.transform, lArmPivot.transform, rArmPivot.transform);

            return player;
        }

        #endregion

        #region Fixed Third-Person Camera Setup

        private static void SetupThirdPersonCamera(Transform playerTransform)
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                GameObject camGo = new GameObject("Main Camera");
                cam = camGo.AddComponent<Camera>();
                camGo.tag = "MainCamera";
                camGo.AddComponent<AudioListener>();
            }

            cam.clearFlags = CameraClearFlags.Skybox;
            cam.fieldOfView = 60f;
            cam.nearClipPlane = 0.2f;

            var tpc = cam.GetComponent<ThirdPersonCamera>();
            if (tpc == null) tpc = cam.gameObject.AddComponent<ThirdPersonCamera>();
            tpc.SetTarget(playerTransform);

            var uac = cam.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
            if (uac == null) uac = cam.gameObject.AddComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
            uac.antialiasing = UnityEngine.Rendering.Universal.AntialiasingMode.FastApproximateAntialiasing;
        }

        #endregion

        #region Faculty Presentation Modern UI System

        private static GameObject BuildFacultyGameUI()
        {
            GameObject canvasGo = new GameObject("Canvas_UI");
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            canvasGo.AddComponent<GraphicRaycaster>();

            if (FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject es = new GameObject("EventSystem");
                es.AddComponent<UnityEngine.EventSystems.EventSystem>();
#if ENABLE_INPUT_SYSTEM
                es.AddComponent<InputSystemUIInputModule>();
#else
                es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
#endif
            }

            var uiManager = canvasGo.AddComponent<UIManager>();

            // 1. In-Game HUD Panel
            GameObject hud = CreateUIPanel("HUD_Panel", canvasGo.transform, new Color(0f, 0f, 0f, 0f));
            RectTransform hudRect = hud.GetComponent<RectTransform>();
            hudRect.anchorMin = Vector2.zero;
            hudRect.anchorMax = Vector2.one;
            hudRect.offsetMin = Vector2.zero;
            hudRect.offsetMax = Vector2.zero;

            // Coin counter pill badge
            GameObject coinBadge = CreateUIPanel("CoinBadge", hud.transform, new Color(0.06f, 0.09f, 0.16f, 0.88f));
            RectTransform cbRect = coinBadge.GetComponent<RectTransform>();
            cbRect.anchorMin = new Vector2(0f, 1f);
            cbRect.anchorMax = new Vector2(0f, 1f);
            cbRect.pivot = new Vector2(0f, 1f);
            cbRect.anchoredPosition = new Vector2(30f, -30f);
            cbRect.sizeDelta = new Vector2(290f, 60f);

            Text coinText = CreateUIText("CoinText", coinBadge.transform, "★ COINS: 0 / 20", 25, TextAnchor.MiddleCenter, new Color(1f, 0.86f, 0.12f));
            RectTransform ctRect = coinText.GetComponent<RectTransform>();
            ctRect.anchorMin = Vector2.zero;
            ctRect.anchorMax = Vector2.one;
            ctRect.offsetMin = Vector2.zero;
            ctRect.offsetMax = Vector2.zero;

            // Level Title Badge (Top Center)
            GameObject titleBadge = CreateUIPanel("TitleBadge", hud.transform, new Color(0.06f, 0.09f, 0.16f, 0.70f));
            RectTransform tbRect = titleBadge.GetComponent<RectTransform>();
            tbRect.anchorMin = new Vector2(0.5f, 1f);
            tbRect.anchorMax = new Vector2(0.5f, 1f);
            tbRect.pivot = new Vector2(0.5f, 1f);
            tbRect.anchoredPosition = new Vector2(0f, -25f);
            tbRect.sizeDelta = new Vector2(380f, 44f);
            CreateUIText("GameTitleText", titleBadge.transform, "SKYBOUND ADVENTURE", 19, TextAnchor.MiddleCenter, new Color(0.85f, 0.92f, 1f), Vector2.zero, new Vector2(380f, 44f));

            // Controls Hint (Clean, docked at Bottom Center)
            Text hintText = CreateUIText("ControlsHint", hud.transform, "WASD: Move  •  SHIFT: Sprint  •  SPACE: Jump  •  R: Backflip  •  ESC: Pause", 18, TextAnchor.MiddleCenter, new Color(1f, 1f, 1f, 0.92f));
            RectTransform hintRect = hintText.GetComponent<RectTransform>();
            hintRect.anchorMin = new Vector2(0.5f, 0f);
            hintRect.anchorMax = new Vector2(0.5f, 0f);
            hintRect.pivot = new Vector2(0.5f, 0f);
            hintRect.anchoredPosition = new Vector2(0f, 22f);
            hintRect.sizeDelta = new Vector2(900f, 40f);

            // Pause Button (Top Right)
            Button pauseBtn = CreateUIButton("PauseButton", hud.transform, "PAUSE [ESC]", new Vector2(1f, 1f), new Vector2(-120f, -50f), new Vector2(160f, 50f), new Color(0.18f, 0.22f, 0.32f, 0.92f), 18);

            // 2. Faculty Main Menu Presentation Card
            GameObject mainMenu = CreateCardPanel("MainMenu_Panel", canvasGo.transform, new Vector2(720f, 520f));
            CreateUIText("Title", mainMenu.transform, "SKYBOUND ADVENTURE", 46, TextAnchor.MiddleCenter, new Color(1f, 0.88f, 0.15f), new Vector2(0f, 160f), new Vector2(660f, 75f));
            CreateUIText("Subtitle", mainMenu.transform, "A 3D Floating Island Platformer", 23, TextAnchor.MiddleCenter, new Color(0.85f, 0.92f, 1f), new Vector2(0f, 105f), new Vector2(620f, 40f));
            CreateUIText("Instructions", mainMenu.transform, "• WASD: Movement (Camera-Independent Level Coordinates)\n• SHIFT: Sprint / Dash  •  SPACE: Jump (Full Air Control)\n• Ride the Great Sky Ferry and avoid Sweeper hazards\n• Checkpoint beacons securely preserve your journey\n• Collect all 20 golden coins to reach the Sky Sanctuary!", 19, TextAnchor.MiddleCenter, new Color(0.82f, 0.88f, 0.95f), new Vector2(0f, 0f), new Vector2(620f, 130f));
            Button playBtn = CreateUIButton("PlayButton", mainMenu.transform, "PLAY ADVENTURE", new Vector2(0.5f, 0.5f), new Vector2(0f, -145f), new Vector2(290f, 65f), new Color(0.18f, 0.68f, 0.28f), 24);
            mainMenu.SetActive(false);

            // 3. Pause Menu Panel
            GameObject pauseMenu = CreateCardPanel("PauseMenu_Panel", canvasGo.transform, new Vector2(500f, 420f));
            CreateUIText("Title", pauseMenu.transform, "GAME PAUSED", 40, TextAnchor.MiddleCenter, Color.white, new Vector2(0f, 130f), new Vector2(400f, 60f));
            Button resumeBtn = CreateUIButton("ResumeButton", pauseMenu.transform, "RESUME", new Vector2(0.5f, 0.5f), new Vector2(0f, 45f), new Vector2(260f, 55f), new Color(0.18f, 0.52f, 0.85f), 22);
            Button pauseRestartBtn = CreateUIButton("RestartButton", pauseMenu.transform, "RESTART", new Vector2(0.5f, 0.5f), new Vector2(0f, -30f), new Vector2(260f, 55f), new Color(0.88f, 0.50f, 0.15f), 22);
            Button pauseMenuBtn = CreateUIButton("MenuButton", pauseMenu.transform, "MAIN MENU", new Vector2(0.5f, 0.5f), new Vector2(0f, -105f), new Vector2(260f, 55f), new Color(0.42f, 0.45f, 0.55f), 20);
            pauseMenu.SetActive(false);

            // 4. Game Over Panel
            GameObject gameOver = CreateCardPanel("GameOver_Panel", canvasGo.transform, new Vector2(550f, 420f));
            CreateUIText("Title", gameOver.transform, "YOU FELL!", 44, TextAnchor.MiddleCenter, new Color(0.95f, 0.25f, 0.25f), new Vector2(0f, 130f), new Vector2(450f, 60f));
            Text goScoreText = CreateUIText("ScoreText", gameOver.transform, "Coins: 0 / 20", 24, TextAnchor.MiddleCenter, Color.white, new Vector2(0f, 55f), new Vector2(450f, 70f));
            Button retryBtn = CreateUIButton("RetryButton", gameOver.transform, "RESTART LEVEL", new Vector2(0.5f, 0.5f), new Vector2(0f, -40f), new Vector2(290f, 60f), new Color(0.20f, 0.68f, 0.30f), 22);
            Button goMenuBtn = CreateUIButton("MenuButton", gameOver.transform, "MAIN MENU", new Vector2(0.5f, 0.5f), new Vector2(0f, -120f), new Vector2(260f, 55f), new Color(0.42f, 0.45f, 0.55f), 20);
            gameOver.SetActive(false);

            // 5. Level Complete Panel
            GameObject levelComp = CreateCardPanel("LevelComplete_Panel", canvasGo.transform, new Vector2(640f, 480f));
            CreateUIText("Title", levelComp.transform, "SANCTUARY CONQUERED!", 46, TextAnchor.MiddleCenter, new Color(1f, 0.85f, 0.15f), new Vector2(0f, 145f), new Vector2(540f, 70f));
            Text compScoreText = CreateUIText("ScoreText", levelComp.transform, "You ascended through all 5 Sky Zones!\nTotal Coins Collected: 20 / 20", 24, TextAnchor.MiddleCenter, Color.white, new Vector2(0f, 55f), new Vector2(540f, 80f));
            Button playAgainBtn = CreateUIButton("PlayAgainButton", levelComp.transform, "PLAY AGAIN", new Vector2(0.5f, 0.5f), new Vector2(0f, -45f), new Vector2(280f, 60f), new Color(0.18f, 0.68f, 0.28f), 24);
            Button compMenuBtn = CreateUIButton("MenuButton", levelComp.transform, "MAIN MENU", new Vector2(0.5f, 0.5f), new Vector2(0f, -125f), new Vector2(280f, 55f), new Color(0.42f, 0.45f, 0.55f), 20);
            levelComp.SetActive(false);

            uiManager.SetupReferences(
                hud, mainMenu, pauseMenu, gameOver, levelComp,
                coinText, goScoreText, compScoreText,
                pauseBtn, playBtn, resumeBtn, pauseRestartBtn, pauseMenuBtn,
                retryBtn, goMenuBtn, playAgainBtn, compMenuBtn
            );

            return canvasGo;
        }

        private static GameObject CreateCardPanel(string name, Transform parent, Vector2 size)
        {
            GameObject panel = CreateUIPanel(name, parent, new Color(0.06f, 0.09f, 0.16f, 0.94f));
            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;
            return panel;
        }

        private static GameObject CreateUIPanel(string name, Transform parent, Color bgColor)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            Image img = go.AddComponent<Image>();
            img.color = bgColor;
            return go;
        }

        private static Text CreateUIText(string name, Transform parent, string content, int fontSize, TextAnchor alignment, Color color, Vector2? pos = null, Vector2? size = null)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            Text txt = go.AddComponent<Text>();
            txt.text = content;
            txt.fontSize = fontSize;
            txt.alignment = alignment;
            txt.color = color;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Resources.GetBuiltinResource<Font>("Arial.ttf");

            RectTransform rect = go.GetComponent<RectTransform>();
            if (pos.HasValue) rect.anchoredPosition = pos.Value;
            if (size.HasValue) rect.sizeDelta = size.Value;
            return txt;
        }

        private static Button CreateUIButton(string name, Transform parent, string text, Vector2 anchor, Vector2 pos, Vector2 size, Color btnColor, int fontSize)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            Image img = go.AddComponent<Image>();
            img.color = btnColor;

            Button btn = go.AddComponent<Button>();
            ColorBlock cb = btn.colors;
            cb.highlightedColor = btnColor * 1.25f;
            cb.pressedColor = btnColor * 0.8f;
            btn.colors = cb;

            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;

            CreateUIText("Text", go.transform, text, fontSize, TextAnchor.MiddleCenter, Color.white, Vector2.zero, size);
            return btn;
        }

        #endregion

        #region Lighting & Atmosphere Configuration

        private static void ConfigureAtmosphereAndLighting(MaterialDictionary mats)
        {
            if (mats.Skybox != null)
            {
                RenderSettings.skybox = mats.Skybox;
            }

            // Balanced sunlight intensity (1.05) to prevent washed-out surfaces
            Light sun = RenderSettings.sun;
            if (sun == null)
            {
                var lights = FindObjectsByType<Light>(FindObjectsInactive.Include);
                foreach (var l in lights)
                {
                    if (l.type == LightType.Directional)
                    {
                        sun = l;
                        break;
                    }
                }
            }

            if (sun != null)
            {
                sun.color = new Color(1.0f, 0.98f, 0.92f);
                sun.intensity = 1.05f;
                sun.shadows = LightShadows.Soft;
                sun.shadowStrength = 0.65f;
                sun.shadowBias = 0.05f;
                sun.transform.rotation = Quaternion.Euler(46f, -38f, 0f);
            }

            // Atmospheric fog for rich fantasy depth
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.68f, 0.82f, 0.96f);
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.0038f;

            // Vibrant ambient Trilight fill
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.62f, 0.80f, 0.98f);
            RenderSettings.ambientEquatorColor = new Color(0.75f, 0.82f, 0.90f);
            RenderSettings.ambientGroundColor = new Color(0.36f, 0.40f, 0.46f);
        }

        #endregion

        private class MaterialDictionary
        {
            public Material Grass;
            public Material GrassDark;
            public Material Rock;
            public Material RockDark;
            public Material RockBrown;
            public Material StonePath;
            public Material Wood;
            public Material WoodDark;
            public Material WoodBridge;
            public Material Soil;
            public Material Gold;
            public Material GoldBody;
            public Material GoldRim;
            public Material GoldSymbol;
            public Material Cloud;
            public Material CanopyGreen;
            public Material CanopyAutumn;
            public Material CanopyPink;
            public Material Trunk;
            public Material FlowerPink;
            public Material FlowerYellow;
            public Material MushroomCap;
            public Material MushroomStem;
            public Material Portal;
            public Material RuneGlow;
            public Material PlayerSuit;
            public Material PlayerVisor;
            public Material PlayerWhite;
            public Material Skybox;

            public Material CrystalBlue;
            public Material CrystalPurple;
            public Material Water;
            public Material ObstacleSweeper;
            public Material DisappearingWarn;
            public Material BouncePad;
            public Material MarbleWhite;
            public Material LandingPad;
            public Material ParticleSparkle;
        }
#endif
    }
}
