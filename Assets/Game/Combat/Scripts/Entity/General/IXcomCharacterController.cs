using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IXcomCharacterController
{
    public abstract void EnterCoroutine(IEnumerator coroutine);
    public abstract Vector2Int GetPosition();
    public abstract void TakeDamage(float damage);
}
