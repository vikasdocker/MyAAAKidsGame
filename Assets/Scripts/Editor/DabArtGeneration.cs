using UnityEditor;

namespace Dab.Editor
{
    /// <summary>
    /// One-shot entry point that runs every art generator. The per-runner menu
    /// items still exist for regenerating a single asset type, but CI and a
    /// first-time checkout call this once.
    ///
    /// Headless: -executeMethod Dab.Editor.DabArtGeneration.GenerateAllArt
    /// </summary>
    public static class DabArtGeneration
    {
        [MenuItem("Tools/Dab/Art/Generate All Art")]
        public static void GenerateAllArt()
        {
            CreatureFXAssetGenerator.GenerateAll();
            CreatureAnimatorControllerGenerator.GenerateAll();
        }
    }
}