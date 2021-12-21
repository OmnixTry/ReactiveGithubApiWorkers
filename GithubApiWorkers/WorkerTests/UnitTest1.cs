using GithubApiWorkers.Models;
using GithubApiWorkers.Services;
using Moq;
using NUnit.Framework;
using Octokit;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WorkerTests
{
	public class Tests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void ProcessNewRepos_DoesntFfilterNew()
		{
			// Aarrange
			const int id = 1;
			List<RepoModel> repos = new List<RepoModel>() 
			{ 
				new RepoModel() { FullName="first/name", Date = System.DateTime.Now, Language="cs", Name="name", Quantity=2, Url="dddd"},
				new RepoModel() { FullName="second/name", Date = System.DateTime.Now, Language="js", Name="name", Quantity=5, Url="dddd"},
				new RepoModel() { FullName="third/name", Date = System.DateTime.Now, Language="ts", Name="name", Quantity=42, Url="dddd"},
				new RepoModel() { FullName="fourth/name", Date = System.DateTime.Now, Language="cs", Name="name", Quantity=1, Url="dddd"},
				new RepoModel() { FullName="fifth/name", Date = System.DateTime.Now, Language="js", Name="name", Quantity=2, Url="dddd"}
			};

			var client = CreateClient();
			var updateService = CreateupdateService();
			var distinct = CreateDistinctService();
			var dispatcher = CreateDispatcher();
			
			// Act
			var github = new GithubConnectionService(client, updateService, distinct, dispatcher);
			var resulting = github.ProcessNewRepos(id, repos);

			// Assert
			foreach (var item in resulting.Zip(repos))
			{
				Assert.AreEqual(item.First, item.Second);
			}
			
		}

		[Test]
		public void ProcessNewRepos_FiltersDuplicates()
		{
			// Aarrange
			const int id = 1;
			List<RepoModel> repos = new List<RepoModel>()
			{
				new RepoModel() { FullName="first/name", Date = System.DateTime.Now, Language="cs", Name="name", Quantity=2, Url="dddd"},
				new RepoModel() { FullName="second/name", Date = System.DateTime.Now, Language="js", Name="name", Quantity=5, Url="dddd"},
				new RepoModel() { FullName="third/name", Date = System.DateTime.Now, Language="ts", Name="name", Quantity=42, Url="dddd"},
				new RepoModel() { FullName="fourth/name", Date = System.DateTime.Now, Language="cs", Name="name", Quantity=1, Url="dddd"},
				new RepoModel() { FullName="fifth/name", Date = System.DateTime.Now, Language="js", Name="name", Quantity=2, Url="dddd"}
			};

			var client = CreateClient();
			var updateService = CreateupdateService();
			var distinct = CreateDistinctService();
			var dispatcher = CreateDispatcher();

			// Act
			var github = new GithubConnectionService(client, updateService, distinct, dispatcher);
			var resulting = github.ProcessNewRepos(id, repos);
			resulting = github.ProcessNewRepos(id, repos);

			// Assert
			Assert.AreEqual(resulting.Count(), 0);
		}
		[Test]
		public async Task ProcessTop5_IncludesAllNecessary()
		{
			// Aarrange
			const int id = 1;
			List<RepoModel> repos = new List<RepoModel>()
			{
				new RepoModel() { FullName="first/name", Date = System.DateTime.Now, Language="cs", Name="name", Quantity=2, Url="dddd"},
				new RepoModel() { FullName="second/name", Date = System.DateTime.Now, Language="js", Name="name", Quantity=5, Url="dddd"},
				new RepoModel() { FullName="third/name", Date = System.DateTime.Now, Language="ts", Name="name", Quantity=42, Url="dddd"},
				new RepoModel() { FullName="fourth/name", Date = System.DateTime.Now, Language="cs", Name="name", Quantity=1, Url="dddd"},
				new RepoModel() { FullName="fifth/name", Date = System.DateTime.Now, Language="js", Name="name", Quantity=2, Url="dddd"}
			};

			var client = CreateClient();
			var updateService = CreateupdateService();
			updateService.AddWord(id);

			var distinct = CreateDistinctService();
			var dispatcher = CreateDispatcher();

			// Act
			var github = new GithubConnectionService(client, updateService, distinct, dispatcher);
			var resulting = await github.ProcessTop5(id, repos);

			// Assert
			foreach (var item in resulting.Zip(repos.OrderByDescending(x => x.Quantity)))
			{
				Assert.AreEqual(item.First, item.Second);
			}
		}
		[Test]
		public async Task KeywordFiltering_UpdatedUantitY()
		{
			// Aarrange
			const int id = 1;
			List<RepoModel> repos = new List<RepoModel>()
			{
				new RepoModel() { FullName="first/name", Date = System.DateTime.Now, Language="cs", Name="name", Quantity=2, Url="dddd"},
				new RepoModel() { FullName="second/name", Date = System.DateTime.Now, Language="js", Name="name", Quantity=5, Url="dddd"},
				new RepoModel() { FullName="third/name", Date = System.DateTime.Now, Language="ts", Name="name", Quantity=42, Url="dddd"},
				new RepoModel() { FullName="fourth/name", Date = System.DateTime.Now, Language="cs", Name="name", Quantity=1, Url="dddd"},
				new RepoModel() { FullName="fifth/name", Date = System.DateTime.Now, Language="js", Name="name", Quantity=2, Url="dddd"}
			};

			List<RepoModel> repos2 = new List<RepoModel>()
			{
				new RepoModel() { FullName="first/name", Date = System.DateTime.Now, Language="cs", Name="name", Quantity=2, Url="dddd"},
				new RepoModel() { FullName="second/name", Date = System.DateTime.Now, Language="js", Name="name", Quantity=5, Url="dddd"},
				new RepoModel() { FullName="third/name", Date = System.DateTime.Now, Language="ts", Name="name", Quantity=42, Url="dddd"},
				new RepoModel() { FullName="fourth/name", Date = System.DateTime.Now, Language="cs", Name="name", Quantity=1, Url="dddd"},
				new RepoModel() { FullName="fifth/name", Date = System.DateTime.Now, Language="js", Name="name", Quantity=3, Url="dddd"}
			};

			var client = CreateClient();
			var updateService = CreateupdateService();
			updateService.AddWord(id);

			var distinct = CreateDistinctService();
			var dispatcher = CreateDispatcher();

			// Act
			var github = new GithubConnectionService(client, updateService, distinct, dispatcher);
			var resulting = github.ProcessNewRepos(id, repos);
			resulting = github.ProcessNewRepos(id, repos2);

			// Assert
			Assert.AreEqual(1, resulting.Count());
			Assert.AreEqual(repos2.Last(), resulting.First());
			
		}

		[Test]
		public async Task KeyWordFiltering_DistinctName()
		{
			// Aarrange
			const int id = 1;
			List<RepoModel> repos = new List<RepoModel>()
			{
				new RepoModel() { FullName="first/name", Date = System.DateTime.Now, Language="cs", Name="name", Quantity=2, Url="dddd"},
				new RepoModel() { FullName="second/name", Date = System.DateTime.Now, Language="js", Name="name", Quantity=5, Url="dddd"},
				new RepoModel() { FullName="third/name", Date = System.DateTime.Now, Language="ts", Name="name", Quantity=42, Url="dddd"},
				new RepoModel() { FullName="fourth/name", Date = System.DateTime.Now, Language="cs", Name="name", Quantity=1, Url="dddd"},
				new RepoModel() { FullName="fifth/name", Date = System.DateTime.Now, Language="js", Name="name", Quantity=2, Url="dddd"}
			};

			List<RepoModel> repos2 = new List<RepoModel>()
			{
				new RepoModel() { FullName="first/name3", Date = System.DateTime.Now, Language="cs", Name="name", Quantity=2, Url="dddd"},
				new RepoModel() { FullName="second/name3", Date = System.DateTime.Now, Language="js", Name="name", Quantity=5, Url="dddd"},
				new RepoModel() { FullName="third/name3", Date = System.DateTime.Now, Language="ts", Name="name", Quantity=42, Url="dddd"},
				new RepoModel() { FullName="fourth/name3", Date = System.DateTime.Now, Language="cs", Name="name", Quantity=1, Url="dddd"},
				new RepoModel() { FullName="fifth/name3", Date = System.DateTime.Now, Language="js", Name="name", Quantity=3, Url="dddd"}
			};

			var client = CreateClient();
			var updateService = CreateupdateService();
			updateService.AddWord(id);

			var distinct = CreateDistinctService();
			var dispatcher = CreateDispatcher();

			// Act
			var github = new GithubConnectionService(client, updateService, distinct, dispatcher);
			var resulting = github.ProcessNewRepos(id, repos);
			resulting = github.ProcessNewRepos(id, repos2);

			// Assert
			foreach (var item in resulting.Zip(repos2))
			{
				Assert.AreEqual(item.First, item.Second);
			}

		}

		[Test]
		public async Task RunKeywordPulling_PullingCreated()
		{
			// Aarrange
			WorkerCreationModel model = new WorkerCreationModel()
			{
				Keyword = "hello",
				KeywordId = 1,
				Frequency = 100,
				NumberOfPages = 1
			};

			var client = CreateClient();
			var updateService = CreateupdateService();
			var distinct = CreateDistinctService();
			var dispatcher = CreateDispatcher();

			// Act
			var github = new GithubConnectionService(client, updateService, distinct, dispatcher);
			var resulting = github.RunKeywordPulling(model);


			// Assert
			Assert.DoesNotThrow(() => github.RemoveKeyword(model.KeywordId));

		}

		private GithubConnectionService CreateGithub()
		{
			var client = CreateClient();
			var updateService = CreateupdateService();
			GithubConnectionService githubConnection = new GithubConnectionService(client, updateService, new DistinctNameInMemoryService(), new ConsoleAndComunicationDispatcherService());
			return githubConnection;

		}

		private GitHubClient CreateClient()
		{
			string token;
			using (StreamReader reader = new StreamReader("crd.txt"))
			{
				token = reader.ReadLine();
			}
			var productInformation = new ProductHeaderValue("ReactiveWorkers");
			var credentials = new Credentials(token);
			var client = new GitHubClient(productInformation) { Credentials = credentials };
			return client;
		}

		private ReposDataUpdateService CreateupdateService()
		{
			var mock = new Mock<ReposDataUpdateService>();
			//mock.Setup(x => x.)
			return mock.Object;
		}

		private DistinctNameInMemoryService CreateDistinctService()
		{
			//var mock = new Mock<ReposDataUpdateService>();
			//mock.Setup(x => x.)
			return new DistinctNameInMemoryService();
		}

		private ConsoleAndComunicationDispatcherService CreateDispatcher()
		{
			return new ConsoleAndComunicationDispatcherService();
		}
	}
}