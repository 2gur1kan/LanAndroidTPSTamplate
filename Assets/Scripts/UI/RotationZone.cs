using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RotationZone : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private Transform player;
    private Transform aimTarget;

    [SerializeField] private Image cameraBTNImage;

    [SerializeField] private float rotationSpeedHorizontal = 0.2f;
    [SerializeField] private float rotationSpeedVertical = 0.2f;
    [SerializeField] private float pitchClampMin = -45f;
    [SerializeField] private float pitchClampMax = 45f;

    private bool isDragging = false;
    private float currentPitch = 0f;
    private float currentYaw = 0f;

    private bool flag = false;
    private bool deadFlag = false;

    public void Flag(bool isDead = false)
    {
        if (deadFlag) return;

        AlignPlayerToAimTarget();

        flag = !flag;

        // tuþa basýldýðýnda rengini deðþitirir
        if (flag) cameraBTNImage.color = Color.red;
        else cameraBTNImage.color = Color.white;

        deadFlag = isDead;
        if (deadFlag) flag = true;
    }

    public void SetTarget(Transform player, Transform aimTarget)
    {
        this.player = player;
        this.aimTarget = aimTarget;
        currentPitch = aimTarget.localEulerAngles.x;

        // Eðer 270 üzerindeyse negatif açýya çevir
        if (currentPitch > 180f) currentPitch -= 360f;

        // yeniden doðunca kamera kilidini kaldýrýr
        deadFlag = false;
        flag = true;

        Flag();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || player == null || aimTarget == null) return;

        // --- YATAY DÖNÜÞ ---
        float deltaX = eventData.delta.x;
        float power = Mathf.Sign(deltaX) * Mathf.Pow(Mathf.Abs(deltaX), 1.2f);
        float targetYaw = power * rotationSpeedHorizontal;

        currentYaw = Mathf.Lerp(currentYaw, targetYaw, Time.deltaTime * 15f);
        Vector3 playerRot = Vector3.up * currentYaw;

        if (flag) aimTarget.Rotate(playerRot, Space.Self);
        else player.Rotate(playerRot, Space.Self);

        // --- DÝKEY DÖNÜÞ ---
        float deltaY = -eventData.delta.y;
        currentPitch += deltaY * rotationSpeedVertical;

        // currentPitch'ý clamp yaparken, mevcut açýyý baz al, ekle, sonra clampla
        float newPitch = aimTarget.localEulerAngles.x;

        // Unity localEulerAngles x açýsý 0-360 aralýðýnda, negatif açý için düzelt
        if (newPitch > 180f) newPitch -= 360f;

        newPitch += deltaY * rotationSpeedVertical;

        newPitch = Mathf.Clamp(newPitch, pitchClampMin, pitchClampMax);

        Vector3 newEuler = aimTarget.localEulerAngles;
        newEuler.x = newPitch;
        aimTarget.localEulerAngles = newEuler;
    }


    public void AlignPlayerToAimTarget()
    {
        if (!flag || player == null || aimTarget == null) return;

        Vector3 aimDirection = player.forward;
        aimDirection.y = 0f; // sadece yatay düzlem (horizontal plane)

        if (aimDirection.sqrMagnitude > 0.001f)
        {
            aimDirection.Normalize();
            Quaternion targetRotation = Quaternion.LookRotation(aimDirection);

            aimTarget.rotation = targetRotation;
 
            // aimTarget.localRotation = Quaternion.identity;
        }
    }

}
