﻿using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Text;
using ServerExperiment.Models.FHIR.Mappers;
using ServerExperiment.Models.POCO;
using ServerExperiment.Utils;
using ServerExperiment.Models.Repository;

namespace ServerExperiment.Controllers.FhirControllers
{
    public class ObservationController : ApiController
    {
        //private ObservationRepository observationRepository = new ObservationRepository();

        private readonly IObservationRepository observationRepository;

        public ObservationController(IObservationRepository repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            this.observationRepository = repository;
        }

        // GET: fhir/Observation/5
        [Route("fhir/Observation/{observationId}")]
        [HttpGet]
        [RequireHttps]
        public HttpResponseMessage Read(int observationId, string _format = "application/xml+FHIR", bool _summary = false)
        {
            HttpResponseMessage message = new HttpResponseMessage();

            Observation observation = (Observation)observationRepository.GetResourceByID(observationId);
            if (observation == null)
            {
                message.StatusCode = HttpStatusCode.NotFound;
                message.Content = new StringContent("observation with id " + observationId + " not found!", Encoding.UTF8, "text/html");
                return message;
            }
            if (observation.IsDeleted)
            {
                message.StatusCode = HttpStatusCode.Gone;
                message.Content = new StringContent("Observation with id " + observationId + " has been deleted!", Encoding.UTF8, "text/html");
                return message;
            }

            Hl7.Fhir.Model.Observation fhirObservation = ObservationMapper.MapModel(observation);

            ObservationRecord record = (ObservationRecord)observationRepository.GetLatestRecord(observationId);

            string fixedFormat = ControllerUtils.FixMimeString(_format);
            string payload = ControllerUtils.Serialize(fhirObservation, fixedFormat, _summary);
            message.Content = new StringContent(payload, Encoding.UTF8, fixedFormat);
            message.Content.Headers.LastModified = record.LastModified;

            return message;
        }

        // PUT: fhir/Observation/5
        [Route("fhir/Observation/{observationId}")]
        [HttpPut]
        [RequireHttps]
        public HttpResponseMessage Update(Hl7.Fhir.Model.Resource resource, int observationId, string _format = "application/xml+FHIR", bool test = false)
        {
            HttpResponseMessage message = new HttpResponseMessage();

            Hl7.Fhir.Model.Observation fhirObservation = null;
            // Check that input resource is of type Observation
            try
            {
                fhirObservation = (Hl7.Fhir.Model.Observation)resource;
            }
            catch (Exception ex)
            {
                message.Content = new StringContent("Resource is of the wrong type, expecting Observation!", Encoding.UTF8, "text/html");
                message.StatusCode = HttpStatusCode.NotFound;
                return message;
            }
            // Check that input resource has a logical ID
            if (fhirObservation.Id == null)
            {
                message.Content = new StringContent("Observation to be updated should have a logical ID!", Encoding.UTF8, "text/html");
                message.StatusCode = HttpStatusCode.BadRequest;
                return message;
            }
            // Check that URL resource ID == input resource logical ID
            if (observationId != int.Parse(fhirObservation.Id))
            {
                message.StatusCode = HttpStatusCode.BadRequest;
                message.Content = new StringContent("Mismatch of observation ID! Provided " + observationId + " in URL but found " + fhirObservation.Id + "in payload!", Encoding.UTF8, "text/html");
                return message;
            }

            Observation observation = ObservationMapper.MapResource(fhirObservation);

            // Determine MediaTypeFormatter to use
            MediaTypeFormatter formatter = ControllerUtils.ChooseMediaTypeFormatter(_format);

            if (ObservationExists(observationId))
            {
                ObservationRecord record = (ObservationRecord)observationRepository.GetLatestRecord(observationId);
                record = (ObservationRecord)observationRepository.AddUpdateRecord(observation, record);
                observationRepository.UpdateResource(observation);

                observationRepository.Save(); // Look out for DbUpdateConcurrencyException
                
                // Save metadata
                fhirObservation = (Hl7.Fhir.Model.Observation)observationRepository.AddMetadata(observation, fhirObservation, record);

                message.StatusCode = HttpStatusCode.OK;
                message.Content = new ObjectContent(typeof(Hl7.Fhir.Model.Observation), fhirObservation, formatter);
            }
            else
            {
                observationRepository.AddResource(observation);
                observationRepository.Save();

                ObservationRecord record = (ObservationRecord)observationRepository.AddCreateRecord(observation);
                observationRepository.Save();

                // Save metadata
                fhirObservation = (Hl7.Fhir.Model.Observation)observationRepository.AddMetadata(observation, fhirObservation, record);

                // For testing purposes only.
                if (test)
                    observation.ObservationId = 7357;

                message.Content = new ObjectContent(typeof(Hl7.Fhir.Model.Observation), fhirObservation, formatter);
                message.StatusCode = HttpStatusCode.Created;
                message.Headers.Location = new Uri(Url.Link("SpecificObservation", new { id = observation.ObservationId }));
            }

            return message;
        }

