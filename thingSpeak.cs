using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ThingSpeakController : MonoBehaviour
{
    public GameObject malfunctionObjectPrefab;
    string temp;
    string pressure;
    string jsonRate;
    public TextMeshProUGUI  temperature;
    public TextMeshProUGUI Pressure;

    void Start()
    {
        StartCoroutine(WaitForRequest());
    }

    void DestroyUI()
    {
        if (temperature != null)
            Destroy(temperature.gameObject);

        if (Pressure != null)
            Destroy(Pressure.gameObject);
    }

    IEnumerator WaitForRequest()
    {
        string url = "https://api.thingspeak.com/channels/2444405/feeds.json?results=1";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string work = www.downloadHandler.text;
                RootObject fields = JsonUtility.FromJson<RootObject>(work);

                jsonRate = fields.feeds[0].field1;
                temp = jsonRate.Substring(0, 2);

                jsonRate = fields.feeds[0].field2;
                pressure = jsonRate.Substring(0, 7);

                Debug.Log("Temperature: " + temp);
                Debug.Log("Pressure: " + pressure);
            }
            else
            {
                Debug.LogError("Error fetching data: " + www.error);
            }
        }
    }

    [System.Serializable]
    public class Channel
    {
        public int id;
        public string name;
        public string latitude;
        public string longitude;
        public string field1;
        public string created;
        public string update;
        public int last_entry_id;
    }

    [System.Serializable]
    public class Feed
    {
        public string created_at;
        public int entry_id;
        public string field1;
        public string field2;
    }

    [System.Serializable]
    public class RootObject
    {
        public Channel channel;
        public List<Feed> feeds;
    }

    void Update()
    {
        if (temperature != null) {
            // Convert temp to a numerical type (float or int) before comparison
            float tempValue = float.Parse(temp);  // Assuming temp is a decimal value

            if(tempValue > 35) {
                temperature.text = "Temp: " + temp + " °C - Status: Abnormal";
                temperature.color = Color.red; // Set text color to red for abnormal temperature
            } else {
                temperature.text = "Temp: " + temp + " °C - Status: Normal";
                temperature.color = Color.green; // Set text color to green for normal temperature
            }
        }

        if (Pressure != null)
            Pressure.text = "Pressure: " + pressure + " hPa";   
    }

}
