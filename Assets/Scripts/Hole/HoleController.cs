using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoleController : MonoBehaviour, IDropHandler
{
    private int _ducksThrown = 0;

    private const int MAX_DUCKS_IN_HOLE = 10;

    public void OnDrop(PointerEventData eventData)
    {
        if (GameManager.Instance.ActiveDucks.Count <= 1)
            return;

        var objDragged = eventData.pointerDrag;

        if (objDragged == null)
            return;

        if (_ducksThrown < MAX_DUCKS_IN_HOLE)
        {
            var duck = objDragged.GetComponent<DuckController>();

            duck.Throw2Hole();
            _ducksThrown++;
        }
        else
        {
            _ducksThrown = 0;
            // TODO: throw an egg with some rarity based on the rarity of the thrown ducks.
        }
    }
}
