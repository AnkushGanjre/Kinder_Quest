using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LineCurvePanelScript : MonoBehaviour
{
    [Header("Tracing & Worksheet")]
    [SerializeField] private Button _tracingBtn;
    [SerializeField] private Button _worksheetBtn;

    [Header("Dot Line Curve")]
    [SerializeField] private Button _lineBtn;
    [SerializeField] private Button _curveBtn;
    [SerializeField] private Button _shapeBtn;

    [Header("Line Curve Panel Components")]
    [SerializeField] private Transform _lcuContentTransform;

    [Header("Boolean Flags")]
    private bool isTracingSelectionOn;

    [Header("LCS Image Array")]
    [SerializeField] private List<Sprite> _lineImgArr = new List<Sprite>();
    [SerializeField] private List<Sprite> _curveImgArr = new List< Sprite>();
    [SerializeField] private List<Sprite> _shapeImgArr = new List<Sprite>();

    private void Awake()
    {
        // Find and set references to UI elements and objects
        _tracingBtn = GameObject.Find("Lcu_Tracing_Btn").GetComponent<Button>();
        _worksheetBtn = GameObject.Find("Lcu_Worksheet_Btn").GetComponent<Button>();

        _lineBtn = GameObject.Find("Lcu_Line_Btn").GetComponent<Button>();
        _curveBtn = GameObject.Find("Lcu_Curve_Btn").GetComponent<Button>();
        _shapeBtn = GameObject.Find("Lcu_Shape_Btn").GetComponent<Button>();

        _lcuContentTransform = GameObject.Find("Lcu_Panel_Content").transform;
    }

    private void Start()
    {
        LoadingAssets();

        // Assign click event handlers to buttons
        _tracingBtn.onClick.AddListener(() => { OnLcuTracingBtn(); });
        _worksheetBtn.onClick.AddListener(() => { OnLCuWorksheetBtn(); });
        _lineBtn.onClick.AddListener(() => { OnLcuLineBtn(); });
        _curveBtn.onClick.AddListener(() => { OnLcuCurveBtn(); });
        _shapeBtn.onClick.AddListener(() => { OnLcuShapeBtn(); });

        AdjustChildSize();
    }

    private void AdjustChildSize()
    {
        float width = _lcuContentTransform.GetComponent<RectTransform>().rect.width;
        float height = _lcuContentTransform.GetComponent<RectTransform>().rect.height;
        width = Mathf.RoundToInt(width / 5.73f);
        height = Mathf.RoundToInt(height / 3.4f);
        _lcuContentTransform.GetComponent<GridLayoutGroup>().cellSize = new Vector2(width, height);
    }

    public void OnPanelInitiate()
    {
        // Initialize the panel with Tracing & Dots category
        OnLcuTracingBtn();
        OnLcuLineBtn();
    }

    #region Main Panel Button's Function

    public void OnLcuTracingBtn()
    {
        isTracingSelectionOn = true;

        // Set the Button color to indicate tracing
        Sprite yellowBtn = WordsPanelScript.Instance.YellowBtnSprite;
        foreach (Transform t in _lcuContentTransform)
        {
            t.GetComponent<Image>().sprite = yellowBtn;
        }
    }

    private void OnLCuWorksheetBtn()
    {
        isTracingSelectionOn = false;

        // Set the Button color to indicate Worksheet
        Sprite orangeBtn = WordsPanelScript.Instance.OrangeBtnSprite;
        foreach (Transform t in _lcuContentTransform)
        {
            t.GetComponent<Image>().sprite = orangeBtn;
        }
    }

    #endregion


    #region Sub Panel Button's Function

    public void OnLcuLineBtn()
    {
        // Change text for each item in the content to represent Line
        for (int i = 0; i < _lcuContentTransform.childCount; i++)
        {
            _lcuContentTransform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = _lineImgArr[i];
        }

        // Start a coroutine to assign event click functions to Line Curve buttons
        StartCoroutine(OnAssignSubBtnFunction());
    }

    private void OnLcuCurveBtn()
    {
        // Change text for each item in the content to represent lines
        for (int i = 0; i < _lcuContentTransform.childCount; i++)
        {
            _lcuContentTransform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = _curveImgArr[i];
        }

        // Start a coroutine to assign event click functions to Line Curve buttons
        StartCoroutine(OnAssignSubBtnFunction());
    }

    private void OnLcuShapeBtn()
    {
        // Change text for each item in the content to represent curves
        for (int i = 0; i < _lcuContentTransform.childCount; i++)
        {
            _lcuContentTransform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = _shapeImgArr[i];
        }

        // Start a coroutine to assign event click functions to Line Curve buttons
        StartCoroutine(OnAssignSubBtnFunction());
    }

    private void LoadingAssets()
    {
        // Change image for each item in the content
        for (int i = 0; i < _lcuContentTransform.childCount; i++)
        {
            Sprite sprite1 = Resources.Load<Sprite>("Line_Curve_Shape/Lcs_L" + (i + 1));
            _lineImgArr.Add(sprite1);

            Sprite sprite2 = Resources.Load<Sprite>("Line_Curve_Shape/Lcs_C" + (i + 1));
            _curveImgArr.Add(sprite2);

            Sprite sprite3 = Resources.Load<Sprite>("Line_Curve_Shape/Lcs_S" + (i + 1));
            _shapeImgArr.Add(sprite3);
        }
    }

    #endregion


    #region Supportive Functions

    private IEnumerator OnAssignSubBtnFunction()
    {
        yield return new WaitForEndOfFrame(); // Wait for the end of the frame.
        yield return new WaitForEndOfFrame(); // Wait for the end of the frame.
        yield return new WaitForEndOfFrame(); // Wait for the end of the frame.
        yield return new WaitForEndOfFrame(); // Wait for the end of the frame.
        // Loop through each subcategory button in the Line Curve panel
        foreach (Transform t in _lcuContentTransform)
        {
            // Remove any existing click listeners
            t.GetComponent<Button>().onClick.RemoveAllListeners();
            yield return new WaitForEndOfFrame(); // Wait for the end of the frame.

            // Getting button sprite name
            string imageName = t.GetChild(0).GetComponent<Image>().sprite.name;

            // Add a new click listener that triggers the OnWordsActivity method with the selected activity name
            t.GetComponent<Button>().onClick.AddListener(() => { OnLineCurveActivity(imageName); });
        }
    }

    private void OnLineCurveActivity(string input)
    {
        if (isTracingSelectionOn)
        {
            // checking if it is Tracing Activity
            TracingMgrScript.Instance.OnStartTracing(input);
        }
        else
        {
            // checking if it is Worksheet Activity
            FreeHandDrawMgrScript.Instance.OnStartWorksheet(input);
        }
    }

    #endregion

}
