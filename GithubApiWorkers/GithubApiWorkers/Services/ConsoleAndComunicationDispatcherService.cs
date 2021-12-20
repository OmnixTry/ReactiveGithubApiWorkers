using GithubApiWorkers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GithubApiWorkers.Services
{
	public class ConsoleAndComunicationDispatcherService : ConsoleResultDispatcher
	{
		public ConsoleAndComunicationDispatcherService()
		{
			
		}
		public override async Task DispatchMostMentioned(ReposResultModel repos)
		{
			await base.DispatchMostMentioned(repos);
			using (var http = new HttpClient())
			{
				var requestTask = await http.PostAsync("https://github-livetracker-main.herokuapp.com/keywords/top-projects", JsonContent.Create(repos));				
			}
		}

		public override async Task DispatchNewRepos(ReposResultModel repos)
		{
			await base.DispatchNewRepos(repos);
			using (var http = new HttpClient())
			{
				var requestTask = await http.PostAsync("https://github-livetracker-main.herokuapp.com/keywords/new-projects", JsonContent.Create(repos));
			}
		}

		public override async Task DispatchPopularLanguages(LanguageResultModel repos)
		{
			await base.DispatchPopularLanguages(repos);
			using (var http = new HttpClient())
			{
				var requestTask = await http.PostAsync("https://github-livetracker-main.herokuapp.com/keywords/language-frequencies", JsonContent.Create(repos));
 			}
		}
	}
}
