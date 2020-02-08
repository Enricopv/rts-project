
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
    if (Input.GetKey(KeyCode.Escape))
    {
      units.Clear();
    }
    if (Input.GetMouseButtonDown(0))
    {
      Ray ray = cam.ScreenPointToRay(Input.mousePosition);
      RaycastHit hit;

      if (Physics.Raycast(ray, out hit, 100))
      {
        GameObject obj = hit.transform.gameObject;

        PlayerController clicked = obj.GetComponent<PlayerController>();
        if (clicked)
        {
          units.Add(clicked);
        }

      }
    }


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
