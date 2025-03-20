using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Roottech.Tracking.Library.Models.Grid
{
    [DataContract]
    public class Filter
    {
        [DataMember]
        public string groupOp { get; set; }
        [DataMember]
        public Rule[] rules { get; set; }
        [DataMember]
        public Filter[] groups { get; set; }

        public static Filter Create(string jsonData)
        {
            try
            {
                var serializer = new DataContractJsonSerializer(typeof(Filter));
                var reader = new System.IO.StringReader(jsonData);
                var ms = new System.IO.MemoryStream(Encoding.Default.GetBytes(jsonData));
                return serializer.ReadObject(ms) as Filter;
            }
            catch
            {
                return null;
            }
        }
    }
}