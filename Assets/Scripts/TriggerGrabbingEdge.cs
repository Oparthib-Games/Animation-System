using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerGrabbingEdge : MonoBehaviour
{
    public GameObject rootPos;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "handShphere")  other.GetComponentInParent<HelmetShed>().GrabEdge(rootPos);
    }
}
