using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MouseController : Subject
{
    public OverlayTile focusedOnTile;


    // Update is called once per frame
    void FixedUpdate()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        OverlayTile newFocusedOnTile = GetOverlayTile(mousePos);

        if(newFocusedOnTile)
        {
            if(focusedOnTile != newFocusedOnTile && !newFocusedOnTile.IsOccupied)
            {
                focusedOnTile = newFocusedOnTile;
                transform.position = newFocusedOnTile.transform.position;
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            NotifyObservers();
            focusedOnTile.Show();
        }
    }

    public OverlayTile GetOverlayTile(Vector3 mousePos)
    {
        var mousePos2d = new Vector2(mousePos.x, mousePos.y);

        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
        {
            Debug.Log(hitInfo.transform.gameObject.name);
        }

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2d, Vector2.zero);

        if(hits.Length > 0)
        {
            var firstHit = hits.OrderByDescending(i => i.collider.transform.position.z).First();
            return firstHit.collider.gameObject.GetComponent<OverlayTile>();
        }
        return null;
    }
}
