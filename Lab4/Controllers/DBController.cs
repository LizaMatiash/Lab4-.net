using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Mvc;
using Lab4.Models;
using Lab4.Models.Service;

namespace Lab4.Controllers
{
    public class DBController
    {
        private Models.Service.EFUnitOfWork unitOfWork;
        private Models.User currentUser;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsLogined()
        {
            currentUser = (Models.User)HttpContext.Current.Session["CurrrentUser"];
            if (currentUser != null)
            {
                currentUser = (Models.User)HttpContext.Current.Session["CurrrentUser"];
                return true;
                //HttpContext.Current.Session.Add("cur", currentUser);
            }
            else
            {
                return false;
            }
        }
        public DBController()
        {
            unitOfWork = new Models.Service.EFUnitOfWork();
        }


        public string CurrentUserNickname()
        {
            if (currentUser == null)
            {
                return "користувач";
            }
            else
            {
                return currentUser.Nickname;
            }
        }
        public bool IsNicknameUnique(string nickname)
        {
            List<User> uList = unitOfWork.User_s.Where(p => p.Nickname == nickname).ToList();
            return uList.Count == 0;
        }

        public bool IsLoginUnique(string login)
        {
            List<User> uList = unitOfWork.User_s.Where(p => p.Login == login).ToList();
            return uList.Count == 0;
        }

        public bool IsLoginCorrect(string login)
        {
            List<User> uList = unitOfWork.User_s.Where(p => p.Login == login).ToList();
            return uList.Count == 1;
        }

        private bool IsPasswordCorrect(string userPassword, string dbPassword)
        {
            return userPassword == dbPassword;
        }

        public string Register(string login, string nickname, string password)
        {
            string result = "Unexpected error!";
            if (IsLoginUnique(login) && IsNicknameUnique(nickname))
            {
                unitOfWork.User_s.Create(new User { ID = Guid.NewGuid(), Login = login, Nickname = nickname, Password = password });
                unitOfWork.Save();
                result = "Користувача " + nickname + " успішно зареєстровано.";
            }
            else
            {
                result = "Користувач з таким нікнеймом або логіном вже зареєстрований, оберіть інший нікнейм і/або логін.";
            }
            return result;
        }

        public string Login(string login, string password)
        {
            if (IsLoginCorrect(login))
            {
                List<User> uList = unitOfWork.User_s.Where(p => p.Login == login).ToList();
                uList = uList.Where(p => IsPasswordCorrect(password, p.Password)).ToList();
                if (uList.Count == 1)
                {
                    //logic
                    currentUser = uList.First();
                    return "Ласкаво просимо, " + CurrentUserNickname() + ".";
                }
                else
                {
                    return "Невірний пароль.";
                }
            }
            else
            {
                return "Не знайдено користувача з таким логіном.";
            }
        }

        public string Logout()
        {
            if (currentUser == null)
            {
                return "Щоб вийти, потрібно спочатку увійти.";
            }
            else
            {
                currentUser = null;
                return "Ви вийшли зі свого аккаунта.";
            }
        }

