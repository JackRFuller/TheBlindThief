using UnityEngine;
using System.Collections;

public class MovingPlatformWaypoints : MonoBehaviour {

	[SerializeField] private MovingPlatformBehaviour mpbScript;
    public MovingPlatformBehaviour MPBScript
    {
        set { mpbScript = value; }
    }

    [Header("Waypoints")]
    [SerializeField] private Transform startingPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private LineRenderer line;

    [Header("Color Lerping")]
    [SerializeField] private MeshRenderer[] endPoints;
    [SerializeField] private float colorLerpTime;
    [SerializeField] private AnimationCurve colorLerpCurve;
    private float timeStartedLerp;
    private bool isLerping;
    private Material newMaterial;
    private Material lineRendererMaterial;
    private Color newColor;
    private Color lineRendererColor;

    public void SetStartAndEndPoint()
    {
        if (mpbScript != null)
        {
            startingPoint.position = mpbScript.FirstPosition;
            endPoint.position = mpbScript.SecondPosition;

            line.SetPosition(0, startingPoint.position);
            line.SetPosition(1, endPoint.position);
        }        
    }

    public void SetToBehind()
    {
        transform.position = new Vector3(0,0,10);
    }

    public void RealignLine()
    {
        line.SetPosition(0, startingPoint.position);
        line.SetPosition(1, endPoint.position);
    }

    //Initiates Color Lerp
    void ActivatePlatform()
    {
        newMaterial = new Material(endPoints[0].material);
        newColor = endPoints[0].material.color;

        lineRendererMaterial = new Material(line.material);
        lineRendererColor = lineRendererMaterial.color;

        timeStartedLerp = Time.time;
        isLerping = true;
    }

    void Update()
    {
        if(isLerping)
            LerpColor();
    }

    void LerpColor()
    {
        
        float _timeSinceStarted = Time.time - timeStartedLerp;
        float _percentageComplete = _timeSinceStarted/colorLerpTime;

        newColor.a = Mathf.Lerp(newColor.a, 1, colorLerpCurve.Evaluate(_percentageComplete));
        lineRendererColor.a = Mathf.Lerp(newColor.a, 255, colorLerpCurve.Evaluate(_percentageComplete));

        newMaterial.color = newColor;
        lineRendererMaterial.color = lineRendererColor;

        for (int i = 0; i < endPoints.Length; i++)
        {
            endPoints[i].material = newMaterial;
        }

        line.material = lineRendererMaterial;

        if (_percentageComplete >= 1.0f)
        {
            isLerping = false;
        }
    }
}
