using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GithubApiWorkers.Models
{
	public class WorkerCreationModel
	{
		public int KeywordId { get; set; }
		public string Keyword { get; set; }
		public int Frequency { get; set; } = 1;
	}
}