        public string Invite(string nickname)
        {
            if (currentUser != null)
            {
                User owner = currentUser;
                List<User> uList = unitOfWork.User_s.Where(p => p.Nickname == nickname).ToList();
                if (uList.Count() == 1)
                {
                    User included = uList.First();
                    List<Friendship> fList = unitOfWork.Friendship_s.Where(p => (p.ListOwnerID == owner.ID && p.IncludedPersonID == included.ID)).ToList();
                    if (fList.Count == 0)
                    {
                        Friendship newFriendship = new Friendship
                        { ID = Guid.NewGuid(), ListOwnerID = owner.ID, IncludedPersonID = included.ID, RequestSenderID = owner.ID, RequestReceiverID = included.ID, RequestConfirmed = false };
                        unitOfWork.Friendship_s.Create(newFriendship);
                        unitOfWork.Save();
                        return "Запрошення в ваш список друзів надіслано користувачу.";
                    }
                    else
                    {
                        Friendship existingFriendship = fList.First();
                        if (existingFriendship.RequestConfirmed)
                        {
                            return "Користувач вже є у вашому списку друзів.";
                        }
                        else
                        {
                            if (existingFriendship.RequestSenderID == owner.ID)
                            {
                                return "Ви вже надсилали запрошення цьому користувачу.";
                            }
                            else
                            {
                                return "Користувач вже надіслав запит у ваш список друзів, підтвердьте його запит натомість.";
                            }
                        }
                    }
                }
                else
                {
                    return "Не знайдено користувача з таким нікнеймом.";
                }
            }
            else
            {
                return "Спочатку увійдіть в систему.";
            }
        }
        public string InviteMe(string nickname)
        {
            if (currentUser != null)
            {
                User included = currentUser;
                List<User> uList = unitOfWork.User_s.Where(p => p.Nickname == nickname).ToList();
                if (uList.Count() == 1)
                {
                    User owner = uList.First();
                    List<Friendship> fList = unitOfWork.Friendship_s.Where(p => (p.ListOwnerID == owner.ID && p.IncludedPersonID == included.ID)).ToList();
                    if (fList.Count == 0)
                    {
                        Friendship newFriendship = new Friendship
                        { ID = Guid.NewGuid(), ListOwnerID = owner.ID, IncludedPersonID = included.ID, RequestSenderID = included.ID, RequestReceiverID = owner.ID, RequestConfirmed = false };
                        unitOfWork.Friendship_s.Create(newFriendship);
                        unitOfWork.Save();
                        return "Користувачу надіслано ваш запит в його список друзів.";
                    }
                    else
                    {
                        Friendship existingFriendship = fList.First();
                        if (existingFriendship.RequestConfirmed)
                        {
                            return "Ви вже у списку друзів цього користувача.";
                        }
                        else
                        {
                            if (existingFriendship.RequestSenderID == included.ID)
                            {
                                return "Ви вже надсилали запит у список друзів цього користувача.";
                            }
                            else
                            {
                                return "Користувач вже запросив вас у свій список друзів, підтвердьте його запрошення натомість.";
                            }
                        }
                    }
                }
                else
                {
                    return "Не знайдено користувача з таким нікнеймом.";
                }
            }
            else
            {
                return "Спочатку увійдіть в систему.";
            }
        }

