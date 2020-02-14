
using UnityEngine;
using UnityEngine.AI;

interface ISelectable
{
  void setSelected();
  void setDeselect();

  bool isSelected
  {
    get;
    set;
  }

  string unitId
  {
    get;
  }
}


public class UnitController : MonoBehaviour, ISelectable
{

  private string generatedId;

  public string unitId
  {
    get
    {
      return generatedId;
    }
  }

  private Color baseColor;
  private Color selectedColor;

  public NavMeshAgent agent;

  private bool selected;

  public bool isSelected
  {
    get
    {
      return selected;
    }
    set
    {
      selected = value;
    }
  }

  void Awake()
  {
    // System.Random rng = new System.Random();
    // generatedId = rng.getRandomString();
    // Debug.Log("My Id " + generatedId);
    generatedId = this.GetInstanceID().ToString();
  }

  public void setSelected()
  {
    selected = true;
    Material material = GetComponent<Renderer>().material;
    ColorUtility.TryParseHtmlString("#5DF386", out selectedColor);
    material.color = selectedColor;
  }

  public void setDeselect()
  {
    selected = false;
    Material material = GetComponent<Renderer>().material;
    ColorUtility.TryParseHtmlString("#5DF3F3", out baseColor);
    material.color = baseColor;
  }

  public void MoveUnit(Vector3 point)
  {
    agent.SetDestination(point);
  }
}
