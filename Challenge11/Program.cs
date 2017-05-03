using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Tylerian.Challenge11
{
    class Program
    {
        const string TestInputFileName = "./testinput.txt";

        static void Main(string[] args)
        {
            try
            {
                var tuentiverses = ParseInputFile(TestInputFileName);

                foreach (var tuentiverse in tuentiverses)
                {
                    try
                    {
                        Console.WriteLine($"Case #{tuentiverse.Id}: {tuentiverse.ResolveTuentiverseSpaceTime()}");
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine($"Case #{tuentiverse.Id}: {ex.Message}");
                        Console.WriteLine(ex.StackTrace);
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            Console.ReadKey(true);
        }

        static List<Tuentiverse> ParseInputFile(string path)
        {
            var input  = File.ReadAllLines(path);
            var output = new List<Tuentiverse>();

            if (input.Length <= 1)
            {
                throw new Exception("Error while parsing input file.");
            }
            
            for (int csid = 1, line = 1; line < input.Length; csid++)
            {
                var colnum = int.Parse(input[line++]);
                var colors = new Color[colnum];

                // Parse colors
                for (var cid = 0; cid < colnum; cid++)
                {
                    var bits = input[line++].Split(
                        new[] { ' ' },
                        StringSplitOptions.RemoveEmptyEntries
                    );

                    var name = bits[0];
                    var cnum = int.Parse(bits[1]);
                    var comp = new Color[cnum];

                    // Parse composites
                    for (var pid = 0; pid < cnum; pid++)
                    {
                        var cname = bits[2 + pid];
                        comp[pid] = colors.Where((x) => x.Name == cname).First();
                    }

                    colors[cid] = new Color((int) Math.Pow(2, cid), name, comp);
                }

                var galnum   = int.Parse(input[line++]);
                var galaxies = new Galaxy[galnum];

                // Parse galaxies
                for (var gid = 0; gid < galnum; gid++)
                {
                    var gcolnum = int.Parse(input[line++]);
                    var gcolors = new Color[gcolnum];

                    // Parse galaxy colors
                    for (var gcid = 0; gcid < gcolnum; gcid++)
                    {
                        var gcbits = input[line++].Split(
                            new[] { ' ' },
                            StringSplitOptions.RemoveEmptyEntries
                        );

                        var gcname = gcbits[0];
                        var gctime = int.Parse(gcbits[1]);

                        gcolors[gcid] = new Color(colors.Where((x) => x.Name == gcname).First()) { Delay = gctime };
                    }

                    galaxies[gid] = new Galaxy(gid, gcolors);
                }

                var wrmnum = int.Parse(input[line++]);
                var wrmhls = new Wormhole[wrmnum];

                // Parse wormholes
                for (var wid = 0; wid < wrmnum; wid++)
                {
                    var wbits = input[line++].Split(
                        new[] { ' ' },
                        StringSplitOptions.RemoveEmptyEntries
                    );

                    var wcolor = colors.Where((x) => x.Name == wbits[0]).First();
                    var worign = int.Parse(wbits[1]);
                    var wtargt = int.Parse(wbits[2]);

                    var gorign = galaxies[worign];
                    var gtargt = galaxies[wtargt];

                    var wormhole = new Wormhole(
                        wcolor, gorign, gtargt
                    );

                    wrmhls[wid] = wormhole;

                    gorign.Wormholes.Add(wormhole);
                 // gtargt.Wormholes.Add(wormhole);
                }

                output.Add(new Tuentiverse(csid, colors, galaxies, wrmhls));
            }

            return output;
        }
    }

    class Color
    {
        public int Id
        {
            get;
        }

        public string Name
        {
            get;
        }

        public int Delay
        {
            get;
            set;
        }

        public bool Primary
        {
            get
            {
                return Composites != null;
            }
        }

        public Color[] Composites
        {
            get;
        }

        public Color(Color color)
        {
            Id = color.Id;
            Name = color.Name;
            Composites = color.Composites;
        }

        public Color(int id, string name, Color[] composites)
        {
            Id = id;
            Name = name;
            Composites = composites;
        }
    }

    class Entity
    {
        public int AvailableColors
        {
            get;
            set;
        }

        public Galaxy CurrentGalaxy
        {
            get;
        }

        public Dictionary<int, Color> MyColors
        {
            get;
        }

        public bool HasColor(Color color)
        {
            bool value = true;

            if (color.Composites?.Length == 0)
                value = (AvailableColors & color.Id) != 0;

            else
            {
                foreach (var comp in color.Composites)
                {
                    value &= (AvailableColors & comp.Id) != 0;
                }
            }

            return value;
        }

        public void EnableColors(int colors)
        {
            AvailableColors |= colors;
        }

        public void DisableColors(int colors)
        {
            AvailableColors &= ~colors;
        }

        public Entity()
        {
            MyColors = new Dictionary<int, Color>();
        }
    }

    class Galaxy
    {
        public int Id
        {
            get;
        }

        public Dictionary<int,Color> Colors
        {
            get;
        }

        public List<Wormhole> Wormholes
        {
            get;
        }

        public bool CanAccessGalaxy(Entity entity)
        {
            return false;
        }

        public Galaxy(int id, Color[] colors)
        {
            Id = id;
            Colors = new Dictionary<int, Color>();

            foreach (var color in colors)
            {
                Colors.Add(color.Id, color);
            }

            Wormholes = new List<Wormhole>();
        }

        public override string ToString()
        {
            return $"Galaxy[Id={Id}, Colours={string.Join<Color>(", ", Colors.Values)}, Wormholes={string.Join(", ", Wormholes)}]";
        }
    }

    class Wormhole
    {
        public Color Color
        {
            get;
        }

        public Galaxy Origin
        {
            get;
        }

        public Galaxy Target
        {
            get;
        }

        public Wormhole(Color color, Galaxy origin, Galaxy target)
        {
            Color  = color;
            Origin = origin;
            Target = target;
        }

        public bool IsTravelingAllowed(Entity entity)
        {
            return entity.HasColor(Color);
        }
    }

    class Tuentiverse
    {
        public int Id
        {
            get;
        }

        public List<Wormhole> Wormholes
        {
            get;
        }

        public Dictionary<int, Color> Colors
        {
            get;
        }

        public Dictionary<int, Galaxy> Galaxies
        {
            get;
        }

        public Tuentiverse(int id, Color[] colors, Galaxy[] galaxies, Wormhole[] wormholes)
        {
            Id = id;

            Wormholes = new List<Wormhole>();
            Wormholes.AddRange(wormholes);

            Colors = new Dictionary<int, Color>();
            foreach (var color in colors)
            {
                Colors.Add(color.Id, color);
            }

            Galaxies = new Dictionary<int, Galaxy>();
            foreach (var galaxy in galaxies)
            {
                Galaxies.Add(galaxy.Id, galaxy);
            }
        }

        public int TryCalculateTimeToReachGalaxy(int galaxyId)
        {
            var elapsedTime = 0;

            var blob = new Entity();

            var targetGalaxy  = Galaxies[galaxyId];
            var currentGalaxy = Galaxies[0];
            var previusGalaxy = (Galaxy)null;

            var colorTimes = new List<Tuple<int, int>>();

            // Run through all galaxies
            // Till we find our destination
            while (currentGalaxy != null)
            {
                var nextGalaxy = (Galaxy) null;

                // Did we traveled?
                if (previusGalaxy != null)
                {
                    foreach (var wormhole in previusGalaxy.Wormholes)
                    {
                        // Is this the wormhole we moved on?
                        if (wormhole.Origin == previusGalaxy 
                        &&  wormhole.Target == currentGalaxy)
                        {
                            // Add elapsed time from prev. galaxy
                            var color = wormhole.Color;
                            
                            if (color.Composites.Length == 0)
                            {
                                elapsedTime += blob.MyColors[color.Id].Delay;
                            }
                            else
                            {
                                foreach (var composite in color.Composites)
                                {
                                    elapsedTime += blob.MyColors[composite.Id].Delay;
                                }
                            }

                            // Remove drained color energy
                            if (color.Composites?.Length == 0)
                            {
                                blob.DisableColors(color.Id);
                                blob.MyColors.Remove(color.Id);
                            }

                            else
                            {
                                // check if composite != null if != null delte composites, else delete id!
                                foreach (var composite in color.Composites)
                                {
                                    blob.DisableColors(composite.Id);
                                    blob.MyColors.Remove(color.Id);
                                }
                            }
                            break;
                        }
                    }
                }

                // Are we at our destination?
                if (currentGalaxy.Id == targetGalaxy.Id)
                    break;

                // Add all colors of our current galaxy,
                // We'll sort out which to pick later.
                foreach (var color in currentGalaxy.Colors.Values)
                {
                    if (color.Composites?.Length == 0)
                    {
                        blob.EnableColors(color.Id);
                    }

                    else
                    {
                        foreach (var composite in color.Composites)
                        {
                            blob.EnableColors(composite.Id);
                        }
                    }
                }
                
                foreach (var wormhole in currentGalaxy.Wormholes)
                {
                    if (wormhole.IsTravelingAllowed(blob))
                    {
                        foreach (var color in currentGalaxy.Colors.Values)
                        {
                            if (color.Composites?.Length == 0)
                            {
                                if (!blob.MyColors.ContainsKey(color.Id))
                                {
                                    blob.MyColors.Add(color.Id, color);
                                }
                            }

                            else
                            {
                                foreach (var composite in color.Composites)
                                {
                                    if (!blob.MyColors.ContainsKey(composite.Id))
                                    {
                                        blob.MyColors.Add(composite.Id, color);
                                    }
                                }
                            }
                        }
                        
                        nextGalaxy = wormhole.Target;
                    }
                }

                // start exploring next galaxy
                previusGalaxy = currentGalaxy;
                currentGalaxy = nextGalaxy;
            }

            return elapsedTime;
        }

        public string ResolveTuentiverseSpaceTime()
        {
            var outputs = new List<int>();

            foreach (var galaxy in Galaxies.Values)
            {
                outputs.Add(TryCalculateTimeToReachGalaxy(galaxy.Id));
            }

            return string.Join(" ", outputs);
        }
    }
}
