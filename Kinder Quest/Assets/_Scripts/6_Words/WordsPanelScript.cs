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
    [SerializeField] Transform _wrdContentTransform;
    [SerializeField] Transform _wholeWordTransform;
    [SerializeField] Transform _jumbledWordTransform;

    [Header("Prefabs")]
    [SerializeField] GameObject _wholeWordPrefab;

    [Header("Sub Category Array")]
    [SerializeField] string[] _foodsItem = { "Pizza", "Burger", "Pasta", "Candy", "Donut", "Bread", "Fries", "Cookies", "Noodle", "Chips", "Cake", "Pie" };
    [SerializeField] string[] _fruitsItem = { "Mango", "Apple", "Banana", "Grapes", "Peach", "Orange", "Cherry", "Papaya", "Pear", "Guava", "Berry", "Kiwi" };
    [SerializeField] string[] _animalsItem = { "Dog", "Cat", "Hen", "Bear", "Lion", "Tiger", "Wolf", "Deer", "Cow", "Monkey", "Horse", "Zebra" };
    [SerializeField] string[] _colorsItem = { "Red", "Blue", "Green", "Yellow", "Orange", "Pink", "Brown", "Black", "White", "Gray", "Silver", "Gold" };

    [Header("Array For Jumbling")]
    [SerializeField] private List<Sprite> _letterSprites = new List<Sprite>();
    [SerializeField] private List<Transform> _selectedJumbledLtr = new List<Transform>();
    [SerializeField] private List<Transform> _allJumbledLtr = new List<Transform>();

    [Header("Next panel Reference")]
    private GameObject _nextPanel;


    private void Awake()
    {
        Instance = Instance == null ? this : Instance;  // Setting Singleton Instance
        if (Instance != this) Destroy(gameObject);  // If not Active Singleton, destroy it
        DontDestroyOnLoad(gameObject);  // Ensure that the Singleton persists across scene changes

        // Find and initialize references to various components
        _wrdContentTransform = GameObject.Find("Wrd_Panel_Content").transform;
        _wholeWordTransform = GameObject.Find("Wor_Whole_Word").transform;
        _jumbledWordTransform = GameObject.Find("Wor_Jumbled_Letter").transform;
        _nextPanel = GameObject.Find("Wor_Final_Image");
    }

    void Start()
    {
        // Set up click listeners for word categories
        Transform wrdPanel = HomeScreenMgrScript.Instance.ActivityCanvas.transform.GetChild(3);

        // Attach click listeners to category buttons
        wrdPanel.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { OnWrdFoodsBtn(); });
        wrdPanel.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { OnWrdFruitsBtn(); });
        wrdPanel.GetChild(3).GetComponent<Button>().onClick.AddListener(() => { OnWrdAnimalsBtn(); });
        wrdPanel.GetChild(4).GetComponent<Button>().onClick.AddListener(() => { OnWrdColorBtn(); });
        

        // Attach a click listener to the "Next" button
        _nextPanel.transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(() => { OnNextBtnSelection(); });

        // Attach a click listener to the back button
        Button backBtn = HomeScreenMgrScript.Instance.DragDropCanvas.transform.GetChild(0).GetChild(0).GetComponent<Button>();
        backBtn.onClick.AddListener(() => { OnBackBtnSelection(); });

        // Load the whole word prefab
        _wholeWordPrefab = Resources.Load<GameObject>("Words_Prefab/Whole_Wrd_Img");

        // Initialize the panel with the "Foods" category
        OnWrdFoodsBtn();
    }


    #region Main Button's Function

    public void OnWrdFoodsBtn()
    {
        // This method is called when the "Food" button is clicked
        // Populate the word content with food-related words & Change Btn Sprite

        Sprite yellowBtn = TracingMgrScript.Instance.YellowBtnSprite;
        for (int i = 0; i < _foodsItem.Length; i++)
        {
            _wrdContentTransform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = _foodsItem[i];
            _wrdContentTransform.GetChild(i).GetComponent<Image>().sprite = yellowBtn;
        }

        // Start a coroutine to assign click functions to word buttons
        StartCoroutine(OnAssignSubBtnFunction());
    }

    public void OnWrdFruitsBtn()
    {
        // This method is called when the "Fruits" button is clicked
        // Populate the word content with fruit-related words

        Sprite yellowBtn = TracingMgrScript.Instance.OrangeBtnSprite;
        for (int i = 0; i < _fruitsItem.Length; i++)
        {
            _wrdContentTransform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = _fruitsItem[i];
            _wrdContentTransform.GetChild(i).GetComponent<Image>().sprite = yellowBtn;
        }

        // Start a coroutine to assign click functions to word buttons
        StartCoroutine(OnAssignSubBtnFunction());
    }

    public void OnWrdAnimalsBtn()
    {
        // This method is called when the "Animals" button is clicked
        // Populate the word content with animal-related words

        Sprite blueBtn = TracingMgrScript.Instance.BlueBtnSprite;
        for (int i = 0; i < _animalsItem.Length; i++)
        {
            _wrdContentTransform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = _animalsItem[i];
            _wrdContentTransform.GetChild(i).GetComponent<Image>().sprite = blueBtn;
        }

        // Start a coroutine to assign click functions to word buttons
        StartCoroutine(OnAssignSubBtnFunction());
    }

    public void OnWrdColorBtn()
    {
        // This method is called when the "Color" button is clicked
        // Populate the word content with color-related words

        Sprite greenBtn = TracingMgrScript.Instance.GreenBtnSprite;
        for (int i = 0; i < _colorsItem.Length; i++)
        {
            _wrdContentTransform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = _colorsItem[i];
            _wrdContentTransform.GetChild(i).GetComponent<Image>().sprite = greenBtn;
        }

        // Start a coroutine to assign click functions to word buttons
        StartCoroutine(OnAssignSubBtnFunction());
    }

    #endregion


    #region Sub Button Function

    private void OnWordsActivity(string input)
    {
        // Hide the next panel (if it's visible) and show the drawing canvas
        _nextPanel.SetActive(false);
        HomeScreenMgrScript.Instance.DragDropCanvas.transform.GetChild(0).gameObject.SetActive(true);

        // Hide the jumbled word panel and instantiate the whole word images
        _jumbledWordTransform.gameObject.SetActive(false);
        foreach (char letter in input)
        {
            GameObject go = Instantiate(_wholeWordPrefab, _wholeWordTransform);
            Sprite sprite = Resources.Load<Sprite>("Words_Prefab/All_Letters/" + letter);
            _letterSprites.Add(sprite);
            go.GetComponent<Image>().sprite = sprite;
        }

        // Invoke methods to jumble and initiate jumbled letters
        Invoke("OnJumbleLetterSelect", 2f);
        Invoke("OnInitiateJumbledLtr", 5f);
    }

    private void OnInitiateJumbledLtr()
    {
        // Show the jumbled word panel
        _jumbledWordTransform.gameObject.SetActive(true);

        // Assign Jumbled Letter Sprites to the selected jumbled letter positions
        for (int i = 0; i < _letterSprites.Count; i++)
        {
            _selectedJumbledLtr[i].GetComponent<Image>().sprite = _letterSprites[i];
            _selectedJumbledLtr[i].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            _selectedJumbledLtr[i].GetComponent<CanvasGroup>().blocksRaycasts = true;
        }

        // Reduce the alpha of letters in the whole word panel
        foreach (Transform t in _wholeWordTransform)
        {
            t.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
        }
    }

    private void OnJumbleLetterSelect()
    {
        // Populate the list of child transforms.
        foreach (Transform child in _jumbledWordTransform)
        {
            _allJumbledLtr.Add(child);
        }

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
        StopCoroutine(OnAssignSubBtnFunction());

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
        // Hide the "Next" panel to allow the user to continue the activity
        _nextPanel.SetActive(false);

        // Additional functionality or actions can be added here when the "Next" button is clicked.
        // For example, you can transition to the next part of the activity or perform other relevant tasks.
    }

    #endregion

}
