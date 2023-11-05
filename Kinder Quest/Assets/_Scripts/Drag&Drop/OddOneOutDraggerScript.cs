using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OddOneOutDraggerScript : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [Header("Componenets Refrences")]
    private RectTransform _rectTransform;
    private Transform _dragableObjHolder;
    private Transform _tempHolder;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        // Find and assign references to the dragable object holder and temporary holder.
        _dragableObjHolder = GameObject.Find("Oou_OddEvenImage_Grid").transform;
        _tempHolder = GameObject.Find("Oou_Temporary_Holder").transform;
    }

    private void Start()
    {
        // Initialize references and components.
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        // Disable raycast blocking for the dragged object.
        _canvasGroup.blocksRaycasts = false;

        // Disable GridLayoutGroup to prevent layout reordering during drag.
        _dragableObjHolder.GetComponent<GridLayoutGroup>().enabled = false;

        // Change the parent of the dragged object to the temporary holder.
        transform.SetParent(_tempHolder.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Update the anchored position of the RectTransform based on drag delta.
        _rectTransform.anchoredPosition += eventData.delta / HomeScreenMgrScript.Instance.DragDropCanvas.GetComponent<Canvas>().scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Enable raycast blocking for the dragged object.
        _canvasGroup.blocksRaycasts = true;

        // Set the parent of the object back to the original dragable object holder.
        gameObject.transform.SetParent(_dragableObjHolder);

        // Re-enable GridLayoutGroup to restore layout functionality.
        _dragableObjHolder.GetComponent<GridLayoutGroup>().enabled = true;

        // Retrieve the index of the current object from its name and adjust it for array indexing.
        int currentObjIndex = int.Parse(gameObject.name.Substring(6));
        gameObject.transform.SetSiblingIndex(currentObjIndex - 1);
    }
}
