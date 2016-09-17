using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour
{

	[SerializeField] protected Animator animController;

    public virtual void TurnOnAnimation(string _targetAnim)
    {
        if(animController)
            for (int i = 0; i < animController.parameters.Length; i++)
            {
                if (_targetAnim == animController.parameters[i].name)
                {
                    animController.SetBool(_targetAnim, true);
                }
                else
                {
                    string _paraName = animController.parameters[i].name;
                    animController.SetBool(_paraName, false);
                }
            }
    }
}
