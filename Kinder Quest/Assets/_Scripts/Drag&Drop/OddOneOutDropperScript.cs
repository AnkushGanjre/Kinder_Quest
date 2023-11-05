using UnityEngine;
using UnityEngine.EventSystems;

public class OddOneOutDropperScript : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        // Get the GameObject that was dropped.
        GameObject droppedObj = eventData.pointerDrag;

        if (droppedObj != null)
        {
            // Get the name of the dropped object.
            string dropObjName = droppedObj.name;

            // Call a method to check the current objective in the OddOneOutPanelScript.
            OddOneOutPanelScript.Instance.CheckCurrentObjective(dropObjName);
        }
    }
}
