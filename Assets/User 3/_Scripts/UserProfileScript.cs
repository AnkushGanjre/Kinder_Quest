using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public class CurrentUserData
{
    public string Name;
    public string Email;
    public string DateOfBirth;
    public string Age;
    public string Gender;
    public string SubscriptionPlan;
}

public class UserProfileScript : MonoBehaviour
{
    [Header("Singleton Reference")]
    public static UserProfileScript Instance;

    [Header("Subscription Plan")]
    public string UserSubsciptionPlan;

    [Header("User Details")]
    public CurrentUserData CurrentUserData;
    public string _userName;
    public Sprite _userDP;
    public Sprite _defaultDP;
    public bool _isUserExists;

    [Header("Back Buttons")]
    private Button _loginPgBackBtn;
    private Button _RegisterPgBackBtn;
    private Button _ProfilePgBackBtn;

    [Header("Login")]
    private TextMeshProUGUI _loginLogText;
    private TMP_InputField _loginEmailIdInputField;
    private TMP_InputField _loginPasswordInputField;

    [Header("Register")]
    private TMP_InputField _registerNameInputField;
    private TMP_InputField _registerEmailIdInputField;
    private TMP_InputField _registerDobInputField;
    private TMP_InputField _registerAgeInputField;
    private TMP_InputField _registerGenderInputField;
    private TMP_InputField _registerPasswordInputField;
    private TMP_InputField _registerConfirmPasswordInputField;

    [Header("Profile")]
    private bool _isProfileEditing;
    private Button _profileUpdateBtn;
    private Button _profileResetPassBtn;
    private Button _profileUpdateOkBtn;
    private Button _profileDPUpdateBtn;
    private TMP_InputField _profileNameInputField;
    private TMP_InputField _profileEmailIdInputField;
    private TMP_InputField _profileDobInputField;
    private TMP_InputField _profileAgeInputField;
    private TMP_InputField _profileGenderInputField;
    private TMP_InputField _profileSubscriptionInputField;


    private void Awake()
    {
        OnAwakeEvents();
    }

    private void Start()
    {
        OnStartEvents();
    }

    #region Lifecycle Events

    private void OnAwakeEvents()
    {
        Instance = Instance ?? this;  // Setting Singleton Instance
        if (Instance != this) Destroy(gameObject);  // If not Active Singleton, destroy it
        DontDestroyOnLoad(gameObject);  // Ensure that the Singleton persists across scene changes


        //_userProfileTranf = GameObject.Find("User_Profile_Content").GetComponent<Transform>();
        _loginPgBackBtn = GameObject.Find("LoginPg_BackBtn").GetComponent<Button>();
        _RegisterPgBackBtn = GameObject.Find("RegisterPg_BackBtn").GetComponent<Button>();
        _ProfilePgBackBtn = GameObject.Find("ProfilePg_BackBtn").GetComponent<Button>();

        _loginLogText = GameObject.Find("Login_Log_Text").GetComponent<TextMeshProUGUI>();

        _loginEmailIdInputField = GameObject.Find("Login_Email_InputField").GetComponent<TMP_InputField>();
        _loginPasswordInputField = GameObject.Find("Login_Password_InputField").GetComponent<TMP_InputField>();

        _registerNameInputField = GameObject.Find("Register_Name_InputField").GetComponent<TMP_InputField>();
        _registerEmailIdInputField = GameObject.Find("Register_Email_InputField").GetComponent<TMP_InputField>();
        _registerDobInputField = GameObject.Find("Register_DOB_InputField").GetComponent<TMP_InputField>();
        _registerAgeInputField = GameObject.Find("Register_Age_InputField").GetComponent<TMP_InputField>();
        _registerGenderInputField = GameObject.Find("Register_Gender_InputField").GetComponent<TMP_InputField>();
        _registerPasswordInputField = GameObject.Find("Register_Password_InputField").GetComponent<TMP_InputField>();
        _registerConfirmPasswordInputField = GameObject.Find("Register_ConfirmPassword_InputField").GetComponent<TMP_InputField>();

        _profileUpdateBtn = GameObject.Find("Profile_Update_Btn").GetComponent<Button>();
        _profileResetPassBtn = GameObject.Find("Profile_Reset_PassBtn").GetComponent<Button>();
        _profileUpdateOkBtn = GameObject.Find("Profile_UpdateOk_Btn").GetComponent<Button>();
        _profileDPUpdateBtn = GameObject.Find("DP_ChangeBtn").GetComponent<Button>();
        _profileNameInputField = GameObject.Find("Profile_Name_InputField").GetComponent<TMP_InputField>();
        _profileEmailIdInputField = GameObject.Find("Profile_Email_InputField").GetComponent<TMP_InputField>();
        _profileDobInputField = GameObject.Find("Profile_DOB_InputField").GetComponent<TMP_InputField>();
        _profileAgeInputField = GameObject.Find("Profile_Age_InputField").GetComponent<TMP_InputField>();
        _profileGenderInputField = GameObject.Find("Profile_Gender_InputField").GetComponent<TMP_InputField>();
        _profileSubscriptionInputField = GameObject.Find("Profile_Subscription_InputField").GetComponent<TMP_InputField>();
    }

