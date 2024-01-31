using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroStartScript : MonoBehaviour
{
    [SerializeField] float introWaitTime = 6f;

    void Start()
    {
        StartCoroutine(WaitForIntro());
    }

    private IEnumerator WaitForIntro()
    {
        yield return new WaitForSeconds(introWaitTime);

        SceneManager.LoadScene(1);
    }
}
