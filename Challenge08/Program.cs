using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Tylerian.Challenge08
{
    class Program
    {
		const string TestInputFileName   = "./testinput.txt";
		const string SubmitInputFileName = "./submitinput.txt";

		static void Main(string[] args)
		{
			try
			{
				var problems = ParseFileInput(SubmitInputFileName);
				
                foreach (var problem in problems)
				{
					try
					{
						Console.WriteLine($"Case #{problem.Id}: {problem.TryNormalizeInputToHexNumber()}");
					}

					catch (Exception ex)
					{
                        // Print N/A instead of ex.Message 
                        // to match challenge's requirements!
                        Console.WriteLine($"Case #{problem.Id}: {ex.Message}");
					}
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
			var input  = File.ReadAllLines(path, Encoding.Unicode);

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

                var unicode = input[line++];

				output.Add(new Problem(caseId, unicode));
			}

			return output;
		}
    }

    public class Problem
    {
        private static Regex Validator;

        private const string ValidInputPattern = "^[\\p{Z}]*(?<numbers>[\\p{N}]+)[\\p{Z}]*$";

        public int Id
        {
            get;
        }

        public string Unicode
        {
            get;
        }

        static Problem()
        {
            Validator = new Regex(
                ValidInputPattern,
                RegexOptions.Compiled
            );
        }

        public Problem(int id, string unicode)
        {
            this.Id = id;
            this.Unicode = unicode.Trim();
        }

        public string TryNormalizeInputToHexNumber()
        {
            // Is our input in a valid form?
            if (!Validator.IsMatch(Unicode))
            {
                throw new Exception("Invalid input format.");
            }

            // get numeric block from my regex pattern and normalize it
            // decomposing each unicode character to it's basic values
            var match = Validator.Match(Unicode).Result("${numbers}");
                match = match.Normalize(NormalizationForm.FormD);

            var value = new StringBuilder();

            foreach (var character in match)
            {
                value.Append(CharUnicodeInfo.GetDigitValue(character));
            }

            // return result in hex format.
            return long.Parse(value.ToString()).ToString("x");
        }
    }
}
