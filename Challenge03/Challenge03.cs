using System;
using System.IO;
using System.Collections.Generic;

namespace Challenge3
{
    class Program
    {
        const short MaxCases = 10000;

		const string TestInputFileName = "./testinput.txt";
		const string SubmitInputFileName = "./submitinput.txt";

		static void Main(string[] args)
		{
			try
			{
				var cases = ParseInputFile(SubmitInputFileName);

				foreach (var caze in cases)
				{
					try
					{
                        Console.WriteLine($"Case #{caze.Id}: {caze.CalculateMinimumCardsNeeded()}");
					}

					catch (Exception ex)
					{
                        Console.WriteLine($"Case #{caze.Id}: {ex.Message}");
					}
				}
			}

			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			Console.ReadKey(true);
		}

		static List<Case> ParseInputFile(string path)
		{
			var output = new List<Case>();
			var input = File.ReadAllLines(path);

			var totalCases = int.Parse(input[0]);

			if (totalCases > MaxCases)
			{
				throw new Exception($"You can only have a max of {MaxCases} cases.");
			}

			// Start at line 1, because line 0
			// is the total number of cases
			for (int caseId = 0, line = 1; line < input.Length;)
			{
				// Increase case ID
				caseId++;

				var points = int.Parse(input[line++]); // I don't need it btw.

				output.Add(new Case(caseId, points));
			}

			return output;
		}
    }

    class Case
    {
        public int Id;
        public int Points;

        const int MaxPoints = 1000000000;


        public Case(int id, int points)
        {
            this.Id = id;
            this.Points = points;
        }

        public int CalculateMinimumCardsNeeded()
		{
            if (Points > MaxPoints)
            {
                throw new Exception("Maximum points limit exceeded.");
            }

            return (int)Math.Ceiling(Math.Log(Points) / Math.Log(2));
        }
    }
}
