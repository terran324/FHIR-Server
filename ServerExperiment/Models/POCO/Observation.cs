﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ServerExperiment.Models.FHIR.Helpers.Observation;
using ServerExperiment.Controllers.FhirControllers;

namespace ServerExperiment.Models.POCO
{
    public class Observation
    {
        public Observation()
        {
            VersionId = 0;
            IsDeleted = false;

            ObservationId = 0;

            _categoryCode = new List<string>();
            _categoryDisplay = new List<string>();
            _categorySystem = new List<string>();
            _codeCode = new List<string>();
            _codeDisplay = new List<string>();
            _codeSystem = new List<string>();
            _componentCodeCode = new List<string>();
            _componentCodeDisplay = new List<string>();
            _componentCodeSystem = new List<string>();
            _performerReferences = new List<string>();
            _valueCode = new List<string>();
            _valueDisplay = new List<string>();
            _valuePeriodEnd = new List<DateTime>();
            _valuePeriodStart = new List<DateTime>();
            _valueQuantityCode = new List<string>();
            _valueQuantitySystem = new List<string>();
            _valueQuantityUnit = new List<string>();
            _valueQuantityValue = new List<decimal>();
            _ValueSampledDataData = new List<string>();
            _valueSampledDataDimensions = new List<int>();
            _valueSampledDataOriginCode = new List<string>();
            _valueSampledDataOriginSystem = new List<string>();
            _ValueSampledDataOriginUnit = new List<string>();
            _valueSampledDataOriginValue = new List<decimal>();
            _valueSampledDataPeriod = new List<decimal>();
            _valueString = new List<string>();
            _valueSystem = new List<string>();
            _ValueText = new List<string>();

            EffectiveDateTime = DateTime.UtcNow;
            EffectivePeriodEnd = DateTime.UtcNow;
            EffectivePeriodStart = DateTime.UtcNow;
            Issued = DateTime.UtcNow;
        }

        // Each Record is immutable, in case of updates we create a new record and 
        // keep track of Version, Time of modification and action type like CREATE/UPDATE
        public int VersionId { get; set; }
        public bool IsDeleted { get; set; }

        // Logical Identifier
        [Key]
        public int ObservationId { get; set; }

        // Observation properties
        public ObsStatus Status { get; set; }

        // Observation Category 
        private List<string> _categorySystem;
        public List<string> CategorySystem
        {
            get { return _categorySystem; }
            set { _categorySystem = value; }
        }
        public string CategorySystemSerialised
        {
            get { return string.Join(";", _categorySystem); }
            set { _categorySystem = value.Split(';').ToList(); }
        }
        private List<string> _categoryCode;
        public List<string> CategoryCode
        {
            get { return _categoryCode; }
            set { _categoryCode = value; }
        }
        public string CategoryCodeSerialised
        {
            get { return String.Join(";", _categoryCode); }
            set { _categoryCode = value.Split(';').ToList(); }
        }
        private List<string> _categoryDisplay;
        public List<string> CategoryDisplay
        {
            get { return _categoryDisplay; }
            set { _categoryDisplay = value; }
        }
        public string CategoryDisplaySerialised
        {
            get { return String.Join(";", _categoryDisplay); }
            set { _categoryDisplay = value.Split(';').ToList(); }
        }
        public string CategoryText { get; set; }

        // Observation Code
        private List<string> _codeSystem;
        public List<string> CodeSystem
        {
            get { return _codeSystem; }
            set { _codeSystem = value; }
        }
        public string CodeSystemSerialised
        {
            get { return String.Join(";", _codeSystem); }
            set { _codeSystem = value.Split(';').ToList(); }
        }
        private List<string> _codeCode;
        public List<string> CodeCode
        {
            get { return _codeCode; }
            set { _codeCode = value; }
        }
        public string CodeCodeSerialised
        {
            get { return String.Join(";", _codeCode); }
            set { _codeCode = value.Split(';').ToList(); }
        }
        private List<string> _codeDisplay;
        public List<string> CodeDisplay
        {
            get { return _codeDisplay; }
            set { _codeDisplay = value; }
        }
        public string CodeDisplaySerialised
        {
            get { return String.Join(";", _codeDisplay); }
            set { _codeDisplay = value.Split(';').ToList(); }
        }
        public string CodeText { get; set; }

