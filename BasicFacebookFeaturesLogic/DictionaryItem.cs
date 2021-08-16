using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FacebookWrapper.ObjectModel;
using FacebookWrapper;

namespace BasicFacebookFeatures
{
    public class DictionaryItem
    {
        public string Key { get; set; }
        public List<Carpool> Value { get; set; }
    }
}
