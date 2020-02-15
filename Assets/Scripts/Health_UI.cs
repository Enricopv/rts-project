using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health_UI : MonoBehaviour
{
  public GameObject uiPrefab;
  public Transform target;
  // Start is called before the first frame update
  Transform cam;
  Transform ui;
  Image healthSlider;
  void Start()
  {
    cam = Camera.main.transform;
    foreach (Canvas c in FindObjectsOfType<Canvas>())
    {
      if (c.name == "UnitCanvas")
      {
        ui = Instantiate(uiPrefab, c.transform).transform;
        healthSlider = ui.GetChild(0).GetComponent<Image>();
        break;
      }
    }

  }

  // Update is called once per frame
  void LateUpdate()
  {
    ui.position = target.position;


    ui.forward = -cam.forward;

  }
}
