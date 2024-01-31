using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BigSmallPanelScript : MonoBehaviour
{
    [Header("Singleton Reference")]
    public static BigSmallPanelScript Instance;

    [Header("TextMeshPro")]
    private TextMeshProUGUI _headerText;

    [Header("Transforms")]
    public Transform DraggerContent;
    public Transform _bigItemDropZone;
    public Transform _smallItemDropZone;

    [Header("Big & Small sibling index Num & Size")]
    public int SmallObjSiblingNum = 0;
    public int BigObjSiblingNum = 0;
    public float SmallHeightWidth = 125f;
    public float BigHeightWidth = 375f;

    [Header("User Selection Bools")]
    public bool isBigImgIdentified;
    public bool isSmallImgIdentified;

    [Header("Next panel & Back button")]
    private GameObject _nextPanel;
    private Button _bsBackBtn;


    private void Awake()
    {
        Instance = Instance ?? this;  // Setting Singleton Instance
        if (Instance != this) Destroy(gameObject);  // If not Active Singleton, destroy it
        DontDestroyOnLoad(gameObject);  // Ensure that the Singleton persists across scene changes

        _headerText = GameObject.Find("BS_Header_Text").GetComponent<TextMeshProUGUI>();
        DraggerContent = GameObject.Find("BS_Dragger_Content").transform;
        _bigItemDropZone = GameObject.Find("Bigger_Item_DropZone").transform;
        _smallItemDropZone = GameObject.Find("Smaller_Item_DropZone").transform;
        _nextPanel = GameObject.Find("BS_Next_Panel");
        _bsBackBtn = GameObject.Find("BS_BigSmall_BackButton").GetComponent<Button>();
    }

    private void Start()
    {
        // Add a click event listener to a button in the next panel
        _nextPanel.transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { _bsBackBtn.onClick.Invoke(); });
        _nextPanel.transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(() => { OnNextBtnSelection(); });
    }

    public void OnInitiateBigSmall()
    {
        AnimationScript.Instance.PlayAnimation();

        // Deactivate the next panel
        _nextPanel.SetActive(false);

        // Reset the identification flags & Grid Layout
        DraggerContent.GetComponent<GridLayoutGroup>().enabled = true;
        isSmallImgIdentified = isBigImgIdentified = false;

        // Getting Random Object
        //string selectedCategory;
        string randomObject = GetRandomObject();

        // Setting the heading text
        _headerText.text = "Identify the Big & Small " + randomObject;

        // Load the random image sprite.
        Sprite bsSprite = Resources.Load<Sprite>("AllObjectImages/" + randomObject);

        // Instantiate image objects based on the random number.
        for (int i = 0; i < DraggerContent.childCount; i++)
        {
            DraggerContent.GetChild(i).GetComponent<Image>().sprite = bsSprite;
        }

        // Assign a random value to small and big objects
        (SmallObjSiblingNum, BigObjSiblingNum) = GetBigSmallSibIndex();

        // Resizing the small and big objects accordingly
        StartCoroutine(ResizingBigSmall());
    }

    public void CheckCurrentObjective()
    {
        if (isSmallImgIdentified && isBigImgIdentified)
            // Activate the next panel if both images are identified
            _nextPanel.SetActive(true);
    }

    private void OnNextBtnSelection()
    {
        // Reset the image colors and enable raycasting
        Color defaulColor = new Color32(255, 255, 255, 255);
        for (int i = 0; i < DraggerContent.childCount; i++)
        {
            DraggerContent.GetChild(i).GetComponent<Image>().color = defaulColor;
            DraggerContent.GetChild(i).GetComponent<CanvasGroup>().blocksRaycasts = true;
        }

        // Reinstating Big Small Text
        _bigItemDropZone.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Big";
        _smallItemDropZone.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Small";

        // Reinitialize the big and small objects
        OnInitiateBigSmall();
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

    private (int, int) GetBigSmallSibIndex()
    {
        int firstIndex = Random.Range(0, DraggerContent.childCount);
        int secondIndex;

        do
        {
            secondIndex = Random.Range(0, DraggerContent.childCount);
        } while (secondIndex == firstIndex);

        return (firstIndex, secondIndex);
    }

    private IEnumerator ResizingBigSmall()
    {
        yield return new WaitForEndOfFrame();
        DraggerContent.GetComponent<GridLayoutGroup>().enabled = false;
        RectTransform smallObjectRect = DraggerContent.GetChild(SmallObjSiblingNum).GetComponent<RectTransform>();
        RectTransform bigObjectRect = DraggerContent.GetChild(BigObjSiblingNum).GetComponent<RectTransform>();
        smallObjectRect.sizeDelta = new Vector2(SmallHeightWidth, SmallHeightWidth);
        bigObjectRect.sizeDelta = new Vector2(BigHeightWidth, BigHeightWidth);
    }
}
