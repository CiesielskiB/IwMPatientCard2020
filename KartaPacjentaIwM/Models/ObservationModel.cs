using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace KartaPacjentaIwM.Models
{
	public class ObservationModel
	{
		public string Id { get; set; }
		[DisplayName("Version id")]
		public string VersionId { get; set; }
		[DisplayName("Observation description")]
		public string Text { get; set; }
		[DisplayName("Patient id")]
		public string SubjectId { get; set; }
		[DisplayName("Observation date")]
		public DateTimeOffset? IssuedDate { get; set; }
		public decimal Value { get; set; }
		public string Unit { get; set; }
	}
}
