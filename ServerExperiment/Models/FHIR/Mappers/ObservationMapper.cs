﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Castle.Core.Internal;
using Hl7.Fhir.Model;
using static Hl7.Fhir.Model.Observation;
using ServerExperiment.Models.FHIR.Helpers.Observation;

namespace ServerExperiment.Models.FHIR.Mappers
{
    public class ObservationMapper
    {
        /// <summary>
        /// Given a Observation Resource, maps the data in the resource to a Observation POCO.
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static POCO.Observation MapResource(Resource resource)
        {
            var source = resource as Observation;
            if (source == null)
            {
                throw new ArgumentException("Resource in not a HL7 FHIR observation resouce");
            }

            POCO.Observation observation = new POCO.Observation();

            int resultId = 0;
            int.TryParse(resource.Id, out resultId);
            observation.ObservationId = resultId;

            // observation Status
            var status = source.Status.GetValueOrDefault();
            switch (status)
            {
                case ObservationStatus.Registered:
                    observation.Status = ObsStatus.registered;
                    break;
                case ObservationStatus.Preliminary:
                    observation.Status = ObsStatus.preliminary;
                    break;
                case ObservationStatus.Final:
                    observation.Status = ObsStatus.final;
                    break;
                case ObservationStatus.Amended:
                    observation.Status = ObsStatus.amended;
                    break;
                case ObservationStatus.Cancelled:
                    observation.Status = ObsStatus.cancelled;
                    break;
                case ObservationStatus.Unknown:
                    observation.Status = ObsStatus.unknown;
                    break;
                case ObservationStatus.EnteredInError:
                    observation.Status = ObsStatus.entered_in_error;
                    break;
                default:
                    observation.Status = ObsStatus.entered_in_error;
                    break;
            }

            // Observation Category
            if (source.Category != null)
            {
                foreach (Coding coding in source.Category.Coding)
                {
                    if (!coding.Code .IsNullOrEmpty())
                        observation.CategoryCode.Add(coding.Code);
                    if (!coding.Display.IsNullOrEmpty())
                        observation.CategoryDisplay.Add(coding.Display);
                    if (!coding.System.IsNullOrEmpty())
                        observation.CategorySystem.Add(coding.System);
                }
                observation.CategoryText = source.Category.Text;
            }

            // Observation Code
            if (source.Code != null)
            {
                foreach (Coding coding in source.Code.Coding)
                {
                    if (!coding.Code.IsNullOrEmpty())
                        observation.CodeCode.Add(coding.Code);
                    if (!coding.Display.IsNullOrEmpty())
                        observation.CodeDisplay.Add(coding.Display);
                    if (!coding.System.IsNullOrEmpty())
                        observation.CodeSystem.Add(coding.System);
                }
                observation.CodeText = source.Code.Text;
            }

            // Observation references to other resources
            if (source.Subject != null)
                observation.PatientReference = source.Subject.Reference;
            if (source.Device != null)
                observation.DeviceReference = source.Device.Reference;
            foreach (var reference in source.Performer)
            {
                if (!reference.Reference.IsNullOrEmpty())
                    observation.PerformerReferences.Add(reference.Reference);
            }
            
            // Observation effective times
            if (source.Effective != null)
            {
                if (source.Effective is FhirDateTime)
                {
                    var effective = source.Effective as FhirDateTime;
                    observation.EffectiveDateTime = DateTime.Parse(effective.Value);
                }
                else
                {
                    var effective = source.Effective as Period;
                    observation.EffectivePeriodStart = DateTime.Parse(effective.Start);
                    observation.EffectivePeriodEnd = DateTime.Parse(effective.End);
                }
            }

            // Observation Interpretation
            if (source.Interpretation != null)
            {
                if (!source.Interpretation.Coding.IsNullOrEmpty())
                {
                    observation.InterpretationCode = source.Interpretation.Coding.FirstOrDefault().Code;
                    observation.InterpretationDisplay = source.Interpretation.Coding.FirstOrDefault().Display;
                    observation.InterpretationSystem = source.Interpretation.Coding.FirstOrDefault().System;
                }
                observation.InterpretationText = source.Interpretation.Text;
            }

            // Observation Comments
            if (!source.Comments.IsNullOrEmpty())
                observation.Comments = source.Comments;

            // Site of Body where Observation was made
            if (source.BodySite != null)
            {
                if (!source.BodySite.Coding.IsNullOrEmpty())
                {
                    observation.BodySiteCode = source.BodySite.Coding.FirstOrDefault().Code;
                    observation.BodySiteDisplay = source.BodySite.Coding.FirstOrDefault().Display;
                    observation.BodySiteSystem = source.BodySite.Coding.FirstOrDefault().System;
                }
                observation.BodySiteText = source.BodySite.Text;
            }

            // Observation values / components
            // If only one value, then simply a value type. If more than one, then Component type.
            if (source.Component == null || source.Component.Count == 0) // Must be a value
            {
                if (source.Value is Quantity)
                {
                    var value = source.Value as Quantity;
                    if (!value.Code.IsNullOrEmpty())
                        observation.ValueQuantityCode.Add(value.Code);
                    if (!value.System.IsNullOrEmpty())
                        observation.ValueQuantitySystem.Add(value.System);
                    if (!value.Unit.IsNullOrEmpty())
                        observation.ValueQuantityUnit.Add(value.Unit);
                    observation.ValueQuantityValue.Add((decimal)value.Value);
                }
                else if (source.Value is CodeableConcept)
                {
                    var value = source.Value as CodeableConcept;
                    if (!value.Coding.IsNullOrEmpty())
                    {
                        observation.ValueSystem.Add(value.Coding.FirstOrDefault().System);
                        observation.ValueCode.Add(value.Coding.FirstOrDefault().Code);
                        observation.ValueDisplay.Add(value.Coding.FirstOrDefault().Display);
                    }
                    observation.ValueText.Add(value.Text);
                }
                else if (source.Value is FhirString)
                {
                    var value = source.Value as FhirString;
                    observation.ValueString.Add(value.Value);
                }
                else if (source.Value is SampledData)
                {
                    var value = source.Value as SampledData;
                    observation.ValueSampledDataOriginCode.Add(value.Origin.Code);
                    observation.ValueSampledDataOriginSystem.Add(value.Origin.System);
                    observation.ValueSampledDataOriginUnit.Add(value.Origin.Unit);
                    observation.ValueSampledDataOriginValue.Add((decimal)value.Origin.Value);
                    observation.ValueSampledDataPeriod.Add((decimal)value.Period);
                    observation.ValueSampledDataDimensions.Add((int)value.Dimensions);
                    observation.ValueSampledDataData.Add(value.Data);
                }
                else if (source.Value is Period)
                {
                    var value = source.Value as Period;
                    observation.ValuePeriodStart.Add(DateTime.Parse(value.Start));
                    observation.ValuePeriodEnd.Add(DateTime.Parse(value.End));
                }
            }
            else // Must be composite(s)
            {
                foreach (var component in source.Component)
                {
                    observation.ComponentCodeCode.Add(component.Code.Coding.FirstOrDefault().Code);
                    observation.ComponentCodeSystem.Add(component.Code.Coding.FirstOrDefault().System);
                    observation.ComponentCodeDisplay.Add(component.Code.Coding.FirstOrDefault().Display);
                    observation.ComponentCodeText = component.Code.Text;

                    if (component.Value is Quantity)
                    {
                        var value = component.Value as Quantity;
                        observation.ValueQuantityCode.Add(value.Code);
                        observation.ValueQuantitySystem.Add(value.System);
                        observation.ValueQuantityUnit.Add(value.Unit);
                        observation.ValueQuantityValue.Add((decimal)value.Value);
                    }
                    else if (component.Value is CodeableConcept)
                    {
                        var value = component.Value as CodeableConcept;
                        observation.ValueSystem.Add(value.Coding.FirstOrDefault().System);
                        observation.ValueCode.Add(value.Coding.FirstOrDefault().Code);
                        observation.ValueDisplay.Add(value.Coding.FirstOrDefault().Display);
                        observation.ValueText.Add(value.Text);
                    }
                    else if (component.Value is FhirString)
                    {
                        var value = component.Value as FhirString;
                        observation.ValueString.Add(value.Value);
                    }
                    else if (component.Value is SampledData)
                    {
                        var value = component.Value as SampledData;
                        observation.ValueSampledDataOriginCode.Add(value.Origin.Code);
                        observation.ValueSampledDataOriginSystem.Add(value.Origin.System);
                        observation.ValueSampledDataOriginUnit.Add(value.Origin.Unit);
                        observation.ValueSampledDataOriginValue.Add((decimal)value.Origin.Value);
                        observation.ValueSampledDataPeriod.Add((decimal)value.Period);
                        observation.ValueSampledDataDimensions.Add((int)value.Dimensions);
                        observation.ValueSampledDataData.Add(value.Data);
                    }
                    else if (component.Value is Period)
                    {
                        var value = component.Value as Period;
                        observation.ValuePeriodStart.Add(DateTime.Parse(value.Start));
                        observation.ValuePeriodEnd.Add(DateTime.Parse(value.End));
                    }
                }
            }

            return observation;
        }

