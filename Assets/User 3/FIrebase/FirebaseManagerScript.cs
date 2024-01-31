using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using ListString = System.Collections.Generic.List<string>;
using Google.MiniJSON;


public class FirebaseManagerScript : MonoBehaviour
{
    [Header("Firebase")]
    private DependencyStatus DependencyStatus;
    private DatabaseReference _dbRef;
    private FirebaseAuth _auth;
    private FirebaseUser _user;

    [Header("Login")]
    private TextMeshProUGUI _loginLogText;
    private TMP_InputField _emailLoginField;
    private TMP_InputField _passwordLoginField;
    private Button _loginBtn;
    private Button _loginResetPassBtn;

    [Header("Registration")]
    private TextMeshProUGUI _registerLogText;
    private TMP_InputField _nameRegisterField;
    private TMP_InputField _emailRegisterField;
    private TMP_InputField _dobRegisterField;
    private TMP_InputField _ageRegisterField;
    private TMP_InputField _genderRegisterField;
    private TMP_InputField _passwordRegisterField;
    private TMP_InputField _confirmPasswordRegisterField;
    private Button _registerBtn;

    [Header("Logout")]
    private Button _logoutBtn;

    [Header("Profile")]
    private Button _emailVerifyBtn;
    private Button _profileResetPassBtn;
    private TextMeshProUGUI _ProfileLogText;

    private Button _checkingButton;

    private void Awake()
    {
        _dbRef = FirebaseDatabase.DefaultInstance.RootReference;

        _loginLogText = GameObject.Find("Login_Log_Text").GetComponent<TextMeshProUGUI>();
        _emailLoginField = GameObject.Find("Login_Email_InputField").GetComponent<TMP_InputField>();
        _passwordLoginField = GameObject.Find("Login_Password_InputField").GetComponent<TMP_InputField>();
        _loginBtn = GameObject.Find("Login_Enter_Btn").GetComponent<Button>();
        _loginResetPassBtn = GameObject.Find("Login_Reset_PassBtn").GetComponent<Button>();

        _registerLogText = GameObject.Find("Register_Log_Text").GetComponent<TextMeshProUGUI>();
        _nameRegisterField = GameObject.Find("Register_Name_InputField").GetComponent<TMP_InputField>();
        _emailRegisterField = GameObject.Find("Register_Email_InputField").GetComponent<TMP_InputField>();
        _dobRegisterField = GameObject.Find("Register_DOB_InputField").GetComponent<TMP_InputField>();
        _ageRegisterField = GameObject.Find("Register_Age_InputField").GetComponent<TMP_InputField>();
        _genderRegisterField = GameObject.Find("Register_Gender_InputField").GetComponent<TMP_InputField>();
        _passwordRegisterField = GameObject.Find("Register_Password_InputField").GetComponent<TMP_InputField>();
        _confirmPasswordRegisterField = GameObject.Find("Register_ConfirmPassword_InputField").GetComponent<TMP_InputField>();
        _registerBtn = GameObject.Find("Register_Enter_Btn").GetComponent<Button>();

        _logoutBtn = GameObject.Find("Logout_Enter_Btn").GetComponent<Button>();

        _emailVerifyBtn = GameObject.Find("Profile_Email_VerifyBtn").GetComponent<Button>();
        _profileResetPassBtn = GameObject.Find("Profile_Reset_PassBtn").GetComponent<Button>();
        _ProfileLogText = GameObject.Find("Profile_Log_Text").GetComponent<TextMeshProUGUI>();


        //_checkingButton = GameObject.Find("Checking_Button").GetComponent<Button>();
    }

    private void Start()
    {
        StartCoroutine(CheckAndFixDependenciesAsync());
        _loginBtn.onClick.AddListener(() => { OnUserLogin(); });
        _loginResetPassBtn.onClick.AddListener(() => { OnResetPassword(_emailLoginField.text); });

        _registerBtn.onClick.AddListener(() => { OnUserRegister(); });
        _emailVerifyBtn.onClick.AddListener(() => { SendEmailForVerification(); });

        _logoutBtn.onClick.AddListener(() => { OnUserLogout(); });
        _profileResetPassBtn.onClick.AddListener(() => { OnResetPassword(_user.Email); });

        //_checkingButton.onClick.AddListener(() => { SetActiveSessionFlag("ankush@test.com", true); });
    }


    #region Initilization

    private IEnumerator CheckAndFixDependenciesAsync()
    {
        var dependencyTask = FirebaseApp.CheckAndFixDependenciesAsync();

        yield return new WaitUntil(() => dependencyTask.IsCompleted);

        DependencyStatus = dependencyTask.Result;

        if (DependencyStatus == DependencyStatus.Available)
        {
            InitializeFirebase();
            yield return new WaitForEndOfFrame();
            StartCoroutine(CheckForAutoLogin());
        }
        else
        {
            Debug.LogWarning("Could not resolve all firebase dependencies: " + DependencyStatus);
        }
    }

