namespace Lift.Simulator
{
    public enum LiftState
    {
        Idle,           // Stationat la baza
        MovingUp,       // Se deplaseaza in sus
        MovingDown,     // Se deplaseaza in jos
        AtStation,      // A ajuns la statia selectata
        EmergencyStop   // Oprire de urgenta
    }

    public enum StationLevel
    {
        Base = 0,
        Station1 = 1,
        Station2 = 2,
        Station3 = 3,
        Station4 = 4
    }
}