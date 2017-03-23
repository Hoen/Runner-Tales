using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour
{
    // Variables
    [HideInInspector] public Animator anim;

    protected bool isDestroyable = true;

    // Use this for initialization
    protected void Awake ()
    {
        anim = GetComponent<Animator> ();
    }

    // Update is called once per frame
    protected virtual void FixedUpdate ()
    {
        AnimatorClipInfo[] clipsInfo = anim.GetCurrentAnimatorClipInfo (0);
        AnimationClip clip = clipsInfo[0].clip;

        Debug.Log (clip.length);
    }
}