    void InitializeFirebase()
    {
        // Set the default instance object
        _auth = FirebaseAuth.DefaultInstance;

        _auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (_auth.CurrentUser != _user)
        {
            bool signedIn = _user != _auth.CurrentUser && _auth.CurrentUser != null;

            if (!signedIn && _user != null)
            {
                Debug.LogWarning("Signed out " + _user.DisplayName);
                UserProfileScript.Instance._isUserExists = false;
            }

            _user = _auth.CurrentUser;

            if (signedIn)
            {
                Debug.LogWarning("Signed In " + _user.DisplayName);
                UserProfileScript.Instance._isUserExists = true;
            }
        }

        UiHandlerScript.Instance.OnUserAuthentication();
    }

    #endregion


    #region AutoLogin

    private IEnumerator CheckForAutoLogin()
    {
        if (_user != null)
        {
            var reloadUserTask = _user.ReloadAsync();

            yield return new WaitUntil(() => reloadUserTask.IsCompleted);

            LoadUserDataFromDB();
            AutoLogin();
        }
        else
        {
            UserProfileScript.Instance._isUserExists = false;
            UiHandlerScript.Instance.OnUserAuthentication();
        }
    }

    private void AutoLogin()
    {
        if (_user != null)
        {
            UserProfileScript.Instance._isUserExists = true;
        }
        else
        {
            UserProfileScript.Instance._isUserExists = false;
        }
        UiHandlerScript.Instance.OnUserAuthentication();
    }

    #endregion


    #region Login

    public void OnUserLogin()
    {
        // Call CheckActiveSession to verify active session
        StartCoroutine(CheckActiveSession(_emailLoginField.text));
    }

