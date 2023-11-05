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
    [SerializeField] TextMeshProUGUI _tmproText;
    [SerializeField] TextMeshProUGUI _tmproScore;

    [Header("Canvas Components")]
    public RectTransform CanvasRect;
    private float _canvasWidth;
    private float _canvasHeight;

    [Header("Bubble Components")]
    private string _selectedChar;
    private float _spawnInterval = 2;
    public float Padding = 150f;
    public float UpwardForce = 150f; // Adjust the force as needed

    [Header("Array;s reference")]
    private Color[] _colours = {Color.black, Color.blue, Color.cyan, Color.gray, Color.green, Color.grey, Color.magenta, Color.red, Color.white, Color.yellow };
    private char[] _alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    private string[] _numbers = {"0","1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","16","17","18","19","20"};

    [Header("Properties reference")]
    int _score;
    int _turn;


    private void Awake()
    {
        Instance = Instance == null ? this : Instance;  // Setting Singleton Instance
        if (Instance != this) Destroy(gameObject);  // If not Active Singleton, destroy it
        DontDestroyOnLoad(gameObject);  // Ensure that the Singleton persists across scene changes

        // Find and assign references to UI elements
        _bubbleParent = GameObject.Find("Bubble_Parent").transform;
        _ptbColourBtn = GameObject.Find("ptb_Colour_Btn").GetComponent<Button>();
        _ptbAlphabetsBtn = GameObject.Find("ptb_Alphabets_Btn").GetComponent<Button>();
        _ptbNumbersBtn = GameObject.Find("ptb_Numbers_Btn").GetComponent<Button>();
        _ptbBackButton = GameObject.Find("Ptb_BackButton").GetComponent<Button>();
        _tmproText = GameObject.Find("ptb_tmpro_Text").GetComponent<TextMeshProUGUI>();
        _tmproScore = GameObject.Find("ptb_tmpro_Text_1").GetComponent<TextMeshProUGUI>();
    }


    void Start()
    {
        // Load the bubble prefab from the Resources folder
        _bubblePrefab = Resources.Load<GameObject>("Bubble_Prefab/Bubble_Prefab");

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
        // Activate all child objects within the PopTheBubbleCanvas
        foreach (Transform t in HomeScreenMgrScript.Instance.PopTheBubbleCanvas.transform)
        {
            t.gameObject.SetActive(true);
        }

        // Initialize game variables for the color mode
        StartCoroutine(OnColorsDeploy());
    }

    private void OnPtbAphabetsBtn()
    {
        // Activate all child objects within the PopTheBubbleCanvas
        foreach (Transform t in HomeScreenMgrScript.Instance.PopTheBubbleCanvas.transform)
        {
            t.gameObject.SetActive(true);
        }

        // Initialize game variables for the alphabets mode
        StartCoroutine(OnAlphabetDeploy());
    }

    private void OnPtbNumbersBtn()
    {
        // Activate all child objects within the PopTheBubbleCanvas
        foreach (Transform t in HomeScreenMgrScript.Instance.PopTheBubbleCanvas.transform)
        {
            t.gameObject.SetActive(true);
        }

        // Initialize game variables for the numbers mode
        StartCoroutine(OnNumbersDeploy());
    }

    private void OnPtbBackButton()
    {
        // Deactivate all child objects within the PopTheBubbleCanvas
        foreach (Transform t in HomeScreenMgrScript.Instance.PopTheBubbleCanvas.transform)
        {
            t.gameObject.SetActive(false);
        }

        // If there are bubbles in the scene, destroy them
        if (_bubbleParent.childCount > 0)
        {
            foreach (Transform t in _bubbleParent)
            {
                Destroy(t.gameObject);
            }
        }

        // Stop all running coroutines
        StopAllCoroutines();
    }

    #endregion


    #region Bubble Functions

    private void ToSpawnBubble(string bubbleText)
    {
        // Set a random X position within the canvas width
        float width = _canvasWidth / 2;
        float randomX = Random.Range(-(width-350), width-150);

        // Set the Y position at the bottom of the canvas
        float randomY = -((_canvasHeight/2) + Padding); // You can adjust this if you want some padding from the bottom

        // Create an instance of the image prefab
        GameObject newBubble = Instantiate(_bubblePrefab, _bubbleParent);
        newBubble.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = bubbleText;

        // Set the position of the instantiated image
        RectTransform imageRect = newBubble.GetComponent<RectTransform>();
        imageRect.anchoredPosition = new Vector2(randomX, randomY);
    }
    
    private void ToSpawnColorBubble(Color input)
    {
        // Set a random X position within the canvas width
        float width = _canvasWidth / 2;
        float randomX = Random.Range(-(width-350), width-150);

        // Set the Y position at the bottom of the canvas
        float randomY = -((_canvasHeight/2) + Padding); // You can adjust this if you want some padding from the bottom

        // Create an instance of the image prefab
        GameObject newBubble = Instantiate(_bubblePrefab, _bubbleParent);
        newBubble.GetComponent<Image>().color = input;
        newBubble.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ConvertColorToString(input);
        newBubble.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 0);

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
            _tmproScore.text = "Score: " + _score;

            // Check if the player has reached a score of 10 to complete the game
            if (_score >= 10)
                Debug.Log("Complete"); // Output a message to indicate completion
                StopAllCoroutines(); // Stop all coroutines to end the game
        }
    }

    #endregion


    #region Colors Selection

    private IEnumerator OnColorsDeploy()
    {
        Color[] _optionsArr = new Color[3]; // Array to store color options
        Color selectedColor = GetRandomColor(); // Get a random color
        _selectedChar = ConvertColorToString(selectedColor); // Set the selected color as the target
        _score = 0; // Reset the player's score
        _turn = 0; // Initialize the turn counter

        // Display game instructions
        _tmproText.text = "Click On The " + _selectedChar + " Bubble";
        _tmproScore.text = "Wait";

        // Countdown before starting the game
        yield return new WaitForSeconds(1);
        _tmproScore.text = "3";
        yield return new WaitForSeconds(1);
        _tmproScore.text = "2";
        yield return new WaitForSeconds(1);
        _tmproScore.text = "1";
        yield return new WaitForSeconds(1);
        _tmproScore.text = "Start";

        // Display the player's score and spawn bubbles
        yield return new WaitForSeconds(1);
        _tmproScore.text = "Score: ";

        while (_score < 10)
        {
            if (_turn == 0)
            {
                // Spawn a bubble with the selected color
                ToSpawnColorBubble(_optionsArr[0]);
                _turn++;
            }
            else if (_turn == 1)
            {
                // Spawn two bubbles with random colors
                ToSpawnColorBubble(_optionsArr[1]);
                yield return new WaitForSeconds(_spawnInterval);
                ToSpawnColorBubble(_optionsArr[2]);
                _turn++;
            }
            else if (_turn == 2)
            {
                // Spawn multiple bubbles with various colors
                ToSpawnColorBubble(_optionsArr[0]);
                yield return new WaitForSeconds(_spawnInterval);
                ToSpawnColorBubble(_optionsArr[0]);
                yield return new WaitForSeconds(_spawnInterval);
                ToSpawnColorBubble(_optionsArr[2]);
                yield return new WaitForSeconds(_spawnInterval);
                ToSpawnColorBubble(_optionsArr[1]);
                _turn = 0;
            }

            // Generate new random colors for the next turn
            _optionsArr[1] = GetRandomColor();
            _optionsArr[2] = GetRandomColor();
            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    Color GetRandomColor()
    {
        // Function to select a random Color
        // Generate a random index within the range of the Color array
        int randomIndex = Random.Range(0, _colours.Length);

        // Return the Color at the random index
        return _colours[randomIndex];
    }

    string ConvertColorToString(Color inputColor)
    {
        // Convert a Color to its corresponding string representation
        if (inputColor == Color.black)
            return "Black";
        else if (inputColor == Color.blue)
            return "Blue";
        else if (inputColor == Color.cyan)
            return "Cyan";
        else if (inputColor == Color.gray)
            return "Gray";
        else if (inputColor == Color.green)
            return "Green";
        else if (inputColor == Color.grey)
            return "Grey";
        else if (inputColor == Color.magenta)
            return "Pink";
        else if (inputColor == Color.red)
            return "Red";
        else if (inputColor == Color.white)
            return "White";
        else if (inputColor == Color.yellow)
            return "Yellow";

        return "Unknown"; // Return "Unknown" if the color doesn't match known colors
    }

    #endregion


    #region Alphabets Selection

    private IEnumerator OnAlphabetDeploy()
    {
        string[] _optionsArr = new string[3]; // Array to store alphabet options
        char selectedLetter = GetRandomAlphabet(); // Get a random alphabet letter
        _selectedChar = selectedLetter.ToString(); // Set the selected letter as the target
        _score = 0; // Reset the player's score
        _turn = 0; // Initialize the turn counter

        // Display game instructions
        _tmproText.text = "Click On Bubble Letter " + _selectedChar;
        _tmproScore.text = "Wait";

        // Countdown before starting the game
        yield return new WaitForSeconds(1);
        _tmproScore.text = "3";
        yield return new WaitForSeconds(1);
        _tmproScore.text = "2";
        yield return new WaitForSeconds(1);
        _tmproScore.text = "1";
        yield return new WaitForSeconds(1);
        _tmproScore.text = "Start";

        // Display the player's score and spawn bubbles with alphabet letters
        yield return new WaitForSeconds(1);
        _tmproScore.text = "Score: ";

        while (_score < 10)
        {
            if (_turn == 0)
            {
                // Spawn a bubble with the selected alphabet letter
                ToSpawnBubble(_optionsArr[0]);
                _turn++;
            }
            else if (_turn == 1)
            {
                // Spawn two bubbles with random alphabet letters
                ToSpawnBubble(_optionsArr[1]);
                yield return new WaitForSeconds(_spawnInterval);
                ToSpawnBubble(_optionsArr[2]);
                _turn++;
            }
            else if (_turn == 2)
            {
                // Spawn multiple bubbles with various alphabet letters
                ToSpawnBubble(_optionsArr[0]);
                yield return new WaitForSeconds(_spawnInterval);
                ToSpawnBubble(_optionsArr[0]);
                yield return new WaitForSeconds(_spawnInterval);
                ToSpawnBubble(_optionsArr[2]);
                yield return new WaitForSeconds(_spawnInterval);
                ToSpawnBubble(_optionsArr[1]);
                _turn = 0;
            }

            // Generate new random alphabet letters for the next turn
            _optionsArr[1] = GetRandomAlphabet().ToString();
            _optionsArr[2] = GetRandomAlphabet().ToString();
            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    char GetRandomAlphabet()
    {
        // Function to select a random alphabet
        // Generate a random index within the range of the alphabet array
        int randomIndex = Random.Range(0, _alphabets.Length);

        // Return the alphabet at the random index
        return _alphabets[randomIndex];
    }

    #endregion


    #region Numbers Selection

    private IEnumerator OnNumbersDeploy()
    {
        string[] _optionsArr = new string[3]; // Array to store number options
        string selected = GetRandomNumbers().ToString(); // Get a random number
        _selectedChar = selected; // Set the selected number as the target
        _score = 0; // Reset the player's score
        _turn = 0; // Initialize the turn counter

        // Display game instructions
        _tmproText.text = "Click On Number Bubble " + _selectedChar;
        _tmproScore.text = "Wait";

        // Countdown before starting the game
        yield return new WaitForSeconds(1);
        _tmproScore.text = "3";
        yield return new WaitForSeconds(1);
        _tmproScore.text = "2";
        yield return new WaitForSeconds(1);
        _tmproScore.text = "1";
        yield return new WaitForSeconds(1);
        _tmproScore.text = "Start";

        // Display the player's score and spawn bubbles with numbers
        yield return new WaitForSeconds(1);
        _tmproScore.text = "Score: ";

        while (_score < 10)
        {
            if (_turn == 0)
            {
                // Spawn a bubble with the selected number
                ToSpawnBubble(_optionsArr[0]);
                _turn++;
            }
            else if (_turn == 1)
            {
                // Spawn two bubbles with random numbers
                ToSpawnBubble(_optionsArr[1]);
                yield return new WaitForSeconds(_spawnInterval);
                ToSpawnBubble(_optionsArr[2]);
                _turn++;
            }
            else if (_turn == 2)
            {
                // Spawn multiple bubbles with various numbers
                ToSpawnBubble(_optionsArr[0]);
                yield return new WaitForSeconds(_spawnInterval);
                ToSpawnBubble(_optionsArr[0]);
                yield return new WaitForSeconds(_spawnInterval);
                ToSpawnBubble(_optionsArr[2]);
                yield return new WaitForSeconds(_spawnInterval);
                ToSpawnBubble(_optionsArr[1]);
                _turn = 0;
            }

            // Generate new random numbers for the next turn
            _optionsArr[1] = GetRandomNumbers().ToString();
            _optionsArr[2] = GetRandomNumbers().ToString();
            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    string GetRandomNumbers()
    {
        // Function to select a random Number
        // Generate a random index within the range of the Number array
        int randomIndex = Random.Range(0, _numbers.Length);

        // Return the Number at the random index
        return _numbers[randomIndex];
    }

    #endregion

}
