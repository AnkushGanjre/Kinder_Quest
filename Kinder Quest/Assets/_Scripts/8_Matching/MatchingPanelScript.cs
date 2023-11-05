using UnityEngine;
using UnityEngine.UI;

public class MatchingPanelScript : MonoBehaviour
{
    [Header("Singleton Reference")]
    public static MatchingPanelScript Instance;

    [Header("Buttons")]
    [SerializeField] Button _mthLetterBtn;
    [SerializeField] Button _mthShapesBtn;
    [SerializeField] Button _mthAnimalsBtn;
    [SerializeField] Button _mthClocksBtn;

    [Header("Matching Parent Panel")]
    [SerializeField] Image[] _upperPanelImages;
    [SerializeField] Sprite[] _parentImages;

    [Header("Matching Child Images")]
    [SerializeField] Image[] _lowerPanelImages;
    [SerializeField] Sprite[] _childImages;

    [Header("Next panel Reference")]
    private GameObject _nextPanel;


    private void Awake()
    {
        Instance = Instance == null ? this : Instance;  // Setting Singleton Instance
        if (Instance != this) Destroy(gameObject);  // If not Active Singleton, destroy it
        DontDestroyOnLoad(gameObject);  // Ensure that the Singleton persists across scene changes

        // Find and initialize references to buttons and the next panel
        _mthLetterBtn = GameObject.Find("mth_Letters_Btn").GetComponent<Button>();
        _mthShapesBtn = GameObject.Find("mth_Shapes_Btn").GetComponent<Button>();
        _mthAnimalsBtn = GameObject.Find("mth_Animals_Btn").GetComponent<Button>();
        _mthClocksBtn = GameObject.Find("mth_Clocks_Btn").GetComponent<Button>();
        _nextPanel = GameObject.Find("Mat_Final_Image");
    }

    void Start()
    {
        // Set click event handlers for buttons
        _mthLetterBtn.onClick.AddListener(() => { OnMthLetterBtn(); });
        _mthShapesBtn.onClick.AddListener(() => { OnMthShapesBtn(); });
        _mthAnimalsBtn.onClick.AddListener(() => { OnMthAnimalsBtn(); });
        _mthClocksBtn.onClick.AddListener(() => { OnMthClocksBtn(); });

        // Add a click listener to the next button in the panel
        _nextPanel.transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(() => { OnNextBtnSelection(); });
    }


    #region Button Functions


    private void OnMthLetterBtn()
    {
        // Handle "Letters" button click
        HomeScreenMgrScript.Instance.DragDropCanvas.transform.GetChild(1).gameObject.SetActive(true);
        OnAssignComponents();
    }

    private void OnMthShapesBtn()
    {
        // Handle "Shapes" button click
        HomeScreenMgrScript.Instance.DragDropCanvas.transform.GetChild(1).gameObject.SetActive(true);
        OnAssignComponents();
    }

    private void OnMthAnimalsBtn()
    {
        // Handle "Animals" button click
        HomeScreenMgrScript.Instance.DragDropCanvas.transform.GetChild(1).gameObject.SetActive(true);
        OnAssignComponents();
    }

    private void OnMthClocksBtn()
    {
        // Handle "Clocks" button click
        HomeScreenMgrScript.Instance.DragDropCanvas.transform.GetChild(1).gameObject.SetActive(true);
        OnAssignComponents();
    }

    private void OnNextBtnSelection()
    {
        // Handle "Next" button click to proceed to the next panel
        _nextPanel.SetActive(false);
        OnAssignComponents();
    }

    #endregion


    #region Supportive Function

    private void OnAssignComponents()
    {
        // Assign random sprites to the upper and lower panel images
        ShuffleArray(_parentImages); // Shuffle the sprite array
        ShuffleArray(_childImages); // Shuffle the sprite array

        for (int i = 0; i < 4; i++)
        {
            _upperPanelImages[i].sprite = _parentImages[i]; // Assign sprites to Image components
            _lowerPanelImages[i].sprite = _childImages[i]; // Assign sprites to Image components

            _upperPanelImages[i].color = new Color32(255, 255, 255, 255);
            _lowerPanelImages[i].color = new Color32(255, 255, 255, 255);

            _upperPanelImages[i].gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
            _lowerPanelImages[i].gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }

        _nextPanel.SetActive(false);
    }

    private void ShuffleArray(Sprite[] array)
    {
        // Fisher-Yates shuffle algorithm to shuffle an array
        int n = array.Length;
        for (int i = 0; i < n; i++)
        {
            int r = i + Random.Range(0, n - i);
            Sprite temp = array[i];
            array[i] = array[r];
            array[r] = temp;
        }
    }

    public void CheckMatchingProgress()
    {
        // Check if all upper panel images have been matched (blocksRaycasts are set to false)
        for (int i = 0; i < _upperPanelImages.Length; i++)
        {
            if (_upperPanelImages[i].GetComponent<CanvasGroup>().blocksRaycasts == true)
            {
                return; // If any image is not matched, exit the function
            }
        }

        // If all upper panel images have been matched, activate the next panel
        _nextPanel.SetActive(true);
    }

    #endregion
    
}
