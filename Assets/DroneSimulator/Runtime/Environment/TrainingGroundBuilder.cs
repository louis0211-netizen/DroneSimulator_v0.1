using UnityEngine;

namespace DroneSimulator.Environment
{
    public sealed class TrainingGroundBuilder : MonoBehaviour
    {
        [SerializeField] private Material groundMaterial;
        [SerializeField] private Material gateMaterial;
        [SerializeField] private Vector3 groundSize = new Vector3(40f, 1f, 40f);
        [SerializeField] private EnvironmentTheme initialTheme = EnvironmentTheme.City;

        private Transform activeThemeRoot;
        private EnvironmentTheme currentTheme;

        public EnvironmentTheme CurrentTheme => currentTheme;

        private void Awake()
        {
            currentTheme = initialTheme;
        }

        private void Start()
        {
            BuildTheme(currentTheme);
        }

        public void SetTheme(EnvironmentTheme theme)
        {
            if (currentTheme == theme && activeThemeRoot != null)
            {
                return;
            }

            currentTheme = theme;
            BuildTheme(theme);
        }

        public void BuildTheme(EnvironmentTheme theme)
        {
            ClearActiveTheme();
            ConfigureAtmosphere(theme);

            GameObject root = new GameObject(theme + " Training Ground");
            root.transform.SetParent(transform);
            activeThemeRoot = root.transform;

            switch (theme)
            {
                case EnvironmentTheme.Forest:
                    BuildForest(activeThemeRoot);
                    break;
                case EnvironmentTheme.Mountain:
                    BuildMountain(activeThemeRoot);
                    break;
                case EnvironmentTheme.Beach:
                    BuildBeach(activeThemeRoot);
                    break;
                default:
                    BuildCity(activeThemeRoot);
                    break;
            }
        }

        private void BuildCity(Transform root)
        {
            CreateGround(root, "Asphalt Training Field", new Color(0.13f, 0.15f, 0.16f), new Vector3(60f, 1f, 60f));
            CreateRunwayMarkings(root, new Color(0.88f, 0.82f, 0.48f));
            CreateTakeoffPad(root, new Color(0.08f, 0.1f, 0.11f), new Color(0.15f, 0.82f, 0.95f));
            CreateGate(root, new Vector3(0f, 1.5f, 9f), new Color(0.16f, 0.88f, 1f));
            CreateGate(root, new Vector3(-8f, 2.1f, 17f), new Color(1f, 0.45f, 0.16f));

            for (int i = -3; i <= 3; i++)
            {
                if (i == 0)
                {
                    continue;
                }

                float height = 3.5f + Mathf.Abs(i) * 1.25f;
                CreateObstacle(root, "City Tower", new Vector3(i * 5.5f, height * 0.5f - 0.02f, 22f + Mathf.Abs(i)), new Vector3(2.5f, height, 2.5f), new Color(0.32f, 0.39f, 0.46f));
                CreateWindowBand(root, new Vector3(i * 5.5f, height - 0.7f, 20.72f + Mathf.Abs(i)), new Vector3(1.9f, 0.18f, 0.04f), new Color(0.95f, 0.78f, 0.36f));
            }

            CreateObstacle(root, "Concrete Barrier", new Vector3(6f, 0.45f, 6f), new Vector3(2.5f, 0.9f, 0.55f), new Color(0.56f, 0.61f, 0.64f));
            CreateObstacle(root, "Container", new Vector3(-7f, 0.7f, 5f), new Vector3(3.5f, 1.4f, 1.4f), new Color(0.72f, 0.2f, 0.12f));
            CreateHorizonBlocks(root, new Color(0.18f, 0.2f, 0.23f));
        }

        private void BuildForest(Transform root)
        {
            CreateGround(root, "Forest Clearing", new Color(0.18f, 0.32f, 0.18f), new Vector3(64f, 1f, 64f));
            CreateTakeoffPad(root, new Color(0.2f, 0.18f, 0.13f), new Color(0.85f, 0.7f, 0.32f));
            CreateGate(root, new Vector3(0f, 1.5f, 9f), new Color(0.82f, 0.55f, 0.24f));
            CreateGate(root, new Vector3(8f, 1.6f, 18f), new Color(0.38f, 0.78f, 0.28f));

            for (int i = 0; i < 34; i++)
            {
                float angle = i * 137.5f * Mathf.Deg2Rad;
                float radius = 10f + (i % 7) * 3.0f;
                Vector3 pos = new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius + 8f);
                if (Mathf.Abs(pos.x) < 4f && pos.z < 16f)
                {
                    pos.x += Mathf.Sign(pos.x + 0.1f) * 6f;
                }

                float treeHeight = 2.3f + (i % 5) * 0.35f;
                CreateTree(root, pos, treeHeight);
            }

