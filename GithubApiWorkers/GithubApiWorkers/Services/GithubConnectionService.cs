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

		public async Task<SearchCodeResult> LoadPage(string keyword, int page, int perPage = 100)
		{
			var searchRequest = new SearchCodeRequest("KmsKeyLoader");
			searchRequest.Language = Language.CSharp;
			searchRequest.SortField = CodeSearchSort.Indexed;
			searchRequest.PerPage = 100;

			return await client.Search.SearchCode(searchRequest);
		}
	}
}
