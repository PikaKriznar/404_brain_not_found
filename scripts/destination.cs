using System.Collections;
using UnityEngine;

public class destination : MonoBehaviour
{

    public float xpos;
    public float negxpos;
    public float zpos;
    public float negzpos;
    IEnumerator Start()
    {
        while (true){
            yield return new WaitForSeconds(Random.Range(5, 10));
            transform.position = new Vector3(Random.Range(negxpos,xpos), 50, Random.Range(negzpos,zpos));
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
        }
    }
}
