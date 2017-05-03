using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Security.Cryptography;
using Force.Crc32;
using System.Numerics;

namespace Challenge09
{
	class Program
	{
		const string TestInputFileName   = "./testinput.txt";
		const string SubmitInputFileName = "./submitinput.txt";

		static void Main(string[] args)
		{
			try
			{
                Git.Initialize();

				var accounts = ParseFileInput(SubmitInputFileName);

				foreach (var account in accounts)
				{
					try
					{
						Console.WriteLine($"Case #{account.Id}: {account.TryRestorePassword()}");
					}

					catch (Exception ex)
					{
						Console.WriteLine($"Case #{account.Id}: {ex.Message}");
					}
				}
			}

			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
			}

            Console.ReadKey();
		}

		static List<Account> ParseFileInput(string path)
		{
			var output = new List<Account>();
			var input = File.ReadAllLines(path);

			// Is the input in the correct format?

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

                var userid  = bits[0];
                var mdfcnt  = int.Parse(bits[1]);
                var mdfctns = new Modification[mdfcnt];

                for (var idx = 0; idx < mdfcnt; idx++)
                {
                    var bitz = input[line++].Split(
                        new char[] { ' ' },
                        StringSplitOptions.RemoveEmptyEntries
                    );

                    var date = bitz[0];
                    var iter = int.Parse(bitz[1]);

                    mdfctns[idx] = new Modification(date, iter);
                }

                output.Add(new Account(caseId, userid, mdfctns));
			}

