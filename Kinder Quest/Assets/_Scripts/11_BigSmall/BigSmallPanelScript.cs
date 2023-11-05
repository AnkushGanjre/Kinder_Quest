using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BigSmallPanelScript : MonoBehaviour
{
    [Header("Singleton Reference")]
    public static BigSmallPanelScript Instance;

    [Header("Big & Small Dropping Image")]
    public Transform _bigImageDropZone;
    public Transform _smallImageDropZone;

    [Header("Big & Small Dragging Image")]
    public Transform _bigImage;
    public Transform _smallImage;

    [Header("Big & Small sibling index Num")]
    public int SmallObjSiblingNum = 0;
    public int BigObjSiblingNum = 0;

    [Header("User Selection Bools")]
    public bool isBigImgIdentified;
    public bool isSmallImgIdentified;

    [Header("next panel GameObject")]
    private GameObject _nextPanel;


    private void Awake()
    {
        Instance = Instance == null ? this : Instance;  // Setting Singleton Instance
        if (Instance != this) Destroy(gameObject);  // If not Active Singleton, destroy it
        DontDestroyOnLoad(gameObject);  // Ensure that the Singleton persists across scene changes

        // Find and assign the transforms for drop zones and images
        _bigImageDropZone = GameObject.Find("Bigger_Item_DropZone").transform;
        _smallImageDropZone = GameObject.Find("Smaller_Item_DropZone").transform;

        _bigImage = GameObject.Find("BS_Big_Image").transform;
        _smallImage = GameObject.Find("BS_Small_Image").transform;
        _nextPanel = GameObject.Find("BS_Final_Image");
    }

    private void Start()
    {
        // Add a click event listener to a button in the next panel
        _nextPanel.transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(() => { OnNextBtnSelection(); });
    }

    public void OnInitiateBigSmall()
    {
        // Reset the identification flags
        isSmallImgIdentified = isBigImgIdentified = false;

        // Assign a random value to small and big objects
        int randomValue = Random.Range(0, 2);
        SmallObjSiblingNum = randomValue;
        BigObjSiblingNum = 1 - randomValue;

        // Set the sibling indexes of small and big objects accordingly
        _smallImage.SetSiblingIndex(SmallObjSiblingNum);
        _bigImage.SetSiblingIndex(BigObjSiblingNum);

        // Deactivate the next panel
        _nextPanel.SetActive(false);
    }

    public void CheckCurrentObjective()
    {
        if (isSmallImgIdentified && isBigImgIdentified)
            // Activate the next panel if both images are identified
            _nextPanel.SetActive(true);
    }

    private void OnNextBtnSelection()
    {
        Color defaulColor = new Color32(255, 255, 255, 255);
        // Reset the image colors and enable raycasting
        _bigImage.GetComponent<Image>().color = _smallImage.GetComponent<Image>().color = defaulColor;
        _bigImage.GetComponent<CanvasGroup>().blocksRaycasts = _smallImage.GetComponent<CanvasGroup>().blocksRaycasts = true;

        // Set the text of drop zones to "Big" and "Small"
        _bigImageDropZone.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Big";
        _smallImageDropZone.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Small";

        // Reinitialize the big and small objects
        OnInitiateBigSmall();
    }
}
