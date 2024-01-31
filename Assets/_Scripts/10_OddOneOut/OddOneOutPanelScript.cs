using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OddOneOutPanelScript : MonoBehaviour
{
    [Header("Singleton Reference")]
    public static OddOneOutPanelScript Instance;

    [Header("TextMeshPro")]
    private TextMeshProUGUI _headerText;

    [Header("Transforms")]
    private Transform _imageHolderTransform;
    private GameObject _nextPanel;
    public int oddObjIndex;
    private int _arrayCount = 4;

    [Header("Back Button")]
    private Button _oouBackBtn;


    private void Awake()
    {
        Instance = Instance ?? this;  // Setting Singleton Instance
        if (Instance != this) Destroy(gameObject);  // If not Active Singleton, destroy it
        DontDestroyOnLoad(gameObject);  // Ensure that the Singleton persists across scene changes

        // Find and set references to various objects
        _headerText = GameObject.Find("Oou_Header_Text").GetComponent<TextMeshProUGUI>();
        _imageHolderTransform = GameObject.Find("Oou_OddEvenImage_Grid").transform;
        _nextPanel = GameObject.Find("Oou_Next_Panel");
        _oouBackBtn = GameObject.Find("Oou_OddOneOut_BackButton").GetComponent<Button>();
    }

    private void Start()
    {
        // Add a click event listener to a button in the next panel
        _nextPanel.transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { _oouBackBtn.onClick.Invoke(); });
        _nextPanel.transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(() => { OnNextBtnSelection(); });
    }

    public void OnOddOneOutInitiate()
    {
        AnimationScript.Instance.PlayAnimation();

        // Getting Random Object
        string selectedCategory;
        string[] selectedArray;
        string oddObject;
        (selectedCategory, selectedArray, oddObject) = GetRandomObject();

        // Setting the heading text
        _headerText.text = "Identify the Odd " + selectedCategory + " Item";

        // Generate a random index for the odd object
        oddObjIndex = Random.Range(0, 9); // Random number from 0 to 8 (inclusive)

        // Set the image to all the object
        LoadImage(selectedArray, oddObjIndex, oddObject);

        // Deactivate the next panel
        _nextPanel.SetActive(false);
    }

    public void CheckCurrentObjective(string input)
    {
        // Generate the expected object name based on the odd object index
        string oddObjName = "Image_" + (oddObjIndex + 1);

        // Check if the input matches the expected object name
        if (input == oddObjName)
            // Activate the next panel
            _nextPanel.SetActive(true);
    }

    private void OnNextBtnSelection()
    {
        // Deactivate the next panel and reinitialize the odd one out activity
        _nextPanel.SetActive(false);
        OnOddOneOutInitiate();
    }

    private (string, string[], string) GetRandomObject()
    {
        int firstIndex = Random.Range(0, _arrayCount);
        int secondIndex;

        do
        {
            secondIndex = Random.Range(0, _arrayCount);
        } while (secondIndex == firstIndex);

        string[] selectedArray;
        string arrayName;
        (arrayName, selectedArray) = GetSOArray(firstIndex);

        // Shuffling & Reducing the size to equal childcount
        List<string> newList = reducingListSize(selectedArray);
        newList = ShuffleList(newList);

        string[] oddObjArray;
        string oddObjName;
        (oddObjName, oddObjArray) = GetSOArray(secondIndex);

        int randomIndex = Random.Range(0, oddObjArray.Length);
        string oddObjName2 = oddObjArray[randomIndex];

        return (arrayName, newList.ToArray(), oddObjName2);
    }

    private (string, string[]) GetSOArray(int index)
    {
        string arrayName;
        string[] selectedArray;

        switch (index)
        {
            case 0:
                selectedArray = HomeScreenMgrScript.Instance.BDSO.FoodList;
                arrayName = "Food";
                break;
            case 1:
                selectedArray = HomeScreenMgrScript.Instance.BDSO.AnimalList;
                arrayName = "Animal";
                break;
            case 2:
                selectedArray = HomeScreenMgrScript.Instance.BDSO.FruitList;
                arrayName = "Fruit";
                break;
            case 3:
                selectedArray = HomeScreenMgrScript.Instance.BDSO.ExtrasList;
                arrayName = "Object";
                break;
            default:
                selectedArray = HomeScreenMgrScript.Instance.BDSO.FoodList;
                arrayName = "Food";
                break; // In case something goes wrong
        }

        return (arrayName, selectedArray);
    }

    private List<string> reducingListSize(string[] inputArray)
    {
        List<string> list = new List<string>(inputArray);
        while (list.Count > _imageHolderTransform.childCount)
        {
            int randomIndex = Random.Range(0, list.Count);
            list.RemoveAt(randomIndex);
        }
        return list;
    }

    // Fisher-Yates shuffle algorithm to shuffle a list
    private List<string> ShuffleList(List<string> inputList)
    {
        List<string> shuffledList = new List<string>(inputList);
        int n = shuffledList.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            string temp = shuffledList[i];
            shuffledList[i] = shuffledList[j];
            shuffledList[j] = temp;
        }
        return shuffledList;
    }

    private void LoadImage(string[] inputArray, int oddObjIndex, string oddObjName)
    {
        for (int i = 0; i < _imageHolderTransform.childCount; i++)
        {
            Sprite sprite = Resources.Load<Sprite>("AllObjectImages/" + inputArray[i]);
            Image image = _imageHolderTransform.GetChild(i).GetComponent<Image>();
            image.sprite = sprite ?? sprite;
        }

        Sprite sprite2 = Resources.Load<Sprite>("AllObjectImages/" + oddObjName);
        Image image2 = _imageHolderTransform.GetChild(oddObjIndex).GetComponent<Image>();
        image2.sprite = sprite2 ?? sprite2;
    }
}
