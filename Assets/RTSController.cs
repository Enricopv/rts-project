
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class RTSController : MonoBehaviour
{

  public Camera cam;
  public Canvas canvas;
  public Image selectionBox;
  public KeyCode copyKey = KeyCode.LeftControl;
  private Vector3 startScreenPos;
  private BoxCollider worldCollider;
  private RectTransform rt;
  private bool isSelecting;
  public Dictionary<string, UnitController> units = new Dictionary<string, UnitController>();

  private Vector2 startPos;



  void Awake()
  {
    if (canvas == null)
      canvas = FindObjectOfType<Canvas>();

    if (selectionBox != null)
    {
      // selectionBox.
      //We need to reset anchors and pivot to ensure proper positioning
      rt = selectionBox.GetComponent<RectTransform>();
      rt.pivot = Vector2.one * .5f;
      rt.anchorMin = Vector2.one * .5f;
      rt.anchorMax = Vector2.one * .5f;
      selectionBox.gameObject.SetActive(false);
    }
  }


  // Update is called once per frame
  void Update()
  {

    // [Escape] to clear unit selection
    if (Input.GetKey(KeyCode.Escape))
    {
      foreach (KeyValuePair<string, UnitController> unit in units)
      {
        // do something with entry.Value or entry.Key
        unit.Value.setDeselect();
      }
      // units.ForEach(delegate (UnitController unit)
      // {
      //   unit.setDeselect();
      // });
      units.Clear();
    }



    // [Left Click] to select and add units to our selection
    if (Input.GetMouseButtonDown(0))
    {

      // Get info on where the mouse hit
      Ray mouseToWorldRay = Camera.main.ScreenPointToRay(Input.mousePosition);
      RaycastHit hitInfo;


      // Check if something got and hit and see if it has a PlayerController Component
      // If so, lets add it to our selected Units and make call setSelected on the unit
      if (Physics.Raycast(mouseToWorldRay, out hitInfo, 100))
      {
        GameObject inWorldObject = hitInfo.transform.gameObject;


        // :: I'd rather do an interface thing here
        UnitController controller = inWorldObject.GetComponent<UnitController>();


        if (controller != null)
        {

          if (Input.GetKey(copyKey))
          {
            UpdateSelection(controller);
          }
          else
          {
            ClearSelected();
            UpdateSelection(controller);
          }

          //If we clicked on a Selectable, we don't want to enable our SelectionBox
          return;
        }

      }
    }

    // [Right Click] to call the units to move in a direction
    if (Input.GetMouseButtonDown(1))
    {
      Ray ray = cam.ScreenPointToRay(Input.mousePosition);
      RaycastHit hit;

      if (Physics.Raycast(ray, out hit))
      {
        foreach (KeyValuePair<string, UnitController> unit in units)
        {
          // do something with entry.Value or entry.Key
          unit.Value.MoveUnit(hit.point);
        }
        // Get all our selected units, and call MoveUnit() on each
        // units.ForEach(delegate (UnitController unit)
        // {
        //   unit.MoveUnit(hit.point);
        // });
      }
    }
  }


  void UpdateSelection(UnitController unit)
  {
    Dictionary<string, UnitController> localUnits = new Dictionary<string, UnitController>(units);

    if (unit.isSelected)
    {
      Debug.Log("Clicked ID " + unit.unitId);

      // foreach (KeyValuePair<string, UnitController> gunit in localUnits)
      // {
      //   // do something with entry.Value or entry.Key
      //   Debug.Log("Unit Ids : " + gunit.Value.unitId);
      //   Debug.Log("Unit keys : " + gunit.Key);
      // }




      if (localUnits.Remove(unit.unitId))
      {
        unit.setDeselect();
      }
      else
      {
        // Debug.Log("unit not removed");
      }

    }
    else
    {
      localUnits.Add(unit.unitId, unit);
      unit.setSelected();
    }
    units.Clear();
    foreach (var lunit in localUnits)
      units.Add(lunit.Key, lunit.Value);
  }



  void ClearSelected()
  {
    Dictionary<string, UnitController> localUnits = new Dictionary<string, UnitController>(units);
    foreach (KeyValuePair<string, UnitController> unit in localUnits)
    {
      unit.Value.setDeselect();
      // localUnits.Remove(unit.Value.unitId);
    }
    units.Clear();
  }

}


