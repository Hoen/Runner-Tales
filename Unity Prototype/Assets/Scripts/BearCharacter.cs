using System;
using UnityEngine;

public class BearCharacter : PlayableCharacter
{
    private float nextHPCheck;
    private float hpCheckInterval = 5.0f;

    // Détection des collisions avec les obstacles. Le fonctionnemnt diffère du comportement originel.
    protected override void OnTriggerEnter2D (Collider2D other)
    {
        // Le moment où le personnage va perdre automatiquement un point de vie est repoussé.
        if (other.CompareTag ("Obstacle") && playerState == "ALIVE")
        {
            if (!isPowerActive)
                ResetNextCheckTime ();

            else
                SetHitPoints (Math.Min (hp + 1, 5));
        }
    }

    protected override void FixedUpdate ()
    {
        // La vie de l'Ours baisse périodiquement.
        if (playerState == "ALIVE")
        {
            // Initialisation de la variable
            if (nextHPCheck <= 0f)
                ResetNextCheckTime ();

            if (nextHPCheck < Time.time && !isPowerActive)
            {
                SetHitPoints (hp - 1);
                ResetNextCheckTime ();
            }
        }

        // On exécute le code commun à tous les personnages.
        base.FixedUpdate ();
    }

    protected override void ActivatePower ()
    {
        if (!isPowerActive && power >= 100)
            isPowerActive = true;
    }

    protected override void DeactivatePower ()
    {
        hpCheckInterval = 5.0f;
        ResetNextCheckTime ();
        isPowerActive = false;
    }

    private void ResetNextCheckTime ()
    {
        nextHPCheck = Time.time + hpCheckInterval;
    }
}
