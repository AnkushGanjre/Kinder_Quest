using UnityEngine;
using UnityEngine.UI;

public class DrawPanelScript : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] Button _drwCanvasBtn;
    [SerializeField] Button _drwPaintBtn;
    [SerializeField] Button _drwConnectBtn;

    private void Awake()
    {
        // Find and initialize references to buttons
        _drwCanvasBtn = GameObject.Find("drw_Canvas_Btn").GetComponent<Button>();
        _drwPaintBtn = GameObject.Find("drw_Paint_Btn").GetComponent<Button>();
        _drwConnectBtn = GameObject.Find("drw_Connect_Btn").GetComponent<Button>();
    }

    void Start()
    {
        // Add button click listeners
        _drwCanvasBtn.onClick.AddListener(() => { OnDrwCanvasBtn(); });
        _drwPaintBtn.onClick.AddListener(() => { OnDrwPaintBtn(); });
        _drwConnectBtn.onClick.AddListener(() => { OnDrwConnectBtn(); });
    }

    private void OnDrwCanvasBtn()
    {
        // Handle "Canvas" drawing button click by triggering the drawing method
        FreeHandDrawMgrScript.Instance.OnDrawing("Canvas");
    }

    private void OnDrwPaintBtn()
    {
        // Handle "Paint" drawing button click by triggering the drawing method
        FreeHandDrawMgrScript.Instance.OnDrawing("Paint");
    }

    private void OnDrwConnectBtn()
    {
        // Handle "Connect" drawing button click by triggering the drawing method
        FreeHandDrawMgrScript.Instance.OnDrawing("Connect");
    }
}
