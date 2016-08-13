using UnityEngine;
using System.Collections;

/// <summary>
/// Used to control Actors and make them revaulate their paths if eith a platform has been rotated or moved
/// </summary>
public class PathController : Singleton<PathController>
{
    public delegate void reEvaluate();
    public reEvaluate ReEvaluate;

    /// <summary>
    /// Activated when a platform finishes rotating or moving
    /// </summary>
    public IEnumerator RegisterMovementOfPlatforms()
    {
        //Find all Nodes
        yield return StartCoroutine(NodeController.Instance.GetNodes());

        //Make all Actors reasses their path
        if (ReEvaluate != null)
            ReEvaluate();

    }
	
}
