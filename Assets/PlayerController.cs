
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
  //   public Camera cam;

  public Color baseColor = new Color(93, 242, 243);
  public Color selectedColor = new Color(93, 243, 98);

  public NavMeshAgent agent;



  public void setSelected()
  {
    Material material = GetComponent<Renderer>().material;
    ColorUtility.TryParseHtmlString("#5DF386", out selectedColor);
    material.color = selectedColor;
  }

  public void setDeselect()
  {
    Material material = GetComponent<Renderer>().material;
    ColorUtility.TryParseHtmlString("#5DF3F3", out baseColor);
    material.color = baseColor;
  }

  public void MoveUnit(Vector3 point)
  {
    agent.SetDestination(point);
  }
}
