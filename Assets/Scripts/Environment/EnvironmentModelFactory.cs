using System.Collections.Generic;
using UnityEngine;

namespace SkyboundAdventure
{
    public static class EnvironmentModelFactory
    {
        #region Faceted Rock Generation

        /// <summary>
        /// Generates an irregular, faceted stylized 3D boulder mesh with flat shading.
        /// Matches the angular cliff/monolith aesthetic in ref_palm_rocks.jpg.
        /// </summary>
        public static Mesh CreateFacetedBoulderMesh(int seed, Vector3 scale)
        {
            Random.State oldState = Random.state;
            Random.InitState(seed);

            // Base icosahedron vertex definitions
            float t = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;
            List<Vector3> baseVerts = new List<Vector3>
            {
                new Vector3(-1,  t,  0).normalized,
                new Vector3( 1,  t,  0).normalized,
                new Vector3(-1, -t,  0).normalized,
                new Vector3( 1, -t,  0).normalized,
                new Vector3( 0, -1,  t).normalized,
                new Vector3( 0,  1,  t).normalized,
                new Vector3( 0, -1, -t).normalized,
                new Vector3( 0,  1, -t).normalized,
                new Vector3( t,  0, -1).normalized,
                new Vector3( t,  0,  1).normalized,
                new Vector3(-t,  0, -1).normalized,
                new Vector3(-t,  0,  1).normalized
            };

            // Apply randomized planar shears and fracture variations
            for (int i = 0; i < baseVerts.Count; i++)
            {
                Vector3 v = baseVerts[i];
                float noise = Mathf.PerlinNoise((v.x + 2f) * 1.8f, (v.z + 2f) * 1.8f);
                float radiusVariation = 0.72f + noise * 0.45f + Random.Range(-0.10f, 0.15f);

                // Flatten bottom to sit stably on terrain
                if (v.y < -0.3f)
                {
                    radiusVariation *= 0.65f;
                    v.y = Mathf.Max(v.y, -0.6f);
                }

                // Add angular facet planes
                v.x *= scale.x * radiusVariation * (1.0f + Random.Range(-0.08f, 0.08f));
                v.y *= scale.y * radiusVariation;
                v.z *= scale.z * radiusVariation * (1.0f + Random.Range(-0.08f, 0.08f));
                baseVerts[i] = v;
            }

            int[] icosahedronIndices = new int[]
            {
                0, 11, 5,   0, 5, 1,    0, 1, 7,    0, 7, 10,   0, 10, 11,
                1, 5, 9,    5, 11, 4,   11, 10, 2,  10, 7, 6,   7, 1, 8,
                3, 9, 4,    3, 4, 2,    3, 2, 6,    3, 6, 8,    3, 8, 9,
                4, 9, 5,    2, 4, 11,   6, 2, 10,   8, 6, 7,    9, 8, 1
            };

            // Unshare vertices to generate crisp flat-shaded facet normals
            List<Vector3> flatVerts = new List<Vector3>();
            List<int> flatTris = new List<int>();
            List<Vector2> flatUVs = new List<Vector2>();

            for (int i = 0; i < icosahedronIndices.Length; i += 3)
            {
                Vector3 v0 = baseVerts[icosahedronIndices[i]];
                Vector3 v1 = baseVerts[icosahedronIndices[i + 1]];
                Vector3 v2 = baseVerts[icosahedronIndices[i + 2]];

                int idx = flatVerts.Count;
                flatVerts.Add(v0);
                flatVerts.Add(v1);
                flatVerts.Add(v2);

                flatTris.Add(idx);
                flatTris.Add(idx + 1);
                flatTris.Add(idx + 2);

                flatUVs.Add(new Vector2(v0.x + v0.z, v0.y));
                flatUVs.Add(new Vector2(v1.x + v1.z, v1.y));
                flatUVs.Add(new Vector2(v2.x + v2.z, v2.y));
            }

            Mesh mesh = new Mesh
            {
                name = $"FacetedBoulder_{seed}"
            };
            mesh.SetVertices(flatVerts);
            mesh.SetTriangles(flatTris, 0);
            mesh.SetUVs(0, flatUVs);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            Random.state = oldState;
            return mesh;
        }

        #endregion

        #region Organic Tree Trunk & Branch Modeling

