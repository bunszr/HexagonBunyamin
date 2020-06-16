using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// En başta mesh deformasyonu ile güzel animasyonlar elde etmek için mesh ile yapmaya başlamıştım. Performans sıkıntısı olabilir die vazgeçtim.

public class HexMeshGenerator : MonoBehaviour
{
    public MeshFilter meshFilter;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();

        Mesh mesh = GenerateHexagon(HexInfo.outerRadius);
        meshFilter.mesh = mesh;
    }

    public Mesh GenerateHexagon (float radius)
    {
        List<Vector3> segments = new List<Vector3> ();
        float stepAngle = 6f;
        float stepAngleSize = 360f / stepAngle;
        for (int i = 0; i <= stepAngle; i++)
        {
            float angle = 360f - i * stepAngleSize;
            float x = Mathf.Cos (angle * Mathf.Deg2Rad) * radius;
            float y = Mathf.Sin (angle * Mathf.Deg2Rad) * radius;

            segments.Add (new Vector3 (x , y));
        }
        Vector3[] vertices = new Vector3[segments.Count + 1];
        int[] triangle = new int[(vertices.Length - 2) * 3];
        vertices[0] = Vector2.zero;
        for (int i = 0; i < vertices.Length - 1; i++)
        {
            vertices[i + 1] = segments[i];

            if (i < vertices.Length - 2)
            {
                triangle[i * 3] = 0;
                triangle[i * 3 + 1] = i + 1;
                triangle[i * 3 + 2] = i + 2;
            }
        }
        Mesh mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.uv = GetUvs(radius, mesh);
        mesh.triangles = triangle;
        mesh.RecalculateNormals ();
        return mesh;
    }

    Vector2[] GetUvs (float radius, Mesh mesh)
    {
        Vector2[] uvs = new Vector2[mesh.vertices.Length];
        Vector2 A = new Vector3(.5f, .5f);
        uvs[0] = A;
        for (int i = 1; i < uvs.Length; i++)
        {
            uvs[i] = A + new Vector2(mesh.vertices[i].x, mesh.vertices[i].y) / radius / 2;
        }
        return uvs;
    }

}