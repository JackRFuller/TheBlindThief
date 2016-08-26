using UnityEngine;
using System.Collections;

public class GameController : Singleton<GameController>
{
    private int characterGender; //Determines what gender the character is 0 - Female, 1 - Male
    public int ChatacterGender { get { return characterGender; } }

	public override void Awake()
    {
        base.Awake();

        DeterminePlayerCharacterGender();
    }

    void DeterminePlayerCharacterGender()
    {
        characterGender = Random.Range(0, 2);
    }
}