        /// <summary>
        /// Creates an organic, tapered tree trunk with natural buttressed root flares,
        /// graceful S-curve lean, bark fluting, and orthogonal Frenet-frame cross sections.
        /// </summary>
        public static Mesh CreateOrganicTrunkMesh(int seed, float height, float baseRadius, float topRadius, float bendX, float bendZ, int rootToesCount = 4)
        {
            Random.State oldState = Random.state;
            Random.InitState(seed);

            int heightSegments = 14;
            int radialSegments = 16;

            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            // Generate spine points with a graceful natural organic S-curve
            Vector3[] spine = new Vector3[heightSegments + 1];
            for (int i = 0; i <= heightSegments; i++)
            {
                float t = (float)i / heightSegments;
                // Natural organic S-curve lean (gentle backward bow in lower half, sweeping forward-right at fork)
                float x = Mathf.Sin(t * Mathf.PI * 0.90f) * bendX + Mathf.Sin(t * Mathf.PI * 2f) * 0.05f;
                float z = (1f - Mathf.Cos(t * Mathf.PI * 0.70f)) * bendZ - Mathf.Sin(t * Mathf.PI) * 0.08f;
                float y = t * height;
                spine[i] = new Vector3(x, y, z);
            }

            for (int i = 0; i <= heightSegments; i++)
            {
                float t = (float)i / heightSegments;
                // Graceful taper from base to fork
                float r = Mathf.Lerp(baseRadius, topRadius, Mathf.Pow(t, 0.70f));

                // Calculate orthogonal Frenet frame vectors for zero distortion/shearing
                Vector3 forward = (i < heightSegments) ? (spine[i + 1] - spine[i]).normalized : (spine[i] - spine[i - 1]).normalized;
                Vector3 right = Vector3.Cross(Vector3.forward, forward).normalized;
                if (right.sqrMagnitude < 0.001f) right = Vector3.Cross(Vector3.up, forward).normalized;
                Vector3 up = Vector3.Cross(forward, right).normalized;

                Vector3 center = spine[i];

                // Flared organic root buttresses (spreads over lower 35% of trunk height)
                float rootSpread = 1.0f;
                float rootFactor = 0f;
                if (t < 0.35f)
                {
                    float rootT = 1.0f - (t / 0.35f); // 1.0 at ground, 0.0 at t=0.35
                    rootSpread = 1.0f + Mathf.Pow(rootT, 1.6f) * 0.75f;
                    rootFactor = rootT;
                }

                for (int j = 0; j < radialSegments; j++)
                {
                    float angle = (float)j / radialSegments * Mathf.PI * 2f;

                    // 4 distinct root buttress legs (legs project along 35, 125, 215, 305 degrees)
                    float toeWave = Mathf.Cos((angle - 0.25f) * rootToesCount);
                    float rootNodule = Mathf.Max(0f, toeWave);
                    float rootExtra = Mathf.Pow(rootNodule, 2.2f) * Mathf.Pow(rootFactor, 1.4f) * 0.95f;

                    // Longitudinal bark texture ripples (natural bark ridges and clefts)
                    float barkRipples = 1.0f + Mathf.Sin(angle * 6f + t * 4f) * 0.045f + Mathf.Sin(angle * 12f) * 0.02f;

                    float finalRadius = r * (rootSpread + rootExtra) * barkRipples;

                    Vector3 offset = (right * Mathf.Cos(angle) + up * Mathf.Sin(angle)) * finalRadius;

                    verts.Add(center + offset);
                    uvs.Add(new Vector2((float)j / radialSegments, t * (height / 2f)));
                }
            }

            for (int i = 0; i < heightSegments; i++)
            {
                int row0 = i * radialSegments;
                int row1 = (i + 1) * radialSegments;

                for (int j = 0; j < radialSegments; j++)
                {
                    int next = (j + 1) % radialSegments;
                    int b0 = row0 + j;
                    int b1 = row0 + next;
                    int t0 = row1 + j;
                    int t1 = row1 + next;

                    tris.Add(b0); tris.Add(t0); tris.Add(b1);
                    tris.Add(b1); tris.Add(t0); tris.Add(t1);
                }
            }

            // Top end cap
            int topRowStart = heightSegments * radialSegments;
            int topCenterIdx = verts.Count;
            verts.Add(spine[heightSegments]);
            uvs.Add(new Vector2(0.5f, 1f));
            for (int j = 0; j < radialSegments; j++)
            {
                int next = (j + 1) % radialSegments;
                tris.Add(topRowStart + j);
                tris.Add(topCenterIdx);
                tris.Add(topRowStart + next);
            }

            Mesh mesh = new Mesh { name = $"Trunk_Organic_{seed}" };
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.SetUVs(0, uvs);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            Random.state = oldState;
            return mesh;
        }

