using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace Tylerian.Challenge01
{
    class Program
    {
        const  int MaxCases = 100;

        const string TestInputFileName   = "./testinput.txt";
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
                        Console.WriteLine($"Case #{caze.CaseId}: {caze.CalculatePizzasNeeded()}");
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static List<Case> ParseInputFile(string path)
        {
            var output = new List<Case>();
            var input  = File.ReadAllLines(path);
            
            // Is the input in the correct format?
            // C (number of cases)
            // G (number of guests)
            // S (number of slices)
            if (input.Length % 2 != 1)
            {
                throw new Exception("Error while parsing input file.");
            }

            var totalCases = int.Parse(input[0]);

            if (totalCases > MaxCases)
            {
                throw new Exception($"You can only have a max of {MaxCases} cases.");
            }

            // Start at line 1, 
            // because line 0 is the total of cases
            for (int caseId = 0, line = 1; line < input.Length;)
            {
                // Increase case ID
                caseId++;
                
                var guests = int.Parse(input[line++]);
                var slices = input[line++].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                
                output.Add(new Case(caseId, guests, slices));
            }

            return output;
        }
    }

    class Case
    {
        public int CaseId;
        public int Guests;
        public int Slices;

        const short MaxGuestsPerCase  = 1000;
        const byte  MaxSlicesPerPizza = 8;
        const short MaxSlicesPerGuest = 100;

        public Case(int caseId, int guests, string[] slices)
        {
            CaseId = caseId;
            Guests = guests;

            foreach (var slice in slices)
            {
                var i = int.Parse(slice);

                if (i > MaxSlicesPerGuest)
                {
                    Slices = -1;
                    break;
                }

                Slices += i;
            }
        }

        public int CalculatePizzasNeeded()
        {
            // Slices is always -1 only if 
            // maxSlicesPerGuest limit is exceeded
            if (Slices == -1)
            {
                throw new Exception($"Case #{CaseId}: SlicePerGuest's limit exceeded.");
            }

            else if (Guests > MaxGuestsPerCase)
            {
                throw new Exception($"Case #{CaseId}: MaxGuestsPerCase's limit exceeded.");
            }

            else
            {
                return (int)Math.Ceiling(Slices / (double)MaxSlicesPerPizza);
            }
        }
    }
}