            CreateLog(root, new Vector3(-6f, 0.35f, 7f), 22f);
            CreateLog(root, new Vector3(6f, 0.35f, 12f), -38f);
            CreateObstacle(root, "Rock", new Vector3(3.8f, 0.55f, 5.5f), new Vector3(1.3f, 1.1f, 1.1f), new Color(0.36f, 0.35f, 0.31f));
        }

        private void BuildMountain(Transform root)
        {
            CreateGround(root, "Mountain Plateau", new Color(0.32f, 0.31f, 0.28f), new Vector3(70f, 1f, 70f));
            CreateTakeoffPad(root, new Color(0.21f, 0.23f, 0.25f), new Color(0.72f, 0.86f, 1f));
            CreateGate(root, new Vector3(0f, 1.8f, 10f), new Color(0.74f, 0.86f, 0.96f));
            CreateGate(root, new Vector3(-10f, 2.4f, 20f), new Color(0.95f, 0.68f, 0.28f));

            for (int i = -3; i <= 3; i++)
            {
                float height = 5f + (3 - Mathf.Abs(i)) * 2.5f;
                CreateMountainPeak(root, new Vector3(i * 8f, 0f, 28f + Mathf.Abs(i) * 3f), height);
            }

            CreateObstacle(root, "Cliff Pillar", new Vector3(6.5f, 1.8f, 8.5f), new Vector3(2.1f, 3.6f, 1.8f), new Color(0.27f, 0.28f, 0.27f));
            CreateObstacle(root, "Cliff Pillar", new Vector3(-7f, 1.45f, 12f), new Vector3(1.7f, 2.9f, 1.5f), new Color(0.34f, 0.34f, 0.31f));
            CreateObstacle(root, "Snow Marker", new Vector3(0f, 0.08f, -10f), new Vector3(16f, 0.08f, 1.5f), new Color(0.82f, 0.88f, 0.9f));
        }

        private void BuildBeach(Transform root)
        {
            CreateGround(root, "Sand Training Beach", new Color(0.73f, 0.62f, 0.42f), new Vector3(72f, 1f, 48f));
            CreateWater(root, new Vector3(0f, -0.03f, 25f), new Vector3(80f, 0.06f, 26f));
            CreateTakeoffPad(root, new Color(0.32f, 0.25f, 0.18f), new Color(0.12f, 0.75f, 0.95f));
            CreateGate(root, new Vector3(0f, 1.5f, 8f), new Color(0.12f, 0.82f, 0.95f));
            CreateGate(root, new Vector3(9f, 1.5f, 17f), new Color(0.95f, 0.78f, 0.18f));

            for (int i = 0; i < 9; i++)
            {
                CreatePalm(root, new Vector3(-18f + i * 4.5f, 0f, -13f - (i % 2) * 2f), i % 2 == 0 ? -12f : 16f);
            }

            CreateDock(root, new Vector3(-7f, 0.12f, 17f));
            CreateObstacle(root, "Beach Rock", new Vector3(6f, 0.45f, 6f), new Vector3(1.6f, 0.9f, 1.2f), new Color(0.38f, 0.35f, 0.31f));
            CreateBuoy(root, new Vector3(-4f, 0.28f, 24f), new Color(0.95f, 0.18f, 0.12f));
            CreateBuoy(root, new Vector3(4f, 0.28f, 27f), new Color(0.95f, 0.95f, 0.2f));
        }

