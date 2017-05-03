using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Tylerian.Challenge02
{
    class Program
    {
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
                        Console.WriteLine($"Case #{caze.Id}: {caze.PukeMatchResult()}");
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

            Console.ReadKey(true);
        }

        static List<Case> ParseInputFile(string path)
        {
            var output = new List<Case>();
            var input = File.ReadAllLines(path);

            // Is the input in the correct format?
            // C (number of cases)
            // R (number of rolls)
            // K (number of knocked pins)
            if (input.Length % 2 != 1)
            {
                throw new Exception("Error while parsing input file.");
            }
            
            // Start at line 1, because line 0
            // is the total number of cases
            for (int caseId = 0, line = 1; line < input.Length;)
            {
                // Increase case ID
                caseId++;

                var rolls  = int.Parse(input[line++]); // I don't need it btw.
                var knocks = input[line++].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                output.Add(new Case(caseId, rolls, knocks));
            }

            return output;
        }
    }

    class Case
    {
        public int   Id;
        public int   Rolls;
        public int[] Knocks;
        public int[] Scores;

        public bool[] Spares;
        public bool[] Strikes;

        const byte RollsPerFrame  = 2;
        const byte FramesPerMatch = 10;

        const byte PinsNeededToStrike = 10;

        public Case(int Id, int rolls, string[] knocks)
        {
            this.Id     = Id;
            this.Rolls  = rolls;
            this.Knocks = new int[knocks.Length];
            this.Scores = new int[FramesPerMatch];

            this.Spares  = new bool[FramesPerMatch];
            this.Strikes = new bool[FramesPerMatch];

            for (var idx = 0; idx < knocks.Length; idx++)
            {
                Knocks[idx] = int.Parse(knocks[idx]);
            }
        }

        public string PukeMatchResult()
        {
            CalculateScores();

            return string.Join(" ", Aggregate(Scores));
        }

        private void CalculateScores()
        {
            var turn = 0;

            // First spin for normal scoring
            for (var frame = 0; frame < FramesPerMatch; frame++)
            {
                for (var roll = 0; roll < RollsPerFrame; roll++)
                {
                    var knocks = Knocks[turn];

                    // Add knocks to counter
                    Scores[frame] += knocks;

                    // Is it a strike?
                    if (IsFirstFrameRoll(roll)
                    &&  knocks == PinsNeededToStrike) {

                        // Yes it's a strike!!
                        Strikes[frame] = true;

                        // Add next two roll scores
                        Scores[frame] += Knocks[turn + 1] + Knocks[turn + 2];

                        // Skip second roll.
                        turn++; break;
                    }

					// Is it a spare?
					else if (!IsFirstFrameRoll(roll)
                    &&  knocks + Knocks[turn - 1] == PinsNeededToStrike) {

                        // Yes it's a spare!!
                        Spares[frame] = true;

                        // Add next roll to score
                        Scores[frame] += Knocks[turn + 1];
                    }

                    // next roll
                    turn++;
                }
            }

            /*
            // Second spin for special points
            for (var frame = 0; frame < FramesPerMatch; frame++)
            {
                var Spared  = Spares[frame];
                var Striked = Strikes[frame];

                if (Striked)
                {
                    // Add scores from next two rolls
                    Scores[frame] = Scores[frame] + Knocks[frame + 1] + Knocks[frame + 2];
                }

                else if (Spared)
                {
                    // Add scores from next roll
                    Scores[frame] = Scores[frame] + Knocks[frame + 1];
                }
            }
            */
        }

        private int[] Aggregate(int[] values)
        {
            var result = new int[values.Length];

            for (var i = 0; i < result.Length; i++)
            {
                if (i == 0)
                    result[i] = values[i];
                else
                    result[i] = values[i] + result[i - 1];
            }

            return result;
        }

        private bool IsFirstFrameRoll(int roll)
        {
            return roll == 0;
        }
    }
}