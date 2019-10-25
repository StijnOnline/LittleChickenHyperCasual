using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    [SerializeField] private List<Transform> nodes = new List<Transform>();
    private float length;
    public float getLength() { return length; }

    void Start()
    {
        length = 0;
        nodes.Clear();

        foreach(Transform child in GetComponentsInChildren<Transform>()) {
            if(child != transform) {
                nodes.Add(child);
            }
        }

        for(int i = 0; i < nodes.Count; i++) {
            if(i > 0) {
                Vector3 toNext = (nodes[i].position - nodes[i - 1].position);
                length += toNext.magnitude;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        length = 0;
        nodes.Clear();

        foreach(Transform child in GetComponentsInChildren<Transform>()) {
            if(child != transform) {
                nodes.Add(child);
            }
        }

        for (int i =0;i<nodes.Count;i++) {
            Gizmos.DrawWireSphere(nodes[i].position, 0.1f);
            if(i>0) {
                Vector3 toNext = (nodes[i].position - nodes[i-1].position);
                length += toNext.magnitude;

                Gizmos.DrawLine(nodes[i-1].position,nodes[i].position);                
            }
        }
    }

    public Vector3 Evaluate(float dist) {
        if(dist == 0) { return nodes[0].position; }
        float distLeft = dist;
        int n = 0;        

        while(distLeft > 0) {
            if(n == nodes.Count - 1) { return nodes[n].position; } //if there aren't any more nodes

            Vector3 toNext = (nodes[n + 1].position - nodes[n].position);
            if((distLeft) < toNext.magnitude) { //if next node is enough
                return nodes[n].position + toNext.normalized * distLeft;
            } else {
                distLeft -= toNext.magnitude;
                n++;
            }
        }
        Debug.LogError("Error Evaluating path");
        return Vector3.zero;
    }

    public List<Transform> GetPath() {
        return nodes;
    }
}