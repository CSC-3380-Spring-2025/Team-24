using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using System.Collections.Specialized;

public class BoatFishing : MonoBehaviour
{
    private GameObject player;
    public bool startBoat = false;
    public bool boatMoving = false;
    public bool move;
    public bool firstLoad;

    private Rigidbody2D rb;
    private Animator camAnimator;
    public float speed;

    public string sceneName;
    public Transform destination;
    public Transform playerDestination;

    private Animator animator;

    void Start()
    {
        boatMoving = true;
        firstLoad = true;
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        camAnimator = Camera.main.GetComponent<Animator>();
        animator =  GetComponent<Animator>();

        animator.ResetTrigger("Arrive");
        animator.ResetTrigger("Leave");
    }

    // Update is called once per frame
    void Update()
    {
        if (firstLoad)
        {
            player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.up * speed;
            rb.linearVelocity = Vector2.up * speed;

            if (Vector2.Distance(transform.position, destination.position) <= 0.2f) 
            {
                rb.linearVelocity = Vector2.zero;
                player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
                

                player.transform.parent = null;
                player.transform.position = playerDestination.position;
                player.GetComponent<PlayerScript>().leftBoat = true;
                
                transform.eulerAngles = new Vector3(0,0,0);

                animator.SetTrigger("Arrive");
                boatMoving = false;
                firstLoad = false;
            }
        }
        else
        {
            if (startBoat && Input.GetKeyDown(KeyCode.E) && !boatMoving)
            {
                animator.SetTrigger("Leave");
                boatMoving = true;
                player.GetComponent<PlayerScript>().Boat();

                StartCoroutine(SailAway());
            }

            if (move)
            {
                player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.down * speed;
                rb.linearVelocity = Vector2.down * speed;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player") 
        {
            startBoat = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            startBoat = false;  
        }
    }

    IEnumerator SailAway() 
    {
        player.transform.parent = transform;
        yield return new WaitForSeconds(0.5f);
        move = true;

        yield return new WaitForSeconds(1.3f);
        camAnimator.SetBool("Fade", true);
        yield return new WaitForSeconds(.7f);
        SceneManager.LoadScene(sceneName);
    }
}
