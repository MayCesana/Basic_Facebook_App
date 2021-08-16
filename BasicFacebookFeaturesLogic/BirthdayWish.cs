using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using FacebookWrapper.ObjectModel;

namespace BasicFacebookFeatures
{
    public class BirthdayWish
    {
        public string WishText { get; set; }
        public string WishImageName { get; set; }
        
        private static string[] getBirthdayArray(string i_BirthdayString)
        {
            string[] seperatedBirthday = new string[2];

            seperatedBirthday[0] = i_BirthdayString.Substring(3, 2);
            seperatedBirthday[1] = i_BirthdayString.Substring(0, 2);

            return seperatedBirthday;
        }

        public static List<User> CheckWhoCelebratingBirthdayToday(User i_LoggedInUser)
        {
            List<User> friendWhoCelebratingBirthdayToday = new List<User>();
            foreach (User friend in i_LoggedInUser.Friends)
            {
                string[] birthdayDate = getBirthdayArray(friend.Birthday);

                if (DateTime.Today.Day == int.Parse(birthdayDate[0]))
                {
                    if (DateTime.Today.Month == int.Parse(birthdayDate[1]))
                    {
                        friendWhoCelebratingBirthdayToday.Add(friend);
                    }
                }
            }

            return friendWhoCelebratingBirthdayToday;
        }

        public void PostBirthdayWish(User i_ChoosenFriend)
        {
            string imagePath = String.Format("{0}/Resources/{1}", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), WishImageName);
            if (!string.IsNullOrEmpty(WishImageName))
            {
                i_ChoosenFriend.PostPhoto(imagePath, WishText);
                i_ChoosenFriend.PostStatus(WishText, null, WishImageName);
            }
            else
            {
                i_ChoosenFriend.PostStatus(WishText);
            }
        }
    }
}
