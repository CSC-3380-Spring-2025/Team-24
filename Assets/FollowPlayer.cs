
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private GameObject player;

    [SerializeField] private Vector3 offset;
    [SerializeField] private float damping = 0.2f;

    private Vector3 vel = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        SmoothFollow();
    }

    void SmoothFollow()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
    }

    void LagFollow()
    {
        Vector3 targetPosi = player.transform.position + offset;
        targetPosi.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosi, ref vel, damping);
    }
}
