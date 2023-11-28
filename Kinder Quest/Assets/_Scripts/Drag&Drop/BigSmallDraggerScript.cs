using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BigSmallDraggerScript : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [Header("Component's Refreences")]
    private RectTransform _rectTransform;
    private Transform _dragableObjHolder;
    private Transform _tempHolder;
    private CanvasGroup _canvasGroup;
    private Vector3 _originalPosition;
    private int _objSiblingIndex;

    [Header("Boolean Flags")]
    public bool IsDropSuccessful;

    private void Awake()
    {
        _tempHolder = GameObject.Find("BS_Temporary_Holder").transform;
    }

    private void Start()
    {
        // Find and assign references to the dragable object holder and temporary holder.
        _dragableObjHolder = BigSmallPanelScript.Instance.DraggerContent;

        // Initialize references and components.
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Disable raycast blocking for the dragged object.
        _canvasGroup.blocksRaycasts = false;

        // Get the current sibling index & Position of the dragged object.
        _objSiblingIndex = GetChildIndex(transform, transform.parent);
        _originalPosition = transform.position;

        // Change the parent of the dragged object to the temporary holder.
        transform.SetParent(_tempHolder.transform);
        IsDropSuccessful = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Update the anchored position of the RectTransform based on drag delta.
        _rectTransform.anchoredPosition += eventData.delta / HomeScreenMgrScript.Instance.DragDropCanvas.GetComponent<Canvas>().scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Reset the position of the object using the GmObjPosReset method if it was not successfully dropped.
        GmObjPosReset(gameObject, false);

        // Enable raycast blocking for the dragged object if it was not successfully dropped.
        if (!IsDropSuccessful)
            _canvasGroup.blocksRaycasts = true;
    }

    public void GmObjPosReset(GameObject gmObj, bool isDropped)
    {
        // Reset the position of the game object and enable horizontal layout group.
        gmObj.transform.SetParent(_dragableObjHolder);
        transform.position = _originalPosition;

        // Reset the sibling index of the dragged object.
        gameObject.transform.SetSiblingIndex(_objSiblingIndex);

        // If the object was successfully dropped, update its color and set the IsDropSuccessful flag.
        if (isDropped)
        {
            IsDropSuccessful = isDropped;
            gmObj.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
        }
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
