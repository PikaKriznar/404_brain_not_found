using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class agenttest : MonoBehaviour
{
    NavMeshAgent agent;
    public Transform destination;

    Collider[] colliders;

    public LayerMask foodmask;
    public bool goingToFoood;

    public LayerMask enemymask;
    public bool sawEnemy;
    public bool isFlying;
    public GameObject wings;

    GameObject runfromenemydest;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(movearound());
        StartCoroutine(foodlocator());
        StartCoroutine(enemylocator());
        StartCoroutine(flying());
    }

    IEnumerator movearound(){
        while(true){
            yield return new WaitForSeconds(2);
            if (goingToFoood || isFlying){continue;}
            agent.SetDestination(destination.position);
        }
    }

    IEnumerator foodlocator(){
        while(true){
            yield return new WaitForSeconds(3);
            colliders = Physics.OverlapSphere(transform.position, 10, foodmask);
            if (colliders.Length > 0){
                agent.SetDestination(colliders[0].transform.position);
                goingToFoood = true;
            }
        }
    }

    IEnumerator enemylocator(){
        while(true){
            yield return new WaitForSeconds(3.5f);
            colliders = Physics.OverlapSphere(transform.position, 10, enemymask);
            if (colliders.Length > 0){
                sawEnemy = true;
                runfromenemydest.transform.position = new Vector3(2 * transform.position.x - colliders[0].transform.position.x, 50, 2 * transform.position.z - colliders[0].transform.position.z);
                Ray ray = new Ray(transform.position, -transform.up);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
                {
                    transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z);
                }
                else
                {
                    ray = new Ray(transform.position, transform.up);
                    if (Physics.Raycast(ray, out hitInfo))
                    {
                        transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z);
                    }
                }
                agent.SetDestination(runfromenemydest.transform.position);
            }
            else{
                sawEnemy = false;
            }
        }
    }

    IEnumerator flying(){
        while(true){
            yield return new WaitForSeconds(1.4f);
            if (isFlying){
                wings.SetActive(true);
                agent.enabled = false;
                int i = 0;
                while(i < 10){
                    yield return new WaitForSeconds(1f);
                    Debug.Log(transform.position);
                    transform.position = new Vector3(0, transform.position.y + 0.1f, 0);
                    Debug.Log(transform.position);
                    i++;
                }
                while(i > 0){
                    yield return new WaitForSeconds(0.1f);
                    transform.position = new Vector3(0, transform.position.y - 0.1f, 0);
                    i--;
                }
                wings.SetActive(false);
                
                agent.enabled = true;
                isFlying = false;
            }
        }
    }
}
