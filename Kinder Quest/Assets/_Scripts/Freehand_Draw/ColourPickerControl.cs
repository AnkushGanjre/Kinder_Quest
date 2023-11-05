using UnityEngine;
using UnityEngine.UI;

public class ColourPickerControl : MonoBehaviour
{
    [Header("Hue, Saturation & value")]
    public float CurrentHue;
    public float CurrentSat;
    public float CurrentVal;

    [Header("Raw Images")]
    [SerializeField] RawImage hueImage;
    [SerializeField] RawImage satValImage;
    [SerializeField] RawImage OutputImage;
    
    [Header("UI Components")]
    [SerializeField] Slider hueSlider;
    [SerializeField] Image finalColorImage;
    [SerializeField] Button _okButton;

    [Header("Texture Components")]
    private Texture2D hueTexture, svTexture, outputTexture;

    private void Awake()
    {
        // Find and assign references to UI elements.
        hueImage = GameObject.Find("Hue_Bar_Panel").GetComponent<RawImage>();
        satValImage = GameObject.Find("Sat_Val_Panel").GetComponent<RawImage>();
        OutputImage = GameObject.Find("Output_Image").GetComponent<RawImage>();
        hueSlider = GameObject.Find("Hue_Slider").GetComponent<Slider>();
        finalColorImage = GameObject.Find("Final_Colour_Image").GetComponent<Image>();
        _okButton = GameObject.Find("Done_Button").GetComponent<Button>();
    }

    private void Start()
    {
        // Add a click event listener to the OK button to trigger OnColourSelectedBtn method.
        _okButton.onClick.AddListener(() => { OnColourSelectedBtn(); });

        // Create the hue, saturation/value, and output textures.
        CreateHueImage();
        CreateSVImage();
        CreateOutputImage();

        // Update the output image with the current color.
        UpdateOutputImage();
    }

    private void CreateHueImage()
    {
        // Create a new 1x16 texture for displaying the hue spectrum.
        hueTexture = new Texture2D(1, 16);
        hueTexture.wrapMode = TextureWrapMode.Clamp;
        hueTexture.name = "HueTexture";

        // Fill the hue texture with colors representing the entire hue spectrum.
        for (int i = 0; i < hueTexture.height; i++)
        {
            // Calculate the hue color based on the position in the texture.
            // The value (float)i / (hueTexture.height - 1) ranges from 0 to 1,
            // representing the full spectrum of hues.
            hueTexture.SetPixel(0, i, Color.HSVToRGB((float)i / (hueTexture.height - 1), 1, 1));
        }

        // Apply the changes to the texture.
        hueTexture.Apply();
        CurrentHue = 0;

        // Set the hueImage UI element's texture to the created hue texture.
        hueImage.texture = hueTexture;
    }

    private void CreateSVImage()
    {
        // Create a new 16x16 texture for displaying the saturation/value spectrum.
        svTexture = new Texture2D(16, 16);
        svTexture.wrapMode = TextureWrapMode.Clamp;
        svTexture.name = "SatValTexture";

        // Fill the saturation/value texture with colors based on the current hue.
        for (int y = 0; y < svTexture.height; y++)
        {
            for (int x = 0; x < svTexture.width; x++)
            {
                // Calculate the color in the saturation/value texture based on the current hue,
                // and the position (x, y) within the texture.
                svTexture.SetPixel(x, y, Color.HSVToRGB(CurrentHue, (float)x / svTexture.width, (float)y / svTexture.height));
            }
        }

        // Apply the changes to the texture.
        svTexture.Apply();
        CurrentSat = 1f;
        CurrentVal = 1f;

        // Set the satValImage UI element's texture to the created saturation/value texture.
        satValImage.texture = svTexture;
    }

    private void CreateOutputImage()
    {
        // Create a new 1x16 texture for displaying the output color.
        outputTexture = new Texture2D(1, 16);
        outputTexture.wrapMode = TextureWrapMode.Clamp;
        outputTexture.name = "OutputTexture";

        // Generate an initial color based on the current hue, saturation, and value.
        Color currentColour = Color.HSVToRGB(CurrentHue, CurrentSat, CurrentVal);

        // Fill the output texture with the initial color.
        for (int i = 0; i < outputTexture.height; i++)
        {
            outputTexture.SetPixel(0, i, currentColour);
        }

        // Apply the changes to the texture.
        outputTexture.Apply();

        // Set the OutputImage UI element's texture to the created output texture.
        OutputImage.texture = outputTexture;
    }

    private void UpdateOutputImage()
    {
        // Calculate the current color based on the current hue, saturation, and value.
        Color currentColor = Color.HSVToRGB(CurrentHue, CurrentSat, CurrentVal);

        // Update the entire output texture with the current color.
        for (int i = 0; i < outputTexture.height; i++)
        {
            outputTexture.SetPixel(0, i, currentColor);
        }

        // Apply the changes to the texture to update it.
        outputTexture.Apply();
    }

    public void SetSV(float S, float V)
    {
        // Set the current saturation and value based on the provided values.
        CurrentSat = S;
        CurrentVal = V;

        // Update the output image to reflect the changes.
        UpdateOutputImage();
    }

    public void UpdateSVImage()
    {
        // Update the current hue based on the value of the hueSlider UI element.
        CurrentHue = hueSlider.value;

        // Update the saturation/value (SV) image with colors based on the new hue.
        for (int y = 0; y < svTexture.height; y++)
        {
            for (int x = 0; x < svTexture.width; x++)
            {
                // Calculate the color in the saturation/value (SV) image based on the current hue,
                // and the position (x, y) within the texture.
                svTexture.SetPixel(x, y, Color.HSVToRGB(CurrentHue, (float)x / svTexture.width, (float)y / svTexture.height));
            }
        }

        // Apply the changes to the saturation/value (SV) image texture.
        svTexture.Apply();

        // Update the output image to reflect the new selected hue.
        UpdateOutputImage();
    }

    private void OnColourSelectedBtn()
    {
        // Calculate the current color based on the current hue, saturation, and value.
        Color currentColor = Color.HSVToRGB(CurrentHue, CurrentSat, CurrentVal);

        // Set the color of the finalColorImage UI element to the selected color.
        finalColorImage.color = currentColor;

        // Set the LineColor in the FreeHandDrawMgrScript to the selected color.
        FreeHandDrawMgrScript.Instance.LineColor = currentColor;

        // Set the flag in the FreeHandDrawMgrScript to indicate that another panel is not open.
        FreeHandDrawMgrScript.Instance.isOtherPanelOpen = false;
    }
}
