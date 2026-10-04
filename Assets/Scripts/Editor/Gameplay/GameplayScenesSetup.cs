using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Dab.Runtime.Minigames;
using Dab.Runtime.UI;

namespace Dab.EditorTools.Gameplay
{
    /// <summary>
    /// Authoring for the two scenes that make the loop playable outside the
    /// painting demo: the main menu (SCN_MainMenu) and the playground
    /// (SCN_Playground), each a single bootstrap GameObject that builds its own
    /// content at runtime — the same "bootstrap in code, never hand-author
    /// .meta GUIDs" rule that PlaytestSceneSetup documents.
    ///
    /// Run from the Dab menu, or headless via -executeMethod.
    /// </summary>
    public static class GameplayScenesSetup
    {
        private const string MenuScenePath = "Assets/Scenes/SCN_MainMenu.unity";
        private const string MenuRootName = "SCN_MainMenu";
        private const string PlaytestScenePath = "Assets/Scenes/Playtest.unity";

        private const string PlaygroundScenePath = "Assets/Scenes/SCN_Playground.unity";
        private const string PlaygroundRootName = "SCN_Playground";

        [MenuItem("Dab/Scenes/Rebuild Main Menu Scene")]
        public static void RebuildMainMenuFromMenu()
        {
            RebuildMainMenu();
        }

        [MenuItem("Dab/Scenes/Rebuild Playground Scene")]
        public static void RebuildPlaygroundFromMenu()
        {
            RebuildPlayground();
        }

        [MenuItem("Dab/Scenes/Rebuild ALL Scenes")]
        public static void RebuildAllFromMenu()
        {
            RebuildMainMenu();
            RebuildPlayground();
        }

        /// <summary>Entry point for <c>-executeMethod</c>.</summary>
        public static void RebuildMainMenu()
        {
            BuildScene(
                MenuScenePath,
                MenuRootName,
                typeof(MainMenuBootstrap));
        }

        /// <summary>Entry point for <c>-executeMethod</c>.</summary>
        public static void RebuildPlayground()
        {
            BuildScene(
                PlaygroundScenePath,
                PlaygroundRootName,
                typeof(PlaygroundBootstrap));
        }

        /// <summary>Entry point for <c>-executeMethod</c>; rebuilds both hubs.</summary>
        public static void RebuildAll()
        {
            RebuildMainMenu();
            RebuildPlayground();
        }

        private static void BuildScene(string scenePath, string rootName, System.Type bootstrap)
        {
            var scene = EditorSceneManager.NewScene(
                NewSceneSetup.EmptyScene,
                NewSceneMode.Single);

            var root = new GameObject(rootName);

            // Left enabled deliberately: Awake builds the world, and edit mode
            // does not run Awake on scene objects, so nothing builds at author
            // time — everything waits for play mode. (See PlaytestSceneSetup.)
            root.AddComponent(bootstrap);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, scenePath);

            RegisterScenesInBuildSettings();

            Debug.Log(
                $"[GameplayScenesSetup] Wrote {scenePath} with a single '{rootName}' " +
                $"GameObject holding {bootstrap.Name}. The world is built by Awake " +
                "at runtime.");
        }

        /// <summary>
        /// Registers every scene in build settings with the playtest scene first,
        /// then the menu and the playground. SceneManager.LoadScene/Async only
        /// resolves registered scenes, so PlayMode tests that boot any hub need
        /// this. GUIDs are resolved from the AssetDatabase, never written by hand.
        /// </summary>
        private static void RegisterScenesInBuildSettings()
        {
            var scenes = new List<EditorBuildSettingsScene>();
            AppendPlayableScene(scenes, PlaytestScenePath, enabled: true);
            AppendPlayableScene(scenes, MenuScenePath, enabled: true);
            AppendPlayableScene(scenes, PlaygroundScenePath, enabled: true);

            EditorBuildSettings.scenes = scenes.ToArray();
        }

        private static void AppendPlayableScene(
            List<EditorBuildSettingsScene> scenes,
            string scenePath,
            bool enabled)
        {
            var guid = AssetDatabase.AssetPathToGUID(scenePath);
            if (string.IsNullOrEmpty(guid) || !System.IO.File.Exists(scenePath))
            {
                Debug.LogWarning(
                    $"[GameplayScenesSetup] Skipping {scenePath}: no scene asset " +
                    "found yet. Rebuild all scenes once so every hub is registered.");
                return;
            }

            var alreadyAdded = scenes.Exists(s => s.path == scenePath);
            if (!alreadyAdded)
            {
                scenes.Add(new EditorBuildSettingsScene(scenePath, enabled));
            }
        }
    }
}