        // POST: fhir/Observation
        [Route("fhir/Observation")]
        [HttpPost]
        [RequireHttps]
        public HttpResponseMessage Create(Hl7.Fhir.Model.Resource resource, string _format = "application/xml+FHIR", bool test = false)
        {
            HttpResponseMessage message = new HttpResponseMessage();

            Hl7.Fhir.Model.Observation fhirObservation = null;
            try
            {
                fhirObservation = (Hl7.Fhir.Model.Observation)resource;
            }
            catch (Exception ex)
            {
                message.Content = new StringContent("Resource is of the wrong type, expecting Observation!", Encoding.UTF8, "text/html");
                message.StatusCode = HttpStatusCode.NotFound;
                return message;
            }

            if (fhirObservation.Id != null)
            {
                message.Content = new StringContent("Observation to be added should NOT already have a logical ID!", Encoding.UTF8, "text/html");
                message.StatusCode = HttpStatusCode.BadRequest;
                return message;
            }

            Observation observation = ObservationMapper.MapResource(fhirObservation);
            observationRepository.AddResource(observation);
            observationRepository.Save();

            ObservationRecord record = (ObservationRecord)observationRepository.AddCreateRecord(observation);
            observationRepository.Save();

            // For testing purposes only.
            if (test && observation.ObservationId == 0)
                observation.ObservationId = 7357;

            // Save metadata
            fhirObservation = (Hl7.Fhir.Model.Observation)observationRepository.AddMetadata(observation, fhirObservation, record);

            // Determine MediaTypeFormatter to use
            MediaTypeFormatter formatter = ControllerUtils.ChooseMediaTypeFormatter(_format);

            message.Content = new ObjectContent(typeof(Hl7.Fhir.Model.Observation), fhirObservation, formatter);
            message.StatusCode = HttpStatusCode.Created;
            message.Headers.Location = new Uri(Url.Link("SpecificObservation", new { id = observation.ObservationId }));

            return message;
        }

        // DELETE: fhir/Observation/5
        [Route("fhir/Observation/{observationId}")]
        [HttpDelete]
        [RequireHttps]
        public HttpResponseMessage Delete(int observationId)
        {
            HttpResponseMessage message = new HttpResponseMessage();

            Observation observation = (Observation)observationRepository.GetResourceByID(observationId);
            if (observation == null)
            {
                message.StatusCode = HttpStatusCode.NoContent;
                return message;
            }
            if (observation.IsDeleted)
            {
                message.StatusCode = HttpStatusCode.OK;
                message.ReasonPhrase = "Resource already deleted";
                return message;
            }

            observationRepository.DeleteResource(observation); // Does not actually delete from DB, simply flips isDeleted flag.

            ObservationRecord record = (ObservationRecord)observationRepository.GetLatestRecord(observationId);
            observationRepository.AddDeleteRecord(observation, record);
            observationRepository.Save();

            message.StatusCode = HttpStatusCode.NoContent;
            return message;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                observationRepository.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ObservationExists(int id)
        {
            return observationRepository.ResourceExists(id);
        }
    }
}