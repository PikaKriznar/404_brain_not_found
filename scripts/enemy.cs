using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class enemy : MonoBehaviour
{

    [SerializeField]GameObject player;
    NavMeshAgent agent;
    bool goingToPlayer;

    [SerializeField]GameObject destination;
    public GameObject dest;
    
    IEnumerator Start()
    {
        yield return new WaitForSeconds(3);
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(Roam());
        StartCoroutine(GoToPlayer());
        dest = Instantiate(destination);
    }

    IEnumerator Roam(){
        while(true){
            yield return new WaitForSeconds(5f);
            if (goingToPlayer)
                continue;
            Debug.Log(dest.transform.position + "");
            agent.SetDestination(dest.transform.position);
        }
    }

    IEnumerator GoToPlayer(){
        while(true){
            yield return new WaitForSeconds(1f);
            if (Vector3.Distance(transform.position, player.transform.position) < 10){
                goingToPlayer = true;
                agent.SetDestination(player.transform.position);
            }
            else{
                goingToPlayer = false;
            }
        }
    }

}
