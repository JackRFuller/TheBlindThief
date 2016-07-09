using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeController : Singleton<NodeController>
{
    private List<Vector3> nodePositions = new List<Vector3>();
    public List<Vector3> NodePositions
    {
        get { return nodePositions; }
    }

    public override void Awake()
    {
        base.Awake();

        GetNodes();
    }

    void Nodes()
    {
        GetNodes();   
    }
    
    void GetNodes()
    {
        //Clear Lists to make sure they don't already contain nodes
        if(nodePositions.Count > 0)
            nodePositions.Clear();

        NodeBehaviour[] nodes = FindObjectsOfType<NodeBehaviour>();

        for (int i = 0; i < nodes.Length; i++)
        {
            nodePositions.Add(nodes[i].transform.position);
        }
    }
}
