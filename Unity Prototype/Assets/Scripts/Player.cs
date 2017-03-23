using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 320f;
    public GameObject standingHitbox;
    public GameObject crouchingHitbox;

    private Animator anim;
    private Rigidbody2D rb2D;
    private Transform groundChecker;
    private Transform frontChecker;

    private int powerGauge = 0;
    private float lastPowerGaugeUpdate;
    private bool isUsingPower = false;
    private bool isJumping = false;
    private bool isCrouching = false;
    private bool isGrounded = false;
    private bool isDead = false;
    private bool hasWon = false;

    void Awake ()
    {
        anim = GetComponent<Animator> ();
        rb2D = GetComponent<Rigidbody2D> ();
        groundChecker = GameObject.Find ("Player Ground Checker").transform;
        frontChecker = GameObject.Find ("Player Front Checker").transform;

        lastPowerGaugeUpdate = Time.time;
    }

    void Update ()
    {
        if (!isDead && !hasWon)
        {
            if (!isUsingPower)
            {
                if (lastPowerGaugeUpdate + 0.1f < Time.time && rb2D.velocity.x != 0)
                {
                    powerGauge = Mathf.Clamp (powerGauge + 1, 0, 100);
                    GameObject.Find ("Power Gauge Text").GetComponent<Text> ().text = powerGauge.ToString ();
                    lastPowerGaugeUpdate = Time.time;
                }
            }

            else
            {
                if (lastPowerGaugeUpdate + 0.025f < Time.time && rb2D.velocity.x != 0)
                {
                    if (powerGauge > 0)
                    {
                        speed = 3f;
                        powerGauge = Mathf.Clamp (powerGauge - 1, 0, 100);
                        GameObject.Find ("Power Gauge Text").GetComponent<Text> ().text = powerGauge.ToString ();
                    }

                    else
                    {
                        speed = 5f;
                        isUsingPower = false;
                    }

                    lastPowerGaugeUpdate = Time.time;
                }
            }

            // Vérification du contact éventuel entre le personnage et un sol
            if (groundChecker != null)
                isGrounded = Physics2D.Linecast (transform.position, groundChecker.position, 1 << LayerMask.NameToLayer ("Ground"));

#if UNITY_STANDALONE || UNITY_WEBPLAYER

            // On détermine si le personnage doit se baisser.
            float vInput = Input.GetAxis ("Vertical");

            if (vInput < 0)
                isCrouching = true;
            else
                isCrouching = false;

            // On détermine si le personnage doit sauter.
            if (Input.GetButtonDown ("Jump"))
                isJumping = true;
            else
                isJumping = false;

            // Activation du pouvoir
            if (Input.GetButtonDown ("Fire1") && powerGauge >= 100 && !isUsingPower)
                isUsingPower = true;

#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

        if (Input.touchCount > 0)
        {
            // On détermine si le personnage doit se baisser.
            if (Input.GetTouch (0).phase == TouchPhase.Stationary)
                isCrouching = true;
            else
                isCrouching = false;

            // On détermine si le personnage doit sauter.
            if (Input.GetTouch (0).phase == TouchPhase.Ended)
                isJumping = true;
            else
                isJumping = false;
        }

#endif

            // On met à jour l'animation et la hitbox active.
            SetAnimation ();
            SetHitbox ();
        }
    }

    void SetAnimation ()
    {
        if (isCrouching)
            anim.SetBool ("isCrouching", true);
        else
            anim.SetBool ("isCrouching", false);
    }

    void SetHitbox ()
    {
        if (isCrouching)
        {
            standingHitbox.SetActive (false);
            crouchingHitbox.SetActive (true);
            frontChecker.position = new Vector3 (transform.position.x + 0.96f, frontChecker.position.y, frontChecker.position.z);
        }

        else
        {
            standingHitbox.SetActive (true);
            crouchingHitbox.SetActive (false);
            frontChecker.position = new Vector3 (transform.position.x + 0.64f, frontChecker.position.y, frontChecker.position.z);
        }
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        if (other.CompareTag ("Obstacle"))
        {
            isDead = true;
            speed = 0f;
            GameManager.singleton.GameOver (hasWon);
        }
    }

    void FixedUpdate ()
    {
        rb2D.velocity = new Vector2 (speed, rb2D.velocity.y);

        if (!hasWon && transform.position.x > GameObject.FindGameObjectWithTag ("Goal").transform.position.x)
        {
            hasWon = true;
            speed = 0f;
            GameManager.singleton.GameOver (hasWon);
        }

        if (isGrounded)
        {
            if (anim.GetBool ("isJumping"))
                anim.SetBool ("isJumping", false);

            if (isJumping)
            {
                rb2D.AddForce (new Vector2 (0f, jumpForce));
                anim.SetBool ("isJumping", true);
            }
        }

        if (frontChecker != null && Physics2D.Linecast (new Vector3 (transform.position.x, frontChecker.position.y, transform.position.z), frontChecker.position, 1 << LayerMask.NameToLayer ("Ground")))
        {
            isDead = true;
            speed = 0f;
            GameManager.singleton.GameOver (hasWon);
        }
    }
}
