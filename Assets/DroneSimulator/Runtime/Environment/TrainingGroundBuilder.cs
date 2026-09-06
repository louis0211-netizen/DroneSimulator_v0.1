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
            CreatePanoramaEnvironment(activeThemeRoot, theme);

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
            ScatterSurfaceDetail(root, 34, 26f, 26f, 2.8f, new Color(0.19f, 0.2f, 0.2f), new Color(0.08f, 0.09f, 0.1f), 101);
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
            ScatterGrass(root, 64, 28f, 28f, 3.5f, new Color(0.2f, 0.38f, 0.18f), 221);
            ScatterSurfaceDetail(root, 40, 28f, 28f, 4f, new Color(0.16f, 0.1f, 0.07f), new Color(0.33f, 0.28f, 0.2f), 222);
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
            CreateRock(root, "Mossy Rock", new Vector3(3.8f, 0.42f, 5.5f), new Vector3(1.3f, 0.85f, 1.1f), new Color(0.34f, 0.35f, 0.31f), 31);
            CreateRock(root, "Forest Boulder", new Vector3(-4.6f, 0.35f, 14.5f), new Vector3(1.6f, 0.7f, 1.3f), new Color(0.28f, 0.29f, 0.25f), 32);
        }

        private void BuildMountain(Transform root)
        {
            CreateGround(root, "Mountain Plateau", new Color(0.32f, 0.31f, 0.28f), new Vector3(70f, 1f, 70f));
            ScatterSurfaceDetail(root, 55, 30f, 30f, 4f, new Color(0.22f, 0.22f, 0.21f), new Color(0.52f, 0.5f, 0.46f), 303);
            CreateTakeoffPad(root, new Color(0.21f, 0.23f, 0.25f), new Color(0.72f, 0.86f, 1f));
            CreateGate(root, new Vector3(0f, 1.8f, 10f), new Color(0.74f, 0.86f, 0.96f));
            CreateGate(root, new Vector3(-10f, 2.4f, 20f), new Color(0.95f, 0.68f, 0.28f));

            for (int i = -3; i <= 3; i++)
            {
                float height = 5f + (3 - Mathf.Abs(i)) * 2.5f;
                CreateMountainPeak(root, new Vector3(i * 8f, 0f, 28f + Mathf.Abs(i) * 3f), height);
            }

            CreateRock(root, "Cliff Pillar", new Vector3(6.5f, 1.5f, 8.5f), new Vector3(2.1f, 3.2f, 1.8f), new Color(0.27f, 0.28f, 0.27f), 41);
            CreateRock(root, "Cliff Pillar", new Vector3(-7f, 1.18f, 12f), new Vector3(1.7f, 2.55f, 1.5f), new Color(0.34f, 0.34f, 0.31f), 42);
            CreateRock(root, "Loose Boulder", new Vector3(-3.8f, 0.42f, 7.5f), new Vector3(1.4f, 0.8f, 1.15f), new Color(0.37f, 0.36f, 0.32f), 43);
            CreateObstacle(root, "Snow Marker", new Vector3(0f, 0.08f, -10f), new Vector3(16f, 0.08f, 1.5f), new Color(0.82f, 0.88f, 0.9f));
        }

        private void BuildBeach(Transform root)
        {
            CreateGround(root, "Sand Training Beach", new Color(0.73f, 0.62f, 0.42f), new Vector3(72f, 1f, 48f));
            ScatterSurfaceDetail(root, 42, 32f, 19f, 3.6f, new Color(0.56f, 0.46f, 0.3f), new Color(0.86f, 0.78f, 0.56f), 401);
            CreateWater(root, new Vector3(0f, -0.03f, 25f), new Vector3(80f, 0.06f, 26f));
            CreateTakeoffPad(root, new Color(0.32f, 0.25f, 0.18f), new Color(0.12f, 0.75f, 0.95f));
            CreateGate(root, new Vector3(0f, 1.5f, 8f), new Color(0.12f, 0.82f, 0.95f));
            CreateGate(root, new Vector3(9f, 1.5f, 17f), new Color(0.95f, 0.78f, 0.18f));

            for (int i = 0; i < 9; i++)
            {
                CreatePalm(root, new Vector3(-18f + i * 4.5f, 0f, -13f - (i % 2) * 2f), i % 2 == 0 ? -12f : 16f);
            }

            CreateDock(root, new Vector3(-7f, 0.12f, 17f));
            CreateRock(root, "Beach Rock", new Vector3(6f, 0.35f, 6f), new Vector3(1.6f, 0.7f, 1.2f), new Color(0.38f, 0.35f, 0.31f), 51);
            CreateBuoy(root, new Vector3(-4f, 0.28f, 24f), new Color(0.95f, 0.18f, 0.12f));
            CreateBuoy(root, new Vector3(4f, 0.28f, 27f), new Color(0.95f, 0.95f, 0.2f));
        }

        private void CreateGround(Transform root, string name, Color color, Vector3 scale)
        {
            GameObject ground = new GameObject(name);
            ground.name = name;
            ground.transform.SetParent(root);
            ground.transform.localPosition = Vector3.zero;

            MeshFilter meshFilter = ground.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = ground.AddComponent<MeshRenderer>();
            MeshCollider meshCollider = ground.AddComponent<MeshCollider>();

            Vector3 finalScale = scale == Vector3.zero ? groundSize : scale;
            meshFilter.sharedMesh = CreateGroundMesh(finalScale.x, finalScale.z, GetGroundRoughness(name));
            meshCollider.sharedMesh = meshFilter.sharedMesh;
            meshRenderer.sharedMaterial = groundMaterial != null ? groundMaterial : CreateTexturedMaterial(CreateGroundTexture(name, color), Color.white);
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
            bool horizontal = scale.x > scale.y && scale.x > scale.z;
            GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            part.name = partName;
            part.transform.SetParent(parent);
            part.transform.localPosition = localPosition;
            part.transform.localRotation = horizontal ? Quaternion.Euler(0f, 0f, 90f) : Quaternion.identity;
            float radius = Mathf.Max(scale.x, scale.z) * 0.5f;
            float length = horizontal ? scale.x * 0.5f : scale.y * 0.5f;
            part.transform.localScale = new Vector3(radius, length, radius);
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
            trunk.transform.localScale = new Vector3(0.16f, height * 0.25f, 0.16f);
            ApplyMaterial(trunk, null, new Color(0.29f, 0.17f, 0.1f));

            for (int i = 0; i < 3; i++)
            {
                GameObject crown = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                crown.name = "Layered Tree Crown";
                crown.transform.SetParent(root);
                crown.transform.localPosition = basePosition + new Vector3((i - 1) * 0.16f, height * (0.62f + i * 0.08f), (i % 2) * 0.12f);
                float width = 0.8f + height * 0.11f - i * 0.08f;
                crown.transform.localScale = new Vector3(width, height * 0.18f, width * 0.9f);
                ApplyMaterial(crown, null, new Color(0.08f + i * 0.025f, 0.27f + i * 0.03f, 0.12f));
            }
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
            CreateObstacle(root, "Cut Log Face", position + new Vector3(0.02f, 0f, 0.78f), new Vector3(0.52f, 0.04f, 0.52f), new Color(0.58f, 0.38f, 0.18f));
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
            GameObject water = GameObject.CreatePrimitive(PrimitiveType.Cube);
            water.name = "Ocean Surface";
            water.transform.SetParent(root);
            water.transform.localPosition = position;
            water.transform.localScale = scale;
            ApplyMaterial(water, null, new Color(0.08f, 0.36f, 0.56f, 0.72f));

            for (int i = 0; i < 7; i++)
            {
                CreateObstacle(root, "Foam Line", new Vector3(-28f + i * 9f, 0.04f, 13.2f + (i % 2) * 1.2f), new Vector3(5.2f, 0.025f, 0.12f), new Color(0.82f, 0.9f, 0.86f));
            }
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
                frond.transform.localScale = new Vector3(0.14f, 0.035f, 1.95f);
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

        private static void ScatterGrass(Transform root, int count, float halfWidth, float halfDepth, float avoidRadius, Color color, int seed)
        {
            System.Random random = new System.Random(seed);
            Material material = CreateRuntimeMaterial(color);

            for (int i = 0; i < count; i++)
            {
                Vector3 position = PickSurfacePoint(random, halfWidth, halfDepth, avoidRadius);
                GameObject tuft = new GameObject("Grass Tuft");
                tuft.transform.SetParent(root);
                tuft.transform.localPosition = new Vector3(position.x, 0.025f, position.z);
                tuft.transform.localRotation = Quaternion.Euler(0f, (float)random.NextDouble() * 360f, 0f);

                MeshFilter meshFilter = tuft.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = tuft.AddComponent<MeshRenderer>();
                float width = 0.18f + (float)random.NextDouble() * 0.18f;
                float height = 0.28f + (float)random.NextDouble() * 0.34f;
                meshFilter.sharedMesh = CreateCrossCardMesh(width, height);
                meshRenderer.sharedMaterial = material;
            }
        }

        private static void ScatterSurfaceDetail(Transform root, int count, float halfWidth, float halfDepth, float avoidRadius, Color dark, Color light, int seed)
        {
            System.Random random = new System.Random(seed);

            for (int i = 0; i < count; i++)
            {
                Vector3 position = PickSurfacePoint(random, halfWidth, halfDepth, avoidRadius);
                float radius = 0.12f + (float)random.NextDouble() * 0.34f;
                Color color = LerpColor(dark, light, (float)random.NextDouble());

                if (i % 4 == 0)
                {
                    CreateRock(root, "Ground Stone", new Vector3(position.x, radius * 0.22f, position.z), new Vector3(radius * 1.5f, radius * 0.45f, radius), color, seed + i);
                }
                else
                {
                    CreateSurfacePatch(root, position, radius, color, random);
                }
            }
        }

        private static Vector3 PickSurfacePoint(System.Random random, float halfWidth, float halfDepth, float avoidRadius)
        {
            for (int attempt = 0; attempt < 12; attempt++)
            {
                float x = Mathf.Lerp(-halfWidth, halfWidth, (float)random.NextDouble());
                float z = Mathf.Lerp(-halfDepth, halfDepth, (float)random.NextDouble());
                if ((new Vector3(x, 0f, z)).magnitude >= avoidRadius)
                {
                    return new Vector3(x, 0f, z);
                }
            }

            return new Vector3(avoidRadius + 1f, 0f, 0f);
        }

        private static void CreateSurfacePatch(Transform root, Vector3 position, float radius, Color color, System.Random random)
        {
            GameObject patch = new GameObject("Surface Patch");
            patch.transform.SetParent(root);
            patch.transform.localPosition = new Vector3(position.x, 0.018f, position.z);
            patch.transform.localRotation = Quaternion.Euler(0f, (float)random.NextDouble() * 360f, 0f);
            patch.transform.localScale = new Vector3(1f + (float)random.NextDouble() * 1.4f, 1f, 0.55f + (float)random.NextDouble() * 0.9f);

            MeshFilter meshFilter = patch.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = patch.AddComponent<MeshRenderer>();
            meshFilter.sharedMesh = CreateFlatOvalMesh(radius, 12);
            meshRenderer.sharedMaterial = CreateRuntimeMaterial(color);
        }

        private static void CreateRock(Transform root, string name, Vector3 position, Vector3 scale, Color color, int seed)
        {
            GameObject rock = new GameObject(name);
            rock.transform.SetParent(root);
            rock.transform.localPosition = position;
            rock.transform.localScale = scale;
            rock.transform.localRotation = Quaternion.Euler(0f, seed * 31f, 0f);

            MeshFilter meshFilter = rock.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = rock.AddComponent<MeshRenderer>();
            MeshCollider meshCollider = rock.AddComponent<MeshCollider>();
            Mesh mesh = CreateRockMesh(seed);
            meshFilter.sharedMesh = mesh;
            meshCollider.sharedMesh = mesh;
            meshRenderer.sharedMaterial = CreateTexturedMaterial(CreateStoneTexture(color, seed), Color.white);
        }

        private static Mesh CreateGroundMesh(float width, float depth, float roughness)
        {
            const int cells = 34;
            Vector3[] vertices = new Vector3[(cells + 1) * (cells + 1)];
            Vector2[] uv = new Vector2[vertices.Length];
            int[] triangles = new int[cells * cells * 6];

            int vertexIndex = 0;
            for (int z = 0; z <= cells; z++)
            {
                float vz = z / (float)cells;
                for (int x = 0; x <= cells; x++)
                {
                    float vx = x / (float)cells;
                    float worldX = (vx - 0.5f) * width;
                    float worldZ = (vz - 0.5f) * depth;
                    float y = (Mathf.Sin(worldX * 0.73f) + Mathf.Cos(worldZ * 0.61f) + Mathf.Sin((worldX + worldZ) * 0.29f)) * roughness;
                    vertices[vertexIndex] = new Vector3(worldX, y, worldZ);
                    uv[vertexIndex] = new Vector2(vx * width * 0.18f, vz * depth * 0.18f);
                    vertexIndex++;
                }
            }

            int triangleIndex = 0;
            for (int z = 0; z < cells; z++)
            {
                for (int x = 0; x < cells; x++)
                {
                    int current = z * (cells + 1) + x;
                    int next = current + cells + 1;
                    triangles[triangleIndex++] = current;
                    triangles[triangleIndex++] = next;
                    triangles[triangleIndex++] = current + 1;
                    triangles[triangleIndex++] = current + 1;
                    triangles[triangleIndex++] = next;
                    triangles[triangleIndex++] = next + 1;
                }
            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        private static Mesh CreateRockMesh(int seed)
        {
            System.Random random = new System.Random(seed);
            const int ring = 10;
            Vector3[] vertices = new Vector3[ring * 2 + 2];
            int[] triangles = new int[ring * 12];

            vertices[0] = new Vector3(0f, 0.62f, 0f);
            vertices[vertices.Length - 1] = new Vector3(0f, -0.45f, 0f);

            for (int i = 0; i < ring; i++)
            {
                float angle = i * Mathf.PI * 2f / ring;
                float variation = 0.72f + (float)random.NextDouble() * 0.36f;
                vertices[i + 1] = new Vector3(Mathf.Cos(angle) * variation, 0.1f + (float)random.NextDouble() * 0.18f, Mathf.Sin(angle) * variation * 0.82f);
                vertices[i + 1 + ring] = new Vector3(Mathf.Cos(angle) * variation * 0.82f, -0.34f, Mathf.Sin(angle) * variation * 0.7f);
            }

            int triangleIndex = 0;
            for (int i = 0; i < ring; i++)
            {
                int a = i + 1;
                int b = i == ring - 1 ? 1 : i + 2;
                int c = a + ring;
                int d = b + ring;
                int bottom = vertices.Length - 1;

                triangles[triangleIndex++] = 0;
                triangles[triangleIndex++] = a;
                triangles[triangleIndex++] = b;

                triangles[triangleIndex++] = a;
                triangles[triangleIndex++] = c;
                triangles[triangleIndex++] = d;
                triangles[triangleIndex++] = a;
                triangles[triangleIndex++] = d;
                triangles[triangleIndex++] = b;

                triangles[triangleIndex++] = bottom;
                triangles[triangleIndex++] = d;
                triangles[triangleIndex++] = c;
            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        private static Mesh CreateFlatOvalMesh(float radius, int segments)
        {
            int safeSegments = Mathf.Max(8, segments);
            Vector3[] vertices = new Vector3[safeSegments + 1];
            int[] triangles = new int[safeSegments * 3];
            vertices[0] = Vector3.zero;

            for (int i = 0; i < safeSegments; i++)
            {
                float angle = i * Mathf.PI * 2f / safeSegments;
                vertices[i + 1] = new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
            }

            int triangleIndex = 0;
            for (int i = 0; i < safeSegments; i++)
            {
                triangles[triangleIndex++] = 0;
                triangles[triangleIndex++] = i + 1;
                triangles[triangleIndex++] = i == safeSegments - 1 ? 1 : i + 2;
            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        private static Mesh CreateCrossCardMesh(float width, float height)
        {
            Vector3[] vertices =
            {
                new Vector3(-width, 0f, 0f), new Vector3(-width * 0.18f, height, 0f), new Vector3(width * 0.18f, height, 0f), new Vector3(width, 0f, 0f),
                new Vector3(0f, 0f, -width), new Vector3(0f, height, -width * 0.18f), new Vector3(0f, height, width * 0.18f), new Vector3(0f, 0f, width)
            };
            int[] triangles = { 0, 1, 2, 0, 2, 3, 4, 5, 6, 4, 6, 7 };

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        private static float GetGroundRoughness(string name)
        {
            if (name.Contains("Asphalt"))
            {
                return 0.012f;
            }

            if (name.Contains("Mountain"))
            {
                return 0.12f;
            }

            if (name.Contains("Sand"))
            {
                return 0.045f;
            }

            return 0.075f;
        }

        private static Texture2D CreateGroundTexture(string name, Color baseColor)
        {
            int seed = name.GetHashCode();
            System.Random random = new System.Random(seed);
            Texture2D texture = new Texture2D(96, 96, TextureFormat.RGBA32, false);
            texture.wrapMode = TextureWrapMode.Repeat;
            texture.filterMode = FilterMode.Bilinear;

            for (int y = 0; y < 96; y++)
            {
                for (int x = 0; x < 96; x++)
                {
                    float noise = (float)random.NextDouble();
                    float wave = (Mathf.Sin(x * 0.34f) + Mathf.Cos(y * 0.27f)) * 0.5f;
                    float variation = Mathf.Clamp01(0.42f + noise * 0.38f + wave * 0.12f);
                    texture.SetPixel(x, y, LerpColor(ScaleColor(baseColor, 0.68f), ScaleColor(baseColor, 1.28f), variation));
                }
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D CreateStoneTexture(Color baseColor, int seed)
        {
            System.Random random = new System.Random(seed);
            Texture2D texture = new Texture2D(48, 48, TextureFormat.RGBA32, false);
            texture.wrapMode = TextureWrapMode.Repeat;
            texture.filterMode = FilterMode.Bilinear;

            for (int y = 0; y < 48; y++)
            {
                for (int x = 0; x < 48; x++)
                {
                    float grain = (float)random.NextDouble();
                    float band = Mathf.Sin((x + y) * 0.21f) * 0.12f;
                    texture.SetPixel(x, y, LerpColor(ScaleColor(baseColor, 0.72f), ScaleColor(baseColor, 1.18f), Mathf.Clamp01(grain + band)));
                }
            }

            texture.Apply();
            return texture;
        }

        private static Color LerpColor(Color from, Color to, float t)
        {
            float clamped = Mathf.Clamp01(t);
            return new Color(
                Mathf.Lerp(from.r, to.r, clamped),
                Mathf.Lerp(from.g, to.g, clamped),
                Mathf.Lerp(from.b, to.b, clamped),
                Mathf.Lerp(from.a, to.a, clamped));
        }

        private static Color ScaleColor(Color color, float multiplier)
        {
            return new Color(
                Mathf.Clamp01(color.r * multiplier),
                Mathf.Clamp01(color.g * multiplier),
                Mathf.Clamp01(color.b * multiplier),
                color.a);
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
                    fogDensity = 0.002f;
                    RenderSettings.ambientLight = new Color(0.42f, 0.48f, 0.4f);
                    break;
                case EnvironmentTheme.Mountain:
                    skyColor = new Color(0.52f, 0.62f, 0.72f);
                    fogColor = new Color(0.5f, 0.56f, 0.6f);
                    fogDensity = 0.0015f;
                    RenderSettings.ambientLight = new Color(0.5f, 0.5f, 0.48f);
                    break;
                case EnvironmentTheme.Beach:
                    skyColor = new Color(0.48f, 0.68f, 0.82f);
                    fogColor = new Color(0.62f, 0.72f, 0.74f);
                    fogDensity = 0.001f;
                    RenderSettings.ambientLight = new Color(0.62f, 0.58f, 0.48f);
                    break;
                default:
                    skyColor = new Color(0.28f, 0.38f, 0.46f);
                    fogColor = new Color(0.22f, 0.25f, 0.28f);
                    fogDensity = 0.0015f;
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

        private static void CreatePanoramaEnvironment(Transform root, EnvironmentTheme theme)
        {
            Texture2D texture = LoadPanoramaTexture(theme);
            if (texture == null)
            {
                texture = LoadPreviewTexture(theme);
            }

            if (texture == null)
            {
                return;
            }

            GameObject panorama = new GameObject(theme + " 360 Photo Environment");
            panorama.transform.SetParent(root);
            panorama.transform.localPosition = new Vector3(0f, 1.15f, 0f);

            MeshFilter meshFilter = panorama.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = panorama.AddComponent<MeshRenderer>();
            meshFilter.sharedMesh = CreateInsideOutSphereMesh(48, 24, 88f);
            meshRenderer.sharedMaterial = CreateTexturedMaterial(texture, Color.white);
        }

        private static Texture2D LoadPanoramaTexture(EnvironmentTheme theme)
        {
            switch (theme)
            {
                case EnvironmentTheme.Forest:
                    return Resources.Load<Texture2D>("EnvironmentPanoramas/forest_hochsal_forest");
                case EnvironmentTheme.Mountain:
                    return Resources.Load<Texture2D>("EnvironmentPanoramas/mountain_table_mountain_2");
                case EnvironmentTheme.Beach:
                    return Resources.Load<Texture2D>("EnvironmentPanoramas/beach_umhlanga_sunrise");
                default:
                    return Resources.Load<Texture2D>("EnvironmentPanoramas/city_wide_street_02");
            }
        }

        private static Texture2D LoadPreviewTexture(EnvironmentTheme theme)
        {
            switch (theme)
            {
                case EnvironmentTheme.Forest:
                    return Resources.Load<Texture2D>("EnvironmentBackdrops/forest_hochsal_forest");
                case EnvironmentTheme.Mountain:
                    return Resources.Load<Texture2D>("EnvironmentBackdrops/mountain_table_mountain_2");
                case EnvironmentTheme.Beach:
                    return Resources.Load<Texture2D>("EnvironmentBackdrops/beach_umhlanga_sunrise");
                default:
                    return Resources.Load<Texture2D>("EnvironmentBackdrops/city_wide_street_02");
            }
        }

        private static Material CreateTexturedMaterial(Texture texture, Color tint)
        {
            Shader shader = Shader.Find("Unlit/Texture");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            Material material = new Material(shader);
            material.color = tint;
            material.mainTexture = texture;
            return material;
        }

        private static Mesh CreateInsideOutSphereMesh(int longitudeSegments, int latitudeSegments, float radius)
        {
            int safeLongitudeSegments = Mathf.Max(16, longitudeSegments);
            int safeLatitudeSegments = Mathf.Max(8, latitudeSegments);
            Vector3[] vertices = new Vector3[(safeLongitudeSegments + 1) * (safeLatitudeSegments + 1)];
            Vector2[] uv = new Vector2[vertices.Length];
            int[] triangles = new int[safeLongitudeSegments * safeLatitudeSegments * 6];

            int vertexIndex = 0;
            for (int y = 0; y <= safeLatitudeSegments; y++)
            {
                float v = y / (float)safeLatitudeSegments;
                float phi = Mathf.PI * v;
                float sinPhi = Mathf.Sin(phi);
                float cosPhi = Mathf.Cos(phi);

                for (int x = 0; x <= safeLongitudeSegments; x++)
                {
                    float u = x / (float)safeLongitudeSegments;
                    float theta = Mathf.PI * 2f * u;
                    vertices[vertexIndex] = new Vector3(
                        Mathf.Sin(theta) * sinPhi * radius,
                        cosPhi * radius,
                        Mathf.Cos(theta) * sinPhi * radius);
                    uv[vertexIndex] = new Vector2(1f - u, 1f - v);
                    vertexIndex++;
                }
            }

            int triangleIndex = 0;
            for (int y = 0; y < safeLatitudeSegments; y++)
            {
                for (int x = 0; x < safeLongitudeSegments; x++)
                {
                    int current = y * (safeLongitudeSegments + 1) + x;
                    int next = current + safeLongitudeSegments + 1;

                    triangles[triangleIndex++] = current;
                    triangles[triangleIndex++] = next + 1;
                    triangles[triangleIndex++] = next;
                    triangles[triangleIndex++] = current;
                    triangles[triangleIndex++] = current + 1;
                    triangles[triangleIndex++] = next + 1;
                }
            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
