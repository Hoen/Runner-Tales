using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static public GameManager singleton;

    private GameObject cam;
    private GameObject player;
    private GameObject gameOverWindow;

    private Text hpUI;
    private Text powerUI;
    private Text scoreUI;

    private Image powerBarUI;

    void Awake ()
    {
        if (singleton == null)
            singleton = this;
        else if (singleton != this)
            Destroy (gameObject);
    }

    // Use this for initialization
    void Start ()
    {
        cam = GameObject.FindGameObjectWithTag ("MainCamera");
        gameOverWindow = GameObject.Find ("Game Over Window");

        hpUI = GameObject.Find ("Hit Points Text").GetComponent<Text> ();
        powerUI = GameObject.Find ("Power Gauge Text").GetComponent<Text> ();
        scoreUI = GameObject.Find ("Score Text").GetComponent<Text> ();

        powerBarUI = GameObject.Find ("Power Level Progress Bar").GetComponent<Image> ();

        // On récupère la référence vers le personnage du joueur.
        if (player == null)
        {
            if (GameObject.FindGameObjectWithTag ("Player"))
                player = GameObject.FindGameObjectWithTag ("Player");

            else
                player = Instantiate (DataManager.singleton.characterPrefabs [DataManager.singleton.chosenCharacter]);
        }

        gameOverWindow.SetActive (false);

        // Positionnement initial de la caméra
        cam.transform.position = player.transform.position + new Vector3 (4f, 2f, -10f);
    }

    void Update ()
    {
        // Pause du jeu
        if (Input.GetButtonDown ("Cancel"))
        {
            if (Time.timeScale == 1)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;
        }

        // HUD
        PlayableCharacter p1 = player.GetComponent<PlayableCharacter> ();

        hpUI.text = "HP: " + p1.hp.ToString ();

        powerUI.text = "Power: " + p1.power.ToString ();

        RectTransform powerBarRect = powerBarUI.GetComponent<RectTransform> ();
        powerBarRect.sizeDelta = new Vector2 (p1.power * 3, powerBarRect.rect.height);

        int score = (int) player.transform.position.x;
        scoreUI.text = "Score: " + (score + p1.bonusScore).ToString ();
    }

    void LateUpdate ()
    {
        // Positionnement de la caméra
        SetCameraPosition ();
    }

    // Positionnement de la caméra, avec une hauteur basée sur le sol juste en dessous
    void SetCameraPosition ()
    {
        // On a besoin de deux vérificateur de hauteur pour positionner correctement la caméra : l'un doit être dans le sol, l'autre non.
        Vector3 camPos = cam.transform.position; // Référence plus simple à la position actuelle de la caméra
        Vector3 lowerCheckPos = new Vector3 (camPos.x, camPos.y - 2.0f, camPos.z);
        Vector3 upperCheckPos = new Vector3 (camPos.x, camPos.y - 1.9f, camPos.z);
        Vector3 gapCheckPos = new Vector3 (camPos.x, camPos.y - 64f, camPos.z);

        // On vérifie si un sol se trouve à chaque position testée
        bool lowerCollision = Physics2D.Linecast (camPos, lowerCheckPos, 1 << LayerMask.NameToLayer ("Ground"));
        bool upperCollision = Physics2D.Linecast (camPos, upperCheckPos, 1 << LayerMask.NameToLayer ("Ground"));
        bool gapDetection = Physics2D.Linecast (camPos, gapCheckPos, 1 << LayerMask.NameToLayer ("Ground"));

        // On détermine maintenant si la caméra doit monter, descendre ou rester à la même hauteur.
        float newYPos = camPos.y; // On prend la hauteur de la caméra. Si on a pas besoin d'y toucher, on l'utilisera en l'état.

        if (gapDetection) // Un sol doit se trouver quelque part sous la caméra, sinon, c'est qu'il y a un vide en dessous, et la caméra ne doit pas changer de hauteur.
        {
            if (!lowerCollision) // Si il n'y pas de sol au point le plus bas, la caméra doit descendre.
                newYPos -= .025f;

            else if (upperCollision) // En revanche, si les deux tests sont positifs, la caméra est trop basse et doit remonter.
                newYPos += .025f;
        }

        // Pour finir, on applique la nouvelle position à la caméra.
        Vector3 pPos = player.transform.position; // Référence plus simple à la position du personnage

        cam.transform.position = new Vector3 (pPos.x + 4f, newYPos, pPos.z - 10f);
    }

    public void GameOver (bool hasWon)
    {
        gameOverWindow.SetActive (true);

        Text gameOverText = GameObject.Find ("Main Game Over Text").GetComponent<Text> ();

        if (hasWon)
            gameOverText.text = "Victoire !";
        else
            gameOverText.text = "Game over";

        Time.timeScale = 0f;
    }

    public void RestartLevel ()
    {
        Scene actualScene = SceneManager.GetActiveScene ();

        if (Time.timeScale != 1)
            Time.timeScale = 1;

        SceneManager.LoadScene (actualScene.name);
    }

    public void ExitToMainMenu ()
    {
        if (Time.timeScale != 1)
            Time.timeScale = 1;

        SceneManager.LoadScene ("Main Menu");
    }

    // Outils de debug in-game
    void OnGUI ()
    {
        bool debugGUI = true;

        if (debugGUI)
        {
            // Référence au script du joueur
            PlayableCharacter p1Script = player.GetComponent<PlayableCharacter> ();

            // Pause
            if (GUI.Button (new Rect(10, 10, 50, 50), "Pause"))
            {
                if (Time.timeScale > 0)
                    Time.timeScale = 0;

                else
                    Time.timeScale = 1;
            }

            // Réglage vitesse du joueur
            if (GUI.Button (new Rect (10, 70, 25, 25), "-"))
                p1Script.actualSpeed -= 0.25f;

            if (GUI.Button (new Rect (120, 70, 25, 25), "+"))
                p1Script.actualSpeed += 0.25f;

            GUI.Box (new Rect (40, 70, 75, 25), "Speed: " + p1Script.actualSpeed);

            // Réglage hauteur saut du joueur
            if (GUI.Button (new Rect (10, 100, 25, 25), "-"))
                p1Script.jumpForce -= 15f;

            if (GUI.Button (new Rect (120, 100, 25, 25), "+"))
                p1Script.jumpForce += 15f;

            GUI.Box (new Rect (40, 100, 75, 25), "Jump: " + p1Script.jumpForce);

            // Variables booléennes
            GUI.Box (new Rect (10, 130, 135, 25), "isGrounded: " + p1Script.isGrounded);
        }
    }
}
