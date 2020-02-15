using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RTSGame;

public class EnemyController : MonoBehaviour
{

  public float lookRadius = 10f;

  public RTSController TargetPlayerController;

  Dictionary<string, UnitController> possibleEnemies = new Dictionary<string, UnitController>();
  Transform target;
  NavMeshAgent agent;
  // Start is called before the first frame update
  void Start()
  {
    agent = GetComponent<NavMeshAgent>();
    possibleEnemies = TargetPlayerController.ControllerState.GetSelectableUnits();
  }

  // Update is called once per frame
  void Update()
  {
    CheckIfEnemyNearBy();
  }

  void CheckIfEnemyNearBy()
  {

    if (possibleEnemies.Count > 0)
    {
      foreach (KeyValuePair<string, UnitController> unit in possibleEnemies)
      {
        //If the screenPosition of the worldobject is within our selection bounds, we can add it to our selection
        float distance = Vector3.Distance(unit.Value.transform.position, transform.position);

        if (distance <= lookRadius)
        {
          agent.SetDestination(unit.Value.transform.position);
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
