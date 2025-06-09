using UnityEngine;
using Mirror;
using Cinemachine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Player : NetworkBehaviour
{
    [Header("Movement Settings (Hareket Ayarlarý)")]
    [SerializeField] private float moveSpeed = 5f;
    private float speedMull = 1f;
    [SerializeField] private float jumpForce = 60f;

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

        SetName(DataBaseManager.Instance.Name);
    }

    private void Start()
    {
        if (!isLocalPlayer)
        {
            Invoke("SetPointerInvoke", 2f); // diðer oyuncularda pointer oluþturur
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

    private void OnDestroy()
    {
        if (pointer != null) pointer.Destroy();
    }

    #region Movement

    private void HandleInput()
    {
        moveInput = new Vector3(joystick.Horizontal, 0f, joystick.Vertical);
    }

    private void ApplyMovement()
    {
        if (aimTarget == null) return; // aimTarget yoksa iþlem yapma

        // aimTarget'ýn ileri yönü (forward direction) - sadece yatay eksende
        Vector3 aimForward = aimTarget.forward;
        aimForward.y = 0f;
        aimForward.Normalize(); // birim vektör yap (normalize)

        // aimTarget'ýn sað yönü (right direction) - sadece yatay eksende
        Vector3 aimRight = aimTarget.right;
        aimRight.y = 0f;
        aimRight.Normalize();

        // Giriþe (input) göre hareket yönünü hesapla (movement direction)
        Vector3 move = (aimForward * moveInput.z + aimRight * moveInput.x).normalized;

        // Rigidbody hýzýný ayarla (velocity)
        Vector3 velocity = new Vector3(move.x * moveSpeed * speedMull, rb.velocity.y, move.z * moveSpeed * speedMull);
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

    #region Name System

    [HideInInspector] [SyncVar(hook = nameof(OnNameChanged))] public string Name;

    /// <summary>
    /// playerýn ismini bütün serverda ayarlar 
    /// </summary>
    /// <param name="newName"></param>
    public void SetName(string newName)
    {
        if (isLocalPlayer)
        {
            CmdSetName(newName);
        }
    }

    [Command]
    private void CmdSetName(string newName)
    {
        Name = newName;
    }

    private void OnNameChanged(string oldName, string newName)
    {
        Debug.Log($"Ýsim deðiþti: {oldName} -> {newName}");
    }

    [Command]
    private void CmdRequestNames()
    {
        foreach (var conn in NetworkServer.connections)
        {
            if (conn.Value.identity.TryGetComponent<Player>(out var player))
            {
                player.TargetSendName(player.connectionToClient, player.Name);
            }
        }
    }

    [TargetRpc]
    private void TargetSendName(NetworkConnection target, string name)
    {
        CmdSetName(name);
    }

    #endregion

    #region In Local Fonc

    private Pointer pointer;

    private void SetPointerInvoke()
    {
        if (PointersPanelController.Instance == null) return;

        pointer = PointersPanelController.Instance.CreateEnemyPointer(transform);

        pointer.setText(Name);
    }

    #endregion
}
