using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{

    Rigidbody _rigidbody;
    Vector3 _velocity;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        _velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * 10;
    }

    void FixedUpdate()
    {
        _rigidbody.MovePosition(_rigidbody.position + _velocity * Time.fixedDeltaTime);
    }
}