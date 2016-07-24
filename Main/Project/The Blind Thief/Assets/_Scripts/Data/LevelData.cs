using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Level", menuName = "DataObjects/Level", order = 1)]
public class LevelData : ScriptableObject
{
    public GameObject levelGeometry;
    public int numberOfKeys;

}
