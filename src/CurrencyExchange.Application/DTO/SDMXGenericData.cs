using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace CurrencyExchange.Application.DTO
{
    [XmlRoot("GenericData", Namespace = "http://www.sdmx.org/resources/sdmxml/schemas/v2_1/message")]
    [DataContract]
    public class SDMXGenericData
    {
        [DataMember]
        public SDMXDataSet DataSet { get; set; }
    }

    [DataContract(Name = "DataSet", Namespace = "http://www.sdmx.org/resources/sdmxml/schemas/v2_1/message")]
    public class SDMXDataSet
    {
        [DataMember]
        [XmlElement("Series", Namespace = "http://www.sdmx.org/resources/sdmxml/schemas/v2_1/data/generic")]
        public SDMXSeries[] Series { get; set; }
    }

    [DataContract(Name = "Series", Namespace = "http://www.sdmx.org/resources/sdmxml/schemas/v2_1/message")]
    public class SDMXSeries
    {
        [DataMember]
        [XmlArrayItem("Value")]
        public SDMXGenericValue[] SeriesKey { get; set; }

        [XmlElement("Obs")]
        public Obs[] Obs { get; set; }
    }

    [DataContract]
    public class SDMXSeriesKey
    {
        [DataMember]
        public SDMXGenericValue GenericValue { get; set; }
    }

    [DataContract]
    public class SDMXGenericValue
    {
        [DataMember]
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [DataMember]
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [DataContract]
    public class Obs
    {
        [DataMember]
        public ObsValue ObsValue { get; set; }

        [DataMember]
        public ObsDimension ObsDimension { get; set; }
    }

    [DataContract]
    public class ObsValue
    {
        [DataMember]
        [XmlAttribute(AttributeName = "value")]
        public decimal Value { get; set; }
    }

    [DataContract]
    public class ObsDimension
    {
        [DataMember]
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }
}