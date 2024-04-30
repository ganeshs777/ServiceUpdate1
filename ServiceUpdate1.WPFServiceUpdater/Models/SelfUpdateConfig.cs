using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ServiceUpdate1.WPFServiceUpdater.Models
{
    [Serializable()]
    [XmlRootAttribute("SelfUpdateConfig", Namespace = "", IsNullable = false)]
    public class SelfUpdateConfig
    {
        [XmlElement(ElementName = "VersionUrl")]
        public string VersionUrl { get; set; }
        [XmlElement(ElementName = "UpdateUrl")]
        public string UpdateUrl { get; set; }
        [XmlElement(ElementName = "ApplicationDirectory")]
        public string ApplicationDirectory { get; set; }
        [XmlElement(ElementName = "SkipVersionCheck")]
        public bool SkipVersionCheck { get; set; }

        public SelfUpdateConfig GetSelfUpdateConfig()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SelfUpdateConfig));
            StreamReader reader = new StreamReader("Files\\SelfUpdateConfig.xml");
            var selfUpdateConfig = (SelfUpdateConfig)serializer.Deserialize(reader);
            reader.Close();
            return selfUpdateConfig;
        }

    }
}
