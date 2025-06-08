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

        // YATAY Karakter döner
        float deltaX = eventData.delta.x;
        Vector3 playerRot = Vector3.up * deltaX * rotationSpeedHorizontal;
        player.Rotate(playerRot);

        // DÝKEY aimTarget pitch (yukarý-aþaðý) döner
        float deltaY = -eventData.delta.y; // yukarý sürükleme negatif olduðu için ters çevrilir
        currentPitch += deltaY * rotationSpeedVertical;
        currentPitch = Mathf.Clamp(currentPitch, pitchClampMin, pitchClampMax);

        Vector3 newEuler = aimTarget.localEulerAngles;
        newEuler.x = currentPitch;
        aimTarget.localEulerAngles = newEuler;
    }
}
