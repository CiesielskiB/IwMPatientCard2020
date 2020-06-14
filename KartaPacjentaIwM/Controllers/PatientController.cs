using KartaPacjentaIwM.Interfaces;
using KartaPacjentaIwM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace KartaPacjentaIwM.Controllers
{
	public class PatientController : Controller
	{
		public readonly IFhirDataLoader _fhirDataLoader;

		public PatientController(IFhirDataLoader loader)
		{
			_fhirDataLoader = loader;
		}

		public IActionResult Index(string searchString)
		{
			var patients = _fhirDataLoader.GetPatientList();

			if (!string.IsNullOrEmpty(searchString))
			{
				patients = patients.Where(s => s.Surname.Contains(searchString));
			}

			var patientsViewModel = new PatientListViewModel
			{
				Patients = patients.ToList(),
				SearchString = searchString
			};
			return View(patientsViewModel);
		}

		[HttpGet]
		public IActionResult Details(string id, int? fromMonthObs, int? fromYearObs, int? toMonthObs, int? toYearObs, int? fromMonthMed, int? fromYearMed, int? toMonthMed, int? toYearMed)
		{
			var startObsDate = DateTimeOffset.MinValue;
			var endObsDate = DateTimeOffset.MaxValue;
			var startMedDate = DateTimeOffset.MinValue;
			var endMedDate = DateTimeOffset.MaxValue;

			if (fromMonthObs.HasValue && fromYearObs.HasValue && toMonthObs.HasValue && toYearObs.HasValue)
			{
				if(fromYearObs > toYearObs)
				{
					ModelState.AddModelError("FromMonthObs", "Date range is invalid");
				}
				else if(fromYearObs == toYearObs && fromMonthObs > toMonthObs)
				{
					ModelState.AddModelError("FromMonthObs", "Date range is invalid");
				}
				else
				{
					startObsDate = new DateTime(fromYearObs.Value, fromMonthObs.Value, 1);
					endObsDate = new DateTime(toYearObs.Value, toMonthObs.Value, DateTime.DaysInMonth(toYearObs.Value, toMonthObs.Value));
				}
			}
			
			if (fromMonthMed.HasValue && fromYearMed.HasValue && toMonthMed.HasValue && toYearMed.HasValue)
			{
				if (fromYearMed > toYearMed)
				{
					ModelState.AddModelError("ToMonthMed", "Date range is invalid");
				}
				else if (fromYearMed == toYearMed && fromMonthMed > toMonthMed)
				{
					ModelState.AddModelError("ToMonthMed", "Date range is invalid");
				}
				else
				{
					startMedDate = new DateTime(fromYearMed.Value, fromMonthMed.Value, 1);
					endMedDate = new DateTime(toYearMed.Value, toMonthMed.Value, DateTime.DaysInMonth(toYearMed.Value, toMonthMed.Value));
				}
			}

			ViewBag.Months = new SelectList(Enumerable.Range(1, 12).Select(x =>
				  new SelectListItem()
				  {
					  Text = CultureInfo.InvariantCulture.DateTimeFormat.AbbreviatedMonthNames[x - 1] + " (" + x + ")",
					  Value = x.ToString()
				  }), "Value", "Text");
			

			var observations = _fhirDataLoader.GetObservationsForPatient(id).OrderBy(obs => obs.IssuedDate).ToList();
			var earliestDateObs = observations.First(x => x.IssuedDate.HasValue).IssuedDate.Value;
			var latestDateObs = observations.Last(x => x.IssuedDate.HasValue).IssuedDate.Value;


			var medicationRequests = _fhirDataLoader.GetMedicationsForPatient(id).OrderBy(med => med.AuthoredOn).ToList();
			var earliestDateMed = medicationRequests.First(x => x.AuthoredOn.HasValue).AuthoredOn.Value;
			var latestDateMed = medicationRequests.Last(x => x.AuthoredOn.HasValue).AuthoredOn.Value;
			var patient = _fhirDataLoader.GetPatient(id);

			ViewBag.YearsObs = new SelectList(Enumerable.Range(earliestDateObs.Year, latestDateObs.Year - earliestDateObs.Year + 1).Select(x =>

					   new SelectListItem()
					   {
						   Text = x.ToString(),
						   Value = x.ToString()
					   }), "Value", "Text");

			ViewBag.YearsMed = new SelectList(Enumerable.Range(earliestDateMed.Year, latestDateMed.Year - earliestDateMed.Year + 1).Select(x =>

					   new SelectListItem()
					   {
						   Text = x.ToString(),
						   Value = x.ToString()
					   }), "Value", "Text") ;

			var viewModel = new DetailsViewModel
			{
				Patient = patient,
				EarliestObservationDate = earliestDateObs,
				LatestObservationDate = latestDateObs,
				Observations = observations.Where(obs => obs.IssuedDate >= startObsDate && obs.IssuedDate <= endObsDate).ToList(),
				EarliestMediacationRequestDate = earliestDateMed,
				LatestMediacationRequestDate = latestDateMed,
				MedicationRequests = medicationRequests.Where(med => med.AuthoredOn >= startMedDate && med.AuthoredOn <= endMedDate).ToList(),
				FromMonthObs = fromMonthObs ?? 1,
				FromYearObs = fromYearObs ?? earliestDateObs.Year,
				ToMonthObs = toMonthObs ?? 12,
				ToYearObs = toYearObs ?? latestDateObs.Year,
				FromMonthMed = fromMonthMed ?? 1,
				FromYearMed = fromYearMed ?? earliestDateMed.Year,
				ToMonthMed = toMonthMed ?? 12,
				ToYearMed = toYearMed ?? latestDateMed.Year
			};

			return View(viewModel);
		}

		[HttpGet]
		public IActionResult ObservationDetails(string id)
		{
			var observation = _fhirDataLoader.GetObservation(id);
			
			return View(observation);
		}


		[HttpGet]
		public IActionResult MedicationRequestDetails(string id)
		{
			var medication = _fhirDataLoader.GetMedication(id);

			return View(medication);
		}

	}
}