        /// <summary>
        /// Creates a tapered 3D branch tube that extends outward and sweeps gently upward to support foliage.
        /// </summary>
        public static Mesh CreateBranchTubeMesh(int seed, float length, float startRadius, float endRadius, float curveAngle, float upwardLift)
        {
            Random.State oldState = Random.state;
            Random.InitState(seed);

            int lengthSegments = 10;
            int radialSegments = 10;

            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            Vector3[] spine = new Vector3[lengthSegments + 1];
            for (int i = 0; i <= lengthSegments; i++)
            {
                float t = (float)i / lengthSegments;
                // Branch curves forward-right and sweeps upward
                float z = t * length;
                float x = Mathf.Sin(t * curveAngle * Mathf.Deg2Rad) * length * 0.35f;
                float y = Mathf.Pow(t, 1.4f) * upwardLift * length;
                spine[i] = new Vector3(x, y, z);
            }

            for (int i = 0; i <= lengthSegments; i++)
            {
                float t = (float)i / lengthSegments;
                float r = Mathf.Lerp(startRadius, endRadius, t);

                Vector3 forward = (i < lengthSegments) ? (spine[i + 1] - spine[i]).normalized : (spine[i] - spine[i - 1]).normalized;
                Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
                if (right.sqrMagnitude < 0.001f) right = Vector3.right;
                Vector3 up = Vector3.Cross(forward, right).normalized;

                Vector3 center = spine[i];

                for (int j = 0; j < radialSegments; j++)
                {
                    float angle = (float)j / radialSegments * Mathf.PI * 2f;
                    float fluting = 1.0f + Mathf.Sin(angle * 3f) * 0.06f;
                    Vector3 offset = (right * Mathf.Cos(angle) + up * Mathf.Sin(angle)) * r * fluting;

                    verts.Add(center + offset);
                    uvs.Add(new Vector2((float)j / radialSegments, t));
                }
            }

            for (int i = 0; i < lengthSegments; i++)
            {
                int row0 = i * radialSegments;
                int row1 = (i + 1) * radialSegments;

                for (int j = 0; j < radialSegments; j++)
                {
                    int next = (j + 1) % radialSegments;
                    int b0 = row0 + j;
                    int b1 = row0 + next;
                    int t0 = row1 + j;
                    int t1 = row1 + next;

                    tris.Add(b0); tris.Add(t0); tris.Add(b1);
                    tris.Add(b1); tris.Add(t0); tris.Add(t1);
                }
            }

            // Tip cap
            int tipRowStart = lengthSegments * radialSegments;
            int tipCenterIdx = verts.Count;
            verts.Add(spine[lengthSegments]);
            uvs.Add(new Vector2(0.5f, 1f));
            for (int j = 0; j < radialSegments; j++)
            {
                int next = (j + 1) % radialSegments;
                tris.Add(tipRowStart + j);
                tris.Add(tipCenterIdx);
                tris.Add(tipRowStart + next);
            }

            Mesh mesh = new Mesh { name = $"Branch_Tube_{seed}" };
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.SetUVs(0, uvs);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            Random.state = oldState;
            return mesh;
        }

        #endregion

        #region Volumetric Foliage Modeling

