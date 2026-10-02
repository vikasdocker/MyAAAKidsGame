using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Dab.EditorTools.Playtest
{
    /// <summary>
    /// Puts the playtest scene into the one shape it needs: a single GameObject
    /// carrying <c>CreaturePlaytestBootstrap</c>, which builds the camera, light,
    /// creature, material, and input bindings at runtime.
    ///
    /// The alternative — authoring the hierarchy as scene objects — needs .meta
    /// GUIDs that Unity has not generated yet on a project's first open, which
    /// AGENTS.md forbids hand-authoring. Driving the scene from one component
    /// sidesteps that entirely and keeps the rig reviewable as code.
    ///
    /// Run from the menu, or headless via -executeMethod.
    /// </summary>
    public static class PlaytestSceneSetup
    {
        private const string ScenePath = "Assets/Scenes/Playtest.unity";
        private const string RootName = "Playtest";

        [MenuItem("Dab/Playtest/Rebuild Playtest Scene")]
        public static void RebuildFromMenu()
        {
            Rebuild();
        }

        /// <summary>
        /// Entry point for <c>-executeMethod</c>. Editor-scoped, so the menu item
        /// and the CLI path share one implementation rather than drifting.
        /// </summary>
        public static void Rebuild()
        {
            var scene = EditorSceneManager.NewScene(
                NewSceneSetup.EmptyScene,
                NewSceneMode.Single);

            var root = new GameObject(RootName);

            // Left enabled on purpose. Awake is what builds the rig, and Unity
            // does not call Awake on a disabled component, so disabling it here
            // would produce a scene that renders an empty room forever. Awake
            // does not run in edit mode for a scene object, so enabling it is
            // also safe at author time: nothing is built until play mode starts.
            root.AddComponent<Dab.Runtime.Playtest.CreaturePlaytestBootstrap>();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);

            RegisterSceneInBuildSettings();

            Debug.Log($"[PlaytestSceneSetup] Wrote {ScenePath} with a single " +
                      $"'{RootName}' GameObject holding CreaturePlaytestBootstrap. " +
                      "The rig itself is built by Awake at runtime.");
        }

        /// <summary>
        /// Puts the playtest scene first in the build settings list.
        ///
        /// SceneManager.LoadSceneAsync only resolves scenes registered in build
        /// settings, so the playmode tests fail to load the scene without this.
        /// The GUID comes from the AssetDatabase rather than being written into
        /// ProjectSettings/EditorBuildSettings.asset by hand: AGENTS.md forbids
        /// hand-authored GUIDs, and a hand-written one would silently mismatch
        /// the asset it claims to point at.
        /// </summary>
        private static void RegisterSceneInBuildSettings()
        {
            var guid = AssetDatabase.AssetPathToGUID(ScenePath);
            if (string.IsNullOrEmpty(guid))
            {
                Debug.LogError(
                    $"[PlaytestSceneSetup] Could not resolve a GUID for {ScenePath}. " +
                    "The scene will not load in play mode tests.");
                return;
            }

            var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>
            {
                new(ScenePath, true)
            };

            // Keep any other registered scenes, but demote them to disabled so
            // index 0 stays the playtest scene without deleting existing entries.
            foreach (var existing in EditorBuildSettings.scenes)
            {
                if (existing.path != ScenePath)
                {
                    scenes.Add(new EditorBuildSettingsScene(existing.path, false));
                }
            }

            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
