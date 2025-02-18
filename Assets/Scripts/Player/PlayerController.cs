using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private Collider2D normalCollider;
    [SerializeField] private Collider2D slideCollider;
    [SerializeField] private PhysicsMaterial2D frictionless;

    [Header("Health")]
    [SerializeField] private int chance;
    [SerializeField] private float recoilFromWallSpeed;
    [SerializeField] private float stunDelay;
    [SerializeField] private float recoverDelay;
    private int currentChance;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    private bool canMove;

    [Header("Wall Jump")]
    [SerializeField] private Transform wallCheckPosition;
    [SerializeField] private float wallCheckDistance;
    private bool canWallJump;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPosition;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;
    private bool sliding;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.sharedMaterial = frictionless;
        canMove = true;

        normalCollider.enabled = true;
        slideCollider.enabled = false;

        InputManager.Instance.OnJump += OnJump;
        InputManager.Instance.OnInteract += OnInteract;

        InputManager.Instance.OnSlideStart += StartSlide;
        InputManager.Instance.OnSlideEnd += EndSlide;

        currentChance = chance;
    }

    private void Update()
    {
        GroundCheck();
        Movement();
        WallCheck();
    }

    private void GroundCheck()
    {
        bool onGround = Physics2D.OverlapBox(groundCheckPosition.position, groundCheckSize, 0, groundLayer);

        if (onGround)
        {
            if(!isGrounded)
            {
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                canMove = true;
            }

            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
    private void Movement()
    {
        if(canMove)
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
    }
    private void WallCheck()
    {
        if(Physics2D.Raycast(wallCheckPosition.position, transform.right, wallCheckDistance, groundLayer))
        {
            if (isGrounded)
            {
                Stun();
            }
            else
            {
                canWallJump = true;
                canMove = false;
            }
        }
        else
        {
            canWallJump = false;
        }
    }

    private void Stun()
    {
        if(currentChance < 0)
        {
            GameManager.Instance.SetState(GameState.Over);
            return;
        }

        rb.sharedMaterial = null;
        currentChance--;
        canMove = false;
        rb.velocity = Vector2.zero;
        rb.AddForce(-transform.right * recoilFromWallSpeed, ForceMode2D.Impulse);

        Invoke("RecoverFromStun", stunDelay);
        Invoke("RecoverChance", recoverDelay);
    }

    private void RecoverFromStun()
    {
        canMove = true;
        rb.sharedMaterial = frictionless;
    }

    private void RecoverChance()
    {
        currentChance++;
        if (currentChance > chance)
            currentChance = chance;
    }

    //Input
    private void OnJump()
    {
        if (sliding)
            return;

        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0.0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        else
        {
            if(canWallJump)
            {
                rb.velocity = Vector2.zero;

                if (transform.eulerAngles.y < 180.0f)
                    transform.eulerAngles = Vector3.up * 180.0f;
                else
                    transform.eulerAngles = Vector3.zero;

                rb.AddForce(new Vector2(transform.right.x, 0.8f) * jumpForce, ForceMode2D.Impulse);
            }
        }
    }
    private void OnInteract()
    {
        Debug.Log("Interact");
    }

    private void StartSlide()
    {
        sliding = true;
        normalCollider.enabled = false;
        slideCollider.enabled = true;
    }
    private void EndSlide()
    {
        sliding = false;
        normalCollider.enabled = true;
        slideCollider.enabled = false;
    }

    //Cleanup
    private void OnDestroy()
    {
        InputManager.Instance.OnJump -= OnJump;
        InputManager.Instance.OnInteract -= OnInteract;

        InputManager.Instance.OnSlideStart -= StartSlide;
        InputManager.Instance.OnSlideEnd -= EndSlide;
    }

    //Debug
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheckPosition.position, groundCheckSize);

        Gizmos.DrawRay(wallCheckPosition.position, transform.right * wallCheckDistance);
    }
}
