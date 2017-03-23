using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (AutoPositioning))]
public class CustomEditorFunctions : Editor
{
    public Vector2 gridSpacing = new Vector2 (5.12f, 1.28f);

    private bool debugLines = true;

    void OnSceneGUI ()
    {
        Event e = Event.current;
        AutoPositioning t = target as AutoPositioning;

        if (t == null)
            return;

        if (e.button == 0)
        {
            if (e.type == EventType.MouseUp)
                t.SetNewPosition (gridSpacing);

            if (debugLines)
                Debug.DrawLine (t.transform.position, t.GetNewPosition (gridSpacing));
        }
    }
}
