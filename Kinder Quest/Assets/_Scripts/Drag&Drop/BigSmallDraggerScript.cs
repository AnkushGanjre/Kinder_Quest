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

    [Header("Boolean Flags")]
    public bool IsDropSuccessful;

    private void Awake()
    {
        // Find and assign references to the dragable object holder and temporary holder.
        _dragableObjHolder = GameObject.Find("Item_Dragger_Zone").transform;
        _tempHolder = GameObject.Find("BS_Temporary_Holder").transform;
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

        // Disable horizontal layout group to prevent layout reordering during drag.
        _dragableObjHolder.GetComponent<HorizontalLayoutGroup>().enabled = false;

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
        _dragableObjHolder.GetComponent<HorizontalLayoutGroup>().enabled = true;

        // Determine the sibling index based on the object's name.
        if (gmObj.name == "BS_Small_Image")
        {
            gmObj.transform.SetSiblingIndex(BigSmallPanelScript.Instance.SmallObjSiblingNum);
        }
        else
        {
            gmObj.transform.SetSiblingIndex(BigSmallPanelScript.Instance.BigObjSiblingNum);
        }

        // If the object was successfully dropped, update its color and set the IsDropSuccessful flag.
        if (isDropped)
        {
            IsDropSuccessful = isDropped;
            gmObj.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
        }
    }
}
