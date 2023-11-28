using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BigSmallDropperScript : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        // Get the GameObject that was dropped.
        GameObject droppedObj = eventData.pointerDrag;
        float width = droppedObj.GetComponent<RectTransform>().rect.width;

        if (droppedObj != null)
        {
            string dropObjName = droppedObj.name;

            if (gameObject.name == "Bigger_Item_DropZone" && width == BigSmallPanelScript.Instance.BigHeightWidth)
            {
                // Reset the position of the dropped object using a method in BigSmallDraggerScript.
                droppedObj.GetComponent<BigSmallDraggerScript>().GmObjPosReset(droppedObj, true);

                // Update the text in the child TextMeshPro component.
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Done";

                // Set a flag in the BigSmallPanelScript to indicate the big image has been identified.
                BigSmallPanelScript.Instance.isBigImgIdentified = true;

                // Check the current objective in the BigSmallPanelScript.
                BigSmallPanelScript.Instance.CheckCurrentObjective();
            }
            else if (gameObject.name == "Smaller_Item_DropZone" && width == BigSmallPanelScript.Instance.SmallHeightWidth)
            {
                // Reset the position of the dropped object using a method in BigSmallDraggerScript.
                droppedObj.GetComponent<BigSmallDraggerScript>().GmObjPosReset(droppedObj, true);

                // Update the text in the child TextMeshPro component.
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Done";

                // Set a flag in the BigSmallPanelScript to indicate the small image has been identified.
                BigSmallPanelScript.Instance.isSmallImgIdentified = true;

                // Check the current objective in the BigSmallPanelScript.
                BigSmallPanelScript.Instance.CheckCurrentObjective();
            }
        }
    }
}
