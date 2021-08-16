using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FacebookWrapper.ObjectModel;
using FacebookWrapper;

namespace BasicFacebookFeatures
{
    public partial class FormMain : Form
    {
        public enum eListType { Friends = 1, Pages, Groups, Events };

        private User m_LoggedInUser;
        private LoginResult m_LoginResult;
        private int m_AlbumIndex = 0;
        private int m_AlbumNameIndex = 0;
        private eListType m_ListTypeToShow;
        private AppSettings m_AppSettings;
        private CarpoolFeature m_CarpoolsToMyEvents = null;

        public FormMain()
        {
            InitializeComponent();
            initializeNextAlbumsButton();

            m_AppSettings = AppSettings.LoadAppSettingsFromFile();

            this.StartPosition = FormStartPosition.Manual;
            this.Location = m_AppSettings.LastWindowLocation;
            this.Size = m_AppSettings.LastWindowSize;
            this.checkBoxRememberMe.Checked = m_AppSettings.RememberUser;
        }

        private void initializeNextAlbumsButton()
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(0, 0, buttonNextAlbums.Width - 1, buttonNextAlbums.Height - 1);
            buttonNextAlbums.Region = new Region(path);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (m_AppSettings.RememberUser && !string.IsNullOrEmpty(m_AppSettings.LastAccessToken))
            {
                m_LoginResult = FacebookService.Connect(m_AppSettings.LastAccessToken);
                populateUIFromFacebookData();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            m_AppSettings.LastWindowSize = this.Size;
            m_AppSettings.LastWindowLocation = this.Location;
            m_AppSettings.RememberUser = this.checkBoxRememberMe.Checked;

            if (m_AppSettings.RememberUser)
            {
                m_AppSettings.LastAccessToken = m_LoginResult.AccessToken;
            }
            else
            {
                m_AppSettings.LastAccessToken = null;
            }

            m_AppSettings.SaveAppSettingsToFile();
            if(m_CarpoolsToMyEvents !=null && m_CarpoolsToMyEvents.AllCarpoolToEvents.Count != 0)
            {
                m_CarpoolsToMyEvents.SaveCarpoolsToFile();
            }
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            Clipboard.SetText("design.patterns21cc"); /// the current password for Desig Patter

            m_LoginResult = FacebookService.Login(
                    "575245793470583",
                    "email",
                    "public_profile",
                    "user_age_range",
                    "user_birthday",
                    "user_events",
                    "user_friends",
                    "user_gender",
                    "user_hometown",
                    "user_likes",
                    "user_link",
                    "user_location",
                    "user_photos",
                    "user_posts",
                    "user_videos");

            if (!string.IsNullOrEmpty(m_LoginResult.AccessToken))
            {
                populateUIFromFacebookData();
            }
            else
            {
                MessageBox.Show(m_LoginResult.ErrorMessage, "Login Failed");
            }
        }

        private void populateUIFromFacebookData()
        {
            m_LoggedInUser = m_LoginResult.LoggedInUser;
            buttonLogin.Text = $"Logged in as {m_LoginResult.LoggedInUser.Name}";
            pictureBoxProfilePicture.LoadAsync(m_LoggedInUser.PictureLargeURL);
            fetchUserAbout();
            fetchAlbums();
            fetchPosts();
        }

