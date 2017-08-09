using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;


public class RewardSystem : MonoBehaviour
{
    [System.Serializable]
    private struct Prize
    {
        [SerializeField]
        private int coin;
        [SerializeField]
        private int gem;

        public int Gem
        {
            get
            {
                return gem;
            }

            set
            {
                gem = value;
            }
        }

        public int Coin
        {
            get
            {
                return coin;
            }

            set
            {
                coin = value;
            }
        }
    }

    static class TimeConstants
    {
        public const int basedRewardHour = 23;
        public const int basedResetHour = 47;
        public const int basedMinutes = 59;
        public const int basedSeconds = 60;
    }



    enum ProgressStates
    {
        Progressable,
        ResetProgress,
        NoProgress
    };

    private enum TimeType
    {
        min,
        hour,
        day,

    }


    private TimeSpan remainedTime;
    public TimeSpan RemainedTime
    {
        get
        {
            return remainedTime;
        }

        set
        {
            remainedTime = value;
        }
    }



    private TimeSpan resetTime;
    public TimeSpan ResetTime
    {
        get
        {
            return resetTime;
        }

        set
        {
            resetTime = value;
        }
    }

    [SerializeField]
    private int user_id;


    [SerializeField]
    private List<Prize> userPrizes;

    [SerializeField]
    private TimeType timeKind;

    [SerializeField]
    private Time favouriteTimes;
    private string sid;
    private long lastRewardedTime;
    private int userProgress;
    private TimeSpan timeOfNow;
    private string result;

    public System.Action<int, TimeSpan, TimeSpan> GiveRewardAction;

    void Start()

    {
        //PlayerPrefs.DeleteAll();
        //TimeSpan timeOfNow = new TimeSpan(DateTime.Now.Ticks);
        //print("system time : " + timeOfNow.TotalMinutes);
        StartCoroutine(GetTime(GetTimeCallBack));
        
        //TimeSpan tmp_2 = new TimeSpan(11, 00, 00);
        //int diff = (int)tmp_2.TotalHours - (int)tmp_1.TotalHours;
        //double diff_1 = tmp_2.TotalHours - tmp_1.TotalHours;
        //print(tmp_2.TotalHours);
        //print(tmp_1.TotalHours);
        //print((int)tmp_2.TotalHours);
        //print((int)tmp_1.TotalHours);

        //print("int hrs diff : " + diff);
        //print("double hrs diff : " + diff_1);
        
    }

    void GetTimeCallBack(TimeSpan result)
    {

        timeOfNow = result;
        //print("get time callback" + timeOfNow);
        // print("::::" + timeOfNow.TotalMinutes);

        LoadFromDB();
        GetReward();
    }

    private void LoadFromDB()
    {
        sid = user_id.ToString();

        if (CheckPlayerExistence())
        {
            //print("player existed");
            lastRewardedTime = long.Parse(PlayerPrefs.GetString("LastRewarded:" + sid));
            TimeSpan tmp = new TimeSpan(lastRewardedTime);
            print("last rewarded time from playerpref : " + tmp);
            userProgress = PlayerPrefs.GetInt("Progress:" + user_id);
        }
        else
        {
            print("player didn't exist");
            DateTime initialTime = new DateTime(2010, 7, 17, 14, 30, 05);
            lastRewardedTime = initialTime.Ticks;
            userProgress = 1;
            SaveToDB();
        }

    }

