using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GithubApiWorkers.Models
{
	public class LanguageResultModel
	{
		public int KeywordId { get; set; }

		public IEnumerable<LanguageModel> Data { get; set; }
	}
}
