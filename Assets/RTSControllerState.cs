
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;


namespace RTSGame
{
  public class RTSControllerState
  {

    public RTSController OwningController;
    public RTSControllerState(RTSController owner)
    {
      OwningController = owner;
    }

    protected Dictionary<string, UnitController> _selectableUnits = new Dictionary<string, UnitController>();
    protected Dictionary<string, UnitController> SelectableUnits
    {
      get { return _selectableUnits; }
    }

    public Dictionary<string, UnitController> _currentSelectedUnits = new Dictionary<string, UnitController>();
    public Dictionary<string, UnitController> GetCurrectSelectedUnits
    {
      get { return _currentSelectedUnits; }
    }

    public void AddSelectableUnit(UnitController unit)
    {
      SelectableUnits.Add(unit.unitId, unit);
    }

    public Dictionary<string, UnitController> GetSelectableUnits()
    {
      return SelectableUnits;
    }

    public void AddUnit(UnitController unit)
    {
      unit.setSelected();
      unit.setHighlight();
      _currentSelectedUnits.Add(unit.unitId, unit);

    }

    public void RemoveUnit(UnitController unit)
    {
      unit.deSelect();
      unit.removeHighlight();
      _currentSelectedUnits.Remove(unit.unitId);
    }

    public void ClearUnits()
    {
      if (_currentSelectedUnits.Count != 0)
      {
        foreach (KeyValuePair<string, UnitController> unit in _currentSelectedUnits)
        {
          unit.Value.deSelect();
          unit.Value.removeHighlight();

        }
        _currentSelectedUnits.Clear();
      }
    }

    public void AddHighlightedUnits()
    {
      foreach (KeyValuePair<string, UnitController> unit in _selectableUnits)
      {
        if (unit.Value.isHighlighted)
        {
          _currentSelectedUnits.Add(unit.Value.unitId, unit.Value);
        }
      }
    }
    public void AddUnitHighlight(UnitController unit)
    {
      unit.setHighlight();
    }

    public void RemoveUnitHightlight(UnitController unit)
    {
      unit.removeHighlight();
    }
  }






}

