using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;
    public float turnSpeed = 10f;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private Transform cam;
    private float groundedTimer;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        cam = Camera.main.transform;
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 camForward = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;

        Vector3 move = camRight * x + camForward * z;
        move = Vector3.ClampMagnitude(move, 1f);

        if (move.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
        }

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float speed = isRunning ? runSpeed : walkSpeed;

        // Ground check with a short grace period so it doesn't flicker
        if (controller.isGrounded) groundedTimer = 0.15f;
        else groundedTimer -= Time.deltaTime;
        bool isGrounded = groundedTimer > 0f;

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = jumpForce;
            groundedTimer = 0f;
            animator.SetTrigger("Jump");
        }

        velocity.y += gravity * Time.deltaTime;

        // One Move call per frame
        Vector3 total = move * speed + velocity;
        controller.Move(total * Time.deltaTime);

        animator.SetFloat("Speed", move.magnitude);
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsGround", isGrounded);
    }
}