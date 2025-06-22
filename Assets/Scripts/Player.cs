using UnityEngine;
using Mirror;
using Cinemachine;
using System.Collections;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Player : NetworkBehaviour
{
    [Header("Movement Settings (Hareket Ayarlarý)")]
    [SerializeField] private float moveSpeed = 5f;
    private float speedMull = 1f;
    [SerializeField] private float jumpForce = 60f;

    [Header("References (Referanslar)")]
    [SerializeField] private Transform aimTarget;
    [SerializeField] private Transform gunAim;

    private Joystick joystick;
    private Rigidbody rb;
    private Collider col;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerAnimationController pac;

    private bool isGrounded;
    private bool canJump;
    private float jumpTimerCounter = 1f;

    private Vector3 moveInput;
    private bool moveFlag = true;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        joystick = FindObjectOfType<Joystick>();

        setCamera();

        RotationZone rotZone = FindObjectOfType<RotationZone>();
        if (rotZone != null) rotZone.SetTarget(this.transform,this.aimTarget);

        CustomBTN btn = GameObject.FindGameObjectWithTag("JumpBTN").GetComponent<CustomBTN>();
        if (btn != null) btn.onDown += Jump;

        btn = GameObject.FindGameObjectWithTag("AttackBTN").GetComponent<CustomBTN>();
        if (btn != null) btn.onDown += CalcualteAttackType;

        SetName(DataBaseManager.Instance.Name);
    }

    /// <summary>
    /// kamerayý karakter öldükten sorna ayarlayabilemk için 
    /// </summary>
    public void setCamera()
    {
        CinemachineVirtualCamera vcam = FindObjectOfType<CinemachineVirtualCamera>();

        if (vcam != null)
        {
            vcam.Follow = aimTarget;
            vcam.LookAt = aimTarget;
        }
    }

    private void Start()
    {
        AttachWeaponToHand(weaponName);

        if (!isLocalPlayer)
        {
            Invoke("SetPointerInvoke", 2f); // diðer oyuncularda pointer oluþturur
            return;
        }

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        rb.freezeRotation = true;

        DataBaseManager.Instance.Team = team;
        gameObject.layer = LayerMask.NameToLayer(team == TeamName.A ? "TeamA" : "TeamB");
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

        return;

        //if (GameManager.Instance != null) GameManager.Instance.RemovePlayer(this);

        Debug.Log("karakter yok olup olmadýðýný gam managera taþý");
    }

    #region Movement

    private void HandleInput()
    {
        moveInput = new Vector3(joystick.Horizontal, 0f, joystick.Vertical);
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

    [Header("AttackSystems")]
    [SyncVar] [SerializeField] private TeamName team;
    [SyncVar] [SerializeField] private int maxHealth = 100;
    [SyncVar] [SerializeField] private int currentHealth = 100;
    [SyncVar] private int score;

    [Server]
    public bool TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(currentHealth);

        if (currentHealth < 0)
        {
            Debug.Log("Öldüm Öldüm");

            RpcUpdateScoreboard();
            return true;
        }

        return false;
    }

    [ClientRpc]
    private void RpcUpdateScoreboard()
    {
        ScoreboardManager.Instance.UpdateScoreboard();
    }

    public TeamName TeamName { get => team; set => team = value; }
    public int Score { get => score; set => score = value; }

    private bool attackFlag = false;

    public void CalcualteAttackType()
    {
        switch (weaponType)
        {
            case WeaponType.Pistol:
                UseWeapon();
                break;

            case WeaponType.Rifle:
                UseWeapon();
                break;

            case WeaponType.None:
                NoneWeaponAttack();
                break;
        }
    }

    public void UseWeapon()
    {
        if (attackFlag) return;

        attackFlag = true;
        Invoke("resetAttackFlag", fireRate + .05f);

        Crossair.Instance.RotateCrossair(fireRate);
        BounceAimTarget(fireRate);

        if (WeaponSC != null) CmdUseWeapon(gunAim.position);

        SetTriggerPunch();
    }

    [Command]
    public void CmdUseWeapon(Vector3 targetPos)
    {
        WeaponSC.Fire(targetPos);
    }

    private void resetAttackFlag() => attackFlag = false;
    public void BounceAimTarget(float fireRate, float bounceHeight = 5f) => StartCoroutine(BounceRotationRoutine(fireRate, bounceHeight));

    private IEnumerator BounceRotationRoutine(float fireRate, float bounceAngle)
    {
        Quaternion originalRot = aimTarget.rotation;
        Quaternion targetRot = originalRot * Quaternion.Euler(-bounceAngle, 0f, 0f);

        float duration = fireRate > .1f ? .09f : fireRate / 3;
        float timer = 0f;

        while (timer < duration)
        {
            float t = timer / duration;
            aimTarget.rotation = Quaternion.Slerp(originalRot, targetRot, t);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    /// ///////////////// punch

    private void NoneWeaponAttack()
    {
        SetTriggerPunch();
    }

    private void SetTriggerPunch()
    {
        if (animator == null) return;

        animator.SetBool("Punch", true);

        Invoke("ResetTriggerPunchInvoke", .1f);
    }
    private void ResetTriggerPunchInvoke() => animator.SetBool("Punch", false);

    public void PunchMe(Vector3 attackerPosition)
    {
        if (!isServer) return;

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

    #region Weapon Systems

    [Header("Weapon")]
    [SerializeField] private GameObject WeaponPref;
    [SerializeField] private WeaponController WeaponSC;
    [SerializeField] private WeaponName weaponName;
    [SerializeField] private WeaponType weaponType;

    [SerializeField] private float fireRate = 1f;

    public void AttachWeaponToHand(WeaponName gg)
    {
        Transform rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        if (rightHand == null) return;

        pac.SetAimRigWeight(gg != WeaponName.None ? 1 : 0);

        if (gg == WeaponName.None)
        {
            Destroy(WeaponPref);
            SetWeaponAnimator(WeaponType.None);
            return;
        }

        Weapon weapon = DataBaseManager.Instance.GetWeapon(gg);

        SetWeaponAnimator(weapon.type);
        weaponName = gg;
        weaponType = weapon.type;
        fireRate = weapon.fireRate;

        if (WeaponPref != null) Destroy(WeaponPref);

        WeaponPref = Instantiate(weapon.go, rightHand);

        WeaponPref.transform.localPosition = weapon.go.transform.localPosition;
        WeaponPref.transform.localRotation = weapon.go.transform.localRotation;
        WeaponPref.transform.localScale = weapon.go.transform.localScale;

        WeaponSC = WeaponPref.GetComponent<WeaponController>();
        WeaponSC.team = team;
        WeaponSC.CurrentPlayer = this;
    }

    private void SetWeaponAnimator(WeaponType WT) => animator.SetInteger("Weapon", (int)WT);

    #endregion

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
        if (PointersPanelController.Instance == null || team != DataBaseManager.Instance.Team) return;

        pointer = PointersPanelController.Instance.CreateEnemyPointer(transform);

        pointer.setText(Name);
    }

    #endregion
}
