using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region P U B L I C  V A R I A B L E S

    public float JumpForce;
    public float DoubleJumpForce;
    public float PropellerForce;
    public float Speed;
    public TrailRenderer Trail;
    public ParticleSystem PickUp;
    public Material PlayerMaterial;

    #endregion

    #region P R I V A T E  V A R I A B L E S

    bool canJump;
    bool canDoubleJump;
    bool canGlide;
    bool isGliding;
    bool isMoving;
    bool isBoosted;
    float boostSpeed;
    float magnetRange;
    Vector3 initialPosition;
    Rigidbody rb;
    Animation playerAnim;

    #endregion

    #region A W A K E || S T A R T || U P D A T E

    private void OnEnable()
    {
        EventManager.respawn += Respawn;
    }

    private void OnDisable()
    {
        EventManager.respawn -= Respawn;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animation>();
        playerAnim["OpenUp"].speed = 3;
        playerAnim["StartRoll"].speed = 3;
        initialPosition = new Vector3(0.5f, 0.5f, 0);
        this.transform.position = initialPosition;
        isBoosted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            // Move the player endelessly and limit the max speed
            rb.AddForce(Speed * transform.right, ForceMode.Acceleration);

            if(isBoosted)
            {
                if (rb.velocity.x != boostSpeed)
                {
                    rb.velocity = new Vector3(boostSpeed, rb.velocity.y);
                }
            }

            else
            {
                if (rb.velocity.x != Speed)
                {
                    rb.velocity = new Vector3(Speed, rb.velocity.y);
                }
            }

            // Limit glide speed
            if (isGliding && rb.velocity.y < 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * 0.785f, 0);
            }
        }
    }

    #endregion

    #region F U N C T I O N S || C O R O U T I N E S 

    public void ChangeSkin(Texture Skin)
    {
        PlayerMaterial.SetTexture("_MainTex", Skin); //Sets Albedo
        PlayerMaterial.SetTexture("_MetallicGlossMap", Skin); //Sets Gloss Map
    }

    public void InitializePlayer()
    {
        //Play the player animation
        ClosePlayer();
        Invoke("OpenAnimation", 0.4f);

        //Initialize the player booleans and positions
        this.transform.position = initialPosition;
        isMoving = false;
        canJump = true;
        canDoubleJump = false;
        canGlide = false;
        isGliding = false;
        Trail.enabled = false;

        //Reset the rigidbody component
        rb.useGravity = true;
        rb.detectCollisions = true;
        rb.velocity = Vector3.zero;      
    }

    public void InitializeBoostSpeed(float boost)
    {
        boostSpeed = boost;
    }

    public void ToggleBoost()
    {
        isBoosted = !isBoosted;
    }

    public void InitializeMagnet(float r)
    {
        magnetRange = r;
        StartCoroutine("Attract");
    }

    IEnumerator Attract()
    {
        while (true)
        {
            foreach (Collider collider in Physics.OverlapSphere(this.transform.position + Vector3.up * 0.2f, magnetRange))
            {
                if (collider.tag == "Coin")
                {
                    collider.gameObject.GetComponent<PickUpController>().StartSeek(this.transform);
                }
            }

            yield return null;
        }
    }

    void ClosePlayer()
    {
        playerAnim["Close"].speed = 100;
        playerAnim.Play("Close");
    }

    void OpenAnimation()
    {
        playerAnim.Play("OpenUp");
        playerAnim["Close"].speed = 1;
    }

    public void StartRoll()
    {
        if(playerAnim.IsPlaying("OpenUp"))
        {
            Invoke("StartRoll", (playerAnim["OpenUp"].length - playerAnim["OpenUp"].time)/playerAnim["OpenUp"].speed);
        }

        else
        {
            playerAnim.Play("StartRoll");
            Invoke("StartMoving", playerAnim["StartRoll"].length / playerAnim["StartRoll"].speed);
        }
    }

    void StartMoving()
    {
        EventManager.playerMoved.Invoke();

        playerAnim.Play("Roll");
        playerAnim["Roll"].speed = 1;
        canJump = true;
        canDoubleJump = false;
        canGlide = false;
        isGliding = false;
        isMoving = true;
        rb.velocity = new Vector3(Speed, 0, 0);
        Trail.enabled = true;
        rb.detectCollisions = true;   
    }

    public void StopMoving()
    {
        //Freeze the player in his position
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        isMoving = false;
        canJump = false;
        canDoubleJump = false;
        canGlide = false;

        if (isGliding)
        {
            isGliding = false;

            SoundManager.Instance.Glide.Stop();
        }

        Trail.enabled = false;

        //Prevents the player from moving if the player quits and choose another level
        CancelInvoke();
    }

    //Freeze/Unfreeze player in place to show tutorial pop up
    public void ToggleMove()
    {
        if(isMoving)
        { 
            //Place the character on the ground
            this.transform.position += Vector3.down * this.transform.position.y + Vector3.up * 0.5f;
            rb.velocity = Vector3.zero;
            isMoving = false;
            playerAnim["Roll"].speed = 0;

            if(isGliding)
            {
                SoundManager.Instance.Glide.Stop();
            }
        }

        else
        {
            isMoving = true;
            rb.velocity = Vector3.right * Speed;
            playerAnim["Roll"].speed = 1;
            canJump = true;
            canDoubleJump = false;
            canGlide = false;
            isGliding = false;
        }
    }

    public void Down()
    {
        if (isMoving)
        {
            if (canJump)
            {
                Jump();
            }
            else if (canDoubleJump)
            {
                DoubleJump();
            }

            else if(canGlide && !isGliding)
            {
                Glide();
            }
        }  
    }

    public void Up()
    {
        if(isMoving)
        {
            // Enables free fall/stop gliding
            if (isGliding)
            {
                isGliding = false;
                playerAnim["Roll"].speed = 1;

                SoundManager.Instance.Glide.Stop();
            }
        }
    }

    public void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, 0);
        rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        canDoubleJump = true;
        canJump = false;
        playerAnim["Roll"].speed = 1.5f;

        SoundManager.Instance.Jump.time = 0.1f;
        SoundManager.Instance.Jump.Play();

        //Zoom Out
        Camera.main.GetComponentInParent<CameraFollow>().SetZoom(1);
    }

    public void DoubleJump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, 0);
        rb.AddForce(Vector3.up * DoubleJumpForce, ForceMode.Impulse);
        canDoubleJump = false;
        canJump = false;
        canGlide = true;
        playerAnim["Roll"].speed = 2;

        SoundManager.Instance.DoubleJump.time = 0.1f;
        SoundManager.Instance.DoubleJump.Play();

        //Zoom Out
        Camera.main.GetComponentInParent<CameraFollow>().SetZoom(2);
    }

    public void Glide()
    {
        isGliding = true;
        playerAnim["Roll"].speed = 0.5f;
        SoundManager.Instance.Glide.Play();

        //Zoom Out
        Camera.main.GetComponentInParent<CameraFollow>().SetZoom(3);
    }

    void Died()
    {
        playerAnim["Roll"].speed = 0;
        SoundManager.Instance.Lost.Play();
        EventManager.died.Invoke();
        StopMoving();
        isMoving = false;
        rb.velocity = Vector3.zero;
        rb.detectCollisions = false;
    }

    //Initialize values and respawn player
    public void Respawn()
    {
        //Reset the player
        InitializePlayer();

        //Stop current animation and cancel all invoked functions
        CancelInvoke();

        //Play Animation start moving once it's done
        Invoke("OpenAnimation", 0.5f);
        Invoke("StartRoll", playerAnim["OpenUp"].length / playerAnim["OpenUp"].speed + 0.5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isMoving)
        {
            if (collision.transform.tag == "Ground")
            {
                if (isGliding)
                {
                    SoundManager.Instance.Glide.Stop();
                }

                //Zoom In
                Camera.main.GetComponentInParent<CameraFollow>().SetZoom(0);

                //Reset the roll speed
                playerAnim["Roll"].speed = 1;

                //Enables jumping
                canJump = true;
                isGliding = false;
                canGlide = false;
            }

            else if (collision.transform.tag == "NonWalkable")
            {
                Died();
                
                //Check if the obstacle have an animation and stops it if it does
                Animation animation = collision.gameObject.GetComponentInParent<Animation>();
                if(animation != null)
                {
                    animation.Stop();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isMoving)
        {
            //Coin Collected
            if (other.tag == "Coin")
            {
                var main = PickUp.main;
                main.startColor = Color.yellow;
                PickUp.Play();
                other.GetComponent<PickUpController>().Deactivate();
                EventManager.coin.Invoke(1);
                SoundManager.Instance.Coin.Play();
            }

            //Level Won
            else if (other.tag == "Destination")
            {
                //Check that the player is on the ground
                if (canJump)
                {
                    playerAnim.Play("StopRoll");
                }

                else
                {
                    playerAnim["Roll"].speed = 0;
                }

                SoundManager.Instance.Win.Play();
                EventManager.reachedDestination.Invoke();
                StopMoving();
            }

            //Respawns
            else if (other.tag == "RespawnY")
            {
                Died();
            }

            //Propells the player up in the air
            else if (other.tag == "Propeller")
            {
                if(isGliding)
                {
                    isGliding = false;
                    SoundManager.Instance.Glide.Stop();
                }
                
                rb.velocity = Vector3.zero;
                rb.AddForce(other.transform.up * PropellerForce, ForceMode.Impulse);
                playerAnim["Roll"].speed = 5;
                SoundManager.Instance.Propeller.time = 0.15f;
                SoundManager.Instance.Propeller.Play();

                //Zoom Out
                Camera.main.GetComponentInParent<CameraFollow>().SetZoom(3);
            }

            //Star collected
            else if (other.tag == "Stars")
            {
                EventManager.star.Invoke(other.gameObject);
                var main = PickUp.main;
                main.startColor = Color.yellow;
                PickUp.Play();
                SoundManager.Instance.Star.Play();
            }

            //Magnet Collected
            else if (other.tag == "Magnet")
            {
                var main = PickUp.main;
                main.startColor = Color.white;
                PickUp.Play();
                SoundManager.Instance.PowerUp.Play();
                EventManager.magnetCollected.Invoke();
                other.GetComponent<PickUpController>().Deactivate();
            }

            //Boost Collected
            else if (other.tag == "Boost")
            {
                var main = PickUp.main;
                main.startColor = Color.white;
                PickUp.Play();
                SoundManager.Instance.PowerUp.Play();
                EventManager.boostCollected.Invoke();
                other.GetComponent<PickUpController>().Deactivate();
            }

            //Tutorial Trigger
            else if (other.tag == "Tutorial" && GameManager.Instance.tutorialEnabled)
            {
                EventManager.pop.Invoke();
            }
        } 
    }

    #endregion
}