using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class textwriter : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [TextArea(3,10)]
    [SerializeField] string texttoprint;
     [SerializeField] AudioClip wawa;
    // [SerializeField] AudioClip pickme;
     [SerializeField] AudioSource ass;
    // [SerializeField] Animator anim;
    // [SerializeField] Transform cam;
    // Vector3 targetpos = new Vector3(290.702637f,3.96321726f,-38.779789f);

    bool writting;

    public void domsg(string msg){
        texttoprint = msg;
        if (writting)
            return;
        StartCoroutine(Msg());
    }

    IEnumerator Msg(){
        writting = true;
text.text = "";
        //yield return new WaitForSeconds(1);

        // cam.parent = null;

        // while(Vector3.Distance(cam.position, targetpos) > 1){
        //     yield return null;
        //     cam.position = Vector3.MoveTowards(cam.position, targetpos, 5 * Time.deltaTime);
        // }

        ass.loop = true;
        ass.clip = wawa;
        ass.volume = 0.3f;

        ass.Play();

        foreach(char c in texttoprint){
            if (c == '\\'){
                text.text += "\n";
                continue;
            }
            yield return new WaitForSeconds(0.025f);
            text.text += c;
        }
        ass.Stop();

        // ass.loop = false;
        // ass.clip = pickme;
        // ass.volume = 0.7f;
        // ass.Play();

        writting = false;

    }
}
