using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matchmaker.Net.Network
{
    [Obsolete("Switched to JSON",true)]
    public class SerializeBinder : System.Runtime.Serialization.SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            if (assemblyName.Equals("NA"))
                return Type.GetType(typeName);
            else
                return BindToType(assemblyName, typeName);
        }

        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            // specify a neutral code for the assembly name to be recognized by the BindToType method.
            assemblyName = "";
            typeName = serializedType.FullName;
        }
    }
}
