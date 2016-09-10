using UnityEngine;
using System.Collections;

namespace UnityStandardAssets.ImageEffects
{
    public class CameraEffectsController : MonoBehaviour
    {
        [Header("Effects")]
        [SerializeField]
        private VignetteAndChromaticAberration vignetteController;


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
        
        void Start()
        {
            SubscribeToEvents();
        }   

        void SubscribeToEvents()
        {
            PlayerBreathingController.Instance.StartedHoldingBreath += InitiateBreathingInEffect;
            PlayerBreathingController.Instance.StartedReleasingBreath += InitiateBreathingInEffect;
            PlayerBreathingController.Instance.HoldingBreath += HoldingBreathEffect;
            PlayerBreathingController.Instance.ReleasingBreath += ReleasingBreathEffect;
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

    }
}
