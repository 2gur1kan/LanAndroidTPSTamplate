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
    [SerializeField] private Animator animator;

    private bool isGrounded;
    private bool canJump;
    private float jumpTimerCounter = 1f;

    private Vector3 moveInput;
    private bool moveFlag = true;

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

        btn = GameObject.FindGameObjectWithTag("AttackBTN").GetComponent<CustomBTN>();
        if (btn != null) btn.onDown += SetTriggerPunch;

        SetName(DataBaseManager.Instance.Name);
    }

    private void Start()
    {
        Invoke("AddMeScoreboardInvoke", 2f); // 2 olamsýnýn nedeni ne olur ne olamz belki isimler geç gelir diye

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

        if (ScoreboardManager.Instance != null) ScoreboardManager.Instance.RemovePlayer(transform);
    }

    #region Movement

    private void HandleInput()
    {
        moveInput = new Vector3(joystick.Horizontal, 0f, joystick.Vertical);

        if (moveInput != Vector3.zero) animator.SetBool("Run", true);
        else animator.SetBool("Run", false);
    }

    private void ApplyMovement()
    {
        if (aimTarget == null || !moveFlag ) return;

        Vector3 aimForward = aimTarget.forward.normalized;
        aimForward.y = 0f;
        aimForward.Normalize();

        Vector3 aimRight = aimTarget.right.normalized;
        aimRight.y = 0f;
        aimRight.Normalize();

        Vector3 move = (aimForward * moveInput.z + aimRight * moveInput.x);

        Vector3 velocity = new Vector3(move.x * moveSpeed * speedMull, rb.velocity.y, move.z * moveSpeed * speedMull);

        if (!isGrounded)
        {
            velocity = Vector3.MoveTowards(
                new Vector3(rb.velocity.x, 0, rb.velocity.z),
                new Vector3(velocity.x, 0, velocity.z),
                Time.fixedDeltaTime * moveSpeed / .5f
            );

            velocity.y = rb.velocity.y;
        }

        rb.velocity = velocity;


        if (animator == null) return;

        Vector3 localMove = transform.InverseTransformDirection(move);

        animator.SetFloat("Horizontal", localMove.x);
        animator.SetFloat("Vertical", localMove.z);
    }


    private void Jump()
    {
        if (!canJump) return;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        canJump = false;

        if (animator == null) return;

        animator.SetBool("Jump", true);
        Invoke("ResetJumpTriggerInvoke", .5f);
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

        if (animator != null) animator.SetBool("IsGrounded", isGrounded);

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
    private void ResetJumpTriggerInvoke() => animator.SetBool("Jump", false);
    public void resetMoveFlag() => moveFlag = true;

    #endregion

    #region Attack Systems

    private void SetTriggerPunch()
    {
        animator.SetBool("Punch", true);

        Invoke("ResetTriggerPunchInvoke", .1f);
    }
    private void ResetTriggerPunchInvoke() => animator.SetBool("Punch", false);

    public void PunchMe(Vector3 attackerPosition)
    {
        if (!isServer) return;

        Debug.Log("ben: " + Name);

        TargetApplyKnockback(connectionToClient, attackerPosition);
    }

    [TargetRpc]
    public void TargetApplyKnockback(NetworkConnection conn, Vector3 attackerPos)
    {
        Debug.Log("Bana vuruldu!");

        moveFlag = false;

        Vector3 dir = (transform.position - attackerPos).normalized;
        dir.y += 1f;

        rb.velocity = Vector3.zero;
        rb.AddForce(dir * 31f, ForceMode.Impulse);

        Invoke("resetMoveFlag", .3f);
    }


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

    private void AddMeScoreboardInvoke() => ScoreboardManager.Instance.RegisterPlayer(Name, transform);

    #endregion
}
