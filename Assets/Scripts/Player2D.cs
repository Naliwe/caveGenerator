using UnityEngine;
using System.Collections;

public class Player2D : MonoBehaviour
{
    Rigidbody2D _rigidbody;
    Vector2 _velocity;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        _velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * 10;
    }

    void FixedUpdate()
    {
        _rigidbody.MovePosition(_rigidbody.position + _velocity * Time.fixedDeltaTime);
    }
}