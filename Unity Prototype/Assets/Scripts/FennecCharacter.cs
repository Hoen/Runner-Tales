using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FennecCharacter : PlayableCharacter
{
    protected override void ActivatePower ()
    {
        customTimeScale = 0.5f;

        SetEnemiesTimescale (customTimeScale);
    }

    protected override void DeactivatePower ()
    {
        customTimeScale = 1.0f;

        SetEnemiesTimescale (customTimeScale);
    }

    // Fonction appliquant le pouvoir du Fennec aux ennemis, à savoir le ralentissement du temps via le ralentissement de la lecture des animations
    private void SetEnemiesTimescale (float newTimeScale)
    {
        // On récupère la liste de chaque ennemi grâce au tag qui leur est appliqué.
        GameObject[] enemyList = GameObject.FindGameObjectsWithTag ("Enemy");

        // Pour chaque ennemi, on modifie la vitesse de lecture de leurs animations.
        foreach (GameObject enemy in enemyList)
        {
            Animator anim = enemy.GetComponent<Animator> ();
            anim.SetFloat ("animSpeed", newTimeScale);
        }
    }
}
