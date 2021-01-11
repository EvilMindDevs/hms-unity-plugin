using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    [Header("Objects")]
    public Text GameTime;
    public Text healthDisplay;
    public GameObject losePanel;

    [Header("Character Skills")]
    public float speed;
    public int health;
    public float startDashTime;
    public float extraSpeed;
    [SerializeField]
    private float DeltaTime;

    Rigidbody2D rb;
    Animator anim;
    private float input;
    Touch touch;

    private float dashTime;
    private bool isDashing;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        healthDisplay.text = health.ToString();
        GameTime.text = "0";
    }

    private void Update()
    {

        if (Application.platform == RuntimePlatform.Android 
            || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            AnimateForMobile();
        }else
        {
            if (input != 0)
            {
                anim.SetBool("isRunning", true);
            }
            else
            {
                anim.SetBool("isRunning", false);
            }

            // rotating the character based on running direction
            if (input > 0)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else if (input < 0)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
        }        
    }

    // physics calculate stuff
    void FixedUpdate()
    {

        DeltaTime += Time.deltaTime;
        GameTime.text = DeltaTime.ToString("0.00");

        if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);

                if (touch.position.x > Screen.width / 2)
                {
                    // right
                    rb.velocity = new Vector2(speed, rb.velocity.y);

                }
                else if (touch.position.x <= Screen.width / 2)
                {
                    // left
                    rb.velocity = new Vector2(-speed, rb.velocity.y);

                }
            }else
            {
                // because of velocity is not going to stop, when we finish clicking, we need to change the velocity to 0
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
        else
        {
            input = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(input * speed, rb.velocity.y);

            if (Input.GetKeyDown(KeyCode.Space) && !isDashing)
            {
                speed += extraSpeed;
                isDashing = true;

            }

            if (dashTime <= 0 && isDashing)
            {
                isDashing = false;
                speed -= extraSpeed;
            }
            else
            {
                dashTime -= Time.deltaTime;
            }

                
        }

    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        healthDisplay.text = health.ToString();

        if (health <= 0)
        {
            healthDisplay.text = "0";
            losePanel.SetActive(true);
            // destroy player
            Destroy(gameObject);
            SendAnalytics(DeltaTime);
        }
    }

    public void SendAnalytics(float GameTime)
    {
        AnalyticsResult result = Analytics.CustomEvent("EndGame", new Dictionary<string, object>
        {
            { "GameTime", GameTime }
        });
        Debug.Log("AnalyticsResult:" + result);
    }

    public void AnimateForMobile()
    {
        Debug.Log(touch.phase);
        if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved|| touch.phase == TouchPhase.Stationary)
        {
            anim.SetBool("isRunning", true);
        }else if(touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
        {
            anim.SetBool("isRunning", false);
        }

        // rotating the character based on running direction
        if (touch.position.x > Screen.width / 2)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (touch.position.x < Screen.width / 2)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }

    }
}
