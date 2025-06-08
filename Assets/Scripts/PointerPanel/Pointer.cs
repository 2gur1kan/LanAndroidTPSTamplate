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

    private float scale;

    public Transform Target { get => target; set => target = value; }

    public void setText(string gg) => text.text = gg;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        scale = Screen.height / 15;

        itsMe = GetComponent<Image>();

        //setText(target.GetComponent<DragonController>().DragonName);
    }

    private void Start()
    {
        //rectTransform.localScale = new(scale, scale, 1);
    }

    private void Update()
    {
        if (target == null || rectTransform == null) return;

        float borderSize = Screen.height / 20;

        // Hedefin dünya pozisyonunu ekran pozisyonuna çevir
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(target.position);

        // Eğer hedef kamera arkasındaysa (Z < 0), yönü ters çevir
        if (screenPoint.z < 0)
        {
            screenPoint *= -1;
        }

        // Ekran dışı mı?
        bool isOffScreen = screenPoint.x <= borderSize || screenPoint.x >= Screen.width - borderSize ||
                           screenPoint.y <= borderSize || screenPoint.y >= Screen.height - borderSize;

        if (isOffScreen)
        {
            itsMe.enabled = true;

            // Clamp ile pozisyonu ekran sınırları içine al
            screenPoint.x = Mathf.Clamp(screenPoint.x, borderSize, Screen.width - borderSize);
            screenPoint.y = Mathf.Clamp(screenPoint.y, borderSize, Screen.height - borderSize);

            // Panel referansı
            RectTransform panelRect = transform.parent.GetComponent<RectTransform>();

            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRect, screenPoint, Camera.main, out localPoint))
            {
                rectTransform.localPosition = new Vector3(localPoint.x, localPoint.y, 0f);
            }
        }
        else
        {
            itsMe.enabled = true;

            // Hedef ekran içindeyse direkt olarak oraya yerleştir
            Vector2 localPos;
            Canvas canvas = GetComponentInParent<Canvas>();
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, canvas.worldCamera, out localPos);
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
