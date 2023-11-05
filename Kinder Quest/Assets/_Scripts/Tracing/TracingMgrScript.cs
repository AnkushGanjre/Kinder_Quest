using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TracingMgrScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Singleton Reference")]
    public static TracingMgrScript Instance;

    [Header("Button Images")]
    public Sprite BlueBtnSprite;
    public Sprite GreenBtnSprite;
    public Sprite OrangeBtnSprite;
    public Sprite YellowBtnSprite;

    [Header("Tracing Panel Components")]
    [SerializeField] Transform _tracingCanvas;
    [SerializeField] Transform _prefabHolder;
    [SerializeField] Image _maskImage;
    [SerializeField] Image _dragHandlerImg;
    [SerializeField] Image _finalImage;
    [SerializeField] Button _nextButton;
    [SerializeField] Button _tracingBackButton;

    [Header("Required Parameters")]
    [SerializeField] Transform[] _bezierControlPoints; // Reference to Bezier control points
    [SerializeField] float _bezierClosestPoint;
    [SerializeField] Vector2 _previousPosition;
    [SerializeField] Texture2D _maskImgTexture;
    [SerializeField] Slider _currentSlider;
    [SerializeField] int _strokeNum;
    [SerializeField] int _currentStroke;
    [SerializeField] bool isDrawingAllowed;


    private void Awake()
    {
        Instance = Instance == null ? this : Instance;  // Setting Singleton Instance

        // Find and set references to UI elements and objects
        _tracingCanvas = GameObject.Find("Tracing_Canvas").transform;
        _prefabHolder = GameObject.Find("Tracing_PrefabHolder").transform;
        _maskImage = GameObject.Find("Tracing_Mask_Image").GetComponent<Image>();
        _dragHandlerImg = GameObject.Find("Tracing_Drag_Handler").GetComponent<Image>();
        _finalImage = GameObject.Find("Tracing_Final_Image").GetComponent<Image>();
        _nextButton = GameObject.Find("Tracing_NextButton").GetComponent<Button>();
        _tracingBackButton = GameObject.Find("Tracing_BackButton").GetComponent<Button>();
    }

    private void Start()
    {
        // Loading Sprite from Resources
        BlueBtnSprite = Resources.Load<Sprite>("UI/Tracing/Sub_Btn_Blue");
        GreenBtnSprite = Resources.Load<Sprite>("UI/Tracing/Sub_Btn_Green");
        OrangeBtnSprite = Resources.Load<Sprite>("UI/Tracing/Sub_Btn_Orange");
        YellowBtnSprite = Resources.Load<Sprite>("UI/Tracing/Sub_Btn_yellow");

        // Assign click event handlers to buttons
        _tracingBackButton.onClick.AddListener(() => { OnTracingBackButton(); });
        _nextButton.onClick.AddListener(() => { OnTracingNextButton(); });

        // Deactiviting all Components of Tracing Canvas
        OnTracingBackButton();
    }

    public void OnStartTracing(string input)
    {
        // Activating Tracing Components
        OnActivateTracingComponent();

        // Getting Sprite and prefab from resources folder.
        OnGettingResources(input);

        // setting position of drag Handler & Bezier Points 
        OnTracingInitiate();
    }


    #region Required Component Function

    private void OnActivateTracingComponent()
    {
        // Activating Tracing Components
        foreach (Transform t in _tracingCanvas)
        {
            t.gameObject.SetActive(true);
        }
        _finalImage.gameObject.SetActive(false);
        _nextButton.gameObject.SetActive(false);
    }

    private void OnTracingBackButton()
    {
        // Destroying Prefab if it exists!
        Destroy(_prefabHolder.childCount > 0 ? _prefabHolder.GetChild(0).gameObject : null);

        // Deactivating Tracing Components
        foreach (Transform t in _tracingCanvas)
        {
            t.gameObject.SetActive(false);
        }
    }

    #endregion


    #region Initiating Tracing Setup

    private void OnGettingResources(string input)
    {
        // Getting Sprite and prefab from resources folder and Activitaing Them.
        _maskImage.sprite = Resources.Load<Sprite>("Tracing_Images/Mask/" + input);
        _dragHandlerImg.sprite = Resources.Load<Sprite>("Tracing_Images/Dragger/" + input);
        _finalImage.sprite = Resources.Load<Sprite>("Tracing_Images/FinalImage/" + input);
        GameObject prefab = Resources.Load<GameObject>("Tracing_Prefab/" + input);
        Instantiate(prefab, _prefabHolder);
    }

    private void OnTracingInitiate()
    {
        bool isPrefabThere = _prefabHolder.childCount == 0 ? false : true;
        if (isPrefabThere)
        {
            // Getting image's texture
            _maskImgTexture = _maskImage.sprite.texture;

            // Setting Bezier points of first stroke
            List<Transform> newList = new List<Transform>();
            Transform stroke_1 = _prefabHolder.GetChild(0).GetChild(0).GetChild(0);
            foreach (Transform t in stroke_1)
            {
                newList.Add(t);
            }
            _bezierControlPoints = newList.ToArray();
            _currentSlider = _prefabHolder.GetChild(0).GetChild(1).GetChild(0).GetComponent<Slider>();

            // Getting number of strokes of current letters.
            int childCount = _prefabHolder.GetChild(0).GetChild(0).childCount;
            _strokeNum = childCount;
            _currentStroke = 1;

            // Setting DragHandler Position
            _dragHandlerImg.transform.position = stroke_1.GetChild(0).position;
        }
    }

    #endregion


    #region Execute Tracing

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Record the initial position of the pointer when dragging starts
        _previousPosition = eventData.position;

        // Calculate the closest point on the Bezier curve to the initial pointer position
        _bezierClosestPoint = GetClosestPointOnBezier(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnTextureCheck(eventData);

        if (isDrawingAllowed)
        {
            // Calculate the position on the Bezier curve closest to the pointer position
            _bezierClosestPoint = GetClosestPointOnBezier(eventData.position);

            // Calculate the position on the Bezier curve based on t
            Vector3 newPosition = CalculateBezierPoint(_bezierControlPoints, _bezierClosestPoint);

            // Move the image to the new position
            transform.position = newPosition;
            //transform.position = eventData.position;

            // Record the previous Position
            _previousPosition = eventData.position;

            // Setting the Slider
            _currentSlider.value = _bezierClosestPoint + 0.1f;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Check if the tracing is complete or not
        if (_currentSlider.value >= 0.95f)
        {
            //TracingMgrScript.Instance.audioSource.Stop();
            _currentStroke++;
            if (_currentStroke! > _strokeNum)
            {
                _dragHandlerImg.gameObject.SetActive(false);
                _finalImage.gameObject.SetActive(true);
                _nextButton.gameObject.SetActive(true);
                //UnlockingNextTracingLtr();
                return;
            }
            OnNextStrokeSelection(_currentStroke);
        }
    }

    #endregion


    #region Function Checking Tracing 

    private Vector3 CalculateBezierPoint(Transform[] points, float t)
    {
        // Function to calculate a point on the Bezier curve
        // Implement your Bezier curve calculation logic here
        // You can use the control points to calculate the position at 't'

        // Code for a Bezier curve with five control points
        Vector3 p0 = points[0].position;
        Vector3 p1 = points[1].position;
        Vector3 p2 = points[2].position;
        Vector3 p3 = points[3].position;
        Vector3 p4 = points[4].position;

        Vector3 result = Mathf.Pow(1 - t, 4) * p0 + 4 * Mathf.Pow(1 - t, 3) * t * p1 +
                         6 * Mathf.Pow(1 - t, 2) * Mathf.Pow(t, 2) * p2 +
                         4 * (1 - t) * Mathf.Pow(t, 3) * p3 + Mathf.Pow(t, 4) * p4;

        return result;
    }

    private float GetClosestPointOnBezier(Vector2 pointerPosition)
    {
        // Initialize variables to keep track of the closest distance and t value
        float closestDistance = float.MaxValue;
        float closestT = 0f;

        int steps = 100; // Number of steps to sample the curve

        for (int i = 0; i < steps; i++)
        {
            float t = i / (float)steps;
            Vector3 bezierPoint = CalculateBezierPoint(_bezierControlPoints, t);

            // Calculate the distance between the pointer position and the point on the curve
            float distance = Vector2.Distance(pointerPosition, bezierPoint);

            // Update closestDistance and closestT if this point is closer
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestT = t;
            }
        }

        return closestT;
    }

    private void OnTextureCheck(PointerEventData eventData)
    {
        // Calculate the local position of the drag within the RectTransform.
        Vector2 localDragPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_maskImage.rectTransform, eventData.position, eventData.pressEventCamera, out localDragPosition))
        {
            // Calculate the pixel coordinates within the texture.
            Vector2 pixelUV = new Vector2((localDragPosition.x / _maskImage.rectTransform.rect.width) + 0.5f, (localDragPosition.y / _maskImage.rectTransform.rect.height) + 0.5f);
            pixelUV.x = Mathf.Clamp01(pixelUV.x);
            pixelUV.y = Mathf.Clamp01(pixelUV.y);

            int pixelX = Mathf.FloorToInt(pixelUV.x * _maskImgTexture.width);
            int pixelY = Mathf.FloorToInt(pixelUV.y * _maskImgTexture.height);

            // Get the color of the pixel at the dragged position.
            Color pixelColor = _maskImgTexture.GetPixel(pixelX, pixelY);

            // Check the alpha value of the pixel.
            if (pixelColor.a > 0)
            {
                // Dragged over a transparent pixel.
                isDrawingAllowed = false;

                // Sound Play
                //TracingMgrScript.audioSource.PlayOneShot(TracingMgrScript.audioClip);
            }
            else
            {
                // Dragged over Image.
                isDrawingAllowed = true;
            }
        }
    }

    #endregion


    #region Next Stroke & Tracing

    private void OnNextStrokeSelection(int strokeNum)
    {
        // Setting Bezier points of next stroke
        List<Transform> newList = new List<Transform>();
        Transform nextStroke = _prefabHolder.GetChild(0).GetChild(0).GetChild(strokeNum - 1);
        foreach (Transform t in nextStroke)
        {
            newList.Add(t);
        }
        _bezierControlPoints = newList.ToArray();

        //Setting Current Slider
        _currentSlider = _prefabHolder.GetChild(0).GetChild(1).GetChild(strokeNum - 1).GetComponent<Slider>();

        //Setting Draghandler Position
        _dragHandlerImg.transform.position = nextStroke.GetChild(0).position;
    }

    private void OnTracingNextButton()
    {
        Debug.Log("Next");
    }

    #endregion
}
