using UnityEngine;

public class OverlayTile : MonoBehaviour, IObserver
{
    public Vector2Int gridLocation;
    public Color showColor = Color.white;
    public bool IsOccupied = false;

    public OverlayTile previous;
    public Subject _tileSubject;

    public GameObject node;

    public int G;
    public int H;
    public int F { get { return (G + H); } }


    private void OnDisable()
    {
        _tileSubject.RemoveObserver(this);
    }

    public void Show()
    {
        gameObject.GetComponent<SpriteRenderer>().color = showColor;
    }

    public void Hide()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
    }

    public void OnNotify()
    {
        Hide();
    }

    public void ShowNode()
    {
        node.GetComponent<SpriteRenderer>().color = new Color(0, 0.9372549f, 1, 1);
    }

    public void HideNode()
    {
        node.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
    }

    public void OnNotify(ResourceValue resource)
    {
        throw new System.NotImplementedException();
    }

    public void OnNotify(Job job)
    {
        throw new System.NotImplementedException();
    }
}
