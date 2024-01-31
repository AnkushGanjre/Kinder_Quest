using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class DateInputHandlerScript : MonoBehaviour
{
    private TMP_InputField dateInputField;
    private const string dateFormat = "dd-mm-yyyy";
    private int dateVal;

    private void Start()
    {
        dateInputField = GetComponent<TMP_InputField>();
        if (dateInputField != null)
        {
            dateInputField.onValueChanged.AddListener(OnDateValueChanged);
        }
    }

    private void OnDateValueChanged(string newValue)
    {
        if (!string.IsNullOrEmpty(newValue) && newValue.Length <= 10)
        {
            if (newValue.Length == 2 || newValue.Length == 5)
            {
                if (newValue[newValue.Length - 1] != '-')
                {
                    dateInputField.text = newValue.Insert(newValue.Length, "-");
                    dateInputField.stringPosition++;
                }
            }
        }
    }
}
