using UnityEngine;

public class OverlayTile : MonoBehaviour
{
    public Vector2Int gridLocation;
    public Color showColor = Color.white;
    public bool IsOccupied = false;

    public void Show()
    {
        gameObject.GetComponent<SpriteRenderer>().color = showColor;
    }

    public void Hide()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
    }
}
