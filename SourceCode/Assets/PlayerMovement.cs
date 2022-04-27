using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private Vector2 direction; // (2, 3)
    public float speed = 0.75f;
    public Animator animator;
    new public BoxCollider2D collider;
    public LayerMask collisionMask;
    private RaycastHit2D hit;
    private SoundRandomizer walkSounds;
    public enum dir_state
    {
        up, right, down, left
    }
    public int facing;

    // Start is called before the first frame update
    void Start()
    {
        walkSounds = GetComponent<SoundRandomizer>();
        // This is used to remember where the player was when moving between rooms for shop, minigame, etc
        if (PauseMenu.Instance) transform.position = PauseMenu.Instance.playerPosition;
    }

    // Update is called once per frame
    void Update()
    {
        // Cancel if we're paused
        if (PauseMenu.Instance != null) {
            if (PauseMenu.Instance.menuCanvas.enabled) {
                direction = Vector2.zero;
                animator.SetFloat("Speed", 0);
                walkSounds.StopSound();
                return;
            }
        }

        direction.x = Input.GetAxisRaw("Horizontal");
        direction.y = Input.GetAxisRaw("Vertical");
        direction.Normalize();

        //Debug.Log(direction);


        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);
        if (direction.x != 0 || direction.y != 0)
        {
            animator.SetFloat("Speed", 1);
            walkSounds.StartSound();

            if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
            {
                if (direction.y > 0) {
                    facing = (int) dir_state.down;
                }
                else
                {
                    facing = (int) dir_state.up;
                }
            }
            else
            {
                if (direction.x > 0)
                {
                    facing = (int) dir_state.right;
                }
                else
                {
                    facing = (int) dir_state.left;
                }
            }
            animator.SetInteger("Facing", facing);
        }
        else
        {
            animator.SetFloat("Speed", 0);
            walkSounds.StopSound();
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
