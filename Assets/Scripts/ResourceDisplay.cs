using TMPro;
using UnityEngine;

public class ResourceDisplay : MonoBehaviour
{
    public ResourceColors Color;
    public ResourcesController ResourcesController;
    public TextMeshProUGUI _text;
    // Start is called before the first frame update
    void Start()
    {
         _text = gameObject.GetComponent<TextMeshProUGUI>();
        if (Color == ResourceColors.Blue)
        {
            _text.color = new Color(0,0,1);
        }

        if (Color == ResourceColors.Red)
        {
            _text.color = new Color(1, 0, 0);
        }

        if (Color == ResourceColors.Green)
        {
            _text.color = new Color(0, 1, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        var colors = ResourcesController.resourceValues.Keys;

        foreach (var key in colors)
        {
            if(key == Color)
            {
                _text.text = ResourcesController.resourceValues[Color].ToString();
            }
        }
    }
}
