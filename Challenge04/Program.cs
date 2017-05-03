using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Tylerian.Challenge04
{
    class Case
    {
        public int Id;
        public long[] Sides;
        
        const long MaxSideSize = 4294967296;
            
		public Case(int id, string[] sides)
        {
            this.Id = id;

            if (sides != null)
            {
                this.Sides = new long[sides.Length];

                for (var idx = 0; idx < sides.Length; idx++)
                {
                    Sides[idx] = long.Parse(sides[idx]);
                }
            }
        }

        public long CalculateSmallestTrianglePerimeter()
        {
            // We can find the smallest triangle possible
            // given a number of sides by using euclidean geometry
            // Euclides said that a triangle can be formed only if
            // the sum of two of its sides is greather than the third side.
            // that's it -> a + b > c, a + c > b, + b + c > a
            

            // Check we didn't exceeded side limits
            if (Sides == null)
            {
                throw new Exception("Error -> Max sides limit exceeded.");
            }

            long smallest = long.MaxValue;
            
            // Sort our sides from smallest to largest
            // That way, arr[i] + arr[i + 1] < arr[i + 2] will always be true
            Array.Sort(Sides, (x, y) => { if (x > y) return 1; if (x < y) return -1; return 0; });
            
            // Now iterate through all our sides
            for (int i = 0, j = 1; j < Sides.Length; i++, j++)
            {
                var a = Sides[i];

                // Shortcut third for loop with k - 1 hack
                for (int k = j + 1; k < Sides.Length; k++)
                {
                    var b = Sides[k - 1];
                    var c = Sides[k];

                    // Apply euclides check
                    if (a + b > c && a + c > b && b + c > a)
                    {
                        // Check side size limits
                        if (a > MaxSideSize || b > MaxSideSize || c > MaxSideSize)
                        {
                            throw new Exception("Error -> Side size limit exceeded.");
                        }

                        // We've got a valid triang!
                        var perimeter = a + b + c;

                        if (perimeter < smallest)
                        {
                            smallest = perimeter;
                        }
                    }
                }
            }
            
            /*
            // Holly molly!! that's a very slow algo!
            for (long a = 0, i = 0; i < Sides.Length; i++)
            {
                a = Sides[i];

                for (long b = 0, j = 0; j < Sides.Length; j++)
                {
                    if (i != j)
                    {
                        b = Sides[j];

                        for (long c = 0, k = 0; k < Sides.Length; k++)
                        {
                            if (i != k && j != k)
                            {
                                c = Sides[k];

                                // Apply euclides check
                                if (a + b > c && a + c > b && b + c > a)
                                {
                                    // Check side size limits
                                    if (a > MaxSideSize || b > MaxSideSize || c > MaxSideSize)
                                    {
                                        throw new Exception("Error -> Side size limit exceeded.");
                                    }

                                    // We've got a valid triang!
                                    var perimeter = a + b + c;

                                    if (perimeter < smallestPerimeter)
                                    {
                                        smallestPerimeter = perimeter;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            */

            // Any valid triangle found?
            if (smallest < 0 
            ||  smallest == long.MaxValue)
            {
                throw new Exception("IMPOSSIBLE");
            }

            else
            {
                // return the smallest one!!
                return smallest;
            }
        }
    }

    class Program
    {
        const int SidesOfTriangle = 3;
        const int MaxNumberOfSides = 100000;

        const string TestInputFileName  = "./testinput.txt";
        const string TestOutputFileName = "./testoutput.txt";

        const string SubmitInputFileName  = "./submitinput.txt";
        const string SubmitOutputFileName = "./submitoutput.txt";

        static void Main(string[] args)
        {
			try
			{
				var cases  = ParseInputFile(SubmitInputFileName);
                var output = new StringBuilder();

				foreach (var kase in cases)
				{
					try
					{
                        var line = $"Case #{kase.Id}: {kase.CalculateSmallestTrianglePerimeter()}";

                        Console.WriteLine(line);
                        output.AppendLine(line);
					}

					catch (Exception ex)
					{
						var line = $"Case #{kase.Id}: {ex.Message}";

                        Console.WriteLine(line);
                        output.AppendLine(line);
					}
				}

                File.WriteAllText(SubmitOutputFileName, output.ToString());
			}

			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
        }

        static List<Case> ParseInputFile(string path)
        {
            var output = new List<Case>();
            var input = File.ReadAllLines(path);

            // Is the input in the correct format?
            // C (number of cases)
            // S (number of sides)
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

                var sides = input[line++].Split(new[] { ' ' }, 2);
                var sideCount = int.Parse(sides[0]);

                // Check we don't exceed the side limits
                if (sideCount < SidesOfTriangle 
                ||  sideCount > MaxNumberOfSides)
                {
                    sides = null;
                }

                else
                {
                    sides = sides[1].Split(
                        new[] { ' ' },
                        StringSplitOptions.RemoveEmptyEntries
                    );
                }

                output.Add(new Case(caseId, sides));
            }
            
            return output;
        }
    }
}