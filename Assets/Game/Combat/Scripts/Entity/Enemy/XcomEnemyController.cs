using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XcomEnemyController : MonoBehaviour, IXcomCharacterController
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void EnterCoroutine(IEnumerator coroutine)
    {
        throw new System.NotImplementedException();
    }

    public Vector2Int GetPosition()
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(float damage)
    {
        throw new System.NotImplementedException();
    }
}
