using Hl7.Fhir.Model;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KartaPacjentaIwM.Models
{
	public class PatientModel
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Surname { get; set; }
		public AdministrativeGender Gender { get; set; }
		[DisplayName("Brith date")]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
		public string BirthDate { get; set; }
		public string Address { get; set; }
		public string City { get; set; }
		[DisplayName("Version id")]
		public string VersionId { get; set; }
		public string Active { get; set; }

		[DisplayName("Last updated date")]
		public DateTimeOffset? LastUpdated { get; set; }
	}
}