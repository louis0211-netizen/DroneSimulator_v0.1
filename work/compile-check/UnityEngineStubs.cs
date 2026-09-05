using System;

namespace UnityEngine
{
    public class Object
    {
        public static T FindFirstObjectByType<T>() where T : Object => default(T);
        public static T[] FindObjectsByType<T>(FindObjectsSortMode sortMode) where T : Object => new T[0];
        public static void Destroy(Object obj) { }
    }
    public class Component : Object
    {
        public Transform transform { get; } = new Transform();
        public GameObject gameObject { get; } = new GameObject();
        public T GetComponent<T>() { return default(T); }
        public T GetComponentInParent<T>() { return default(T); }
    }

    public class MonoBehaviour : Component { }
    public class ScriptableObject : Object
    {
        public static T CreateInstance<T>() where T : ScriptableObject, new() => new T();
    }

    [AttributeUsage(AttributeTargets.Field)] public sealed class SerializeField : Attribute { }
    [AttributeUsage(AttributeTargets.Field)] public sealed class HeaderAttribute : Attribute { public HeaderAttribute(string header) { } }
    [AttributeUsage(AttributeTargets.Field)] public sealed class RangeAttribute : Attribute { public RangeAttribute(float min, float max) { } }
    [AttributeUsage(AttributeTargets.Field)] public sealed class MinAttribute : Attribute { public MinAttribute(float min) { } }
    [AttributeUsage(AttributeTargets.Class)] public sealed class CreateAssetMenuAttribute : Attribute { public string menuName; public string fileName; }
    [AttributeUsage(AttributeTargets.Class)] public sealed class DisallowMultipleComponent : Attribute { }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)] public sealed class RequireComponent : Attribute { public RequireComponent(Type type) { } }

    public struct Vector2
    {
        public float x;
        public float y;
        public Vector2(float x, float y) { this.x = x; this.y = y; }
        public static Vector2 zero => new Vector2(0f, 0f);
        public static Vector2 one => new Vector2(1f, 1f);
        public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.x + b.x, a.y + b.y);
        public static Vector2 operator *(Vector2 value, float scalar) => new Vector2(value.x * scalar, value.y * scalar);
        public static Vector2 operator /(Vector2 value, float scalar) => new Vector2(value.x / scalar, value.y / scalar);
        public static Vector2 ClampMagnitude(Vector2 vector, float maxLength) => vector;
    }

    public struct Vector2Int
    {
        public int x;
        public int y;
        public Vector2Int(int x, int y) { this.x = x; this.y = y; }
    }

    public struct Rect
    {
        public Vector2 position;
        public Vector2 size;
        public float width => size.x;
        public float height => size.y;
        public Vector2 center => position + (size * 0.5f);
        public static bool operator ==(Rect a, Rect b) => true;
        public static bool operator !=(Rect a, Rect b) => false;
        public override bool Equals(object obj) => obj is Rect;
        public override int GetHashCode() => 0;
    }

    public struct Vector3
    {
        public float x;
        public float y;
        public float z;
        public Vector3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }
        public float magnitude => 0f;
        public Vector3 normalized => this;
        public static Vector3 zero => new Vector3(0f, 0f, 0f);
        public static Vector3 up => new Vector3(0f, 1f, 0f);
        public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        public static Vector3 operator -(Vector3 value) => new Vector3(-value.x, -value.y, -value.z);
        public static Vector3 operator *(Vector3 value, float scalar) => new Vector3(value.x * scalar, value.y * scalar, value.z * scalar);
        public static bool operator ==(Vector3 a, Vector3 b) => true;
        public static bool operator !=(Vector3 a, Vector3 b) => false;
        public override bool Equals(object obj) => obj is Vector3;
        public override int GetHashCode() => 0;
        public static Vector3 Lerp(Vector3 a, Vector3 b, float t) => b;
    }

    public struct Quaternion
    {
        public static Quaternion identity => new Quaternion();
        public static Quaternion Euler(float x, float y, float z) => new Quaternion();
        public static Quaternion LookRotation(Vector3 forward, Vector3 upwards) => new Quaternion();
    }

    public static class Mathf
    {
        public const float PI = 3.14159265359f;
        public const float Deg2Rad = 0.0174532924f;
        public static float Abs(float value) => Math.Abs(value);
        public static float Clamp(float value, float min, float max) => Math.Max(min, Math.Min(max, value));
        public static float Clamp01(float value) => Clamp(value, 0f, 1f);
        public static float Cos(float value) => (float)Math.Cos(value);
        public static float Sin(float value) => (float)Math.Sin(value);
        public static float Min(float a, float b) => Math.Min(a, b);
        public static float Max(float a, float b) => Math.Max(a, b);
        public static int Max(int a, int b) => Math.Max(a, b);
        public static float MoveTowards(float current, float target, float maxDelta) => target;
        public static float Exp(float power) => (float)Math.Exp(power);
        public static float Lerp(float a, float b, float t) => a + ((b - a) * t);
        public static float Sign(float value) => value >= 0f ? 1f : -1f;
    }

    public class Transform : Component
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector3 localScale;
        public Vector3 localEulerAngles;
        public Vector3 TransformDirection(Vector3 direction) => direction;
        public Vector3 InverseTransformDirection(Vector3 direction) => direction;
        public Vector3 TransformPoint(Vector3 point) => point;
        public void SetPositionAndRotation(Vector3 position, Quaternion rotation) { this.position = position; this.rotation = rotation; }
        public void SetParent(Transform parent) { }
    }

    public enum ForceMode { Force }

    public class Rigidbody : Component
    {
        public float mass;
        public bool useGravity;
        public float maxAngularVelocity;
        public float linearDamping;
        public float angularDamping;
        public RigidbodyInterpolation interpolation;
        public CollisionDetectionMode collisionDetectionMode;
        public Vector3 angularVelocity;
        public Vector3 linearVelocity;
        public void AddForceAtPosition(Vector3 force, Vector3 position, ForceMode mode) { }
        public void AddRelativeTorque(Vector3 torque, ForceMode mode) { }
        public void AddForce(Vector3 force, ForceMode mode) { }
        public void AddTorque(Vector3 torque, ForceMode mode) { }
    }

    public enum RigidbodyInterpolation { Interpolate }
    public enum CollisionDetectionMode { ContinuousDynamic }

    public class Collision
    {
        public Vector3 relativeVelocity;
    }

    public class GameObject : Object
    {
        public string name;
        public Transform transform { get; } = new Transform();
        public GameObject() { }
        public GameObject(string name) { this.name = name; }
        public GameObject(string name, params Type[] components) { this.name = name; }
        public static GameObject CreatePrimitive(PrimitiveType type) => new GameObject();
        public T GetComponent<T>() { return default(T); }
        public T AddComponent<T>() where T : new() { return new T(); }
    }

    public enum PrimitiveType { Cube, Sphere, Capsule, Cylinder }
    public class Shader : Object { public static Shader Find(string name) => new Shader(); }
    public class Material : Object
    {
        public Color color;
        public Material() { }
        public Material(Shader shader) { }
    }
    public class Renderer : Component { public Material sharedMaterial; }
    public class MeshRenderer : Renderer { }
    public class MeshFilter : Component { public Mesh sharedMesh; }
    public class MeshCollider : Component { public Mesh sharedMesh; }
    public class Mesh : Object
    {
        public Vector3[] vertices;
        public int[] triangles;
        public void RecalculateNormals() { }
        public void RecalculateBounds() { }
    }

    public class Camera : Component
    {
        public bool enabled;
        public float fieldOfView;
        public CameraClearFlags clearFlags;
        public Color backgroundColor;
    }

    public enum CameraClearFlags { SolidColor }
    public enum FogMode { ExponentialSquared }
    public enum FindObjectsSortMode { None }

    public static class RenderSettings
    {
        public static bool fog;
        public static FogMode fogMode;
        public static Color fogColor;
        public static float fogDensity;
        public static Color ambientLight;
    }

    public enum LightType { Directional }

    public class Light : Component
    {
        public LightType type;
        public float intensity;
        public Color color;
    }

    public class Canvas : Component
    {
        public RenderMode renderMode;
        public Camera worldCamera;
    }

    public enum RenderMode { ScreenSpaceOverlay }

    public class RectTransform : Transform
    {
        public Vector2 anchoredPosition;
        public Vector2 anchorMin;
        public Vector2 anchorMax;
        public Vector2 pivot;
        public Vector2 sizeDelta;
        public Vector2 offsetMin;
        public Vector2 offsetMax;
        public Rect rect;
    }

    public static class RectTransformUtility
    {
        public static bool ScreenPointToLocalPointInRectangle(RectTransform rect, Vector2 screenPoint, Camera camera, out Vector2 localPoint)
        {
            localPoint = screenPoint;
            return true;
        }
    }

    public static class Time
    {
        public static float time;
        public static float deltaTime;
        public static float fixedDeltaTime;
    }

    public enum ScreenOrientation { LandscapeLeft, LandscapeRight }

    public static class Screen
    {
        public static ScreenOrientation orientation;
        public static int sleepTimeout;
        public static int width;
        public static int height;
        public static Rect safeArea;
    }

    public static class SleepTimeout
    {
        public const int NeverSleep = -1;
    }

    public static class Application
    {
        public static int targetFrameRate;
        public static bool isBatchMode;
    }

    public static class QualitySettings
    {
        public static int vSyncCount;
    }

    public enum KeyCode { W, S, A, D, Space, R, C, F }

    public enum UIOrientation { LandscapeLeft }

    public static class Input
    {
        public static bool GetKey(KeyCode key) => false;
        public static bool GetKeyDown(KeyCode key) => false;
        public static float GetAxisRaw(string axisName) => 0f;
    }

    public static class Debug
    {
        public static void LogWarning(string message) { }
    }

    public struct Color
    {
        public float r;
        public float g;
        public float b;
        public float a;
        public Color(float r, float g, float b, float a = 1f) { this.r = r; this.g = g; this.b = b; this.a = a; }
        public static Color white => new Color(1f, 1f, 1f, 1f);
        public static Color gray => new Color(0.5f, 0.5f, 0.5f, 1f);
    }

    public class Font : Object { }

    public static class Resources
    {
        public static T GetBuiltinResource<T>(string path) where T : Object, new() => new T();
    }

    public enum TextAnchor { MiddleLeft, MiddleCenter }
}