        public string Accept(string nickname)
        {
            if (currentUser != null)
            {
                User owner = currentUser;
                List<User> uList = unitOfWork.User_s.Where(p => p.Nickname == nickname).ToList();
                if (uList.Count() == 1)
                {
                    User included = uList.First();
                    List<Friendship> fList = unitOfWork.Friendship_s.Where
                    (p => (p.ListOwnerID == owner.ID && p.RequestReceiverID == owner.ID
                    && p.IncludedPersonID == included.ID && p.RequestSenderID == included.ID
                    && p.RequestConfirmed == false)).ToList();
                    if (fList.Count == 1)
                    {
                        Friendship friend = fList.First();
                        friend.RequestConfirmed = true;
                        unitOfWork.Friendship_s.Update(friend);
                        unitOfWork.Save();
                        return "Запит користувача у ваш список друзів підтверджено.";
                    }
                    else
                    {
                        return "Користувач з таким нікнеймом не надсилав запит у ваш список друзів.";
                    };


                }
                else
                {
                    return "Не знайдено користувача з таким нікнеймом.";
                }

            }
            else
            {
                return "Спочатку увійдіть в систему.";
            }
        }
        public string AcceptMe(string nickname)
        {
            if (currentUser != null)
            {
                User included = currentUser;
                List<User> uList = unitOfWork.User_s.Where(p => p.Nickname == nickname).ToList();
                if (uList.Count() == 1)
                {
                    User owner = uList.First();
                    List<Friendship> fList = unitOfWork.Friendship_s.Where
                    (p => (p.IncludedPersonID == included.ID && p.RequestReceiverID == included.ID
                    && p.ListOwnerID == owner.ID && p.RequestSenderID == owner.ID
                    && p.RequestConfirmed == false)).ToList();
                    if (fList.Count == 1)
                    {
                        Friendship friend = fList.First();
                        friend.RequestConfirmed = true;
                        unitOfWork.Friendship_s.Update(friend);
                        unitOfWork.Save();
                        return "Запрошення вас користувачем у його список друзів підтверджено.";
                    }
                    else
                    {
                        return "Користувач з таким нікнеймом не запрошував вас у список друзів.";
                    };

                }
                else
                {
                    return "Не знайдено користувача з таким нікнеймом.";
                }
            }
            else
            {
                return "Спочатку увійдіть в систему.";
            }
        }
        public string Send(string nickname, string body)
        {
            if (currentUser != null)
            {
                List<User> uList = unitOfWork.User_s.Where(p => p.Nickname == nickname).ToList();
                if (uList.Count() == 1)
                {
                    User friend = uList.First();
                    List<Friendship> fList = unitOfWork.Friendship_s.Where(p =>
                    (p.RequestConfirmed == true && p.ListOwnerID == currentUser.ID && p.IncludedPersonID == friend.ID) ||
                    (p.RequestConfirmed == true && p.ListOwnerID == friend.ID && p.IncludedPersonID == currentUser.ID)).ToList();
                    if (fList.Count() == 1 || fList.Count() == 2)
                    {
                        Message message = new Message
                        { ID = Guid.NewGuid(), SenderID = currentUser.ID, ReceiverID = friend.ID, SentTimeStamp = DateTime.Now, MessageBody = body };
                        unitOfWork.Message_s.Create(message);
                        unitOfWork.Save();
                        return "Повідомлення надіслано.";
                    }
                    else
                    {
                        return "Користувач не пов'язаний з вами дружбою. Повідомлення не надіслано.";
                    }
                }
                else
                {
                    return "Не знайдено користувача з таким нікнеймом. Повідомлення не надіслано.";
                }
            }
            else
            {
                return "Спочатку увійдіть в систему.";
            }
        }
        public string Show(string nickname)
        {
            if (currentUser != null)
            {
                List<User> uList = unitOfWork.User_s.Where(p => p.Nickname == nickname).ToList();
                if (uList.Count() == 1)
                {
                    User friend = uList.First();
                    List<Friendship> fList = unitOfWork.Friendship_s.Where(p =>
                    (p.RequestConfirmed == true && p.ListOwnerID == currentUser.ID && p.IncludedPersonID == friend.ID) ||
                    (p.RequestConfirmed == true && p.ListOwnerID == friend.ID && p.IncludedPersonID == currentUser.ID)).ToList();
                    if (fList.Count() == 1 || fList.Count() == 2)
                    {
                        List<Message> mList = unitOfWork.Message_s.Where(p =>
                        (p.SenderID == currentUser.ID && p.ReceiverID == friend.ID) ||
                        (p.ReceiverID == currentUser.ID && p.SenderID == friend.ID)
                        ).ToList();
                        mList = mList.OrderBy(p => p.SentTimeStamp).ToList();
                        StringBuilder sb = new StringBuilder();
                        foreach (Message m in mList)
                        {
                            //sb.Append("\t");
                            if (m.SenderID == currentUser.ID)
                            {
                                sb.Append(currentUser.Nickname);
                            }
                            else
                            {
                                sb.Append(friend.Nickname);
                            }
                            sb.Append(" о ");
                            sb.Append(m.SentTimeStamp.ToString("f"));
                            sb.Append(": -");
                            sb.Append(m.MessageBody);
                            sb.Append("\r\n");
                        }
                        return sb.ToString();
                    }
                    else
                    {
                        return "Користувач не пов'язаний з вами дружбою.";
                    }
                }
                else
                {
                    return "Не знайдено користувача з таким нікнеймом.";
                }
            }
            else
            {
                return "Спочатку увійдіть в систему.";
            }
        }