        // Observation references to other resources
        public string PatientReference { get; set; }
        public string DeviceReference { get; set; }
        private List<string> _performerReferences;
        public List<string> PerformerReferences
        {
            get { return _performerReferences; }
            set { _performerReferences = value; }
        }
        public string PerformerReferencesSerialised
        {
            get { return String.Join(";", _performerReferences); }
            set { _performerReferences = value.Split(';').ToList(); }
        }
        
        // Observation effective times
        public DateTime EffectiveDateTime { get; set; }
        public DateTime EffectivePeriodStart { get; set; }
        public DateTime EffectivePeriodEnd { get; set; }
        public DateTime Issued { get; set; }
        
        // Observation Interpretation
        public string InterpretationSystem { get; set; }
        public string InterpretationCode { get; set; }
        public string InterpretationDisplay { get; set; }
        public string InterpretationText { get; set; }

        // Observation comments
        public string Comments { get; set; }

        // Site of Body where Observation was made
        public string BodySiteSystem { get; set; }
        public string BodySiteCode { get; set; }
        public string BodySiteDisplay { get; set; }
        public string BodySiteText { get; set; }

        // Observation Values
        // This can apply for both single value (only one item in list) and Componentised values.
        private List<string> _componentCodeSystem;
        public List<string> ComponentCodeSystem
        {
            get { return _componentCodeSystem; }
            set { _componentCodeSystem = value; }
        }
        public string ComponentCodeSystemSerialised
        {
            get { return String.Join(";", _componentCodeSystem); }
            set { _componentCodeSystem = value.Split(';').ToList(); }
        }

        private List<string> _componentCodeCode;
        public List<string> ComponentCodeCode
        {
            get { return _componentCodeCode; }
            set { _componentCodeCode = value; }
        }
        public string ComponentCodeCodeSerialised
        {
            get { return String.Join(";", _componentCodeCode); }
            set { _componentCodeCode = value.Split(';').ToList(); }
        }

        private List<string> _componentCodeDisplay;
        public List<string> ComponentCodeDisplay
        {
            get { return _componentCodeDisplay; }
            set { _componentCodeDisplay = value; }
        }
        public string ComponentCodeDisplaySerialised
        {
            get { return String.Join(";", _componentCodeDisplay); }
            set { _componentCodeDisplay = value.Split(';').ToList(); }
        }
        public string ComponentCodeText { get; set; }

        private List<string> _valueQuantitySystem;
        public List<string> ValueQuantitySystem
        {
            get { return _valueQuantitySystem; }
            set { _valueQuantitySystem = value; }
        }
        public string ValueQuantitySystemSerialised
        {
            get { return String.Join(";", _valueQuantitySystem); }
            set { _valueQuantitySystem = value.Split(';').ToList(); }
        }

        private List<string> _valueQuantityCode;
        public List<string> ValueQuantityCode
        {
            get { return _valueQuantityCode; }
            set { _valueQuantityCode = value; }
        }
        public string ValueQuantityCodeSerialised
        {
            get { return String.Join(";", _valueQuantityCode); }
            set { _valueQuantityCode = value.Split(';').ToList(); }
        }

        private List<string> _valueQuantityUnit;
        public List<string> ValueQuantityUnit
        {
            get { return _valueQuantityUnit; }
            set { _valueQuantityUnit = value; }
        }
        public string ValueQuantityUnitSerialised
        {
            get { return String.Join(";", _valueQuantityUnit); }
            set { _valueQuantityUnit = value.Split(';').ToList(); }
        }

