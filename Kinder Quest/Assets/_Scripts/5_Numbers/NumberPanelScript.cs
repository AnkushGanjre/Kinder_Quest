using UnityEngine;
using UnityEngine.UI;

public class NumberPanelScript : MonoBehaviour
{
    [Header("Tracing & Worksheet")]
    [SerializeField] private Button _tracingBtn;
    [SerializeField] private Button _worksheetBtn;

    [Header("Alphabet Panel Components")]
    [SerializeField] private Transform _numContentTransform;

    [Header("Boolean Flags")]
    private bool isTracingSelectionOn;

    private void Awake()
    {
        // Find and link the tracing and worksheet buttons in the Unity Editor
        _tracingBtn = GameObject.Find("Num_Tracing_Btn").GetComponent<Button>();
        _worksheetBtn = GameObject.Find("Num_Worksheet_Btn").GetComponent<Button>();

        // Find and link the number panel content transform in the Unity Editor
        _numContentTransform = GameObject.Find("Num_Panel_Content").transform;
    }

    private void Start()
    {
        // Set click event handlers for buttons
        _tracingBtn.onClick.AddListener(() => { OnNumTracingBtn(); });
        _worksheetBtn.onClick.AddListener(() => { OnNumWorksheetBtn(); });

        // Assigning Function to all Alphabets
        foreach (Transform t in _numContentTransform)
        {
            t.GetComponent<Button>().onClick.AddListener(() => { OnNumberSelected(t.name); });
        }

        AdjustChildSize();
    }

    private void AdjustChildSize()
    {
        float width = _numContentTransform.GetComponent<RectTransform>().rect.width;
        float height = _numContentTransform.GetComponent<RectTransform>().rect.height;
        width = Mathf.RoundToInt(width / 8.6f);
        height = Mathf.RoundToInt(height / 5.2f);
        _numContentTransform.GetComponent<GridLayoutGroup>().cellSize = new Vector2(width, height);
    }
    public void OnPanelInitiate()
    {
        // Initialize the panel with Tracing category
        OnNumTracingBtn();
    }

    #region Main Panel Button's Function

    public void OnNumTracingBtn()
    {
        isTracingSelectionOn = true;


        // Set the Button color to indicate tracing
        Sprite yellowBtn = WordsPanelScript.Instance.YellowBtnSprite;
        foreach (Transform t in _numContentTransform)
        {
            t.GetComponent<Image>().sprite = yellowBtn;
        }
    }

    private void OnNumWorksheetBtn()
    {
        isTracingSelectionOn = false;

        // Set the Button color to indicate Worksheet
        Sprite orangeBtn = WordsPanelScript.Instance.OrangeBtnSprite;
        foreach (Transform t in _numContentTransform)
        {
            t.GetComponent<Image>().sprite = orangeBtn;
        }
    }

    #endregion


    #region Supportive Functions

    private void OnNumberSelected(string Number)
    {
        if (isTracingSelectionOn)
        {
            // checking if it is Tracing Activity
            TracingMgrScript.Instance.OnStartTracing("Num_" + Number);
        }
        else
        {
            // checking if it is Worksheet Activity
            FreeHandDrawMgrScript.Instance.OnStartWorksheet("Num_" + Number);
        }
    }

    #endregion

}
