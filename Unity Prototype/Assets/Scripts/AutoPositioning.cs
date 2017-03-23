using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AutoPositioning : MonoBehaviour
{

#if UNITY_EDITOR

    private Vector2 defaultGridSpacing = new Vector2 (5.12f, 1.28f);

    public Vector3 GetNewPosition (Vector2 gridSpacing = default (Vector2))
    {
        if (gridSpacing == Vector2.zero)
            gridSpacing = defaultGridSpacing;

        float newXPos = Mathf.Round (transform.position.x / gridSpacing.x) * gridSpacing.x;
        float newYPos = Mathf.Round (transform.position.y / gridSpacing.y) * gridSpacing.y;

        return new Vector3 (newXPos, newYPos, transform.position.z);
    }

    public void SetNewPosition (Vector2 gridSpacing = default (Vector2))
    {
        if (gridSpacing == Vector2.zero)
            gridSpacing = defaultGridSpacing;

        transform.position = GetNewPosition (gridSpacing);
    }

#endif

}
