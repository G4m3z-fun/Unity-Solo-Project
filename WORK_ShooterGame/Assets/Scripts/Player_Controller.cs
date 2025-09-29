using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    Vector2 cameraRotation;
    Vector3 cameraOffset;
    public Vector3 respawnPoint;
    InputAction lookVector;
    Camera playerCam;
    Transform camHolder;
    Ray interactRay;
    Ray jumpRay;
    RaycastHit interactHit;
    Rigidbody rb;
    GameObject pickupObj;

    float verticalMove;
    float horizontalMove;

    public float speed = 5f;
    public float jumpHeight = 10f;
    public float Xsensitivity = 1.0f;
    public float Ysensitivity = 1.0f;
    public float camRotationLimit = 90.0f;
    public float groundDetectLength = .5f;
    public float interactDistance = 5f;

    public int health = 5;
    public int maxHealth = 5;

    public PlayerInput input;
    public Transform weaponSlot;
    public Weapon currentWeapon;

    public bool attacking = false;

    // Camera System
    public void Start()
    {
        jumpRay = new Ray(transform.position, -transform.up);

        cameraOffset = new Vector3(0, 5f, 5f);
        respawnPoint = new Vector3(0, 1, 0);
        rb = GetComponent<Rigidbody>();
        playerCam = Camera.main;
        lookVector = GetComponent<PlayerInput>().currentActionMap.FindAction("Look");
        cameraRotation = Vector2.zero;
        camHolder = transform.GetChild(1);
        weaponSlot = transform.GetChild(0);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (health <= 0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);


        // Camera Roation System

        cameraRotation.x += lookVector.ReadValue<Vector2>().x * Xsensitivity;
        cameraRotation.y += lookVector.ReadValue<Vector2>().y * Ysensitivity;

        cameraRotation.y = Mathf.Clamp(cameraRotation.y, -camRotationLimit, camRotationLimit);

        camHolder.rotation = Quaternion.Euler(-cameraRotation.y, cameraRotation.x, 0);
        transform.rotation = Quaternion.AngleAxis(cameraRotation.x, Vector3.up);

        // Movement System(Controller)

       

        Vector3 temp = rb.velocity;

        temp.x = verticalMove * speed;
        temp.z = horizontalMove * speed;

        jumpRay.origin = transform.position;
        jumpRay.direction = -transform.up;

        interactRay.origin = playerCam.transform.position;
        interactRay.direction = playerCam.transform.forward;

        if (Physics.Raycast(interactRay, out interactHit, interactDistance))
        {
            if (interactHit.collider.gameObject.tag == "weapon")
            {
                pickupObj = interactHit.collider.gameObject;
            }
        }
        else
            pickupObj = null;

        if (currentWeapon)
            if (currentWeapon.holdToAttack && attacking)
                currentWeapon.fire();

        rb.velocity = (temp.x * transform.forward) +
                            (temp.y * transform.up) +
                            (temp.z * transform.right);

    }

    public void Move(InputAction.CallbackContext context)
    {

        Vector2 inputAxis = context.ReadValue<Vector2>();

        verticalMove = inputAxis.y;
        horizontalMove = inputAxis.x;
    }

    public void Jump()
    {
        if (Physics.Raycast(jumpRay, groundDetectLength))
            rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);

    }
    // For when the Player get off grid
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Killzone")
            health = 0;



    }

    // Things that can help/harm the Player

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Hazard")
        {
            health--;
            Destroy(collision.gameObject);
        }


        if ((collision.gameObject.tag == "health") && (health < maxHealth))
        {
            health++;
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "Enemy")
        {
            health--;
        }

        if (collision.gameObject.tag == "BulletObject")
        {
            health--;
        }

    }
    public void Attack(InputAction.CallbackContext context)
    {
        if (currentWeapon)
        {
            if (currentWeapon.holdToAttack)
            {
                if (context.ReadValueAsButton())
                    attacking = true;
                else
                    attacking = false;
            }

            else
                if (context.ReadValueAsButton())
                currentWeapon.fire();
        }
    }
    public void Reload()
    {
        if (currentWeapon)
            if (!currentWeapon.reloading)
                currentWeapon.reload();
    }
    public void Interact()
    {
        if (pickupObj)
        {
            if (pickupObj.tag == "weapon")
            {
                if (currentWeapon)
                    DropWeapon();

                pickupObj.GetComponent<Weapon>().equip(this);
            }
            pickupObj = null;
        }
        else
            Reload();
    }
    public void DropWeapon()
    {
        if (currentWeapon)
        {
            currentWeapon.GetComponent<Weapon>().unequip();
        }
    }



}
