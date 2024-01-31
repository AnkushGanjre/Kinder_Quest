using UnityEngine;

public class LineScript : MonoBehaviour
{
    [Header("Line Renderer")]
    private LineRenderer lr;

    private void Awake()
    {
        // Get the LineRenderer component attached to this GameObject
        lr = GetComponent<LineRenderer>();
    }

    // Method for setting the position of the line
    public void SetPosition(Vector2 pos)
    {
        // Check if a new point can be appended based on distance criteria
        if (!CanAppend(pos))
            return;

        // Increment the number of positions in the LineRenderer and set the position
        lr.positionCount++;
        lr.SetPosition(lr.positionCount - 1, pos);

        // Set the line width to the value specified in FreeHandDrawMgrScript
        lr.startWidth = lr.endWidth = FreeHandDrawMgrScript.Instance.LineWidth;

        // Create a new material for the LineRenderer
        lr.material = new Material(Shader.Find("Sprites/Default"));

        // Set the color of the line based on the value specified in FreeHandDrawMgrScript
        FreeHandDrawMgrScript.Instance.LineColor.a = 1f;
        lr.material.color = FreeHandDrawMgrScript.Instance.LineColor;

        // Set the sorting order for the LineRenderer
        lr.sortingOrder = FreeHandDrawMgrScript.Instance.LineIndex;

        // Adjust the sorting order of the drawing canvas
        HomeScreenMgrScript.Instance.DrawToolsCanvas.GetComponent<Canvas>().sortingOrder = lr.sortingOrder + 1;
    }

    // Check if a new position can be appended to the line based on distance criteria
    private bool CanAppend(Vector2 pos)
    {
        // If there are no existing positions, a new position can always be added
        if (lr.positionCount == 0)
            return true;

        // Check if the distance between the last position and the new position is greater than a threshold
        return Vector2.Distance(lr.GetPosition(lr.positionCount - 1), pos) > FreeHandDrawMgrScript.Instance.ThresholdDistance;
    }
}
