using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BubblePrefabScript : MonoBehaviour, IPointerClickHandler
{
    [Header("Prefab Components")]
    private Rigidbody2D rb;
    private RectTransform _rectTransf;

    [Header("Prefab Properties")]
    private float padding;
    private float canvasHeight;

    private void Start()
    {
        // Get a reference to the Rigidbody2D component and the RectTransform.
        rb = GetComponent<Rigidbody2D>();
        _rectTransf = GetComponent<RectTransform>();

        // Get the padding and canvas height from the PopTheBubblePanelScript.
        padding = PopTheBubblePanelScript.Instance.Padding;
        canvasHeight = PopTheBubblePanelScript.Instance.CanvasRect.rect.height;

        // Apply an upward force when the bubble is spawned.
        float upwardForce = PopTheBubblePanelScript.Instance.UpwardForce;
        rb.AddForce(Vector2.up * upwardForce, ForceMode2D.Impulse);
    }

    private void Update()
    {
        // Check if the image is above the screen and destroy it if it is.
        if (_rectTransf.position.y >= canvasHeight + padding)
            Destroy(gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Cast a ray from the click position to detect collisions.
        RaycastHit2D hit = Physics2D.Raycast(eventData.position, Vector2.zero);

        if (hit.collider != null)
        {
            // Extract the text message from the clicked bubble.
            string msg = hit.collider.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;

            // Call the OnBubbleClicked method in the PopTheBubblePanelScript and destroy the clicked bubble.
            PopTheBubblePanelScript.Instance.OnBubbleClicked(msg);
            Destroy(hit.collider.gameObject);
        }
    }
}
