using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragnDropUnit : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{

    private RectTransform rectTransform;
    [SerializeField] private Canvas canvas;
    private CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Awake()
    {

        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

    }

    public void OnPointerDown(PointerEventData eventData)
    {

        Debug.Log("OnPointerDown");

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

}
