using UnityEngine;

namespace DroneSimulator.Camera
{
    public sealed class DroneCameraRig : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Camera fpvCamera;
        [SerializeField] private UnityEngine.Camera chaseCamera;
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 chaseOffset = new Vector3(0f, 2.2f, -5.5f);
        [SerializeField] private float chaseSharpness = 8f;

        private bool useFpv = true;

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            if (chaseCamera != null)
            {
                Vector3 desiredPosition = target.TransformPoint(chaseOffset);
                chaseCamera.transform.position = Vector3.Lerp(chaseCamera.transform.position, desiredPosition, 1f - Mathf.Exp(-chaseSharpness * Time.deltaTime));
                chaseCamera.transform.rotation = Quaternion.LookRotation(target.position - chaseCamera.transform.position, Vector3.up);
            }
        }

        public void ToggleCamera()
        {
            useFpv = !useFpv;
            if (fpvCamera != null)
            {
                fpvCamera.enabled = useFpv;
            }

            if (chaseCamera != null)
            {
                chaseCamera.enabled = !useFpv;
            }
        }
    }
}

