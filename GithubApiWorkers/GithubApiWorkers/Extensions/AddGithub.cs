using Microsoft.Extensions.DependencyInjection;
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GithubApiWorkers.Extensions
{
	public static class AddGithub
	{
		public static void AddGithubClient(this IServiceCollection services)
		{
			string token;
			using (StreamReader reader = new StreamReader("crd.txt"))
			{
				token = reader.ReadLine();
			}
			var productInformation = new ProductHeaderValue("ReactiveWorkers");
			var credentials = new Credentials(token);
			var client = new GitHubClient(productInformation) { Credentials = credentials };
			services.AddTransient<GitHubClient>(x => { return client; });
		}
	}
}
