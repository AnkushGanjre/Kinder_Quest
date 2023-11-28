using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchingPanelScript : MonoBehaviour
{
    [Header("Singleton Reference")]
    public static MatchingPanelScript Instance;

    [Header("Buttons")]
    [SerializeField] Button _mthLetterBtn;
    [SerializeField] Button _mthShapesBtn;
    [SerializeField] Button _mthAnimalsBtn;
    [SerializeField] Button _mthColorBtn;

    [Header("Transform")]
    public Transform UpperPanel;
    [SerializeField] Transform _lowerPanel;

    [Header("Next panel & Back Button")]
    private GameObject _nextPanel;
    private Button _mthBackBtn;

    [Header("Matching List")]
    public List<string> UpperMatchList = new List<string>();
    public List<string> LowerMatchList = new List<string>();

    [SerializeField] string _currentCategory;

    private void Awake()
    {
        Instance = Instance ?? this;  // Setting Singleton Instance
        if (Instance != this) Destroy(gameObject);  // If not Active Singleton, destroy it
        DontDestroyOnLoad(gameObject);  // Ensure that the Singleton persists across scene changes

        // Find and initialize references to buttons and the next panel
        _mthLetterBtn = GameObject.Find("mth_Letters_Btn").GetComponent<Button>();
        _mthShapesBtn = GameObject.Find("mth_Shapes_Btn").GetComponent<Button>();
        _mthAnimalsBtn = GameObject.Find("mth_Animals_Btn").GetComponent<Button>();
        _mthColorBtn = GameObject.Find("mth_Color_Btn").GetComponent<Button>();
        _nextPanel = GameObject.Find("Mat_Next_Panel");
        _mthBackBtn = GameObject.Find("Mat_Matching_BackButton").GetComponent<Button>();

        UpperPanel = GameObject.Find("Mat_Upper_Part").transform;
        _lowerPanel = GameObject.Find("Mat_Lower_Part").transform;
    }

    void Start()
    {
        // Set click event handlers for buttons
        _mthLetterBtn.onClick.AddListener(() => { OnMthLetterBtn(); });
        _mthShapesBtn.onClick.AddListener(() => { OnMthShapesBtn(); });
        _mthAnimalsBtn.onClick.AddListener(() => { OnMthAnimalsBtn(); });
        _mthColorBtn.onClick.AddListener(() => { OnMthColorBtn(); });

        // Add a click listener to the next button in the panel
        _nextPanel.transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { _mthBackBtn.onClick.Invoke(); });
        _nextPanel.transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(() => { OnNextBtnSelection(); });
    }


    #region Button Functions


    private void OnMthLetterBtn()
    {
        _currentCategory = "Letter";

        // Handle "Letters" button click
        OnResetComponents();

        HomeScreenMgrScript.Instance.DragDropCanvas.transform.GetChild(1).gameObject.SetActive(true);
        List<string> upperPanelList = SelectFiveRandomItems(HomeScreenMgrScript.Instance.BDSO.AllAlphabet);
        List<string> lowerPanelList = new List<string>();

        for (int i = 0; i < upperPanelList.Count; i++)
        {
            lowerPanelList.Add(upperPanelList[i]);
        }

        UpperMatchList = lowerPanelList;
        lowerPanelList = ShuffleList(lowerPanelList);
        LowerMatchList = lowerPanelList;
        LoadingLetterAssets(upperPanelList, lowerPanelList);
    }

    private void OnMthShapesBtn()
    {
        _currentCategory = "Shape";

        // Handle "Shapes" button click
        OnResetComponents();
        HomeScreenMgrScript.Instance.DragDropCanvas.transform.GetChild(1).gameObject.SetActive(true);
        List<string> mainShapeList = new List<string>(HomeScreenMgrScript.Instance.BDSO.ShapeList);
        List<string> mainObjectList = new List<string>(HomeScreenMgrScript.Instance.BDSO.ShapeObjList);
        List<string> shapeList = SelectFiveRandomItems(HomeScreenMgrScript.Instance.BDSO.ShapeList);
        List<string> objectShapeList = new List<string>();

        for (int i = 0; i < shapeList.Count; i++)
        {
            for (int j = 0; j < mainShapeList.Count; j++)
            {
                if (shapeList[i] == mainShapeList[j])
                {
                    objectShapeList.Add(mainObjectList[j]);
                }
            }
        }

        UpperMatchList = objectShapeList;
        objectShapeList = ShuffleList(objectShapeList);
        LowerMatchList = objectShapeList;
        LoadingShapeAssets(shapeList, objectShapeList);
    }

    private void OnMthAnimalsBtn()
    {
        _currentCategory = "Animal";

        // Handle "Animals" button click
        OnResetComponents();
        HomeScreenMgrScript.Instance.DragDropCanvas.transform.GetChild(1).gameObject.SetActive(true);
        List<string> animalList = SelectFiveRandomItems(HomeScreenMgrScript.Instance.BDSO.AnimalList);
        List<string> childAnimalList = new List<string>();

        for (int i = 0; i < animalList.Count; i++)
        {
            childAnimalList.Add(animalList[i]);
        }

        UpperMatchList = childAnimalList;
        childAnimalList = ShuffleList(childAnimalList);
        LowerMatchList = childAnimalList;
        LoadingAnimalAssets(animalList, childAnimalList);
    }

    private void OnMthColorBtn()
    {
        _currentCategory = "Color";

        // Handle "Color" button click
        OnResetComponents();
        HomeScreenMgrScript.Instance.DragDropCanvas.transform.GetChild(1).gameObject.SetActive(true);
        List<string> mainColorList = new List<string>(HomeScreenMgrScript.Instance.BDSO.ColorList);
        List<string> mainObjectList = new List<string>(HomeScreenMgrScript.Instance.BDSO.ColorObjList);
        List<string> mainHexCodeList = new List<string>(HomeScreenMgrScript.Instance.BDSO.HexCodeList);
        List<string> colorList = SelectFiveRandomItems(HomeScreenMgrScript.Instance.BDSO.ColorList);
        List<string> objectColorList = new List<string>();
        List<string> hexCodeList = new List<string>();

        for (int i = 0; i < colorList.Count; i++)
        {
            for (int j = 0; j < mainColorList.Count; j++)
            {
                if (colorList[i] == mainColorList[j])
                {
                    objectColorList.Add(mainObjectList[j]);
                    hexCodeList.Add(mainHexCodeList[j]);
                }
            }
        }

        UpperMatchList = objectColorList;
        objectColorList = ShuffleList(objectColorList);
        LowerMatchList = objectColorList;
        LoadColorAssets(colorList, objectColorList, hexCodeList);
    }

    #endregion


    #region Supportive Function

    private void OnResetComponents()
    {
        AnimationScript.Instance.PlayAnimation();

        for (int i = 0; i < UpperPanel.childCount; i++)
        {
            UpperPanel.GetChild(i).GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            _lowerPanel.GetChild(i).GetComponent<Image>().color = new Color32(255, 255, 255, 255);

            UpperPanel.GetChild(i).GetComponent<CanvasGroup>().blocksRaycasts = true;
            _lowerPanel.GetChild(i).GetComponent<CanvasGroup>().blocksRaycasts = true;
        }

        UpperMatchList.Clear();
        LowerMatchList.Clear();
        _nextPanel.SetActive(false);
    }

    public void CheckMatchingProgress()
    {
        // Check if all upper panel images have been matched (blocksRaycasts are set to false)
        for (int i = 0; i < UpperPanel.childCount; i++)
        {
            if (UpperPanel.GetChild(i).GetComponent<CanvasGroup>().blocksRaycasts == true)
            {
                return; // If any image is not matched, exit the function
            }
        }

        // If all upper panel images have been matched, activate the next panel
        _nextPanel.SetActive(true);
    }

    private List<string> SelectFiveRandomItems(string[] inputList)
    {
        List<string> list = new List<string>(inputList);
        while (list.Count > UpperPanel.childCount)
        {
            int randomIndex = Random.Range(0, list.Count);
            list.RemoveAt(randomIndex);
        }
        return list;
    }

    private void LoadingLetterAssets(List<string> upPanelList, List<string> lowPanelList)
    {
        for (int i = 0; i < UpperPanel.childCount; i++)
        {
            Sprite sprite1 = Resources.Load<Sprite>("Letters/Capital_Letters/" + upPanelList[i].ToLower());
            Sprite sprite2 = Resources.Load<Sprite>("Letters/Small_Letters/" + lowPanelList[i].ToLower());

            UpperPanel.GetChild(i).GetComponent<Image>().sprite = sprite1;
            _lowerPanel.GetChild(i).GetComponent<Image>().sprite = sprite2;
        }
    }

    private void LoadingAnimalAssets(List<string> upPanelList, List<string> lowPanelList)
    {
        for (int i = 0; i < UpperPanel.childCount; i++)
        {
            Sprite sprite1 = Resources.Load<Sprite>("AllObjectImages/" + upPanelList[i]);
            Sprite sprite2 = Resources.Load<Sprite>("Child_Animals/" + lowPanelList[i]);

            UpperPanel.GetChild(i).GetComponent<Image>().sprite = sprite1;
            _lowerPanel.GetChild(i).GetComponent<Image>().sprite = sprite2;
        }
    }

    private void LoadingShapeAssets(List<string> upPanelList, List<string> lowPanelList)
    {
        for (int i = 0; i < UpperPanel.childCount; i++)
        {
            Sprite sprite1 = Resources.Load<Sprite>("Shapes/" + upPanelList[i]);
            Sprite sprite2 = Resources.Load<Sprite>("AllObjectImages/" + lowPanelList[i]);

            UpperPanel.GetChild(i).GetComponent<Image>().sprite = sprite1;
            _lowerPanel.GetChild(i).GetComponent<Image>().sprite = sprite2;
        }
    }

    private void LoadColorAssets(List<string> colorList, List<string> lowPanelList, List<string> hexCodeList)
    {
        Sprite sprite1 = Resources.Load<Sprite>("Circle");
        for (int i = 0; i < UpperPanel.childCount; i++)
        {
            Sprite sprite2 = Resources.Load<Sprite>("AllObjectImages/" + lowPanelList[i]);
            _lowerPanel.GetChild(i).GetComponent<Image>().sprite = sprite2;

            Image circleImage = UpperPanel.GetChild(i).GetComponent<Image>();

            Color newColor;
            if (ColorUtility.TryParseHtmlString(hexCodeList[i], out newColor))
            {
                circleImage.color = newColor;
                circleImage.sprite = sprite1;
            }
        }

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

    #endregion


    private void OnNextBtnSelection()
    {
        switch (_currentCategory)
        {
            case "Letter":
                OnMthLetterBtn();
                return;
            case "Shape":
                OnMthShapesBtn();
                return;
            case "Animal":
                OnMthAnimalsBtn();
                return;
            case "Color":
                OnMthColorBtn();
                return;
            default: return;
        }
    }
}
