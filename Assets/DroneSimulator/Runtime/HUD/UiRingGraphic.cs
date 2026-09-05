using UnityEngine;
using UnityEngine.UI;

namespace DroneSimulator.HUD
{
    public sealed class UiRingGraphic : Graphic
    {
        [SerializeField] private float innerRadius = 0.62f;
        [SerializeField] private int segments = 48;

        public float InnerRadius
        {
            get => innerRadius;
            set
            {
                innerRadius = Mathf.Clamp01(value);
                SetVerticesDirty();
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            Rect rect = rectTransform.rect;
            float outer = Mathf.Min(rect.width, rect.height) * 0.5f;
            float inner = outer * Mathf.Clamp01(innerRadius);
            Vector2 center = rect.center;
            int safeSegments = Mathf.Max(12, segments);

            for (int i = 0; i < safeSegments; i++)
            {
                float angle = i * Mathf.PI * 2f / safeSegments;
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                AddVertex(vh, center + direction * outer);
                AddVertex(vh, center + direction * inner);
            }

            for (int i = 0; i < safeSegments; i++)
            {
                int next = (i + 1) % safeSegments;
                int outerA = i * 2;
                int innerA = outerA + 1;
                int outerB = next * 2;
                int innerB = outerB + 1;

                vh.AddTriangle(outerA, outerB, innerB);
                vh.AddTriangle(outerA, innerB, innerA);
            }
        }

        private void AddVertex(VertexHelper vh, Vector2 position)
        {
            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = color;
            vertex.position = position;
            vh.AddVert(vertex);
        }
    }
}
