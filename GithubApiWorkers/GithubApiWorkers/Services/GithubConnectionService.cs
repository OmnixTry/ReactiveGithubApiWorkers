using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GithubApiWorkers.Services
{
	public class GithubConnectionService
	{
		private readonly GitHubClient client;

		public GithubConnectionService(GitHubClient client)
		{
			this.client = client;
		}

		public async Task<SearchCodeResult> LoadPage(string keyword, int page, Language language, int perPage = 100)
		{
			var searchRequest = new SearchCodeRequest(keyword);
			searchRequest.SortField = CodeSearchSort.Indexed;
			searchRequest.Language = language;
			searchRequest.Page = page;
			searchRequest.PerPage = perPage;

			return await client.Search.SearchCode(searchRequest);
		}
	}
}
