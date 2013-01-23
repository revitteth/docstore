using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Ildss
{
    public class SecurityLogEvent : IEvent
    {
        public long InstanceId { get; set; }
        public string UserName { get; set; }
        public string DomainName { get; set; }
        public string ObjectType { get; set; }
        public string ObjectName { get; set; }
        public string ProcessName { get; set; }
        public string AccessMask { get; set; }
        public string TranslatedAccessMask { get; set; }
        public string ResourceAttributes { get; set; }
        public DateTime TimeGenerated { get; set; }

        public void PrintEvent()
        {
            Type type = this.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                Console.WriteLine("Name: " + property.Name + "," + " Value: " + property.GetValue(this, null));
            }
        }
    }
}