    private bool CheckPlayerExistence()
    {
        if (PlayerPrefs.HasKey("LastRewarded:" + user_id))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private ProgressStates GetProgressState()
    {
        int remainingRewardHours = 0;
        int remainingResetHours = 0;
        int remainingMinutes = 0;
        int remainingSeconds = 0;
        
        TimeSpan lastRewarded = new TimeSpan(lastRewardedTime);
        print(" last get progress state : " + lastRewarded);
        int totalMin = (int)timeOfNow.TotalMinutes - (int)lastRewarded.TotalMinutes;
        //print(" timeOfNow : " + timeOfNow);
        //print(" lasrewarded  : " + lastRewarded);
        //print(" timeOfNow tth : " + timeOfNow.TotalHours);
        //print(" lasrewarded tth : " + lastRewarded.TotalHours);
        int totalHrs = (int)timeOfNow.TotalHours - (int)lastRewarded.TotalHours;
        //print(" timeofnow days : " + timeOfNow.Days);
        //print(" lastrewarded days : " + lastRewarded.Days);
        double totalDays = timeOfNow.Days - lastRewarded.Days;
        
        //just for test********
        //TimeSpan tmp_1 = new TimeSpan(10,11, 00, 00);
        //TimeSpan tmp_2 = new TimeSpan(12, 13, 00, 00);
        //print(" timeofnow days : " + tmp_1.Days);
        //print(" lastrewarded days : " + tmp_2.Days);
        //double totalDays = tmp_2.Days - tmp_1.Days;
        //***********************


        switch (timeKind)
        {

            case TimeType.day:
                remainingRewardHours = TimeConstants.basedRewardHour - timeOfNow.Hours;
                remainingResetHours = TimeConstants.basedResetHour - timeOfNow.Hours;
                remainingMinutes = TimeConstants.basedMinutes - timeOfNow.Minutes;
                remainingSeconds = TimeConstants.basedSeconds - timeOfNow.Seconds;
                RemainedTime = new TimeSpan(remainingRewardHours, remainingMinutes, remainingSeconds);
                ResetTime = new TimeSpan(remainingResetHours, remainingMinutes, remainingSeconds);
                print("total days : " + totalDays);
                print("day_remained reward time : " + remainedTime);
                print("day_remained reset time : " + resetTime);
                if (totalDays == 1)    //means player came to game regularly.
                {
                    return ProgressStates.Progressable;
                }
                else if (totalDays > 1)  //means it's more than one they that player didn't come in game.
                {

                    return ProgressStates.ResetProgress;

                }
                else   // means layer came to game again in one day.
                {
                    return ProgressStates.NoProgress;
                }


            case TimeType.hour:
                remainingMinutes = TimeConstants.basedMinutes - timeOfNow.Minutes;
                remainingSeconds = TimeConstants.basedSeconds - timeOfNow.Seconds;
                RemainedTime = new TimeSpan(remainingRewardHours, remainingMinutes, remainingSeconds);
                ResetTime = new TimeSpan(1, remainingMinutes, remainingSeconds);
                //print("now : " + timeOfNow);
                //print("hour_remained time" + RemainedTime);
                //print("hour_reset time" + ResetTime);
                print("total hours : " + totalHrs);
                if (totalHrs == 1)
                {

                    return ProgressStates.Progressable;

                }
                else if (totalHrs > 1)
                {

                    return ProgressStates.ResetProgress;

                }
                else
                {

                    return ProgressStates.NoProgress;

                }



            case TimeType.min:
                remainingSeconds = TimeConstants.basedSeconds - timeOfNow.Seconds;
                RemainedTime = new TimeSpan(remainingRewardHours, remainingMinutes, remainingSeconds);
                ResetTime = new TimeSpan(remainingRewardHours, 1, remainingSeconds);
                print("now : " + timeOfNow);
                print("minute_remained time : " + RemainedTime);
                print("minute_remained reset time : " + resetTime);
                if (totalMin == 1)
                {

                    return ProgressStates.Progressable;
                }

                else if (totalMin > 1)
                {

                    //print("totalmin>1 : progress must be reset now");
                    return ProgressStates.ResetProgress;

                }
                else
                {

                    return ProgressStates.NoProgress;

                }

            default:
                RemainedTime = new TimeSpan(remainingRewardHours, remainingMinutes, remainingSeconds);
                print("now : " + timeOfNow);
                print("other_remained time" + RemainedTime);
                return ProgressStates.NoProgress;


        }
    }

    public void GetReward()
    {
        ProgressStates state = GetProgressState();

        switch (state)
        {
            case ProgressStates.Progressable:
                if (userProgress < userPrizes.Count)
                {
                    userProgress++;
                    GiveReward();
                }
                else if (userProgress == userPrizes.Count)
                {
                    GiveReward();
                }
                
                break;
            case ProgressStates.ResetProgress:
                userProgress = 1;
                GiveReward();
                break;
            case ProgressStates.NoProgress:
                break;
            default:
                break;
        }

        SaveToDB();
    }

    private void GiveReward()
    {
        //print("Get Prized...");
        print("user progress : " + userProgress);
        print(userPrizes[userProgress - 1].Coin + " coins were awarded to user with user id = " + user_id);
        print(userPrizes[userProgress - 1].Gem + " gems were awarded to user with user id = " + user_id);
        //print("user progress = " + userProgress);
        lastRewardedTime = timeOfNow.Ticks; //time of now must be taken from servers not system.
        TimeSpan lastRewarded = new TimeSpan(lastRewardedTime);
        print("lastRewardedTime updated : " + lastRewarded);
        if (GiveRewardAction != null)
        {
            GiveRewardAction(userProgress, RemainedTime, ResetTime);
        }
    }

    private void SaveToDB()
    {
        PlayerPrefs.SetInt("Progress:" + user_id, userProgress);
        PlayerPrefs.SetString("LastRewarded:" + user_id, lastRewardedTime.ToString());
    }

    IEnumerator GetGoogleTime()
    {
        UnityWebRequest req;
        req = UnityWebRequest.Get("http://google.com");
        Dictionary<string, string> date;
        result = string.Empty;
        yield return req.Send();
        if (!req.isError)
        {
            date = req.GetResponseHeaders();
            if (date.ContainsKey("Date"))
            {
                result = date["Date"];
                //Debug.Log("Google Time Successfull");
            }
        }
        yield return result;
    }

    IEnumerator GetCurrentmillisTime()
    {
        UnityWebRequest req;
        req = UnityWebRequest.Get("http://currentmillis.com/time/minutes-since-unix-epoch.php");
        result = string.Empty;
        yield return req.Send();
        if (!req.isError)
        {
            result = req.downloadHandler.text;
            Debug.Log("Currentmillis Successfull");

        }
        yield return result;
    }

    IEnumerator GetTime(System.Action<TimeSpan> resultAction)
    {
        TimeSpan timeOfNow;
        yield return GetGoogleTime();
        if (result != string.Empty)
        {

            DateTime time = Convert.ToDateTime(result);

            timeOfNow = new TimeSpan(time.Ticks);

        }
        else
        {
            yield return GetCurrentmillisTime();

            if (result != string.Empty)
            {
                DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                epoch = epoch.AddMinutes(Double.Parse(result));
                timeOfNow = new TimeSpan(epoch.Ticks);
            }
            else
            {
                timeOfNow = new TimeSpan(System.DateTime.Now.Ticks);
            }

        }
        if (resultAction != null)
            resultAction(timeOfNow);


        //UnityWebRequest req;
        //req = UnityWebRequest.Get("http://google.com");
        //Dictionary<string, string> date;

        //yield return req.Send();
        //if (req.isError)
        //{
        //    Debug.Log(req.error);
        //    req = UnityWebRequest.Get("http://currentmillis.com/time/minutes-since-unix-epoch.php");
        //    yield return req.Send();
        //    if (req.isError)
        //    {
        //        Debug.Log(req.error);
        //        timeOfNow = new TimeSpan(DateTime.Now.Ticks);
        //    }
        //    else
        //    {
        //        date = req.GetResponseHeaders();
        //        string tmp = date["Date"];
        //        DateTime time = Convert.ToDateTime(tmp);
        //        timeOfNow = new TimeSpan(time.Ticks);
        //        print("currentMilis time : " + timeOfNow.Hours);

        //    }
        //}

        //else
        //{
        //    date = req.GetResponseHeaders();
        //    string tmp = null;          
        //    if (date.ContainsKey("Date"))
        //    {
        //        tmp = date["Date"];
        //    }
        //    else
        //    {

        //    }
        //    DateTime time = Convert.ToDateTime(tmp);
        //    timeOfNow = new TimeSpan(time.Ticks);
        //    print("google time : " + timeOfNow.Hours);
        //}

        /*if (!req.isError)
        {
           
        }
        else
        {
            req = UnityWebRequest.Get("http://currentmillis.com/time/minutes-since-unix-epoch.php");
            yield return req.Send();
            if (!req.isError)
            {

            }
            else
            {

            }
        }

        UnityWebRequest www = UnityWebRequest.Get("http://currentmillis.com/time/minutes-since-unix-epoch.php");
        UnityWebRequest www2 = UnityWebRequest.Get("http://google.com");

        yield return www.Send();
        yield return www2.Send();

        if (www.isError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text

            string EpochMin = www.downloadHandler.text;
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            epoch = epoch.AddMinutes(Double.Parse(EpochMin));
            print(epoch);

            Dictionary<String, string> test = www2.GetResponseHeaders();
            Debug.Log(test["Date"]);
            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
        }*/


    }
}