        private void fetchUserAbout()
        {
            if (m_LoggedInUser.Email != null)
            {
                labelEmail.Text = m_LoggedInUser.Email;
                labelEmail.Visible = true;
            }

            if (m_LoggedInUser.Gender != null)
            {
                labelGender.Text = m_LoggedInUser.Gender == User.eGender.male ? "Male" : "Female";
                labelGender.Visible = true;
            }

            if (m_LoggedInUser.Hometown != null)
            {
                labelHomeTown.Text = m_LoggedInUser.Hometown.Name;
                labelHomeTown.Visible = true;
            }

            if (m_LoggedInUser.Location != null)
            {
                labelResidence.Text = m_LoggedInUser.Location.Name;
                labelResidence.Visible = true;
            }
        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {
            FacebookService.LogoutWithUI();
            buttonLogin.Text = "Login";
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

        }

        private void fetchPosts()
        {
            listBoxPosts.Items.Clear();

            foreach (Post post in m_LoggedInUser.Posts)
            {
                if (post.Message != null)
                {
                    listBoxPosts.Items.Add(post.Message);
                }
                else if (post.Caption != null)
                {
                    listBoxPosts.Items.Add(post.Caption);
                }
                else
                {
                    listBoxPosts.Items.Add(string.Format("[{0}]", post.Type));
                }
            }
        }

        private void fetchAlbums()
        {
            foreach (Control album in groupBoxAlbums.Controls)
            {
                if (album.GetType() == typeof(PictureBox))
                {
                    if (m_AlbumIndex < m_LoggedInUser.Albums.Count)
                    {
                        (album as PictureBox).LoadAsync(m_LoggedInUser.Albums[m_AlbumIndex].PictureAlbumURL);
                        (album as PictureBox).SizeMode = PictureBoxSizeMode.CenterImage;
                        m_AlbumIndex++;
                    }
                    else
                    {
                        (album as PictureBox).Image = null;
                    }
                }
                else if (album.GetType() == typeof(Label))
                {
                    if (m_AlbumNameIndex < m_LoggedInUser.Albums.Count)
                    {
                        (album as Label).Text = m_LoggedInUser.Albums[m_AlbumNameIndex].Name;
                        m_AlbumNameIndex++;
                    }
                    else
                    {
                        (album as Label).Text = "";
                    }
                }
            }
        }

        private void listBoxPosts_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Shows the comments for the selected post
            Post selected = m_LoggedInUser.Posts[listBoxPosts.SelectedIndex];
            listBoxComments.DisplayMember = "Message";
            listBoxComments.DataSource = selected.Comments;
        }

