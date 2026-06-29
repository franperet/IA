using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Node> neighbors = new List<Node>();

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.2f);
        Gizmos.color = Color.cyan;
        foreach (var n in neighbors)
            if (n != null) Gizmos.DrawLine(transform.position, n.transform.position);
    }
}
