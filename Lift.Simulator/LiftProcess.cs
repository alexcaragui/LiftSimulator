namespace Lift.Simulator
{
    public class LiftProcess
    {
        // Starea curenta
        public LiftState CurrentState { get; private set; } = LiftState.Idle;
        public StationLevel CurrentLevel { get; private set; } = StationLevel.Base;
        public StationLevel? TargetLevel { get; private set; } = null;

        // Lampile P1-P4 (true = aprinsa)
        public bool[] Lamps { get; private set; } = new bool[5]; // index 1-4

        // Senzori
        public bool SensorB10 => CurrentLevel == StationLevel.Base; // senzor baza
        public bool SensorB11 { get; private set; } = false;        // senzor protectie

        // Evenimente
        public event Action<string>? OnEventLogged;
        public event Action<LiftState>? OnStateChanged;

        private bool _emergencyStop = false;
        private Thread? _processThread;
        private bool _running = false;

        // Porneste simulatorul pe un thread separat
        public void Start()
        {
            _running = true;
            _processThread = new Thread(ProcessLoop)
            {
                IsBackground = true,
                Name = "LiftProcessThread"
            };
            _processThread.Start();
            LogEvent("Simulator pornit.");
        }

        // Opreste simulatorul
        public void Stop()
        {
            _running = false;
            LogEvent("Simulator oprit.");
        }

        // Buton S0 - oprire urgenta
        public void EmergencyStop()
        {
            _emergencyStop = true;
            ChangeState(LiftState.EmergencyStop);
            LogEvent("OPRIRE DE URGENTA activata.");
        }

        // Butoane S1-S4 - selectare nivel
        public bool SelectLevel(int level)
        {
            if (level < 1 || level > 4) return false;

            if (!SensorB10 && TargetLevel == null)
            {
                LogEvent($"Selectare nivel {level} refuzata - liftul nu e la baza.");
                return false;
            }

            if (_emergencyStop)
            {
                LogEvent($"Selectare nivel {level} refuzata - oprire de urgenta activa.");
                return false;
            }

            TargetLevel = (StationLevel)level;
            Lamps[level] = true;
            LogEvent($"Nivel {level} selectat. Lampa P{level} aprinsa.");
            return true;
        }

        // Buton S5 - coborare la baza (continuu)
        public void MoveToBase()
        {
            if (CurrentLevel == StationLevel.Base)
            {
                LogEvent("Liftul e deja la baza.");
                return;
            }

            _emergencyStop = false;
            TargetLevel = StationLevel.Base;
            ChangeState(LiftState.MovingDown);
            LogEvent("Coborare la baza initiata (S5 apasat).");
        }

        // Loop-ul principal al procesului
        private void ProcessLoop()
        {
            while (_running)
            {
                if (_emergencyStop)
                {
                    Thread.Sleep(500);
                    continue;
                }

                switch (CurrentState)
                {
                    case LiftState.Idle:
                        if (TargetLevel.HasValue && TargetLevel != StationLevel.Base)
                        {
                            ChangeState(LiftState.MovingUp);
                            LogEvent($"Pornire deplasare catre statia {(int)TargetLevel}.");
                        }
                        break;

                    case LiftState.MovingUp:
                        Thread.Sleep(1000); // 1 secunda per nivel simulat
                        var nextUp = CurrentLevel + 1;
                        CurrentLevel = (StationLevel)nextUp;
                        LogEvent($"Lift la nivelul {(int)CurrentLevel}.");

                        if (CurrentLevel == TargetLevel)
                        {
                            ChangeState(LiftState.AtStation);
                            LogEvent($"Lift ajuns la statia {(int)CurrentLevel}.");
                        }
                        break;

                    case LiftState.MovingDown:
                        Thread.Sleep(1000);
                        var nextDown = CurrentLevel - 1;
                        CurrentLevel = (StationLevel)nextDown;
                        LogEvent($"Lift la nivelul {(int)CurrentLevel}.");

                        if (CurrentLevel == StationLevel.Base)
                        {
                            // Stinge toate lampile la revenirea la baza
                            for (int i = 1; i <= 4; i++) Lamps[i] = false;
                            TargetLevel = null;
                            ChangeState(LiftState.Idle);
                            LogEvent("Lift ajuns la baza. Gata pentru o noua cerere.");
                        }
                        break;

                    case LiftState.AtStation:
                        // Asteapta S5 pentru coborare
                        Thread.Sleep(500);
                        break;
                }

                Thread.Sleep(100);
            }
        }

        private void ChangeState(LiftState newState)
        {
            CurrentState = newState;
            OnStateChanged?.Invoke(newState);
        }

        private void LogEvent(string message)
        {
            var entry = $"[{DateTime.Now:HH:mm:ss}] {message}";
            Console.WriteLine(entry);
            OnEventLogged?.Invoke(entry);
        }
    }
}