        /// <summary>
        /// Generates a volumetric, organic 3D foliage cloud cluster with 2-level geodesic subdivision
        /// (320 smooth triangles, 162 vertices), convex pillowy leaf puff lobes, and radial blended normals.
        /// Completely eliminates faceted boulder planes and rock-like appearance.
        /// </summary>
        public static Mesh CreateVolumetricFoliageMesh(int seed, Vector3 scale, float lobeStrength = 0.22f)
        {
            Random.State oldState = Random.state;
            Random.InitState(seed);

            // Base icosahedron definitions
            float t = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;
            List<Vector3> icoVerts = new List<Vector3>
            {
                new Vector3(-1,  t,  0).normalized,
                new Vector3( 1,  t,  0).normalized,
                new Vector3(-1, -t,  0).normalized,
                new Vector3( 1, -t,  0).normalized,
                new Vector3( 0, -1,  t).normalized,
                new Vector3( 0,  1,  t).normalized,
                new Vector3( 0, -1, -t).normalized,
                new Vector3( 0,  1, -t).normalized,
                new Vector3( t,  0, -1).normalized,
                new Vector3( t,  0,  1).normalized,
                new Vector3(-t,  0, -1).normalized,
                new Vector3(-t,  0,  1).normalized
            };

            int[] icoTris = new int[]
            {
                0, 11, 5,   0, 5, 1,    0, 1, 7,    0, 7, 10,   0, 10, 11,
                1, 5, 9,    5, 11, 4,   11, 10, 2,  10, 7, 6,   7, 1, 8,
                3, 9, 4,    3, 4, 2,    3, 2, 6,    3, 6, 8,    3, 8, 9,
                4, 9, 5,    2, 4, 11,   6, 2, 10,   8, 6, 7,    9, 8, 1
            };

            // Midpoint cache for geodesic subdivision
            Dictionary<int, int> midPointCache = new Dictionary<int, int>();
            int GetMidpoint(int p1, int p2)
            {
                int smaller = Mathf.Min(p1, p2);
                int greater = Mathf.Max(p1, p2);
                int key = (smaller << 16) + greater;
                if (midPointCache.TryGetValue(key, out int ret)) return ret;

                Vector3 point1 = icoVerts[p1];
                Vector3 point2 = icoVerts[p2];
                Vector3 middle = ((point1 + point2) * 0.5f).normalized;
                int idx = icoVerts.Count;
                icoVerts.Add(middle);
                midPointCache.Add(key, idx);
                return idx;
            }

            // Subdivide icosahedron level 1 (20 -> 80 triangles, 42 vertices)
            List<int> subTris1 = new List<int>();
            for (int i = 0; i < icoTris.Length; i += 3)
            {
                int v1 = icoTris[i];
                int v2 = icoTris[i + 1];
                int v3 = icoTris[i + 2];

                int a = GetMidpoint(v1, v2);
                int b = GetMidpoint(v2, v3);
                int c = GetMidpoint(v3, v1);

                subTris1.Add(v1); subTris1.Add(a); subTris1.Add(c);
                subTris1.Add(v2); subTris1.Add(b); subTris1.Add(a);
                subTris1.Add(v3); subTris1.Add(c); subTris1.Add(b);
                subTris1.Add(a);  subTris1.Add(b); subTris1.Add(c);
            }

            // Subdivide icosahedron level 2 (80 -> 320 triangles, 162 vertices)
            List<int> subTris2 = new List<int>();
            for (int i = 0; i < subTris1.Count; i += 3)
            {
                int v1 = subTris1[i];
                int v2 = subTris1[i + 1];
                int v3 = subTris1[i + 2];

                int a = GetMidpoint(v1, v2);
                int b = GetMidpoint(v2, v3);
                int c = GetMidpoint(v3, v1);

                subTris2.Add(v1); subTris2.Add(a); subTris2.Add(c);
                subTris2.Add(v2); subTris2.Add(b); subTris2.Add(a);
                subTris2.Add(v3); subTris2.Add(c); subTris2.Add(b);
                subTris2.Add(a);  subTris2.Add(b); subTris2.Add(c);
            }

            // Subdivide icosahedron level 3 (320 -> 1280 triangles, 642 vertices)
            // Ensures silky smooth, high-resolution curvature with zero flat polygonal facets
            List<int> subTris3 = new List<int>();
            for (int i = 0; i < subTris2.Count; i += 3)
            {
                int v1 = subTris2[i];
                int v2 = subTris2[i + 1];
                int v3 = subTris2[i + 2];

                int a = GetMidpoint(v1, v2);
                int b = GetMidpoint(v2, v3);
                int c = GetMidpoint(v3, v1);

                subTris3.Add(v1); subTris3.Add(a); subTris3.Add(c);
                subTris3.Add(v2); subTris3.Add(b); subTris3.Add(a);
                subTris3.Add(v3); subTris3.Add(c); subTris3.Add(b);
                subTris3.Add(a);  subTris3.Add(b); subTris3.Add(c);
            }

            // Define 8 organic convex directional lobes (simulates rounded, billowy leaf clumps/clouds)
            float rotY = (seed % 19) * 0.33f;
            Quaternion seedRot = Quaternion.Euler(0f, rotY * Mathf.Rad2Deg, 0f);

            Vector3[] baseLobeDirs = new Vector3[]
            {
                new Vector3( 0.0f,  0.88f,  0.20f).normalized,
                new Vector3( 0.72f, 0.42f,  0.42f).normalized,
                new Vector3(-0.72f, 0.42f,  0.42f).normalized,
                new Vector3( 0.65f, 0.30f, -0.65f).normalized,
                new Vector3(-0.65f, 0.30f, -0.65f).normalized,
                new Vector3( 0.0f,  0.22f,  0.92f).normalized,
                new Vector3( 0.45f, 0.72f, -0.35f).normalized,
                new Vector3(-0.45f, 0.72f, -0.35f).normalized
            };

            Vector3[] lobeDirs = new Vector3[baseLobeDirs.Length];
            for (int l = 0; l < baseLobeDirs.Length; l++)
            {
                lobeDirs[l] = seedRot * baseLobeDirs[l];
            }

            float[] lobeWeights = new float[] { 0.22f, 0.18f, 0.18f, 0.16f, 0.16f, 0.15f, 0.14f, 0.14f };

            // Apply 3D volumetric organic convex lobe displacement
            List<Vector3> deformedVerts = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();

            float seedX = (seed % 17) * 1.37f;
            float seedZ = (seed % 31) * 1.83f;

            for (int i = 0; i < icoVerts.Count; i++)
            {
                Vector3 n = icoVerts[i];

                // Puffy convex cloud lobes (always positive, smooth power curve)
                float puff = 0f;
                for (int l = 0; l < lobeDirs.Length; l++)
                {
                    float dot = Mathf.Max(0f, Vector3.Dot(n, lobeDirs[l]));
                    puff += Mathf.Pow(dot, 2.2f) * lobeWeights[l] * (lobeStrength / 0.22f);
                }

                // Subtle organic leaf tuft micro-undulation (breaks up specular highlights naturally)
                float wave = Mathf.Sin(n.x * 6.5f + seedX) * Mathf.Cos(n.z * 6.5f + seedZ) * 0.025f;

                // Continuous smooth underside tuck (eliminates harsh equator creasing)
                float bottomTuck = 1.0f - Mathf.Clamp01(-n.y) * 0.20f;
                float radius = (1.0f + puff + wave) * bottomTuck;

                Vector3 pos = new Vector3(
                    n.x * scale.x * radius,
                    n.y * scale.y * radius,
                    n.z * scale.z * radius
                );

                deformedVerts.Add(pos);

                // Spherical UV mapping
                float phi = Mathf.Atan2(n.z, n.x);
                float u = 0.5f + phi / (Mathf.PI * 2f);
                float v = 0.5f - Mathf.Asin(Mathf.Clamp(n.y, -1f, 1f)) / Mathf.PI;
                uvs.Add(new Vector2(u, v));
            }

            Mesh mesh = new Mesh { name = $"Foliage_Volumetric_{seed}" };
            mesh.SetVertices(deformedVerts);
            mesh.SetTriangles(subTris3, 0);
            mesh.SetUVs(0, uvs);
            mesh.RecalculateNormals();

            // Blend surface normals with radial center normals for AAA stylized volumetric lighting (82% radial, 18% surface)
            Vector3[] computedNormals = mesh.normals;
            for (int i = 0; i < computedNormals.Length; i++)
            {
                Vector3 radial = icoVerts[i];
                computedNormals[i] = Vector3.Lerp(computedNormals[i], radial, 0.82f).normalized;
            }
            mesh.normals = computedNormals;
            mesh.RecalculateBounds();

            Random.state = oldState;
            return mesh;
        }

