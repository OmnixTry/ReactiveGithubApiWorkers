using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GithubApiWorkers.Models
{
	public class RepoModel
	{
		public string Name { get; set; }
		public string FullName { get; set; }
		public string Url { get; set; }
		public int Quantity { get; set; }
		public string Language { get; set; }
		public DateTime Date { get; set; }
	}
}