        private void CreateGround(Transform root, string name, Color color, Vector3 scale)
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = name;
            ground.transform.SetParent(root);
            ground.transform.localPosition = new Vector3(0f, -0.55f, 0f);
            ground.transform.localScale = scale == Vector3.zero ? groundSize : scale;
            ApplyMaterial(ground, groundMaterial, color);
        }

        private void CreateTakeoffPad(Transform root, Color padColor, Color ringColor)
        {
            GameObject pad = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pad.name = "Takeoff Pad";
            pad.transform.SetParent(root);
            pad.transform.localPosition = new Vector3(0f, 0.02f, 0f);
            pad.transform.localScale = new Vector3(2.4f, 0.04f, 2.4f);
            ApplyMaterial(pad, null, padColor);

            CreateObstacle(root, "Takeoff Pad Ring North", new Vector3(0f, 0.08f, 1.25f), new Vector3(2.6f, 0.04f, 0.12f), ringColor);
            CreateObstacle(root, "Takeoff Pad Ring South", new Vector3(0f, 0.08f, -1.25f), new Vector3(2.6f, 0.04f, 0.12f), ringColor);
            CreateObstacle(root, "Takeoff Pad Ring East", new Vector3(1.25f, 0.08f, 0f), new Vector3(0.12f, 0.04f, 2.6f), ringColor);
            CreateObstacle(root, "Takeoff Pad Ring West", new Vector3(-1.25f, 0.08f, 0f), new Vector3(0.12f, 0.04f, 2.6f), ringColor);
        }

        private void CreateGate(Transform root, Vector3 position, Color color)
        {
            GameObject gate = new GameObject("Gate");
            gate.transform.SetParent(root);
            gate.transform.localPosition = position;

            CreateGatePart(gate.transform, "Left Post", new Vector3(-1.4f, 0f, 0f), new Vector3(0.12f, 3f, 0.12f), color);
            CreateGatePart(gate.transform, "Right Post", new Vector3(1.4f, 0f, 0f), new Vector3(0.12f, 3f, 0.12f), color);
            CreateGatePart(gate.transform, "Top Bar", new Vector3(0f, 1.5f, 0f), new Vector3(2.9f, 0.12f, 0.12f), color);
        }

        private void CreateGatePart(Transform parent, string partName, Vector3 localPosition, Vector3 scale, Color color)
        {
            GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);
            part.name = partName;
            part.transform.SetParent(parent);
            part.transform.localPosition = localPosition;
            part.transform.localScale = scale;
            ApplyMaterial(part, gateMaterial, color);
        }

        private void CreateObstacle(Transform root, string name, Vector3 position, Vector3 scale, Color color)
        {
            GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obstacle.name = name;
            obstacle.transform.SetParent(root);
            obstacle.transform.localPosition = position;
            obstacle.transform.localScale = scale;
            ApplyMaterial(obstacle, null, color);
        }

        private void CreateRunwayMarkings(Transform root, Color color)
        {
            CreateObstacle(root, "Runway Stripe", new Vector3(0f, 0.015f, 8f), new Vector3(0.35f, 0.05f, 20f), color);
            CreateObstacle(root, "Runway Cross Stripe", new Vector3(0f, 0.02f, -5f), new Vector3(10f, 0.05f, 0.25f), color);
        }

        private void CreateWindowBand(Transform root, Vector3 position, Vector3 scale, Color color)
        {
            CreateObstacle(root, "Lit Window Band", position, scale, color);
        }

        private void CreateHorizonBlocks(Transform root, Color color)
        {
            for (int i = -5; i <= 5; i++)
            {
                float height = 2f + Mathf.Abs(i % 4);
                CreateObstacle(root, "Distant Skyline", new Vector3(i * 7f, height * 0.5f - 0.1f, 34f), new Vector3(4f, height, 2.2f), color);
            }
        }

        private void CreateTree(Transform root, Vector3 basePosition, float height)
        {
            GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.name = "Tree Trunk";
            trunk.transform.SetParent(root);
            trunk.transform.localPosition = basePosition + new Vector3(0f, height * 0.25f, 0f);
            trunk.transform.localScale = new Vector3(0.22f, height * 0.25f, 0.22f);
            ApplyMaterial(trunk, null, new Color(0.32f, 0.18f, 0.09f));

            GameObject crown = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            crown.name = "Tree Crown";
            crown.transform.SetParent(root);
            crown.transform.localPosition = basePosition + new Vector3(0f, height * 0.78f, 0f);
            crown.transform.localScale = new Vector3(1.0f, height * 0.32f, 1.0f);
            ApplyMaterial(crown, null, new Color(0.09f, 0.34f, 0.13f));
        }

        private void CreateLog(Transform root, Vector3 position, float yaw)
        {
            GameObject log = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            log.name = "Fallen Log";
            log.transform.SetParent(root);
            log.transform.localPosition = position;
            log.transform.localRotation = Quaternion.Euler(0f, yaw, 90f);
            log.transform.localScale = new Vector3(0.35f, 2.2f, 0.35f);
            ApplyMaterial(log, null, new Color(0.34f, 0.19f, 0.09f));
        }

        private void CreateMountainPeak(Transform root, Vector3 basePosition, float height)
        {
            GameObject peak = CreateCone("Mountain Peak", 18);
            peak.name = "Mountain Peak";
            peak.transform.SetParent(root);
            peak.transform.localPosition = basePosition + new Vector3(0f, height * 0.5f - 0.1f, 0f);
            peak.transform.localScale = new Vector3(height * 0.55f, height * 0.5f, height * 0.55f);
            ApplyMaterial(peak, null, new Color(0.28f, 0.29f, 0.28f));

            GameObject snow = CreateCone("Snow Cap", 18);
            snow.name = "Snow Cap";
            snow.transform.SetParent(root);
            snow.transform.localPosition = basePosition + new Vector3(0f, height * 0.88f, 0f);
            snow.transform.localScale = new Vector3(height * 0.18f, height * 0.12f, height * 0.18f);
            ApplyMaterial(snow, null, new Color(0.86f, 0.9f, 0.92f));
        }

        private void CreateWater(Transform root, Vector3 position, Vector3 scale)
        {
            CreateObstacle(root, "Ocean Surface", position, scale, new Color(0.08f, 0.36f, 0.56f));
        }

        private void CreatePalm(Transform root, Vector3 basePosition, float leanDegrees)
        {
            GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.name = "Palm Trunk";
            trunk.transform.SetParent(root);
            trunk.transform.localPosition = basePosition + new Vector3(0f, 1.2f, 0f);
            trunk.transform.localRotation = Quaternion.Euler(0f, 0f, leanDegrees);
            trunk.transform.localScale = new Vector3(0.18f, 1.2f, 0.18f);
            ApplyMaterial(trunk, null, new Color(0.48f, 0.29f, 0.12f));

            for (int i = 0; i < 5; i++)
            {
                GameObject frond = GameObject.CreatePrimitive(PrimitiveType.Cube);
                frond.name = "Palm Frond";
                frond.transform.SetParent(root);
                frond.transform.localPosition = basePosition + new Vector3(0f, 2.45f, 0f);
                frond.transform.localRotation = Quaternion.Euler(0f, i * 72f, 18f);
                frond.transform.localScale = new Vector3(0.18f, 0.05f, 1.8f);
                ApplyMaterial(frond, null, new Color(0.08f, 0.42f, 0.18f));
            }
        }

        private void CreateDock(Transform root, Vector3 position)
        {
            CreateObstacle(root, "Wood Dock", position, new Vector3(2.2f, 0.2f, 7f), new Color(0.42f, 0.25f, 0.12f));
            CreateObstacle(root, "Dock Left Rail", position + new Vector3(-1.2f, 0.45f, 0f), new Vector3(0.12f, 0.45f, 7f), new Color(0.25f, 0.15f, 0.08f));
            CreateObstacle(root, "Dock Right Rail", position + new Vector3(1.2f, 0.45f, 0f), new Vector3(0.12f, 0.45f, 7f), new Color(0.25f, 0.15f, 0.08f));
        }

        private void CreateBuoy(Transform root, Vector3 position, Color color)
        {
            GameObject buoy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            buoy.name = "Training Buoy";
            buoy.transform.SetParent(root);
            buoy.transform.localPosition = position;
            buoy.transform.localScale = new Vector3(0.55f, 0.55f, 0.55f);
            ApplyMaterial(buoy, null, color);
        }

        private void ConfigureAtmosphere(EnvironmentTheme theme)
        {
            Color skyColor;
            Color fogColor;
            float fogDensity;

            switch (theme)
            {
                case EnvironmentTheme.Forest:
                    skyColor = new Color(0.42f, 0.56f, 0.5f);
                    fogColor = new Color(0.18f, 0.28f, 0.22f);
                    fogDensity = 0.012f;
                    RenderSettings.ambientLight = new Color(0.42f, 0.48f, 0.4f);
                    break;
                case EnvironmentTheme.Mountain:
                    skyColor = new Color(0.52f, 0.62f, 0.72f);
                    fogColor = new Color(0.5f, 0.56f, 0.6f);
                    fogDensity = 0.009f;
                    RenderSettings.ambientLight = new Color(0.5f, 0.5f, 0.48f);
                    break;
                case EnvironmentTheme.Beach:
                    skyColor = new Color(0.48f, 0.68f, 0.82f);
                    fogColor = new Color(0.62f, 0.72f, 0.74f);
                    fogDensity = 0.006f;
                    RenderSettings.ambientLight = new Color(0.62f, 0.58f, 0.48f);
                    break;
                default:
                    skyColor = new Color(0.28f, 0.38f, 0.46f);
                    fogColor = new Color(0.22f, 0.25f, 0.28f);
                    fogDensity = 0.008f;
                    RenderSettings.ambientLight = new Color(0.44f, 0.45f, 0.46f);
                    break;
            }

            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = fogColor;
            RenderSettings.fogDensity = fogDensity;

            foreach (UnityEngine.Camera camera in Object.FindObjectsByType<UnityEngine.Camera>(FindObjectsSortMode.None))
            {
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = skyColor;
            }

            EnsureSunLight(theme);
        }

        private void EnsureSunLight(EnvironmentTheme theme)
        {
            Light sun = null;
            foreach (Light light in Object.FindObjectsByType<Light>(FindObjectsSortMode.None))
            {
                if (light.type == LightType.Directional)
                {
                    sun = light;
                    break;
                }
            }

            if (sun == null)
            {
                GameObject sunObject = new GameObject("Sun");
                sunObject.transform.SetParent(transform);
                sun = sunObject.AddComponent<Light>();
                sun.type = LightType.Directional;
            }

            sun.transform.rotation = Quaternion.Euler(48f, -32f, 0f);
            sun.intensity = theme == EnvironmentTheme.Beach ? 1.15f : 0.95f;
            sun.color = theme == EnvironmentTheme.Mountain ? new Color(0.86f, 0.9f, 1f) : new Color(1f, 0.9f, 0.72f);
        }

        private void ClearActiveTheme()
        {
            if (activeThemeRoot == null)
            {
                return;
            }

            Destroy(activeThemeRoot.gameObject);
            activeThemeRoot = null;
        }

        private static GameObject CreateCone(string name, int segments)
        {
            GameObject cone = new GameObject(name);
            MeshFilter meshFilter = cone.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = cone.AddComponent<MeshRenderer>();
            MeshCollider meshCollider = cone.AddComponent<MeshCollider>();

            Mesh mesh = new Mesh();
            Vector3[] vertices = new Vector3[segments + 2];
            int[] triangles = new int[segments * 6];

            vertices[0] = new Vector3(0f, 1f, 0f);
            vertices[1] = Vector3.zero;
            for (int i = 0; i < segments; i++)
            {
                float angle = i * Mathf.PI * 2f / segments;
                vertices[i + 2] = new Vector3(Mathf.Cos(angle), -1f, Mathf.Sin(angle));
            }

            int triangleIndex = 0;
            for (int i = 0; i < segments; i++)
            {
                int current = i + 2;
                int next = i == segments - 1 ? 2 : i + 3;

                triangles[triangleIndex++] = 0;
                triangles[triangleIndex++] = next;
                triangles[triangleIndex++] = current;

                triangles[triangleIndex++] = 1;
                triangles[triangleIndex++] = current;
                triangles[triangleIndex++] = next;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            meshFilter.sharedMesh = mesh;
            meshCollider.sharedMesh = mesh;
            meshRenderer.sharedMaterial = CreateRuntimeMaterial(Color.gray);
            return cone;
        }

        private static void ApplyMaterial(GameObject target, Material material, Color fallbackColor)
        {
            Renderer renderer = target.GetComponent<Renderer>();
            if (renderer == null)
            {
                return;
            }

            if (material != null)
            {
                renderer.sharedMaterial = material;
            }
            else
            {
                renderer.sharedMaterial = CreateRuntimeMaterial(fallbackColor);
            }
        }

        private static Material CreateRuntimeMaterial(Color color)
        {
            Shader shader = Shader.Find("Standard");
            if (shader == null)
            {
                shader = Shader.Find("Universal Render Pipeline/Lit");
            }

            if (shader == null)
            {
                shader = Shader.Find("Sprites/Default");
            }

            Material material = new Material(shader);
            material.color = color;
            return material;
        }
    }
}
