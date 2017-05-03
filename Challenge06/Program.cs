using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Challenge06
{
    class Program
    {
        const string TestInputFileName = "./testinput.txt";

        static void Main1(string[] args)
        {
            try
            {
                var cases = ParseFileInput(TestInputFileName);

                foreach (var kase in cases)
                {
                    //Console.WriteLine($"Case #{kase.Id}: {kase.CalculateYearsToBecomeRanker()}");
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        static List<Problem> ParseFileInput(string path)
        {
            var output = new List<Problem>();
            var input = File.ReadAllLines(path);

            // Is the input in the correct format?
            // C (number of cases)
            // F S (f: number of floors, s: number of shortcuts)
            // [... S]
            // A B Y (a: from shortcut, b: to shortcut, y: years taken)
            // [/... S]

            if (input.Length <= 1)
            {
                throw new Exception("Error while parsing input file.");
            }

            // Start at line 1, because line 0
            // is the total number of cases
            for (int caseId = 0, line = 1; line < input.Length;)
            {
                // Increase case ID
                caseId++;

                var bits = input[line++].Split(
                    new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries
                );

                var floors = int.Parse(bits[0]);
                var sctnum = int.Parse(bits[1]);
                var shrcts = new Shortcut[sctnum];

                for (var idx = 0; idx < sctnum; idx++)
                {
                    var shortcut = input[line++].Split(
                        new char[] { ' ' },
                        StringSplitOptions.RemoveEmptyEntries
                    );

                    var floorA = int.Parse(shortcut[0]);
                    var floorB = int.Parse(shortcut[1]);
                    var years = int.Parse(shortcut[2]);

                    shrcts[idx] = new Shortcut(floorA, floorB, years);
                }

                output.Add(new Problem(caseId, floors, sctnum, shrcts));
            }

            return output;
        }
    }

    class Problem
    {
        public int Id
        {
            get;
        }

        private int CurrentFloor;
        private int LastExploredFloor; // F
        private int NumberOfShortcuts; // S

        private int MaxHypotheticYears;

        private Shortcut[] Shortcuts;

        const int MinNumberOfYears = 1;
        const int MaxNumberOfYears = int.MaxValue;

        const int MinNumberOfFloors = 1;
        const int MaxNumberOfFloors = int.MaxValue;

        const int MaxNumberOfShortcuts = 2500;

        public Problem(int id, int lastExploredFloor, int numberOfShortcuts, Shortcut[] shortcuts)
        {
            Id = id;
            CurrentFloor = 1;
            Shortcuts = shortcuts;
            LastExploredFloor = lastExploredFloor;
            NumberOfShortcuts = numberOfShortcuts;

            //MaxHypotheticYears = Accumulate(0, lastExploredFloor);
        }

        /*
        public int CalculateYearsToBecomeRanker()
        {
            var dists = new int[LastExploredFloor + 1];
            var nodes = new List<Node>();
 
            for (var currentFloor = 1; currentFloor < dist.Length; currentFloor++)
            {
                dist[currentFloor] = currentFloor == 1 ? 0 : int.MaxValue;
                nodes.Add(new Node(currentFloor, ))
            }

			// First add floors and shortcuts to our list
			for (var currentFloor = 1; currentFloor <= LastExploredFloor; currentFloor++)
			{
				// Set alg. weight to 0 if starting floor. otherwise infinity!
				var weight = currentFloor == 1 ? 0 : int.MaxValue;

				// Get shortcuts and add them to floor as adjacencies
				var shortcuts = Shortcuts.Where(
					(x) => currentFloor == x.FloorB
				);


				var adjacents = new List<Edge> {
					new Edge(currentFloor + 1, currentFloor)
				};

				foreach (var shortcut in shortcuts)
				{
					adjacents.Add(new Edge(shortcut.FloorB, shortcut.Years));
				}

				if (currentFloor > 1)
				{
					//  adjacents.Add(new Edge(currentFloor - 1, 0);
				}


				floors.Add(new Floor(currentFloor, adjacencts, weight));
			}

            var Q = new List<Node>();


        }

        /*
        public int CalculateYearsToBecomeRanker()
        {
            var elapsedYears = 0;

            List<FloorNode> floors = new List<FloorNode>();
            List<Shortcut> shortct = new List<Shortcut>();

            // First we create our floors
            for (var currentFloor = 1; currentFloor <= LastExploredFloor; currentFloor++)
            {
                var floor = new FloorNode(
                    currentFloor,
                    currentFloor + 1,
                    currentFloor == 1 ? 0 : int.MaxValue
                );

                floors.Add(floor);
            }

            // Next we create our floor edges
            for (var currentFloor = 1; currentFloor <= LastExploredFloor; currentFloor++)
            {
                var edges = new List<Edge>();

                edges.Add(new Edge());
            }

            return elapsedYears;
        }

		public int CalculateMinimumYearsToRanker()
		{
			var elapsedYears = 0;

			var floors = new List<Floor>();
			var visits = new List<Floor>();

			// First add floors and shortcuts to our list
			for (var currentFloor = 1; currentFloor <= LastExploredFloor; currentFloor++)
			{
				// Set alg. weight to 0 if starting floor. otherwise infinity!
				var weight = currentFloor == 1 ? 0 : int.MaxValue;

                // Get shortcuts and add them to floor as adjacencies
                var shortcuts = Shortcuts.Where(
                    (x) => currentFloor == x.FloorB
                );


                var adjacents = new List<Edge> {
                    new Edge(currentFloor + 1, currentFloor)
                };

                foreach (var shortcut in shortcuts)
                {
                    adjacents.Add(new Edge(shortcut.FloorB, shortcut.Years));
                }

                if (currentFloor > 1)
                {
                //  adjacents.Add(new Edge(currentFloor - 1, 0);
                }


                floors.Add(new Floor(currentFloor, adjacencts, weight));
			}

			// while floors aint empty
			while (floors.Count > 0)
			{
				floors.Sort(
					(x, y) => x.Weight - y.Weight
				);

				// Pop node with min. weight
				var currentFloor = floors.First();
				floors.Remove(currentFloor);

                foreach (var adjacent in floor.Adjacents)
                {
                    
                }

			}

			return elapsedYears;

		}

        private Shortcut FindFastestShortcut(Shortcut[] shortcuts)
        {
            var fastestShortcut = (Shortcut) null;

            foreach (var shortcut in shortcuts)
            {
                if (fastestShortcut == null)
                {
                    fastestShortcut = shortcut;
                }

                else
                {
                    if (shortcut.Years < fastestShortcut.Years)
                    {
                        fastestShortcut = shortcut;
                    }
                }
            }

            return fastestShortcut;
        }
*/
        /*
        public int CalculateYearsToBecomeRanker()
        {
            var shortcuts = Shortcuts.ToList();

            for (var floor = 1; floor <= LastExploredFloor; floor++)
            {
                var shortcut = new Shortcut(floor, floor + 1, floor);

                if (!Shortcuts.Contains(shortcut))
                {
                    shortcuts.Add(shortcut);

                    Console.WriteLine($"Adding {shortcut}");
                }
            }

            Shortcuts = shortcuts.ToArray();

            // Sort shortcuts by floorA to avoid
            // unnecesary loops through the array
            Array.Sort(Shortcuts, (x, y) =>
            {
                if (x.Years > y.Years)
                    return 1;
                if (x.Years < y.Years)
                    return -1;
                return 0;
            });
			
            var elapsedYears = 0;

            for (var currentFloor = 1; currentFloor < LastExploredFloor;)
            {
                var shortcutYears = -1;
                var shortcutFloor = -1;

                // Loop through all shortcuts
                foreach (var shortcut in Shortcuts)
                {
                    // Stop looping if shortcuts are big
                    if (currentFloor < shortcut.FloorA)
                    {
                        // break;
                    }

                    if (currentFloor >= shortcut.FloorA)
                    {
                        Console.WriteLine($"Testing {shortcut} on floor #{currentFloor}");

                        var yearsWoShortcut = Accumulate(
                            currentFloor, shortcut.FloorB
                        );

                        // Is it worth to take the shortcut?
                        if (shortcut.Years < yearsWoShortcut)
                        {
                            if (shortcutYears == -1
                            &&  shortcutFloor == -1)
                            {
                                shortcutYears = shortcut.Years;
                                shortcutFloor = shortcut.FloorB;

                                Console.WriteLine($"Taking first {shortcut} on floor #{currentFloor}");
                            }

                            else
                            {
                                // Ya existe un shortcut
                                // Comparar si este es mejor

                                if (shortcut.Years <= shortcutYears
                                && shortcut.FloorB >= shortcutFloor)
                                {
                                    shortcutFloor = shortcut.FloorB;
                                    shortcutYears = shortcut.Years;

                                    Console.WriteLine($"Taking other {shortcut} on floor #{currentFloor}");
                                }
                            }
                        }
                    }
                }

                if (shortcutYears != -1
                &&  shortcutFloor != -1)
                {
                    currentFloor = shortcutFloor;
                    elapsedYears += shortcutYears;
                    
                    Console.WriteLine($"++ Shortcut choosen on Floor #{currentFloor} to Floor #{shortcutFloor}");

                    continue;
                }

                elapsedYears += currentFloor;//++;

                Console.WriteLine($"No shortcut choosen on floor #{currentFloor++}, elapsed years #{elapsedYears}");
            }

            // Console.WriteLine($"Elapsed years from Floor #{startingFloor}: {elapsedYears}");


            return elapsedYears;
        }*/
        /*
        private int Accumulate(int min, int max)
        {
            var accumulator = 0;

            for (int i = min; i < max; i++)
            {
                accumulator += i;
            }

            return accumulator;
        }
    }

    class Shortcut
	{
		public int Years
        {
            get;
        }

        public int FloorA
        {
            get;
        }

        public int FloorB
        {
            get;
        }

        public Shortcut(int floorA, int floorB, int years)
        {
            this.Years  = years;
            this.FloorA = floorA;
            this.FloorB = floorB;
        }

        public override bool Equals(object obj)
        {
            if (obj is Shortcut)
            {
                var s = (Shortcut)obj;

                return s.FloorA == FloorA && s.FloorB == FloorB;
            }

            return false;
        }

        public override string ToString()
        {
            return string.Format("Shortcut[FloorA={1}, FloorB={2}, Years={0}]", Years, FloorA, FloorB);
        }
    }*/
    }
}