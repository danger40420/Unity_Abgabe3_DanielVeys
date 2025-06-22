using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterControllerSide : MonoBehaviour
{
    [SerializeField] private float speed = 10f; // Horizontal movement speed
    [SerializeField] private float jumpForce = 5f; // Force applied when jumping
    private float direction = 0f; // Current horizontal movement direction

    #region GhostPhysics
    [SerializeField] private float normalGravity = 3f; // Gravity when not hovering
    [SerializeField] private float hoverGravity = 0.3f; // Lower gravity while hovering
    [SerializeField] private float hoverFallSpeedLimit = -2f; // Max downward speed during hover
    #endregion

    #region CoyoteTime
    [SerializeField] private float coyoteTime = 0.2f; // Time window to still jump after falling
    private float coyoteTimeCounter; // Countdown for coyote time
    #endregion

    bool canMove = false; // Controls whether player movement is allowed
    private bool isGrounded = false; // Whether the player is touching the ground

    private Vector3 startPosition; // 🔁 Store player's initial spawn position

    private Rigidbody2D rb;

    [Header("GroundCheck")]
    [SerializeField] private Transform transformGroundCheck; // Ground check position
    [SerializeField] private LayerMask layerGround; // Ground layers to detect

    [Header("Manager")]
    [SerializeField] private CollectibleManager collectibleManager; // Reference to collectible manager
    [SerializeField] private UIManager uiManager; // Reference to UI manager

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get Rigidbody2D component
        rb.gravityScale = normalGravity; // Set initial gravity

        startPosition = transform.position; // 🔁 Save the initial position for resets
    }

    public void EnableMovement()
    {
        canMove = true; // Allow the player to start moving
    }

    public void DisableMovement()
    {
        canMove = false; // Stop movement input
        rb.linearVelocity = Vector2.zero; // Stop motion
    }

    public void ResetToStartPosition() // 🔁 Public method to reset player to original location
    {
        transform.position = startPosition; // Move player back to start
        rb.linearVelocity = Vector2.zero; // Clear motion
    }

    void Update()
    {
        if (canMove)
        {
            direction = 0f; // Reset direction each frame

            if (Keyboard.current.aKey.isPressed)
                direction = -1; // Move left if A is pressed

            if (Keyboard.current.dKey.isPressed)
                direction = 1; // Move right if D is pressed

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
                Jump(); // Try jumping if space was just pressed

            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y); // Apply horizontal movement
        }

        CheckGrounded(); // Update isGrounded state

        if (isGrounded)
            coyoteTimeCounter = coyoteTime; // Reset coyote time when on ground
        else
            coyoteTimeCounter -= Time.deltaTime; // Decrease coyote timer while in air

        if (!isGrounded && Keyboard.current.spaceKey.isPressed)
        {
            rb.gravityScale = hoverGravity; // Lower gravity while holding jump midair

            if (rb.linearVelocity.y < hoverFallSpeedLimit)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, hoverFallSpeedLimit); // Cap fall speed while hovering
        }
        else
        {
            rb.gravityScale = normalGravity; // Restore normal gravity when not hovering
        }
    }

    void CheckGrounded()
    {
        // Use an overlap circle to check if player is on the ground
        isGrounded = Physics2D.OverlapCircle(transformGroundCheck.position, 0.1f, layerGround);
    }

    void Jump()
    {
        if (coyoteTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); // Apply upward jump force
            coyoteTimeCounter = 0f; // Reset coyote time to prevent double jump
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("We collided with something!");

        if (other.CompareTag("Wisps"))
        {
            Debug.Log("It was a Wisp!");
            collectibleManager.AddWisp(); // Tell manager to add a Wisp
            Destroy(other.gameObject); // Remove the Wisp from the scene
        }

        if (other.CompareTag("Orbs"))
        {
            Debug.Log("It was a Spirit Orb!");
            collectibleManager.AddOrb(); // Tell manager to add an Orb
            Destroy(other.gameObject); // Remove the Orb from the scene
        }

        if (other.CompareTag("Traps"))
        {
            Debug.Log("It was a Trap!");
            uiManager.ShowPanelLost(); // Trigger the "you lost" panel
            rb.linearVelocity = Vector2.zero; // Stop all player movement
            canMove = false; // Disable further input/movement
        }

        if (other.CompareTag("Goal"))
        {
            Debug.Log("You won!");
            uiManager.ShowPanelWin(); // Trigger the "you win" panel
            rb.linearVelocity = Vector2.zero; // Stop all player movement
            canMove = false; // Disable further input/movement
        }
    }
}
