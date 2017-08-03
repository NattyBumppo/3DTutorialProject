using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TutorialHelper : MonoBehaviour {

    public bool showNormals;
    public bool showVertices;
    public bool showBoundingBox;

    public float vertexRadius;
    public float normalLength;

    private List<Material> initialMaterials = new List<Material>();
    public Material uvMaterial;
    public Material vertexCurvingMaterial;
    public Material vertexAnimatingMaterial;
    public Material furMaterial;
    public Material uShadingMaterial;
    public Material vShadingMaterial;
    public Material uvShadingMaterial;

    public List<MeshFilter> objectMeshFilters = new List<MeshFilter>();
    public List<MeshRenderer> objectMeshRenderers = new List<MeshRenderer>();

    public MeshFilter triMeshFilter;
    public MeshFilter heightmapMeshFilter;

    public Text meshSummaryText;

    void Start()
    {

        //GenerateTriangle();
        //GenerateHeightmapMesh();

        for (int i = 0; i < objectMeshRenderers.Count; i++)
        {
            MeshRenderer mr = objectMeshRenderers[i];
            initialMaterials.Add(mr.sharedMaterial);
        }

        meshSummaryText.text = SummarizeMeshes();
	}
	
    // Make a single triangle
    private void GenerateTriangle()
    {
        // We'll make a triangle that looks like this:
        //
        //                       2
        //                   __/ |      ^
        //                __/    |      |
        //             __/       |      |
        //            /          |      |
        //          O------------1      |
        //                              y
        //    x ---------------->
        //

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[3];
        Vector2[] uvs = new Vector2[3];
        int[] triangles = new int[9];

        vertices[0] = new Vector3(0.0f, -1.0f, 0.0f);
        vertices[1] = new Vector3(1.0f, -1.0f, 0.0f);
        vertices[2] = new Vector3(1.0f, 0.0f, 0.0f);

        uvs[0] = new Vector2(0.0f, 0.0f);
        uvs[1] = new Vector2(0.0f, 1.0f);
        uvs[2] = new Vector2(1.0f, 1.0f);

        // Clockwise winding order
        triangles[0] = 0;
        triangles[1] = 2;
        triangles[2] = 1;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        // Auto-calculate normals
        mesh.RecalculateNormals();
        // Set correct bounding box
        mesh.RecalculateBounds();

        // Apply mesh to mesh filter
        triMeshFilter.sharedMesh = mesh;

        // Add to list of meshFilters/meshRenderers
        objectMeshFilters.Add(triMeshFilter);
        objectMeshRenderers.Add(triMeshFilter.GetComponent<MeshRenderer>());
    }

    // Make a heightmap !
    private void GenerateHeightmapMesh()
    {
        float[,] heightmapValues = new float[4, 4]
        {
            {0.0f, 0.1f, 0.15f, 0.1f },
            {0.1f, 0.2f, 0.25f, 0.2f },
            { 0.15f, 0.25f, 0.6f, 0.4f },
            { 0.1f, 0.2f, 0.25f, 0.2f }
        };
        int numPointsX = 4;
        int numPointsZ = 4;
        float cellDiameter = 0.25f;

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[(numPointsX - 1) * (numPointsZ - 1) * 4];
        Vector2[] uvs = new Vector2[(numPointsX - 1) * (numPointsZ - 1) * 4];
        int[] triangles = new int[(numPointsX - 1) * (numPointsZ - 1) * 2 * 3];

        // Let's make strips of quads (two triangles), one for each two adjacent rows of heightmap values.
        // We won't worry about reusing vertices (possible, but would complicate the math a bit).
        // Values increase in x as we go down a row; as we go down a column, values *decrease* in z.
        // (This is necessary because of Unity's left-handed coordinate system.)
        for (int zIndex = 0; zIndex < numPointsZ - 1; zIndex++)
        {
            for (int xIndex = 0; xIndex < numPointsX - 1; xIndex++)
            {
                // Let's make a quad that looks like this:
                //
                //          O-------------2      z
                //          |         __/ |      |
                //          |      __/    |      |
                //          |   __/       |      |
                //          |  /          |      |
                //          1-------------3      |
                //                               v
                //    x ---------------->
                //

                //Vector3 vertex0 = new Vector3((float)xIndex * cellDiameter, heightmapValues[zIndex, xIndex], -(float)zIndex * cellDiameter);
                //Vector3 vertex1 = new Vector3((float)xIndex * cellDiameter, heightmapValues[zIndex, xIndex], -(float)(zIndex + 1) * cellDiameter);
                //Vector3 vertex2 = new Vector3((float)(xIndex + 1) * cellDiameter, heightmapValues[zIndex, xIndex], (float)zIndex * cellDiameter);
                //Vector3 vertex3 = new Vector3((float)(xIndex + 1) * cellDiameter, heightmapValues[zIndex, xIndex], (float)(zIndex + 1) * cellDiameter);

                Vector3 vertex0 = new Vector3(xIndex * cellDiameter, heightmapValues[zIndex, xIndex], -zIndex * cellDiameter);
                Vector3 vertex1 = new Vector3(xIndex * cellDiameter, heightmapValues[zIndex+1, xIndex], -(zIndex + 1) * cellDiameter);
                Vector3 vertex2 = new Vector3((xIndex + 1) * cellDiameter, heightmapValues[zIndex, xIndex+1], -zIndex * cellDiameter);
                Vector3 vertex3 = new Vector3((xIndex + 1) * cellDiameter, heightmapValues[zIndex+1, xIndex+1], -(zIndex + 1) * cellDiameter);

                // Add verts
                int vert0Index = (zIndex * (numPointsX - 1) + xIndex) * 4;
                vertices[vert0Index] = vertex0;
                vertices[vert0Index + 1] = vertex1;
                vertices[vert0Index + 2] = vertex2;
                vertices[vert0Index + 3] = vertex3;

                // Add tris (clockwise winding order)
                triangles[(zIndex * (numPointsX - 1) + xIndex) * 6] = vert0Index; // tri 0
                triangles[(zIndex * (numPointsX - 1) + xIndex) * 6 + 1] = vert0Index + 2; // tri 2
                triangles[(zIndex * (numPointsX - 1) + xIndex) * 6 + 2] = vert0Index + 1; // tri 1

                triangles[(zIndex * (numPointsX - 1) + xIndex) * 6 + 3] = vert0Index + 1; // tri 1
                triangles[(zIndex * (numPointsX - 1) + xIndex) * 6 + 4] = vert0Index + 2; // tri 2
                triangles[(zIndex * (numPointsX - 1) + xIndex) * 6 + 5] = vert0Index + 3; // tri 3

                // Set UVs according to where we are in the heightmap (0,0 in upper left corner; 1,1 in lower left corner)
                uvs[(zIndex * (numPointsX - 1) + xIndex) * 4] = new Vector2((float)xIndex / (float)(numPointsX - 1), 1.0f - (float)zIndex / (float)(numPointsZ - 1));
                uvs[(zIndex * (numPointsX - 1) + xIndex) * 4 + 1] = new Vector2((float)xIndex / (float)(numPointsX - 1), 1.0f - (float)(zIndex + 1) / (float)(numPointsZ - 1));
                uvs[(zIndex * (numPointsX - 1) + xIndex) * 4 + 2] = new Vector2((float)(xIndex + 1) / (float)(numPointsX - 1), 1.0f - (float)zIndex / (float)(numPointsZ - 1));
                uvs[(zIndex * (numPointsX - 1) + xIndex) * 4 + 3] = new Vector2((float)(xIndex + 1) / (float)(numPointsX - 1), 1.0f - (float)(zIndex + 1) / (float)(numPointsZ - 1));
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        // Auto-calculate normals
        mesh.RecalculateNormals();
        // Set correct bounding box
        mesh.RecalculateBounds();

        // Apply mesh to mesh filter
        heightmapMeshFilter.sharedMesh = mesh;

        // Add to list of meshFilters/meshRenderers
        objectMeshFilters.Add(heightmapMeshFilter);
        objectMeshRenderers.Add(heightmapMeshFilter.GetComponent<MeshRenderer>());
    }

    private string SummarizeMeshes()
    {
        string summaryText = "";

        for (int i = 0; i < objectMeshFilters.Count; i++)
        {
            summaryText += "<b>" + objectMeshFilters[i].gameObject.name + "</b>\n";
            summaryText += "Vertex Count: " + objectMeshFilters[i].sharedMesh.vertexCount + "\n";
            summaryText += "Triangle Count: " + objectMeshFilters[i].sharedMesh.triangles.Length / 3 + "\n\n";
        }

        return summaryText;
    }

	// Update is called once per frame
	void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            ApplyUvMaterial();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            ApplyInitialMaterials();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            ApplyVertexTweakingMaterial();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            ApplyFurMaterial();
        }

        //if (Input.GetKeyDown(KeyCode.G))
        //{
        //    GenerateTriangle();
        //}

        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    GenerateHeightmapMesh();
        //}
    }

    private void ApplyInitialMaterials()
    {
        for (int i = 0; i < objectMeshRenderers.Count; i++)
        {
            MeshRenderer mr = objectMeshRenderers[i];
            mr.sharedMaterial = initialMaterials[i];
        }
    }

    private void ApplyUvMaterial()
    {
        for (int i = 0; i < objectMeshRenderers.Count; i++)
        {
            MeshRenderer mr = objectMeshRenderers[i];
            mr.sharedMaterial = uvMaterial;
        }
    }

    private void ApplyVertexTweakingMaterial()
    {
        for (int i = 0; i < objectMeshRenderers.Count; i++)
        {
            MeshRenderer mr = objectMeshRenderers[i];
            mr.sharedMaterial = vertexAnimatingMaterial;
        }
    }

    private void ApplyFurMaterial()
    {
        for (int i = 0; i < objectMeshRenderers.Count; i++)
        {
            MeshRenderer mr = objectMeshRenderers[i];
            mr.sharedMaterial = furMaterial;
        }
    }

    void OnDrawGizmos()
    {
        if (showVertices)
        {
            for (int i = 0; i < objectMeshFilters.Count; i++)
            {
                Transform t = objectMeshFilters[i].transform;
                for (int j = 0; j < objectMeshFilters[i].sharedMesh.vertices.Length; j++)
                {
                    Vector3 vertex = objectMeshFilters[i].sharedMesh.vertices[j];
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(t.TransformPoint(vertex), vertexRadius);
                }

            }
        }

        if (showNormals)
        {
            for (int i = 0; i < objectMeshFilters.Count; i++)
            {
                Transform t = objectMeshFilters[i].transform;
                for (int j = 0; j < objectMeshFilters[i].sharedMesh.vertices.Length; j++)
                {
                    Vector3 vertex = objectMeshFilters[i].sharedMesh.vertices[j];
                    Vector3 normal = objectMeshFilters[i].sharedMesh.normals[j];
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawLine(t.TransformPoint(vertex), t.TransformPoint(vertex + normal * normalLength * (1.0f / t.localScale.x)));
                }
            }
        }

        if (showBoundingBox)
        {
            for (int i = 0; i < objectMeshFilters.Count; i++)
            {
                Transform t = objectMeshFilters[i].transform;
                Gizmos.DrawWireCube(t.TransformPoint(objectMeshFilters[i].sharedMesh.bounds.center), t.TransformVector(objectMeshFilters[i].sharedMesh.bounds.size));
            }
        }

    }
}
