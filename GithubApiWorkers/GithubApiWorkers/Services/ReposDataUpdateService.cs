using GithubApiWorkers.Models;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace GithubApiWorkers.Services
{
	public class ReposDataUpdateService
	{
		public ReposDataUpdateService()
		{
			Repos = new Dictionary<int, Subject<IEnumerable<RepoModel>>>();
			Top5Repos = new Dictionary<int, BehaviorSubject<IEnumerable<RepoModel>>>();
		}

		public Dictionary<int, Subject<IEnumerable<RepoModel>>> Repos { get; set; }
		public Dictionary<int, BehaviorSubject<IEnumerable<RepoModel>>> Top5Repos { get; set; }

		public void SendWord(int wordId, IEnumerable<RepoModel> searchCode)
		{
			Repos[wordId].OnNext(searchCode);			
		}

		public void AddWord(int id)
		{
			Repos.Add(id, new Subject<IEnumerable<RepoModel>>());
			Top5Repos.Add(id, new BehaviorSubject<IEnumerable<RepoModel>>(new List<RepoModel>()));
		}
	}
}
