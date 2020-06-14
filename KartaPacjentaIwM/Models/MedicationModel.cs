using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using static Hl7.Fhir.Model.MedicationRequest;

namespace KartaPacjentaIwM.Models
{
	public class MedicationModel
	{
		public string Id { get; set; }
		[DisplayName("Version id")]
		public string VersionId { get; set; }
		[DisplayName("Medication description")]
		public string Text { get; set; }
		[DisplayName("Patient id")]
		public string SubjectId { get; set; }
		public string Practitioner { get; set; }
		[DisplayName("Dosage Instructions")]
		public List<Dosage> DosageInstructions { get; set; }
		public medicationrequestStatus? Status { get; set; }
		[DisplayName("Authorization date")]

		public DateTimeOffset? AuthoredOn { get; set; }
		public medicationRequestIntent? Intent { get; internal set; }
	}
}