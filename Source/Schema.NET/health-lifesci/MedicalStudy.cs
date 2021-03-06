namespace Schema.NET
{
    using System;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    /// <summary>
    /// A medical study is an umbrella type covering all kinds of research studies relating to human medicine or health, including observational studies and interventional trials and registries, randomized, controlled or not. When the specific type of study is known, use one of the extensions of this type, such as MedicalTrial or MedicalObservationalStudy. Also, note that this type should be used to mark up data that describes the study itself; to tag an article that publishes the results of a study, use MedicalScholarlyArticle. Note: use the code property of MedicalEntity to store study IDs, e.g. clinicaltrials.gov ID.
    /// </summary>
    [DataContract]
    public partial class MedicalStudy : MedicalEntity
    {
        /// <summary>
        /// Gets the name of the type as specified by schema.org.
        /// </summary>
        [DataMember(Name = "@type", Order = 1)]
        public override string Type => "MedicalStudy";

        /// <summary>
        /// Specifying the health condition(s) of a patient, medical study, or other target audience.
        /// </summary>
        [DataMember(Name = "healthCondition", Order = 206)]
        [JsonConverter(typeof(ValuesConverter))]
        public OneOrMany<MedicalCondition>? HealthCondition { get; set; }

        /// <summary>
        /// Expected or actual outcomes of the study.
        /// </summary>
        [DataMember(Name = "outcome", Order = 207)]
        [JsonConverter(typeof(ValuesConverter))]
        public Values<MedicalEntity, string>? Outcome { get; set; }

        /// <summary>
        /// Any characteristics of the population used in the study, e.g. 'males under 65'.
        /// </summary>
        [DataMember(Name = "population", Order = 208)]
        [JsonConverter(typeof(ValuesConverter))]
        public OneOrMany<string>? Population { get; set; }

        /// <summary>
        /// The status of the study (enumerated).
        /// </summary>
        [DataMember(Name = "status", Order = 209)]
        [JsonConverter(typeof(ValuesConverter))]
        public Values<EventStatusType?, MedicalStudyStatus?, string>? Status { get; set; }

        /// <summary>
        /// The location in which the study is taking/took place.
        /// </summary>
        [DataMember(Name = "studyLocation", Order = 210)]
        [JsonConverter(typeof(ValuesConverter))]
        public OneOrMany<AdministrativeArea>? StudyLocation { get; set; }

        /// <summary>
        /// A subject of the study, i.e. one of the medical conditions, therapies, devices, drugs, etc. investigated by the study.
        /// </summary>
        [DataMember(Name = "studySubject", Order = 211)]
        [JsonConverter(typeof(ValuesConverter))]
        public OneOrMany<MedicalEntity>? StudySubject { get; set; }
    }
}
