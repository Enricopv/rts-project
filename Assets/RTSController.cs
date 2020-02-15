
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

    //The selection squares 4 corner positions

    //To determine if we are clicking with left mouse or holding down left mouse
    float delay = 0.1f;
    float clickTime = 0f;

    Vector3 startScreenPos;

    private ControllerState ControllerState;
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
      ControllerState = new ControllerState();
    }

    void Start()
    {
      // We want to have a catalog of all selectable units for iterating over so we don't have to
      // Always be doing a call for all objects.
      var allUnitControllers = FindObjectsOfType<MonoBehaviour>().OfType<ISelectable>();
      foreach (UnitController unit in allUnitControllers)
      {
        ControllerState.State.SelectableUnits.Add(unit.unitId, unit);
      }

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
      RightClick();


    }

    void RightClick()
    {
      if (Input.GetMouseButtonDown(1))
      {

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
          foreach (KeyValuePair<string, UnitController> unit in ControllerState.State.CurrentSelectedUnits)
          {
            unit.Value.MoveUnit(hit.point);
          }
        }
      }
    }

    void PressLeftClick()
    {
      if (Input.GetMouseButtonDown(0))
      {
        ControllerState.State.ClearUnits();
        startScreenPos = Input.mousePosition;
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

        if (Time.time - clickTime > 0.1f)
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
          if (ControllerState.State.SelectableUnits.Count > 0)
          {
            foreach (KeyValuePair<string, UnitController> unit in ControllerState.State.SelectableUnits)
            {
              //If the screenPosition of the worldobject is within our selection bounds, we can add it to our selection
              Vector3 screenPos = cam.WorldToScreenPoint(unit.Value.transform.position);
              screenPos.z = 0;
              if (b.Contains(screenPos))
              {
                ControllerState.State.AddUnitHighlight(unit.Value);
              }
              else
              {
                ControllerState.State.RemoveUnitHightlight(unit.Value);
              }
            }
          }
        }
      }

      if (Input.GetMouseButtonUp(0))
      {
        if (hasCreatedSquare)
        {
          hasCreatedSquare = true;
          selectionBox.gameObject.SetActive(false);
          clickTime = 0f;
          ControllerState.State.AddHighlightedUnits();
        }
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



  }

}
