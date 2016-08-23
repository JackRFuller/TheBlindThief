using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WireGeneration : MonoBehaviour
{
    [SerializeField]
    private Transform[] wireWaypoints;

    [Header("Platform Target")]
    [SerializeField]
    private GameObject[] targetPlatforms;

    [Header("Fill Rate")]
    [SerializeField] private float fillRate;

    //Wire Behaviour Elements
    [SerializeField]    
    private List<Image> wireImages;
    private int wireCount;
    private Image activeWire;
    private bool activateWires;          

    [Header("Prefabs - Used for Wire Generation")]
    [SerializeField] private GameObject wire;
    [SerializeField] private GameObject wireConnector;
    [SerializeField] private GameObject wireBorder;
    [SerializeField] private GameObject wireConnectorBorder;
    private GameObject wireHolder;
    

    //TODO: Need to add audio to wires
    void Start()
    {
        InitiateWireFill();
    }

    void Update()
    {
        if (activateWires)
            FillWire();
    }

    #region WireBehaviour

    void InitiateWireFill()
    {
        wireCount = 0;
        activeWire = wireImages[0];
    }

    void ActivateWire()
    {        
        activateWires = true;
        Debug.Log("Success");

    }

    void FillWire()
    {
        activeWire.fillAmount += fillRate * Time.smoothDeltaTime;

        if(activeWire.fillAmount >= 1.0f)
        {
            //Determine if we're at the end of the wire
            if(wireCount < wireImages.Count -1)
            {
                wireCount++;
                activeWire = wireImages[wireCount];
            }
            else
            {
                EndWireFill();
            }
        }
    }

    void EndWireFill()
    {
        activateWires = false;

        for(int i = 0; i < targetPlatforms.Length; i++)
        {
            targetPlatforms[i].SendMessage("ActivatePlatform", SendMessageOptions.DontRequireReceiver);
        }
    }

    #endregion


    #region WireGeneration

    public void CreateWires()
    {
        if(wireHolder != null)
        {
            DestroyImmediate(wireHolder);
        }

        if (wireHolder == null)
        {
            wireHolder = new GameObject();
            wireHolder.name = "Wire Holder";
            wireHolder.transform.parent = this.transform;            
        }      

        //Initialise List
        wireImages = new List<Image>();

        int wirePosition = 0;

        for (int i = 0; i < wireWaypoints.Length - 1; i++)
        {
            GameObject _newConnector = null;

            if (i != 0)
            {
                //Generate Connection Borders
                GameObject newConnectorBorder = (GameObject)Instantiate(wireConnectorBorder);
                newConnectorBorder.transform.position = wireWaypoints[i].position;
                newConnectorBorder.GetComponent<RectTransform>().SetParent(wireHolder.transform);

                //Generation Connections
                _newConnector = (GameObject)Instantiate(wireConnector);
                _newConnector.transform.position = wireWaypoints[i].position;
                _newConnector.GetComponent<RectTransform>().SetParent(wireHolder.transform);

                Image connectorImage = _newConnector.GetComponent<Image>();
                wireImages.Add(connectorImage);                           
            }

            //Create wire
            GameObject _newWire = (GameObject)Instantiate(wire);
            RectTransform _transform = _newWire.GetComponent<RectTransform>();

            //Create Wire Border
            GameObject newWireBorder = (GameObject)Instantiate(wireBorder);
            RectTransform wireBorderTransform = newWireBorder.GetComponent<RectTransform>();

            //Place Wire
            _transform.position = GetNewWirePosition(wireWaypoints[i], wireWaypoints[i + 1]);
            wireBorderTransform.position = _transform.position;

            //GetWireSize
            Vector3 _wireSize = GetWireSize(wireWaypoints[i].position, wireWaypoints[i + 1].position);
            _transform.localScale = _wireSize;
            wireBorderTransform.localScale = _transform.localScale;

            //Rotate Wire
            Vector3 _wireRotation = GetWireRotation(wireWaypoints[i].position, wireWaypoints[i + 1].position);
            _transform.rotation = Quaternion.Euler(_wireRotation);
            wireBorderTransform.rotation = _transform.rotation;
            wireBorderTransform.SetParent(wireHolder.transform);
            wireBorderTransform.SetSiblingIndex(wirePosition);
            wirePosition++;

            _transform.SetParent(wireHolder.transform);
            _transform.SetSiblingIndex(wirePosition);
            wirePosition++;            

            Image wireImage = _newWire.GetComponent<Image>();
            wireImages.Add(wireImage);
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

    //TODO: Make it so connector rotation is set automatically
    Vector3 GetConnectorRotation(Vector3 wireRot)
    {
        float zRot = wireRot.z;
        zRot = Mathf.Round(zRot);

        Debug.Log(zRot);

        float newZRot = 0;

        if(zRot == 0)
        {
            newZRot = 90;
        }
        else if(zRot == 180)
        {
            newZRot = -90;
        }
        else if(zRot == 90)
        {
            newZRot = -180;
        }
        else if(zRot == 270)
        {
            newZRot = 0;
        }

        Vector3 newRot = new Vector3(0,0,newZRot);
        return newRot;
    }


    Vector3 GetNewWirePosition(Transform _wireWaypoint1, Transform _wireWaypoint2)
    {
        Vector3 _newPos = Vector3.zero;        
        _newPos = (_wireWaypoint2.position + _wireWaypoint1.position) * 0.5f;       

        return _newPos;
    }

    #endregion

}
