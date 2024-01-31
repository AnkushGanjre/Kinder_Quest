using UnityEngine;
using UnityEngine.UI;

public class NotiPrefToggleBtnScript : MonoBehaviour
{
    [SerializeField] Button _toggleBtn;
    [SerializeField] Sprite _toggleOnBtn;
    [SerializeField] Sprite _toggleOffBtn;

    [SerializeField] bool _isToggleOn;

    private void Awake()
    {
        _toggleBtn = GameObject.Find("NotiPref_Toggle_Btn").GetComponent<Button>();
    }

    private void Start()
    {
        _toggleBtn.onClick.AddListener(() => { OnToggleBtnClick(); });
        _toggleBtn.GetComponent<Image>().sprite = _toggleOnBtn;
        _isToggleOn = true;
    }

    private void OnToggleBtnClick()
    {
        if (_isToggleOn)
        {
            _toggleBtn.GetComponent<Image>().sprite = _toggleOffBtn;
            _isToggleOn= false;
        }
        else
        {
            _toggleBtn.GetComponent<Image>().sprite = _toggleOnBtn;
            _isToggleOn= true;
        }
    }
}
