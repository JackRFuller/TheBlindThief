using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WireMeshGeneration : MonoBehaviour {

    [Header("WireWaypoints")]
	[SerializeField] private Transform[] wireWaypoints;

    [Header("Wire")]
    [SerializeField] private GameObject wireMesh;

    private GameObject wireHolder;

    //Mesh Filters
    private MeshFilter[] meshFilters;
    private CombineInstance[] combine;

    public void PlaceWires()
    {
        if (wireHolder != null)
           DestroyImmediate(wireHolder);

        GameObject _WireHolder = new GameObject();
        _WireHolder.name = "Wires";
        //Instantiate(_WireHolder);

        //Mesh
        meshFilters = new MeshFilter[wireWaypoints.Length - 1];

        _WireHolder.transform.parent = this.transform;

        for(int i = 0; i < wireWaypoints.Length - 1; i++)
        {
            //Spawn in wire mesh
            GameObject _wire = (GameObject)Instantiate(wireMesh);

            //Work out placement position
            _wire.transform.position = GetWirePlacement(wireWaypoints[i].position, wireWaypoints[i + 1].position);

            //Work out Rotation
            Vector3 _wireRotation = GetWireRotation(wireWaypoints[i].position, wireWaypoints[i + 1].position);
            _wire.transform.rotation = Quaternion.Euler(_wireRotation);

            //Work out Size
            _wire.transform.localScale = GetWireSize(wireWaypoints[i].position, wireWaypoints[i + 1].position);

            _wire.transform.parent = _WireHolder.transform;

            meshFilters[i] = _wire.GetComponent<MeshFilter>();
        }

        wireHolder = _WireHolder;

        //CreateMesh();
    }

    void CreateMesh()
    {
        combine = new CombineInstance[meshFilters.Length];

        int i = 0;

        while(i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
            i++;
        }

        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.gameObject.active = true;


    }

    Vector3 GetWireSize(Vector3 _wire1Pos, Vector3 _wire2Pos)
    {
        Vector3 _wireSize = Vector3.zero;

        float _dist = Vector3.Distance(_wire2Pos, _wire1Pos);

        _wireSize = new Vector3(1, _dist, 1);

        return _wireSize;
    }

    Vector3 GetWireRotation(Vector3 _wire1Pos, Vector3 _wire2Pos)
    {
        Vector3 _wireRotation = Vector3.zero;

        if (_wire2Pos.y > _wire1Pos.y)
            _wireRotation = new Vector3(0, 0, 180);
        else if (_wire2Pos.y < _wire1Pos.y)
            _wireRotation = new Vector3(0, 0, 0);
        else if (_wire2Pos.x > _wire1Pos.x)
            _wireRotation = new Vector3(0, 0, 90);
        else if (_wire2Pos.x < _wire1Pos.x)
            _wireRotation = new Vector3(0, 0, 270);

        return _wireRotation;
    }

    Vector3 GetWirePlacement(Vector3 _wire1Pos, Vector3 _wire2Pos)
    {
        Vector3 _midPoint = Vector3.zero;
        _midPoint = (_wire1Pos + _wire2Pos) / 2;
        return _midPoint;
    }
}
