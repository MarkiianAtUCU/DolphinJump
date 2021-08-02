using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class TouchInputManager
{
    public UnityAction jump;
    public UnityAction longJump;
    public UnityAction diveStart;

    private float _lastTimePressedDown;
    private float _holdTime;
    private bool _isDiving;

    public TouchInputManager(float holdTime)
    {
        _holdTime = holdTime;
    }

    private bool _touchStarted;


    public void Update()
    {
#if UNITY_EDITOR
        SimulationUpdate();
#endif
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                _lastTimePressedDown = Time.time;
                _touchStarted = true;
            }

            if ((touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved) && _touchStarted)
            {
                if (Time.time - _lastTimePressedDown > _holdTime)
                {
                    if (!_isDiving)
                        diveStart.Invoke();

                    _isDiving = true;
                }
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                if (_isDiving)
                {
                    longJump.Invoke();
                    _isDiving = false;
                }
                else
                {
                    jump.Invoke();
                }

                _touchStarted = false;
            }
        }
    }


#if UNITY_EDITOR
    private void SimulationUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _lastTimePressedDown = Time.time;
            _touchStarted = true;
        }

        if (Input.GetKey(KeyCode.Space) && _touchStarted)
        {
            if (Time.time - _lastTimePressedDown > _holdTime)
            {
                if (!_isDiving)
                    diveStart.Invoke();

                _isDiving = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (_isDiving)
            {
                longJump.Invoke();
                _isDiving = false;
            }
            else
            {
                jump.Invoke();
            }

            _touchStarted = false;
        }
    }
#endif
}