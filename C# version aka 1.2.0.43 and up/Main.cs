// Simple Traffic Loader Version 1.0
// By ItsClonkAndre

using System;
using System.Collections.Generic;
using GTA;
using GTA.Native;

namespace SimpleTrafficLoader {
    public class Main : Script {

        #region Variables
        private string DataFolderLocation = Game.InstallFolder + "\\scripts\\SimpleTrafficLoader\\";
        private Random rnd;
        private List<string> vehicles;

        private int randomValue;
        private bool showErrors;
        private bool flushEnabled = true;
        #endregion

        #region Methods
        private void LoadVehicles()
        {
            try {
                vehicles.Clear(); // Clears the list

                if (System.IO.File.Exists(DataFolderLocation + "vehicles.txt")) {
                    string[] lines = System.IO.File.ReadAllLines(DataFolderLocation + "vehicles.txt");
                    if (lines.Length != 0) {

                        // Add vehicles to list
                        for (int i = 0; i < lines.Length; i++) {
                            if (!string.IsNullOrWhiteSpace(lines[i])) {
                                if (!lines[i].StartsWith("//")) {
                                    vehicles.Add(lines[i]);
                                }
                            }
                        }

                        // Clear array
                        lines = null;

                    }
                    else {
                        Game.Console.Print("Simple Traffic Loader: Could not load vehicles. 'vehicles.txt' is empty.");
                        if (showErrors) { Game.DisplayText("Error in Simple Traffic Loader. Check the console for details.", 4500); }
                    }
                }
                else {
                    Game.Console.Print("Simple Traffic Loader: Could not load vehicles. 'vehicles.txt' is missing.");
                    if (showErrors) { Game.DisplayText("Error in Simple Traffic Loader. Check the console for details.", 4500); }
                }
            }
            catch (Exception ex) {
                Game.Console.Print("Simple Traffic Loader error while loading vehicles. Details: " + ex.Message);
                if (showErrors) { Game.DisplayText("Error in Simple Traffic Loader. Check the console for details.", 4500); }
            }
        }
        #endregion

        public Main()
        {
            showErrors = Settings.GetValueBool("ShowErrors", "Options", false);

            rnd = new Random(DateTime.Now.Millisecond);
            vehicles = new List<string>();
            LoadVehicles(); // Load all vehicles from file

            this.Interval = Settings.GetValueInteger("FlushTrafficInterval", "Options", 10000);
            this.Tick += Main_Tick;
            this.ConsoleCommand += Main_ConsoleCommand;
        }

        private void Main_ConsoleCommand(object sender, ConsoleEventArgs e)
        {
            switch (e.Command.ToLower()) {
                case "stl:reloadvehicles": // Command to reload the vehicle list
                    Game.Console.Print("Simple Traffic Loader: Reloading vehicle list...");
                    LoadVehicles();
                    break;
                case "stl:getvehiclecount": // Command to get the vehicle count
                    Game.Console.Print("Simple Traffic Loader vehicle count: " + vehicles.Count.ToString());
                    break;
                case "stl:getcurrentvehicleindex": // Command to get the current vehicle index
                    Game.Console.Print("Simple Traffic Loader current vehicle index: " + randomValue.ToString());
                    break;
                case "stl:getvehicle": // Command to get a vehicle from the list by its index
                    try {
                        if (e.ParameterCount == 1) {
                            int index;
                            bool parseSuccessful = int.TryParse(e.Parameter[0].ToString(), out index);
                            if (parseSuccessful) {
                                Game.Console.Print(string.Format("Simple Traffic Loader: Vehicle '{0}' is a '{1}'", index, vehicles[index]));
                            }
                        }
                        else {
                            Game.Console.Print("Simple Traffic Loader: stl:getvehicle usage: stl:getvehicle NUMBER");
                        }
                    }
                    catch (Exception ex) {
                        Game.Console.Print("Simple Traffic Loader: stl:getvehicle error. Details: " + ex.Message);
                    }
                    break;
            }
        }

        private void Main_Tick(object sender, EventArgs e)
        {
            if (flushEnabled) {
                if (vehicles.Count != 0) {
                    try {
                        randomValue = rnd.Next(0, vehicles.Count);
                        Model model = new Model(vehicles[randomValue]);
                        Function.Call("REQUEST_MODEL", model.Hash);
                    }
                    catch (Exception ex) {
                        Game.Console.Print("Simple Traffic Loader error while requesting model. Details: " + ex.Message);
                        if (showErrors) { Game.DisplayText("Error in Simple Traffic Loader. Check the console for details.", 4500); }
                    }
                }
            }
        }

    }
}
