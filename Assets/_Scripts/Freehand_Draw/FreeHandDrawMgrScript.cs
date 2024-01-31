using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FreeHandDrawMgrScript : MonoBehaviour
{
    [Header("Singleton Reference")]
    public static FreeHandDrawMgrScript Instance;

    [Header("Camera & Script Reference")]
    private LineScript _lineScript;
    private Camera _cam;

    [Header("Drawing Components")]
    [SerializeField] GameObject _prefab;
    [SerializeField] LineScript _linePrefab;
    [SerializeField] GraphicRaycaster _freehandDrawRaycaster;
    [SerializeField] Button _freeHandDrawBackBtn;

    [Header("Line Properties")]
    public float ThresholdDistance = 0.1f;
    [Range(0.05f, 2)]
    public float LineWidth = 0.1f;
    public Color LineColor;
    public int LineIndex = 0;

    [Header("Transforms")]
    public Transform LineParent;
    private Transform _colorPalatteContent;

    [Header("Boolean flags")]
    private bool isDrawingAllowed;
    public bool isOtherPanelOpen;
    private bool isInsideDrawable;



    private void Awake()
    {
        Instance = Instance ?? this;  // Setting Singleton Instance
        if (Instance != this) Destroy(gameObject);  // If not Active Singleton, destroy it
        DontDestroyOnLoad(gameObject);  // Ensure that the Singleton persists across scene changes

        // Getting all the instances of all Buttons & other Components
        LineParent = GameObject.Find("Line_Parent").transform;
        _colorPalatteContent = GameObject.Find("Color_Palatte_Content").transform;

        _freeHandDrawBackBtn = GameObject.Find("FreehandDraw_BackButton").GetComponent<Button>();
        _freehandDrawRaycaster = GameObject.Find("FreeHandDraw_Canvas").GetComponent<GraphicRaycaster>();
    }

    private void Start()
    {
        // Get the main camera and set LineIndex
        _cam = Camera.main;
        LineIndex = HomeScreenMgrScript.Instance.FreeHandDrawCanvas.GetComponent<Canvas>().sortingOrder;

        // Set click event handlers for buttons
        _freeHandDrawBackBtn.onClick.AddListener(() => { OnFreeHandDrawBackBtn(); });

        // Load LinePrefab from Assets
        // Assigning _prefab to LineScript
        _prefab = Resources.Load<GameObject>("Prefabs/Line_Prefab");
        _linePrefab = _prefab.GetComponent<LineScript>();

        // Assigning color selection function to all the buttons
        OnColorPalatteBtn();
    }

    private void Update()
    {
        Vector2 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);

        // Check if the mouse is over the freehandDrawR canvas.
        isDrawingAllowed = IsMouseOverCanvas(_freehandDrawRaycaster) && !isOtherPanelOpen;

        if (Input.GetMouseButtonDown(0))
        {
            if (isDrawingAllowed && !isOtherPanelOpen)
            {
                isInsideDrawable = true;
                _lineScript = Instantiate(_linePrefab, mousePos, Quaternion.identity, LineParent);
                _lineScript.name = LineIndex.ToString();
                LineIndex++;
            }
            else
            {
                isInsideDrawable = false;
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (isDrawingAllowed && !isOtherPanelOpen && isInsideDrawable)
            {
                _lineScript.SetPosition(mousePos);
            }
            else
            {
                isInsideDrawable = false;
            }
        }
    }

    public void OnStartWorksheet(string input)
    {
        AnimationScript.Instance.PlayAnimation();

        // Disabling all Screen Space Overlay Canvas
        TurnOffScreenSpaceOverlayCanvas();

        // Activating Worksheet Components
        OnActivatingWorksheet();

        // Getting Sprite and prefab from resources folder.
        OnGettingWorksheetResources(input);

        LineWidth = 0.1f;
    }

    public void OnDrawing(string input)
    {
        // Disabling all Screen Space Overlay Canvas
        TurnOffScreenSpaceOverlayCanvas();

        switch (input)
        {
            case "Canvas":
                OnActivatingCanvas();
                break;
            case "Paint":
                OnActivatingPaint();
                break;
            case "Connect":
                OnActivatingConnect();
                break;
        }

        LineWidth = 0.1f;
        Scrollbar scroll = _colorPalatteContent.parent.parent.GetChild(1).GetComponent<Scrollbar>();
        scroll.value = 1;
    }


    #region Required Component Function

    private void TurnOffScreenSpaceOverlayCanvas()
    {
        // Disabling all Screen Space Overlay Canvas
        HomeScreenMgrScript.Instance.HomescreenCanvas.SetActive(false);
        HomeScreenMgrScript.Instance.SettingsCanvas.SetActive(false);
        HomeScreenMgrScript.Instance.ActivityCanvas.SetActive(false);
        HomeScreenMgrScript.Instance.TracingCanvas.SetActive(false);
        HomeScreenMgrScript.Instance.DragDropCanvas.SetActive(false);
        HomeScreenMgrScript.Instance.PopTheBubbleCanvas.SetActive(false);
    }

    private void TurnOnScreenSpaceOverlayCanvas()
    {
        // Activating all Screen Space Overlay Canvas
        HomeScreenMgrScript.Instance.HomescreenCanvas.SetActive(true);
        HomeScreenMgrScript.Instance.SettingsCanvas.SetActive(true);
        HomeScreenMgrScript.Instance.ActivityCanvas.SetActive(true);
        HomeScreenMgrScript.Instance.TracingCanvas.SetActive(true);
        HomeScreenMgrScript.Instance.DragDropCanvas.SetActive(true);
        HomeScreenMgrScript.Instance.PopTheBubbleCanvas.SetActive(true);
    }

    private void OnActivatingWorksheet()
    {
        // Activate necessary components for a worksheet
        Transform background = HomeScreenMgrScript.Instance.DrawingBGCanvas.transform.GetChild(0);
        Transform displayImg = HomeScreenMgrScript.Instance.DrawingBGCanvas.transform.GetChild(1);
        Transform worksheet = HomeScreenMgrScript.Instance.FreeHandDrawCanvas.transform.GetChild(0);
        Transform backButton = HomeScreenMgrScript.Instance.DrawToolsCanvas.transform.GetChild(0);
        Transform toolsPanel = HomeScreenMgrScript.Instance.DrawToolsCanvas.transform.GetChild(1);

        // Set components to be visible
        background.gameObject.SetActive(true);
        displayImg.gameObject.SetActive(true);
        worksheet.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);
        toolsPanel.gameObject.SetActive(true);
    }

    private void OnActivatingCanvas()
    {
        // Activate components for the canvas drawing mode
        Transform background = HomeScreenMgrScript.Instance.DrawingBGCanvas.transform.GetChild(0);
        Transform drawableCanvas = HomeScreenMgrScript.Instance.FreeHandDrawCanvas.transform.GetChild(1);
        Transform backButton = HomeScreenMgrScript.Instance.DrawToolsCanvas.transform.GetChild(0);
        Transform toolsPanel = HomeScreenMgrScript.Instance.DrawToolsCanvas.transform.GetChild(1);
        Transform colorPickerButton = HomeScreenMgrScript.Instance.DrawToolsCanvas.transform.GetChild(3);

        // Set components to be visible
        background.gameObject.SetActive(true);
        drawableCanvas.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);
        toolsPanel.gameObject.SetActive(true);
        colorPickerButton.gameObject.SetActive(true);
    }
    
    private void OnActivatingPaint()
    {
        // Activate components for the paint drawing mode
        Transform drawableCanvas = HomeScreenMgrScript.Instance.FreeHandDrawCanvas.transform.GetChild(1);
        Transform backButton = HomeScreenMgrScript.Instance.DrawToolsCanvas.transform.GetChild(0);
        Transform toolsPanel = HomeScreenMgrScript.Instance.DrawToolsCanvas.transform.GetChild(1);
        Transform refrenceImage = HomeScreenMgrScript.Instance.DrawToolsCanvas.transform.GetChild(2);
        Transform colorPickerButton = HomeScreenMgrScript.Instance.DrawToolsCanvas.transform.GetChild(3);

        // Set components to be visible
        drawableCanvas.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);
        toolsPanel.gameObject.SetActive(true);
        refrenceImage.gameObject.SetActive(true);
        colorPickerButton.gameObject.SetActive(true);
    }
    
    private void OnActivatingConnect()
    {
        // Activate components for the connect drawing mode
        Transform drawableCanvas = HomeScreenMgrScript.Instance.FreeHandDrawCanvas.transform.GetChild(1);
        Transform backButton = HomeScreenMgrScript.Instance.DrawToolsCanvas.transform.GetChild(0);
        Transform toolsPanel = HomeScreenMgrScript.Instance.DrawToolsCanvas.transform.GetChild(1);
        Transform refrenceImage = HomeScreenMgrScript.Instance.DrawToolsCanvas.transform.GetChild(2);
        Transform colorPickerButton = HomeScreenMgrScript.Instance.DrawToolsCanvas.transform.GetChild(3);

        // Set components to be visible
        drawableCanvas.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);
        toolsPanel.gameObject.SetActive(true);
        refrenceImage.gameObject.SetActive(true);
        colorPickerButton.gameObject.SetActive(true);
    }

    private void OnFreeHandDrawBackBtn()
    {
        TurnOnScreenSpaceOverlayCanvas();

        // Setting off all Freehand Drawing panel
        foreach (Transform t in HomeScreenMgrScript.Instance.DrawingBGCanvas.transform)
        {
            t.gameObject.SetActive(false);
        }
        foreach (Transform t in HomeScreenMgrScript.Instance.FreeHandDrawCanvas.transform)
        {
            t.gameObject.SetActive(false);
        }
        foreach (Transform t in HomeScreenMgrScript.Instance.DrawToolsCanvas.transform)
        {
            t.gameObject.SetActive(false);
        }

        // Destroying All Lines Drawn
        foreach (Transform t in LineParent)
        {
            Destroy(t.gameObject);
        }

        // Reseting Line Index
        LineIndex = HomeScreenMgrScript.Instance.FreeHandDrawCanvas.GetComponent<Canvas>().sortingOrder;
    }

    private void OnColorPalatteBtn()
    {
        for (int i = 0; i < _colorPalatteContent.childCount; i++)
        {
            Image img = _colorPalatteContent.GetChild(i).GetComponent<Image>();
            Button btn = _colorPalatteContent.GetChild(i).GetComponent<Button>();
            btn.onClick.AddListener(() => { LineColor = img.color; });
            btn.onClick.AddListener(() => { DrawToolsScript.Instance.SetEraserOff(); });
        }
    }

    #endregion


    #region Initiating Worksheet Setup

    private void OnGettingWorksheetResources(string input)
    {
        Debug.Log(input);
        //// Getting Sprite and prefab from resources folder and Activitaing Them.
        //_maskImage.sprite = Resources.Load<Sprite>("Tracing_Images/Mask/" + input);
        //_dragHandlerImg.sprite = Resources.Load<Sprite>("Tracing_Images/Dragger/" + input);
        //_finalImage.sprite = Resources.Load<Sprite>("Tracing_Images/FinalImage/" + input);
        //GameObject prefab = Resources.Load<GameObject>("Tracing_Prefab/" + input);
        //Instantiate(prefab, _prefabHolder);
    }

    #endregion


    #region Function Checking Drawable Area

    private bool IsMouseOverCanvas(GraphicRaycaster raycaster)
    {
        // Check if the mouse pointer is over a specified canvas

        // Create a PointerEventData to simulate a mouse event at the current mouse position
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        // Perform a raycast with the specified raycaster to determine if any UI elements are under the mouse pointer
        var results = new List<RaycastResult>();
        raycaster.Raycast(eventData, results);

        // If there are any results (UI elements) under the mouse pointer, return true, indicating the mouse is over the canvas
        return results.Count > 0;
    }

    #endregion




}