        private List<decimal> _valueQuantityValue;
        public List<decimal> ValueQuantityValue
        {
            get { return _valueQuantityValue; }
            set { _valueQuantityValue = value; }
        }
        public string ValueQuantityValueSerialised
        {
            get
            {
                if (_valueQuantityValue == null)
                    return null;
                else
                    return String.Join(";", _valueQuantityValue);
            }
            set
            {
                List<string> quantityValues = value.Split(';').ToList();
                if (quantityValues[0] == string.Empty)
                {
                    _valueQuantityValue = null;
                }
                else
                {
                    _valueQuantityValue = quantityValues.Select(decimal.Parse).ToList();
                }
                //_valueQuantityValue = value.Split(';').ToList().Select(decimal.Parse).ToList();
            }
        }

        private List<string> _valueSystem;
        public List<string> ValueSystem
        {
            get { return _valueSystem; }
            set { _valueSystem = value; }
        }
        public string ValueSystemSerialised
        {
            get { return String.Join(";", _valueSystem); }
            set { _valueSystem = value.Split(';').ToList(); }
        }

        private List<string> _valueCode;
        public List<string> ValueCode
        {
            get { return _valueCode; }
            set { _valueCode = value; }
        }
        public string ValueCodeSerialised
        {
            get { return String.Join(";", _valueCode); }
            set { _valueCode = value.Split(';').ToList(); }
        }

        private List<string> _valueDisplay;
        public List<string> ValueDisplay
        {
            get { return _valueDisplay; }
            set { _valueDisplay = value; }
        }
        public string ValueDisplaySerialised
        {
            get { return String.Join(";", _valueDisplay); }
            set { _valueDisplay = value.Split(';').ToList(); }
        }

        private List<string> _ValueText;
        public List<string> ValueText
        {
            get { return _ValueText; }
            set { _ValueText = value; }
        }
        public string ValueTextSerialised
        {
            get { return String.Join(";", _ValueText); }
            set { _ValueText = value.Split(';').ToList(); }
        }

        private List<string> _valueString;
        public List<string> ValueString
        {
            get { return _valueString; }
            set { _valueString = value; }
        }
        public string ValueStringSerialised
        {
            get { return String.Join(";", _valueString); }
            set { _valueString = value.Split(';').ToList(); }
        }

        private List<string> _valueSampledDataOriginSystem;
        public List<string> ValueSampledDataOriginSystem
        {
            get { return _valueSampledDataOriginSystem; }
            set { _valueSampledDataOriginSystem = value; }
        }
        public string ValueSampledDataOriginSystemSerialised
        {
            get { return String.Join(";", _valueSampledDataOriginSystem); }
            set { _valueSampledDataOriginSystem = value.Split(';').ToList(); }
        }

        private List<string> _valueSampledDataOriginCode;
        public List<string> ValueSampledDataOriginCode
        {
            get { return _valueSampledDataOriginCode; }
            set { _valueSampledDataOriginCode = value; }
        }
        public string ValueSampledDataOriginCodeSerialised
        {
            get { return String.Join(";", _valueSampledDataOriginCode); }
            set { _valueSampledDataOriginCode = value.Split(';').ToList(); }
        }

        private List<string> _ValueSampledDataOriginUnit;
        public List<string> ValueSampledDataOriginUnit
        {
            get { return _ValueSampledDataOriginUnit; }
            set { _ValueSampledDataOriginUnit = value; }
        }
        public string ValueSampledDataOriginUnitSerialised
        {
            get { return String.Join(";", _ValueSampledDataOriginUnit); }
            set { _ValueSampledDataOriginUnit = value.Split(';').ToList(); }
        }

        private List<decimal> _valueSampledDataOriginValue;
        public List<decimal> ValueSampledDataOriginValue
        {
            get { return _valueSampledDataOriginValue; }
            set { _valueSampledDataOriginValue = value; }
        }
        public string ValueSampledDataOriginValueSerialised
        {
            get
            {
                if (_valueSampledDataOriginValue == null)
                    return null;
                else
                    return String.Join(";", _valueSampledDataOriginValue);
            }
            set
            {
                List<string> sampledDataValues = value.Split(';').ToList();
                if (sampledDataValues[0] == string.Empty)
                {
                    _valueSampledDataOriginValue = null;
                }
                else
                {
                    _valueSampledDataOriginValue = sampledDataValues.Select(decimal.Parse).ToList();
                }

                //_ValueSampledDataOriginValue = value.Split(';').ToList().Select(decimal.Parse).ToList(); 
            }
        }

