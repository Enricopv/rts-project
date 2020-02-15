
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace RTSGame
{
  public class RTSController : MonoBehaviour
  {

    public Camera cam;
    public Canvas canvas;
    public Image selectionBox;
    public KeyCode copyKey = KeyCode.LeftControl;

    private RectTransform rt;

    public Dictionary<string, UnitController> units = new Dictionary<string, UnitController>();
    public Dictionary<string, UnitController> allSelectableUnits = new Dictionary<string, UnitController>();
    //The selection squares 4 corner positions
    Vector3 TL, TR, BL, BR;

    //To determine if we are clicking with left mouse or holding down left mouse
    float delay = 0.1f;
    float clickTime = 0f;


    private ControllerState ControllerState;
    //If it was possible to create a square
    bool hasCreatedSquare;

    public bool isHolding = false;

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

      // We want to have a catalog of all selectable units for iterating over so we don't have to
      // Always be doing a call for all objects.
      var allUnitControllers = FindObjectsOfType<MonoBehaviour>().OfType<ISelectable>();
      foreach (UnitController unit in allUnitControllers)
      {
        allSelectableUnits.Add(unit.unitId, unit);
      }
      ControllerState = new ControllerState(cam, allSelectableUnits);

    }

    // Update is called once per frame
    void Update()
    {

      // [Escape]
      PressEscape();

      // [Left Click]
      PressLeftClick();

      // [Left Click - Hold]
      HoldLeftClick();

      // [Right Click]

      // SelectUnits();

    }

    void PressLeftClick()
    {
      if (Input.GetMouseButtonDown(0))
      {
        Ray mouseToWorldRay = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(mouseToWorldRay, out hitInfo, 100))
        {
          GameObject inWorldObject = hitInfo.transform.gameObject;

          // :: I'd rather do an interface thing here
          UnitController unit = inWorldObject.GetComponent<UnitController>();

          if (unit != null)
          {
            if (!unit.isSelected)
            {
              ControllerState.State.AddUnit(unit);
            }
            else
            {
              ControllerState.State.RemoveUnit(unit);
            }
          }
        }
      }
    }

    void HoldLeftClick()
    {

      // Logic to check if being held
      if (Input.GetMouseButton(0))
      {
        if (clickTime == 0f)
        {
          clickTime = Time.time;
        }

        if (Time.time - clickTime > 0.3f)
        {
          Debug.Log("HOLD STARTED!!!");
        }
      }

      if (Input.GetMouseButtonUp(0))
      {
        Debug.Log("I AM RELEASED");
        clickTime = 0f;
      }


    }

    void PressEscape()
    {
      // [Escape] to clear unit selection
      if (Input.GetKey(KeyCode.Escape))
      {
        ControllerState.State.ClearUnits();

      }
    }

    void SelectUnits()
    {


      //Release the mouse button
      if (Input.GetMouseButtonUp(0))
      {
        ControllerState.State.IsHoldingDown = false;
        if (Time.time - clickTime <= delay)
        {
          ControllerState.State.IsClicking = true;
        }
        else
        {
          ControllerState.State.IsClicking = false;
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
          ControllerState.State.IsHoldingDown = true;
        }
      }

      //Select one unit with left mouse and deselect all units with left mouse by clicking on what's not a unit
      if (ControllerState.State.IsClicking)
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
          else
          {

          }

        }
      }

      //Drag the mouse to select all units within the square
      if (ControllerState.State.IsHoldingDown)
      {
        hasCreatedSquare = true;

        selectionBox.gameObject.SetActive(true);
        Bounds b = new Bounds();
        //The center of the bounds is inbetween startpos and current pos
        b.center = Vector3.Lerp(ControllerState.State.StartScreenPos, Input.mousePosition, 0.5f);
        //We make the size absolute (negative bounds don't contain anything)
        b.size = new Vector3(Mathf.Abs(ControllerState.State.StartScreenPos.x - Input.mousePosition.x),
            Mathf.Abs(ControllerState.State.StartScreenPos.y - Input.mousePosition.y),
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
            HightlightUnit(unit.Value);
          }
          else
          {
            unit.Value.removeHighlight();
          }
        }
      }
    }



    void HightlightUnit(UnitController unit)
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
        unit.setHighlight();
      }
      else
      {
        if (localUnits.Remove(unit.unitId))
        {
          unit.setDeselect();
          unit.removeHighlight();
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
        unit.Value.removeHighlight();
      }
      units.Clear();
    }


  }

}
