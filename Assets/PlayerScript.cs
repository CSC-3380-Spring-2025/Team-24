using UnityEngine;

public class PlayerScript : MonoBehaviour
{
   private Animator animator;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
       animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f);

        transform.position += moveDirection * speed * Time.deltaTime;

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
}
