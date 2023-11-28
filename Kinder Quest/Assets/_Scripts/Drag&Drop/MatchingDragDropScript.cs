using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MatchingDragDropScript : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    [Header("References")]
    private RectTransform _rectTransform;
    private Transform _upperPanelObjHolder;
    private Transform _lowerPanelObjHolder;
    private Transform _tempHolder;
    private CanvasGroup _canvasGroup;

    [Header("Int & Bool Variable")]
    private int _objSiblingIndex;
    private bool _isObjFrmUpperPanel;

    private void Awake()
    {
        // Find and assign references to the upper panel, lower panel, and temporary holder.
        _upperPanelObjHolder = GameObject.Find("Mat_Upper_Part").transform;
        _lowerPanelObjHolder = GameObject.Find("Mat_Lower_Part").transform;
        _tempHolder = GameObject.Find("Mat_Temporary_Holder").transform;
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

        // Disable horizontal layout groups to prevent layout reordering during drag.
        _upperPanelObjHolder.GetComponent<HorizontalLayoutGroup>().enabled = false;
        _lowerPanelObjHolder.GetComponent<HorizontalLayoutGroup>().enabled = false;

        // Check if the object is from the upper panel.
        _isObjFrmUpperPanel = (transform.parent == _upperPanelObjHolder) ? true : false;

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

        // Set the parent of the object back to its original panel.
        gameObject.transform.SetParent(_isObjFrmUpperPanel ? _upperPanelObjHolder : _lowerPanelObjHolder);

        // Reset the sibling index of the dragged object.
        gameObject.transform.SetSiblingIndex(_objSiblingIndex);

        // Re-enable horizontal layout groups to restore layout functionality.
        _upperPanelObjHolder.GetComponent<HorizontalLayoutGroup>().enabled = true;
        _lowerPanelObjHolder.GetComponent<HorizontalLayoutGroup>().enabled = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObj = eventData.pointerDrag;

        if (droppedObj != null)
        {
            StartCoroutine(OnDropCheck(droppedObj, gameObject));
        }

        
    }

    private IEnumerator OnDropCheck(GameObject droppedGO, GameObject receiveGO)
    {
        yield return new WaitForEndOfFrame();

        if (droppedGO.transform.parent != receiveGO.transform.parent)
        {
            int droppedObjIndex = GetChildIndex(droppedGO.transform, droppedGO.transform.parent);
            int receiveObjIndex = GetChildIndex(receiveGO.transform, receiveGO.transform.parent);

            string droppedObjName;
            string receingObjName;

            if (transform.parent == MatchingPanelScript.Instance.UpperPanel)
            {
                droppedObjName = MatchingPanelScript.Instance.LowerMatchList[droppedObjIndex];
                receingObjName = MatchingPanelScript.Instance.UpperMatchList[receiveObjIndex];
            }
            else
            {
                droppedObjName = MatchingPanelScript.Instance.UpperMatchList[droppedObjIndex];
                receingObjName = MatchingPanelScript.Instance.LowerMatchList[receiveObjIndex];
            }

            // Check if the names match, and if they do, disable the objects.
            if (droppedObjName == receingObjName)
            {
                StartCoroutine(OnDisablingObject(droppedGO, receiveGO));
            }
        }
    }

    private IEnumerator OnDisablingObject(GameObject go1, GameObject go2)
    {
        yield return new WaitForEndOfFrame();

        // Disable raycast blocking for both objects.
        go1.GetComponent<CanvasGroup>().blocksRaycasts = false;
        go2.GetComponent<CanvasGroup>().blocksRaycasts = false;

        // Change the colors of both objects to indicate their matching.
        go1.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
        go2.GetComponent<Image>().color = new Color32(255, 255, 255, 100);

        // Check the matching progress in the MatchingPanelScript.
        MatchingPanelScript.Instance.CheckMatchingProgress();
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
