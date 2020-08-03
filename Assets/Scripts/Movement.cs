using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    Rigidbody2D rb;
    public float speed;

    public bool grounded = false;

    public LayerMask whatIsGround;

    BoxCollider2D boxCollider;

    public float extraHeight;
    public Transform spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");

        //float timeScale = Mathf.Abs(xInput);
        //timeScale = Mathf.Clamp(timeScale, 0.01f, 1);
        //Time.timeScale = timeScale;

        if (Input.GetKey(KeyCode.Q))
        {
            transform.position = spawnPoint.position;
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

        if (!grounded)
        {
            return;
        }

        rb.velocity = new Vector2(xInput * speed, rb.velocity.y);

    }
}
