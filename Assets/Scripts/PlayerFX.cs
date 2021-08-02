using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerFX : MonoBehaviour
{
    [SerializeField] private GameObject splashPrefab;

    [Header("Kinematic settings")] [SerializeField]
    public AnimationCurve yJump;

    [SerializeField] private float yGroundLevel;

    [SerializeField] private float jumpDuration;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float longJumpDuration;
    [SerializeField] private float longJumpHeight;

    [SerializeField] private AnimationCurve yBuoyonce;
    [SerializeField] private float buoyonceDuration;
    [SerializeField] private float buoyonceHeight;

    [SerializeField] private AnimationCurve yDive;
    [SerializeField] private float diveDuration;
    [SerializeField] private float diveHeight;

    private Animator _animator;

    private static readonly int AnimFallStart = Animator.StringToHash("fallStart");
    private static readonly int AnimJumpStart = Animator.StringToHash("jumpStart");
    private static readonly int AnimGrounded = Animator.StringToHash("grounded");
    private static readonly int AnimDiveStart = Animator.StringToHash("diveStart");

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private IEnumerator RawFX(float fxDuration, float fxScale, AnimationCurve curve)
    {
        float expiredSeconds = 0;
        float progress = 0;

        float yStartPosition = transform.position.y;


        while (progress < 1)
        {
            expiredSeconds += Time.deltaTime;
            progress = expiredSeconds / fxDuration;

            transform.position = new Vector3(
                transform.position.x,
                yStartPosition + curve.Evaluate(progress) * fxScale,
                transform.position.z);
            yield return null;
        }
    }

    public IEnumerator BuoyancyFX()
    {
        return RawBuoyancyFX(buoyonceDuration, buoyonceHeight);
    }

    public IEnumerator LongJumpFX()
    {
        return RawJumpFX(longJumpDuration, longJumpHeight);
    }


    public IEnumerator ShortJumpFX()
    {
        return RawJumpFX(jumpDuration, jumpHeight);
    }

    public IEnumerator DiveFX()
    {
        _animator.SetTrigger(AnimDiveStart);
        return RawDiveFX(diveDuration, diveHeight);
    }


    private IEnumerator RawBuoyancyFX(float fxDuration, float fxScale)
    {
        return RawFX(fxDuration, fxScale, yBuoyonce);
    }


    private IEnumerator RawDiveFX(float fxDuration, float fxScale)
    {
        return RawFX(fxDuration, fxScale, yDive);
    }


    private IEnumerator RawJumpFX(float fxDuration, float maxJumpHeight)
    {
        _animator.SetTrigger(AnimJumpStart);

        bool splashSpawned = false;
        float expiredSeconds = 0;
        float progress = 0;

        float yStartPosition = transform.position.y;

        float previousY = transform.position.y;
        bool goingDownFlag = false;
        float apogee = 0;

        float correction;

        while (progress < 1)
        {
            expiredSeconds += Time.deltaTime;
            progress = expiredSeconds / fxDuration;

            if (!goingDownFlag && previousY > transform.position.y)
            {
                apogee = transform.position.y;
                _animator.SetTrigger(AnimFallStart);
                goingDownFlag = true;
            }

            previousY = transform.position.y;


            if (!goingDownFlag)
            {
                transform.position = new Vector3(
                    transform.position.x,
                    yStartPosition + yJump.Evaluate(progress) * (maxJumpHeight - yStartPosition),
                    transform.position.z
                );

                if (transform.position.y - yGroundLevel < 0.05 && !splashSpawned)
                {
                    Instantiate(splashPrefab, transform.position, Quaternion.identity);
                    splashSpawned = true;
                }
            }
            else
            {
                transform.position = new Vector3(
                    transform.position.x,
                    yStartPosition + yJump.Evaluate(progress) * (apogee - yGroundLevel) +
                    ((apogee - yStartPosition) - (apogee - yGroundLevel)),
                    transform.position.z
                );
            }

            yield return null;
        }
        _animator.SetTrigger(AnimGrounded);
        Instantiate(splashPrefab, transform.position, Quaternion.identity);
    }
}