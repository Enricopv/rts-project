
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class GameInfo : MonoBehaviour
{

  public Camera cam;

  public List<PlayerController> units = new List<PlayerController>();


  // Start is called before the first frame update
  void Start()
  {

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

    // [Left Click] to select
    if (Input.GetMouseButtonDown(0))
    {
      Ray ray = cam.ScreenPointToRay(Input.mousePosition);
      RaycastHit hit;

      if (Physics.Raycast(ray, out hit, 100))
      {
        GameObject obj = hit.transform.gameObject;

        PlayerController controller = obj.GetComponent<PlayerController>();
        if (controller)
        {
          units.Add(controller);
          controller.setSelected();
          //   Material material = obj.GetComponent<Renderer>().material;
          //   Debug.Log(material);
          //   material.color = Color.green;

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
        // MOVE OUR AGENT
        // Call function
        units.ForEach(delegate (PlayerController unit)
        {
          unit.MoveUnit(hit.point);
        });
      }
    }
  }
}