        public void Exit()
        {
            unitOfWork.Dispose();
        }
        public string GetFriends()
        {
            if (currentUser != null)
            {
                User owner = currentUser;
                List<Friendship> fList = unitOfWork.Friendship_s.Where
                    (p => (p.ListOwnerID == owner.ID && p.RequestConfirmed == true)).ToList();
                StringBuilder sb = new StringBuilder();
                User included;
                foreach (Friendship fs in fList)
                {
                    included = unitOfWork.User_s.Get(fs.IncludedPersonID);
                    sb.Append("\t");
                    sb.Append(included.Nickname);
                    sb.Append("\r\n");
                }
                return sb.ToString();
            }
            else
            {
                return "Спочатку увійдіть в систему.";
            }
        }

        public string GetFriendsMe()
        {
            if (currentUser != null)
            {
                User included = currentUser;
                List<Friendship> fList = unitOfWork.Friendship_s.Where
                    (p => (p.IncludedPersonID == included.ID && p.RequestConfirmed == true)).ToList();
                StringBuilder sb = new StringBuilder();
                User owner;
                foreach (Friendship fs in fList)
                {
                    owner = unitOfWork.User_s.Get(fs.ListOwnerID);
                    sb.Append("\t");
                    sb.Append(owner.Nickname);
                    sb.Append("\r\n");
                }
                return sb.ToString();
            }
            else
            {
                return "Спочатку увійдіть в систему.";
            }
        }

        public string GetAllFriends()
        {
            if (currentUser != null)
            {
                List<Friendship> fListIncluded = unitOfWork.Friendship_s.Where
                    (p => (p.ListOwnerID == currentUser.ID && p.RequestConfirmed == true)).ToList();
                List<Friendship> fListOwners = unitOfWork.Friendship_s.Where
                    (p => (p.IncludedPersonID == currentUser.ID && p.RequestConfirmed == true)).ToList();
                StringBuilder sb = new StringBuilder();
                User friend;
                foreach (Friendship fs in fListIncluded)
                {
                    friend = unitOfWork.User_s.Get(fs.IncludedPersonID);
                    sb.Append("\t");
                    sb.Append(friend.Nickname);
                    sb.Append("\r\n");
                }
                foreach (Friendship fs in fListOwners)
                {
                    friend = unitOfWork.User_s.Get(fs.ListOwnerID);
                    sb.Append("\t");
                    sb.Append(friend.Nickname);
                    sb.Append("\r\n");
                }
                return sb.ToString();
            }
            else
            {
                return "Спочатку увійдіть в систему.";
            }
        }

        public string GetAccepts()
        {
            if (currentUser != null)
            {
                User owner = currentUser;
                List<Friendship> fList = unitOfWork.Friendship_s.Where
                    (p => (p.ListOwnerID == owner.ID && p.RequestReceiverID == owner.ID && p.RequestConfirmed == false)).ToList();
                StringBuilder sb = new StringBuilder();
                User inviter;
                foreach (Friendship friend in fList)
                {
                    inviter = unitOfWork.User_s.Get(friend.IncludedPersonID);
                    sb.Append("\t");
                    sb.Append(inviter.Nickname);
                    sb.Append("\r\n");
                }
                return sb.ToString();
            }
            else
            {
                return "Спочатку увійдіть в систему.";
            }
        }
        public string GetAcceptsMe()
        {
            if (currentUser != null)
            {
                User included = currentUser;
                List<Friendship> fList = unitOfWork.Friendship_s.Where
                    (p => (p.IncludedPersonID == included.ID && p.RequestReceiverID == included.ID && p.RequestConfirmed == false)).ToList();
                StringBuilder sb = new StringBuilder();
                User owner;
                foreach (Friendship friend in fList)
                {
                    owner = unitOfWork.User_s.Get(friend.ListOwnerID);
                    sb.Append("\t");
                    sb.Append(owner.Nickname);
                    sb.Append("\r\n");
                }
                return sb.ToString();
            }
            else
            {
                return "Спочатку увійдіть в систему.";
            }
        }

        //public string Func(string arg1, string arg2) { return ""; }
    }
}