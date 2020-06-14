using Hl7.Fhir.Model;
using KartaPacjentaIwM.Models;
using System.Collections.Generic;

namespace KartaPacjentaIwM.Interfaces
{
	public interface IFhirDataLoader
	{
		IEnumerable<PatientModel> GetPatientList();
		PatientModel GetPatient(string id);
		IEnumerable<ObservationModel> GetObservationsForPatient(string id);
		ObservationModel GetObservation(string id);
		IEnumerable<MedicationModel> GetMedicationsForPatient(string id);
		MedicationModel GetMedication(string id);
	}
}