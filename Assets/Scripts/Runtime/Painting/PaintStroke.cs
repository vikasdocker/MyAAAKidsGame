using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dab.Runtime.Painting
{
    [Serializable]
    public struct PaintSample
    {
        public Vector2 Uv;
        public Color Color;

        public PaintSample(Vector2 uv, Color color)
        {
            Uv = uv;
            Color = color;
        }
    }

    [Serializable]
    public sealed class PaintStroke
    {
        public List<PaintSample> Samples = new List<PaintSample>(32);
    }
}
