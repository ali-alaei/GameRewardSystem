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

    private int remainigTime;

    public int RemainigTime
    {
        get
        {
            return remainigTime;
        }

        set
        {
            remainigTime = value;
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

    public System.Action<int> GiveRewardAction;

    void Start()

    {
        LoadFromDB();
        GetReward();  
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
       
        print("rewarded time format : " + lastRewardedTime);
        TimeSpan lastRewarded = new TimeSpan(lastRewardedTime);
        TimeSpan timeOfNow = new TimeSpan(DateTime.Now.Ticks);
        print("last: " + lastRewarded.Hours);
        print("now:" + timeOfNow.Hours);
        double totalMin = timeOfNow.Minutes - lastRewarded.Minutes;
        double totalHrs = timeOfNow.Hours - lastRewarded.Hours;
        double totalDays = timeOfNow.Days - lastRewarded.Days;
       
        switch (timeKind)
        {
            
            case TimeType.day:
                if (totalDays == 1)    //means player came to game regularly.
                {
                    return ProgressStates.Progressable;
                }
                else if(totalDays > 1)  //means it's more than one they that player didn't come in game.
                {
                    return ProgressStates.ResetProgress;

                }
                else   // means layer came to game again in one day.
                {
                    return ProgressStates.NoProgress;
                }
                

            case TimeType.hour:
                if (totalHrs == 1)
                {
                    return ProgressStates.Progressable;

                }
                else if(totalHrs > 1)
                {

                    return ProgressStates.ResetProgress;

                }
                else
                {
                    return ProgressStates.NoProgress;

                }

               

            case TimeType.min:

                if (totalMin == 1)
                {
                    return ProgressStates.Progressable;
                }

                else if(totalMin > 1)
                {
                    print("totalmin>1 : progress must be reset now");
                    return ProgressStates.ResetProgress;

                }
                else
                {
                    return ProgressStates.NoProgress;
                       
                }
               
            default:
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
        print("user progress = " + userProgress);
        lastRewardedTime = DateTime.Now.Ticks;
        if (GiveRewardAction!=null)
        {
            GiveRewardAction(userProgress);
        }
    }

    private void SaveToDB()
    {
        PlayerPrefs.SetInt("Progress:" + user_id, userProgress);
        PlayerPrefs.SetString("LastRewarded:" + user_id, lastRewardedTime.ToString());
    }
}
