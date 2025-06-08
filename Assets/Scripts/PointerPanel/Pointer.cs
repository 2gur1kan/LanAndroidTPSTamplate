using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Pointer : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private TextMeshProUGUI text;
    private RectTransform rectTransform;
    private Image itsMe;
    private Collider col;

    private float scale;

    public Transform Target { get => target; set => target = value; }

    public void setText(string gg) => text.text = gg;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        scale = Screen.height / 15;

        itsMe = GetComponent<Image>();
    }

    private void Start()
    {
        col = target.GetComponent<Collider>();
    }

    private void Update()
    {
        if (target == null || rectTransform == null || col == null) return;

        float borderSize = Screen.height / 20f;

        // Collider'ın en üst noktası + offset
        Vector3 topPoint = col.bounds.center + Vector3.up * (col.bounds.extents.y + 0.2f); // 0.2f = offset
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(topPoint);

        // Eğer hedef kamera arkasındaysa (Z < 0), yönü ters çevir
        if (screenPoint.z < 0)
        {
            screenPoint *= -1;
        }

        // Ekran dışı kontrolü
        bool isOffScreen = screenPoint.x <= borderSize || screenPoint.x >= Screen.width - borderSize ||
                           screenPoint.y <= borderSize || screenPoint.y >= Screen.height - borderSize;

        if (isOffScreen)
        {
            itsMe.enabled = true;

            // Sınırlar içine al
            screenPoint.x = Mathf.Clamp(screenPoint.x, borderSize, Screen.width - borderSize);
            screenPoint.y = Mathf.Clamp(screenPoint.y, borderSize, Screen.height - borderSize);

            RectTransform panelRect = transform.parent.GetComponent<RectTransform>();

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRect, screenPoint, null, out Vector2 localPoint))
            {
                rectTransform.localPosition = new Vector3(localPoint.x, localPoint.y, 0f);
            }
        }
        else
        {
            itsMe.enabled = true;

            Canvas canvas = GetComponentInParent<Canvas>();
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null, out Vector2 localPos);
            rectTransform.localPosition = new Vector3(localPos.x, localPos.y, 0);
        }
    }


    public void Destroy()
    {
        target = null;
        rectTransform = null;
        Destroy(gameObject, .1f);
    }
}