    private void OnStartEvents()
    {
        _loginPgBackBtn.onClick.AddListener(() => { OnLoginPgBackBtnClick(); });
        _RegisterPgBackBtn.onClick.AddListener(() => { OnRegisterPgBackBtnClick(); });
        _ProfilePgBackBtn.onClick.AddListener(() => { OnProfilePgBackBtnClick(); });

        _profileUpdateBtn.onClick.AddListener(() => { OnProfileUpdateBtnClick(); });
        _profileUpdateOkBtn.onClick.AddListener(() => { OnProfileUpdateOkBtn(); });
        _profileDPUpdateBtn.onClick.AddListener(() => { OnProfileDPChangeBtn(); });
        _profileDPUpdateBtn.gameObject.SetActive(false);
        _profileUpdateOkBtn.gameObject.SetActive(false);
    }

    #endregion


    #region Login Events

    public void OnSuccessfulLogin()
    {
        _loginLogText.gameObject.SetActive(true);
        _loginLogText.text = "Login Successfull!";
        StartCoroutine(SetGameObjectFalse("LOGIN"));
    }

    public void OnSuccessfulRegistration()
    {
        _RegisterPgBackBtn.transform.parent.gameObject.SetActive(false);
        _loginLogText.gameObject.SetActive(true);
        _loginLogText.text = "Registration Successfull Please Login!";
        StartCoroutine(SetGameObjectFalse("REGISTER"));
    }

    private IEnumerator SetGameObjectFalse(string eventName)
    {
        yield return new WaitForSeconds(3);
        if (eventName == "LOGIN")
        {
            _loginPgBackBtn.onClick.Invoke();
        }
        else
        {
            _RegisterPgBackBtn.onClick.Invoke();
        }

        _loginLogText.gameObject.SetActive(false);
    }

    #endregion


    #region User Profile

    public void UpdateProfileUI()
    {
        _userName = CurrentUserData.Name;
        UserSubsciptionPlan = CurrentUserData.SubscriptionPlan;

        _profileNameInputField.text = CurrentUserData.Name;
        _profileEmailIdInputField.text = CurrentUserData.Email;
        _profileDobInputField.text = CurrentUserData.DateOfBirth;
        _profileAgeInputField.text = CurrentUserData.Age;
        _profileGenderInputField.text = CurrentUserData.Gender;
        _profileSubscriptionInputField.text = CurrentUserData.SubscriptionPlan;
    }

    private void OnProfileUpdateBtnClick()
    {
        _isProfileEditing = true;
        _profileDPUpdateBtn.gameObject.SetActive(true);
        _profileDobInputField.interactable = true;
        _profileAgeInputField.interactable = true;
        _profileGenderInputField.interactable = true;

        _profileUpdateBtn.gameObject.SetActive(false);
        _profileResetPassBtn.gameObject.SetActive(false);
        _profileUpdateOkBtn.gameObject.SetActive(true);
    }

    private void OnProfileUpdateOkBtn()
    {
        CurrentUserData.DateOfBirth = _profileDobInputField.text;
        CurrentUserData.Age = _profileAgeInputField.text;
        CurrentUserData.Gender = _profileGenderInputField.text;

        _isProfileEditing = false;
        _profileDPUpdateBtn.gameObject.SetActive(false);
        _profileDobInputField.interactable = false;
        _profileAgeInputField.interactable = false;
        _profileGenderInputField.interactable = false;

        _profileUpdateBtn.gameObject.SetActive(true);
        _profileResetPassBtn.gameObject.SetActive(true);
        _profileUpdateOkBtn.gameObject.SetActive(false);

        FirebaseManagerScript script = GetComponent<FirebaseManagerScript>();
        script.OnUserDataUpdate(CurrentUserData);
    }

    private void OnProfileDPChangeBtn()
    {
        Debug.Log("Changing DP");
    }

    #endregion


    #region Assign Button Functions

    #region User Login Page

    private void OnLoginPgBackBtnClick()
    {
        _loginEmailIdInputField.text = "";
        _loginPasswordInputField.text = "";
    }

    #endregion

    #region User Register Page

    private void OnRegisterPgBackBtnClick()
    {
        _registerNameInputField.text = "";
        _registerEmailIdInputField.text = "";
        _registerDobInputField.text = "";
        _registerAgeInputField.text = "";
        _registerGenderInputField.text = "";
        _registerPasswordInputField.text = "";
        _registerConfirmPasswordInputField.text = "";
    }

    #endregion

    #region User Profile Page

    private void OnProfilePgBackBtnClick()
    {
        if (_isProfileEditing)
        {
            _isProfileEditing = false;
            _profileDPUpdateBtn.gameObject.SetActive(false);
            _profileDobInputField.interactable = false;
            _profileAgeInputField.interactable = false;
            _profileGenderInputField.interactable = false;

            _profileUpdateBtn.gameObject.SetActive(true);
            _profileResetPassBtn.gameObject.SetActive(true);
            _profileUpdateOkBtn.gameObject.SetActive(false);
        }
    }


    #endregion

    #endregion


}
