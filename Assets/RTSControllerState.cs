
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

    public Camera _playerCamera;
    public Camera PlayerCamera
    {
      get { return _playerCamera; }
      set { _playerCamera = value; }
    }

    public abstract void AddUnit(UnitController unit);
    public abstract void RemoveUnit(UnitController unit);

    public abstract void ClearUnits();
    public abstract void AddHighlightedUnits();
  }

  class DefaultState : State
  {
    public DefaultState(State state) :
      this(state._playerCamera, state._currentSelectedUnits)
    {
    }

    public DefaultState(Camera _playerCamera, Dictionary<string, UnitController> units)
    {
      this._currentSelectedUnits = units;
      this._playerCamera = _playerCamera;
      Initialize();
    }

    private void Initialize()
    {

    }


    public override void AddUnit(UnitController unit)
    {
      unit.setSelected();
      unit.setHighlight();
      CurrentSelectedUnits.Add(unit.unitId, unit);

    }

    public override void RemoveUnit(UnitController unit)
    {
      unit.setDeselect();
      unit.removeHighlight();
      CurrentSelectedUnits.Remove(unit.unitId);
    }

    public override void ClearUnits()
    {
      foreach (KeyValuePair<string, UnitController> unit in CurrentSelectedUnits)
      {
        unit.Value.setDeselect();
        unit.Value.removeHighlight();

      }
      CurrentSelectedUnits.Clear();
    }

    public override void AddHighlightedUnits()
    {
      CurrentSelectedUnits.Clear();
      foreach (KeyValuePair<string, UnitController> unit in SelectableUnits)
      {
        if (unit.Value.isHighlighted)
        {
          CurrentSelectedUnits.Add(unit.Value.unitId, unit.Value);
        }
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
        CurrentSelectedUnits.Add(unit.unitId, unit);
      }
      public override void RemoveUnit(UnitController unit)
      {
        unit.setDeselect();
        unit.removeHighlight();
        CurrentSelectedUnits.Remove(unit.unitId);
      }



      public override void ClearUnits()
      {
        foreach (KeyValuePair<string, UnitController> unit in CurrentSelectedUnits)
        {
          unit.Value.setDeselect();
          unit.Value.removeHighlight();

        }
        CurrentSelectedUnits.Clear();
      }
      public override void AddHighlightedUnits() { }
    }

  }
  class ControllerState
  {
    private State _state;
    public ControllerState(Camera _playerCamera, Dictionary<string, UnitController> units)
    {
      State state = new DefaultState(_playerCamera, units);
      state.IsClicking = false;
      state.IsHoldingDown = false;
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

