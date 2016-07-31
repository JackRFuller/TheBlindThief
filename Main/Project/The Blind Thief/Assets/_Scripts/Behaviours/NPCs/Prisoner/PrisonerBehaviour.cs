using UnityEngine;
using System.Collections;

public class PrisonerBehaviour : AnimationController
{
    [SerializeField] private SkinnedMeshRenderer mesh;
    private bool isCharacterActive;

    //Color Lerping
    [Header("Color Lerping")]
    [SerializeField] private float colorLerpTime;
    [SerializeField] private AnimationCurve colorLerpCurve;
    private bool isColorLerping;
    private float timeStarted;
    private Material newMaterial;
    private Color newColor;

    void ActivateCharacter()
    {
        newMaterial = mesh.materials[0];
        newColor = newMaterial.color;

        timeStarted = Time.time;
        isColorLerping = true;

        //Show Text
        TurnOnAnimation("FadeInText");
    }

    void Update()
    {
        if (isColorLerping)
            LerpMeshColor();
    }

    void LerpMeshColor()
    {
        float _timeSInceStarted = Time.time - timeStarted;
        float _percentageComplete = _timeSInceStarted / colorLerpTime;

        newColor.a = Mathf.Lerp(newColor.a, 255, colorLerpCurve.Evaluate(_percentageComplete));
        newMaterial.color = newColor;

        mesh.materials[0] = newMaterial;

        if(_percentageComplete >= 1.0f)
        {
            isColorLerping = false;            
        }
    }    

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (!isCharacterActive)
            {
                ActivateCharacter();
                Debug.Log("Hit");
            }
        }
    }
	
}
