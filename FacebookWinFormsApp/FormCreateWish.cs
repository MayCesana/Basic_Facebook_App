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
    public partial class FormCreateWish : Form
    {
        private BirthdayWish m_BirthdayWish = new BirthdayWish();
        private User m_FriendToSendWish;

        public FormCreateWish(User i_FriendToSendWish)
        {
            InitializeComponent();
            m_FriendToSendWish = i_FriendToSendWish;
            buttonPostWish.Text = $"Post your birthday wish on {m_FriendToSendWish} wall";
        }

        private void FormCreateWish_Load(object sender, EventArgs e)
        {

        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            buttonAddCard.Visible = true;
            panelAllCards.Visible = true;
            textBoxWriteWish.Visible = true;
            buttonWishTemplate.Visible = true;

            buttonBack.Visible = false;
            listBoxTemplates.Visible = false;
        }

        private void textBoxWriteWIsh_Click(object sender, EventArgs e)
        {
            if (textBoxWriteWish.Text == "Write your wish here...")
            {
                textBoxWriteWish.Clear();
            }
        }

        private void buttonAddCard_Click(object sender, EventArgs e)
        {
            panelAllCards.Visible = true;
        }

        private void buttonWishTemplate_Click(object sender, EventArgs e)
        {
            buttonAddCard.Visible = false;
            panelAllCards.Visible = false;
            textBoxWriteWish.Visible = false;
            buttonWishTemplate.Visible = false;
            buttonPostWish.Visible = false;
            labelOr.Visible = false;

            buttonBack.Visible = true;
            listBoxTemplates.Visible = true;
            listBoxTemplates.Items.Add("Happy Birthday!");
            listBoxTemplates.Items.Add("Happy birthday. I pray all your birthday wishes to come true.");
            listBoxTemplates.Items.Add("May all the joy you have spread around come back to you a hundredfold. Happy birthday.");
            listBoxTemplates.Items.Add("It's your birthday!! I wish that whatever you want in life comes to you just the way you imagined it or even better.");
            listBoxTemplates.Items.Add("I wish you the very best of life and all it has to offer.");
            listBoxTemplates.Items.Add("Sending you a birthday wish all wrapped up in love. Happy Birthday.");
            listBoxTemplates.Items.Add("I am blessed to have you in my life. Happy birthday, dear friend.");
            listBoxTemplates.Items.Add("Sending you best wishes for success, health, and good fortune today and in the year to come. Enjoy your special day. Happy Birthday!");
        }

        private void buttonPostWish_Click(object sender, EventArgs e)
        {
            m_BirthdayWish.WishText = textBoxWriteWish.Text;
            try
            {
                m_BirthdayWish.PostBirthdayWish(m_FriendToSendWish);
                MessageBox.Show("Birthday wish Posted!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            this.Close(); 
        }

        private void panelCard1_Click(object sender, EventArgs e)
        {
            m_BirthdayWish.WishImageName = panelCard1.Tag.ToString();
            markSelectedBorderPanel(sender as Panel);
        }

        private void panelCard2_Click(object sender, EventArgs e)
        {
            m_BirthdayWish.WishImageName = panelCard2.Tag.ToString();
            markSelectedBorderPanel(sender as Panel);
        }

        private void panelCard3_Click(object sender, EventArgs e)
        {
            m_BirthdayWish.WishImageName = panelCard3.Tag.ToString();
            markSelectedBorderPanel(sender as Panel);
        }

        private void panelCard4_Click(object sender, EventArgs e)
        {
            m_BirthdayWish.WishImageName = panelCard4.Tag.ToString();
            markSelectedBorderPanel(sender as Panel);
        }

        private void panelCard5_Click(object sender, EventArgs e)
        {
            m_BirthdayWish.WishImageName = panelCard5.Tag.ToString();
            markSelectedBorderPanel(sender as Panel);
        }

        private void panelCard6_Click(object sender, EventArgs e)
        {
            m_BirthdayWish.WishImageName = panelCard6.Tag.ToString();
            markSelectedBorderPanel(sender as Panel);
        }

        private void panelCard7_Click(object sender, EventArgs e)
        {
            m_BirthdayWish.WishImageName = panelCard7.Tag.ToString();
            markSelectedBorderPanel(sender as Panel);
        }

        private void panelCard8_Click(object sender, EventArgs e)
        {
            m_BirthdayWish.WishImageName = panelCard8.Tag.ToString();
            markSelectedBorderPanel(sender as Panel);
            markSelectedBorderPanel(sender as Panel);
        }

        private void markSelectedBorderPanel(Panel i_selectedPanel)
        {
            foreach (Panel panel in panelAllCards.Controls)
            {
                panel.BorderStyle = BorderStyle.None;
            }

            i_selectedPanel.BorderStyle = BorderStyle.Fixed3D;
        }

        private void listBoxTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listBoxTemplates.SelectedItems.Count == 1)
            {
                MessageBox.Show("Template selected!");
                textBoxWriteWish.Text = listBoxTemplates.SelectedItem.ToString();
            }
        }
    }
}
