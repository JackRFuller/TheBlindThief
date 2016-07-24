using UnityEngine;
using System.Collections;

public class PlatformBehaviour : MonoBehaviour
{

    [Header("Controlling Switch")]
    [SerializeField] private SwitchBehaviour swScript;
    
    [Header("Sprites")]
    [SerializeField] protected SpriteRenderer[] sprites;
    [SerializeField] protected float spriteFadeInSpeed;
    [SerializeField] protected AnimationCurve fadeInCurve;
    private bool isFadingInSprite;
    private float timeFadeInStarted;

    public virtual void ActivateSwitchBehaviour(Transform _enablerer)
    {
        
    }

    public virtual void ActivatePlatform()
    {
        InitiateFadeIn();
    }

    void InitiateFadeIn()
    {
        timeFadeInStarted = Time.time;
        isFadingInSprite = true;
    }

    public virtual void Update()
    {
        if(isFadingInSprite)
            FadeInSprites();

    }

    void FadeInSprites()
    {
        float _timeSinceStarted = Time.time - timeFadeInStarted;
        float _percentageComplete = _timeSinceStarted / spriteFadeInSpeed;

        for (int i = 0; i < sprites.Length; i++)
        {
            Color _spriteColor = sprites[i].color;
            _spriteColor.a = Mathf.Lerp(_spriteColor.a, 1, fadeInCurve.Evaluate(_percentageComplete));
            sprites[i].color = _spriteColor;
        }

        if (_percentageComplete >= 1.0f)
        {
            isFadingInSprite = false;

            if (swScript != null)
                swScript.IsActivated = true;


        }

    }
}
