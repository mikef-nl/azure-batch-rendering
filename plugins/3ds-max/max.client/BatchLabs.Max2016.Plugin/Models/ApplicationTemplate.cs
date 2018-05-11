
using System.Runtime.Serialization;

namespace BatchLabs.Max2016.Plugin.Models
{
    [DataContract]
    public class ApplicationTemplate
    {
        [DataMember(Name="id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }
    }
}
