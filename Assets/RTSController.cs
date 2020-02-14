
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
  private RectTransform rt;
  public Dictionary<string, UnitController> units = new Dictionary<string, UnitController>();
  public Dictionary<string, UnitController> allSelectableUnits = new Dictionary<string, UnitController>();
  //The selection squares 4 corner positions
  Vector3 TL, TR, BL, BR;

  //To determine if we are clicking with left mouse or holding down left mouse
  float delay = 0.1f;
  float clickTime = 0f;

  Vector3 squareStartPos;
  Vector3 squareEndPos;

  //If it was possible to create a square
  bool hasCreatedSquare;

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

  void Start()
  {
    var allUnitControllers = FindObjectsOfType<MonoBehaviour>().OfType<ISelectable>();

    foreach (UnitController unit in allUnitControllers)
    {
      allSelectableUnits.Add(unit.unitId, unit);
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

      units.Clear();
    }


    SelectUnits();

  }

  void SelectUnits()
  {
    //Are we clicking with left mouse or holding down left mouse
    bool isClicking = false;
    bool isHoldingDown = false;

    //Click the mouse button
    if (Input.GetMouseButtonDown(0))
    {
      clickTime = Time.time;

      //We dont yet know if we are drawing a square, but we need the first coordinate in case we do draw a square
      RaycastHit hit;
      //Fire ray from camera
      if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 200f, 1 << 8))
      {
        //The corner position of the square
        squareStartPos = hit.point;

      }
      startScreenPos = Input.mousePosition;
    }

    //Release the mouse button
    if (Input.GetMouseButtonUp(0))
    {
      if (Time.time - clickTime <= delay)
      {
        isClicking = true;
      }

      //Select all units within the square if we have created a square
      if (hasCreatedSquare)
      {
        hasCreatedSquare = false;

        //Deactivate the square selection image
        selectionBox.gameObject.SetActive(false);

        //Clear the list with selected unit
        // ClearSelected();


        foreach (KeyValuePair<string, UnitController> unit in allSelectableUnits)
        {
          if (unit.Value.isHighlighted)
          {
            units.Add(unit.Value.unitId, unit.Value);
          }
        }
      }

    }


    // :: [Right Click] to call the units to move in a direction
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

    //Holding down the mouse button
    if (Input.GetMouseButton(0))
    {
      if (Time.time - clickTime > delay)
      {
        isHoldingDown = true;
      }
    }

    //Select one unit with left mouse and deselect all units with left mouse by clicking on what's not a unit
    if (isClicking)
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
            Debug.Log("DONT RUN");
            ClearSelected();
            UpdateSelection(controller, true);
          }

          //If we clicked on a Selectable, we don't want to enable our SelectionBox
          return;
        }

      }
    }

    //Drag the mouse to select all units within the square
    if (isHoldingDown)
    {
      hasCreatedSquare = true;

      selectionBox.gameObject.SetActive(true);
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
      foreach (KeyValuePair<string, UnitController> unit in allSelectableUnits)
      {
        //If the screenPosition of the worldobject is within our selection bounds, we can add it to our selection
        Vector3 screenPos = cam.WorldToScreenPoint(unit.Value.transform.position);
        screenPos.z = 0;
        if (b.Contains(screenPos))
        {
          HightlighUnit(unit.Value);
        }
        else
        {
          unit.Value.removeHighlight();
        }
      }
    }
  }



  void HightlighUnit(UnitController unit)
  {
    unit.setHighlight();
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
        // Debug.Log("unit not removed");
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


