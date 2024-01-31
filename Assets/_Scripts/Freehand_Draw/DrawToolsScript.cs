using UnityEngine;
using UnityEngine.UI;

public class DrawToolsScript : MonoBehaviour
{
    [Header("Singleton Reference")]
    public static DrawToolsScript Instance;

    [Header("GameObjects & Transform")]
    [SerializeField] GameObject _toolsPanel;
    [SerializeField] GameObject _pencilSizeSelector;
    [SerializeField] Transform _eraserInAction;
    [SerializeField] RectTransform _eraserRectTransform;

    [Header("Camera & canvas Rect")]
    public Camera screenSpaceCamera;
    public RectTransform canvasRect;

    [Header("Boolean flag")]
    private bool isEraserToolOn;

    private void Awake()
    {
        Instance = Instance ?? this;  // Setting Singleton Instance

        // Find and set references to various UI elements and objects
        _toolsPanel = GameObject.Find("Tools_Panel");
        _pencilSizeSelector = GameObject.Find("Pencil_size_selector");
        _eraserInAction = GameObject.Find("Eraser_InAction").transform;
        _eraserRectTransform = _eraserInAction.GetComponent<RectTransform>();
    }

    private void Start()
    {
        // Set click event handlers for buttons
        _toolsPanel.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => { OnPencilBtn(); });
        _toolsPanel.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { OnEraserBtn(); });
        _toolsPanel.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { OnTrashCanBtn(); });
        _toolsPanel.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => { OnUndoBtn(); });

        // Set up button click listeners for pencil size selection
        for (int i = 0; i  < _pencilSizeSelector.transform.childCount; i++)
        {
            int a = i;
            _pencilSizeSelector.transform.GetChild(a).GetComponent<Button>().onClick.AddListener(() => { OnPencilSizeSelection(a); });
        }

        // Disable the pencil size selector and eraser in action at start
        _pencilSizeSelector.SetActive(false);
        _eraserInAction.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Check if the eraser tool is active and perform erasing
        
        if (isEraserToolOn)
            OnErasingLine();
    }

    #region All Tool's Functions

    private void OnPencilBtn()
    {
        if (_pencilSizeSelector.activeInHierarchy)
        {
            _pencilSizeSelector.SetActive(false);
            return;
        }
        if (isEraserToolOn)
        {
            SetEraserOff();
            return;
        }

        // Check if a specific canvas element is active on the home screen
        if (HomeScreenMgrScript.Instance.FreeHandDrawCanvas.transform.GetChild(0).gameObject.activeInHierarchy)
            // If that canvas element is active, do nothing (return)
            return;

        // Enable the pencil size selector and mark that another panel is open
        _pencilSizeSelector.SetActive(true);
        FreeHandDrawMgrScript.Instance.isOtherPanelOpen = true;
    }

    private void OnEraserBtn()
    {
        if (_pencilSizeSelector.activeInHierarchy)
        {
            _pencilSizeSelector.SetActive(false);
        }
        if (isEraserToolOn)
        {
            SetEraserOff();
            return;
        }

        // Set the flag to indicate that the eraser tool is active
        isEraserToolOn = true;

        // Activate the eraser in action (Transform) game object
        _eraserInAction.gameObject.SetActive(true);

        // Mark that another panel is open
        FreeHandDrawMgrScript.Instance.isOtherPanelOpen = true;
    }

    private void OnTrashCanBtn()
    {
        if (_pencilSizeSelector.activeInHierarchy)
        {
            _pencilSizeSelector.SetActive(false);
        }
        if (isEraserToolOn)
        {
            SetEraserOff();
            return;
        }

        // Get a reference to the parent of the drawn lines
        Transform lineParent = FreeHandDrawMgrScript.Instance.LineParent;

        // Check if there are any child objects (drawn lines) in the parent
        if (lineParent.childCount > 0)
        {
            // Iterate through each child object and destroy it (erase all drawn lines)
            foreach (Transform t in lineParent)
            {
                Destroy(t.gameObject);
            }
        }
    }

    private void OnUndoBtn()
    {
        if (_pencilSizeSelector.activeInHierarchy)
        {
            _pencilSizeSelector.SetActive(false);
        }
        if (isEraserToolOn)
        {
            SetEraserOff();
            return;
        }

        // Get a reference to the parent of the drawn lines
        Transform lineParent = FreeHandDrawMgrScript.Instance.LineParent;

        // Check if there are any child objects (drawn lines) in the parent
        if (lineParent.childCount > 0)
        {
            // Get the reference to the last child object (latest drawn line)
            Transform lastChild = lineParent.GetChild(lineParent.childCount - 1);

            // Destroy the last child object, effectively undoing the last drawn line
            Destroy(lastChild.gameObject);
        }
    }

    #endregion


    #region Pencil Size Function

    private void OnPencilSizeSelection(int sizeNum)
    {
        // Based on the selected sizeNum, adjust the line width in the drawing manager
        switch (sizeNum)
        {
            case 0:
                FreeHandDrawMgrScript.Instance.LineWidth = 0.1f;
                break;
            case 1:
                FreeHandDrawMgrScript.Instance.LineWidth = 0.5f;
                break;
            case 2:
                FreeHandDrawMgrScript.Instance.LineWidth = 1f;
                break;
        }

        // Disable the pencil size selector and mark that no other panel is open
        _pencilSizeSelector.SetActive(false);
        FreeHandDrawMgrScript.Instance.isOtherPanelOpen = false;
    }

    #endregion


    #region Eraser Erasing Function

    private void OnErasingLine()
    {
        // Get the current screen mouse position
        Vector2 screenMousePosition = Input.mousePosition;

        // Convert the screen mouse position to canvas space
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenMousePosition, screenSpaceCamera, out localPos);

        // Get the world position of the canvas element (assuming it's a child of the canvas)
        Vector3 worldMousePosition = canvasRect.TransformPoint(localPos);

        // Move the eraser in action to the calculated worldMousePosition
        _eraserInAction.position = worldMousePosition;

        // Check if the left mouse button is pressed
        if (Input.GetMouseButton(0))
        {
            // Iterate through existing lines and check if the eraser overlaps
            if (FreeHandDrawMgrScript.Instance.LineParent.childCount > 0)
            {
                for (int i = 0; i < FreeHandDrawMgrScript.Instance.LineParent.childCount; i++)
                {
                    LineRenderer line = FreeHandDrawMgrScript.Instance.LineParent.GetChild(i).GetComponent<LineRenderer>();

                    for (int j = 0; j < line.positionCount; j++)
                    {
                        // Convert the line's world position to screen space
                        Vector3 screenPosition = screenSpaceCamera.WorldToScreenPoint(line.GetPosition(j));

                        // Check if the eraser's RectTransform contains the screen position
                        bool isOverImage = RectTransformUtility.RectangleContainsScreenPoint(_eraserRectTransform, screenPosition, screenSpaceCamera);

                        if (isOverImage)
                        {
                            if (j == 0 || j == line.positionCount - 1)
                            {
                                // If the point is at the start or end, simply remove it
                                RemovePointFromLineRenderer(line, j);
                            }
                            else
                            {
                                // If the point is in the middle, split the line into two line objects
                                SplitLineRenderer(line, j);
                            }
                        }
                    }
                }
            }
        }

        // Disable the eraser tool when not in use
        if (!_eraserInAction.gameObject.activeInHierarchy)
        {
            _eraserInAction.gameObject.SetActive(false);
            FreeHandDrawMgrScript.Instance.isOtherPanelOpen = false;
            isEraserToolOn = false;
        }
    }

    void RemovePointFromLineRenderer(LineRenderer lineRenderer, int pointIndexToRemove)
    {
        int currentPointCount = lineRenderer.positionCount;
        Vector3[] currentPositions = new Vector3[currentPointCount];
        lineRenderer.GetPositions(currentPositions);

        // Create a new array without the point to remove
        Vector3[] newPositions = new Vector3[currentPointCount - 1];
        int newIndex = 0;
        for (int i = 0; i < currentPointCount; i++)
        {
            if (i != pointIndexToRemove)
            {
                newPositions[newIndex] = currentPositions[i];
                newIndex++;
            }
        }

        // Update the LineRenderer with the new positions
        lineRenderer.positionCount = currentPointCount - 1;
        lineRenderer.SetPositions(newPositions);

        if (currentPointCount <= 3)
        {
            Destroy(lineRenderer.gameObject);
        }
    }

    void SplitLineRenderer(LineRenderer lineRenderer, int splitPointIndex)
    {
        // Get the current point count and positions of the LineRenderer
        int currentPointCount = lineRenderer.positionCount;
        Vector3[] currentPositions = new Vector3[currentPointCount];
        lineRenderer.GetPositions(currentPositions);

        // Create two new arrays for the positions of the first and second lines
        Vector3[] firstLinePositions = new Vector3[splitPointIndex];
        Vector3[] secondLinePositions = new Vector3[currentPointCount - splitPointIndex - 1];

        // Fill the firstLinePositions array with positions before the split point
        for (int i = 0; i < splitPointIndex; i++)
        {
            firstLinePositions[i] = currentPositions[i];
        }

        // Fill the secondLinePositions array with positions after the split point
        for (int i = splitPointIndex + 1; i < currentPointCount; i++)
        {
            secondLinePositions[i - splitPointIndex - 1] = currentPositions[i];
        }

        // Create a new GameObject for the second line
        GameObject secondLineObject = new GameObject("SecondLine");
        secondLineObject.transform.SetParent(FreeHandDrawMgrScript.Instance.LineParent);

        // Add a LineRenderer component to the second line object
        LineRenderer secondLineRenderer = secondLineObject.AddComponent<LineRenderer>();
        secondLineRenderer.positionCount = secondLinePositions.Length;
        secondLineRenderer.SetPositions(secondLinePositions);

        // Set the start and end widths of the second line
        secondLineRenderer.startWidth = secondLineRenderer.endWidth = lineRenderer.startWidth;

        // Create a new material for the second line
        secondLineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        // Set the color of the second line
        FreeHandDrawMgrScript.Instance.LineColor.a = 1f;
        secondLineRenderer.material.color = FreeHandDrawMgrScript.Instance.LineColor;
        //secondLineRenderer.material.SetColor("_Tint", FreeHandDrawMgrScript.Instance.LineColor);

        // Set the sorting order of the second line (rendering order)
        secondLineRenderer.sortingOrder = lineRenderer.sortingOrder;

        // Update the original LineRenderer with the positions of the first line
        lineRenderer.positionCount = firstLinePositions.Length;
        lineRenderer.SetPositions(firstLinePositions);

        // Optionally, you can remove this script from the object to prevent further splitting
        if (currentPointCount <= 3)
            Destroy(lineRenderer.gameObject);
    }

    #endregion


    public void SetEraserOff()
    {
        if (_pencilSizeSelector.activeInHierarchy)
        {
            _pencilSizeSelector.SetActive(false);
        }

        // If the eraser tool is active, switch it off
        if (isEraserToolOn)
        {
            isEraserToolOn = false;
            FreeHandDrawMgrScript.Instance.isOtherPanelOpen = false;
            _eraserInAction.gameObject.SetActive(false);
        }
        
    }

}
