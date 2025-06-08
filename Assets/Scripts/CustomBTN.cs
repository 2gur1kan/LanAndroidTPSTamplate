using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class CustomBTN : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Action onHold;
    public Action onDown;
    public Action onUp;

    private bool isHolding = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
        onDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
        onUp?.Invoke();
    }

    void Update()
    {
        if (isHolding)
        {
            onHold?.Invoke();
        }
    }
}