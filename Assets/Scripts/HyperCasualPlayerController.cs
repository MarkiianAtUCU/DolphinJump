using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[RequireComponent(typeof(Rigidbody), typeof(PlayerFX))]
public class HyperCasualPlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;

    private TouchInputManager _playerInputManager = new TouchInputManager(0.2f);

    private bool _jumpInProgress;

    private delegate IEnumerator FXDunctionDelegate();

    private FXDunctionDelegate _trailingAnimation;
    private Coroutine _divingCoroutine;
    private Coroutine _buoyancyCoroutine;

    private PlayerFX _playerFX;

    private void Start()
    {
        _playerFX = GetComponent<PlayerFX>();

        _playerInputManager.jump += Jump;
        _playerInputManager.longJump += LongJump;
        _playerInputManager.diveStart += DiveStart;
    }

    void DiveStart()
    {
        // Player holding dive button, while in jump -> dive anim should be executed right after jump end
        if (_jumpInProgress)
        {
            _trailingAnimation = _playerFX.DiveFX;
        }
        else
        {
            // Stop buoyancy animation if running, start dive animation
            if (_buoyancyCoroutine != null)
                StopCoroutine(_buoyancyCoroutine);

            _divingCoroutine = StartCoroutine(_playerFX.DiveFX());
        }
    }


    void Jump()
    {
        GeneralJump(_playerFX.ShortJumpFX);
    }

    void LongJump()
    {
        GeneralJump(_playerFX.LongJumpFX);
    }

    void GeneralJump(FXDunctionDelegate fxFunction)
    {
        if (_jumpInProgress)
            return;
        _trailingAnimation = null;
        StopAllCoroutines();
        StartCoroutine(JumpRoutine(fxFunction));
    }


    private IEnumerator JumpRoutine(FXDunctionDelegate fxFunction)
    {
        // jump animation
        _jumpInProgress = true;
        yield return StartCoroutine(fxFunction());
        _jumpInProgress = false;
        
        // if player still holding dive button -> play dive animation, if not -> play bouyancy anim
        if (_trailingAnimation != null)
        {
            if (_buoyancyCoroutine != null)
                StopCoroutine(_buoyancyCoroutine);
            _divingCoroutine = StartCoroutine(_trailingAnimation());
        }
        else
        {
            _buoyancyCoroutine = StartCoroutine(_playerFX.BuoyancyFX());
            yield return _buoyancyCoroutine;
        }
    }


    void Update()
    {
        transform.position += new Vector3(movementSpeed * Time.deltaTime, 0, 0);
        _playerInputManager.Update();
    }
}