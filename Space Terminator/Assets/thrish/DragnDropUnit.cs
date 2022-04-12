using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragnDropUnit : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerUpHandler
{

    private RectTransform rectTransform;
    [SerializeField] private Canvas canvas;
    private CanvasGroup canvasGroup;
    Vector2 pos;

    // Start is called before the first frame update
    void Awake()
    {

        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
    }

    private void Start()
    {
        pos = gameObject.GetComponent<RectTransform>().localPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        //Debug.Log("OnPointerDown");

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("OnBeginDrag");
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        //Debug.Log("OnEndDrag");
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag");
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        gameObject.GetComponent<RectTransform>().localPosition = pos;
    }
}
