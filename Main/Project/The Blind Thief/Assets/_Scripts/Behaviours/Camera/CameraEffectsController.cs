using UnityEngine;
using System.Collections;

namespace UnityStandardAssets.ImageEffects
{
    public class CameraEffectsController : MonoBehaviour
    {
        [SerializeField]
        private Camera mainCamera;
        

        private static CameraEffectsController instance;
        public static CameraEffectsController Instance
        {
            get { return instance; }
        }

        [Header("Effects")]
        [SerializeField]
        private VignetteAndChromaticAberration vignetteController;
        [SerializeField]
        private CameraShake cameraShake;

        [Header("Camera Shake")]
        [SerializeField]
        private CameraShakeAttributes holdingBreathCameraShake;
        [SerializeField]
        private CameraShakeAttributes enemySoundWave;
        private CameraShakeAttributes activeCameraShake;

        private bool hasActivatedCameraShake;

        //CameraZoom
        private float originalZoom;
        private float maxZoom;
        

        [Header("Breathing In Effects")]
        [SerializeField]
        private float vignetteStartingValue;
        [SerializeField]
        private float vignetteEndValue;
        [SerializeField]
        private float vignetteHoldingBreathSpeed;
        [SerializeField]
        private float vignetteReleasingBreathSpeed;
        [SerializeField]
        private AnimationCurve vignetteCurve;
        private float timeStartedHoldingBreath;

        private VignetteEffect vignette;
        private float timeStartedVignette;
        private bool isShowingVignette;

        private CameraZoomEffect cameraZoom;
        private float timeStartedZooming;
        private bool isZooming;

        void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
                
        void Start()
        {
            SubscribeToEvents();
        }   

        void SubscribeToEvents()
        {
            //Vignette
            PlayerBreathingController.Instance.StartedHoldingBreath += InitiateBreathingInEffect;
            PlayerBreathingController.Instance.StartedReleasingBreath += InitiateBreathingInEffect;
            PlayerBreathingController.Instance.HoldingBreath += HoldingBreathEffect;
            PlayerBreathingController.Instance.ReleasingBreath += ReleasingBreathEffect;

            //CameraShake
            PlayerBreathingController.Instance.RunningOutOfBreath += SetHoldingBreathToCameraShake;
            PlayerBreathingController.Instance.ReleasingBreath += StopCameraShake;
        }

        public void IntitateVignetteEffect(VignetteEffect vignetteEffect)
        {
            vignette = vignetteEffect;
            timeStartedVignette = Time.time;
            isShowingVignette = true;
        }

        public void InitiateZoomEffect(CameraZoomEffect Zoom)
        {
            cameraZoom = Zoom;

            if (cameraZoom.isZoomingOut)
                cameraZoom.startingPoint = mainCamera.orthographicSize;

            timeStartedZooming = Time.time;
            isZooming = true;
        }

        public void StopZoomEffect()
        {
            isZooming = false;
        }

        void Update()
        {
            if (isShowingVignette)
                LerpVignette();

            if (isZooming)
                LerpZoom();
        }

        void LerpZoom()
        {
            float timeSinceStarted = Time.time - timeStartedZooming;
            float percentageComplete = timeSinceStarted / cameraZoom.zoomSpeed;

            mainCamera.orthographicSize = Mathf.Lerp(cameraZoom.startingPoint, cameraZoom.endPoint, cameraZoom.movementCurve.Evaluate(percentageComplete));

            if(percentageComplete >= 1.0)
            {
                isZooming = false;
            }
        }

        void LerpVignette()
        {
            float timeSinceStarted = Time.time - timeStartedVignette;
            float percentageComplete = timeSinceStarted / vignette.speed;

            vignetteController.intensity = Mathf.Lerp(vignette.startingValue, vignette.endValue, vignette.movementCurve.Evaluate(percentageComplete));

            if(percentageComplete > 1.0f)
            {
                isShowingVignette = false;
            }
        }

        /// <summary>
        /// Triggered by Starting Holding Breath delegate
        /// </summary>
        void InitiateBreathingInEffect()
        {
            timeStartedHoldingBreath = Time.time;
        }

        /// <summary>
        /// Triggered by Holding Breath delegate
        /// </summary>
        void HoldingBreathEffect()
        {
            float timeSinceStarted = Time.time - timeStartedHoldingBreath;
            float percentageComplete = timeSinceStarted / vignetteHoldingBreathSpeed;

            vignetteController.intensity = Mathf.Lerp(vignetteStartingValue, vignetteEndValue, vignetteCurve.Evaluate(percentageComplete));
        }

        void ReleasingBreathEffect()
        {
            float timeSinceStarted = Time.time - timeStartedHoldingBreath;
            float percentageComplete = timeSinceStarted / vignetteReleasingBreathSpeed;

            vignetteController.intensity = Mathf.Lerp(vignetteEndValue, vignetteStartingValue, vignetteCurve.Evaluate(percentageComplete));
        }

        #region CameraShake

        void SetHoldingBreathToCameraShake()
        {
            if(!hasActivatedCameraShake)
            {
                activeCameraShake = holdingBreathCameraShake;
                CameraShake();
            }
           
        }

        public void SetEnemySoundWaveToCameraShake()
        {
            if (!hasActivatedCameraShake)
            {
                activeCameraShake = enemySoundWave;
                CameraShake();
            }

        }

        void CameraShake()
        {
            cameraShake.ShakeCamera(activeCameraShake);
            hasActivatedCameraShake = true;
        }

        public void StopCameraShake()
        {
            cameraShake.StopCameraShake();
            hasActivatedCameraShake = false;
        }

        #endregion

    }
}