namespace UnityEngine.EventSystems
{
    public class EventSystem : UnityEngine.Component { }
    public class StandaloneInputModule : UnityEngine.Component { }
    public interface IPointerDownHandler { void OnPointerDown(PointerEventData eventData); }
    public interface IDragHandler { void OnDrag(PointerEventData eventData); }
    public interface IPointerUpHandler { void OnPointerUp(PointerEventData eventData); }

    public class PointerEventData
    {
        public UnityEngine.Vector2 position;
    }
}

namespace UnityEngine.UI
{
    public class Graphic : UnityEngine.Component
    {
        public UnityEngine.Color color;
        public UnityEngine.RectTransform rectTransform { get; } = new UnityEngine.RectTransform();
        public void SetVerticesDirty() { }
        protected virtual void OnPopulateMesh(VertexHelper vh) { }
    }

    public struct UIVertex
    {
        public UnityEngine.Color color;
        public UnityEngine.Vector2 position;
        public static UIVertex simpleVert => new UIVertex();
    }

    public class VertexHelper
    {
        public void Clear() { }
        public void AddVert(UIVertex vertex) { }
        public void AddTriangle(int a, int b, int c) { }
    }

    public class GraphicRaycaster : UnityEngine.Component { }

    public class CanvasScaler : UnityEngine.Component
    {
        public ScaleMode uiScaleMode;
        public UnityEngine.Vector2 referenceResolution;
        public float matchWidthOrHeight;

        public enum ScaleMode { ScaleWithScreenSize }
    }

    public class Image : UnityEngine.Component
    {
        public UnityEngine.Color color;
    }

    public class Button : UnityEngine.Component
    {
        public UnityEngine.Events.UnityEvent onClick = new UnityEngine.Events.UnityEvent();
    }

    public class Text : UnityEngine.Component
    {
        public string text;
        public UnityEngine.Font font;
        public int fontSize;
        public UnityEngine.Color color;
        public UnityEngine.TextAnchor alignment;
        public bool raycastTarget;
    }
}

namespace UnityEngine.Events
{
    public delegate void UnityAction();

    public class UnityEvent
    {
        public void Invoke() { }
    }
}

namespace UnityEngine.SceneManagement
{
    public struct Scene { }
}
