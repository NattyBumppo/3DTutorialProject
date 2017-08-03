using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialHelper : MonoBehaviour {

    public bool showNormals;
    public bool showVertices;
    public bool showBoundingBox;

    public float vertexRadius;
    public float normalLength;

    private List<Material> initialMaterials = new List<Material>();
    public Material uvMaterial;
    public Material vertexTweakingMaterial;
    public Material furMaterial;

    public List<MeshFilter> meshFilters = new List<MeshFilter>();
    public List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

    public Text meshSummaryText;

    void Start()
    {
        for (int i = 0; i < meshRenderers.Count; i++)
        {
            MeshRenderer mr = meshRenderers[i];
            initialMaterials.Add(mr.sharedMaterial);
        }

        meshSummaryText.text = SummarizeMeshes();
	}
	
    private string SummarizeMeshes()
    {
        string summaryText = "";

        for (int i = 0; i < meshFilters.Count; i++)
        {
            summaryText += "<b>" + meshFilters[i].gameObject.name + "</b>\n";
            summaryText += "Vertex Count: " + meshFilters[i].sharedMesh.vertexCount + "\n";
            summaryText += "Triangle Count: " + meshFilters[i].sharedMesh.triangles.Length / 3 + "\n\n";
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
    }

    private void ApplyInitialMaterials()
    {
        for (int i = 0; i < meshRenderers.Count; i++)
        {
            MeshRenderer mr = meshRenderers[i];
            mr.sharedMaterial = initialMaterials[i];
        }
    }

    private void ApplyUvMaterial()
    {
        for (int i = 0; i < meshRenderers.Count; i++)
        {
            MeshRenderer mr = meshRenderers[i];
            mr.sharedMaterial = uvMaterial;
        }
    }

    private void ApplyVertexTweakingMaterial()
    {
        for (int i = 0; i < meshRenderers.Count; i++)
        {
            MeshRenderer mr = meshRenderers[i];
            mr.sharedMaterial = vertexTweakingMaterial;
        }
    }

    private void ApplyFurMaterial()
    {
        for (int i = 0; i < meshRenderers.Count; i++)
        {
            MeshRenderer mr = meshRenderers[i];
            mr.sharedMaterial = furMaterial;
        }
    }

    void OnDrawGizmos()
    {
        if (showVertices)
        {
            for (int i = 0; i < meshFilters.Count; i++)
            {
                Transform t = meshFilters[i].transform;
                for (int j = 0; j < meshFilters[i].sharedMesh.vertices.Length; j++)
                {
                    Vector3 vertex = meshFilters[i].sharedMesh.vertices[j];
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(t.TransformPoint(vertex), vertexRadius);
                }

            }
        }

        if (showNormals)
        {
            for (int i = 0; i < meshFilters.Count; i++)
            {
                Transform t = meshFilters[i].transform;
                for (int j = 0; j < meshFilters[i].sharedMesh.vertices.Length; j++)
                {
                    Vector3 vertex = meshFilters[i].sharedMesh.vertices[j];
                    Vector3 normal = meshFilters[i].sharedMesh.normals[j];
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawLine(t.TransformPoint(vertex), t.TransformPoint(vertex + normal * normalLength * (1.0f / t.localScale.x)));
                }
            }
        }

        if (showBoundingBox)
        {
            for (int i = 0; i < meshFilters.Count; i++)
            {
                Gizmos.DrawWireCube(meshFilters[i].sharedMesh.bounds.center, meshFilters[i].sharedMesh.bounds.size);
            }
        }

    }
}
