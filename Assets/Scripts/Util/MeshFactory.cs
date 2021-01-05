using System.Collections.Generic;
using UnityEngine;

public static class MeshFactory
{
    public static Mesh CreateSquare2D()
    {
        var vertices = new List<Vector3>(4);
        var triangles = new List<int>(6);
        var uvs = new List<Vector2>(4);

        vertices.Add(new Vector3(-0.5f, -0.5f, 0.0f));
        vertices.Add(new Vector3(0.5f, -0.5f, 0.0f));
        vertices.Add(new Vector3(0.5f, 0.5f, 0.0f));
        vertices.Add(new Vector3(-0.5f, 0.5f, 0.0f));

        triangles.Add(0);
        triangles.Add(2);
        triangles.Add(1);

        triangles.Add(2);
        triangles.Add(0);
        triangles.Add(3);

        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(1, 0));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(0, 1));

        var mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

        return mesh;
    }
}
