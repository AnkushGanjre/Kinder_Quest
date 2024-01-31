using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TracingMgrScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Singleton Reference")]
    public static TracingMgrScript Instance;

    [Header("Tracing Panel Components")]
    [SerializeField] Transform _tracingCanvas;
    [SerializeField] Transform _prefabHolder;
    [SerializeField] Image _maskImage;
    [SerializeField] Image _dragHandlerImg;
    [SerializeField] GameObject _nextPanel;
    [SerializeField] Button _tracingBackButton;

    [Header("Required Parameters")]
    [SerializeField] Transform[] _bezierControlPoints; // Reference to Bezier control points
    [SerializeField] float _bezierClosestPoint;
    [SerializeField] Vector2 _startingPosition;
    [SerializeField] Vector2 _previousPosition;
    [SerializeField] Texture2D _maskImgTexture;
    [SerializeField] Slider _currentSlider;
    [SerializeField] int _strokeNum;
    [SerializeField] int _currentStroke;
    [SerializeField] bool isDrawingAllowed;
    [SerializeField] float _sliderAdjustValue = 0.01f;
    [SerializeField] string _currentlyTracing;
    [SerializeField] GameObject dotPrefab;


    private void Awake()
    {
        Instance = Instance == null ? this : Instance;  // Setting Singleton Instance

        // Find and set references to UI elements and objects
        _tracingCanvas = GameObject.Find("Tracing_Canvas").transform;
        _prefabHolder = GameObject.Find("Tracing_PrefabHolder").transform;
        _maskImage = GameObject.Find("Tracing_Mask_Image").GetComponent<Image>();
        _dragHandlerImg = GameObject.Find("Tracing_Drag_Handler").GetComponent<Image>();
        _nextPanel = GameObject.Find("Tracing_Next_Panel");
        _tracingBackButton = GameObject.Find("Tracing_BackButton").GetComponent<Button>();
    }

    private void Start()
    {
        // Assign click event handlers to buttons
        _tracingBackButton.onClick.AddListener(() => { OnTracingBackButton(); });
        _nextPanel.transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { _tracingBackButton.onClick.Invoke(); });
        _nextPanel.transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(() => { OnTracingNextButton(); });

        // Deactiviting all Components of Tracing Canvas
        OnTracingBackButton();
    }

    public void OnStartTracing(string input)
    {
        _currentlyTracing = input;

        AnimationScript.Instance.PlayAnimation();

        // Activating Tracing Components
        OnActivateTracingComponent();

        // Getting Sprite and prefab from resources folder.
        OnGettingResources(input);
    }


    #region Required Component Function

    private void OnActivateTracingComponent()
    {
        // Activating Tracing Components
        foreach (Transform t in _tracingCanvas)
        {
            t.gameObject.SetActive(true);
        }
        _nextPanel.SetActive(false);
        GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        GetComponent<Image>().raycastTarget = true;
    }

    private void OnTracingBackButton()
    {
        // Destroying Prefab if it exists!
        //Destroy(_prefabHolder.childCount > 0 ? _prefabHolder.GetChild(0).gameObject : null);
        if (_prefabHolder.childCount > 0)
        {
            foreach(Transform t in _prefabHolder)
            {
                Destroy(t.gameObject);
            }
        }

        // Deactivating Tracing Components
        foreach (Transform t in _tracingCanvas)
        {
            t.gameObject.SetActive(false);
        }
        gameObject.SetActive(true);
        GetComponent<Image>().color = new Color32(255, 255, 255, 0);
        GetComponent<Image>().raycastTarget = false;

        StopAllCoroutines();
        isFading = false;
    }

    #endregion


    #region Initiating Tracing Setup

    private void OnGettingResources(string input)
    {
        // Getting Sprite and prefab from resources folder and Activitaing Them.
        Sprite maskImg = Resources.Load<Sprite>("Tracing_Mask_Image/" + input);
        _maskImage.sprite = maskImg ?? maskImg;

        string dragHandlerName = GetDraggerName(input);
        Sprite dragImg = Resources.Load<Sprite>("AllObjectImages/" + dragHandlerName);
        _dragHandlerImg.sprite = dragImg ?? dragImg;

        //_finalImage.sprite = Resources.Load<Sprite>("Tracing_Images/FinalImage/" + input);

        GameObject prefab = Resources.Load<GameObject>("Bezier_Prefabs/" + input);
        if (prefab != null)
        {
            GameObject go = Instantiate((prefab != null) ? prefab : null, _prefabHolder);
            Vector3 scaleFactor = CalculateScaleFactor();
            go.GetComponent<RectTransform>().localScale = scaleFactor;
        }

        // setting position of drag Handler & Bezier Points 
        OnTracingInitiate();
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
            _currentSlider = _prefabHolder.GetChild(0).GetChild(2).GetChild(0).GetComponent<Slider>();

            // Displaying current stroke path.
            VisualizeBezierCurvePath();

            // Getting number of strokes of current letters.
            int childCount = _prefabHolder.GetChild(0).GetChild(0).childCount;
            _strokeNum = childCount;
            _currentStroke = 1;

            // Setting DragHandler Position
            _dragHandlerImg.transform.position = stroke_1.GetChild(0).position;
        }
    }

    private string GetDraggerName(string input)
    {
        string result = null;

        if (input.Substring(0, 3) == "Cap" || input.Substring(0, 3) == "Sma")
        {
            string[] alphabetArray = HomeScreenMgrScript.Instance.BDSO.AllAlphabet;
            for (int i = 0; i < alphabetArray.Length; i++)
            {
                if (input.Substring(4) == alphabetArray[i])
                {
                    int a = i;
                    result = HomeScreenMgrScript.Instance.BDSO.AlphabetList[a];
                    return result;
                }
            }
        }
        else
        {
            // Randomly select one of the four arrays
            string[] selectedArray = GetRandomArray();

            // Check if the selected array is not null and has at least one element
            result = (selectedArray != null && selectedArray.Length > 0) ? selectedArray[Random.Range(0, selectedArray.Length)] : null;
        }

        return result;
    }

    private string[] GetRandomArray()
    {
        // Generate a random number from 1 to 4 to select one of the arrays
        int randomIndex = Random.Range(1, 5);

        switch (randomIndex)
        {
            case 1:
                return HomeScreenMgrScript.Instance.BDSO.FoodList;
            case 2:
                return HomeScreenMgrScript.Instance.BDSO.AnimalList;
            case 3:
                return HomeScreenMgrScript.Instance.BDSO.FruitList;
            case 4:
                return HomeScreenMgrScript.Instance.BDSO.ExtrasList;
            default:
                return null; // In case something goes wrong
        }
    }

    #endregion


    #region Execute Tracing

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Record the initial position of the pointer when dragging starts
        _previousPosition = eventData.position;
        _startingPosition = transform.position;

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

            // Record the previous Position
            _previousPosition = eventData.position;

            // Setting the Slider
            //_currentSlider.value = _bezierClosestPoint + 0.1f;
            _currentSlider.value = _bezierClosestPoint + _sliderAdjustValue;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Check if the tracing is complete or not
        if (_currentSlider.value >= 0.95f)
        {
            //TracingMgrScript.Instance.audioSource.Stop();
            _currentSlider.value = 1f;
            _currentStroke++;
            if (_currentStroke! > _strokeNum)
            {
                _dragHandlerImg.gameObject.SetActive(false);
                _nextPanel.SetActive(true);
                //UnlockingNextTracingLtr();
                return;
            }
            OnNextStrokeSelection(_currentStroke);
        }
        else
        {
            _currentSlider.value = 0f;
            transform.position = _startingPosition;
        }
    }

    #endregion


    #region Function Checking Tracing 

    private Vector3 CalculateBezierPoint(Transform[] points, float t)
    {
        // Function to calculate a point on the Bezier curve
        // You can use the control points to calculate the position at 't'

        int numPoints = points.Length;

        if (numPoints == 3) // Bezier curve with 3 control points
        {
            Vector3 p0 = points[0].position;
            Vector3 p1 = points[1].position;
            Vector3 p2 = points[2].position;

            // Perform calculations for 3 control points
            Vector3 result = Mathf.Pow(1 - t, 2) * p0 +
                             2 * (1 - t) * t * p1 +
                             Mathf.Pow(t, 2) * p2;

            return result;
        }
        else if (numPoints == 5) // Bezier curve with 5 control points
        {
            Vector3 p0 = points[0].position;
            Vector3 p1 = points[1].position;
            Vector3 p2 = points[2].position;
            Vector3 p3 = points[3].position;
            Vector3 p4 = points[4].position;

            // Perform calculations for 5 control points
            Vector3 result = Mathf.Pow(1 - t, 4) * p0 +
                             4 * Mathf.Pow(1 - t, 3) * t * p1 +
                             6 * Mathf.Pow(1 - t, 2) * Mathf.Pow(t, 2) * p2 +
                             4 * (1 - t) * Mathf.Pow(t, 3) * p3 +
                             Mathf.Pow(t, 4) * p4;

            return result;
        }
        else if (numPoints == 7) // Bezier curve with 7 control points
        {
            Vector3 p0 = points[0].position;
            Vector3 p1 = points[1].position;
            Vector3 p2 = points[2].position;
            Vector3 p3 = points[3].position;
            Vector3 p4 = points[4].position;
            Vector3 p5 = points[5].position;
            Vector3 p6 = points[6].position;

            Vector3 result = Mathf.Pow(1 - t, 6) * p0 +
                             6 * Mathf.Pow(1 - t, 5) * t * p1 +
                             15 * Mathf.Pow(1 - t, 4) * Mathf.Pow(t, 2) * p2 +
                             20 * Mathf.Pow(1 - t, 3) * Mathf.Pow(t, 3) * p3 +
                             15 * Mathf.Pow(1 - t, 2) * Mathf.Pow(t, 4) * p4 +
                             6 * (1 - t) * Mathf.Pow(t, 5) * p5 +
                             Mathf.Pow(t, 6) * p6;

            return result;
        }
        else if (numPoints == 9) // Bezier curve with 9 control points
        {
            Vector3 p0 = points[0].position;
            Vector3 p1 = points[1].position;
            Vector3 p2 = points[2].position;
            Vector3 p3 = points[3].position;
            Vector3 p4 = points[4].position;
            Vector3 p5 = points[5].position;
            Vector3 p6 = points[6].position;
            Vector3 p7 = points[7].position;
            Vector3 p8 = points[8].position;

            Vector3 result = Mathf.Pow(1 - t, 8) * p0 +
                             8 * Mathf.Pow(1 - t, 7) * t * p1 +
                             28 * Mathf.Pow(1 - t, 6) * Mathf.Pow(t, 2) * p2 +
                             56 * Mathf.Pow(1 - t, 5) * Mathf.Pow(t, 3) * p3 +
                             70 * Mathf.Pow(1 - t, 4) * Mathf.Pow(t, 4) * p4 +
                             56 * Mathf.Pow(1 - t, 3) * Mathf.Pow(t, 5) * p5 +
                             28 * Mathf.Pow(1 - t, 2) * Mathf.Pow(t, 6) * p6 +
                             8 * (1 - t) * Mathf.Pow(t, 7) * p7 +
                             Mathf.Pow(t, 8) * p8;

            return result;
        }

        return Vector3.zero; // Default return value for other cases
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

    private float fadeDuration = 0.05f; // Time taken for each fade (in seconds)
    private float delayBetweenFades = 0.2f; // Delay between each fade (in seconds)
    private float waveFrequency = 0.1f; // Adjusts the frequency of the wave effect
    private bool isFading = false;
    [SerializeField] float spacing = 75f;
    // Draw the Bezier curve path
    private void VisualizeBezierCurvePath()
    {
        float curveLength = CalculateBezierCurveLength();
        int numOfDots = Mathf.CeilToInt(curveLength / spacing);
        Transform dotHolder = _prefabHolder.GetChild(0).GetChild(1);

        for (int i = 0; i < numOfDots; i++)
        {
            float t = i / (float)(numOfDots - 1);
            Vector3 point = CalculateBezierPoint(_bezierControlPoints, t);

            // Create a GameObject for the dot
            GameObject dotObj = Instantiate(dotPrefab, point, Quaternion.identity, dotHolder);
        }
        StartCoroutine(RepeatFadeSequence(dotHolder));
    }

    IEnumerator RepeatFadeSequence(Transform dotHolder)
    {
        while (dotHolder.childCount > 0) // Keep repeating until there are no more child objects
        {
            for (int i = 0; i < dotHolder.childCount; i++)
            {
                GameObject child = dotHolder.GetChild(i).gameObject;
                if (child.activeSelf) // Check if the child is active
                {
                    StartCoroutine(WaveFade(child.GetComponent<Image>()));
                    yield return new WaitForSeconds(fadeDuration + delayBetweenFades);
                }
                yield return null;
            }
        }
    }

    IEnumerator WaveFade(Image image)
    {
        if (!isFading)
        {
            isFading = true;
            float currentTime = 0f;
            Color startColor = image.color;

            while (currentTime < fadeDuration)
            {
                currentTime += Time.deltaTime;
                float alpha = startColor.a * Mathf.Sin((currentTime / fadeDuration) * Mathf.PI * waveFrequency);
                image.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                yield return null;
            }

            image.color = new Color(startColor.r, startColor.g, startColor.b, 1f); // Ensure alpha is 0 after the wave
            isFading = false;
        }
    }

    // Calculate the length of the Bezier curve
    private float CalculateBezierCurveLength()
    {
        float totalLength = 0f;
        Vector3 lastPoint = CalculateBezierPoint(_bezierControlPoints, 0f);

        for (float t = 0.01f; t <= 1f; t += 0.01f)
        {
            Vector3 currentPoint = CalculateBezierPoint(_bezierControlPoints, t);
            totalLength += Vector3.Distance(currentPoint, lastPoint);
            lastPoint = currentPoint;
        }

        return totalLength;
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

    private Vector3 CalculateScaleFactor()
    {
        // Calculate scale factor based on screen size or other criteria
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Calculate your scaling logic based on the current screen size
        // For example, you might use a ratio of screen width/height to adjust the scale
        // Here, I'm assuming a fixed aspect ratio of 16:9 (1920x1080)
        float targetWidth = 1920f;
        float targetHeight = 1080f;

        float scaleFactorWidth = screenWidth / targetWidth;
        float scaleFactorHeight = screenHeight / targetHeight;

        // Use the smaller of the two scale factors to maintain aspect ratio
        Vector3 scaleFactor = new Vector3(scaleFactorWidth, scaleFactorHeight, 1f);

        return scaleFactor;
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
        _currentSlider = _prefabHolder.GetChild(0).GetChild(2).GetChild(strokeNum - 1).GetComponent<Slider>();

        // Displaying current stroke path.
        VisualizeBezierCurvePath();

        //Setting Draghandler Position
        _dragHandlerImg.transform.position = nextStroke.GetChild(0).position;
    }

    private void OnTracingNextButton()
    {
        OnTracingBackButton();
        StartCoroutine(WaitTime());
        

        IEnumerator WaitTime()
        {
            yield return new WaitForEndOfFrame();
            OnActivateTracingComponent();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            string currentCategory = _currentlyTracing.Substring(0, 3);
            string currentObject = _currentlyTracing.Substring(_currentlyTracing.Length - 1);
            string newString = _currentlyTracing.Substring(0, _currentlyTracing.Length - 1);
            string[] alphaArr = HomeScreenMgrScript.Instance.BDSO.AllAlphabet;

            switch (currentCategory)
            {
                case "Lcs":
                    int Lcs = int.Parse(currentObject);
                    Lcs++;
                    if (Lcs > 8)
                    {
                        Lcs = 1;
                    }

                    newString = newString + Lcs;
                    _currentlyTracing = newString;

                    break;
                case "Cap":     // Both have same logic
                case "Sma":
                    if (currentObject == "Z")
                    {
                        _currentlyTracing = newString + "A";
                        break;
                    }

                    for (int i = 0; i < alphaArr.Length; i++)
                    {
                        if (currentObject == alphaArr[i])
                        {
                            newString = newString + alphaArr[i + 1];
                            _currentlyTracing = newString;
                        }
                    }
                    break;
                case "Num":
                    int num = int.Parse(currentObject);
                    num++;

                    if (num > 20)
                    {
                        num = 0;
                    }

                    newString = newString + num;
                    _currentlyTracing = newString;

                    break;
                default: break;
            }

            OnStartTracing(_currentlyTracing);
        }
    }

    #endregion

}
