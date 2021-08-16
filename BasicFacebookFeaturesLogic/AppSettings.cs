using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;

namespace BasicFacebookFeatures
{
   public class AppSettings
    {
        private Point m_LastWindowLocation;
        private Size m_LastWindowSize;
        private bool m_RememberUser;
        private string m_LastAccessToken;

        private AppSettings()
        {
            m_LastWindowSize = new Size(890, 721);
            m_LastAccessToken = "";
            m_LastWindowLocation = new Point(260, 0);
            m_RememberUser = false;
        }

        public Point LastWindowLocation
        {
            get { return m_LastWindowLocation; }
            set { m_LastWindowLocation = value; }
        }

        public Size LastWindowSize
        {
            get { return m_LastWindowSize; }
            set { m_LastWindowSize = value; }
        }

        public bool RememberUser
        {
            get { return m_RememberUser; }
            set { m_RememberUser = value; }
        }

        public string LastAccessToken
        {
            get { return m_LastAccessToken; }
            set { m_LastAccessToken = value; }
        }

        public void SaveAppSettingsToFile()
        {
            string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            using (Stream stream = new FileStream(string.Format(@"{0}\AppSettings", directoryPath), FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(this.GetType());
                serializer.Serialize(stream, this);
            }
        }

        public static AppSettings LoadAppSettingsFromFile()
        {
            string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            AppSettings appSettingsObject = null;

            if (File.Exists(string.Format(@"{0}\AppSettings", directoryPath)))
            {
                using (Stream stream = new FileStream(string.Format(@"{0}\AppSettings", directoryPath), FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(AppSettings));
                    appSettingsObject = serializer.Deserialize(stream) as AppSettings;
                }
            }
            else
            {
                appSettingsObject = new AppSettings();
            }

            return appSettingsObject;
        }
    }
}
