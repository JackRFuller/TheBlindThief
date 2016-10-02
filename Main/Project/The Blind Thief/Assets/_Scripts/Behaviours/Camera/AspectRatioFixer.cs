using UnityEngine;

public class AspectRatioFixer : MonoBehaviour
{
	private Camera mainCamera;

	private float cameraHeight;
	private float desiredAspect = 16.0f/9.0f;
	private float oldAspect;

	void Start()
	{
		mainCamera = GetComponent<Camera>();
		cameraHeight = mainCamera.orthographicSize;
	}

	void Update()
	{
		if(oldAspect != mainCamera.aspect)
		{
			FixCameraSize();
		}
	}

	void FixCameraSize()
	{
		oldAspect = mainCamera.aspect;
		if(cameraHeight * desiredAspect > cameraHeight * mainCamera.aspect)
		{
			float ratio = desiredAspect / mainCamera.aspect;
			mainCamera.orthographicSize = cameraHeight * ratio;
		}
		else
		{
			mainCamera.orthographicSize = cameraHeight;
		}
	}
}

