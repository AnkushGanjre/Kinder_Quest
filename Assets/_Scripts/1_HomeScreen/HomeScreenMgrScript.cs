using UnityEngine;
using UnityEngine.UI;

public class HomeScreenMgrScript : MonoBehaviour
{
    [Header("Singleton Reference")]
    public static HomeScreenMgrScript Instance;

    [Header("Script Reference")]
    public BaseDataSO BDSO;
    private LineCurvePanelScript _lineCurvePanelScript;
    private AlphabetPanelScript _alphabetPanelScript;
    private NumberPanelScript _numberPanelScript;

    [Header("UI Canvas References")]
    public GameObject HomescreenCanvas;
    public GameObject SettingsCanvas;
    public GameObject ActivityCanvas;
    public GameObject TracingCanvas;
    public GameObject DrawingBGCanvas;
    public GameObject FreeHandDrawCanvas;
    public GameObject DrawToolsCanvas;
    public GameObject PopTheBubbleCanvas;
    public GameObject DragDropCanvas;
    public GameObject AllButtonsCanvas;

    [Header("Scroll Bar Home screen")]
    //[SerializeField] private Scrollbar _svClouds;
    [SerializeField] private Scrollbar _svIcons;
    //[SerializeField] private Scrollbar _svBottom;

    [Header("Activity Icon Transform")]
    [SerializeField] private Transform _iconContent;

    [Header("Buttons")]
    [SerializeField] private Button _settingsBtn;
    [SerializeField] GameObject[] _backButton;
    [SerializeField] GameObject[] _doubleBackButton;


    private void Awake()
    {
        Instance = Instance ?? this;  // Setting Singleton Instance
        if (Instance != this) Destroy(gameObject);  // If not Active Singleton, destroy it
        DontDestroyOnLoad(gameObject);  // Ensure that the Singleton persists across scene changes

        FindingAwakeReference(); // Find and set references to various objects
    }

    private void Start()
    {
        // Setting function to Scroll's On value Change
        //_svIcons.onValueChanged.AddListener(value => OnScrollBarValueChange());

        // Setting All 3 scroll's value to 0 to start at left
        //_svClouds.value = _svIcons.value = _svBottom.value = 0;
        _svIcons.value = 0;

        BDSO = Resources.Load<BaseDataSO>("AllDataSO");

        AssignFunctionToButtons(); // Set click event handlers for buttons
        DisablingGameObjects(); // Disable all the game objects initially
    }

    private void FindingAwakeReference()
    {
        // Getting all the instances of all Buttons & Gameobjects
        _lineCurvePanelScript = GetComponent<LineCurvePanelScript>();
        _alphabetPanelScript = GetComponent<AlphabetPanelScript>();
        _numberPanelScript = GetComponent<NumberPanelScript>();

        // Find and set references to Homepage ScrollBar UI elements and objects
        //_svClouds = GameObject.Find("Scrollbar_Clouds").GetComponent<Scrollbar>();
        _svIcons = GameObject.Find("Scrollbar_Icons").GetComponent<Scrollbar>();
        //_svBottom = GameObject.Find("Scrollbar_Bottom").GetComponent<Scrollbar>();

        HomescreenCanvas = GameObject.Find("HomeScreen_Canvas");
        SettingsCanvas = GameObject.Find("Settings_Canvas");
        ActivityCanvas = GameObject.Find("Activity_Canvas");
        TracingCanvas = GameObject.Find("Tracing_Canvas");
        DrawingBGCanvas = GameObject.Find("Drawing_BG_Canvas");
        FreeHandDrawCanvas = GameObject.Find("FreeHandDraw_Canvas");
        DrawToolsCanvas = GameObject.Find("Draw_Tools_Canvas");
        PopTheBubbleCanvas = GameObject.Find("PopTheBubble_Canvas");
        DragDropCanvas = GameObject.Find("DragDrop_Canvas");
        AllButtonsCanvas = GameObject.Find("AllButton_Canvas");


        _iconContent = GameObject.Find("IconContent").transform;
        _settingsBtn = GameObject.Find("Settings_Btn").GetComponent<Button>();
        _backButton = GameObject.FindGameObjectsWithTag("BackButton");
        _doubleBackButton = GameObject.FindGameObjectsWithTag("DoubleBackButton");
    }

