using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{

    Rigidbody2D rb;
    public float speed;

    public bool grounded = false;

    public LayerMask whatIsGround;

    BoxCollider2D boxCollider;

    public float extraHeight;
    public Transform spawnPoint;

    public TimeObject[] timeObjects;

    bool isRewinding = false;

    public Camera mainCamera, stackedCamera;
    UniversalAdditionalCameraData cameraData;

    public LayerMask timeChange, regularPP;
    public float jumpForce = 100;

    float fallMultiplier = 2.5f;
    float lowJumpMultiplier = 2;

    float xInput;
    bool jump = false, highJump = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        cameraData = mainCamera.GetUniversalAdditionalCameraData();
    }

    public void PlayRewindEffect()
    {
        // Switch post process layer of main camera and enable the stacked camera.
        cameraData.volumeLayerMask = timeChange;
        stackedCamera.enabled = true;
    }

    public void StopRewindEffect()
    {
        // Switch post process layer of main camera and disable the stacked camera.
        cameraData.volumeLayerMask = regularPP;
        stackedCamera.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");

        if (Input.GetMouseButtonDown(1))
        {
            PlayRewindEffect();
            foreach (TimeObject timeObject in timeObjects)
            {
                timeObject.StartRewind();
                isRewinding = true;
            }
        } else if (Input.GetMouseButtonUp(1))
        {
            foreach (TimeObject timeObject in timeObjects)
            {
                timeObject.StopRewind();
                isRewinding = false;
            }
            StopRewindEffect();
        }

        if (Input.GetKey(KeyCode.Q))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (isRewinding)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            rb.simulated = false;
            return;
        } else
        {
            rb.isKinematic = false;
            rb.simulated = true;
        }

        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, extraHeight, whatIsGround);

        Color rayColor;
        if (raycastHit)
        {
            grounded = true;
            rayColor = Color.green;
        } else
        {
            grounded = false;
            rayColor = Color.red;
        }

        Debug.DrawRay(boxCollider.bounds.center, Vector2.down * (boxCollider.bounds.extents.y + extraHeight), rayColor);

        jump = Input.GetKeyDown(KeyCode.Space);

        highJump = Input.GetKey(KeyCode.Space);

        if (jump && grounded)
        {
            rb.velocity += Vector2.up * jumpForce;
            jump = false;
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(xInput * speed * Time.fixedDeltaTime, rb.velocity.y);


        //if (!grounded && !jump && !highJump && rb.velocity.y )
        //{
        //    rb.velocity = new Vector2(rb.velocity.x, 0);
        //}

        //if (rb.velocity.y < 0)
        //{
        //    rb.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.fixedDeltaTime;
        //}
        //else if (rb.velocity.y > 0 && !highJump)
        //{
        //    rb.velocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        //}
    }
}
