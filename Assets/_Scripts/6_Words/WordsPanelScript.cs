using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WordsPanelScript : MonoBehaviour
{
    [Header("Singleton Reference")]
    public static WordsPanelScript Instance;

    [Header("Words Panel Components")]
    [SerializeField] Image _wrdImageObject;
    [SerializeField] Transform _wrdContentTransform;
    [SerializeField] Transform _wholeWordTransform;
    [SerializeField] Transform _jumbledWordTransform;

    [Header("Prefabs")]
    [SerializeField] GameObject _wholeWordPrefab;

    [Header("Array For Jumbling")]
    [SerializeField] private List<Sprite> _letterSprites = new List<Sprite>();
    [SerializeField] private List<Transform> _selectedJumbledLtr = new List<Transform>();
    [SerializeField] private List<Transform> _allJumbledLtr = new List<Transform>();

    [Header("Next panel & Back Button")]
    private GameObject _nextPanel;
    private Button _wrdBackBtn;

    [Header("Boolean Flags")]
    [SerializeField] bool _isColorSelected;

    [Header("Colored Button Images")]
    public Sprite BlueBtnSprite;
    public Sprite GreenBtnSprite;
    public Sprite OrangeBtnSprite;
    public Sprite YellowBtnSprite;

    [SerializeField] float _jumbleWaitTime = 4f;
    [SerializeField] string _currentWord;
    [SerializeField] string _currentCategory;

    private void Awake()
    {
        Instance = Instance ?? this;  // Setting Singleton Instance
        if (Instance != this) Destroy(gameObject);  // If not Active Singleton, destroy it
        DontDestroyOnLoad(gameObject);  // Ensure that the Singleton persists across scene changes

        // Find and initialize references to various components
        _wrdImageObject = GameObject.Find("Wor_Image_Object").GetComponent<Image>();
        _wrdContentTransform = GameObject.Find("Wrd_Panel_Content").transform;
        _wholeWordTransform = GameObject.Find("Wor_Whole_Word").transform;
        _jumbledWordTransform = GameObject.Find("Wor_Jumbled_Letter").transform;
        _nextPanel = GameObject.Find("Wor_Next_Panel");
        _wrdBackBtn = GameObject.Find("Wor_Words_BackButton").GetComponent<Button>();
    }

    private void Start()
    {
        // Set up click listeners for word categories
        Transform wrdPanel = HomeScreenMgrScript.Instance.ActivityCanvas.transform.GetChild(3);

        // Attach click listeners to category buttons
        wrdPanel.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { OnWrdFoodsBtn(); });
        wrdPanel.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { OnWrdFruitsBtn(); });
        wrdPanel.GetChild(3).GetComponent<Button>().onClick.AddListener(() => { OnWrdAnimalsBtn(); });
        wrdPanel.GetChild(4).GetComponent<Button>().onClick.AddListener(() => { OnWrdColorBtn(); });

        // Attach a click listener to the "Next" button
        _nextPanel.transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { _wrdBackBtn.onClick.Invoke(); });
        _nextPanel.transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(() => { OnNextBtnSelection(); });

        // Attach a click listener to the back button
        Button backBtn = HomeScreenMgrScript.Instance.DragDropCanvas.transform.GetChild(0).GetChild(0).GetComponent<Button>();
        backBtn.onClick.AddListener(() => { OnBackBtnSelection(); });

        // Load the whole word prefab
        _wholeWordPrefab = Resources.Load<GameObject>("Prefabs/Whole_Wrd_Img");
        BlueBtnSprite = Resources.Load<Sprite>("Common_Images/Colored_Buttons/Blue_Btn");
        GreenBtnSprite = Resources.Load<Sprite>("Common_Images/Colored_Buttons/Green_Btn");
        OrangeBtnSprite = Resources.Load<Sprite>("Common_Images/Colored_Buttons/Orange_Btn");
        YellowBtnSprite = Resources.Load<Sprite>("Common_Images/Colored_Buttons/Yellow_Btn");

        AdjustChildSize();
    }

    private void AdjustChildSize()
    {
        // All numbers are default size divide by height/width to adjust size
        // Activity Panel Child size Control
        float width = _wrdContentTransform.GetComponent<RectTransform>().rect.width;
        float height = _wrdContentTransform.GetComponent<RectTransform>().rect.height;
        width = Mathf.RoundToInt(width / 5.73f);
        height = Mathf.RoundToInt(height / 4.45f);
        _wrdContentTransform.GetComponent<GridLayoutGroup>().cellSize = new Vector2(width, height);

        // Drag & Drop Panel Child Size Control
        float panelHeight = _wrdImageObject.transform.parent.GetChild(1).GetComponent<RectTransform>().rect.height;
        float panelWidth = _wrdImageObject.transform.parent.GetChild(1).GetComponent<RectTransform>().rect.width;
        float cellSize = panelHeight / 4.2f;
        panelHeight = panelHeight - 100f;
        panelHeight = (panelHeight > 500f) ? 500 : panelHeight;     // If greater than 500 then set 500
        _wrdImageObject.GetComponent<RectTransform>().sizeDelta = new Vector2(panelHeight, panelHeight);
        _jumbledWordTransform.GetComponent<GridLayoutGroup>().cellSize = new Vector2(cellSize, cellSize);
        float spaceSize = panelWidth / 8.16f;
        _jumbledWordTransform.GetComponent<GridLayoutGroup>().spacing = new Vector2(spaceSize, 40f);
    }

    #region Main Button's Function

    public void OnWrdFoodsBtn()
    {
        // This method is called when the "Food" button is clicked
        // Populate the word content with food-related words & Change Btn Sprite
        _currentCategory = "Food";
        _isColorSelected = false;
        string[] foodsArray = HomeScreenMgrScript.Instance.BDSO.FoodList;
        for (int i = 0; i < foodsArray.Length; i++)
        {
            _wrdContentTransform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = foodsArray[i];
            _wrdContentTransform.GetChild(i).GetComponent<Image>().sprite = YellowBtnSprite;
        }

        // Start a coroutine to assign click functions to word buttons
        StartCoroutine(OnAssignSubBtnFunction());
    }

    public void OnWrdFruitsBtn()
    {
        // This method is called when the "Fruits" button is clicked
        // Populate the word content with fruit-related words
        _currentCategory = "Fruit";
        _isColorSelected = false;
        string[] fruitsArray = HomeScreenMgrScript.Instance.BDSO.FruitList;
        for (int i = 0; i < fruitsArray.Length; i++)
        {
            _wrdContentTransform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = fruitsArray[i];
            _wrdContentTransform.GetChild(i).GetComponent<Image>().sprite = OrangeBtnSprite;
        }

        // Start a coroutine to assign click functions to word buttons
        StartCoroutine(OnAssignSubBtnFunction());
    }

    public void OnWrdAnimalsBtn()
    {
        // This method is called when the "Animals" button is clicked
        // Populate the word content with animal-related words
        _currentCategory = "Animal";
        _isColorSelected = false;
        string[] animalsArray = HomeScreenMgrScript.Instance.BDSO.AnimalList;
        for (int i = 0; i < animalsArray.Length; i++)
        {
            _wrdContentTransform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = animalsArray[i];
            _wrdContentTransform.GetChild(i).GetComponent<Image>().sprite = BlueBtnSprite;
        }

        // Start a coroutine to assign click functions to word buttons
        StartCoroutine(OnAssignSubBtnFunction());
    }

    public void OnWrdColorBtn()
    {
        // This method is called when the "Color" button is clicked
        // Populate the word content with color-related words
        _currentCategory = "Color";
        _isColorSelected = true;
        string[] colorsArray = HomeScreenMgrScript.Instance.BDSO.ColorList;
        for (int i = 0; i < colorsArray.Length; i++)
        {
            _wrdContentTransform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = colorsArray[i];
            _wrdContentTransform.GetChild(i).GetComponent<Image>().sprite = GreenBtnSprite;
        }

        // Start a coroutine to assign click functions to word buttons
        StartCoroutine(OnAssignSubBtnFunction());
    }

    #endregion


    #region Sub Button Function

    private void OnWordsActivity(string input)
    {
        AnimationScript.Instance.PlayAnimation();

        _currentWord = input;

        //Debug.Log(input);
        // Hide the next panel (if it's visible) and show the drawing canvas
        _nextPanel.SetActive(false);
        
        _wrdImageObject.color = Color.white;

        // Setting Word Image
        if (_isColorSelected)
        {
            string[] colorArray = HomeScreenMgrScript.Instance.BDSO.ColorList;
            string hexCode = null; // Initialize hexCode with null in case no match is found

            for (int i = 0; i < colorArray.Length; i++)
            {
                if (input == colorArray[i])
                {
                    hexCode = HomeScreenMgrScript.Instance.BDSO.HexCodeList[i];
                    break;
                }
            }
 
            Sprite wrdImage = Resources.Load<Sprite>("Circle");
            _wrdImageObject.sprite = wrdImage;

            if (hexCode != null)
            {
                Color newColor;
                if (ColorUtility.TryParseHtmlString(hexCode, out newColor))
                {
                    _wrdImageObject.color = newColor;
                }
                else
                {
                    Debug.LogError("Invalid hex color code: " + hexCode);
                }
            }
            else
            {
                Debug.LogError("No matching color found for input: " + input);
            }
        }
        else
        {
            Sprite wrdImage = Resources.Load< Sprite>("AllObjectImages/" + input);
            _wrdImageObject.sprite = wrdImage;
        }
        HomeScreenMgrScript.Instance.DragDropCanvas.transform.GetChild(0).gameObject.SetActive(true);
        // Hide the jumbled word panel and instantiate the whole word images
        _jumbledWordTransform.gameObject.SetActive(false);
        foreach (char letter in input)
        {
            GameObject go = Instantiate(_wholeWordPrefab, _wholeWordTransform);
            Sprite sprite = Resources.Load<Sprite>("Letters/Capital_Letters/" + letter.ToString().ToLower());
            _letterSprites.Add(sprite);
            go.GetComponent<Image>().sprite = sprite;
        }

        // Start Coroutine to jumble and initiate jumbled letters
        StartCoroutine(OnJumbleLetterSelect());
        StartCoroutine(OnInitiateJumbledLtr());
    }

    private IEnumerator OnInitiateJumbledLtr()
    {
        yield return new WaitForSeconds(_jumbleWaitTime);
        // Show the jumbled word panel
        _jumbledWordTransform.gameObject.SetActive(true);

        // Assign Jumbled Letter Sprites to the selected jumbled letter positions
        for (int i = 0; i < _letterSprites.Count; i++)
        {
            yield return new WaitForEndOfFrame();
            _selectedJumbledLtr[i].GetComponent<Image>().sprite = _letterSprites[i];
            _selectedJumbledLtr[i].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            _selectedJumbledLtr[i].GetComponent<CanvasGroup>().blocksRaycasts = true;
        }

        // Reduce the alpha of letters in the whole word panel
        foreach (Transform t in _wholeWordTransform)
        {
            yield return new WaitForEndOfFrame();
            t.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
        }
    }

    private IEnumerator OnJumbleLetterSelect()
    {
        yield return new WaitForEndOfFrame();
        // Populate the list of child transforms.
        foreach (Transform child in _jumbledWordTransform)
        {
            _allJumbledLtr.Add(child);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        // Randomly select images without repeats.
        for (int i = 0; i < _letterSprites.Count; i++)
        {
            int randomIndex = Random.Range(0, _allJumbledLtr.Count);
            _selectedJumbledLtr.Add(_allJumbledLtr[randomIndex]);
            _allJumbledLtr.RemoveAt(randomIndex);
        }
    }

    private IEnumerator OnAssignSubBtnFunction()
    {
        // Loop through each subcategory button in the word panel
        foreach (Transform t in _wrdContentTransform)
        {
            // Remove any existing click listeners
            t.GetComponent<Button>().onClick.RemoveAllListeners();
            yield return new WaitForEndOfFrame(); // Wait for the end of the frame.

            // Get the activity name from the text of the subcategory button
            string activity = t.GetChild(0).GetComponent<TextMeshProUGUI>().text;

            // Add a new click listener that triggers the OnWordsActivity method with the selected activity name
            t.GetComponent<Button>().onClick.AddListener(() => { OnWordsActivity(activity); });
        }
    }


    #endregion


    #region Supportive Functions

    private void OnBackBtnSelection()
    {
        StopAllCoroutines();

        if (_wholeWordTransform.childCount > 0)
        {
            // Destroy any previously instantiated whole word images
            foreach (Transform t in _wholeWordTransform)
            {
                Destroy(t.gameObject);
            }

            // Reset jumbled letter images and interaction properties
            foreach (Transform t in _jumbledWordTransform)
            {
                t.GetComponent<Image>().sprite = null;
                t.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
                t.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }

            // Clear letter sprite data and lists
            _letterSprites.Clear();
            _selectedJumbledLtr.Clear();
            _allJumbledLtr.Clear();
        }
    }

    public void CheckMatchingProgress()
    {
        foreach(Transform t in _jumbledWordTransform)
        {
            // Check if any jumbled letter is still interactive (blocksRaycasts is true)
            if (t.GetComponent<CanvasGroup>().blocksRaycasts == true)
                return; // If any letter is still interactive, exit the method
        }

        // If all jumbled letters have been correctly matched, show the "Next" panel
        _nextPanel.SetActive(true);
    }


    private void OnNextBtnSelection()
    {
        OnBackBtnSelection();

        // Hide the "Next" panel to allow the user to continue the activity
        _nextPanel.SetActive(false);
        string[] categoryList;

        switch (_currentCategory)
        {
            case "Food":
                categoryList = HomeScreenMgrScript.Instance.BDSO.FoodList;
                break;
            case "Fruit":
                categoryList = HomeScreenMgrScript.Instance.BDSO.FruitList;
                break;
            case "Animal":
                categoryList = HomeScreenMgrScript.Instance.BDSO.AnimalList;
                break;
            case "Color":
                categoryList = HomeScreenMgrScript.Instance.BDSO.ColorList;
                break;
            default:
                categoryList = HomeScreenMgrScript.Instance.BDSO.FoodList;
                break;
        }

        for (int i = 0; i < categoryList.Length; i++)
        {
            if (_currentWord == categoryList[i])
            {
                if (i == categoryList.Length - 1)
                {
                    _currentWord = categoryList[0];
                    break;
                }
                _currentWord = categoryList[i+1];
                break;
            }
        }

        OnWordsActivity(_currentWord);
    }

    #endregion

}
