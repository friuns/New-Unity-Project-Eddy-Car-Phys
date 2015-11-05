using System;

public enum GameTypeEnum { pursuitRace, pursuit, DeathMatch, TDM }
public class GameType
{
    public static bool pursuit { get { return bs.room.gameType == GameTypeEnum.pursuit; } }
    public static bool pursuitOrRace { get { return bs.room.gameType == GameTypeEnum.pursuit || bs.room.gameType == GameTypeEnum.pursuitRace; } }
    public static bool race { get { return bs.room.gameType == GameTypeEnum.pursuitRace; } }
    public static bool weapons { get { return bs.room.gameType == GameTypeEnum.DeathMatch || bs.room.gameType == GameTypeEnum.TDM; } }
    public static bool deathmatch { get { return bs.room.gameType == GameTypeEnum.DeathMatch; } }
    public static bool tdm { get { return bs.room.gameType == GameTypeEnum.TDM; } }
    public static bool teamGame { get { return pursuit || tdm; } }
    public static bool bots { get { return bs._Loader.menuLoaded ? bs.room.bots : bs.settings.bots; } }
}