using GithubApiWorkers.Contract;
using GithubApiWorkers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GithubApiWorkers.Services
{
	public class ConsoleResultDispatcher : IResultDispatcher
	{
		public virtual async Task DispatchMostMentioned(ReposResultModel repos)
		{
			Console.WriteLine("-------\nMostMentioned");
			foreach (var item in repos.Data)
			{
				Console.WriteLine($"{item.FullName} {item.Quantity} {item.Language}");
			}
			Console.WriteLine("-------");
		}

		public virtual async Task DispatchNewRepos(ReposResultModel repos)
		{
			foreach (var item in repos.Data)
			{
				Console.WriteLine($"{item.FullName} {item.Quantity} {item.Language}");
			}
			Console.WriteLine("-------");
		}

		public virtual async Task DispatchPopularLanguages(LanguageResultModel repos)
		{
			Console.WriteLine("-------\nMostPopularLanguages");
			foreach (var item in repos.Data)
			{
				Console.WriteLine($"{item.Language} {item.Frequency}");
			}
			Console.WriteLine("-------");
		}
	}
}
