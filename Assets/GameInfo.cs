
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class GameInfo : MonoBehaviour
{

  public Camera cam;
  public Canvas canvas;
  public Image selectionBox;
  public KeyCode copyKey = KeyCode.LeftControl;
  private Vector3 startScreenPos;
  private BoxCollider worldCollider;
  private RectTransform rt;
  private bool isSelecting;
  public List<PlayerController> units = new List<PlayerController>();

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
      units.ForEach(delegate (PlayerController unit)
      {
        unit.setDeselect();
      });
      units.Clear();
    }



    // [Left Click] to select and add units to our selection
    if (Input.GetMouseButtonDown(0))
    {
      // UpdateSelectionBox(Input.mousePosition);
      // Cast lazer beam from camera to where the mouse clicked in the world
      Ray ray = cam.ScreenPointToRay(Input.mousePosition);
      RaycastHit hit;


      // Check if something got and hit and see if it has a PlayerController Component
      // If so, lets add it to our selected Units and make call setSelected on the unit
      if (Physics.Raycast(ray, out hit, 100))
      {
        GameObject obj = hit.transform.gameObject;

        PlayerController controller = obj.GetComponent<PlayerController>();
        if (controller)
        {
          units.Add(controller);
          controller.setSelected();

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
        // Get all our selected units, and call MoveUnit() on each
        units.ForEach(delegate (PlayerController unit)
        {
          unit.MoveUnit(hit.point);
        });
      }
    }
  }

}
//   void UpdateSelectionBox(Vector2 curMousePos)
//   {
//     if (!selectionBox.gameObject.activeInHierarchy)
//       selectionBox.gameObject.SetActive(true);

//     float width = curMousePos.x - startPos.x;
//     float height = curMousePos.y - startPos.y;

//     selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
//     selectionBox.anchoredPosition = startPos + new Vector2(width / 2, height / 2);
//   }
// }
