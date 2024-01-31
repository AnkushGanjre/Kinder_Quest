using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MagicCountPanelScript : MonoBehaviour
{
    [Header("Singleton Reference")]
    public static MagicCountPanelScript Instance;

    [Header("UI Elements")]
    private TextMeshProUGUI _headerText;
    private GameObject _imagePrefab;
    private Transform _imageGridTransform;
    private Transform _macOptionsParent;
    private Button _macBackBtn;
    private GameObject _nextPanel;

    [Header("Options Button Variables")]
    [SerializeField] int _currentCorrectOption;
    [SerializeField] Button[] _optionsButtons = new Button[3];
    [SerializeField] int[] _usedNumbers = new int[2]; // Store the used numbers for the remaining options


    private void Awake()
    {
        Instance = Instance ?? this;  // Setting Singleton Instance
        if (Instance != this) Destroy(gameObject);  // If not Active Singleton, destroy it
        DontDestroyOnLoad(gameObject);  // Ensure that the Singleton persists across scene changes

        // Find and assign references to UI elements.
        _headerText = GameObject.Find("Mac_Count_Text").GetComponent<TextMeshProUGUI>();
        _imageGridTransform = GameObject.Find("Mac_ImageCount_Grid").transform;
        _macOptionsParent = GameObject.Find("Mac_Options_Parent").transform;
        _macBackBtn = GameObject.Find("Mac_MagicCount_BackButton").GetComponent<Button>();
        _nextPanel = GameObject.Find("Mac_Next_Panel");
    }

    private void Start()
    {
        // Load the image prefab and set up UI event listeners.
        _imagePrefab = Resources.Load<GameObject>("Prefabs/Count_Prefab");
        _macBackBtn.onClick.AddListener(() => { OnMacBackButton(); });
        _nextPanel.transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { _macBackBtn.onClick.Invoke(); });
        _nextPanel.transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(() => { OnNextBtnSelection(); });
        
        // Initialize the options buttons.
        for (int i = 0; i < _optionsButtons.Length; i++)
        {
            _optionsButtons[i] = _macOptionsParent.GetChild(i).GetComponent<Button>();
        }
    }

    public void OnMagicCountInitiate()
    {
        AnimationScript.Instance.PlayAnimation();

        // Generate a random number for the correct option.
        _currentCorrectOption = Random.Range(1, 21);

        // Getting Random Object
        string randomObject = GetRandomObject();

        // Setting the heading text
        _headerText.text = "How many " + randomObject + " are there?";

        // Load the random image sprite.
        Sprite countSprite = Resources.Load<Sprite>("AllObjectImages/" + randomObject);

        // Instantiate image objects based on the selected random number.
        for (int i = 0; i < _currentCorrectOption; i++)
        {
            GameObject gm = Instantiate(_imagePrefab, _imageGridTransform);
            gm.GetComponent<Image>().sprite = countSprite;
        }

        // Assign values to the buttons for the multiple choice options.
        OnAssignValueToButton(_currentCorrectOption);

        // Hide the next panel (assuming it's initially hidden).
        _nextPanel.SetActive(false);
    }


    #region Supportive Function

    private void OnAssignValueToButton(int randomNumber)
    {
        // Assign the random number to one of the buttons
        int randomButtonIndex = Random.Range(0, _optionsButtons.Length);
        _optionsButtons[randomButtonIndex].GetComponentInChildren<TextMeshProUGUI>().text = randomNumber.ToString();

        // Generate unique random numbers for the other two buttons
        for (int i = 0; i < _optionsButtons.Length; i++)
        {
            if (i != randomButtonIndex)
            {
                int a = i;
                int uniqueNumber;
                do
                {
                    uniqueNumber = Random.Range(2, 21);
                } while (ArrayContains(_usedNumbers, uniqueNumber) || uniqueNumber == randomNumber);
                a--;
                if (a < 0)
                {
                    _usedNumbers[0] = uniqueNumber;
                }
                else if (_usedNumbers[a] == 0)
                {
                    _usedNumbers[a] = uniqueNumber;
                }
                else
                {
                    _usedNumbers[a + 1] = uniqueNumber;
                }
                _optionsButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = uniqueNumber.ToString();
            }
        }

        // Add click listeners to the option buttons.
        for (int i = 0; i < _optionsButtons.Length; i++)
        {
            int a = i;
            string currentOption = _optionsButtons[a].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
            _optionsButtons[a].onClick.RemoveAllListeners();
            _optionsButtons[a].onClick.AddListener(() => { OnOptionSelection(int.Parse(currentOption)); });
        }

    }

    private bool ArrayContains(int[] array, int value)
    {
        // Helper function to check if an array contains a specific value
        foreach (int item in array)
        {
            if (item == value)
                return true;
        }
        return false;
    }

    private string GetRandomObject()
    {
        int arrayIndex = Random.Range(1, 5);
        string[] selectedArray;

        switch (arrayIndex)
        {
            case 1:
                selectedArray = HomeScreenMgrScript.Instance.BDSO.FoodList;
                break;
            case 2:
                selectedArray = HomeScreenMgrScript.Instance.BDSO.AnimalList;
                break;
            case 3:
                selectedArray = HomeScreenMgrScript.Instance.BDSO.FruitList;
                break;
            case 4:
                selectedArray = HomeScreenMgrScript.Instance.BDSO.ExtrasList;
                break;
            default:
                selectedArray = HomeScreenMgrScript.Instance.BDSO.FoodList;
                break; // In case something goes wrong
        }

        int randomIndex = Random.Range(0, selectedArray.Length);
        return selectedArray[randomIndex];
    }

    #endregion


    #region Function for Button

    private void OnOptionSelection(int input)
    {
        if (input == _currentCorrectOption)
        {
            // The correct option has been selected.
            // Activate the next panel to proceed.
            _nextPanel.SetActive(true);
        }
        else
        {
            // The wrong option was selected. Handle this scenario.
            Debug.Log("Wrong, Try Again");
        }
    }

    private void OnNextBtnSelection()
    {
        // Handle the action when the "Next" button is clicked.
        // Typically used for transitioning to the next task or resetting the current task.
        OnMacBackButton();  // Call the function to reset the current task.
        OnMagicCountInitiate();  // Start a new task.
    }


    [ContextMenu("Back Btn")]
    private void OnMacBackButton()
    {
        // Handle the action when the "Back" button is clicked in the Magic Count panel.
        // This method is responsible for resetting the current task and cleaning up.
        if (_imageGridTransform.childCount > 0)
        {
            // Destroy any remaining image objects from the previous task.
            foreach (Transform t in _imageGridTransform)
            {
                Destroy(t.gameObject);
            }
        }

        // Reset the used numbers for the multiple choice options.
        _usedNumbers[0] = 0;
        _usedNumbers[1] = 0;
    }
    #endregion

}