    private void AssignFunctionToButtons()
    {
        // Assign click event handlers to buttons
        _settingsBtn.onClick.AddListener(() => { OnSettingsBtn(); });
        _iconContent.GetChild(0).GetComponent<Button>().onClick.AddListener(() => { OnLineCurveBtn(); });
        _iconContent.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { OnAlphabetBtn(); });
        _iconContent.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { OnNumbersBtn(); });
        _iconContent.GetChild(3).GetComponent<Button>().onClick.AddListener(() => { OnWordsBtn(); });
        _iconContent.GetChild(4).GetComponent<Button>().onClick.AddListener(() => { OnDrawBtn(); });
        _iconContent.GetChild(5).GetComponent<Button>().onClick.AddListener(() => { OnMatchingBtn(); });
        _iconContent.GetChild(6).GetComponent<Button>().onClick.AddListener(() => { OnPopTheBubblenBtn(); });
        _iconContent.GetChild(7).GetComponent<Button>().onClick.AddListener(() => { OnOddOneOutBtn(); });
        _iconContent.GetChild(8).GetComponent<Button>().onClick.AddListener(() => { OnBigSmallBtn(); });
        _iconContent.GetChild(9).GetComponent<Button>().onClick.AddListener(() => { OnMagicCountBtn(); });

        // Assigning Back Button Function
        for (int i = 0; i < _backButton.Length; i++)
        {
            int a = i;
            _backButton[a].GetComponent<Button>().onClick.AddListener(delegate
            {
                _backButton[a].transform.parent.gameObject.SetActive(false);
            });
        }

        // Assigning Double Back Button Function
        for (int i = 0; i < _doubleBackButton.Length; i++)
        {
            int a = i;
            _doubleBackButton[a].GetComponent<Button>().onClick.AddListener(delegate
            {
                _doubleBackButton[a].transform.parent.parent.gameObject.SetActive(false);
            });
        }
    }

    private void DisablingGameObjects()
    {
        // Setting off all GameObjects of respective panel
        foreach (Transform t in SettingsCanvas.transform)
        {
            t.gameObject.SetActive(false);
        }
        foreach (Transform t in ActivityCanvas.transform)
        {
            t.gameObject.SetActive(false);
        }
        foreach (Transform t in DrawingBGCanvas.transform)
        {
            t.gameObject.SetActive(false);
        }
        foreach (Transform t in FreeHandDrawCanvas.transform)
        {
            t.gameObject.SetActive(false);
        }
        foreach (Transform t in DrawToolsCanvas.transform)
        {
            t.gameObject.SetActive(false);
        }
        foreach (Transform t in DragDropCanvas.transform)
        {
            t.gameObject.SetActive(false);
        }
        foreach (Transform t in AllButtonsCanvas.transform)
        {
            t.gameObject.SetActive(false);
        }
    }

    private void OnScrollBarValueChange()
    {
        // Setting Scrolling value of Background and Foreground Scroll image
        //_svClouds.value = _svIcons.value;
        //_svBottom.value = _svIcons.value;
    }

    private void OnSettingsBtn()
    {
        // Activating Setting Panel
        //foreach (Transform t in SettingsCanvas.transform)
        //{
        //    t.gameObject.SetActive(true);
        //}

        Debug.Log("Settings");
    }

    private void OnLineCurveBtn()
    {
        // Activating Line Curve Panel
        ActivityCanvas.transform.GetChild(0).gameObject.SetActive(true);
        _lineCurvePanelScript.OnPanelInitiate();
    }

    private void OnAlphabetBtn()
    {
        // Activating Alphabet Panel
        ActivityCanvas.transform.GetChild(1).gameObject.SetActive(true);
        _alphabetPanelScript.OnPanelInitiate();
    }

    private void OnNumbersBtn()
    {
        // Activating Number Panel
        ActivityCanvas.transform.GetChild(2).gameObject.SetActive(true);
        _numberPanelScript.OnPanelInitiate();
    }

    private void OnWordsBtn()
    {
        // Activating Words Panel
        ActivityCanvas.transform.GetChild(3).gameObject.SetActive(true);
        WordsPanelScript.Instance.OnWrdFoodsBtn();
    }

    private void OnDrawBtn()
    {
        // Activating Draw Panel
        //ActivityCanvas.transform.GetChild(4).gameObject.SetActive(true);

        AnimationScript.Instance.PlayAnimation();
        FreeHandDrawMgrScript.Instance.OnDrawing("Canvas");
    }

    private void OnMatchingBtn()
    {
        // Activating Matching Panel
        ActivityCanvas.transform.GetChild(5).gameObject.SetActive(true);
    }

    private void OnPopTheBubblenBtn()
    {
        // Activating Pop The Bubble Panel
        ActivityCanvas.transform.GetChild(6).gameObject.SetActive(true);
    }

    private void OnOddOneOutBtn()
    {
        // Activating Pop The Odd One Out Panel
        DragDropCanvas.transform.GetChild(2).gameObject.SetActive(true);
        OddOneOutPanelScript.Instance.OnOddOneOutInitiate();
    }

    private void OnBigSmallBtn()
    {
        // Activating Pop The Big Small Panel
        DragDropCanvas.transform.GetChild(3).gameObject.SetActive(true);
        BigSmallPanelScript.Instance.OnInitiateBigSmall();
    }

    private void OnMagicCountBtn()
    {
        // Activating Pop The Magic Count Panel
        AllButtonsCanvas.transform.GetChild(0).gameObject.SetActive(true);
        MagicCountPanelScript.Instance.OnMagicCountInitiate();
    }
}
