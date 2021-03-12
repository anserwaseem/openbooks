using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace openbooks.Models
{
	public class Request
	{
		public int requestid;
		public int userid;
		public string status;
		public string title;
		public string author;
		public DateTime requestedBookDate;
	}
}