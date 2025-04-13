using UnityEngine;

public class burger : MonoBehaviour
{

    public bool isBurger;
    public bool isRedbull;
    public AudioSource a;
    public AudioClip ac;

    void OnTriggerEnter(Collider other)
    {
        other.GetComponent<agenttest>().goingToFoood = false;
        if (isRedbull)
            other.GetComponent<agenttest>().isFlying = true;
        if (isBurger || isRedbull)
            a.PlayOneShot(ac);
        Destroy(gameObject);
    }
}
