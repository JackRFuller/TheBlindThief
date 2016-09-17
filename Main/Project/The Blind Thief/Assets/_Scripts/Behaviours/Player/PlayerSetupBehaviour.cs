using UnityEngine;
using System.Collections;

public class PlayerSetupBehaviour : Singleton<PlayerSetupBehaviour>
{
    [Header("Character Meshes")]
    [SerializeField]
    private GameObject maleMesh;
    [SerializeField]
    private GameObject femaleMesh;
    private GameObject activeMesh;
    public GameObject ActiveMesh { get { return activeMesh; } }
        
	// Use this for initialization
	void Start ()
    {
        DetermineGender();
	}

    /// <summary>
    /// Used to Determine if the CHaracter is Male or Female
    /// Called right at the beginning of the game
    /// </summary>
	void DetermineGender()
    {
        int characterGender = GameController.Instance.ChatacterGender;

        //Is Female
        if(characterGender == 0)
        {
            femaleMesh.SetActive(true);
            maleMesh.SetActive(false);

            activeMesh = femaleMesh;
        }

        if(characterGender == 1)
        {
            femaleMesh.SetActive(false);
            maleMesh.SetActive(true);

            activeMesh = maleMesh;
        }
    }
}
