[System.Serializable]
public class VignetteEffect
{
    public float startingValue;  
    public float endValue;   
    public float speed;   
    public float returnSpeed;   
    public UnityEngine.AnimationCurve movementCurve;
    private bool isReturningToOriginal;
}
