using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

  public float panSpeed = 20f;
  public float panBorderThickness = 10f;

  public float scrollSpeed = 20f;



  // Update is called once per frame
  void Update()
  {

    Vector3 pos = transform.position;

    if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - panBorderThickness)
    {
      pos.z += panSpeed * Time.deltaTime;
    }
    if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= panBorderThickness)
    {
      pos.z -= panSpeed * Time.deltaTime;
    }
    if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - panBorderThickness)
    {
      pos.x += panSpeed * Time.deltaTime;
    }
    if (Input.GetKey(KeyCode.A) || Input.mousePosition.x <= panBorderThickness)
    {
      pos.x -= panSpeed * Time.deltaTime;
    }

    float scroll = Input.GetAxis("Mouse ScrollWheel");


    pos.y -= scroll * scrollSpeed * 100f * Time.deltaTime;

    transform.position = pos;
  }
}
