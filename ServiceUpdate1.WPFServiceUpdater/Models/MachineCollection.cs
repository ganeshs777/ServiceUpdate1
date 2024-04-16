using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;

namespace ServiceUpdate1.WPFServiceUpdater.Models
{
    [Serializable()]
    [XmlRootAttribute("machines", Namespace = "", IsNullable = false)]
    public class MachineCollection
    {
        [XmlElement(ElementName = "machine")]
        public List<Machine> Machinelist { get; set; }
        public ObservableCollection<Machine> Machines { get; set; }
        public void LoadMachines()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MachineCollection));
            StreamReader reader = new StreamReader("C:\\repo\\ServiceUpdate1\\ServiceUpdate1.WPFServiceUpdater\\Files\\Machines.xml");
            var machines = (MachineCollection)serializer.Deserialize(reader);
            Machines = new ObservableCollection<Machine>(machines.Machinelist);
            reader.Close();
        }

    }
}
