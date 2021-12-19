using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GithubApiWorkers.Models
{
	public class ReposResultModel
	{
		public int KeywordId { get; set; }
		
		public IEnumerable<RepoModel> Data { get; set; } 
	}
}
