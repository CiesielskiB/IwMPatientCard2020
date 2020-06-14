using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KartaPacjentaIwM.Models
{
	public class PatientListViewModel
	{
		public List<PatientModel> Patients { get; set; }
		public string SearchString { get; set; }
	}
}
