using System;
using System.Collections.Generic;

namespace Dijkstras
{
	class Graph
	{
		Dictionary<char, Dictionary<char, int>> vertices = new Dictionary<char, Dictionary<char, int>>();

		public void add_vertex(char name, Dictionary<char, int> edges)
		{
			vertices[name] = edges;
		}

		public List<char> shortest_path(char start, char finish)
		{
			var previous = new Dictionary<char, char>();
			var distances = new Dictionary<char, int>();
			var nodes = new List<char>();

			List<char> path = null;

			foreach (var vertex in vertices)
			{
				if (vertex.Key == start)
				{
					distances[vertex.Key] = 0;
				}
				else
				{
					distances[vertex.Key] = int.MaxValue;
				}

				nodes.Add(vertex.Key);
			}

			while (nodes.Count != 0)
			{
				nodes.Sort((x, y) => distances[x] - distances[y]);

				var smallest = nodes[0];
				nodes.Remove(smallest);

				if (smallest == finish)
				{
					path = new List<char>();
					while (previous.ContainsKey(smallest))
					{
						path.Add(smallest);
						smallest = previous[smallest];
					}

					break;
				}

				if (distances[smallest] == int.MaxValue)
				{
					break;
				}

				foreach (var neighbor in vertices[smallest])
				{
					var alt = distances[smallest] + neighbor.Value;
					if (alt < distances[neighbor.Key])
					{
						distances[neighbor.Key] = alt;
						previous[neighbor.Key] = smallest;
					}
				}
			}

			return path;
		}
	}

	class MainClass
	{
		public static void Main1(string[] args)
		{
			Graph g = new Graph();
			g.add_vertex('1', new Dictionary<char, int>() { { '2', 1 }, { '6', 6 } });
            g.add_vertex('2', new Dictionary<char, int>() { { '1', 1 }, { '2', 3 }, { '4', 1 }, { '6', 2 } });
            g.add_vertex('3', new Dictionary<char, int>() { { '2', 3 /* 0 */ }, { '4', 4 }, { '5', 5 }, { '6', 5 } });
            g.add_vertex('4', new Dictionary<char, int>() { { '3', 3 } /* 0 */, { '5', 5 }, { '2', 1 } /* 0 */ });
            g.add_vertex('5', new Dictionary<char, int>() { { '4', 4 }, {'6', 6}, { '3', 5 } });
			g.add_vertex('6', new Dictionary<char, int>() { { '5', 5 }, { '7', 7 }, { '3', 5 }, { '1', 6 }, { '2', 2 } });
            g.add_vertex('7', new Dictionary<char, int>() { { '6', 6 }, { '8', 7 /*shtct*/ }, { 'A', 10 /*shtct*/}, {'4', 4} });
			g.add_vertex('8', new Dictionary<char, int>() { { '7', 3 }, { '9', 9 } });
			g.add_vertex('9', new Dictionary<char, int>() { { '8', 8 }, { 'A', 10 } });
			g.add_vertex('A', new Dictionary<char, int>() { { '9', 9 }, { '7', 10 } });

			g.shortest_path('1', 'A').ForEach(x => Console.WriteLine(x));
		}
	}
}