using UnityEngine;
using UnityEngine.EventSystems;

public class RotationZone : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private Transform player;
    private Transform aimTarget;

    [SerializeField] private float rotationSpeedHorizontal = 0.2f;
    [SerializeField] private float rotationSpeedVertical = 0.2f;
    [SerializeField] private float pitchClampMin = -45f;
    [SerializeField] private float pitchClampMax = 45f;

    private bool isDragging = false;
    private float currentPitch = 0f;

    private bool flag = false;

    public void Flag()
    {
        AlignPlayerToAimTarget();

        flag = !flag;
    }

    public void SetTarget(Transform player, Transform aimTarget)
    {
        this.player = player;
        this.aimTarget = aimTarget;
        currentPitch = aimTarget.localEulerAngles.x;

        // Eðer 270 üzerindeyse negatif açýya çevir
        if (currentPitch > 180f) currentPitch -= 360f;
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

        // YATAY dönüþ
        float deltaX = eventData.delta.x;
        Vector3 playerRot = Vector3.up * deltaX * rotationSpeedHorizontal;

        if (flag) aimTarget.Rotate(playerRot); // aimTarget döner
        else player.Rotate(playerRot); // normalde player döner

        // DÝKEY aimTarget pitch (yukarý-aþaðý) döner
        float deltaY = -eventData.delta.y;
        currentPitch += deltaY * rotationSpeedVertical;
        currentPitch = Mathf.Clamp(currentPitch, pitchClampMin, pitchClampMax);

        Vector3 newEuler = aimTarget.localEulerAngles;
        newEuler.x = currentPitch;
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