        /// <summary>
        /// Given a observation POCO, maps the data to an Observation Resource.
        /// </summary>
        /// <param name="observation"></param>
        /// <returns></returns>
        public static Observation MapModel(POCO.Observation observation)
        {
            if (observation == null)
            {
                throw new ArgumentNullException("observation");
            }

            var resource = new Observation();

            resource.Id = observation.ObservationId.ToString("D");

            // Observation Status
            switch (observation.Status)
            {
                case ObsStatus.amended:
                    resource.Status = ObservationStatus.Amended;
                    break;
                case ObsStatus.cancelled:
                    resource.Status = ObservationStatus.Cancelled;
                    break;
                case ObsStatus.entered_in_error:
                    resource.Status = ObservationStatus.EnteredInError;
                    break;
                case ObsStatus.final:
                    resource.Status = ObservationStatus.Final;
                    break;
                case ObsStatus.preliminary:
                    resource.Status = ObservationStatus.Preliminary;
                    break;
                case ObsStatus.registered:
                    resource.Status = ObservationStatus.Registered;
                    break;
                case ObsStatus.unknown:
                    resource.Status = ObservationStatus.Unknown;
                    break;
                default:
                    resource.Status = ObservationStatus.EnteredInError;
                    break;
            }

            // Observation Category
            if (!observation.CategoryCode.IsNullOrEmpty() || !observation.CategoryDisplay.IsNullOrEmpty() || !observation.CategorySystem.IsNullOrEmpty() || !observation.CategoryText.IsNullOrEmpty())
            {
                CodeableConcept observationCategory = new CodeableConcept();
                List<Coding> observationCodings = new List<Coding>();

                if (!observation.CategoryCode.IsNullOrEmpty() || !observation.CategoryDisplay.IsNullOrEmpty() || !observation.CategorySystem.IsNullOrEmpty())
                {
                    for (int i = 0; i < observation.CategoryCode.Count ; i++)
                    {
                        Coding observationCoding = new Coding()
                        {
                            System = observation.CategorySystem[i],
                            Display = observation.CategoryDisplay[i],
                            Code = observation.CategoryCode[i]
                        };
                        observationCodings.Add(observationCoding);
                    }
                    observationCategory.Coding = observationCodings;
                }
                observationCategory.Text = observation.CategoryText;
                
                resource.Category = observationCategory;
            }

            // Observation Code
            if (!observation.CodeCode.IsNullOrEmpty() || !observation.CodeDisplay.IsNullOrEmpty() || !observation.CodeSystem.IsNullOrEmpty() || !observation.CodeText.IsNullOrEmpty())
            {
                CodeableConcept observationCode = new CodeableConcept();
                List<Coding> observationCodings = new List<Coding>();

                if (!observation.CodeCode.IsNullOrEmpty() || !observation.CodeDisplay.IsNullOrEmpty() || !observation.CodeSystem.IsNullOrEmpty())
                {
                    for (int i = 0; i < observation.CodeCode.Count; i++)
                    {
                        Coding observationCoding = new Coding()
                        {
                            System = observation.CodeSystem[i],
                            Display = observation.CodeDisplay[i],
                            Code = observation.CodeCode[i]
                        };
                        observationCodings.Add(observationCoding);
                    }
                    observationCode.Coding = observationCodings;
                }
                observationCode.Text = observation.CategoryText;

                resource.Code = observationCode;
            }

            // Observation references to other resources
            if (!observation.PatientReference.IsNullOrEmpty())
            {
                resource.Subject = new ResourceReference();
                resource.Subject.Reference = observation.PatientReference;
            }
            if (!observation.DeviceReference.IsNullOrEmpty())
            {
                resource.Device = new ResourceReference();
                resource.Device.Reference = observation.DeviceReference;
            }

            if (!observation.PerformerReferences.IsNullOrEmpty())
            {
                foreach (var reference in observation.PerformerReferences)
                {
                    ResourceReference performerReference = new ResourceReference();
                    performerReference.Reference = reference;
                    resource.Performer.Add(performerReference);
                }
            }
            
            // Observation Effective times
            // The choice of Type is DateTime.
            if (observation.EffectiveDateTime != null) 
            {
                FhirDateTime dateTime = new FhirDateTime(observation.EffectiveDateTime);
                resource.Effective = dateTime;
            }
            // The choice of Type is Period.
            else
            {
                Period period = new Period
                {
                    Start = observation.EffectivePeriodStart.ToString(),
                    End = observation.EffectivePeriodEnd.ToString()
                };
                resource.Effective = period;
            }

            resource.Issued = observation.Issued;
            
            // Observation Comments
            resource.Comments = observation.Comments;

            // Site of Body where Observation was made
            if (!observation.BodySiteCode.IsNullOrEmpty() || !observation.BodySiteDisplay.IsNullOrEmpty() || !observation.BodySiteSystem.IsNullOrEmpty() || !observation.BodySiteText.IsNullOrEmpty())
            {
                CodeableConcept observationBodySite = new CodeableConcept();
                List<Coding> observationCodings = new List<Coding>();

                if (!observation.BodySiteCode.IsNullOrEmpty() || !observation.BodySiteDisplay.IsNullOrEmpty() || !observation.BodySiteSystem.IsNullOrEmpty())
                {
                    Coding observationCoding = new Coding()
                    {
                        System = observation.BodySiteSystem,
                        Display = observation.BodySiteDisplay,
                        Code = observation.BodySiteCode
                    };
                    observationCodings.Add(observationCoding);
                    observationBodySite.Coding = observationCodings;
                }
                observationBodySite.Text = observation.BodySiteText;

                resource.BodySite = observationBodySite;
            }

            // Observation Interpretation
            if (!observation.InterpretationCode.IsNullOrEmpty() || !observation.InterpretationDisplay.IsNullOrEmpty() || !observation.InterpretationSystem.IsNullOrEmpty() || !observation.InterpretationText.IsNullOrEmpty())
            {
                CodeableConcept observationInterpretation = new CodeableConcept();
                List<Coding> observationCodings = new List<Coding>();

                if (!observation.InterpretationCode.IsNullOrEmpty() || !observation.InterpretationDisplay.IsNullOrEmpty() || !observation.InterpretationSystem.IsNullOrEmpty())
                {
                    Coding observationCoding = new Coding()
                    {
                        System = observation.InterpretationSystem,
                        Display = observation.InterpretationDisplay,
                        Code = observation.InterpretationCode
                    };
                    observationCodings.Add(observationCoding);
                    observationInterpretation.Coding = observationCodings;
                }
                observationInterpretation.Text = observation.InterpretationText;

                resource.Interpretation = observationInterpretation;
            }

            // Observation Values
            // Values are componentised
            if (!observation.ComponentCodeCode.IsNullOrEmpty() || !observation.ComponentCodeText.IsNullOrEmpty())
            {
                for (int i = 0; i < observation.ComponentCodeCode.Count; i++)
                {
                    ComponentComponent component = new ComponentComponent();
                    CodeableConcept concept = new CodeableConcept();
                    Coding coding = new Coding
                    {
                        Code = observation.ComponentCodeCode[i],
                        Display = observation.ComponentCodeDisplay[i],
                        System = observation.ComponentCodeSystem[i]
                    };
                    concept.Coding.Add(coding);
                    concept.Text = observation.ComponentCodeText;
                    component.Code = concept;

                    resource.Component.Add(component);

                    // value is of Type Quantity
                    if (!observation.ValueQuantityValue.IsNullOrEmpty())
                    {
                        Quantity quantity = new Quantity
                        {
                            Code = observation.ValueQuantityCode[i],
                            System = observation.ValueQuantitySystem[i],
                            Unit = observation.ValueQuantityUnit[i],
                            Value = observation.ValueQuantityValue[i]
                        };

                        resource.Component[i].Value = quantity;
                    }
                    // value is of Type CodeableConcept
                    else if (!observation.ValueCode.IsNullOrEmpty())
                    {
                        concept = new CodeableConcept();
                        coding = new Coding
                        {
                            Code = observation.ValueCode[i],
                            Display = observation.ValueDisplay[i],
                            System = observation.ValueSystem[i]
                        };

                        concept.Coding.Add(coding);
                        concept.Text = observation.ValueText[i];
                        resource.Component[i].Value = concept;
                    }
                    // value is of Type String
                    else if (!observation.ValueString.IsNullOrEmpty())
                    {
                        FhirString fhirString = new FhirString();
                        fhirString.Value = observation.ValueString[i];

                        resource.Component[i].Value = fhirString;
                    }
                    // value is of Type SampledData
                    else if (!observation.ValueSampledDataOriginValue.IsNullOrEmpty())
                    {
                        SimpleQuantity quantity = new SimpleQuantity
                        {
                            Code = observation.ValueSampledDataOriginCode[i],
                            System = observation.ValueSampledDataOriginSystem[i],
                            Unit = observation.ValueSampledDataOriginUnit[i],
                            Value = observation.ValueSampledDataOriginValue[i]
                        };

                        SampledData sampleData = new SampledData
                        {
                            Origin = quantity,
                            Data = observation.ValueSampledDataData[i],
                            Dimensions = observation.ValueSampledDataDimensions[i],
                            Period = observation.ValueSampledDataPeriod[i]
                        };

                        resource.Component[i].Value = sampleData;
                    }
                    // value is of Type Period 
                    else if (!observation.ValuePeriodStart.IsNullOrEmpty())
                    {
                        Period period = new Period
                        {
                            Start = observation.ValuePeriodStart[i].ToString(CultureInfo.InvariantCulture),
                            End = observation.ValuePeriodEnd[i].ToString(CultureInfo.InvariantCulture)
                        };

                        resource.Component[i].Value = period;
                    } 
                    // No value provided
                    else
                    {
                        resource.Component[i].Value = null;
                    }
                }
            }
            //There is only one "set" of values
            else
            {
                // value is of Type Quantity
                if (!observation.ValueQuantityValue.IsNullOrEmpty())
                {
                    Quantity quantity = new Quantity();
                    if (!observation.ValueQuantityCode.IsNullOrEmpty())
                        quantity.Code = observation.ValueQuantityCode[0];
                    if (!observation.ValueQuantitySystem.IsNullOrEmpty())
                        quantity.System = observation.ValueQuantitySystem[0];
                    if (!observation.ValueQuantityUnit.IsNullOrEmpty())
                        quantity.Unit = observation.ValueQuantityUnit[0];
                    quantity.Value = observation.ValueQuantityValue[0];

                    resource.Value = quantity;
                }
                // value is of Type CodeableConcept
                else if (!observation.ValueCode.IsNullOrEmpty())
                {
                    CodeableConcept concept = new CodeableConcept();
                    Coding coding = new Coding();
                    if (!observation.ValueQuantityCode.IsNullOrEmpty())
                        coding.Code = observation.ValueCode[0];
                    if (!observation.ValueQuantityCode.IsNullOrEmpty())
                        coding.Display = observation.ValueDisplay[0];
                    if (!observation.ValueQuantityCode.IsNullOrEmpty())
                        coding.System = observation.ValueSystem[0];
                    concept.Coding.Add(coding);
                    concept.Text = observation.ValueText[0];
                    resource.Value = concept;
                }
                // value is of Type String
                else if (!observation.ValueString.IsNullOrEmpty())
                {
                    FhirString fhirString = new FhirString();
                    fhirString.Value = observation.ValueString[0];

                    resource.Value = fhirString;
                }
                // value is of Type SampledData
                else if (!observation.ValueSampledDataOriginValue.IsNullOrEmpty())
                {
                    SimpleQuantity quantity = new SimpleQuantity();
                    if (!observation.ValueSampledDataOriginCode.IsNullOrEmpty())
                        quantity.Code = observation.ValueSampledDataOriginCode[0];
                    if (!observation.ValueSampledDataOriginSystem.IsNullOrEmpty())
                        quantity.System = observation.ValueSampledDataOriginSystem[0];
                    if (!observation.ValueSampledDataOriginUnit.IsNullOrEmpty())
                        quantity.Unit = observation.ValueSampledDataOriginUnit[0];
                    quantity.Value = observation.ValueSampledDataOriginValue[0];

                    SampledData sampleData = new SampledData();
                    sampleData.Origin = quantity;
                    if (!observation.ValueSampledDataData.IsNullOrEmpty())
                        sampleData.Data = observation.ValueSampledDataData[0];
                    if (!observation.ValueSampledDataDimensions.IsNullOrEmpty())
                        sampleData.Dimensions = observation.ValueSampledDataDimensions[0];
                    if (!observation.ValueSampledDataPeriod.IsNullOrEmpty())
                        sampleData.Period = observation.ValueSampledDataPeriod[0];

                    resource.Value = sampleData;
                }
                // value is of Type Period 
                else if (!observation.ValuePeriodStart .IsNullOrEmpty())
                {
                    Period period = new Period
                    {
                        Start = observation.ValuePeriodStart[0].ToString(CultureInfo.InvariantCulture),
                        End = observation.ValuePeriodEnd[0].ToString(CultureInfo.InvariantCulture)
                    };

                    resource.Value = period;
                }
                // No value provided.
                else
                {
                    resource.Value = null;
                }
            }

            return resource;
        }
    }
}