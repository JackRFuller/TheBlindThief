using UnityEngine;
using System.Collections;

namespace UnityStandardAssets.ImageEffects
{
    public class CameraEffectsController : MonoBehaviour
    {
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
        
        void Awake()
        {
            if (instance == null)
                instance = this;
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
