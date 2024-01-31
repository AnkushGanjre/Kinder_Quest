using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WordsDraggerScript : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [Header("References")]
    private RectTransform _rectTransform;
    private Transform _dragableObjHolder;
    private Transform _tempHolder;
    private CanvasGroup _canvasGroup;
    private int _objSiblingIndex;

    private void Awake()
    {
        // Find and assign references to the dragable object holder and temporary holder.
        _dragableObjHolder = GameObject.Find("Wor_Jumbled_Letter").transform;
        _tempHolder = GameObject.Find("Wor_Temporary_Holder").transform;
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

        // Get the current sibling index of the dragged object.
        _objSiblingIndex = GetChildIndex(transform, transform.parent);

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

        // Reset the sibling index of the dragged object.
        gameObject.transform.SetSiblingIndex(_objSiblingIndex);
    }

    private int GetChildIndex(Transform child, Transform parent)
    {
        // Utility function to find the index of a child within a parent's children.
        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i) == child)
                return i;
        }

        return -1;
    }
}
