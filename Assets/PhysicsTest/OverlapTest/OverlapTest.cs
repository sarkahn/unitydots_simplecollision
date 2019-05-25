using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class OverlapTest : MonoBehaviour
{
    public TestCircle a_;
    public TestCircle b_;

    private void OnGUI()
    {
        if (a_ == null || b_ == null)
            return;

        bool overlap = Utils.CirclesOverlap(a_.transform.position, a_.radius_, b_.transform.position, b_.radius_);

        GUILayout.Label(string.Format("Circles overlap: {0}", overlap));
    }
}
