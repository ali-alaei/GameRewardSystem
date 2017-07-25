using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


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


    [SerializeField]
    private TimeType timeKind;

    [SerializeField]
    private Time favouriteTimes;

    [SerializeField]
    private List<Prize> userPrizes;


    [SerializeField]
    private int user_id;




    [SerializeField]
    [Range(0, 59)]
    private int favourite_Minute_Difference;
    [SerializeField]
    [Range(0, 23)]
    private int favourite_Hour_Difference;



    private string sid;
    private long lastRewardedTime;
    private int userProgress;

    void Start()
    {
        //PlayerPrefs.DeleteAll();
        LoadFromDB();
        GetReward();
        //GetProgress(user_id);
    }

    private void LoadFromDB()
    {
        sid = user_id.ToString();

        if (CheckPlayerExistence())
        {

            lastRewardedTime = long.Parse(PlayerPrefs.GetString("LastRewarded:" + sid));
            userProgress = PlayerPrefs.GetInt("Progress:" + user_id);
        }
        else
        {
            DateTime time1 = new DateTime(2010, 7, 17, 14, 30, 05);
            lastRewardedTime = time1.Ticks;
            userProgress = 0;
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
        //____checking with days____//

        print("rewarded time format : " + lastRewardedTime);
        TimeSpan lastRewarded = new TimeSpan(lastRewardedTime);
        TimeSpan timeOfNow = new TimeSpan(DateTime.Now.Ticks);

        print("last: " + lastRewarded.Minutes);

        print("now:" + timeOfNow.Minutes);


        double minutesDifference = timeOfNow.Minutes - lastRewarded.Minutes;
        double hrsDiff = timeOfNow.Hours - lastRewarded.Hours;
        double totalMin = timeOfNow.TotalMinutes - lastRewarded.TotalMinutes;
        double totalHrs = timeOfNow.TotalHours - lastRewarded.TotalHours;
        //____checking with minutes____//

        switch (timeKind)
        {
            case TimeType.hour:

                if (totalHrs<=favourite_Hour_Difference)
                {
                    if (hrsDiff == favourite_Hour_Difference)
                    {
                        return ProgressStates.Progressable;

                    }
                    else
                    {
                        return ProgressStates.NoProgress;

                    }
                }
                else
                {
                    return ProgressStates.ResetProgress;

                }
                break;
            case TimeType.min:
                if (minutesDifference == favourite_Minute_Difference)
                {

                    if (totalMin <=  59)
                    {
                        return ProgressStates.Progressable;
                    }

                }
                break;
           
        }




        if (minutesDifference > favourite_Minute_Difference)
        {
            return ProgressStates.ResetProgress;
        }
        else
        {
            return ProgressStates.NoProgress;
        }

    }





    public void GetReward()
    {



        ProgressStates state = GetProgressState();

        switch (state)
        {
            case ProgressStates.Progressable:
                userProgress++;
                GiveReward();
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
        print(userPrizes[userProgress - 1].Coin + " coins were awarded to user with user id = " + user_id);
        print(userPrizes[userProgress - 1].Gem + " gems were awarded to user with user id = " + user_id);

        lastRewardedTime = DateTime.Now.Ticks;

    }

    private void SaveToDB()
    {
        PlayerPrefs.SetInt("Progress:" + user_id, userProgress);

        PlayerPrefs.SetString("LastRewarded:" + user_id, lastRewardedTime.ToString());
    }





}
