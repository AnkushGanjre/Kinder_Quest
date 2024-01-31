using UnityEngine;

public class ShareTheAppScript : MonoBehaviour
{
    string _shareLink = "https://www.google.com/";

    public static void ShareAppLink()
    {
        ShareTheAppScript instance = FindObjectOfType<ShareTheAppScript>();
        if (instance != null)
        {
            string messageToShare = instance._shareLink;
            instance.ShareText(messageToShare);
        }
        else
        {
            Debug.LogError("ShareTheAppScript instance not found.");
        }
    }

    public void ShareText(string text)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // Create an Android intent for sharing
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

        // Set the action of the intent to ACTION_SEND
        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));

        // Set the type of data to be shared
        intentObject.Call<AndroidJavaObject>("setType", "text/plain");

        // Put the text you want to share
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), text);

        // Get the current activity
        AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

        // Start the Android chooser for sharing
        AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share via");
        currentActivity.Call("startActivity", chooser);

#else
        // This message will appear in the Unity Editor or on non-Android platforms
        Debug.LogWarning("Sharing is not available in the Editor or on non-Android platforms.");
#endif
    }
}
