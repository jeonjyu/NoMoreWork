using System;
using System.Threading;

public class PlayData
{
    public string _name;
    public int _highScore;
    public string _date;

    public string Name { get => _name; set => _name = value; }
    public int HighScore { get => _highScore; set => _highScore = value; }
    public string Date { get => _date; }

    public PlayData(string name, int highScore)
    {
        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ko-KR");
        _name = name;
        _highScore = highScore;
        _date = DateTime.Now.ToShortDateString();
    }
}
