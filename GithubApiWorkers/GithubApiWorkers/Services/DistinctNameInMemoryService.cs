using GithubApiWorkers.Contract;
using GithubApiWorkers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GithubApiWorkers.Services
{
	public class DistinctNameInMemoryService: IDistinctNameService
	{
		private Dictionary<int, Dictionary<string, RepoModel>> Existing { get; set; } = new Dictionary<int, Dictionary<string, RepoModel>>();

		public IEnumerable<RepoModel> FilterUnique(int keyword, IEnumerable<RepoModel> repos)
		{
			var newList = new List<RepoModel>();
			if (!Existing.ContainsKey(keyword))
			{
				Existing.Add(keyword, new Dictionary<string, RepoModel>());
				foreach (var item in repos)
					Existing[keyword].Add(item.FullName, item);
				return repos;
			}
			foreach (var item in repos)
			{
				if (!Existing[keyword].ContainsKey(item.FullName))
				{
					Existing[keyword].Add(item.FullName, item);
					newList.Add(item);
				}
				else if(Existing[keyword][item.FullName].Quantity != item.Quantity)
				{
					Existing[keyword][item.FullName] = item;
				}
			}
			return newList;
		}
	}
}
