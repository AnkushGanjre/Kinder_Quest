using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SVImageControl : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    [Header("Components")]
    [SerializeField] Image pickerImage;
    [SerializeField] RawImage SVimage;
    private ColourPickerControl CC;
    [SerializeField] RectTransform rectTransform, pickerTransform;

    private void Awake()
    {
        // Getting all the instances of all components
        SVimage = GetComponent<RawImage>();
        CC = FindObjectOfType<ColourPickerControl>();
        rectTransform = GetComponent<RectTransform>();

        // Get the RectTransform of the color picker image and position it
        pickerTransform = pickerImage.GetComponent<RectTransform>();
        pickerTransform.position = new Vector2(-(rectTransform.sizeDelta.x * 0.5f), -(rectTransform.sizeDelta.y * 0.5f));
    }

    private void Start()
    {
        // Reset the local position of the color picker image
        pickerTransform.localPosition = Vector3.zero;
    }

    private void UpdateColor(PointerEventData eventData)
    {
        Vector2 localPos;
        // Convert the screen point to local point within this RectTransform
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPos))
        {
            // Calculate the half-size of the RectTransform in both dimensions
            float deltaX = rectTransform.sizeDelta.x * 0.5f;
            float deltaY = rectTransform.sizeDelta.y * 0.5f;

            // Clamp the local position to stay within the bounds of the RectTransform
            localPos.x = Mathf.Clamp(localPos.x, -deltaX, deltaX);
            localPos.y = Mathf.Clamp(localPos.y, -deltaY, deltaY);

            // Calculate normalized coordinates for the color picker
            float xNorm = (localPos.x + deltaX) / rectTransform.sizeDelta.x;
            float yNorm = (localPos.y + deltaY) / rectTransform.sizeDelta.y;

            // Set the position of the color picker and update its color based on the normalized values
            pickerTransform.localPosition = localPos;
            pickerImage.color = Color.HSVToRGB(0, 0, 1 - yNorm);

            // Call a method in the ColorPickerControl script to update the hue and value
            CC.SetSV(xNorm, yNorm);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Call the color update function when the user drags the color picker
        UpdateColor(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Call the color update function when the user clicks on the color picker
        UpdateColor(eventData);
    }

}
