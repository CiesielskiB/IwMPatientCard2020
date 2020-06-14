using System;
using System.Collections.Generic;

namespace KartaPacjentaIwM.Models
{
	public class DetailsViewModel
	{
		public PatientModel Patient { get; set; }
		public DateTimeOffset EarliestObservationDate { get; set; }
		public DateTimeOffset LatestObservationDate { get; set; }

		public DateTimeOffset EarliestMediacationRequestDate { get; set; }
		public DateTimeOffset LatestMediacationRequestDate { get; set; }

		public List<ObservationModel> Observations { get; set; }
		public List<MedicationModel> MedicationRequests { get; set; }

		public int FromMonthObs { get; set; }
		public int FromYearObs { get; set; }

		public int ToMonthObs { get; set; }
		public int ToYearObs { get; set; }

		public int FromMonthMed { get; set; }
		public int FromYearMed { get; set; }

		public int ToMonthMed { get; set; }
		public int ToYearMed { get; set; }

	}
}