        #endregion

        #region Tropical Palm Tree Modeling

        /// <summary>
        /// Creates a segmented, arching palm trunk mesh with flared horizontal rings.
        /// Matches the palm trunk in ref_palm_rocks.jpg.
        /// </summary>
        public static Mesh CreateStylizedPalmTrunkMesh(int segments, float height, float curveAngle, float baseRadius, float topRadius)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            int radialSegments = 12;

            // Generate spine points with a natural parabolic curve
            Vector3[] spine = new Vector3[segments + 1];
            for (int s = 0; s <= segments; s++)
            {
                float t = (float)s / segments;
                float x = Mathf.Sin(t * curveAngle * Mathf.Deg2Rad) * height * 0.28f;
                float y = Mathf.Cos(t * curveAngle * Mathf.Deg2Rad * 0.5f) * t * height;
                float z = (1f - Mathf.Cos(t * 1.2f)) * 0.12f;
                spine[s] = new Vector3(x, y, z);
            }

            for (int s = 0; s < segments; s++)
            {
                float t0 = (float)s / segments;
                float t1 = (float)(s + 1) / segments;

                // Taper radius with segment rib flare
                float r0 = Mathf.Lerp(baseRadius, topRadius, t0);
                float r1 = Mathf.Lerp(baseRadius, topRadius, t1);

                // Root flare at ground ring
                if (s == 0) r0 *= 1.45f;
                else if (s == 1) r0 *= 1.18f;

                // Segment ring overlap lip
                float r0_lip = r0 * 1.12f;

                Vector3 p0 = spine[s];
                Vector3 p1 = spine[s + 1];
                Vector3 forward = (p1 - p0).normalized;
                Vector3 right = Vector3.Cross(forward, Vector3.forward).normalized;
                if (right.sqrMagnitude < 0.001f) right = Vector3.Cross(forward, Vector3.up).normalized;
                Vector3 up = Vector3.Cross(right, forward).normalized;

                int ringStart = verts.Count;

                // Ring 0 (bottom lip of segment)
                for (int r = 0; r < radialSegments; r++)
                {
                    float angle = (float)r / radialSegments * Mathf.PI * 2f;
                    Vector3 offset = (right * Mathf.Cos(angle) + up * Mathf.Sin(angle)) * r0_lip;
                    verts.Add(p0 + offset);
                    uvs.Add(new Vector2((float)r / radialSegments, t0));
                }

                // Ring 1 (top of segment, tucked slightly under the next)
                for (int r = 0; r < radialSegments; r++)
                {
                    float angle = (float)r / radialSegments * Mathf.PI * 2f;
                    Vector3 offset = (right * Mathf.Cos(angle) + up * Mathf.Sin(angle)) * r1;
                    verts.Add(p1 + offset);
                    uvs.Add(new Vector2((float)r / radialSegments, t1));
                }

                // Quad strips
                for (int r = 0; r < radialSegments; r++)
                {
                    int next = (r + 1) % radialSegments;
                    int b0 = ringStart + r;
                    int b1 = ringStart + next;
                    int t0_idx = ringStart + radialSegments + r;
                    int t1_idx = ringStart + radialSegments + next;

                    tris.Add(b0); tris.Add(t0_idx); tris.Add(b1);
                    tris.Add(b1); tris.Add(t0_idx); tris.Add(t1_idx);
                }
            }

