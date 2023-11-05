using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LineCurvePanelScript : MonoBehaviour
{
    [Header("Tracing & Worksheet")]
    [SerializeField] private Button _tracingBtn;
    [SerializeField] private Button _worksheetBtn;

    [Header("Dot Line Curve")]
    [SerializeField] private Button _dotBtn;
    [SerializeField] private Button _lineBtn;
    [SerializeField] private Button _curveBtn;

    [Header("Line Curve Panel Components")]
    [SerializeField] private Transform _lcuContentTransform;

    [Header("Boolean Flags")]
    private bool isTracingSelectionOn;

    private void Awake()
    {
        // Find and set references to UI elements and objects
        _tracingBtn = GameObject.Find("Lcu_Tracing_Btn").GetComponent<Button>();
        _worksheetBtn = GameObject.Find("Lcu_Worksheet_Btn").GetComponent<Button>();

        _dotBtn = GameObject.Find("Lcu_Dots_Btn").GetComponent<Button>();
        _lineBtn = GameObject.Find("Lcu_Line_Btn").GetComponent<Button>();
        _curveBtn = GameObject.Find("Lcu_Curve_Btn").GetComponent<Button>();

        _lcuContentTransform = GameObject.Find("Lcu_Panel_Content").transform;
    }

    private void Start()
    {
        // Assign click event handlers to buttons
        _tracingBtn.onClick.AddListener(() => { OnLcuTracingBtn(); });
        _worksheetBtn.onClick.AddListener(() => { OnLCuWorksheetBtn(); });
        _dotBtn.onClick.AddListener(() => { OnLcuDotBtn(); });
        _lineBtn.onClick.AddListener(() => { OnLcuLineBtn(); });
        _curveBtn.onClick.AddListener(() => { OnLcuCurveBtn(); });
    }

    public void OnPanelInitiate()
    {
        // Initialize the panel with Tracing & Dots category
        OnLcuTracingBtn();
        OnLcuDotBtn();
    }

    #region Main Panel Button's Function

    public void OnLcuTracingBtn()
    {
        isTracingSelectionOn = true;

        
        // Set the Button color to indicate tracing
        Sprite yellowBtn = TracingMgrScript.Instance.YellowBtnSprite;
        foreach (Transform t in _lcuContentTransform)
        {
            t.GetComponent<Image>().sprite = yellowBtn;
        }
    }

    private void OnLCuWorksheetBtn()
    {
        isTracingSelectionOn = false;



        // Set the Button color to indicate Worksheet
        Sprite orangeBtn = TracingMgrScript.Instance.OrangeBtnSprite;
        foreach (Transform t in _lcuContentTransform)
        {
            t.GetComponent<Image>().sprite = orangeBtn;
        }
    }

    #endregion


    #region Sub Panel Button's Function

    public void OnLcuDotBtn()
    {
        // Change text for each item in the content to represent dots
        for (int i = 0; i < _lcuContentTransform.childCount; i++)
        {
            _lcuContentTransform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Shape " + (i + 1);
        }

        // Start a coroutine to assign event click functions to Line Curve buttons
        StartCoroutine(OnAssignSubBtnFunction());
    }

    private void OnLcuLineBtn()
    {
        // Change text for each item in the content to represent lines
        for (int i = 0; i < _lcuContentTransform.childCount; i++)
        {
            _lcuContentTransform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Line " + (i + 1);
        }

        // Start a coroutine to assign event click functions to Line Curve buttons
        StartCoroutine(OnAssignSubBtnFunction());
    }

    private void OnLcuCurveBtn()
    {
        // Change text for each item in the content to represent curves
        for (int i = 0; i < _lcuContentTransform.childCount; i++)
        {
            _lcuContentTransform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Curve " + (i + 1);
        }

        // Start a coroutine to assign event click functions to Line Curve buttons
        StartCoroutine(OnAssignSubBtnFunction());
    }

    #endregion


    #region Supportive Functions

    private IEnumerator OnAssignSubBtnFunction()
    {
        // Loop through each subcategory button in the Line Curve panel
        foreach (Transform t in _lcuContentTransform)
        {
            // Remove any existing click listeners
            t.GetComponent<Button>().onClick.RemoveAllListeners();
            yield return new WaitForEndOfFrame(); // Wait for the end of the frame.

            // Get the activity name from the text of the subcategory button
            string activity = t.GetChild(0).GetComponent<TextMeshProUGUI>().text;

            // Add a new click listener that triggers the OnWordsActivity method with the selected activity name
            t.GetComponent<Button>().onClick.AddListener(() => { OnLineCurveActivity(activity); });
        }
    }

    private void OnLineCurveActivity(string input)
    {
        Debug.Log(isTracingSelectionOn ? "Tracing " + input : "Worksheet " + input);
    }

    #endregion

}
