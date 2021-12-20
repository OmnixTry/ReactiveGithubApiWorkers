using GithubApiWorkers.Contract;
using GithubApiWorkers.Models;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace GithubApiWorkers.Services
{
	public class GithubConnectionService
	{
		private readonly GitHubClient client;
		private readonly ReposDataUpdateService reposDataUpdate;
		private readonly IDistinctNameService distinctNameService;
		private readonly IResultDispatcher resultDispatcher;
		private readonly Dictionary<int, List<IDisposable>> KeywordConnections;

		public GithubConnectionService(GitHubClient client, ReposDataUpdateService reposDataUpdate, IDistinctNameService distinctNameService, IResultDispatcher resultDispatcher)
		{
			this.client = client;
			this.reposDataUpdate = reposDataUpdate;
			this.distinctNameService = distinctNameService;
			this.resultDispatcher = resultDispatcher;
			this.KeywordConnections = new Dictionary<int, List<IDisposable>>();
		}

		public async Task<SearchCodeResult> LoadPage(string keyword, int page, Language language, int perPage = 100)
		{
			var searchRequest = new SearchCodeRequest(keyword);
			searchRequest.SortField = CodeSearchSort.Indexed;
			//searchRequest.Language = language;
			searchRequest.Page = page;
			searchRequest.PerPage = perPage;
			var res = await client.Search.SearchCode(searchRequest);
			return res;
		}

		public IObservable<IEnumerable<RepoModel>> QuerrySeriesOfPages(string keyword, int numberOfPages, Language language, int perPage = 100)
		{
			IObservable<SearchCodeResult>[] pageQuerries = new IObservable<SearchCodeResult>[numberOfPages];
			for(int i = 0; i < numberOfPages; i++)
			{
				int p = i + 1;
				pageQuerries[i] = Observable.FromAsync(
					async () => 
					await LoadPage(keyword, p, language, perPage)
					);
			}

			var observable = Observable.Merge(pageQuerries).Take(numberOfPages).Buffer(numberOfPages).Select(x =>
			{
				return x.SelectMany(x => x.Items);
			}).Select(x => {
				
				return x.GroupBy(x => x.Repository.Id, (x) => (x.Repository, x.Name)).Select(g => {
					string lang = "";
					try
					{
						lang = g.First().Name.Split('.')[1];
					}catch(Exception e) { }
					return new RepoModel()
					{
						FullName = g.First().Repository.FullName,
						Language = lang,
						Name = g.First().Repository.Name,
						Url = g.First().Repository.Url,
						Date = DateTime.Now,
						Quantity = g.Count()
					};
				});
			});

			return observable;
		}

		public async Task RunKeywordPulling(WorkerCreationModel creationModel, int perPage = 100)
		{
			reposDataUpdate.AddWord(creationModel.KeywordId);
			TimeSpan time = TimeSpan.FromSeconds(creationModel.Frequency);
			
			var connection = Observable.Interval(time).Subscribe(x =>
			{
				Console.WriteLine(x);
				QuerrySeriesOfPages(creationModel.Keyword, creationModel.NumberOfPages, Enum.Parse<Language>(creationModel.Language), perPage)
				.Subscribe(x => {
					var list = x.OrderByDescending(c => c.Quantity).ToList();
					reposDataUpdate.SendWord(creationModel.KeywordId, list);
					Console.WriteLine(list.Count());

				}, e => { Console.WriteLine(e.Message); } );
			});

			KeywordConnections.Add(creationModel.KeywordId, new List<IDisposable>());
			var newReposCOnnection = reposDataUpdate.Repos[creationModel.KeywordId].Subscribe(x => ProcessNewRepos(creationModel.KeywordId, x));
			var top5ReposConnection = reposDataUpdate.Repos[creationModel.KeywordId].Subscribe(async x => await ProcessTop5(creationModel.KeywordId, x));
			//var toplangsConnection = reposDataUpdate.Repos[creationModel.KeywordId].Subscribe(async x => await ProcessTop5(creationModel.KeywordId, x));
			KeywordConnections[creationModel.KeywordId].Add(connection);
			KeywordConnections[creationModel.KeywordId].Add(newReposCOnnection);
			KeywordConnections[creationModel.KeywordId].Add(top5ReposConnection);
			var asObs = await Observable.Interval(time).FirstOrDefaultAsync();
			Console.WriteLine(asObs);

		}

		public void RemoveKeyword(int keywordId)
		{
			foreach (var item in KeywordConnections[keywordId])
			{
				item.Dispose();
			}
			KeywordConnections.Remove(keywordId);
		}

		private IEnumerable<RepoModel> ProcessNewRepos(int keywordId, IEnumerable<RepoModel> repos)
		{
			Console.WriteLine("ProcessNewRepos");
			var distinctRepos = distinctNameService.FilterUnique(keywordId, repos);
			resultDispatcher.DispatchNewRepos(new ReposResultModel() { KeywordId = keywordId, Data = repos});
			ProcessTopLanguages(keywordId);
			return distinctRepos;
		}

		private async Task<IEnumerable<RepoModel>> ProcessTop5(int keywordId, IEnumerable<RepoModel> repos)
		{
			Console.WriteLine("ProcessTop5");
			var prevTop = await reposDataUpdate
				.Top5Repos[keywordId]
				.FirstOrDefaultAsync();
			var newTop = prevTop
				.Concat(repos)
				.GroupBy(x => x.FullName)
				.Select(x => x.OrderByDescending(r => r.Quantity).First())
				.OrderByDescending(x => x.Quantity)
				.Take(5);

			prevTop = await reposDataUpdate
				.Top5Repos[keywordId]
				.FirstOrDefaultAsync();
			if(prevTop.Count() != newTop.Count() || AreDifferent())
			{
				reposDataUpdate.Top5Repos[keywordId].OnNext(newTop);
				resultDispatcher.DispatchMostMentioned(new ReposResultModel() { KeywordId = keywordId, Data = newTop });
				return newTop;

			}
			return null;


			bool AreDifferent()
			{
				foreach(var item in prevTop.Zip(newTop))
				{
					if(item.First.FullName != item.Second.FullName || item.First.Quantity != item.Second.Quantity)
					{
						return true;
					}
				}
				return false;
			}
		}

		private IEnumerable<LanguageModel> ProcessTopLanguages(int keywordId)
		{
			Console.WriteLine("ProcessLanguages");
			var langs = distinctNameService
				.GetTodayRepos(keywordId)
				.GroupBy(x => x.Language)
				.Select<IGrouping<string, RepoModel>, LanguageModel>(x => new LanguageModel() { Language = x.Key, Frequency = x.Sum(r => r.Quantity) })
				.OrderByDescending(x => x.Frequency).ToList();
			resultDispatcher.DispatchPopularLanguages(new LanguageResultModel() { KeywordId = keywordId, Data = langs });
			return langs;
		}
	}
}
