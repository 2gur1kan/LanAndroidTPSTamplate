using System.Collections;
using UnityEngine;

public class Crossair : MonoBehaviour
{
    public static Crossair Instance;

    [SerializeField] private GameObject Target;
    [SerializeField] private Camera mainCamera;

    private RectTransform rectTransform;
    private Vector3 currentPos;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        rectTransform = GetComponent<RectTransform>();
        if (mainCamera == null)
            mainCamera = Camera.main;

        currentPos = rectTransform.position;
    }

    private void LateUpdate()
    {
        setCrossair();
    }

    private void setCrossair()
    {
        if (Target == null || mainCamera == null) return;

        Vector3 screenPos = mainCamera.WorldToScreenPoint(Target.transform.position);

        if (screenPos.z > 0)
        {
            currentPos = Vector3.Lerp(currentPos, screenPos, Time.deltaTime * 10f);
            rectTransform.position = currentPos;
        }
        else
        {
            currentPos = Vector3.Lerp(currentPos, new Vector3(-1000, -1000), Time.deltaTime * 10f);
            rectTransform.position = currentPos;
        }
    }

    public void RotateCrossair(float duration)
    {
        StartCoroutine(RotateAndReset(duration));
    }

    private IEnumerator RotateAndReset(float duration)
    {
        float elapsed = 0f;

        Quaternion startRot = Quaternion.Euler(0f, 0f, 0f);
        Quaternion endRot = Quaternion.Euler(0f, 0f, 90f);

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.rotation = Quaternion.Lerp(startRot, endRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = startRot;
    }
}
