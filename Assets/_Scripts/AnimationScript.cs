using UnityEngine;
using UnityEditor;

public class AnimationScript : MonoBehaviour
{
    [Header("Singleton Reference")]
    public static AnimationScript Instance;
    private Animator animator;
    public float AnimationTime = 2f;

    private void Awake()
    {
        Instance = Instance ?? this;  // Setting Singleton Instance
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        Vector2 newSize = new Vector2(screenWidth/2, screenHeight);
        Vector2 newPosition = transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition;

        Vector2 newLeftPos = newPosition;
        newLeftPos.x = -((screenWidth / 2) + 250f);
        Vector2 newRightPos = newPosition;
        newRightPos.x = (screenWidth / 2) + 250f;

        transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = newSize;
        transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = newLeftPos;

        transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = newSize;
        transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = newRightPos;

        PlayAnimation();
    }

    public void PlayAnimation()
    {
        // Set the trigger parameter to play the animation
        animator.SetTrigger("TriggerAnimation");
    }
}