            Mesh mesh = new Mesh { name = "PalmTrunk_Segmented" };
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.SetUVs(0, uvs);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        /// <summary>
        /// Generates a natural tropical palm frond with a central arching spine and individual narrow leaflets
        /// projecting outward and drooping gently, matching real coconut palm foliage.
        /// NEVER creates flat triangular blades or solid planes.
        /// </summary>
        public static Mesh CreateFeatheredPalmFrondMesh(float length, float maxWidth, float droopAngle, int leafletPairs = 18)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            // 1. Central Arching Spine Points (Rachis)
            Vector3[] spine = new Vector3[leafletPairs + 1];
            Vector3[] spineFwd = new Vector3[leafletPairs + 1];
            Vector3[] spineRight = new Vector3[leafletPairs + 1];
            Vector3[] spineUp = new Vector3[leafletPairs + 1];

            for (int i = 0; i <= leafletPairs; i++)
            {
                float t = (float)i / leafletPairs;
                float z = t * length;
                // Natural parabolic arch with gentle S-curve lift at base and graceful gravity droop
                float y = Mathf.Sin(t * Mathf.PI * 0.45f) * 0.12f - Mathf.Pow(t, 1.75f) * Mathf.Sin(droopAngle * Mathf.Deg2Rad) * length * 0.65f;
                spine[i] = new Vector3(0f, y, z);
            }

            for (int i = 0; i <= leafletPairs; i++)
            {
                Vector3 fwd = (i < leafletPairs) ? (spine[i + 1] - spine[i]).normalized : (spine[i] - spine[i - 1]).normalized;
                Vector3 right = Vector3.right;
                Vector3 up = Vector3.Cross(fwd, right).normalized;
                right = Vector3.Cross(up, fwd).normalized;

                spineFwd[i] = fwd;
                spineRight[i] = right;
                spineUp[i] = up;
            }

