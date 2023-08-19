using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Move Setting")]
    [SerializeField] private float MoveScale = 5f;
    [SerializeField] private float MoveBoost = 10f;
    [SerializeField] private float RotateScale = 2f;

    private float moveSpeed = 0f;
    private Rigidbody rigidbody = null;


    [Space(20), Header("Extra Setting")]
    [SerializeField] private Gun GunSystem = null;
    private void Start()
    {
        moveSpeed = MoveScale;
        rigidbody = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
            GunSystem.Shoot();

        if (Input.GetKeyDown(KeyCode.LeftShift))
            moveSpeed = MoveBoost;
        else if (Input.GetKeyUp(KeyCode.LeftShift))
            moveSpeed = MoveScale;

        float YRotate = RotateScale * Input.GetAxis("Mouse X");
        transform.Rotate(0, YRotate, 0);

        Vector3 moveDirection = Vector3.zero;
        moveDirection += transform.right * moveSpeed * Input.GetAxis("Horizontal");
        moveDirection += transform.forward * moveSpeed * Input.GetAxis("Vertical");

        rigidbody.velocity = moveDirection;
    }
}
