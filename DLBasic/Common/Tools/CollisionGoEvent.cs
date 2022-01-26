using System;
using UnityEngine;

public class CollisionGoEvent : MonoBehaviour
{
    public Action<Collider> onTriggerEnter;
    public Action<Collider> onTriggerExit;
    public Action<Collider> onTriggerStay;

    public Action<Collision> onCollisionEnter;
    public Action<Collision> onCollisionExit;
    public Action<Collision> onCollisionStay;

    public bool isCanColl = true;
    private void OnTriggerEnter(Collider other)
    {
        if (!isCanColl) return;
        onTriggerEnter?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isCanColl) return;
        onTriggerExit?.Invoke(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isCanColl) return;
        onTriggerStay?.Invoke(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isCanColl) return;
        onCollisionEnter?.Invoke(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!isCanColl) return;
        onCollisionExit?.Invoke(collision);
    }
    private void OnCollisionStay(Collision collision)
    {
        if (!isCanColl) return;
        onCollisionStay?.Invoke(collision);
    }
}