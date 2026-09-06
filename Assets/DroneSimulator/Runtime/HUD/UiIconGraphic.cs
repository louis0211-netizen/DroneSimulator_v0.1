using UnityEngine;
using UnityEngine.UI;

namespace DroneSimulator.HUD
{
    public enum UiIconType
    {
        Power,
        Reset,
        Camera,
        FlightMode,
        City,
        Forest,
        Mountain,
        Beach,
        MotionSensor,
        Calibrate
    }

    public sealed class UiIconGraphic : Graphic
    {
        [SerializeField] private UiIconType iconType;

        public UiIconType IconType
        {
            get => iconType;
            set
            {
                iconType = value;
                SetVerticesDirty();
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            Rect rect = rectTransform.rect;
            Vector2 center = rect.center;
            float size = Mathf.Min(rect.width, rect.height);
            float unit = size * 0.5f;

            switch (iconType)
            {
                case UiIconType.Reset:
                    AddArc(vh, center, unit * 0.55f, unit * 0.38f, 35f, 320f, 34);
                    AddTriangle(vh, center + Direction(30f) * unit * 0.56f, unit * 0.18f, -25f);
                    break;
                case UiIconType.Camera:
                    AddRect(vh, center + new Vector2(0f, -unit * 0.05f), new Vector2(unit * 1.05f, unit * 0.68f));
                    AddRect(vh, center + new Vector2(-unit * 0.25f, unit * 0.35f), new Vector2(unit * 0.38f, unit * 0.16f));
                    AddRing(vh, center, unit * 0.26f, unit * 0.13f, 28);
                    break;
                case UiIconType.FlightMode:
                    AddTriangle(vh, center + new Vector2(0f, unit * 0.12f), unit * 0.55f, 0f);
                    AddRect(vh, center + new Vector2(0f, -unit * 0.37f), new Vector2(unit * 0.18f, unit * 0.4f));
                    break;
                case UiIconType.City:
                    AddRect(vh, center + new Vector2(-unit * 0.34f, -unit * 0.08f), new Vector2(unit * 0.26f, unit * 0.9f));
                    AddRect(vh, center + new Vector2(0f, -unit * 0.18f), new Vector2(unit * 0.25f, unit * 0.7f));
                    AddRect(vh, center + new Vector2(unit * 0.34f, 0f), new Vector2(unit * 0.26f, unit * 1.05f));
                    break;
                case UiIconType.Forest:
                    AddTriangle(vh, center + new Vector2(-unit * 0.26f, unit * 0.18f), unit * 0.45f, 0f);
                    AddTriangle(vh, center + new Vector2(unit * 0.24f, unit * 0.1f), unit * 0.55f, 0f);
                    AddRect(vh, center + new Vector2(-unit * 0.26f, -unit * 0.5f), new Vector2(unit * 0.12f, unit * 0.3f));
                    AddRect(vh, center + new Vector2(unit * 0.24f, -unit * 0.5f), new Vector2(unit * 0.12f, unit * 0.3f));
                    break;
                case UiIconType.Mountain:
                    AddTriangle(vh, center + new Vector2(-unit * 0.18f, -unit * 0.05f), unit * 0.7f, 0f);
                    AddTriangle(vh, center + new Vector2(unit * 0.32f, -unit * 0.14f), unit * 0.48f, 0f);
                    break;
                case UiIconType.Beach:
                    AddCircle(vh, center + new Vector2(-unit * 0.36f, unit * 0.32f), unit * 0.18f, 18);
                    AddArc(vh, center + new Vector2(unit * 0.1f, -unit * 0.22f), unit * 0.68f, unit * 0.56f, 196f, 344f, 28);
                    AddArc(vh, center + new Vector2(unit * 0.18f, -unit * 0.42f), unit * 0.55f, unit * 0.47f, 200f, 342f, 24);
                    break;
                case UiIconType.MotionSensor:
                    AddArc(vh, center, unit * 0.68f, unit * 0.56f, 210f, 330f, 18);
                    AddArc(vh, center, unit * 0.46f, unit * 0.36f, 215f, 325f, 18);
                    AddCircle(vh, center + new Vector2(0f, -unit * 0.22f), unit * 0.1f, 14);
                    AddRect(vh, center + new Vector2(0f, unit * 0.22f), new Vector2(unit * 0.12f, unit * 0.48f));
                    break;
                case UiIconType.Calibrate:
                    AddRing(vh, center, unit * 0.58f, unit * 0.46f, 32);
                    AddRect(vh, center, new Vector2(unit * 0.85f, unit * 0.08f));
                    AddRect(vh, center, new Vector2(unit * 0.08f, unit * 0.85f));
                    AddCircle(vh, center, unit * 0.12f, 14);
                    break;
                default:
                    AddRing(vh, center, unit * 0.58f, unit * 0.43f, 36);
                    AddRect(vh, center + new Vector2(0f, unit * 0.22f), new Vector2(unit * 0.18f, unit * 0.66f));
                    break;
            }
        }

        private void AddRect(VertexHelper vh, Vector2 center, Vector2 size)
        {
            int start = CurrentVertex(vh);
            Vector2 half = size * 0.5f;
            AddVertex(vh, center + new Vector2(-half.x, -half.y));
            AddVertex(vh, center + new Vector2(-half.x, half.y));
            AddVertex(vh, center + new Vector2(half.x, half.y));
            AddVertex(vh, center + new Vector2(half.x, -half.y));
            vh.AddTriangle(start, start + 1, start + 2);
            vh.AddTriangle(start, start + 2, start + 3);
        }

        private void AddTriangle(VertexHelper vh, Vector2 center, float radius, float rotationDegrees)
        {
            int start = CurrentVertex(vh);
            AddVertex(vh, center + Direction(rotationDegrees + 90f) * radius);
            AddVertex(vh, center + Direction(rotationDegrees + 230f) * radius);
            AddVertex(vh, center + Direction(rotationDegrees + 310f) * radius);
            vh.AddTriangle(start, start + 1, start + 2);
        }

        private void AddCircle(VertexHelper vh, Vector2 center, float radius, int segments)
        {
            int start = CurrentVertex(vh);
            AddVertex(vh, center);

            int safeSegments = Mathf.Max(12, segments);
            for (int i = 0; i <= safeSegments; i++)
            {
                AddVertex(vh, center + Direction(i * 360f / safeSegments) * radius);
            }

            for (int i = 1; i <= safeSegments; i++)
            {
                vh.AddTriangle(start, start + i, start + i + 1);
            }
        }

        private void AddRing(VertexHelper vh, Vector2 center, float outer, float inner, int segments)
        {
            AddArc(vh, center, outer, inner, 0f, 360f, segments);
        }

        private void AddArc(VertexHelper vh, Vector2 center, float outer, float inner, float startDegrees, float endDegrees, int segments)
        {
            int start = CurrentVertex(vh);
            int safeSegments = Mathf.Max(8, segments);

            for (int i = 0; i <= safeSegments; i++)
            {
                float t = i / (float)safeSegments;
                float angle = Mathf.Lerp(startDegrees, endDegrees, t);
                Vector2 direction = Direction(angle);
                AddVertex(vh, center + direction * outer);
                AddVertex(vh, center + direction * inner);
            }

            for (int i = 0; i < safeSegments; i++)
            {
                int a = start + i * 2;
                int b = a + 1;
                int c = a + 2;
                int d = a + 3;
                vh.AddTriangle(a, c, d);
                vh.AddTriangle(a, d, b);
            }
        }

        private void AddVertex(VertexHelper vh, Vector2 position)
        {
            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = color;
            vertex.position = position;
            vh.AddVert(vertex);
        }

        private static Vector2 Direction(float degrees)
        {
            float radians = degrees * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        }

        private static int CurrentVertex(VertexHelper vh)
        {
            return vh.currentVertCount;
        }
    }
}
