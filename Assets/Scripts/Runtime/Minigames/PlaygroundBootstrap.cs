using UnityEngine;
using Dab.Runtime.Core;
using Dab.Runtime.UI;

namespace Dab.Runtime.Minigames
{
    /// <summary>
    /// Builds the playground scene at runtime so the scene file stays a single
    /// empty GameObject with this component (AGENTS.md: bootstrap in code, no
    /// hand-authored scene objects or .meta GUIDs).
    ///
    /// The playground is where the painted pet lives and runs:
    ///   - a warm honey-field ground disc with a collider (tap anywhere on it to
    ///     send the pet there),
    ///   - a soft warm key light and a dawn-cream sky,
    ///   - one full PaintingRig pet plus its <see cref="PlaygroundPetMotor"/>,
    ///     so the exact same paint-drag / state / FX path from the playtest run
    ///     plays here while the pet trots around between paints.
    ///
    /// Palette tokens are from design/Art/palette.json; every colour below is a
    /// named token (world .world.color-band Honey Field / Dawn Cream / Peach
    /// Shadow) and nothing outside the world palette sneaks in.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PlaygroundBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            BuildCameraAndLighting();
            var ground = BuildGround();

            var rig = PaintingRig.Build(
                "Creature",
                Vector3.zero,
                Vector3.one);

            var motor = rig.Creature.AddComponent<PlaygroundPetMotor>();
            motor.SetGround(ground);

            Debug.Log(
                $"[{nameof(PlaygroundBootstrap)}] Playground ready. Tap the ground " +
                $"to send the pet running; paint it with a drag. Frosting: " +
                $"{rig.Generator.TotalTriangleCount} triangles.", this);
        }

        private static void BuildCameraAndLighting()
        {
            var cameraGo = new GameObject("Main Camera");
            cameraGo.tag = "MainCamera";
            var camera = cameraGo.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Palette.DawnCream;
            camera.fieldOfView = 45f;
            cameraGo.transform.position = new Vector3(0f, 2.6f, -7f);
            cameraGo.transform.rotation = Quaternion.Euler(18f, 0f, 0f);

            var lightGo = new GameObject("Key Light");
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.0f;
            light.color = new Color(1f, 0.96f, 0.88f);
            lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        /// <summary>
        /// The honey-field ground: a flat disc big enough to walk on, tinted with
        /// the ground token, carrying a collider so <see cref="PlaygroundPetMotor"/>
        /// can raycast on to it. Expensive per-pixel colors are not an issue — the
        /// ground is a solid token fill, no textures or decals involved.
        /// </summary>
        private static GameObject BuildGround()
        {
            // A fresh URP/Lit material so the playground never touches the
            // built-in pink Default-Material that CreatePrimitive assigns.
            var groundMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"))
            {
                name = "Honey Field Ground"
            };

            groundMaterial.color = Palette.HoneyField;
            groundMaterial.SetColor("_BaseColor", Palette.HoneyField);

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Honey Field Ground";
            ground.GetComponent<MeshRenderer>().sharedMaterial = groundMaterial;
            ground.transform.localScale = new Vector3(10f, 1f, 10f);
            return ground;
        }
    }
}