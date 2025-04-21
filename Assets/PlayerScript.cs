using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class PlayerScript : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 moveDirection;

    public float speed;
    public GameObject interactionMarker;

    public GameObject boat;
    public bool leftBoat = true;

    // Start is called before the first frame update
    void Start()
    {
       rb = GetComponent<Rigidbody2D>();
       animator = GetComponent<Animator>();

        if (leftBoat)
        {
            transform.parent = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f);

        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            transform.eulerAngles = Vector3.zero;
        }

        if(Input.GetAxisRaw("Vertical") > 0) 
        {
            animator.SetBool("Up", true);
            animator.SetBool("Down", false);
            animator.SetBool("Side", false);
        }
        else if (Input.GetAxisRaw("Vertical") < 0)
        {
            animator.SetBool("Up", false);
            animator.SetBool("Down", true);
            animator.SetBool("Side", false);
        }
        else if(Input.GetAxisRaw("Horizontal") != 0) 
        {
            animator.SetBool("Up", false);
            animator.SetBool("Down", false);
            animator.SetBool("Side", true);
        }
        else
        {
            animator.SetBool("Up", false);
            animator.SetBool("Down", false);
            animator.SetBool("Side", false);
        }


        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            animator.SetFloat("Speed", 1);
        }
        else
        {
            animator.SetFloat("Speed", -1);
        }
    }

    private void FixedUpdate()
    {
        if (leftBoat)
        {
            rb.linearVelocity = moveDirection * speed;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Interactible") 
        { 
            interactionMarker.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Interactible")
        {
            interactionMarker.SetActive(false);
        }
    }

    public void Boat() 
    {
        leftBoat = false;
        animator.SetTrigger("Boat");
        transform.position = boat.transform.position;
        animator.ResetTrigger("Boat");
    }
}
