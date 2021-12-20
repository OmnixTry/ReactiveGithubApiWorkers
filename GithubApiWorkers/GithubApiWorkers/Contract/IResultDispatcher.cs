using GithubApiWorkers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GithubApiWorkers.Contract
{
	public interface IResultDispatcher
	{
		Task DispatchNewRepos(ReposResultModel repos);
		Task DispatchMostMentioned(ReposResultModel repos);
		Task DispatchPopularLanguages(LanguageResultModel repos);
	}
}
