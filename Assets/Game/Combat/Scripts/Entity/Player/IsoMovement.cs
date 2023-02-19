using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsoMovement : MonoBehaviour
{
     private enum State {
        West,
        East,
        North,
        South,
        NorthEast,
        NorthWest,
        SouthEast,
        SouthWest
    }

    [SerializeField] private float _speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");
        Vector2 move = new Vector2(xInput, yInput).normalized;

        transform.Translate(move * _speed * Time.deltaTime);
    }
}
