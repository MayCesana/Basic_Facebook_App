using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FacebookWrapper.ObjectModel;
using FacebookWrapper;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace BasicFacebookFeatures
{
    public class CarpoolFeature
    {
        private User m_LoggedInUser;
        private readonly Dictionary<string, List<Carpool>> r_AllCarpoolToEvents;

        private CarpoolFeature(Dictionary<string, List<Carpool>> i_Dic, User i_LoggedInUser)
        {
            if(i_Dic == null || i_Dic.Count == 0)
            {
                r_AllCarpoolToEvents = new Dictionary<string, List<Carpool>>();
                foreach (Event userEvent in i_LoggedInUser.Events)
                {
                    this.r_AllCarpoolToEvents.Add(userEvent.Name, new List<Carpool>());
                }
            }
            else
            {
                r_AllCarpoolToEvents = i_Dic;
            }
            m_LoggedInUser = i_LoggedInUser;
        }

        public Dictionary<string, List<Carpool>> AllCarpoolToEvents
        {
            get { return r_AllCarpoolToEvents; }
        }

        public Carpool FindCarpoolByDriverName(string i_EventName, string i_DriverName)
        {
            Carpool foundCarpool = null;

            foreach(Carpool carpool in r_AllCarpoolToEvents[i_EventName])
            {
                if(i_DriverName == carpool.DriverName)
                {
                    foundCarpool = carpool;
                }
            }

            return foundCarpool;
        }

        public void SaveCarpoolsToFile()
        {
            CarpoolFeatureSerializer serializer = new CarpoolFeatureSerializer();
            serializer.SaveCarpoolsToFile(r_AllCarpoolToEvents, m_LoggedInUser);
        }
         
        public static CarpoolFeature LoadCarpoolsFromFile(User i_LoggedInUser)
        {
            Dictionary<string, List<Carpool>> dic = CarpoolFeatureSerializer.LoadCarpoolsFromFile(i_LoggedInUser);
            return (new CarpoolFeature(dic, i_LoggedInUser));
        }
    }
}
