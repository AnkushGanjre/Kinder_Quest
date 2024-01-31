using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiHandlerScript : MonoBehaviour
{
    [Header("Singleton Reference")]
    public static UiHandlerScript Instance;

    [Header("UI Elements")]
    #region
    [SerializeField] Transform _canvas;
    [SerializeField] GameObject[] _backButton;
    [SerializeField] Button _settingsBtn;
    [SerializeField] Button _settLoginBtn;
    #endregion

    [Header("Profile Panel Buttons")]
    #region
    [SerializeField] Button _profileBtn;
    [SerializeField] Button _paymentsBtn;
    [SerializeField] Button _supportBtn;
    #endregion

    [Header("Other Settings Buttons")]
    #region
    [SerializeField] Button _shareAppBtn;
    [SerializeField] Button _aboutUsBtn;
    [SerializeField] Button _rateUsBtn;
    [SerializeField] Button _privacyPolicyBtn;
    [SerializeField] Button _notificationPrefBtn;
    [SerializeField] Button _userPgRegisterBtn;
    #endregion

    string _urlLink = "https://www.google.com/";


    private void Awake()
    {
        OnAwakeEvents();
    }

    private void Start()
    {
        OnStartEvents();
        OnUserAuthentication();
    }

    #region Lifecycle Events

    private void OnAwakeEvents()
    {
        Instance = Instance ?? this;  // Setting Singleton Instance
        if (Instance != this) Destroy(gameObject);  // If not Active Singleton, destroy it
        DontDestroyOnLoad(gameObject);  // Ensure that the Singleton persists across scene changes

        // Finding Gameobject References
        _canvas = GameObject.Find("Canvas").GetComponent<Transform>();
        _backButton = GameObject.FindGameObjectsWithTag("BackButton");
        _settingsBtn = GameObject.Find("Settings_Button").GetComponent<Button>();
        _settLoginBtn = GameObject.Find("Sett_Logging_Button").GetComponent<Button>();

        #region Profile Panel
        _profileBtn = GameObject.Find("Profile_Btn").GetComponent<Button>();
        _paymentsBtn = GameObject.Find("Payments_Btn").GetComponent<Button>();
        _supportBtn = GameObject.Find("Support_Btn").GetComponent<Button>();
        #endregion

        #region Other Settings
        _shareAppBtn = GameObject.Find("Share_The_App_Btn").GetComponent<Button>();
        _aboutUsBtn = GameObject.Find("About_Us_Btn").GetComponent<Button>();
        _rateUsBtn = GameObject.Find("Rate_Us_Btn").GetComponent<Button>();
        _privacyPolicyBtn = GameObject.Find("Privacy_Policy_Btn").GetComponent<Button>();
        _notificationPrefBtn = GameObject.Find("Notification_Preference_Btn").GetComponent<Button>();
        _userPgRegisterBtn = GameObject.Find("User_Register_Btn").GetComponent<Button>();
        #endregion
    }

    private void OnStartEvents()
    {
        // Adding Button listeners
        _settingsBtn.onClick.AddListener(() => { OnSettingsBtnClick(); });
        _settLoginBtn.onClick.AddListener(() => { OnLoginPageOpen(); });

        #region Profile Panel
        _profileBtn.onClick.AddListener(() => { OnProfileBtnClick(); });
        _paymentsBtn.onClick.AddListener(() => { OnPaymentsBtnClick(); });
        _supportBtn.onClick.AddListener(() => { OnSupportBtnClick(); });
        #endregion

        #region Other Settings
        _shareAppBtn.onClick.AddListener(() => { OnShareAppBtnClick(); });
        _aboutUsBtn.onClick.AddListener(() => { OnAboutUsBtnClick(); });
        _rateUsBtn.onClick.AddListener(() => { OnRateUsBtnClick(); });
        _privacyPolicyBtn.onClick.AddListener(() => { OnPrivacyPolicyBtnClick(); });
        _notificationPrefBtn.onClick.AddListener(() => { OnNotificationPrefBtnClick(); });
        _userPgRegisterBtn.onClick.AddListener(() => { OnRegisterPageOpen(); });
        #endregion

        // Assigning Back Button Function
        for (int i = 0; i < _backButton.Length; i++)
        {
            int a = i;
            _backButton[a].GetComponent<Button>().onClick.AddListener(delegate
            {
                _backButton[a].transform.parent.gameObject.SetActive(false);
            });
        }

        // Turning Off Gameobjects
        _canvas.GetChild(1).gameObject.SetActive(false);
        _canvas.GetChild(2).gameObject.SetActive(false);
        _canvas.GetChild(3).gameObject.SetActive(false);
        _canvas.GetChild(4).gameObject.SetActive(false);
        _canvas.GetChild(1).GetChild(4).gameObject.SetActive(false);
        _canvas.GetChild(1).GetChild(5).gameObject.SetActive(false);
        _canvas.GetChild(1).GetChild(6).gameObject.SetActive(false);
        _canvas.GetChild(1).GetChild(7).gameObject.SetActive(false);
        _canvas.GetChild(1).GetChild(8).gameObject.SetActive(false);
    }

    #endregion

    #region Initialization

    public void OnUserAuthentication()
    {
        // Change User Info if user Exists.
        if (UserProfileScript.Instance._isUserExists)
        {
            // Display Name & DP on user Info
            Image userInfoImage = _canvas.GetChild(1).GetChild(1).GetChild(0).GetComponent<Image>();
            TextMeshProUGUI userNameText = _canvas.GetChild(1).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();

            userInfoImage.sprite = (UserProfileScript.Instance._userDP != null) ? UserProfileScript.Instance._userDP : userInfoImage.sprite;
            userNameText.text = (UserProfileScript.Instance._userName != null) ? UserProfileScript.Instance._userName : userNameText.text;

            // set active true Name text
            _canvas.GetChild(1).GetChild(1).GetChild(1).gameObject.SetActive(true);

            // set active false Log in Button
            _canvas.GetChild(1).GetChild(1).GetChild(2).gameObject.SetActive(false);
        }
        else
        {
            // Display default DP on user Info
            _canvas.GetChild(1).GetChild(1).GetChild(0).GetComponent<Image>().sprite = UserProfileScript.Instance._defaultDP;

            // set active false Name text
            _canvas.GetChild(1).GetChild(1).GetChild(1).gameObject.SetActive(false);

            // set active true Log in Button
            _canvas.GetChild(1).GetChild(1).GetChild(2).gameObject.SetActive(true);
        }
    }

    #endregion

    #region Assign Button Functions

    private void OnSettingsBtnClick()
    {
        _canvas.GetChild(1).gameObject.SetActive(true);
    }

    #region User Panel

    private void OnProfileBtnClick()
    {
        if (UserProfileScript.Instance._isUserExists)
        {
            _canvas.GetChild(1).GetChild(6).gameObject.SetActive(true);
            FirebaseManagerScript script = GetComponent<FirebaseManagerScript>();
            script.CheckEmailVerification();
        }
        else
        {
            _canvas.GetChild(1).GetChild(4).gameObject.SetActive(true);
        }
    }

    private void OnPaymentsBtnClick()
    {
        _canvas.GetChild(1).GetChild(7).gameObject.SetActive(true);
    }

    private void OnSupportBtnClick()
    {
        _canvas.GetChild(1).GetChild(8).gameObject.SetActive(true);
    }

    #endregion

    #region Other Settings

    private void OnShareAppBtnClick()
    {
        //Debug.Log("Share The App");
        ShareTheAppScript.ShareAppLink();
    }

    private void OnAboutUsBtnClick()
    {
        _canvas.GetChild(2).gameObject.SetActive(true);
    }

    private void OnRateUsBtnClick()
    {
        Application.OpenURL(_urlLink);
    }

    private void OnPrivacyPolicyBtnClick()
    {
        _canvas.GetChild(3).gameObject.SetActive(true);
    }

    private void OnNotificationPrefBtnClick()
    {
        _canvas.GetChild(4).gameObject.SetActive(true);
    }

    #endregion

    #region User Credentials

    private void OnLoginPageOpen()
    {
        _canvas.GetChild(1).GetChild(4).gameObject.SetActive(true);
    }

    private void OnRegisterPageOpen()
    {
        _canvas.GetChild(1).GetChild(5).gameObject.SetActive(true);
    }

    #endregion

    #endregion


}
