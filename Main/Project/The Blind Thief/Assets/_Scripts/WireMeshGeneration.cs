using UnityEngine;
using System.Collections;

public class WireMeshGeneration : MonoBehaviour {

	Mesh mesh = new Mesh();

    Vector3[] _verts = new Vector3[4];
    Vector2[] _uvs = new Vector2[4];

    int[] tris = new int[6] {0,1,2,2,1,3};

    void Awake()
    {
 
        _verts[1] = Vector3.right + Vector3.up;
        _verts[2] = -Vector3.right - Vector3.up;
        _verts[3] = Vector3.right - Vector3.up;

        _uvs[0] = new Vector2(0.0f,1.0f);
        _uvs[1] = new Vector2(1.0f,1.0f);
        _uvs[2] = new Vector2(0.0f,0.0f);
        _uvs[3] = new Vector2(1.0f,0.0f);

        mesh.vertices = _verts;
        mesh.triangles = tris;
        mesh.uv = _uvs;

        mesh.RecalculateNormals();

        GameObject newQuad = new GameObject();
        newQuad.AddComponent<MeshFilter>().mesh = mesh;
        newQuad.AddComponent<MeshRenderer>();

    }

    
}
