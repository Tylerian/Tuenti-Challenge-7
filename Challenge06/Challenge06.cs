using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Challenge06
{
    public class Challenge06
    {
		const string TestInputFileName   = "./testinput.txt";
        const string SubmitInputFileName = "./submitinput.txt";

		static void Main(string[] args)
		{
			try
			{
				var towers = ParseFileInput(SubmitInputFileName);
				foreach (var tower in towers)
				{
                    try
                    {
                        Console.WriteLine($"Case #{tower.Id}: {tower.CalculateYearsToBecomeRankerFinal()}");
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine($"Case #{tower.Id}: {ex.Message}");
                    }
				}
			}

			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
			}
		}

		static List<Tower> ParseFileInput(string path)
		{
			var output = new List<Tower>();
			var input  = File.ReadAllLines(path);

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
					var years  = int.Parse(shortcut[2]);

					shrcts[idx] = new Shortcut(floorA, floorB, years);
				}

                output.Add(new Tower(caseId, floors, sctnum, shrcts));
			}

			return output;
		}
    }

    public class Edge
    {
        public Floor Target
        {
            get;
        }

        public int Weight // cost
        {
            get;
        }

        public Edge(Floor target, int weight)
        {
            Target = target;
            Weight = weight;
        }

        public override string ToString()
        {
            return string.Format("Edge[Target={0}, Weight={1}]", Target, Weight);
        }
    }

    public class Floor
    {
        public int Id
        {
            get;
        }

        public int Weight
        {
            get;
            set;
        }

        public bool Visited
        {
            get;
            set;
        }

        public Edge[] Paths
        {
            get;
            set;
        }

        public Floor(int id, Edge[] paths, int weight)
        {
            Id = id;
            Paths = paths;
            Weight = weight;
        }

        public override string ToString()
        {
            return string.Format("Floor[Id={0}, Weight={1}]", Id, Weight);
        }
    }

    public class Tower
    {
        public int Id
        {
            get;
        }

        private int LastExploredFloor;
        private int NumberOfShortcuts;

		private Shortcut[]  Shortcuts;

		const int MinNumberOfYears = 0;
		const int MaxNumberOfYears = int.MaxValue;

		const int MinNumberOfFloors = 1;
		const int MaxNumberOfFloors = int.MaxValue;

        const int MinNumberOfShortcuts = 0;
		const int MaxNumberOfShortcuts = 2500;

		public Tower(int id, int lastExploredFloor, int numberOfShortcuts, Shortcut[] shortcuts)
		{
			Id = id;
			Shortcuts = shortcuts;
			LastExploredFloor = lastExploredFloor;
			NumberOfShortcuts = numberOfShortcuts;
		}

		public int CalculateSequence(int from, int to)
		{
			// Sum = total_numbers * ( first / 2 + last / 2 ) 
			return (to - from) * ((from / 2) + (to / 2));
		}

        public int CalculateYearsToBecomeRankerFinal()
        {
            Array.Sort(Shortcuts, (x, y) => x.FloorA - y.FloorA);


            Dictionary<int, List<Shortcut>> shortcuts = new Dictionary<int, List<Shortcut>>();

            foreach (var shortcut in Shortcuts)
            {
                if (shortcuts.ContainsKey(shortcut.FloorA))
                {
                    shortcuts[shortcut.FloorA].Add(shortcut);
                }

                else
                {
                    shortcuts.Add(shortcut.FloorA, new List<Shortcut> { shortcut });
                }
            }

            shortcuts.OrderBy((x) => x.Key);

			/*
			var elapsedYears = 0;
			var currentFloor = 1;
            var nShortcutYears = 0;

            for (var nop = 0; nop < Shortcuts.Length; nop++)
            {
                nShortcutYears += CalculateSequence(currentFloor, Shorcuts[nop].FloorB);

				foreach (var shortcut in Shortcuts)
				{
					// can take shortcut
					if (shortcut.FloorA <= currentFloor)
					{
						var shortcutYears = CalculateSequence(currentFloor, shortcut.FloorB);

						if (shortcut.Years < shortcutYears)
						{
							shortcutYears = shortcut.Years;
						}
					}
				}
            }
            */

			var elapsedYears = 0;

            for (var currentFloor = 1; currentFloor < LastExploredFloor;)
            {
                shortcuts.
            }

            return elapsedYears;
        }

        public int calculateYearsToBecomeRankerAstar()
        {
			// Check floors limits
			if (LastExploredFloor < MinNumberOfFloors
			||  LastExploredFloor > MaxNumberOfFloors)
			{
				throw new Exception("Min/Max NumberOfFloors limit triggered.");
			}

			// check shortcut limits
			if (NumberOfShortcuts < MinNumberOfShortcuts
			||  NumberOfShortcuts > MaxNumberOfShortcuts)
			{
				throw new Exception("Min/Max NumberOfShortcuts limit triggered.");
			}

            return 0;
        }

        public int CalculateYearsToBecomeRankerDijkstra()
        {
            // Check floors limits
            if (LastExploredFloor < MinNumberOfFloors 
            ||  LastExploredFloor > MaxNumberOfFloors)
            {
                throw new Exception("Min/Max NumberOfFloors limit triggered.");
            }

            // check shortcut limits
            if (NumberOfShortcuts < MinNumberOfShortcuts
            ||  NumberOfShortcuts > MaxNumberOfShortcuts)
            {
                throw new Exception("Min/Max NumberOfShortcuts limit triggered.");
            }

			var visits = new List<Floor>();
			var floors = new List<Floor>();


            foreach (var shortcut in Shortcuts.Distinct<Shortcut>((x) => x.FloorA))
            {
                var weight = shortcut.FloorA;
            }

            // First add floors to our list
            for (var currentFloor = 1; currentFloor <= LastExploredFloor; currentFloor++)
            {
                // Set alg. weight to 0 if starting floor. otherwise infinity!
                var weight = currentFloor == 1 ? 0 : int.MaxValue;

                floors.Add(new Floor(currentFloor, null, weight));
			}

            foreach (var floor in floors)
            {
                // Get shortcuts and add them to floor as adjacencies
                var adjacents = new List<Edge>();

                // first floor?
                if (floor.Id == 1)
                {
                    adjacents.Add(new Edge(null /*floors.Find((x) => x.Id == floor.Id + 1)*/, floor.Id));
                }

                else if (floor.Id == LastExploredFloor)
                {
                    adjacents.Add(new Edge(null /*floors.Find((x) => x.Id == floor.Id - 1)*/, 0)); // cost going backwards is negligible!
                }

                else
                {
                    adjacents.Add(new Edge(null /*floors.Find((x) => x.Id == floor.Id + 1)*/, floor.Id));
                  //adjacents.Add(new Edge(floors.Find((x) => x.Id == floor.Id - 1), 0)); // cost going backwards is negligible!
                }

                var shortcuts = new List<Shortcut>(); //Shortcuts.Where((x) => x.FloorA <= floor.Id);

                foreach (var shortcut in shortcuts)
                {
                    // check shortcut year limits
                    if (shortcut.Years < MinNumberOfYears 
                    ||  shortcut.Years > MaxNumberOfYears)
                    {
						throw new Exception("Min/Max NumberOfYears for shortcuts limit triggered.");

                        // return 0 or tuenti's console starts complaining...
						//return 0;
                    }
                    //Console.WriteLine($"Adding possible {shortcut} to Floor #{floor.Id}");

                    adjacents.Add(new Edge(null /*floors.Find((x) => x.Id == shortcut.FloorB)*/, shortcut.Years));
                }

                // Add possible paths to floor
                floor.Paths = adjacents.ToArray();

                //Console.WriteLine();
                //Console.WriteLine($"Floor edges for #{floor.Id}:");
            }
            //Console.WriteLine($"tardo 1");

            var floorz = new MinHeap<Floor>(floors, Comparer<Floor>.Create((x, y) =>
            {
                if (x.Weight > y.Weight)
                    return 1;
                if (x.Weight < y.Weight)
                    return -1;

                return 0;
            }));

            // while floors aint empty
            while (floorz.Count > 0)
            {
                //floors.Sort(
                //    (x, y) => x.Weight - y.Weight
                //);

                // Pop node with min. weight not already visited
                //var currentFloor = floors.Find((x) => !x.Visited);
                //floors.Remove(currentFloor);

                var currentFloor = floorz.ExtractMin();

                //Console.WriteLine($"Visited Floor #{currentFloor.Id}");

                // Mark this floor as visited
               // currentFloor.Visited = true;
                //visits.Add(currentFloor);

                foreach (var adjacent in currentFloor.Paths)
                {
                    //Console.WriteLine($"Testing possible shortcut {adjacent}");

                    var weight = currentFloor.Weight + adjacent.Weight;

                    if (weight < adjacent.Target?.Weight)
                    {
                        adjacent.Target.Weight = weight;
                        //adjacent.Previous = currentFloor;
                    }
                }
            }

            // return elapsed years
            return visits.Find((x) => x.Id == LastExploredFloor)?.Weight ?? 0;
        }
    }


	public class Shortcut
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
			this.Years = years;
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
	}
}
