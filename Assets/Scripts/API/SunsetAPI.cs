using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SunsetAPI : MonoBehaviour
{

    #region Variables
    [SerializeField] private string currentAltitude = "41.015137", currentLongitude = "28.979530";
    [SerializeField] private int maxWaitForLocation = 3;

    //this data is for Istanbul 15.05.2023
    string mockJsonData = @"{
    ""results"": {
        ""sunrise"": ""2:44:51 AM"",
        ""sunset"": ""5:16:01 PM"",
        ""solar_noon"": ""10:00:26 AM"",
        ""day_length"": ""14:31:10"",
        ""civil_twilight_begin"": ""2:15:09 AM"",
        ""civil_twilight_end"": ""5:45:43 PM"",
        ""nautical_twilight_begin"": ""1:36:19 AM"",
        ""nautical_twilight_end"": ""6:24:33 PM"",
        ""astronomical_twilight_begin"": ""12:52:58 AM"",
        ""astronomical_twilight_end"": ""7:07:54 PM""
    },
    ""status"": ""OK""
}";
    #endregion


    void Start()
    {
        StartCoroutine(TryGetLocation());
    }

    private IEnumerator TryGetLocation()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location is not enabled! Will use default altitude and longitude");
            //I could add Permission.RequestUserPermission(Permission.FineLocation); but i assume that the permission
            //is taken on another script in normal game so i decided to not do that in here.
        }
        else
        {
            Input.location.Start();
            while (Input.location.status == LocationServiceStatus.Initializing && maxWaitForLocation > 0)
            {
                yield return new WaitForSeconds(1);
                maxWaitForLocation--;
            }

            if (Input.location.status != LocationServiceStatus.Running)
            {
                Debug.Log("Unable to determine device location. Will use default altitude and longitude");
            }
            else
            {
                currentAltitude = Input.location.lastData.altitude.ToString();
                currentLongitude = Input.location.lastData.longitude.ToString();
                Debug.Log("Location found on Alt : " + currentAltitude + " Long : ");
            }
        }

        StartCoroutine(GetSunsetData());
    }

    private IEnumerator GetSunsetData()
    {
        string uri = $"https://api.sunrise-sunset.org/json?lat={currentAltitude}&lng={currentLongitude}";
        string currentString = "";
        DateTime sunsetUtc = DateTime.Now;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {

            yield return webRequest.SendWebRequest();
            string selectedJsonData = webRequest.result == UnityWebRequest.Result.ConnectionError ? mockJsonData : webRequest.downloadHandler.text;

            SunriseSunsetResponse response = JsonUtility.FromJson<SunriseSunsetResponse>(selectedJsonData);

            sunsetUtc = DateTime.Parse(response.results.sunset).ToLocalTime();

            DateTime now = DateTime.Now;

            if (now > sunsetUtc && now < DateTime.Today.AddDays(1))
            {
                currentString += "Night Pinball Game! ";
            }
            else
            {
                currentString += "Day Pinball Game! ";
            }
        }
        currentString += $"Sunset Starts At {sunsetUtc:HH:mm}";
        EventManager.TriggerEvent(EventKeys.OnAPICallCompleted, new object[] { currentString });
    }
}


[Serializable]
public class SunriseSunsetData
{
    public string sunrise;
    public string sunset;
    public string solar_noon;
    public string day_length;
    public string civil_twilight_begin;
    public string civil_twilight_end;
    public string nautical_twilight_begin;
    public string nautical_twilight_end;
    public string astronomical_twilight_begin;
    public string astronomical_twilight_end;
}

[Serializable]
public class SunriseSunsetResponse
{
    public SunriseSunsetData results;
    public string status;
}


