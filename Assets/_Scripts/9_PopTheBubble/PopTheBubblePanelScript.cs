using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopTheBubblePanelScript : MonoBehaviour
{
    [Header("Singleton Reference")]
    public static PopTheBubblePanelScript Instance;

    [Header("Prefab")]
    [SerializeField] GameObject _bubblePrefab;
    public Transform _bubbleParent;

    [Header("Buttons")]
    [SerializeField] Button _ptbColourBtn;
    [SerializeField] Button _ptbAlphabetsBtn;
    [SerializeField] Button _ptbNumbersBtn;
    [SerializeField] Button _ptbBackButton;

    [Header("TextmeshPro")]
    [SerializeField] TextMeshProUGUI _headerText;
    [SerializeField] TextMeshProUGUI _startSequence;
    [SerializeField] TextMeshProUGUI _scoreText;

    [Header("Canvas Components")]
    public RectTransform CanvasRect;
    private float _canvasWidth;
    private float _canvasHeight;

    [Header("Bubble Components")]
    private Image _selectedbubble;
    private string _selectedChar;
    private Color _selectedColor;
    private float _spawnInterval = 1.5f;
    public float Padding = 150f;
    public float UpwardForce = 150f; // Adjust the force as needed

    [Header("Properties reference")]
    int _score;
    float _bubbleChangeTimer = 30f;

    [Header("Enum")]
    [SerializeField] private AllBubbleStates _bubbleState;

    private enum AllBubbleStates
    {
        ColorBubble,
        AlphabetBubble,
        NumberBubble
    }

    private void Awake()
    {
        Instance = Instance ?? this;  // Setting Singleton Instance
        if (Instance != this) Destroy(gameObject);  // If not Active Singleton, destroy it
        DontDestroyOnLoad(gameObject);  // Ensure that the Singleton persists across scene changes

        // Find and assign references to UI elements
        _bubbleParent = GameObject.Find("Bubble_Parent").transform;
        _ptbColourBtn = GameObject.Find("ptb_Colour_Btn").GetComponent<Button>();
        _ptbAlphabetsBtn = GameObject.Find("ptb_Alphabets_Btn").GetComponent<Button>();
        _ptbNumbersBtn = GameObject.Find("ptb_Numbers_Btn").GetComponent<Button>();
        _ptbBackButton = GameObject.Find("Ptb_BackButton").GetComponent<Button>();
        _headerText = GameObject.Find("ptb_Header_Text").GetComponent<TextMeshProUGUI>();
        _startSequence = GameObject.Find("ptb_StartSequence_Text").GetComponent<TextMeshProUGUI>();
        _scoreText = GameObject.Find("ptb_Score_Text").GetComponent<TextMeshProUGUI>();
        _selectedbubble = GameObject.Find("ptb_Selected_Bubble").GetComponent<Image>();
    }


    private void Start()
    {
        // Load the bubble prefab from the Resources folder
        _bubblePrefab = Resources.Load<GameObject>("Prefabs/Bubble_Prefab");

        // Attach event listeners to buttons
        _ptbColourBtn.onClick.AddListener(() => { OnPtbColoursBtn(); });
        _ptbAlphabetsBtn.onClick.AddListener(() => { OnPtbAphabetsBtn(); });
        _ptbNumbersBtn.onClick.AddListener(() => { OnPtbNumbersBtn(); });
        _ptbBackButton.onClick.AddListener(() => { OnPtbBackButton(); });

        // Initialize the game in a default state
        OnPtbBackButton();

        // Get a reference to the canvas and its dimensions
        CanvasRect = HomeScreenMgrScript.Instance.PopTheBubbleCanvas.GetComponent<RectTransform>();
        _canvasWidth = CanvasRect.rect.width;
        _canvasHeight = CanvasRect.rect.height;
    }


    #region Functions of Buttons

    private void OnPtbColoursBtn()
    {
        _bubbleState = AllBubbleStates.ColorBubble;

        // Activate all child objects within the PopTheBubbleCanvas
        OnActivatePanelComponent(true);

        // Reset the player's score
        _score = 0;
        _scoreText.text = "Score: " + _score;

        //StartCoroutine(OnColorsDeploy());
        OnNewBubbleGameStart();
    }

    private void OnPtbAphabetsBtn()
    {
        _bubbleState = AllBubbleStates.AlphabetBubble;

        // Activate all child objects within the PopTheBubbleCanvas
        OnActivatePanelComponent(true);

        // Reset the player's score
        _score = 0;
        _scoreText.text = "Score: " + _score;

        //StartCoroutine(OnAlphabetDeploy());
        OnNewBubbleGameStart();
    }

    private void OnPtbNumbersBtn()
    {
        _bubbleState = AllBubbleStates.NumberBubble;

        // Activate all child objects within the PopTheBubbleCanvas
        OnActivatePanelComponent(true);

        // Reset the player's score
        _score = 0;
        _scoreText.text = "Score: " + _score;

        //StartCoroutine(OnNumbersDeploy());
        OnNewBubbleGameStart();
    }

    private void OnActivatePanelComponent(bool inputBool)
    {
        // Activate all child objects within the PopTheBubbleCanvas
        foreach (Transform t in HomeScreenMgrScript.Instance.PopTheBubbleCanvas.transform)
        {
            t.gameObject.SetActive(inputBool);
        }
    }

    private void OnPtbBackButton()
    {
        // Deactivate all child objects within the PopTheBubbleCanvas
        OnActivatePanelComponent(false);

        // If there are bubbles in the scene, destroy them
        if (_bubbleParent.childCount > 0)
        {
            foreach (Transform t in _bubbleParent)
            {
                Destroy(t.gameObject);
            }
        }

        // Stop all running coroutines & InvokeRepeating
        StopAllCoroutines();
        CancelInvoke(nameof(ChangingTheBubble));
    }

    #endregion


    #region Bubble Spawn Functions

    private void BubbleSpawn(string bubbleText, Color color)
    //private void BubbleSpawn(string bubbleText, Color color)
    {
        // Set a random X position within the canvas width
        float width = _canvasWidth / 2;
        float randomX = UnityEngine.Random.Range(-(width - 350), width - 150);

        // Set the Y position at the bottom of the canvas
        float randomY = -((_canvasHeight / 2) + Padding); // You can adjust this if you want some padding from the bottom

        int r = Mathf.RoundToInt(color.r * 255);
        int g = Mathf.RoundToInt(color.g * 255);
        int b = Mathf.RoundToInt(color.b * 255);
        int a = Mathf.RoundToInt(color.a * 255);

        // Create an instance of the image prefab
        GameObject newBubble = Instantiate(_bubblePrefab, _bubbleParent);

        if (_bubbleState == AllBubbleStates.ColorBubble)
        {
            newBubble.GetComponent<Image>().color = color;
            newBubble.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = bubbleText;
            newBubble.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 0);
        }
        else
        {
            newBubble.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = bubbleText;
        }

        // Set the position of the instantiated image
        RectTransform imageRect = newBubble.GetComponent<RectTransform>();
        imageRect.anchoredPosition = new Vector2(randomX, randomY);
    }

    public void OnBubbleClicked(string input)
    {
        // Check if the clicked bubble's content matches the expected character
        if (input == _selectedChar)
        {
            // Increase the player's score and update the score display
            _score++;
            _scoreText.text = "Score: " + _score;
        }
    }

    #endregion


    #region Random Selection Functions

    private (string, Color) GetRandomColor()
    {
        // Function to select a random Color
        // Generate a random index within the range of the Color array
        int randomIndex = UnityEngine.Random.Range(0, HomeScreenMgrScript.Instance.BDSO.ColorList.Length);
        string colorName = HomeScreenMgrScript.Instance.BDSO.ColorList[randomIndex];
        string hexCode = HomeScreenMgrScript.Instance.BDSO.HexCodeList[randomIndex];
        Color newColor;
        if (hexCode != null)
        {
            if (ColorUtility.TryParseHtmlString(hexCode, out newColor))
            {
                return (colorName, newColor);
            }
        }

        // Return the Color at the random index
        return (colorName, Color.white);
    }

    (string, Color) GetRandomAlphabet()
    {
        // Function to select a random alphabet
        // Generate a random index within the range of the alphabet array
        string[] alphbetArray = HomeScreenMgrScript.Instance.BDSO.AllAlphabet;
        int randomIndex = UnityEngine.Random.Range(0, alphbetArray.Length);

        // Return the alphabet at the random index
        return (alphbetArray[randomIndex], Color.white);
    }

    (string, Color) GetRandomNumbers()
    {
        // Function to select a random Number
        // Generate a random index within the range of the Number array
        string[] numberArray = HomeScreenMgrScript.Instance.BDSO.NumberList;
        int randomIndex = UnityEngine.Random.Range(0, numberArray.Length);

        // Return the Number at the random index
        return (numberArray[randomIndex], Color.white);
    }


    #endregion


    #region Bubble Selection Functions

    private void OnNewBubbleGameStart()
    {
        NewSelectionInitialize();

        StartCoroutine(StartSequence());

        InvokeRepeating(nameof(ChangingTheBubble), _bubbleChangeTimer, _bubbleChangeTimer);
    }

    private void NewSelectionInitialize()
    {
        switch (_bubbleState)
        {
            case AllBubbleStates.ColorBubble:
                (_selectedChar, _selectedColor) = GetRandomColor(); // Get a random color
                _selectedbubble.color = _selectedColor;
                _selectedbubble.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 0, 0);

                break;
            case AllBubbleStates.AlphabetBubble:
                (_selectedChar, _selectedColor) = GetRandomAlphabet(); // Get a random Alphabet
                _selectedbubble.color = _selectedColor;
                _selectedbubble.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _selectedChar;
                _selectedbubble.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 0, 255);

                break;
            case AllBubbleStates.NumberBubble:
                (_selectedChar, _selectedColor) = GetRandomNumbers(); // Get a random Number
                _selectedbubble.color = _selectedColor;
                _selectedbubble.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _selectedChar;
                _selectedbubble.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 0, 255);

                break;
            default:
                break;
        }

        // Display game instructions
        _headerText.text = "Pop The " + _selectedChar + " Bubble";
    }

    private IEnumerator StartSequence()
    {
        AnimationScript.Instance.PlayAnimation();
        float waitTime = AnimationScript.Instance.AnimationTime;
        yield return new WaitForSeconds(waitTime);

        _startSequence.gameObject.SetActive(true);
        _startSequence.text = "Wait";

        // Countdown before starting the game
        yield return new WaitForSeconds(1);
        _startSequence.text = "3";
        yield return new WaitForSeconds(1);
        _startSequence.text = "2";
        yield return new WaitForSeconds(1);
        _startSequence.text = "1";
        yield return new WaitForSeconds(1);
        _startSequence.text = "Start";

        // Display the player's score and spawn bubbles
        yield return new WaitForSeconds(1);
        _startSequence.gameObject.SetActive(false);

        StartCoroutine(OnSpawnContinuous());
    }

    private IEnumerator OnSpawnContinuous()
    {
        Func<(string, Color)> storedFunction;

        switch (_bubbleState)
        {
            case AllBubbleStates.ColorBubble:

                storedFunction = GetRandomColor;

                break;
            case AllBubbleStates.AlphabetBubble:

                storedFunction = GetRandomAlphabet;

                break;
            case AllBubbleStates.NumberBubble:

                storedFunction = GetRandomNumbers;

                break;
            default:

                storedFunction = GetRandomNumbers;
                break;
        }

        while (true) // Run infinitely
        {
            int randomNumber = UnityEngine.Random.Range(1, 4);

            if (randomNumber == 1)
            {
                BubbleSpawn(_selectedChar, _selectedColor);
            }
            else
            {
                (string newRandomName, Color newRandomColor) = storedFunction();
                BubbleSpawn(newRandomName, newRandomColor);
            }

            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    private void ChangingTheBubble()
    {
        StopAllCoroutines();
        NewSelectionInitialize();
        StartCoroutine(OnSpawnContinuous());
    }

    #endregion


}
