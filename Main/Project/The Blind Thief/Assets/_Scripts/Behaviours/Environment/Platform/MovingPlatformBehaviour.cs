using UnityEngine;
using System.Collections;

public class MovingPlatformBehaviour : PlatformBehaviour
{
	//Movement
    [Header("Movement")]
	[SerializeField] private float movementTime;
	[SerializeField] private AnimationCurve movementCurve;
	private bool isMoving;
	private float timeStartedLerping;

    private Transform parentSwitch;

	//Targets
	[HideInInspector][SerializeField] private Vector3 firstPosition;
    public Vector3 FirstPosition
    {
        get { return firstPosition; }
    } //Access by the waypoints
	[HideInInspector][SerializeField] private Vector3 secondPosition;
    public Vector3 SecondPosition
    {
        get { return secondPosition; }
    } //Accessed by the waypoints

	private Vector3 startingPosition;
	private Vector3 targetPosition;

	private int positionCount = 0;

    [Header("Waypoints")]
    [SerializeField] private GameObject wayPoints;

    //Events
    public delegate void startedMoving();
    public startedMoving StartedMoving;
    public delegate void endedMoving();
    public endedMoving EndedMoving;

	public override void ActivateSwitchBehaviour(Transform _enablerer)
	{
	    if (parentSwitch == null)
	        parentSwitch = _enablerer;

		if(!isMoving)
        {
            InitiateMovement();
            if (StartedMoving != null)
                StartedMoving();
        }
			
	}

	void InitiateMovement()
	{
		if(positionCount == 0)
		{
			startingPosition = firstPosition;
			targetPosition = secondPosition;
		}
		else
		{
			startingPosition = secondPosition;
			targetPosition = firstPosition;
		}

		positionCount++;
		if(positionCount > 1)
			positionCount = 0;

		timeStartedLerping = Time.time;
		isMoving = true;
	}

	public override void Update()
	{
        base.Update();

        if (isMoving)
			MovePlatform();
	}

	void MovePlatform()
	{
		float _timeSinceStarted = Time.time - timeStartedLerping;
		float _percentageComplete = _timeSinceStarted /movementTime;

		transform.position = Vector3.Lerp(startingPosition,targetPosition,movementCurve.Evaluate(_percentageComplete));

		if(_percentageComplete >= 1.0f)
		{
            StartCoroutine(EndPlatformMovement());
		}
	}

    IEnumerator EndPlatformMovement()
    {
        isMoving = false;
        yield return StartCoroutine(NodeController.Instance.GetNodes());
        parentSwitch.SendMessage("EnableSwitch");
        if (EndedMoving != null)
            EndedMoving();
    }

	#region EditorFunctions

	public void SetToFirstPosition()
	{
		transform.position = firstPosition;
	}

	public void SetFirstPosition(Vector3 _firstPos)
	{
		firstPosition = transform.localPosition;
	}

	public void SetSecondPosition(Vector3 _secondPos)
	{
		secondPosition = transform.localPosition;
	}

    public void CreateWaypoints()
    {
        GameObject _waypoint = (GameObject)Instantiate(wayPoints);
        MovingPlatformWaypoints _mpwScript = _waypoint.GetComponent<MovingPlatformWaypoints>();

        _mpwScript.MPBScript = this;
    }


    #endregion
}
