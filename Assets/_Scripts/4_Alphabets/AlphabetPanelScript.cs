using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlphabetPanelScript : MonoBehaviour
{
    [Header("Tracing & Worksheet")]
    [SerializeField] private Button _tracingBtn;
    [SerializeField] private Button _worksheetBtn;

    [Header("Capital & Small Letter Button")]
    [SerializeField] private Button _capitalLtrBtn;
    [SerializeField] private Button _smallLtrBtn;

    [Header("Activity panel's Alphabet Component")]
    [SerializeField] private Transform _alpContentTransform;

    [Header("Categorizing Bools")]
    private bool isTracingSelectionOn; // Indicates if tracing is selected
    private bool isCapitalLtrSelected; // Indicates if capital letters are selected


    private void Awake()
    {
        // Find and set references to UI elements and objects
        _tracingBtn = GameObject.Find("Alp_Tracing_Btn").GetComponent<Button>();
        _worksheetBtn = GameObject.Find("Alp_Worksheet_Btn").GetComponent<Button>();

        _capitalLtrBtn = GameObject.Find("Alp_Capital_Button").GetComponent<Button>();
        _smallLtrBtn = GameObject.Find("Alp_Small_Button").GetComponent<Button>();

        _alpContentTransform = GameObject.Find("Alp_Panel_Content").transform;
    }

    private void Start()
    {
        // Assign click event handlers to buttons
        _tracingBtn.onClick.AddListener(() => { OnAlpTracingBtn(); });
        _worksheetBtn.onClick.AddListener(() => { OnAlpWorksheetBtn(); });
        _capitalLtrBtn.onClick.AddListener(() => { OnAlpCapitalLtrBtn(); });
        _smallLtrBtn.onClick.AddListener(() => { OnAlpSmallLtrBtn(); });

        // Assign click event handlers to alphabet letters
        foreach (Transform t in _alpContentTransform)
        {
            t.GetComponent<Button>().onClick.AddListener(() => { OnLetterSelected(t.name); });
        }

        AdjustChildSize();
    }

    private void AdjustChildSize()
    {
        float width = _alpContentTransform.GetComponent<RectTransform>().rect.width;
        float height = _alpContentTransform.GetComponent<RectTransform>().rect.height;
        width = Mathf.RoundToInt(width / 8.6f);
        height = Mathf.RoundToInt(height / 5.2f);
        _alpContentTransform.GetComponent<GridLayoutGroup>().cellSize = new Vector2(width, height);
    }

    public void OnPanelInitiate()
    {
        // Initialize the panel with Tracing & Capital letter category
        OnAlpTracingBtn();
        OnAlpCapitalLtrBtn();
    }

    #region Main Panel Button's Function

    public void OnAlpTracingBtn()
    {
        isTracingSelectionOn = true;

        // Set the Button color to indicate tracing
        Sprite yellowBtn = WordsPanelScript.Instance.YellowBtnSprite;
        foreach (Transform t in _alpContentTransform)
        {
            t.GetComponent<Image>().sprite = yellowBtn;
        }
    }

    private void OnAlpWorksheetBtn()
    {
        isTracingSelectionOn = false;
        
        // Set the Button color to indicate Worksheet
        Sprite orangeBtn = WordsPanelScript.Instance.OrangeBtnSprite;
        foreach (Transform t in _alpContentTransform)
        {
            t.GetComponent<Image>().sprite = orangeBtn;
        }
    }

    public void OnAlpCapitalLtrBtn()
    {
        isCapitalLtrSelected = true;

        // Make all alphabet letters uppercase
        if (_capitalLtrBtn.interactable)
        {
            _capitalLtrBtn.interactable = false;
            _smallLtrBtn.interactable= true;
            for (int i =0; i < _alpContentTransform.childCount; i++)
            {
                _alpContentTransform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.UpperCase;
            }
        }
    }

    private void OnAlpSmallLtrBtn()
    {
        isCapitalLtrSelected = false;

        // Make all alphabet letters lowercase
        if (_smallLtrBtn.interactable)
        {
            _smallLtrBtn.interactable = false;
            _capitalLtrBtn.interactable = true;
            for (int i = 0; i < _alpContentTransform.childCount; i++)
            {
                _alpContentTransform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.LowerCase;
            }
        }
    }

    #endregion


    #region Supportive Function

    private void OnLetterSelected(string letter)
    {
        if (isTracingSelectionOn)
        {
            // checking if it is Tracing Activity

            string newLetter;
            if (isCapitalLtrSelected)
            {
                // checking if it is Tracing's Capital letter Activity
                newLetter = "Cap_" + letter;
            }
            else
            {
                // checking if it is Tracing's Small letter Activity
                newLetter = "Sma_" + letter;
            }
            TracingMgrScript.Instance.OnStartTracing(newLetter);
        }
        else
        {
            // checking if it is Worksheet Activity

            string newLetter;
            if (isCapitalLtrSelected)
            {
                // checking if it is Worksheet's Capital letter Activity
                newLetter = "Cap_" + letter;
            }
            else
            {
                // checking if it is Worksheet's Small letter Activity
                newLetter = "Sma_" + letter;
            }
            FreeHandDrawMgrScript.Instance.OnStartWorksheet(newLetter);
        }
    }

    #endregion
}
