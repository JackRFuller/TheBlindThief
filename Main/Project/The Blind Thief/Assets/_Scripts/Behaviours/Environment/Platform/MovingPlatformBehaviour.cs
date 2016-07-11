using UnityEngine;
using System.Collections;

public class MovingPlatformBehaviour : PlatformBehaviour
{
	//Movement
	[SerializeField] private float movementTime;
	[SerializeField] private AnimationCurve movementCurve;
	private bool isMoving;
	private float timeStartedLerping;

	//Targets

	[HideInInspector][SerializeField] private Vector3 firstPosition;
	[HideInInspector][SerializeField] private Vector3 secondPosition;

	private Vector3 startingPosition;
	private Vector3 targetPosition;

	private int positionCount = 0;

	public override void ActivateSwitchBehaviour()
	{
		if(!isMoving)
			InitiateMovement();
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

	void Update()
	{
		if(isMoving)
			MovePlatform();
	}

	void MovePlatform()
	{
		float _timeSinceStarted = Time.time - timeStartedLerping;
		float _percentageComplete = _timeSinceStarted /movementTime;

		transform.position = Vector3.Lerp(startingPosition,targetPosition,movementCurve.Evaluate(_percentageComplete));

		if(_percentageComplete >= 1.0f)
		{
			isMoving = false;
			NodeController.Instance.GetNodes();
		}
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


	#endregion
}
