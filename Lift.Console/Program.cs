using Lift.Simulator;

Console.WriteLine("=== Simulator Lift M42 ===\n");

var lift = new LiftProcess();

// Ne abonam la evenimente
lift.OnEventLogged += (msg) => { /* deja se printeaza in LogEvent */ };
lift.OnStateChanged += (state) => Console.WriteLine($"  >> Stare noua: {state}\n");

lift.Start();
Thread.Sleep(500);

Console.WriteLine("--- Test 1: Selectam nivelul 3 ---");
lift.SelectLevel(3);
Thread.Sleep(5000); // asteptam sa ajunga

Console.WriteLine("\n--- Test 2: Coboram la baza ---");
lift.MoveToBase();
Thread.Sleep(5000);

Console.WriteLine("\n--- Test 3: Oprire urgenta ---");
lift.SelectLevel(2);
Thread.Sleep(1000);
lift.EmergencyStop();
Thread.Sleep(1000);

Console.WriteLine("\n--- Test 4: Selectare refuzata dupa urgenta ---");
lift.SelectLevel(1);
Thread.Sleep(500);

Console.WriteLine("\n--- Test 5: Coboram la baza dupa urgenta ---");
lift.MoveToBase();
Thread.Sleep(4000);

Console.WriteLine("\n--- Test 6: Selectam nivel 1 dupa revenire ---");
lift.SelectLevel(1);
Thread.Sleep(3000);

lift.Stop();
Console.WriteLine("\nSimulator oprit. Apasa orice tasta...");
Console.ReadKey();