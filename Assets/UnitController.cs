
using UnityEngine;
using UnityEngine.AI;

interface ISelectable
{
  void setSelected();
  void setDeselect();

  void setHighlight();
  void removeHighlight();

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


  private bool highlighted;

  public bool isHighlighted
  {
    get
    {
      return highlighted;
    }
    set
    {
      highlighted = value;
    }
  }



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
    generatedId = this.GetInstanceID().ToString();
  }

  public void setSelected()
  {
    selected = true;
    setHighlight();
  }

  public void setHighlight()
  {
    Material material = GetComponent<Renderer>().material;
    ColorUtility.TryParseHtmlString("#5DF386", out selectedColor);
    material.color = selectedColor;
    highlighted = true;
  }

  public void setDeselect()
  {
    selected = false;
    removeHighlight();
  }

  public void removeHighlight()
  {
    Material material = GetComponent<Renderer>().material;
    ColorUtility.TryParseHtmlString("#5DF3F3", out baseColor);
    material.color = baseColor;
    highlighted = false;
  }

  public void MoveUnit(Vector3 point)
  {
    agent.SetDestination(point);
  }
}