            // Model central slender spine tube
            for (int i = 0; i <= leafletPairs; i++)
            {
                float t = (float)i / leafletPairs;
                float r = Mathf.Lerp(0.032f, 0.008f, t);
                Vector3 p = spine[i];
                Vector3 rVec = spineRight[i] * r;
                Vector3 uVec = spineUp[i] * r * 0.75f;

                int baseIdx = verts.Count;
                verts.Add(p + uVec);          // 0: Top
                verts.Add(p - uVec + rVec);   // 1: Bottom right
                verts.Add(p - uVec - rVec);   // 2: Bottom left

                uvs.Add(new Vector2(0.5f, t));
                uvs.Add(new Vector2(1.0f, t));
                uvs.Add(new Vector2(0.0f, t));

                if (i < leafletPairs)
                {
                    int nxt = baseIdx + 3;
                    tris.Add(baseIdx); tris.Add(nxt); tris.Add(baseIdx + 1);
                    tris.Add(baseIdx + 1); tris.Add(nxt); tris.Add(nxt + 1);

                    tris.Add(baseIdx + 1); tris.Add(nxt + 1); tris.Add(baseIdx + 2);
                    tris.Add(baseIdx + 2); tris.Add(nxt + 1); tris.Add(nxt + 2);

                    tris.Add(baseIdx + 2); tris.Add(nxt + 2); tris.Add(baseIdx);
                    tris.Add(baseIdx); tris.Add(nxt + 2); tris.Add(nxt);
                }
            }

            // 2. Individual Narrow Leaflets extending from both sides of the stem
            // Leaflets emerge starting slightly away from the trunk collar (i >= 2)
            for (int i = 2; i < leafletPairs; i++)
            {
                float t = (float)i / leafletPairs;
                Vector3 p = spine[i];
                Vector3 fwd = spineFwd[i];
                Vector3 right = spineRight[i];
                Vector3 up = spineUp[i];

                // Leaflet length profile: short at base (~0.35m), long in mid (~0.85m), tapering at tip (~0.25m)
                float lengthFactor = Mathf.Sin(t * Mathf.PI);
                float leafLen = lengthFactor * maxWidth;
                float leafWidth = Mathf.Lerp(0.048f, 0.025f, t);

                // Leaflet angle sweeps outward and forward (approx 50-65 deg from stem forward)
                float sweepAngle = Mathf.Lerp(62f, 38f, t);
                Vector3 leftDir = (-right * Mathf.Sin(sweepAngle * Mathf.Deg2Rad) + fwd * Mathf.Cos(sweepAngle * Mathf.Deg2Rad)).normalized;
                Vector3 rightDir = (right * Mathf.Sin(sweepAngle * Mathf.Deg2Rad) + fwd * Mathf.Cos(sweepAngle * Mathf.Deg2Rad)).normalized;

                // Leaflet droop (tips curve downward under gravity)
                float leafDroop = -leafLen * Mathf.Lerp(0.18f, 0.32f, t);

                // --- LEFT LEAFLET (Distinct, isolated blade with open gap to neighbors) ---
                Vector3 lBase0 = p - fwd * (leafWidth * 0.5f);
                Vector3 lBase1 = p + fwd * (leafWidth * 0.5f);
                Vector3 lMid = p + leftDir * (leafLen * 0.55f) + up * (leafWidth * 0.35f) + Vector3.up * (leafDroop * 0.35f);
                Vector3 lTip = p + leftDir * leafLen + Vector3.up * leafDroop;

                int lStart = verts.Count;
                verts.Add(lBase0);
                verts.Add(lBase1);
                verts.Add(lMid);
                verts.Add(lTip);

                uvs.Add(new Vector2(0f, 0f));
                uvs.Add(new Vector2(1f, 0f));
                uvs.Add(new Vector2(0.5f, 0.5f));
                uvs.Add(new Vector2(0.5f, 1f));

                // Double-sided triangles for left leaflet
                tris.Add(lStart);     tris.Add(lStart + 2); tris.Add(lStart + 1);
                tris.Add(lStart + 1); tris.Add(lStart + 2); tris.Add(lStart + 3);
                tris.Add(lStart + 1); tris.Add(lStart + 2); tris.Add(lStart);
                tris.Add(lStart + 3); tris.Add(lStart + 2); tris.Add(lStart + 1);

                // --- RIGHT LEAFLET (Distinct, isolated blade with open gap to neighbors) ---
                Vector3 rBase0 = p - fwd * (leafWidth * 0.5f);
                Vector3 rBase1 = p + fwd * (leafWidth * 0.5f);
                Vector3 rMid = p + rightDir * (leafLen * 0.55f) + up * (leafWidth * 0.35f) + Vector3.up * (leafDroop * 0.35f);
                Vector3 rTip = p + rightDir * leafLen + Vector3.up * leafDroop;

                int rStart = verts.Count;
                verts.Add(rBase0);
                verts.Add(rBase1);
                verts.Add(rMid);
                verts.Add(rTip);

                uvs.Add(new Vector2(0f, 0f));
                uvs.Add(new Vector2(1f, 0f));
                uvs.Add(new Vector2(0.5f, 0.5f));
                uvs.Add(new Vector2(0.5f, 1f));

                // Double-sided triangles for right leaflet
                tris.Add(rStart);     tris.Add(rStart + 1); tris.Add(rStart + 2);
                tris.Add(rStart + 1); tris.Add(rStart + 3); tris.Add(rStart + 2);
                tris.Add(rStart + 2); tris.Add(rStart + 1); tris.Add(rStart);
                tris.Add(rStart + 2); tris.Add(rStart + 3); tris.Add(rStart + 1);
            }

