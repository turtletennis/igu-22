using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [SerializeField] float moveSpeed = 1;
    [SerializeField] float rotateSpeed = 1;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float projectileTemperatureDelta;
    [SerializeField] float projectileDelay = 0.1f;
    [SerializeField] float projectileForce = 10f;
    Rigidbody2D rigidBody;

    Vector2 inputDirection;
    bool firing;
    float lastFiredAt;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        lastFiredAt = Time.timeSinceLevelLoad;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Fire();
    }

    private void Fire()
    {
        if(firing)
        {
            if(Time.timeSinceLevelLoad - lastFiredAt > projectileDelay)
            {
                var projectile = Instantiate(projectilePrefab,transform.position,transform.rotation);
                Rigidbody2D pRigidBody = projectile.GetComponent<Rigidbody2D>();
                pRigidBody.AddForce( transform.up * projectileForce);
                lastFiredAt = Time.timeSinceLevelLoad;

            }
        }
    }

    private void Move()
    {
        var rotation = rotateSpeed * Time.deltaTime * inputDirection.x;
        rigidBody.angularVelocity = rotateSpeed * inputDirection.x * -1;
        var translation = transform.up * moveSpeed * inputDirection.y;
        //Debug.Log($"Moving in direction {translation}");
        rigidBody.velocity = translation;
    }

    void OnMove(InputValue input)
    {

        inputDirection = input.Get<Vector2>();
        //Debug.Log($"Input: {inputDirection}");
    }

    void OnFire(InputValue input)
    {

        firing = input.Get<float>() > float.Epsilon;
        //Debug.Log($"Input: {inputDirection}");
    }
}
