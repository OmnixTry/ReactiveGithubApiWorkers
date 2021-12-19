using GithubApiWorkers.Models;
using GithubApiWorkers.Services;
using Microsoft.AspNetCore.Mvc;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ProductHeaderValue = Octokit.ProductHeaderValue;

namespace GithubApiWorkers.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class WorkerController : ControllerBase
	{
		
		private readonly GithubConnectionService github;

		public WorkerController(GithubConnectionService github)
		{
			this.github = github;
		}
		[HttpPost]
		public async Task<IActionResult> CreateWorker(WorkerCreationModel creationModel)
		{
			github.RunKeywordPulling(creationModel);
			return Ok();
			/*var productInformation = new ProductHeaderValue("ReactiveWorkers");
			var credentials = new Credentials("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");
			var client = new GitHubClient(productInformation) { Credentials = credentials };

			var searchRequest = new SearchCodeRequest("KmsKeyLoader");
			searchRequest.Language = Language.CSharp;
			searchRequest.SortField = CodeSearchSort.Indexed;
			searchRequest.PerPage = 100;

			var x = await client.Search.SearchCode(searchRequest);
			return Ok(x);*/

			/*var user = await github.User.Get("half-ogre");
			Console.WriteLine(user.Followers + " folks love the half ogre!");
			//?q = addClass
			var message = new HttpRequestMessage()
			{
				Method = HttpMethod.Get
			};
			//message.Headers.Authorization = new AuthenticationHeaderValue("Token", "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");
			message.RequestUri = new Uri($"https://api.github.com/search/code?q=addClass+in:file+language:js+repo:jquery/jquery");
			http.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
			http.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("AppName", "1.0"));

			var x = await http.SendAsync(message);*/
		}

		[HttpDelete]
		public IActionResult RemoveKeyword([FromQuery] int id)
		{
			github.RemoveKeyword(id);
			return Ok();
		}
	}
}
