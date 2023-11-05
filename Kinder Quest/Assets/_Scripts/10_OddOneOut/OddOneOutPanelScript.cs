using UnityEngine;
using UnityEngine.UI;

public class OddOneOutPanelScript : MonoBehaviour
{
    [Header("Singleton Reference")]
    public static OddOneOutPanelScript Instance;

    [Header("Sprite Reference")]
    [SerializeField] Sprite _apple;
    [SerializeField] Sprite _cat;

    [SerializeField] Transform _imageHolderTransform;
    public int oddObjIndex;
    private GameObject _nextPanel;

    private void Awake()
    {
        Instance = Instance == null ? this : Instance;  // Setting Singleton Instance
        if (Instance != this) Destroy(gameObject);  // If not Active Singleton, destroy it
        DontDestroyOnLoad(gameObject);  // Ensure that the Singleton persists across scene changes

        // Find and set references to various objects
        _imageHolderTransform = GameObject.Find("Oou_OddEvenImage_Grid").transform;
        _nextPanel = GameObject.Find("Oou_Final_Image");
    }

    private void Start()
    {
        // Load sprites from resources
        _apple = Resources.Load<Sprite>("Tracing_Images/Dragger/Cap_A");
        _cat = Resources.Load<Sprite>("Tracing_Images/Dragger/Cap_C");

        // Add a click event listener to a button in the next panel
        _nextPanel.transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(() => { OnNextBtnSelection(); });
    }

    public void OnOddOneOutInitiate()
    {
        // Set all images to display apples initially
        foreach (Transform t in _imageHolderTransform)
        {
            t.GetComponent<Image>().sprite = _apple;
        }

        // Generate a random index for the odd object
        oddObjIndex = Random.Range(0, 9); // Random number from 0 to 8 (inclusive)
        // Set the image at the odd object index to display a cat
        _imageHolderTransform.GetChild(oddObjIndex).GetComponent<Image>().sprite = _cat;

        // Deactivate the next panel
        _nextPanel.SetActive(false);
    }

    public void CheckCurrentObjective(string input)
    {
        // Generate the expected object name based on the odd object index
        string oddObjName = "Image_" + (oddObjIndex + 1);

        // Check if the input matches the expected object name
        if (input == oddObjName)
            // Activate the next panel
            _nextPanel.SetActive(true);
    }

    private void OnNextBtnSelection()
    {
        // Deactivate the next panel and reinitialize the odd one out activity
        _nextPanel.SetActive(false);
        OnOddOneOutInitiate();
    }
}