    private IEnumerator LoginAsync(string email, string password)
    {
        var loginTask = _auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            Debug.LogWarning(loginTask.Exception);

            FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError authError = (AuthError)firebaseException.ErrorCode;

            string failedMessage = "Login Failed! Because: ";

            switch (authError)
            {
                case AuthError.InvalidEmail:
                    failedMessage += "Email is invalid";
                    break;
                case AuthError.WrongPassword:
                    failedMessage += "Wrong Password";
                    break;
                case AuthError.MissingEmail:
                    failedMessage += "Email is missing";
                    break;
                case AuthError.MissingPassword:
                    failedMessage += "Password is missing";
                    break;
                default:
                    failedMessage += "Login Failed";
                    break;
            }

            Debug.LogWarning(failedMessage);
            _loginLogText.text = failedMessage;
            StartCoroutine(CloseLogText(_loginLogText.gameObject));
        }
        else
        {
            _user = loginTask.Result.User;
            SetActiveSessionFlag(_user.Email, true);

            Debug.LogFormat("{0} You Are Succeefully Logged In", _user.DisplayName);
            LoadUserDataFromDB();
            UserProfileScript.Instance.OnSuccessfulLogin();
        }
    }

    #endregion


    #region Logout

    public void OnUserLogout()
    {
        if (_auth != null && _user != null)
        {
            SetActiveSessionFlag(_user.Email, false);
            _auth.SignOut();
        }
    }

    #endregion


    #region Registration

    public void OnUserRegister()
    {
        StartCoroutine(RegisterAsync(_nameRegisterField.text, _emailRegisterField.text, _passwordRegisterField.text));
    }

    private IEnumerator RegisterAsync(string name, string email, string password)
    {
        string displayMessage;

        if (name == "")
        {
            displayMessage = "User Name is empty";
        }
        else if (email == "")
        {
            displayMessage = "Email field is empty";
        }
        else if (_passwordRegisterField.text != _confirmPasswordRegisterField.text)
        {
            displayMessage = "Password does not match";
        }
        else
        {
            var registerTask = _auth.CreateUserWithEmailAndPasswordAsync(email, password);

            yield return new WaitUntil(() => registerTask.IsCompleted);

            if (registerTask.Exception != null)
            {
                Debug.LogWarning(registerTask.Exception);

                FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
                AuthError authError = (AuthError)firebaseException.ErrorCode;

                displayMessage = "Registration Failed! Because: ";
                switch (authError)
                {
                    case AuthError.InvalidEmail:
                        displayMessage += "Email is invalid";
                        break;
                    case AuthError.WrongPassword:
                        displayMessage += "Wrong Password";
                        break;
                    case AuthError.MissingEmail:
                        displayMessage += "Email is missing";
                        break;
                    case AuthError.MissingPassword:
                        displayMessage += "Password is missing";
                        break;
                    default:
                        displayMessage += "Registration Failed";
                        break;
                }
            }
            else
            {
                // Get the User After Registration Success
                _user = registerTask.Result.User;

                UserProfile userProfile = new UserProfile { DisplayName = name};

                var updateProfileTask = _user.UpdateUserProfileAsync(userProfile);

                yield return new WaitUntil(() => updateProfileTask.IsCompleted);

                if (updateProfileTask.Exception != null)
                {
                    // Delete the user if the user update failed
                    _user.DeleteAsync();

                    Debug.LogWarning(updateProfileTask.Exception);

                    FirebaseException firebaseException = updateProfileTask.Exception.GetBaseException() as FirebaseException;
                    AuthError authError = (AuthError)firebaseException.ErrorCode;


                    displayMessage = "Profile update Failed! Because: ";
                    switch (authError)
                    {
                        case AuthError.InvalidEmail:
                            displayMessage += "Email is invalid";
                            break;
                        case AuthError.WrongPassword:
                            displayMessage += "Wrong Password";
                            break;
                        case AuthError.MissingEmail:
                            displayMessage += "Email is missing";
                            break;
                        case AuthError.MissingPassword:
                            displayMessage += "Password is missing";
                            break;
                        default:
                            displayMessage += "Profile Update Failed";
                            break;
                    }
                }
                else
                {
                    displayMessage = "Registration Successful Welcome " + _user.DisplayName;
                    Debug.LogWarning("Registration Successful Welcome " + _user.DisplayName);
                    UserProfileScript.Instance.UserSubsciptionPlan = "Basic";
                    SaveUserDataToDB();
                    UserProfileScript.Instance.OnSuccessfulRegistration();
                    OnUserLogout();
                }
            }
        }
        Debug.Log(displayMessage);
        _registerLogText.text = displayMessage;
        StartCoroutine(CloseLogText(_registerLogText.gameObject));
    }

    #endregion


    #region Email Verification

    public void SendEmailForVerification()
    {
        StartCoroutine(SendEmailForVerificationAsync());
    }

    private IEnumerator SendEmailForVerificationAsync()
    {
        var sendEmailTask = _user.SendEmailVerificationAsync();

        yield return new WaitUntil(() => sendEmailTask.IsCompleted);

        if (sendEmailTask.Exception != null)
        {
            FirebaseException firebaseException = sendEmailTask.Exception.GetBaseException() as FirebaseException;
            AuthError error = (AuthError)firebaseException.ErrorCode;

            string errorMessage = "Unknown Error: Please Try Again Later";

            switch (error)
            {
                case AuthError.Cancelled:
                    errorMessage = "Email Verification Was Cancelled";
                    break;
                case AuthError.TooManyRequests:
                    errorMessage = "Too Many Email Requests";
                    break;
                case AuthError.InvalidRecipientEmail:
                    errorMessage = "The Email You Entered Is Invalid";
                    break;
            }

            _ProfileLogText.text = errorMessage;
        }
        else
        {
            Debug.Log("Verification Email sent successfully");
            _ProfileLogText.text = "Verification Email Sent successfully";
        }

        StartCoroutine(CloseLogText(_ProfileLogText.gameObject));
    }

    public void CheckEmailVerification()
    {
        if (_user != null)
        {
            bool isVerfied = _user.IsEmailVerified;
            if (isVerfied)
            {
                _emailVerifyBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Verified";
                _emailVerifyBtn.interactable = false;
            }
            else
            {
                _emailVerifyBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Verify";
                _emailVerifyBtn.interactable = true;
            }
        }
    }

    #endregion


    #region Reset Password

    private void OnResetPassword(string email)
    {
        StartCoroutine(ResetPasswordAsync(email));
    }

    private IEnumerator ResetPasswordAsync(string email)
    {
        var sendResetPassTask = _auth.SendPasswordResetEmailAsync(email);

        yield return new WaitUntil(() => sendResetPassTask.IsCompleted);

        string msg;

        if (sendResetPassTask.Exception != null)
        {

            FirebaseException firebaseException = sendResetPassTask.Exception.GetBaseException() as FirebaseException;
            AuthError authError = (AuthError)firebaseException.ErrorCode;

            switch (authError)
            {
                case AuthError.InvalidEmail:
                    msg = "Email is invalid";
                    break;
                case AuthError.MissingEmail:
                    msg = "Email is missing";
                    break;
                default:
                    msg = "Password reset encountered an error";
                    break;
            }
        }
        else
        {
            msg = "Password reset email sent successfully.";
        }

        _loginLogText.text = msg;
        _ProfileLogText.text = msg;
        StartCoroutine(CloseLogText(_loginLogText.gameObject));
        StartCoroutine(CloseLogText(_ProfileLogText.gameObject));
    }

    #endregion


    #region Active Sessions

    public void SetActiveSessionFlag(string email, bool isActive)
    {
        _dbRef.Child("ActiveSessions").Child(SanitizeEmail(email)).SetValueAsync(isActive);
    }

    private IEnumerator CheckActiveSession(string email)
    {
        var activeSessionTask = _dbRef.Child("ActiveSessions").Child(SanitizeEmail(email)).GetValueAsync();

        yield return new WaitUntil(() => activeSessionTask.IsCompleted);

        if (activeSessionTask.Exception != null)
        {
            Debug.LogError("Failed to check active session: " + activeSessionTask.Exception);
            yield break;
        }

        DataSnapshot snapshot = activeSessionTask.Result;

        if (snapshot.Exists)
        {
            var value = snapshot.Value; // Retrieve the value from DataSnapshot

            if (value != null)
            {
                bool isActive = (bool)value; // Cast the value to boolean
                if (isActive)
                {
                    // Active session exists for the user
                    Debug.Log("Active session found for " + email);
                    _loginLogText.text = "Please Log out from other device.";
                    StartCoroutine(CloseLogText(_loginLogText.gameObject));

                    _emailLoginField.text = "";
                    _passwordLoginField.text = "";
                }
                else
                {
                    // No active session found
                    //Debug.Log("No active session found for " + email);
                    
                    // Initiate login method
                    StartCoroutine(LoginAsync(_emailLoginField.text, _passwordLoginField.text));
                }
            }
            else
            {
                // Handle case when value is null
                Debug.Log("Value is null for " + email);
            }
        }
        else
        {
            // No active session found
            Debug.Log("No active session found for " + email);

            // Initiate login method
            StartCoroutine(LoginAsync(_emailLoginField.text, _passwordLoginField.text));
        }
    }

    // Function to sanitize email for database usage (replace '.' with ',' to avoid Firebase restrictions)
    private string SanitizeEmail(string email)
    {
        return email.Replace('.', ',');
    }

    #endregion


    #region Firebase Database

    public void SaveUserDataToDB()
    {
        Debug.Log("*****");
        if (_user != null)
        {
            UserProfileScript.Instance.CurrentUserData.Name = _user.DisplayName;
            UserProfileScript.Instance.CurrentUserData.Email = _user.Email;
            UserProfileScript.Instance.CurrentUserData.DateOfBirth = _dobRegisterField.text;
            UserProfileScript.Instance.CurrentUserData.Age = _ageRegisterField.text;
            UserProfileScript.Instance.CurrentUserData.Gender = _genderRegisterField.text;
            UserProfileScript.Instance.CurrentUserData.SubscriptionPlan = UserProfileScript.Instance.UserSubsciptionPlan;

            string json = JsonUtility.ToJson(UserProfileScript.Instance.CurrentUserData);
            _dbRef.Child("UserID").Child(_user.UserId).SetRawJsonValueAsync(json);
        }
    }

    public void LoadUserDataFromDB()
    {
        if (_user != null)
        {
            StartCoroutine(LoadDataEnum());
        }
    }

    private IEnumerator LoadDataEnum()
    {
        var serverData = _dbRef.Child("UserID").Child(_user.UserId).GetValueAsync();
        yield return new WaitUntil(predicate: () => serverData.IsCompleted);

        DataSnapshot snapShot = serverData.Result;
        string jsonData = snapShot.GetRawJsonValue();

        if (jsonData != null)
        {
            Debug.Log("Server Data Found!");
            UserProfileScript.Instance.CurrentUserData = JsonUtility.FromJson<CurrentUserData>(jsonData);
        }
        else
        {
            Debug.Log("No Data Found!");
        }

        UserProfileScript.Instance.UpdateProfileUI();
        CheckEmailVerification();
    }

    public void OnUserDataUpdate(CurrentUserData newData)
    {
        StartCoroutine(UpdateUserDataInDB(newData));
    }

    private IEnumerator UpdateUserDataInDB(CurrentUserData newData)
    {
        if (_user != null)
        {
            // Convert the updated user profile to JSON
            string updatedJsonData = JsonUtility.ToJson(newData);
            var updateDataTask = _dbRef.Child("UserID").Child(_user.UserId).SetRawJsonValueAsync(updatedJsonData);

            yield return new WaitUntil(() => updateDataTask.IsCompleted);

            string displayMessage;
            if (updateDataTask.Exception != null)
            {
                displayMessage = "Failed to check active session: " + updateDataTask.Exception;
                yield break;
            }
            else
            {
                displayMessage = "User data updated successfully";
                // Perform any UI updates or actions upon successful data update
            }

            Debug.Log(displayMessage);
            _ProfileLogText.text = displayMessage;
            StartCoroutine(CloseLogText(_ProfileLogText.gameObject));
        }
    }

    #endregion


    private IEnumerator CloseLogText(GameObject go)
    {
        go.SetActive(true);
        yield return new WaitForSeconds(3);
        go.SetActive(false);
    }

}
