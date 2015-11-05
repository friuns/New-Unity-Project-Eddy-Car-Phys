using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class Team
{
    public TeamEnum team;
    public int score;
    public int score2 { get { return players.Sum(a => a.pv.scoreInt); } }

    public bool cop { get { return team == 0 && GameType.pursuitOrRace; } }
    public IEnumerable<Player> players
    {
        get { return bs._Game.playersList.Where(a => a.pv.teamEnum == team); }
    }
    public int count { get { return players.Count(); } }
    public override string ToString()
    {
        if (GameType.tdm)
            return team.ToString();

        return team == TeamEnum.Blue ? "Cops" : "Gang";

    }
}
public enum TeamEnum { Blue, Red, Dm, Race }