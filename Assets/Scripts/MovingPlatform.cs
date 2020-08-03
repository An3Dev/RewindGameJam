using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour, IMoveable
{

    Rigidbody2D rb;

    public Vector2 direction;

    float delay = 0.5f;
    public float speed = 1;

    bool move = false;
    public GameObject stopper;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Invoke("StartMoving", delay);
    }

    // Update is called once per frame
    void Update()
    {
        if (move)
        {
            rb.velocity = direction * speed;
        }
    }

    public void StartMoving()
    {
        move = true;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == stopper.gameObject.name )
        {
            move = false;
            Debug.Log(collision.gameObject);
        }
    }
}
