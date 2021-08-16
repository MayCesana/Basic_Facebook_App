using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using FacebookWrapper.ObjectModel;
using FacebookWrapper;

namespace BasicFacebookFeatures
{
    public class CarpoolFeatureSerializer
    {
        public readonly List<DictionaryItem> r_ListOfDictionaryItems = new List<DictionaryItem>();

        internal void SaveCarpoolsToFile(Dictionary<string, List<Carpool>> i_Dictionary, User i_LoggedInUser)
        {
            string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            foreach (string eventName in i_Dictionary.Keys)
            {
                DictionaryItem dicItem = new DictionaryItem();
                dicItem.Key = eventName;
                dicItem.Value = i_Dictionary[eventName];
                r_ListOfDictionaryItems.Add(dicItem);
            }

            using (Stream stream = new FileStream(string.Format(@"{0}\{1}", filePath, i_LoggedInUser.Id), FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(this.GetType());
                serializer.Serialize(stream, this);
            }
        }

        internal static Dictionary<string, List<Carpool>> LoadCarpoolsFromFile(User i_LoggedInUser)
        {
            string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Dictionary<string, List<Carpool>> dictionaryForCarpoolFeatureObject = new Dictionary<string, List<Carpool>>();
            CarpoolFeatureSerializer serializerObject = null;

            if (File.Exists(string.Format(@"{0}\{1}", filePath, i_LoggedInUser.Id)))
            {
                using (Stream stream = new FileStream(string.Format(@"{0}\{1}", filePath, i_LoggedInUser.Id), FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(CarpoolFeatureSerializer));
                    serializerObject = serializer.Deserialize(stream) as CarpoolFeatureSerializer;
                }

                foreach (DictionaryItem item in serializerObject.r_ListOfDictionaryItems)
                {
                    dictionaryForCarpoolFeatureObject[item.Key] = item.Value;
                }
            }
            else
            {
                serializerObject = new CarpoolFeatureSerializer();
            }

            return dictionaryForCarpoolFeatureObject;
        }
    }
}