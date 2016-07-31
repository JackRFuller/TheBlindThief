using UnityEngine;
using System.Collections;

public class WireGeneration : MonoBehaviour
{   
    [Header("Wire Waypoints")]
    [SerializeField] private Transform[] wireWaypoints;
    [SerializeField] private GameObject wire;
    private GameObject wireHolder;

    public void CreateWires()
    {
        DestroyImmediate(wireHolder);

        if(wireHolder == null)
        {
            GameObject _wireHolder = new GameObject();
            _wireHolder.name = "Wire Holder";
            _wireHolder.transform.parent = this.transform;

            wireHolder = _wireHolder;
        }
            

        for(int i = 0; i < wireWaypoints.Length - 1; i++)
        {           

            //Create wire
            GameObject _newWire = (GameObject)Instantiate(wire);
            RectTransform _transform = _newWire.GetComponent<RectTransform>();

            //Place Wire
            _transform.position = GetNewWirePosition(wireWaypoints[i], wireWaypoints[i + 1]);

            //GetWireSize
            Vector3 _wireSize = GetWireSize(wireWaypoints[i].position, wireWaypoints[i + 1].position);
            _transform.localScale = _wireSize;

            //Rotate Wire
            Vector3 _wireRotation = GetWireRotation(wireWaypoints[i].position, wireWaypoints[i + 1].position);
            _transform.rotation = Quaternion.Euler(_wireRotation);

           

            _newWire.GetComponent<RectTransform>().SetParent(wireHolder.transform);
        }
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


    Vector3 GetNewWirePosition(Transform _wireWaypoint1, Transform _wireWaypoint2)
    {
        Vector3 _newPos = Vector3.zero;

        Debug.Log(_wireWaypoint2.position + " " + _wireWaypoint1.position);
        _newPos = (_wireWaypoint2.position + _wireWaypoint1.position) * 0.5f;
        Debug.Log(_newPos);

        return _newPos;
    }
	
}
