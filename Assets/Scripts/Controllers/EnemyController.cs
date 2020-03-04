using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RTSGame;

public class EnemyController : MonoBehaviour
{

  private string generatedId;

  public string unitId
  {
    get
    {
      return generatedId;
    }
  }


  public float lookRadius = 10f;
  public float range = 3f;


  public RTSController TargetPlayerController;

  Dictionary<string, UnitController> possibleTargets = new Dictionary<string, UnitController>();
  Transform target;
  NavMeshAgent agent;
  // Audio Variables
  FMOD.Studio.EventInstance WariningSiren;
  // Start is called before the first frame update

  Animator animator;
  void Awake()
  {
    generatedId = this.GetInstanceID().ToString();
  }

  void Start()
  {
    agent = GetComponent<NavMeshAgent>();

    // Add self to Enemies
    TargetPlayerController.ControllerState.AddEnemyUnit(this);

    // Get Possible Targets
    possibleTargets = TargetPlayerController.ControllerState.GetSelectableUnits();
    // Aduio
    WariningSiren = FMODUnity.RuntimeManager.CreateInstance("event:/Toybox/Logic Test");
    WariningSiren.start();
    animator = GetComponentInChildren<Animator>();
  }

  // Update is called once per frame
  void Update()
  {
    CheckIfEnemyNearBy();
  }

  void CheckIfEnemyNearBy()
  {

    // float closeEnough = Vector3.Distance(currentDestination, transform.position);
    // if (closeEnough <= 2f)
    // { canCombat = true; }


    // if (canCombat)
    // {

    foreach (KeyValuePair<string, UnitController> target in possibleTargets)
    {
      float distance = Vector3.Distance(target.Value.transform.position, transform.position);
      if (distance <= range)
      {

        // Face the enemy
        Vector3 direction = (target.Value.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        // Play animation
        animator.SetBool("inCombat", true);

        // Animation should fire a shoot event
        return;

      }
    }
    // }





    if (possibleTargets.Count > 0)
    {
      foreach (KeyValuePair<string, UnitController> unit in possibleTargets)
      {
        //If the screenPosition of the worldobject is within our selection bounds, we can add it to our selection
        float distance = Vector3.Distance(unit.Value.transform.position, transform.position);

        if (distance <= lookRadius)
        {
          agent.SetDestination(unit.Value.transform.position);
          // Audio: Play siren
          WariningSiren.setParameterByName("Enemy Approaches", 1f);

        }
        else
        {
          // Audio: stop siren
          WariningSiren.setParameterByName("Enemy Approaches", 0f);

        }
      }
    }
  }

  // Editor Settings
  void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, lookRadius);
  }
}