        private List<decimal> _valueSampledDataPeriod;
        public List<decimal> ValueSampledDataPeriod
        {
            get { return _valueSampledDataPeriod; }
            set { _valueSampledDataPeriod = value; }
        }
        public string ValueSampledDataPeriodSerialised
        {
            get
            {
                if (_valueSampledDataPeriod == null)
                    return null;
                else
                    return String.Join(";", _valueSampledDataPeriod);
            }
            set
            {
                List<string> sampledDataPeriods = value.Split(';').ToList();
                if (sampledDataPeriods[0] == string.Empty)
                {
                    _valueSampledDataPeriod = null;
                }
                else
                {
                    _valueSampledDataPeriod = sampledDataPeriods.Select(decimal.Parse).ToList();
                }

                //_valueSampledDataPeriod = value.Split(';').ToList().Select(decimal.Parse).ToList();
            }
        }

        private List<int> _valueSampledDataDimensions;
        public List<int> ValueSampledDataDimensions
        {
            get { return _valueSampledDataDimensions; }
            set { _valueSampledDataDimensions = value; }
        }
        public string ValueSampledDataDimensionsSerialised
        {
            get
            {
                if (_valueSampledDataDimensions == null)
                    return null;
                else
                    return String.Join(";", _valueSampledDataDimensions);
            }
            set
            {
                List<string> sampledDataDimensions = value.Split(';').ToList();
                if (sampledDataDimensions[0] == string.Empty)
                {
                    _valueSampledDataDimensions = null;
                }
                else
                {
                    _valueSampledDataDimensions = sampledDataDimensions.Select(int.Parse).ToList();
                }

                //_valueSampledDataDimensions = value.Split(';').ToList().Select(int.Parse).ToList();
            }
        }

        private List<string> _ValueSampledDataData;
        public List<string> ValueSampledDataData
        {
            get { return _ValueSampledDataData; }
            set { _ValueSampledDataData = value; }
        }
        public string ValueSampledDataDataSerialised
        {
            get { return String.Join(";", _ValueSampledDataData); }
            set { _ValueSampledDataData = value.Split(';').ToList(); }
        }

        private List<DateTime> _valuePeriodStart;
        public List<DateTime> ValuePeriodStart
        {
            get { return _valuePeriodStart; }
            set { _valuePeriodStart = value; }
        }
        public string ValuePeriodStartSerialised
        {
            get
            {
                if (_valuePeriodStart == null)
                    return null;
                else
                    return String.Join(";", _valuePeriodStart);
            }
            set
            {
                List<string> valuePeriodStarts = value.Split(';').ToList();
                if (valuePeriodStarts[0] == string.Empty)
                {
                    _valuePeriodStart = null;
                }
                else
                {
                    _valuePeriodStart = valuePeriodStarts.Select(DateTime.Parse).ToList();
                }

                //_valuePeriodStart = value.Split(';').ToList().Select(DateTime.Parse).ToList();
            }
        }

        private List<DateTime> _valuePeriodEnd;
        public List<DateTime> ValuePeriodEnd
        {
            get { return _valuePeriodEnd; }
            set { _valuePeriodEnd = value; }
        }
        public string ValuePeriodEndSerialised
        {
            get
            {
                if (_valuePeriodEnd == null)
                    return null;
                else
                    return String.Join(";", _valuePeriodEnd);
            }
            set
            {
                List<string> valuePeriodEnds = value.Split(';').ToList();
                if (valuePeriodEnds[0] == string.Empty)
                {
                    _valuePeriodEnd = null;
                }
                else
                {
                    _valuePeriodEnd = valuePeriodEnds.Select(DateTime.Parse).ToList();
                }

                //_valuePeriodEnd = value.Split(';').ToList().Select(DateTime.Parse).ToList();
            }
        }
    }
}