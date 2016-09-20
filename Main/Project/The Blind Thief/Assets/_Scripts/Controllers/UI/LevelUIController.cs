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

    void OnEnable()
    {
        EventManager.StartListening("EndOfLevel", EndOfLevel);
    }

    void OnDisable()
    {
        EventManager.StopListening("EndOfLevel", EndOfLevel);
    }    

    void EndOfLevel()
    {
        endOfLevelScreen.SetBool("isFadeIn",true);
    }

    void NextLevel()
    {
        endOfLevelScreen.SetBool("isFadeIn", false);
        endOfLevelScreen.SetBool("isFadeOut", true);
    }

    /// <summary>
    /// Triggered by Player Movement Behaviour
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

    public void OnClickRespawnPlayer()
    {
        EventManager.TriggerEvent("Reset");
        deathScreen.SetActive(false);        
        UnityStandardAssets.ImageEffects.CameraEffectsController.Instance.IntitateVignetteEffect(vignette);
    }

    public void OnClickNextLevel()
    {
        GameController.Instance.IncrementLevel();
        LevelController.Instance.IncrementLevel();
        NextLevel();        
    }
	
}
