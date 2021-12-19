using GithubApiWorkers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GithubApiWorkers.Contract
{
	public interface IDistinctNameService
	{
		IEnumerable<RepoModel> FilterUnique(int keywordId, IEnumerable<RepoModel> repos);

	}
}
