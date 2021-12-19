using GithubApiWorkers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GithubApiWorkers.Contract
{
	public interface IResultDispatcher
	{
		void DispatchNewRepos(ReposResultModel repos);
		void DispatchMostMentioned(ReposResultModel repos);
		void DispatchPopularLanguages(ReposResultModel repos);
	}
}
