using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(BoxCollider))]
public class InteractableRing : InteractableObject
{
    private Animator _anim;

    private void Start()
    {
        _anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.score += 1;
            _anim.SetTrigger("Destroy");
        }
    }

    public void SelfDestroy()
    {
        Destroy(gameObject);
    }
}