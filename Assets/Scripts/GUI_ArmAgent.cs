using UnityEngine;

public class GUI_ArmAgent : MonoBehaviour
{
    [SerializeField] private ArmAgent _armAgent;

    private GUIStyle _defaultStyle = new GUIStyle();
    private GUIStyle _positiveStyle = new GUIStyle();
    private GUIStyle _negativeStyle = new GUIStyle();

    void Start()
    {
        // This is where you can initialize your GUI elements or other components
        _defaultStyle.fontSize = 20;
        _defaultStyle.normal.textColor = Color.yellow;

        _positiveStyle.fontSize = 20;
        _positiveStyle.normal.textColor = Color.green;

        _negativeStyle.fontSize = 20;
        _negativeStyle.normal.textColor = Color.red;
    }

    private void OnGUI()
    {
        string debugEpisode = "Episode: " + _armAgent.CurrentEpisode.ToString();
        string debugReward = "Reward: " + _armAgent.CumulativeReward.ToString(); 

        GUIStyle rewardStyle = _armAgent.CumulativeReward > 0 ? _positiveStyle : _negativeStyle;

        GUI.Label(new Rect(20, 20, 500, 30), debugEpisode, _defaultStyle);
        GUI.Label(new Rect(20, 60, 500, 30), debugReward, rewardStyle);
    }
}
