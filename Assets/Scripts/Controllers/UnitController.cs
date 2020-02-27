
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

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

    public float range = 10f;

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

    Vector3 currentDestination;

    Animator animator;

    bool canCombat = false;

    void Awake()
    {
      generatedId = this.GetInstanceID().ToString();
      deSelect();
      removeHighlight();
    }

    void Start()
    {
      OwningController.ControllerState.AddSelectableUnit(this);
      animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
      float closeEnough = Vector3.Distance(currentDestination, transform.position);
      if (closeEnough <= 2f)
      { canCombat = true; }


      if (canCombat)
      {

        foreach (KeyValuePair<string, EnemyController> enemy in OwningController.ControllerState.getEnemyUnits())
        {
          float distance = Vector3.Distance(enemy.Value.transform.position, transform.position);
          if (distance <= range)
          {

            // Face the enemy
            Vector3 direction = (enemy.Value.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            // Play animation
            animator.SetBool("inCombat", true);

            // Animation should fire a shoot event


          }
        }
      }
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
      canCombat = false;
      currentDestination = point;
      agent.SetDestination(point);
      animator.SetBool("inCombat", false);
    }
  }
}