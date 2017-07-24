using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class RewardSystem : MonoBehaviour {
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

    [SerializeField]
    List<Prize> userPrizes;

    
    [SerializeField]
    private int user_id;

   
    
    private DateTime time1;
    private DateTime time2;

    
    private void PlayerPrefsInit()
    {
        time1 = new DateTime(2017, 7, 17, 14, 59, 05);
        PlayerPrefs.SetString("LastRewarded:10", time1.ToString());
        time2 = new DateTime(2017, 6, 24, 18, 30, 45);
        PlayerPrefs.SetString("LastRewarded:11", time2.ToString());
        PlayerPrefs.SetInt("Progress:10", 1);
        PlayerPrefs.SetInt("Progress:11", 3);
        

    }

    private ProgressStates GetProgressState(int id)
    {
        string sid = id.ToString();
        DateTime lastRewarded = Convert.ToDateTime(PlayerPrefs.GetString("LastRewarded:" + sid));
        int difference = DateTime.Now.DayOfYear - lastRewarded.DayOfYear;
        if (difference == 1 )
        {
            return ProgressStates.Progressable;
        }
        else if(difference > 1)
        {
            return ProgressStates.ResetProgress;
        }
        else
        {
            return ProgressStates.NoProgress;
        }
    }

    public void GetProgress(int uId)
    {
        if (GetProgressState(uId) == ProgressStates.Progressable)
        {
            PlayerPrefs.SetInt("Progress:" + uId, PlayerPrefs.GetInt("Progress:" + uId) + 1);
        }
        else if (GetProgressState(uId) == ProgressStates.ResetProgress)
        {
            PlayerPrefs.SetInt("Progress:" + uId, 0);
        }
        
    }

    private bool CanGetReward(int uId)
    {
        string sid = uId.ToString();
        DateTime lastRewarded = Convert.ToDateTime(PlayerPrefs.GetString("LastRewarded:"+sid));
        int difference = DateTime.Now.DayOfYear - lastRewarded.DayOfYear;
        //print(difference);
        if (difference>=1)
        {
            //print("it works");
            return true;
        }
        else
        {
            //print("wrong answer");
            return false;
        }

    }

    public void GetReward(int uId)
    {       
        //userPrize.setCoin();
        if (CanGetReward(uId))
        {
            //int rand = UnityEngine.Random.Range(0, 10);
            //print("rand = " + rand);
            //if ()
            //{
            //PlayerPrefs.SetInt("UserReward:" + uId, userPrize.Coin);
            //print(userPrize.Coin + " Coin Was Awrarded to user with userID : " + uId);

            //}
            //else if()
            //{
            //PlayerPrefs.SetInt("UserReward:" + uId, userPrize.Gem);
            //print(userPrize.Gem + " Gem Was Awrarded to user with userID : " + uId);
            //}
            int progressState;
            progressState = PlayerPrefs.GetInt("Progress:" + uId);
            if (progressState<=userPrizes.Count)
            {
                print(userPrizes[--progressState].Coin + " coins were awarded to user with user id = " + uId);
                print(userPrizes[--progressState].Gem + " gems were awarded to user with user id = " + uId);

            }
            else
            {
                print("you didn't assign enough prize");
            }

        }
    }
    
    public void RewardDecleration()
    {



    }
    
    // Use this for initialization
	void Start () {
        PlayerPrefsInit();
        GetReward(user_id);       
        //Debug.Log(PlayerPrefs.GetString("LastRewarded:10"));
        //Debug.Log(PlayerPrefs.GetString("LastRewarded:11"));

        //print(Convert.ToDateTime(PlayerPrefs.GetString("LastRewarded:10")));
        //int lastDay = time1.DayOfYear;


    }


}
