
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
      Ray mouseToWorldRay = cam.ScreenPointToRay(Input.mousePosition);
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
            UpdateSelection(controller, !controller.isSelected);
          }
          else
          {
            ClearSelected();
            UpdateSelection(controller, true);
          }

          //If we clicked on a Selectable, we don't want to enable our SelectionBox
          return;
        }

      }
      if (selectionBox == null)
        return;
      //Storing these variables for the selectionBox
      startScreenPos = Input.mousePosition;
      isSelecting = true;
    }

    //If we never set the selectionBox variable in the inspector, we are simply not able to drag the selectionBox to easily select multiple objects. 'Regular' selection should still work
    if (selectionBox == null)
      return;

    // [Left Click Goes Up] We finished our selection box when the key is released
    if (Input.GetMouseButtonUp(0))
    {
      isSelecting = false;
    }


    if (isSelecting)
    {
      Debug.Log("SELECTING");
      Bounds b = new Bounds();
      //The center of the bounds is inbetween startpos and current pos
      b.center = Vector3.Lerp(startScreenPos, Input.mousePosition, 0.5f);
      //We make the size absolute (negative bounds don't contain anything)
      b.size = new Vector3(Mathf.Abs(startScreenPos.x - Input.mousePosition.x),
          Mathf.Abs(startScreenPos.y - Input.mousePosition.y),
          0);

      //To display our selectionbox image in the same place as our bounds
      rt.position = b.center;
      rt.sizeDelta = canvas.transform.InverseTransformVector(b.size);

      //Looping through all the selectables in our world (automatically added/removed through the Selectable OnEnable/OnDisable)
      foreach (KeyValuePair<string, UnitController> unit in units)
      {
        //If the screenPosition of the worldobject is within our selection bounds, we can add it to our selection
        Vector3 screenPos = cam.WorldToScreenPoint(unit.Value.transform.position);
        screenPos.z = 0;
        UpdateSelection(unit.Value, (b.Contains(screenPos)));
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
          unit.Value.MoveUnit(hit.point);
        }

      }
    }
  }


  void UpdateSelection(UnitController unit, bool value)
  {
    Dictionary<string, UnitController> localUnits = new Dictionary<string, UnitController>(units);

    if (unit.isSelected != value)
    {
      localUnits.Add(unit.unitId, unit);
      unit.setSelected();
    }
    else
    {
      if (localUnits.Remove(unit.unitId))
      {
        unit.setDeselect();
      }
      else
      {
        Debug.Log("unit not removed");
      }

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
    }
    units.Clear();
  }

}