            Mesh mesh = new Mesh { name = "PalmFrond_Feathered" };
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.SetUVs(0, uvs);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        /// <summary>
        /// Generates a 3D stylized coconut mesh.
        /// </summary>
        public static Mesh CreateCoconutMesh(float radius = 0.22f)
        {
            int rings = 6;
            int segments = 8;
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            verts.Add(new Vector3(0f, radius * 1.15f, 0f));
            uvs.Add(new Vector2(0.5f, 1f));

            for (int r = 1; r < rings; r++)
            {
                float phi = (float)r / rings * Mathf.PI;
                float y = Mathf.Cos(phi) * radius * 1.15f;
                float ringRad = Mathf.Sin(phi);

                for (int s = 0; s < segments; s++)
                {
                    float theta = (float)s / segments * Mathf.PI * 2f;
                    // 3 longitudinal ribs
                    float rib = 1.0f + Mathf.Cos(theta * 3f) * 0.08f;
                    float x = Mathf.Cos(theta) * ringRad * radius * rib;
                    float z = Mathf.Sin(theta) * ringRad * radius * rib;

                    verts.Add(new Vector3(x, y, z));
                    uvs.Add(new Vector2((float)s / segments, 1f - (float)r / rings));
                }
            }

            int botIdx = verts.Count;
            verts.Add(new Vector3(0f, -radius * 1.15f, 0f));
            uvs.Add(new Vector2(0.5f, 0f));

            for (int s = 0; s < segments; s++)
            {
                int next = (s + 1) % segments;
                tris.Add(0); tris.Add(1 + s); tris.Add(1 + next);
            }

            for (int r = 0; r < rings - 2; r++)
            {
                int row0 = 1 + r * segments;
                int row1 = 1 + (r + 1) * segments;
                for (int s = 0; s < segments; s++)
                {
                    int next = (s + 1) % segments;
                    tris.Add(row0 + s); tris.Add(row1 + s);    tris.Add(row0 + next);
                    tris.Add(row0 + next); tris.Add(row1 + s); tris.Add(row1 + next);
                }
            }

            int lastRow = 1 + (rings - 2) * segments;
            for (int s = 0; s < segments; s++)
            {
                int next = (s + 1) % segments;
                tris.Add(lastRow + s); tris.Add(botIdx); tris.Add(lastRow + next);
            }

            Mesh mesh = new Mesh { name = "Mesh_Coconut" };
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.SetUVs(0, uvs);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        #endregion

        #region Stylized Grass Tufts

        /// <summary>
        /// Generates a 3D stylized grass tuft (cross-plane with natural curved blades).
        /// </summary>
        public static Mesh CreateGrassTuftMesh(float height, float spread)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            int planes = 3;
            for (int p = 0; p < planes; p++)
            {
                float angle = (float)p / planes * Mathf.PI;
                Vector3 dir = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
                Vector3 normal = new Vector3(-dir.z, 0f, dir.x);

                int start = verts.Count;
                verts.Add(-dir * (spread * 0.5f));
                verts.Add(dir * (spread * 0.5f));
                verts.Add(-dir * (spread * 0.35f) + Vector3.up * height + normal * 0.08f);
                verts.Add(dir * (spread * 0.35f) + Vector3.up * height - normal * 0.08f);

                uvs.Add(new Vector2(0f, 0f));
                uvs.Add(new Vector2(1f, 0f));
                uvs.Add(new Vector2(0f, 1f));
                uvs.Add(new Vector2(1f, 1f));

                // Front and back faces
                tris.Add(start); tris.Add(start + 2); tris.Add(start + 1);
                tris.Add(start + 1); tris.Add(start + 2); tris.Add(start + 3);

                tris.Add(start + 1); tris.Add(start + 2); tris.Add(start);
                tris.Add(start + 3); tris.Add(start + 2); tris.Add(start + 1);
            }

            Mesh mesh = new Mesh { name = "GrassTuft" };
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.SetUVs(0, uvs);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        #endregion
    }
}
