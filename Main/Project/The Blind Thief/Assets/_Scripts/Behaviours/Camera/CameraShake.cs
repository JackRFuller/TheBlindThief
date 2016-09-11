using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    private bool isTestMode;

    [SerializeField]
    private float shakeAmount;
    [SerializeField]
    private float shakeDuration;

    private bool losesImpactOverTime; //Determines if shake amount decreases over time
    private bool isContinuous; //Has a duration
    private float shakePercentage;
    private float startAmount;
    private float startDuration;

    private bool isShaking;
    private bool activatedBasicShake; 

    void Start()
    {
        if (isTestMode)
            ShakeCamera();
    }

    void ShakeCamera()
    {
        startAmount = shakeAmount;//Set default (start) values
        startDuration = shakeDuration;//Set default (start) values

        if (!isShaking)
            StartCoroutine(ContinuousShake());//Only call the coroutine if it isn't currently running. Otherwise, just set the variables.
    }

    public void ShakeCamera(CameraShakeAttributes cameraShake)
    {
        losesImpactOverTime = cameraShake.losesImpact;
        isContinuous = cameraShake.isContinuous;
        shakeAmount += cameraShake.shakeAmount;
        startAmount = shakeAmount;
        shakeDuration += cameraShake.shakeDuration;
        startDuration = shakeDuration;

        if (!isShaking)
        {
            activatedBasicShake = true;
            StartCoroutine(ContinuousShake());
        }
            
    }

    public void StopCameraShake()
    {
        ResetCameraShake();
    }

    void ResetCameraShake()
    {
        activatedBasicShake = false;
        shakeAmount = 0;
        shakeDuration = 0;        
    }

    IEnumerator ContinuousShake()
    {
        isShaking = true;

        while (activatedBasicShake)
        {
                Vector3 rotationAmount = Random.insideUnitSphere * shakeAmount;//A Vector3 to add to the Local Rotation
                rotationAmount.z = 0;//Don't change the Z; it looks funny.

                shakePercentage = shakeDuration / startDuration;//Used to set the amount of shake (% * startAmount).

                if (losesImpactOverTime)
                    shakeAmount = startAmount * shakePercentage;//Set the amount of shake (% * startAmount).

                shakeDuration = Mathf.Lerp(shakeDuration, 0, Time.deltaTime);//Lerp the time, so it is less and tapers off towards the end.

                transform.localRotation = Quaternion.Euler(rotationAmount);//Set the local rotation the be the rotation amount.

                yield return null;
        }

        transform.localRotation = Quaternion.identity;//Set the local rotation to 0 when done, just to get rid of any fudging stuff.
        isShaking = false;

    }
}
