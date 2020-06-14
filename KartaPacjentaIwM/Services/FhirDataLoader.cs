using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using KartaPacjentaIwM.Interfaces;
using KartaPacjentaIwM.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KartaPacjentaIwM.Services
{
	public class FhirDataLoader : IFhirDataLoader
	{
		private readonly IFhirClient _fhirClient;

		public FhirDataLoader(IFhirClient fhirClient)
		{
			_fhirClient = fhirClient;
			_fhirClient.PreferredFormat = ResourceFormat.Json;
		}

		public PatientModel GetPatient(string id)
		{
			PatientModel result;
			try
			{
				var patient = _fhirClient.Read<Patient>($"Patient/{id}");
				result = GetMappedPatient(patient);
			}
			catch
			{
				result = GetEmptyPatientWithId(id);
			}
			return result;
		}

		public ObservationModel GetObservation(string id)
		{
			ObservationModel result;
			var observation = _fhirClient.Read<Observation>($"Observation/{id}");
			result = GetMappedObservation(observation);
			return result;
		}

		public IEnumerable<ObservationModel> GetObservationsForPatient(string id)
		{
			var result = new List<ObservationModel>();
			var query = new SearchParams()
				.Where($"patient={id}");
			var observations = _fhirClient.Search<Observation>(query);
			while (observations != null)
			{
				foreach (var observationEntry in observations.Entry)
				{
					result.Add(GetMappedObservation(observationEntry.Resource as Observation));
				}

				observations = _fhirClient.Continue(observations);
			}
			return result;
		}

		public MedicationModel GetMedication(string id)
		{
			MedicationModel result;
			var medication = _fhirClient.Read<MedicationRequest>($"MedicationRequest/{id}");
			result = GetMappedMedication(medication);
			return result;
		}

		public IEnumerable<MedicationModel> GetMedicationsForPatient(string id)
		{
			var result = new List<MedicationModel>();
			var query = new SearchParams()
				.Where($"patient={id}");
			var medications = _fhirClient.Search<MedicationRequest>(query);
			while (medications != null)
			{
				foreach (var medicationEntry in medications.Entry)
				{
					result.Add(GetMappedMedication(medicationEntry.Resource as MedicationRequest));
				}

				medications = _fhirClient.Continue(medications);
			}
			return result;
		}

		public IEnumerable<PatientModel> GetPatientList()
		{
			var result = new List<PatientModel>();
			var patients = _fhirClient.Search<Patient>(pageSize: 50);
			foreach (var patient in patients.Entry)
			{
				try
				{
					result.Add(GetMappedPatient(patient.Resource as Patient));
				}
				catch
				{
					result.Add(GetEmptyPatientWithId(patient.Resource.Id));
				}
			}
			return result;
		}

		private MedicationModel GetMappedMedication(MedicationRequest medication)
		{
			var resultMedicationRequest = new MedicationModel
			{
				Id = medication.Id
			};
			resultMedicationRequest.VersionId = medication.VersionId ?? "";
			var meds = medication.Medication as CodeableConcept;
			resultMedicationRequest.Text = meds.Text;
			resultMedicationRequest.DosageInstructions = medication.DosageInstruction;
			resultMedicationRequest.SubjectId = medication.Subject.Reference.Split('/')[1];
			resultMedicationRequest.Status = medication.Status;
			resultMedicationRequest.AuthoredOn = medication.AuthoredOnElement.ToDateTimeOffset();
			resultMedicationRequest.Practitioner = medication.Requester.Display;
			resultMedicationRequest.Intent = medication.Intent;
			return resultMedicationRequest;
		}

		private ObservationModel GetMappedObservation(Observation observation)
		{
			var resultObservation = new ObservationModel
			{
				Id = observation.Id
			};
			resultObservation.VersionId = observation.VersionId ?? "";
			resultObservation.Text = observation.Code.Text;
			resultObservation.SubjectId = observation.Subject.Reference.Split('/')[1];
			resultObservation.IssuedDate = observation.Issued;
			if (observation.Value is Quantity value)
			{
				resultObservation.Value = value.Value ?? 0;
				resultObservation.Unit = value.Unit;
			}
			return resultObservation;
		}

		private PatientModel GetEmptyPatientWithId(string id)
		{
			return new PatientModel
			{
				Id = id,
				Name = "Unknown",
				Surname = "Unknown",
				Gender = AdministrativeGender.Unknown,
				BirthDate = "Unknown",
				Address = "Unknown",
				City = "Unknown",
				VersionId = "Unknown",
				LastUpdated = null,
				Active = null
			};
		}

		private PatientModel GetMappedPatient(Patient patient)
		{
			var resultPatient = new PatientModel
			{
				Id = patient.Id
			};
			if (patient.Name.FirstOrDefault() != null)
			{
				var nameBuild = "";
				foreach (var name in patient.Name.First().Given)
				{
					nameBuild = nameBuild + " " + name;
				}
				resultPatient.Name = nameBuild;
				resultPatient.Surname = patient.Name.First().Family ?? "";
			}
			else
			{
				resultPatient.Name = "";
				resultPatient.Surname = "";
			}

			resultPatient.Gender = patient.Gender.HasValue ? patient.Gender.Value : AdministrativeGender.Unknown;
			resultPatient.BirthDate = patient.BirthDate ?? "";
			if (patient.Address.FirstOrDefault() != null)
			{
				resultPatient.Address = patient.Address.First().Text ?? "";
				resultPatient.City = patient.Address.First().City ?? "";
			}
			else
			{
				resultPatient.Address = "";
				resultPatient.City = "";
			}

			resultPatient.VersionId = patient.VersionId ?? "";
			resultPatient.LastUpdated = patient.Meta.LastUpdated;
			resultPatient.Active = patient.Active.HasValue ? patient.Active.Value ? "Active" : "Not Active" : "Not Active";
			return resultPatient;
		}
	}
}