using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PplayerMovement : MonoBehaviour
{

    private Vector2 direction; // (2, 3)
    public float speed = 0.75f;
    public Animator animator;
    public BoxCollider2D collider;
    public LayerMask collisionMask;

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

        Debug.Log(direction);


        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);
        if (direction.x != 0 || direction.y != 0){
            animator.SetFloat("Speed", 1);
        }
        else
        {
            animator.SetFloat("Speed", 0);
        }
    }

    private void FixedUpdate()
    {
        transform.Translate(direction * Time.deltaTime * speed);
    }

}
