using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Dan.Main;

public class LeaderBoard : MonoBehaviour
{
    [SerializeField]
    private List<TextMeshProUGUI> names;
    [SerializeField]
    private List<TextMeshProUGUI> scores;

    private string publicLeaderboardKey = "b2b2be78c428b8bca37e1dbded257d92f3031398e1eab6c66eeb95716610cc98";
    string[] badWords = { "hello", "hi" };

    private void Start()
    {
        GetLeaderboard();
    }

    public void GetLeaderboard()
    {
        LeaderboardCreator.GetLeaderboard(publicLeaderboardKey, ((msg) =>
        {
            int loopLength = (msg.Length < names.Count) ? msg.Length : names.Count;
          for(int i=0;i< loopLength; i++)
            {
                names[i].text = msg[i].Username;
                scores[i].text = msg[i].Score.ToString();
            }  
        }));
    }

    public void SetLeaderboardEntry(string username, int score)
    {
        LeaderboardCreator.UploadNewEntry(publicLeaderboardKey, username, score, ((msg) =>
          {
              if (System.Array.IndexOf(badWords, name) != -1) return;
              GetLeaderboard();
          }));
    }
}
