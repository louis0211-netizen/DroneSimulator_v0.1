using UnityEngine;
using UnityEngine.EventSystems;

namespace DroneSimulator.Input
{
    public sealed class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform handle;
        [SerializeField] private float radiusPixels = 92f;
        [SerializeField] private bool springToCenterX = true;
        [SerializeField] private bool springToCenterY = true;

        private RectTransform rectTransform;
        private Canvas canvas;
        private Vector2 value;

        public Vector2 Value => value;

        public void Configure(RectTransform newHandle, float newRadiusPixels, bool newSpringToCenterX, bool newSpringToCenterY)
        {
            handle = newHandle;
            radiusPixels = Mathf.Max(1f, newRadiusPixels);
            springToCenterX = newSpringToCenterX;
            springToCenterY = newSpringToCenterY;
            value = Vector2.zero;
            UpdateHandle();
        }

        private void Awake()
        {
            rectTransform = (RectTransform)transform;
            canvas = GetComponentInParent<Canvas>();
            UpdateHandle();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            UpdateValue(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            UpdateValue(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (springToCenterX)
            {
                value.x = 0f;
            }

            if (springToCenterY)
            {
                value.y = 0f;
            }

            UpdateHandle();
        }

        private void UpdateValue(PointerEventData eventData)
        {
            UnityEngine.Camera camera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, camera, out Vector2 localPoint);
            value = Vector2.ClampMagnitude(localPoint / Mathf.Max(1f, radiusPixels), 1f);
            UpdateHandle();
        }

        private void UpdateHandle()
        {
            if (handle != null)
            {
                handle.anchoredPosition = value * radiusPixels;
            }
        }
    }
}
