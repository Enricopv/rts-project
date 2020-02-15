
using UnityEngine;
using UnityEngine.AI;

interface ISelectable
{

  void setHighlight();
  void removeHighlight();

  bool IsSelected
  {
    get;
  }

  string unitId
  {
    get;
  }

  void setSelected();
  void deSelect();
}

namespace RTSGame
{
  public class UnitController : MonoBehaviour, ISelectable
  {


    public RTSController OwningController;

    public GameObject Model;


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

    private bool _isSelected;
    public bool IsSelected
    {
      get
      {
        return _isSelected;
      }
    }

    private bool _isHighlighted;

    public bool isHighlighted
    {
      get
      {
        return _isHighlighted;
      }
      set
      {
        _isHighlighted = value;
      }
    }



    void Awake()
    {
      generatedId = this.GetInstanceID().ToString();
      deSelect();
      removeHighlight();
    }

    void Start()
    {
      OwningController.ControllerState.AddSelectableUnit(this);

    }

    public void setSelected()
    {
      _isSelected = true;
    }

    public void deSelect()
    {
      _isSelected = false;
    }

    public void setHighlight()
    {
      // Material material = GetComponent<Renderer>().material;
      // Material otherMat = GetComponentInChildren<Renderer>().material;

      // Debug.Log("Other Mat " + otherMat);
      // ColorUtility.TryParseHtmlString("#5DF386", out selectedColor);
      // otherMat.color = selectedColor;

      Light SpotLight = GetComponentInChildren<Light>();

      SpotLight.intensity = 3;



      // material.color = selectedColor;
      _isHighlighted = true;
    }



    public void removeHighlight()
    {
      // Material material = GetComponent<Renderer>().material;
      // Material otherMat = GetComponentInChildren<Renderer>().material;
      // ColorUtility.TryParseHtmlString("#5DF3F3", out baseColor);
      // otherMat.color = baseColor;
      Light SpotLight = GetComponentInChildren<Light>();

      SpotLight.intensity = 0;
      _isHighlighted = false;
    }

    public void MoveUnit(Vector3 point)
    {
      agent.SetDestination(point);
    }
  }
}