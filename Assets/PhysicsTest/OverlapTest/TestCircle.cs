using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCircle : MonoBehaviour
{
    public float radius_;
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius_);
    }

    
}
