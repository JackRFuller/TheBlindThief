using UnityEngine;
using System.Collections;

public class PlatformBehaviour : MonoBehaviour
{
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
        Debug.Log(gameObject.name +" activated");
    }

    void InitiateFadeIn()
    {
        timeFadeInStarted = Time.time;
        isFadingInSprite = true;

        Debug.Log(gameObject.name);
    }

    public virtual void Update()
    {
        if(isFadingInSprite)
            FadeInSprites();

    }

    void FadeInSprites()
    {
        Debug.Log(gameObject.name);

        float _timeSinceStarted = Time.time - timeFadeInStarted;
        float _percentageComplete = _timeSinceStarted / spriteFadeInSpeed;

        for (int i = 0; i < sprites.Length; i++)
        {
            Color _spriteColor = sprites[i].color;
            _spriteColor.a = Mathf.Lerp(_spriteColor.a, 1, fadeInCurve.Evaluate(_percentageComplete));
            sprites[i].color = _spriteColor;
            Debug.Log(sprites[i].color);
        }

        if (_percentageComplete >= 1.0f)
        {
            isFadingInSprite = false;
        }

    }
}
