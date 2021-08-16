using System;
using System.Collections.Generic;

namespace BasicFacebookFeatures
{
    public class Carpool
    {
        private string m_DriverName;
        private string m_DriverPhoneNumber;
        private string m_StartingPointAddress;
        private string m_StartingPointCity;
        private string m_LeavingTime;
        private List<string> m_RidePassengers = new List<string>();
        private int m_MaxCarPassengers;

        public void SetCarpoolDetails(string i_DriverName, string i_DriverPhoneNumber, string i_SourceCity, string i_SourceAddress, string i_LeavingTime, int i_MaxCarPassengers)
        {
            this.m_DriverName = i_DriverName;
            this.m_DriverPhoneNumber = i_DriverPhoneNumber;
            this.m_StartingPointAddress = i_SourceAddress;
            this.m_StartingPointCity = i_SourceCity;
            this.m_MaxCarPassengers = i_MaxCarPassengers;
            this.m_LeavingTime = i_LeavingTime;
        }

        public string DriverName
        {
            get { return m_DriverName; }
            set { m_DriverName = value; }
        }

        public string DriverPhoneNumber
        {
            get { return m_DriverPhoneNumber; }
            set { m_DriverPhoneNumber = value; }
        }

        public string StartingPointAddress
        {
            get { return m_StartingPointAddress; }
            set { m_StartingPointAddress = value; }
        }

        public string StartingPointCity
        {
            get { return m_StartingPointCity; }
            set { m_StartingPointCity = value; }
        }

        public int MaxCarPassengers
        {
            get { return m_MaxCarPassengers; }
            set { m_MaxCarPassengers = value; }
        }

        public List<string> RidePassengers
        {
            get { return m_RidePassengers; }
            set { m_RidePassengers = value; }
        }

        public int VacantSeats
        {
            get { return (m_MaxCarPassengers - m_RidePassengers.Count); }
        }

        public string LeavingTime
        {
            get { return m_LeavingTime; }
            set { m_LeavingTime = value; }
        }

        public void AddPassanger(string i_PassangerName)
        {
            if(m_RidePassengers.Count < m_MaxCarPassengers)
            {
                m_RidePassengers.Add(i_PassangerName);
            }
            else
            {
                throw new Exception("There are no vacant seats!");
            }
        }
    }
}
