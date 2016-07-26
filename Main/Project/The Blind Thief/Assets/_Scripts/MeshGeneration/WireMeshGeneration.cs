using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WireMeshGeneration : MonoBehaviour {

	[SerializeField] private Transform[] wireWaypoints;

    public void CreateWire()
    {
        //TO:DO = Merge meshes
        List<MeshFilter> meshes = new List<MeshFilter>();        

        for(int i = 0; i < wireWaypoints.Length - 1; i++)
        {
            //Work out where in relation the second wire is
            int _travellingDirection = GetWaypointDirection(wireWaypoints[i], wireWaypoints[i + 1]);

            int _amountOfQuads = GetDistanceBetween(wireWaypoints[i], wireWaypoints[i + 1]);

            for(int j = 0; j < _amountOfQuads; j++)
            {
                GameObject quad = CreateQuad();

                Vector3 _placementPosition = GetQuadPlacement(wireWaypoints[i].position, _travellingDirection, j);

                quad.transform.position = _placementPosition;
            }            
        }
    }

    void MergeQuadsInToOneMesh(MeshFilter[] _meshFilters, CombineInstance[] _combine)
    {

    }

    

    GameObject CreateQuad()
    {
        Mesh mesh = new Mesh();

        Vector3[] _verts = new Vector3[4];
        Vector2[] _uvs = new Vector2[4];
        int[] tris = new int[6] { 0, 1, 2, 2, 1, 3 };        

        _verts[0] = -Vector3.right + Vector3.up;
        _verts[1] = Vector3.right + Vector3.up;
        _verts[2] = -Vector3.right - Vector3.up;
        _verts[3] = Vector3.right - Vector3.up; 
        
        _uvs[0] = new Vector2(0.0f, 1.0f);
        _uvs[1] = new Vector2(1.0f, 1.0f);
        _uvs[2] = new Vector2(0.0f, 0.0f);
        _uvs[3] = new Vector2(1.0f, 0.0f);

        mesh.vertices = _verts;
        mesh.triangles = tris;
        mesh.uv = _uvs;

        mesh.RecalculateNormals();

        GameObject newQuad = new GameObject();

        newQuad.AddComponent<MeshFilter>().mesh = mesh;
        newQuad.AddComponent<MeshRenderer>();

        newQuad.name = "Quad";
        newQuad.transform.parent = this.transform;

        return newQuad;
    }

    Vector3 GetQuadPlacement(Vector3 _intialPosition,int _placementDirection, int _numberOfSteps)
    {
        Vector3 _newPosition = _intialPosition;

        switch (_placementDirection)
        {
            case (0): //Going Up
                _newPosition.y += _numberOfSteps;
                break;
            case (1): //Going Down
                _newPosition.y -= _numberOfSteps;
                break;
            case (2): //Going Right
                _newPosition.x += _numberOfSteps;
                break;
            case (3): //Going Left
                _newPosition.x -= _numberOfSteps;
                break;
        }

        return _newPosition;
    }

    //Works out what direction we're travelling in
    int GetWaypointDirection(Transform _waypoint1, Transform _waypoint2)
    {
        int _direction = 0;

        if (_waypoint2.position.y > _waypoint1.position.y)
            _direction = 0; //Going Up
        else if (_waypoint2.position.y < _waypoint1.position.y)
            _direction = 1; //Going down
        else if (_waypoint2.position.x > _waypoint1.position.x)
            _direction = 2; //Going right
        else if (_waypoint2.position.x < _waypoint1.position.x)
            _direction = 3; //Going Left

        return _direction;
    }

    int GetDistanceBetween(Transform _waypoint1, Transform _waypoint2)
    {
        float _distanceBetween = 0;

        if (_waypoint2.position.y > _waypoint1.position.y)
            _distanceBetween = _waypoint2.position.y - _waypoint1.position.y;

        else if (_waypoint2.position.y < _waypoint1.position.y)
            _distanceBetween = _waypoint2.position.y - _waypoint1.position.y;

        else if (_waypoint2.position.x > _waypoint1.position.x)
            _distanceBetween = _waypoint2.position.y - _waypoint1.position.y;

        else if (_waypoint2.position.x < _waypoint1.position.x)
            _distanceBetween = _waypoint2.position.y - _waypoint1.position.y;

        return Mathf.RoundToInt(_distanceBetween);

    }


}
