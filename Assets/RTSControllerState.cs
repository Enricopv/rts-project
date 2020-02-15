
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;


namespace RTSGame
{
  abstract class State
  {
    public float _delay = 0.1f;
    public float _clickTime = 0f;

    public Dictionary<string, UnitController> _selectableUnits;
    public Dictionary<string, UnitController> SelectableUnits
    {
      get { return _selectableUnits; }
      set { _selectableUnits = value; }
    }

    public Dictionary<string, UnitController> _currentSelectedUnits;
    public Dictionary<string, UnitController> CurrentSelectedUnits
    {
      get { return _currentSelectedUnits; }
      set { _currentSelectedUnits = value; }
    }

    public Vector3 _startScreenPos;
    public Vector3 StartScreenPos
    {
      get { return _startScreenPos; }
      set { _startScreenPos = value; }
    }

    protected bool _isClicking;
    public bool IsClicking
    {
      get { return _isClicking; }
      set { _isClicking = value; }
    }

    protected bool _isHoldingDown;
    public bool IsHoldingDown
    {
      get { return _isHoldingDown; }
      set { _isHoldingDown = value; }
    }

    // public Camera _playerCamera;
    // public Camera PlayerCamera
    // {
    //   get { return _playerCamera; }
    //   set { _playerCamera = value; }
    // }

    public abstract void AddUnit(UnitController unit);
    public abstract void RemoveUnit(UnitController unit);

    public abstract void ClearUnits();
    public abstract void AddHighlightedUnits();

    public abstract void AddUnitHighlight(UnitController unit);
    public abstract void RemoveUnitHightlight(UnitController unit);



  }

  class DefaultState : State
  {
    // public DefaultState(State state)
    // {
    // }

    public DefaultState()
    {
      // this._currentSelectedUnits = units;
      // this._playerCamera = _playerCamera;
      Initialize();
    }

    private void Initialize()
    {

    }


    public override void AddUnit(UnitController unit)
    {
      unit.setSelected();
      unit.setHighlight();
      _currentSelectedUnits.Add(unit.unitId, unit);

    }

    public override void RemoveUnit(UnitController unit)
    {
      unit.setDeselect();
      unit.removeHighlight();
      _currentSelectedUnits.Remove(unit.unitId);
    }

    public override void ClearUnits()
    {
      if (_currentSelectedUnits.Count != 0)
      {
        foreach (KeyValuePair<string, UnitController> unit in _currentSelectedUnits)
        {
          unit.Value.setDeselect();
          unit.Value.removeHighlight();

        }
        _currentSelectedUnits.Clear();
      }

    }

    public override void AddHighlightedUnits()
    {
      foreach (KeyValuePair<string, UnitController> unit in _selectableUnits)
      {
        if (unit.Value.isHighlighted)
        {
          _currentSelectedUnits.Add(unit.Value.unitId, unit.Value);
        }
      }
    }
    public override void AddUnitHighlight(UnitController unit)
    {
      unit.setHighlight();
    }

    public override void RemoveUnitHightlight(UnitController unit)
    {
      unit.removeHighlight();
    }
  }

  class SelectedState : State
  {
    public SelectedState(State state)
    {
      Initialize();
    }

    private void Initialize()
    {

    }


    public override void AddUnit(UnitController unit)
    {
      unit.setSelected();
      unit.setHighlight();
      _currentSelectedUnits.Add(unit.unitId, unit);
    }
    public override void RemoveUnit(UnitController unit)
    {
      unit.setDeselect();
      unit.removeHighlight();
      _currentSelectedUnits.Remove(unit.unitId);
    }



    public override void ClearUnits()
    {
      foreach (KeyValuePair<string, UnitController> unit in _currentSelectedUnits)
      {
        unit.Value.setDeselect();
        unit.Value.removeHighlight();

      }
      _currentSelectedUnits.Clear();
    }
    public override void AddUnitHighlight(UnitController unit)
    {
      unit.removeHighlight();
    }

    public override void RemoveUnitHightlight(UnitController unit)
    {
      unit.removeHighlight();
    }
    public override void AddHighlightedUnits()
    {
      _currentSelectedUnits.Clear();
      foreach (KeyValuePair<string, UnitController> unit in _selectableUnits)
      {
        if (unit.Value.isHighlighted)
        {
          _currentSelectedUnits.Add(unit.Value.unitId, unit.Value);
        }
      }
    }


  }
  class ControllerState
  {
    private State _state;
    public ControllerState()
    {
      State state = new DefaultState();
      state.IsClicking = false;
      state.IsHoldingDown = false;
      state.SelectableUnits = new Dictionary<string, UnitController>();
      state.CurrentSelectedUnits = new Dictionary<string, UnitController>();
      // state.PlayerCamera = _playerCamera;
      // state.CurrentSelectedUnits = new Dictionary<string, UnitController>();

      this._state = state;
    }

    public State State
    {
      get { return _state; }
      set { _state = value; }
    }


  }

}