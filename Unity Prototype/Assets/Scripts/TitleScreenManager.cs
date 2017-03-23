using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    // La fonction Update permet d'attendre le moment où le joueur interagira avec le jeu.
    void Update ()
    {
        // Quand l'action requise est effectuée, le jeu charge le menu principal.
        if (Input.GetButtonUp ("Submit") || (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Ended))
            SceneManager.LoadScene ("Main Menu");
    }
}
