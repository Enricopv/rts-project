
using UnityEngine;
using UnityEngine.AI;

interface ISelectable
{
  void setSelected();
  void setDeselect();
}
public class PlayerController : MonoBehaviour, ISelectable
{

  private Color baseColor;
  private Color selectedColor;

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