        private void buttonGroups_Click(object sender, EventArgs e)
        {
            if (m_LoggedInUser == null)
            {
                showErrorMessageWhenNoLoggedInUser();
            }
            else
            {
                listBoxGeneral.Items.Clear();
                listBoxGeneral.DisplayMember = "Name";

                try
                {
                    foreach (Group group in m_LoggedInUser.Groups)
                    {
                        listBoxGeneral.Items.Add(group);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                if (listBoxGeneral.Items.Count == 0)
                {
                    MessageBox.Show("No groups to retrieve :(");
                }
                else
                {
                    m_ListTypeToShow = eListType.Groups;
                }
            }
        }

        private void buttonEvents_Click(object sender, EventArgs e)
        {
            if (m_LoggedInUser == null)
            {
                showErrorMessageWhenNoLoggedInUser();
            }
            else
            {
                listBoxGeneral.Items.Clear();
                listBoxGeneral.DisplayMember = "Name";

                try
                {
                    foreach (Event facebookEvent in m_LoggedInUser.Events)
                    {
                        listBoxGeneral.Items.Add(facebookEvent);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                if (listBoxGeneral.Items.Count == 0)
                {
                    MessageBox.Show("No events to retrieve :(");
                }
                else
                {
                    m_ListTypeToShow = eListType.Events;
                }
            }
        }

        private void showErrorMessageWhenNoLoggedInUser()
        {
            MessageBox.Show("You need to log-in first!");
        }

        private void buttonFriends_Click(object sender, EventArgs e)
        {
            if (m_LoggedInUser == null)
            {
                showErrorMessageWhenNoLoggedInUser();
            }
            else
            {
                listBoxGeneral.Items.Clear();
                listBoxGeneral.DisplayMember = "Name";

                try
                {
                    foreach (User friend in m_LoggedInUser.Friends)
                    {
                        listBoxGeneral.Items.Add(friend);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                if (listBoxGeneral.Items.Count == 0)
                {
                    MessageBox.Show("No friends to show :(");
                }
                else
                {
                    m_ListTypeToShow = eListType.Friends;
                }
            }
        }

        private void buttonPages_Click(object sender, EventArgs e)
        {
            if (m_LoggedInUser == null)
            {
                showErrorMessageWhenNoLoggedInUser();
            }
            else
            {
                listBoxGeneral.Items.Clear();
                listBoxGeneral.DisplayMember = "Name";

                try
                {
                    foreach (Page page in m_LoggedInUser.LikedPages)
                    {
                        listBoxGeneral.Items.Add(page);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                if (listBoxGeneral.Items.Count == 0)
                {
                    MessageBox.Show("No liked pages to retrieve :(");
                }
                else
                {
                    m_ListTypeToShow = eListType.Pages;
                }
            }
        }

        private void buttonFriends_MouseHover(object sender, EventArgs e)
        {
            buttonFriends.Width += 5;
            buttonFriends.Height += 5;
        }

        private void buttonFriends_MouseLeave(object sender, EventArgs e)
        {
            buttonFriends.Width -= 5;
            buttonFriends.Height -= 5;
        }

        private void buttonPages_MouseHover(object sender, EventArgs e)
        {
            buttonPages.Width += 5;
            buttonPages.Height += 5;
        }

        private void buttonPages_MouseLeave(object sender, EventArgs e)
        {
            buttonPages.Width -= 5;
            buttonPages.Height -= 5;
        }

        private void buttonGroups_MouseHover(object sender, EventArgs e)
        {
            buttonGroups.Width += 5;
            buttonGroups.Height += 5;
        }

        private void buttonGroups_MouseLeave(object sender, EventArgs e)
        {
            buttonGroups.Width -= 5;
            buttonGroups.Height -= 5;
        }

        private void buttonEvents_MouseHover(object sender, EventArgs e)
        {
            buttonEvents.Width += 5;
            buttonEvents.Height += 5;
        }

        private void buttonEvents_MouseLeave(object sender, EventArgs e)
        {
            buttonEvents.Width -= 5;
            buttonEvents.Height -= 5;
        }

        private void listBoxGeneral_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (m_ListTypeToShow)
            {
                case eListType.Friends:
                    if (listBoxGeneral.SelectedItems.Count == 1)
                    {
                        pictureBoxObjectFromList.LoadAsync((listBoxGeneral.SelectedItem as User).PictureNormalURL);
                    }
                    break;
                case eListType.Pages:
                    if (listBoxGeneral.SelectedItems.Count == 1)
                    {
                        pictureBoxObjectFromList.LoadAsync((listBoxGeneral.SelectedItem as Page).PictureNormalURL);
                    }
                    break;
                case eListType.Groups:
                    if (listBoxGeneral.SelectedItems.Count == 1)
                    {
                        pictureBoxObjectFromList.LoadAsync((listBoxGeneral.SelectedItem as Group).PictureNormalURL);
                    }
                    break;
                case eListType.Events:
                    if (listBoxGeneral.SelectedItems.Count == 1)
                    {
                        pictureBoxObjectFromList.LoadAsync((listBoxGeneral.SelectedItem as Event).PictureNormalURL);
                    }
                    break;
                default: break;
            }
        }

        private void buttonPostStatus_Click(object sender, EventArgs e)
        {
            try
            {
                Status postedStatus = m_LoggedInUser.PostStatus(textBoxStatus.Text);
                MessageBox.Show("Status Posted!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                textBoxStatus.Clear();
            }
        }

        private void buttonNextAlbums_Click(object sender, EventArgs e)
        {
            fetchAlbums();
        }

        /*-----------------Birthday feature methods---------------------*/
        private void listBoxFriendsCelebratingBdayToday_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxFriendsCelebratingBdayToday.SelectedItems.Count == 1)
            {
                buttonOpenCreateWishForm.Text = $"Create your birthday card for {(listBoxFriendsCelebratingBdayToday.SelectedItem as User).Name}";
                buttonOpenCreateWishForm.Visible = true;
            }
        }

        private void buttonTodaysBirthday_Click(object sender, EventArgs e)
        {
            List<User> friendsWhoCelebratingBirthdayToday = BirthdayWish.CheckWhoCelebratingBirthdayToday(m_LoggedInUser);
            foreach (User friend in friendsWhoCelebratingBirthdayToday)
            {
                listBoxFriendsCelebratingBdayToday.Items.Add(friend);
            }
        }

        private void buttonOpenCreateWishForm_Click(object sender, EventArgs e)
        {
            if (listBoxFriendsCelebratingBdayToday.SelectedItems.Count == 1)
            {
                FormCreateWish wishCard = new FormCreateWish(listBoxFriendsCelebratingBdayToday.SelectedItem as User);
                wishCard.ShowDialog();
            }
        }

        /*-----------------Carpool feature methods---------------------*/

        private void buttonLoadEvents_Click(object sender, EventArgs e)
        {
            if (m_LoggedInUser == null)
            {
                showErrorMessageWhenNoLoggedInUser();
            }
            else
            {
                listBoxEvents.Items.Clear();
                listBoxEvents.DisplayMember = "Name";

                try
                {
                    foreach (Event facebookEvent in m_LoggedInUser.Events)
                    {
                        listBoxEvents.Items.Add(facebookEvent);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                if (listBoxEvents.Items.Count == 0)
                {
                    MessageBox.Show("No events to retrieve :(");
                }
                else
                {
                    m_CarpoolsToMyEvents = CarpoolFeature.LoadCarpoolsFromFile(m_LoggedInUser);
                }
            }
        }

        private void listBoxEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxEvents.SelectedItems.Count == 1)
            {
                Event selectedEvent = listBoxEvents.SelectedItem as Event;
                pictureBoxSingle.LoadAsync(selectedEvent.PictureNormalURL);
                buttonFindCarpool.Visible = true;
                buttonCreateCarpool.Visible = true;
            }
        }

        private void buttonCreateCarpool_Click(object sender, EventArgs e)
        {
            FormCreateCarpool formCarpool = new FormCreateCarpool(m_LoggedInUser);
            buttonFindCarpool.Visible = false;
            buttonCreateCarpool.Visible = false;
            ChangeVisibleCarpoolsProp(false);
            FormCreateCarpool formCreateNewCarpool = new FormCreateCarpool(m_LoggedInUser);
            formCreateNewCarpool.m_NewCarpoolCreatedDelegate += new FormCreateCarpool.newCarpoolCreated(carpoolCreated);
            formCreateNewCarpool.ShowDialog();
        }

        private void ChangeVisibleCarpoolsProp(bool i_IsVisible)
        {
            listViewCarpools.Visible = i_IsVisible;
            labelFilters.Visible = i_IsVisible;
            numericUpDownSeats.Visible = i_IsVisible;
            comboBoxCity.Visible = i_IsVisible;
            checkBoxVacantSeats.Visible = i_IsVisible;
            checkBoxCity.Visible = i_IsVisible;
            labelHowToPick.Visible = i_IsVisible;
        }

        private void carpoolCreated(Carpool i_NewCarpool)
        {
            m_CarpoolsToMyEvents.AllCarpoolToEvents[(listBoxEvents.SelectedItem as Event).Name].Add(i_NewCarpool);
        }

        private void buttonFindCarpool_Click(object sender, EventArgs e)
        {
            ChangeVisibleCarpoolsProp(true);

            if (m_CarpoolsToMyEvents != null)
            {
                if (listBoxEvents.SelectedItems.Count == 1)
                {
                    showAllCarpoolOnListView();
                    List<Carpool> eventCarpools = m_CarpoolsToMyEvents.AllCarpoolToEvents[(listBoxEvents.SelectedItem as Event).Name];
                    foreach (Carpool carpool in eventCarpools)
                    {
                        if(comboBoxCity == null || !(comboBoxCity.Items.Contains(carpool.StartingPointCity)))
                        {
                            comboBoxCity.Items.Add(carpool.StartingPointCity);
                        }
                    }
                }
            }
        }

        private void showAllCarpoolOnListView()
        {
            listViewCarpools.Items.Clear();
            List<Carpool> eventCarpools = m_CarpoolsToMyEvents.AllCarpoolToEvents[(listBoxEvents.SelectedItem as Event).Name];
            foreach (Carpool carpool in eventCarpools)
            {
                addLineToListView(carpool); 
            }
        }

        private void showCarpoolByVacantSeats()
        {
            List<Carpool> eventCarpools = m_CarpoolsToMyEvents.AllCarpoolToEvents[(listBoxEvents.SelectedItem as Event).Name];
            foreach (Carpool carpool in eventCarpools)
            {
                if (carpool.VacantSeats >= numericUpDownSeats.Value)
                {
                    addLineToListView(carpool);
                }
            }
        }

        private void showCarpoolByBothFilters()
        {
            List<Carpool> eventCarpools = m_CarpoolsToMyEvents.AllCarpoolToEvents[(listBoxEvents.SelectedItem as Event).Name];
            foreach (Carpool carpool in eventCarpools)
            {
                if ((carpool.VacantSeats >= numericUpDownSeats.Value) && carpool.StartingPointCity == comboBoxCity.SelectedItem.ToString())
                {
                    addLineToListView(carpool);
                }
            }
        }

        private void switchCarpoolsList()
        {
            listViewCarpools.Items.Clear();
            if (checkBoxCity.Checked && checkBoxVacantSeats.Checked)
            {
                showCarpoolByBothFilters();
            }
            else if(checkBoxCity.Checked)
            {
                showCarpoolFromOneCity();
            }
            else if(checkBoxVacantSeats.Checked)
            {
                showCarpoolByVacantSeats();
            }
            else
            {
                showAllCarpoolOnListView();
            }
        }

        private void showCarpoolFromOneCity()
        {
            List<Carpool> eventCarpools = m_CarpoolsToMyEvents.AllCarpoolToEvents[(listBoxEvents.SelectedItem as Event).Name];
            foreach (Carpool carpool in eventCarpools)
            {
                if(carpool.StartingPointCity == comboBoxCity.SelectedItem.ToString())
                {
                    addLineToListView(carpool);
                }
            }
        }

        private void addLineToListView(Carpool i_Carpool)
        {
            string[] newLine = new string[listViewCarpools.Columns.Count];
            newLine[listViewCarpools.Columns.IndexOf(columnDriverName)] = i_Carpool.DriverName;
            newLine[listViewCarpools.Columns.IndexOf(columnDriverPhone)] = i_Carpool.DriverPhoneNumber;
            newLine[listViewCarpools.Columns.IndexOf(columnSourceCity)] = i_Carpool.StartingPointCity;
            newLine[listViewCarpools.Columns.IndexOf(columnSourceAddress)] = i_Carpool.StartingPointAddress;
            newLine[listViewCarpools.Columns.IndexOf(columnLeavingTime)] = i_Carpool.LeavingTime;
            newLine[listViewCarpools.Columns.IndexOf(columnVacantSeats)] = i_Carpool.VacantSeats.ToString();
            ListViewItem newItem = new ListViewItem(newLine);
            listViewCarpools.Items.Add(newItem);
        }

        private void comboBoxCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            switchCarpoolsList();
        }

        private void numericUpDownSeats_ValueChanged(object sender, EventArgs e)
        {
            switchCarpoolsList();
        }

        private void checkBoxCity_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxCity.Enabled = !comboBoxCity.Enabled;
            if (checkBoxCity.Checked == false)
            {
                switchCarpoolsList();
            }
        }

        private void checkBoxVacantSeats_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownSeats.Enabled = !numericUpDownSeats.Enabled;
            if(checkBoxVacantSeats.Checked == false)
            {
                switchCarpoolsList();
            }
        }

        private void listViewCarpools_ItemActivate(object sender, EventArgs e)
        {
            string eventName = (listBoxEvents.SelectedItem as Event).Name;
            Carpool selectedCarpool = m_CarpoolsToMyEvents.FindCarpoolByDriverName(eventName, listViewCarpools.SelectedItems[0].Text);

            if(selectedCarpool != null)
            {
                try
                {
                    selectedCarpool.AddPassanger(m_LoggedInUser.Name);
                    MessageBox.Show($"You joined {selectedCarpool.DriverName}'s carpool successfully!");
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
