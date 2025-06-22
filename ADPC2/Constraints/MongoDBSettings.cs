using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADPC2.Models.Constraints
{
    public class MongoDBSettings
    {
        public const string ConnectionString = "mongodb+srv://mpat:XAnrZCXv1JluHVUT@gene.bzaj78i.mongodb.net/?retryWrites=true&w=majority&appName=Gene";
        public const string DatabaseName = "GeneDB";
        public const string CollectionName_GeneExpressions = "GeneExp";
    }
}
