using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FacebookWrapper.ObjectModel;
using FacebookWrapper;

namespace BasicFacebookFeatures
{
    public partial class FormCreateCarpool : Form
    {
        public delegate void newCarpoolCreated(Carpool i_NewCarpool);

        private readonly User r_Driver;
        public event newCarpoolCreated m_NewCarpoolCreatedDelegate;

        public FormCreateCarpool(User i_Driver)
        {
            InitializeComponent();
            r_Driver = i_Driver;
        }

        private void buttonSaveCarpool_Click(object sender, EventArgs e)
        {
            if(isValidDetails())
            {
                if (this.DialogResult == DialogResult.OK)
                {
                    Carpool newCarpool = new Carpool();
                    newCarpool.SetCarpoolDetails(textBoxCarpoolDriverName.Text, textBoxCarpoolDriverPhone.Text,
                        textBoxCarpoolSourceCity.Text, textBoxCarpoolAddress.Text,
                        textBoxCarpoolLeavingTime.Text, (int)numericUpDownMaxSeats.Value);
                    MessageBox.Show("Carpool created");
                    this.Close();
                    buttonSaveCarpool_OnClick(newCarpool);
                }
            }
        }
        
        private bool isValidDetails()
        {
            bool isValid = false;

            if(string.IsNullOrEmpty(textBoxCarpoolDriverName.Text))
            {
                MessageBox.Show("You have to enter driver's name!");
            }
            else if(string.IsNullOrEmpty(textBoxCarpoolDriverPhone.Text) || !textBoxCarpoolDriverPhone.Text.All(char.IsDigit))
            {
                MessageBox.Show("Phone number is invalid!");
            }
            else if(numericUpDownMaxSeats.Value == 0)
            {
                MessageBox.Show("You can't create a carpool with no vacant seats!");
            }
            else if(string.IsNullOrEmpty(textBoxCarpoolSourceCity.Text) || string.IsNullOrEmpty(textBoxCarpoolAddress.Text))
            {
                MessageBox.Show("You have to enter source address!");
            }
            else if(string.IsNullOrEmpty(textBoxCarpoolLeavingTime.Text))
            {
                MessageBox.Show("You have to enter leaving time!");
            }
            else
            {
                isValid = true;
            }

            return isValid;

        }

        protected virtual void buttonSaveCarpool_OnClick(Carpool i_NewCarpool)
        {
            if(m_NewCarpoolCreatedDelegate != null)
            {
                m_NewCarpoolCreatedDelegate.Invoke(i_NewCarpool);
            }
        }

        private void textBoxCarpoolLeavingTime_Click(object sender, EventArgs e)
        {
            if(textBoxCarpoolLeavingTime.Text == "[hh:mm]")
            {
                textBoxCarpoolLeavingTime.Clear();
            }
        }
    }
}