			return output;
		}
	}

    class Git
	{
		public static Dictionary<string, Commit> Commits
		{
			get;
		}

		static readonly Regex FsckDanglingMatcher;
		static readonly Regex PrettyCommitMatcher;

        const string PrettyCommitPattern = "commit\\s*(?<hash>[a-f0-9]{40})\\nAuthor:.*\\nDate:\\s*(?<date>[0-9]{4}-[0-9]{2}-[0-9]{2})";
        const string FsckDanglingPattern = "unreachable commit (?<hash>[a-f0-9]{40})";

		const string ListAllCommitsCommand = "rev-list --pretty --all --date=short";
        const string FsckDanglingCommand   = "fsck --unreachable";
        const string GetCommitInfoCommand  = "log -1 --pretty --date=short {0}";
        const string LocalRepositoryFolder = "C:\\Users\\Domobility\\Desktop\\repo";

        static Git()
        {
            // Tired of coding, adding few 
            // missing dates by hand... Sorry!
            Commits = new Dictionary<string, Commit>
            {
              //  {"2012-07-07", new Commit ("560220212aa07e8d3f5916c46f108c6ec59aa9d1", "2012-07-07") { Secret1 = 4631539, Secret2 = 1940871 }},
              //  {"2014-05-02", new Commit ("560220212aa07e8d3f5916c46f108c6ec59aa9d1", "2014-05-02") { Secret1 = 3666129, Secret2 = 1914605 }},
              //  {"2015-02-10", new Commit ("560220212aa07e8d3f5916c46f108c6ec59aa9d1", "2015-02-10") { Secret1 = 8590856, Secret2 = 2112619 }}
            };

            FsckDanglingMatcher = new Regex(
                FsckDanglingPattern,
                RegexOptions.Compiled);

            PrettyCommitMatcher = new Regex(
                PrettyCommitPattern,
                RegexOptions.Compiled
            );
        }

        public static void Initialize()
        {
            Console.WriteLine("Parsing Git repository...");

            LoadAllVisibleCommits();
            LoadAllDanglingCommits();

            Console.WriteLine("Git repository has been parsed successfully.");
        }

        private static void LoadAllVisibleCommits()
        {
            var output = RunGitCommand(
                ListAllCommitsCommand
            );

            var matches = PrettyCommitMatcher.Matches(output);
            
            foreach (Match match in matches)
            {
                var hash = match.Groups["hash"].Value;
				var date = match.Groups["date"].Value;

                if (!Commits.ContainsKey(date))
                    Commits.Add(date, new Commit(hash, date));
                else
                    Console.WriteLine($"Duplicated key 1: {date}");

                Console.Write("+");
            }

            Console.WriteLine();
        }

        private static void LoadAllDanglingCommits()
        {
            var output = RunGitCommand(
                FsckDanglingCommand
            );

            var matches = FsckDanglingMatcher.Matches(output);

            foreach (Match match in matches)
            {
				var hash    = match.Groups["hash"].Value;
				var command = string.Format(GetCommitInfoCommand, hash);

                var cmdinfo = RunGitCommand(command);
                var cmtinfo = PrettyCommitMatcher.Match(cmdinfo);
                var date    = cmtinfo.Result("${date}");
                
                if (!Commits.ContainsKey(date))
                    Commits.Add(date, new Commit(hash, date));
                else
                    Console.WriteLine($"Duplicated key 2: {date}");

                Console.Write("-");
            }

            Console.WriteLine();
        }

        public static string RunGitCommand(string command)
        {
            return Shell.Execute(
                new[]{"git", command}, 
                LocalRepositoryFolder
            );
        }

        public class Commit
		{
			public string Id
			{
				get;
			}

            public string Date
            {
                get;
            }

            public int Secret1
            {
                get;
                set;
            }

            public int Secret2
            {
                get;
                set;
			}

			static readonly Regex SecretMatcher;

            const string SecretsPattern  = "\\$secret1 = (?<secret1>[0-9]+);\\n\\$secret2 = (?<secret2>[0-9]+);";
			const string ShowFileCommand = "show {0}:script.php";

            static Commit()
            {
                SecretMatcher = new Regex(
                    SecretsPattern,
                    RegexOptions.Compiled
                );
            }

            public Commit(string id, string date)
            {
                Id   = id;
                Date = date;

                RetrieveCommitSecrets();
            }

            private void RetrieveCommitSecrets()
            {
                var output = Git.RunGitCommand(
                    string.Format(ShowFileCommand, Id)
                );

                var match = SecretMatcher.Match(output);

				Secret1 = int.Parse(match.Groups["secret1"].Value);
				Secret2 = int.Parse(match.Groups["secret2"].Value);
            }
        }
    }

    class Shell
    {
        public static string Execute(string[] args, string workingDir = "")
        {
			var info = new ProcessStartInfo()
			{
				WorkingDirectory = workingDir,
				FileName = args[0],
				Arguments = args[1],
				CreateNoWindow = true,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				RedirectStandardInput = true,
				//UserName = Environment.UserName
			};

			using (Process process = Process.Start(info))
			{
				using (var output = process.StandardOutput)
				{
					return output.ReadToEnd();
				}
			}
        }
    }

    class Account
    {
        public int Id
        {
            get;
        }

        public string UserId
        {
            get;
        }

        public Modification[] Modifications
        {
            get;
        }

        const string PasswordScriptFolder = "/Users/Tylerian/Desktop/Tuenti/Challenge09/";

        public Account(int id, string userId, Modification[] modifications)
        {
            Id = id;
            UserId = userId;
            Modifications = modifications;
        }

        public string TryRestorePassword()
        {
            var hash = "";
            var pass = "";

            foreach (var modification in Modifications)
            {
                var output = "";
                var commit = Git.Commits[modification.Date];/*(Git.Commit) null;

                if (!Git.Commits.ContainsKey(modification.Date))
                {
                    Console.WriteLine($"Missing commit: {modification.Date}");
                }

                else
                {
                    commit = ;
                }*/
                
                for (var nop = 0; nop < modification.Iterations; nop++)
                {
                    output = PasswordScript.Generate(UserId, hash, commit.Secret1, commit.Secret2);

                    /*if (string.IsNullOrEmpty(hash))
                    {
                        output = Shell.Execute(
                            new[] { "php", $"script.php {commit.Secret1} {commit.Secret2} {UserId}" },
                            PasswordScriptFolder
                        );
                    }

                    else
                    {
                        output = Shell.Execute(
                            new[] { "php", $"script.php {commit.Secret1} {commit.Secret2} {UserId} {hash}" },
                            PasswordScriptFolder
                        );
                    }*/

                    var data = output.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    pass = data[0];

                    if (data.Length == 2)
                    {
                        hash = data[1];
                    }
                }
            }

            return pass;
        }
    }

    class Modification
	{
		public string Date
		{
			get;
		}

        public int Iterations
        {
            get;
        }

        public Modification(string date, int iterations)
        {
            Date = date;
            Iterations = iterations;
        }
    }

    class PasswordScript
	{

		/*
		 *  function modular_pow(base, exponent, modulus)
			if modulus = 1 then return 0
			Assert :: (modulus - 1) * (modulus - 1) does not overflow base
			result := 1
			base := base mod modulus
			while exponent > 0
				if (exponent mod 2 == 1):
				   result := (result * base) mod modulus
				exponent := exponent >> 1
				base := (base * base) mod modulus
			return result
		 */
		static long ModPow(long baze, long exp, long mod)
        {
            long number = 1;

            while (exp > 0)
            {
                if ((exp & 1) == 1)
                    number = number * baze % mod;
                exp >>= 1;

                baze = baze * baze % mod;
            }

            return number;
        }

        public static string Generate(string uuid, string hash, int secret1, int secret2)
        {
			var secret3 = (long)Crc32Algorithm.Compute(Encoding.UTF8.GetBytes(uuid));
			
			if (!string.IsNullOrEmpty(hash))
            {
                secret3 = (long)Crc32Algorithm.Compute(Encoding.UTF8.GetBytes(hash));
            }

            var counter = secret3;
                counter = counter * ModPow(secret1, 10000000, secret2) % secret2;

            /*for (var i = 0; i < 10000000; i++)
            {
                counter = (counter * secret1) % secret2;
            }*/

            var password = new StringBuilder(10);

            for (var i = 0; i < 10; i++)
            {
                counter = (counter * secret1) % secret2;
                password.Append((char) (counter % 94 + 33));
            }

            using (MD5 md5 = MD5.Create())
            {
                hash = BitConverter.ToString(
                    md5.ComputeHash(Encoding.UTF8.GetBytes(password.ToString())), 0
                ).Replace("-", "").ToLower();
            }

            return $"{password.ToString()} {hash}";
        }
    }
}
