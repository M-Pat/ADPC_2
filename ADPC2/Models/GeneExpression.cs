using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADPC2.Models
{
    public class GeneExpression
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("patient_id")]
        public string PatientId { get; set; }

        [BsonElement("cancer_cohort")]
        public string CancerCohort { get; set; }

        [BsonElement("genes")]
        public Dictionary<string, double> GeneValues { get; set; } = new Dictionary<string, double>();

        [BsonElement("clinical_survival")]
        public ClinicalSurvival Clinical { get; set; }
    }
}
