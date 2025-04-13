using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class ObjectSpawnNotifier : MonoBehaviour
{

    string dialog;
    public textwriter tw;
    public TMP_Text text;
    public void NotifyGodAction(string environmentState, string godAction, int points, string profession)
    {
        StartCoroutine(SendToPython(environmentState, godAction, profession));
    }

    public IEnumerator SendToPython(string environment, string action, string profession)
    {
        string url = "http://localhost:5000/object_spawned";

        string jsonPayload = JsonUtility.ToJson(new GodActionData
        {
            environment_state = environment,
            god_action = action,
            char_profession = profession
        });

        using UnityWebRequest req = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Server response: " + req.downloadHandler.text);
            tw.domsg(req.downloadHandler.text);
            text.text = req.downloadHandler.text;
        } else {
            Debug.LogError("Error: " + req.error);
        }
    }

    [System.Serializable]
    public class GodActionData
    {
        public string environment_state;
        public string god_action;
        public string char_profession;
    }
}
