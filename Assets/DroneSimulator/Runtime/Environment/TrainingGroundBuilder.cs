using UnityEngine;

namespace DroneSimulator.Environment
{
    public sealed class TrainingGroundBuilder : MonoBehaviour
    {
        [SerializeField] private Material groundMaterial;
        [SerializeField] private Material gateMaterial;
        [SerializeField] private Vector3 groundSize = new Vector3(40f, 1f, 40f);

        private void Start()
        {
            CreateGround();
            CreateTakeoffPad();
            CreateGate(new Vector3(0f, 1.5f, 8f));
            CreateObstacle(new Vector3(5f, 1f, 5f), new Vector3(1.5f, 2f, 1.5f));
        }

        private void CreateGround()
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "Training Ground";
            ground.transform.SetParent(transform);
            ground.transform.localPosition = new Vector3(0f, -0.55f, 0f);
            ground.transform.localScale = groundSize;
            ApplyMaterial(ground, groundMaterial);
        }

        private void CreateTakeoffPad()
        {
            GameObject pad = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pad.name = "Takeoff Pad";
            pad.transform.SetParent(transform);
            pad.transform.localPosition = new Vector3(0f, 0.02f, 0f);
            pad.transform.localScale = new Vector3(2.4f, 0.04f, 2.4f);
        }

        private void CreateGate(Vector3 position)
        {
            GameObject gate = new GameObject("Gate");
            gate.transform.SetParent(transform);
            gate.transform.localPosition = position;

            CreateGatePart(gate.transform, "Left Post", new Vector3(-1.4f, 0f, 0f), new Vector3(0.12f, 3f, 0.12f));
            CreateGatePart(gate.transform, "Right Post", new Vector3(1.4f, 0f, 0f), new Vector3(0.12f, 3f, 0.12f));
            CreateGatePart(gate.transform, "Top Bar", new Vector3(0f, 1.5f, 0f), new Vector3(2.9f, 0.12f, 0.12f));
        }

        private void CreateGatePart(Transform parent, string partName, Vector3 localPosition, Vector3 scale)
        {
            GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);
            part.name = partName;
            part.transform.SetParent(parent);
            part.transform.localPosition = localPosition;
            part.transform.localScale = scale;
            ApplyMaterial(part, gateMaterial);
        }

        private void CreateObstacle(Vector3 position, Vector3 scale)
        {
            GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obstacle.name = "Obstacle";
            obstacle.transform.SetParent(transform);
            obstacle.transform.localPosition = position;
            obstacle.transform.localScale = scale;
        }

        private static void ApplyMaterial(GameObject target, Material material)
        {
            if (material == null)
            {
                return;
            }

            Renderer renderer = target.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = material;
            }
        }
    }
}

