using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Rigidbody2D rigidBody;

    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float decceleration = 0.2f;

    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpBounds = 0.2f;

    [SerializeField] private float gravity = 2f;
    [SerializeField] private float fallingGravity = 2f;

    [SerializeField] private float fireSpeed = 2f;

    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private GameObject hookPrefab;
    [SerializeField] private VerletRope rope;

    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    private GameObject currentHook;

    public PlayerInputActions inputManager;
    private InputAction move;
    private InputAction jump;
    private InputAction fire;
    private InputAction pause;

    public bool isGrounded = true;

    [SerializeField] private float jumpBufferTime = 0.15f;
    private float jumpBuffer = 0f;
    private bool jumpReleased = false;

    private void Awake()
    {
        inputManager = new PlayerInputActions();
        inputManager.Enable();
        EnableInputs();
    }

    private void OnEnable()
    {
        EnableInputs();
    }

    private void OnDisable()
    {
        DisableInputs(true);
    }

    void Start()
    {
        gameObject.GetComponent<HingeJoint2D>().enabled = false;
    }

    void Update()
    {
        isGrounded = GetIsGrounded();
        Vector2 position = transform.position;
        float direction = move.ReadValue<Vector2>().x;

        // Base movement handling
        float targetVel = (direction * moveSpeed) - rigidBody.linearVelocity.x;
        float accel = Mathf.Abs(targetVel) > 0.01 ? acceleration : decceleration;
        float targetMovement = targetVel * accel;
        rigidBody.AddForce(Vector2.right * targetMovement, ForceMode2D.Force);

        // Jump Handling
        if (isGrounded)
        {
            animator.SetBool("isJumping", false);
            if(jumpBuffer > 0)
            {
                rigidBody.linearVelocityY = 0;
                rigidBody.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
                jumpBuffer = 0;
                animator.SetBool("isJumping", true);
            }
        }

        if (jumpReleased)
        {
            rigidBody.linearVelocityY = Mathf.Min(rigidBody.linearVelocityY, 0.1f);
        }

        jumpBuffer -= Time.deltaTime;
        jumpReleased = false;

        // Falling handling
        if (rigidBody.linearVelocity.y < -0.1f)
        {
            rigidBody.gravityScale = fallingGravity;
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", true);
        }
        else
        {
            rigidBody.gravityScale = gravity;
            animator.SetBool("isFalling", false);
        }

        // Animation states
        if (isGrounded && Mathf.Abs(rigidBody.linearVelocity.x) > 0.1f)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        if (direction > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if(direction < 0)
        {
            spriteRenderer.flipX = true;
        }

        Vector2 anchor = rope.wrapPoint;
        float ropeLen = rope.numNodes * rope.nodeDist;
        float clampX = Mathf.Clamp(transform.position.x, anchor.x - ropeLen, anchor.x + ropeLen);
        float clampY = Mathf.Clamp(transform.position.y, anchor.y - ropeLen, anchor.y + ropeLen);

        transform.position = new Vector3(clampX, clampY, transform.position.z);
    }
    private void Jump(InputAction.CallbackContext context)
    {
        jumpBuffer = jumpBufferTime;
    }

    private void JumpCancel(InputAction.CallbackContext context)
    {
        jumpReleased = true;
    }

    private void Fire(InputAction.CallbackContext context)
    {
        Vector2 position = transform.position;
        Vector2 cursorWorldPos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 cursorDir = (cursorWorldPos - position).normalized;

        if (currentHook == null)
        {
            currentHook = Instantiate(hookPrefab);
            currentHook.transform.position = transform.position;
            currentHook.GetComponent<Rigidbody2D>().linearVelocity = cursorDir * fireSpeed;
        }
        else
        {
            Destroy(currentHook);
        }
    }
    private void Pause(InputAction.CallbackContext context)
    {
        if (!pauseMenu.isPause)
        {
            pauseMenu.Pause();
            DisableInputs(false);
        }
        else
        {
            pauseMenu.Resume();
            EnableInputs();
        }
    }

    /*private void PullRope(InputAction.CallbackContext context)
    {
        rope.RemoveNode();
    }

    private void ReleaseRope(InputAction.CallbackContext context)
    {
        rope.AddNode();
    }*/

    private bool GetIsGrounded()
    {
        Debug.DrawRay(transform.position + (Vector3.left * 0.2f), Vector2.down * jumpBounds, Color.blue);
        bool leftRay = Physics2D.Raycast(transform.position + (Vector3.left * 0.2f), Vector2.down, jumpBounds, LayerMask.GetMask("Ground"));

        Debug.DrawRay(transform.position + (Vector3.right * 0.2f), Vector2.down * jumpBounds, Color.blue);
        bool rightRay = Physics2D.Raycast(transform.position + (Vector3.right * 0.2f), Vector2.down, jumpBounds, LayerMask.GetMask("Ground"));
        return leftRay || rightRay;
    }

    public void EnableInputs()
    {
        move = inputManager.Player.Move;
        move.Enable();

        jump = inputManager.Player.Jump;
        jump.Enable();
        jump.performed += Jump;
        jump.canceled += JumpCancel;

        fire = inputManager.Player.Fire;
        fire.Enable();
        fire.performed += Fire;

        pause = inputManager.Player.Pause;
        pause.Enable();
        pause.performed += Pause;
    }

    public void DisableInputs(bool pDisable)
    {
        move.Disable();
        jump.Disable();
        fire.Disable();
        if (pDisable) pause.Disable();
    }
}
