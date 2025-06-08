using UnityEngine;
using Mirror;
using Cinemachine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Player : NetworkBehaviour
{
    [Header("Movement Settings (Hareket Ayarlarý)")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 20f;

    [Header("References (Referanslar)")]
    [SerializeField] private Transform aimTarget;

    private Joystick joystick;
    private Rigidbody rb;
    private Collider col;

    private bool isGrounded;
    private bool canJump;
    private float jumpTimerCounter = 1f;

    private Vector3 moveInput;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        joystick = FindObjectOfType<Joystick>();

        CinemachineVirtualCamera vcam = FindObjectOfType<CinemachineVirtualCamera>();

        if (vcam != null)
        {
            vcam.Follow = aimTarget;
            vcam.LookAt = this.transform;
        }

        RotationZone rotZone = FindObjectOfType<RotationZone>();
        if (rotZone != null) rotZone.SetTarget(this.transform,this.aimTarget);

                CustomBTN btn = GameObject.FindGameObjectWithTag("JumpBTN").GetComponent<CustomBTN>();
        if (btn != null) btn.onDown += Jump;
    }

    private void Start()
    {
        if (!isLocalPlayer)
        {
            enabled = false;
            GetComponentInChildren<Camera>().enabled = false;
            return;
        }

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        HandleInput();
        CheckGrounded();
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        ApplyMovement();
    }

    #region Movement (Hareket Sistemi)

    private void HandleInput()
    {
        moveInput = new Vector3(joystick.Horizontal, 0f, joystick.Vertical);
    }

    private void ApplyMovement()
    {
        Vector3 move = Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * moveInput.normalized;
        Vector3 velocity = new Vector3(move.x * moveSpeed, rb.velocity.y, move.z * moveSpeed);
        rb.velocity = velocity;
    }

    private void Jump()
    {
        if (!canJump) return;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // y sýfýrlanýr (reset)
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        canJump = false; // Zýpladýktan sonra tekrar zýplamayý engelle
    }

    private void CheckGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        float radius = 0.25f;
        float rayLength = 1f;

        Debug.DrawRay(origin, Vector3.down * rayLength, Color.red);

        bool previousGrounded = isGrounded;

        isGrounded = false; 

        if (Physics.SphereCast(origin, radius, Vector3.down, out RaycastHit hit, rayLength))
        {
            if (!hit.collider.CompareTag("Player"))
            {
                isGrounded = true;
            }
        }

        if (canJump) return;

        if (!previousGrounded && isGrounded)
        {
            canJump = true;
            setJumpTimerCounter();
        }
        else if(isGrounded)
        {
            jumpTimerCounter -= Time.deltaTime;

            if(jumpTimerCounter < 0)
            {
                canJump = true;
                setJumpTimerCounter();
            }
        }

    }

    private void setJumpTimerCounter() => jumpTimerCounter = 1f;

    #endregion
}
