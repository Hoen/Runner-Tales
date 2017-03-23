using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayableCharacter : MonoBehaviour
{
    // Mouvement
    [HideInInspector] public float customTimeScale = 1.0f;
    public float baseSpeed = 5f;
    [HideInInspector] public float actualSpeed = 0f;
    public float jumpForce = 320f;

    // Éléments composant le personnage
    private Animator anim;
    private Rigidbody2D rb2D;

    // Éléments pour vérifier des collisions
    public GameObject standHitbox;
    public GameObject slideHitbox;

    private Vector3 groundCheckOffset = new Vector3 (0f, -0.05f, 0f);
    private Vector3 frontCheckOffset = new Vector3 (0.64f, 0.16f, 0f);

    // États
    public int hp = 5;
    protected string playerState = "ALIVE";
    [HideInInspector] public bool isGrounded = false;
    private bool isSliding = false;
    private bool isJumping = false;

    // Score
    [HideInInspector] public int score = 0;
    [HideInInspector] public int bonusScore = 0;

    // Pouvoir spécial
    [HideInInspector] public int power = 100;
    protected bool isPowerActive = false;
    private float lastPowerTick = 0f;

    // Contrôle tactile
    private float touchTimestamp = 0f;

    // Initialisation
    protected void Awake ()
    {
        actualSpeed = baseSpeed;
        anim = GetComponent<Animator> ();
        rb2D = GetComponent<Rigidbody2D> ();
    }

    protected void Start ()
    {
        SetAnimation ();
        SetHitbox ();
    }

    // Détection des collisions avec les obstacles
    protected virtual void OnTriggerEnter2D (Collider2D other)
    {
        // Le joueur perd la partie si il heurte un obstacle. L'information est traîtée par le GameManager.
        if (other.CompareTag ("Obstacle") && playerState == "ALIVE")
            SetHitPoints (hp - 1);
    }

    // Fonction principale enregistrant les actions du joueur et appliquant les mouvements au personnage
    protected virtual void FixedUpdate ()
    {
        if (playerState == "ALIVE")
        {
            // Gestion du pouvoir spécial. Un comportement par défaut est défini dans la fonction, mais elle peut-être passée outre en cas de besoin.
            PowerManagement ();

            // Est-ce que le personnage touche le sol ?
            isGrounded = Physics2D.Linecast (transform.position, transform.position + groundCheckOffset, 1 << LayerMask.NameToLayer ("Ground"));

#if UNITY_STANDALONE || UNITY_WEBPLAYER

            if (Input.touchCount == 0)
            {
                // L'axe vertical permet de déterminer si le personnage doit se baisser.
                isSliding = Input.GetAxis ("Vertical") < 0;

                // Le bouton de saut détermine si le personnage doit sauter.
                isJumping = Input.GetButton ("Jump");

                // Utilisation du pouvoir spécial
                if (Input.GetButtonUp ("Fire1"))
                    ActivatePower ();
            }

            else
            {
                if (Input.GetTouch (0).phase == TouchPhase.Began)
                    touchTimestamp = Time.time;

                if ((Input.GetTouch (0).phase == TouchPhase.Stationary || Input.GetTouch (0).phase == TouchPhase.Moved) && Time.time >= touchTimestamp + 0.25f)
                    isSliding = true;

                else if (Input.GetTouch (0).phase == TouchPhase.Ended)
                {
                    if (Time.time < touchTimestamp + 0.25f)
                        isJumping = true;
                }
            }

#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

            if (Input.GetTouch (0).phase == TouchPhase.Began)
                touchTimestamp = Time.time;

            if ((Input.GetTouch (0).phase == TouchPhase.Stationary || Input.GetTouch (0).phase == TouchPhase.Moved) && Time.time >= touchTimestamp + 0.25f)
                isSliding = true;

            else if (Input.GetTouch (0).phase == TouchPhase.Ended)
            {
                if (Time.time < touchTimestamp + 0.25f)
                    isJumping = true;
            }

#endif

            // Application du saut si le personnage est au sol et doit sauter
            if (isGrounded && isJumping)
                rb2D.AddForce (new Vector2 (0f, jumpForce));

            // Application de la vitesse constante au personnage
            rb2D.velocity = new Vector2 (actualSpeed, rb2D.velocity.y);

            // Détection d'une éventuelle collision avec un escalier
            if (Physics2D.Linecast (transform.position + new Vector3 (0f, frontCheckOffset.y, 0f), transform.position + frontCheckOffset, 1 << LayerMask.NameToLayer ("Ground")))
                SetHitPoints (0);

            // Détection de la victoire du joueur s'il dépasse le point à atteindre
            if (transform.position.x > GameObject.FindGameObjectWithTag ("Goal").transform.position.x)
                GameOver ("WIN");

            // Détermination de l'animation à jouer et de la hitbox active
            SetAnimation ();
            SetHitbox ();
        }
    }

    // Détermination de l'animation à jouer
    protected void SetAnimation ()
    {
        if (isSliding)
            anim.SetTrigger ("Slide");

        else if (!isGrounded)
        {
            if (rb2D.velocity.y > 0)
                anim.SetTrigger ("Jump");

            else
                anim.SetTrigger ("Fall");
        }

        else
            anim.SetTrigger ("Run");
    }

    // Détemrination de la hitbox active
    protected void SetHitbox ()
    {
        if (isSliding)
        {
            standHitbox.SetActive (false);
            slideHitbox.SetActive (true);
            frontCheckOffset.x = 0.96f;
        }

        else
        {
            standHitbox.SetActive (true);
            slideHitbox.SetActive (false);
            frontCheckOffset.x = 0.64f;
        }
    }

    // Par défaut, la jauge de pouvoir augmente périodiquement, puis se vide lorsque le pouvoir est utilisé. Les personnages peuvent réécrire cette fonction, si besoin est.
    protected virtual void PowerManagement ()
    {
        // L'intervale de temps de l'augmentation périodique de la jauge de pouvoir
        float tickInterval = 0.25f;

        if (lastPowerTick + tickInterval < Time.time)
        {
            // Si le pouvoir spécial est actif, la quantité de pouvoir diminue.
            if (isPowerActive)
                power = Mathf.Clamp (power - 6, 0, 100);

            // Sinon, la quantité de pouvoir augmente.
            else
                power = Mathf.Clamp (power + 3, 0, 100);

            lastPowerTick = Time.time;
        }

        // Désactivation du pouvoir spécial lorsque la jauge de pouvoir atteint zéro
        if (isPowerActive && power <= 0)
            DeactivatePower ();
    }

    // Ces fonctions doivent être écrites pour chaque personnage. Elle sont présentes ici pour pouvoir être appelées.
    protected abstract void ActivatePower ();
    protected abstract void DeactivatePower ();

    // Mise à jour du nombre de points de vie
    protected void SetHitPoints (int newHP)
    {
        hp = newHP;

        if (hp <= 0)
            GameOver ("DEAD");
    }

    // Fonction de fin de partie, quelque soit l'issue pour le joueur
    protected void GameOver (string newState)
    {
        switch (newState)
        {
            default:
                playerState = "ALIVE";
                break;

            case "DEAD":
                GameManager.singleton.GameOver (false);
                playerState = "DEAD";
                break;

            case "WIN":
                GameManager.singleton.GameOver (true);
                playerState = "WIN";
                anim.SetTrigger ("Win");
                break;
        }
    }
}
