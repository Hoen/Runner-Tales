using System;
using UnityEngine;

public class BullCharacter : PlayableCharacter
{
    // Détection des collisions avec les obstacles. Le fonctionnemnt diffère du comportement originel.
    protected override void OnTriggerEnter2D (Collider2D other)
    {
        // Le joueur perd la partie si il heurte un obstacle. L'information est traîtée par le GameManager.
        if (other.CompareTag ("Obstacle") && playerState == "ALIVE")
        {
            if (isPowerActive)
                other.gameObject.SetActive (false);

            else
                SetHitPoints (hp - 1);
        }
    }

    // Le pouvoir spécial du Taureau ressemble à celui du Fennec, mais inversé, et avec une invicibilité.
    protected override void ActivatePower ()
    {
        if (!isPowerActive && power >= 100)
        {
            actualSpeed = baseSpeed * 1.25f;
            isPowerActive = true;
        }
    }

    protected override void DeactivatePower ()
    {
        actualSpeed = baseSpeed;
        isPowerActive = false;
    }
}
