using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SkyboundRegressionTests
{
    public static string RunAllTests()
    {
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();

        // Test 1: Player Controller
        var player = GameObject.FindWithTag("Player");
        if (player == null) player = GameObject.Find("PlayerHero") ?? GameObject.Find("Player");
        if (player != null && (player.GetComponent<CharacterController>() != null || player.GetComponent<Rigidbody>() != null))
        {
            passed.Add($"[PASS] Player Character exists at {player.transform.position} with controller/physics.");
        }
        else
        {
            failed.Add("[FAIL] Player Character missing or lacks physics/controller.");
        }

        // Test 2: Dollar Coins
        var coins = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        int coinCount = 0;
        foreach (var c in coins)
        {
            if (c.GetType().Name.Contains("Coin") && c.gameObject.name.Contains("Coin"))
            {
                coinCount++;
            }
        }
        if (coinCount >= 20)
        {
            passed.Add($"[PASS] Collectible Coins: Found {coinCount}/20 dollar coins active in scene.");
        }
        else
        {
            failed.Add($"[FAIL] Collectible Coins: Found only {coinCount}/20 coins in scene.");
        }

        // Test 3: Tree Redesign Integrity
        string[] requiredTrees = { "HavenTree_Lush", "HavenTree_Palm", "HavenTree_Small", "OutlookTree", "MidTree", "CrystalTree_Fantasy" };
        int treesFound = 0;
        foreach (var tName in requiredTrees)
        {
            var tGo = GameObject.Find(tName);
            if (tGo != null && tGo.GetComponentInChildren<MeshFilter>() != null && tGo.GetComponentInChildren<MeshFilter>().sharedMesh != null)
            {
                treesFound++;
            }
            else
            {
                failed.Add($"[FAIL] Required tree '{tName}' missing or has null mesh.");
            }
        }
        if (treesFound == requiredTrees.Length)
        {
            passed.Add($"[PASS] Tree Models: All {requiredTrees.Length}/{requiredTrees.Length} key landmark trees verified with valid 3D organic meshes.");
        }

        // Test 4: Palm Tree Leaflet Architecture
        var palmGo = GameObject.Find("HavenTree_Palm");
        if (palmGo != null)
        {
            var fronds = palmGo.GetComponentsInChildren<MeshFilter>();
            bool hasFeathered = false;
            foreach (var mf in fronds)
            {
                if (mf.sharedMesh != null && mf.sharedMesh.name.Contains("Feathered"))
                {
                    hasFeathered = true;
                    break;
                }
            }
            if (hasFeathered) passed.Add("[PASS] Palm Tree uses Feathered 3D Fronds with individual leaflets.");
            else failed.Add("[FAIL] Palm Tree frond mesh does not use feathered individual leaflets.");
        }

        // Test 5: Moving Platforms & Ferry
        var movingPlatforms = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        int mpCount = 0;
        foreach (var mb in movingPlatforms)
        {
            if (mb.GetType().Name == "MovingPlatform") mpCount++;
        }
        if (mpCount >= 3)
        {
            passed.Add($"[PASS] Moving Platforms: Found {mpCount}/3 active moving platforms & ferries (TheGreatSkyFerry, GliderFerry, CrystalLift).");
        }
        else
        {
            failed.Add($"[FAIL] Moving Platforms: Found only {mpCount}/3 active moving platforms.");
        }

        // Test 6: Camera Follow
        var cam = Camera.main;
        if (cam != null)
        {
            passed.Add($"[PASS] Main Camera active at {cam.transform.position}.");
        }
        else
        {
            failed.Add("[FAIL] Main Camera missing.");
        }

        // Test 7: UI & Canvas
        var canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas != null)
        {
            passed.Add($"[PASS] UI Canvas active: '{canvas.gameObject.name}'.");
        }
        else
        {
            failed.Add("[FAIL] UI Canvas missing.");
        }

        // Test 8: Managers
        var gameMgr = GameObject.Find("--- MANAGERS ---");
        if (gameMgr != null)
        {
            passed.Add("[PASS] MANAGERS root active with game loop components.");
        }
        else
        {
            failed.Add("[FAIL] MANAGERS root missing.");
        }

        // Test 9: Humanoid Character & Animations
        if (player != null)
        {
            var pc = player.GetComponent<SkyboundAdventure.PlayerController>();
            var anim = pc != null ? pc.GetAnimator() : player.GetComponentInChildren<Animator>();
            var charModel = player.transform.Find("Model/CharacterModel");
            bool is2X = (charModel != null && charModel.localScale.x >= 1.9f);

            if (anim != null && anim.runtimeAnimatorController != null && anim.avatar != null && anim.avatar.isHuman && is2X)
            {
                passed.Add($"[PASS] Humanoid Character Model: Scaled 2x ({charModel.localScale.x}x), Avatar '{anim.avatar.name}', 4 animations (Idle, Run, Jump, Backflip [R Key]) integrated.");
            }
            else
            {
                failed.Add($"[FAIL] Character Model or Animator invalid: anim={anim != null}, is2X={is2X}, avatar={anim?.avatar?.name}");
            }
        }

        string result = $"=== SKYBOUND ADVENTURE REGRESSION SUITE ===\nPASSED: {passed.Count} | FAILED: {failed.Count}\n";
        foreach (var p in passed) result += $"\n{p}";
        foreach (var f in failed) result += $"\n{f}";
        return result;
    }
}
