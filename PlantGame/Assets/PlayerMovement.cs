using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private Vector2 direction; // (2, 3)
    public float speed = 0.75f;
    public Animator animator;
    public BoxCollider2D collider;
    public LayerMask collisionMask;
    private RaycastHit2D hit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        direction.x = Input.GetAxisRaw("Horizontal");
        direction.y = Input.GetAxisRaw("Vertical");
        direction.Normalize();

        //Debug.Log(direction);


        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);
        if (direction.x != 0 || direction.y != 0)
        {
            animator.SetFloat("Speed", 1);
        }
        else
        {
            animator.SetFloat("Speed", 0);
        }
    }

    private void FixedUpdate()
    {
        hit = Physics2D.BoxCast(transform.position, collider.size, 0, new Vector2(0, direction.y), Mathf.Abs(direction.y * Time.deltaTime * speed), LayerMask.GetMask("Collision"));
        if (hit.collider == null)
        { 
           transform.Translate(0, direction.y * Time.deltaTime * speed, 0);
        }
        hit = Physics2D.BoxCast(transform.position, collider.size, 0, new Vector2(direction.x, 0), Mathf.Abs(direction.x * Time.deltaTime * speed), LayerMask.GetMask("Collision"));
        if (hit.collider == null)
        {
            transform.Translate(direction.x * Time.deltaTime * speed,0, 0);
        }
    }

}