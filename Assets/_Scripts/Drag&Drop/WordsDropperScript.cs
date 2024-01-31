using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WordsDropperScript : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        // Get the GameObject that was dropped.
        GameObject droppedObj = eventData.pointerDrag;

        if (droppedObj != null)
        {
            // Get the name of the image sprite of the dropped object.
            string dropObjName = droppedObj.GetComponent<Image>().sprite.name;

            // Get the name of the image sprite of the receiving object.
            string receingObjName = GetComponent<Image>().sprite.name;

            // Check if the dropped object matches the receiving object.
            if (dropObjName == receingObjName)
            {
                // Set the color of the receiving object's image to white, making it invisible.
                GetComponent<Image>().color = new Color32(255, 255, 255, 255);

                // Set the color of the dropped object's image to transparent, making it invisible.
                droppedObj.GetComponent<Image>().color = new Color32(255, 255, 255, 0);

                // Start a coroutine to block raycasts for the dropped object temporarily.
                StartCoroutine(OnWaitToBlockRaycast(droppedObj));
            }
        }
    }

    // Coroutine to temporarily block raycasts for the dropped object.
    private IEnumerator OnWaitToBlockRaycast(GameObject go)
    {
        yield return new WaitForEndOfFrame();

        // Block raycasts for the CanvasGroup of the dropped object.
        go.GetComponent<CanvasGroup>().blocksRaycasts = false;

        // Check the matching progress in the WordsPanelScript.
        WordsPanelScript.Instance.CheckMatchingProgress();
    }
}
