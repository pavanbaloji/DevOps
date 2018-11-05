using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avista.ESB.Utilities.Components
{
    [Serializable]
    public class KeyValueCollections
    {
        private Dictionary<string, string> dictionary = new Dictionary<string, string>();

        public void Add(string key, string value)
        {
            dictionary.Add(key, value);
        }

        public string Find(string key)
        {
            if(dictionary.ContainsKey(key))
            {
               return dictionary[key].ToString();
            }
            else
            {
                throw new Exception(key + " Not found.");
            }
        }
    }
}
