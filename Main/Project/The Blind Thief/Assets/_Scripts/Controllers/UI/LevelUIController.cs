using UnityEngine;
using System.Collections;

public class LevelUIController : Singleton<LevelUIController>
{
    [Header("End of Level")]
    [SerializeField] private Animator endOfLevelScreen;

    [Header("Death Screen")]
    [SerializeField] private GameObject deathScreen;

    [Header("Respawn Screen")]
    [SerializeField] private VignetteEffect vignette;

    void Start()
    {
        SubscribeToEvents();
    }

    void SubscribeToEvents()
    {
        LevelExitBehaviour.Instance.EndOfLevel += EndOfLevel;
    }

    void EndOfLevel()
    {
        endOfLevelScreen.SetBool("isFadeIn",true);
    }

    public void NextLevel()
    {
        endOfLevelScreen.SetBool("isFadeIn", false);
        endOfLevelScreen.SetBool("isFadeOut", true);
    }

    /// <summary>
    /// Triggered by Enemy Movement Behaviour
    /// </summary>
    /// <returns></returns>
    public IEnumerator TriggerDeathScreen(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        ShowDeathScreen();
    }

    void ShowDeathScreen()
    {
        deathScreen.SetActive(true);
    }

    public void RespawnPlayer()
    {
        deathScreen.SetActive(false);
        ResetController.Instance.Reset();
        UnityStandardAssets.ImageEffects.CameraEffectsController.Instance.IntitateVignetteEffect(vignette);
    }
	
}
