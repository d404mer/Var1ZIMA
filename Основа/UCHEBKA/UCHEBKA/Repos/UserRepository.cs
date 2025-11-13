using Microsoft.EntityFrameworkCore;
using System.Linq;
using UCHEBKA.Models;
using System.IO;

namespace UCHEBKA.Repos
{
    public class UserRepository
    {
        private readonly UchebnayaLeto2025Context _db;
        private const string BaseImagePath = "Images\\Users\\";

        public UserRepository(UchebnayaLeto2025Context db)
        {
            _db = db;
        }

        private string GetProjectRootPath()
        {
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var projectRoot = Path.GetFullPath(Path.Combine(currentDir, "..\\..\\..\\"));
            return projectRoot;
        }

        public User Auth(int userId, string password)
        {
            return _db.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.FkRole)
                .FirstOrDefault(u => u.UserId == userId && u.UserPassword == password);
        }

        public void Logout()
        {
            File.WriteAllText("CurrUser.txt", string.Empty);
        }
        public void SaveCurrentUser(int userId, string password)
        {
            File.WriteAllText("CurrUser.txt", $"{userId}|{password}");
        }

        public (int userId, string password)? GetCurrentUser()
        {
            if (!File.Exists("CurrUser.txt"))
                return null;

            var content = File.ReadAllText("CurrUser.txt");
            if (string.IsNullOrWhiteSpace(content))
                return null;

            var parts = content.Split('|');
            if (parts.Length != 2 || !int.TryParse(parts[0], out int userId))
                return null;

            return (userId, parts[1]);
        }

        public string GetUserRole(long userId)
        {
            return _db.UserRoles
                .Include(ur => ur.FkRole)
                .Where(ur => ur.FkUserId == userId)
                .Select(ur => ur.FkRole.RoleName)
                .FirstOrDefault();
        }

        public string GetUserSex(long userId)
        {
            return _db.UserSexes
                .Where(us => us.FkUserId == userId)
                .Select(us => us.FkSex.SexName)
                .FirstOrDefault();
        }


        public User GetUserById(long userId)
        {
            return _db.Users
                .Include(u => u.UserSexes)
                .ThenInclude(us => us.FkSex)
                .FirstOrDefault(u => u.UserId == userId);
        }

        public void UpdateUser(User user)
        {
            _db.Users.Update(user);
            _db.SaveChanges();
        }

        public List<Sex> GetAllSexes()
        {
            return _db.Sexes.ToList();
        }

        public void UpdateUserSex(long userId, long sexId)
        {
            var userSex = _db.UserSexes.FirstOrDefault(us => us.FkUserId == userId);

            if (userSex != null)
            {
                userSex.FkSexId = sexId;
            }
            else
            {
                _db.UserSexes.Add(new UserSex
                {
                    UserSecId = GetNextUserSexId(),
                    FkUserId = userId,
                    FkSexId = sexId
                });
            }

            _db.SaveChanges();
        }

        public bool UpdatePassword(long userId, string currentPassword, string newPassword)
        {
            var user = _db.Users.FirstOrDefault(u => u.UserId == userId && u.UserPassword == currentPassword);

            if (user != null)
            {
                user.UserPassword = newPassword;
                _db.SaveChanges();
                return true;
            }

            return false;
        }

        public User GetCurrentUserData()
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null) return null;

            return GetUserById(currentUser.Value.userId);
        }

        public string GetFullImagePath(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return "2.jpeg";

            return fileName;
        }

        public string GetDisplayImagePath(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return Path.Combine(GetProjectRootPath(), BaseImagePath, "2.jpeg");

            var fullPath = Path.Combine(GetProjectRootPath(), BaseImagePath, fileName);

            if (!System.IO.File.Exists(fullPath))
            {
                Console.WriteLine($"Image not found: {fullPath}");
                return Path.Combine(GetProjectRootPath(), BaseImagePath, "1.jpeg");
            }

            return fullPath;
        }

        public long GetNextUserId()
        {
            return _db.Users.Any() ? _db.Users.Max(u => u.UserId) + 1 : 1;
        }

        public void AddUser(User user)
        {
            _db.Users.Add(user);
            _db.SaveChanges();
        }

        public void AddUserRole(long userId, long roleId)
        {
            _db.UserRoles.Add(new UserRole
            {
                FkUserId = userId,
                FkRoleId = roleId
            });
            _db.SaveChanges();
        }

        public void AddUserEvent(long userId, long eventId)
        {
            _db.UserEvents.Add(new UserEvent
            {
                FkUserId = userId,
                FkEventId = eventId
            });
            _db.SaveChanges();
        }

        public long? GetUserSexId(long userId)
        {
            return _db.UserSexes
                .Where(us => us.FkUserId == userId)
                .Select(us => us.FkSexId)
                .FirstOrDefault();
        }

        public long GetNextUserRoleId()
        {
            return _db.UserRoles.Any() ? _db.UserRoles.Max(ur => ur.UserRoleId) + 1 : 1;
        }

        public long GetNextUserSecId()
        {
            return _db.UserSecs.Any() ? _db.UserSecs.Max(us => us.UserSecId) + 1 : 1;
        }

        public long GetNextUserSexId()
        {
            return _db.UserSexes.Any() ? _db.UserSexes.Max(us => us.UserSecId) + 1 : 1;
        }

        public bool DeleteUser(long userId)
        {
            try
            {
                // Находим пользователя по ID, включая все связанные сущности
                var userToDelete = _db.Users
                    .Include(u => u.UserRoles)
                    .Include(u => u.UserSexes)
                    .Include(u => u.UserSecs)
                    .Include(u => u.UserEvents)
                    .Include(u => u.EventJuries)
                    .FirstOrDefault(u => u.UserId == userId);

                if (userToDelete != null)
                {
                    // Удаляем связанные записи из таблиц связей
                    _db.UserRoles.RemoveRange(userToDelete.UserRoles);
                    _db.UserSexes.RemoveRange(userToDelete.UserSexes);
                    _db.UserSecs.RemoveRange(userToDelete.UserSecs);
                    _db.UserEvents.RemoveRange(userToDelete.UserEvents);
                    _db.EventJuries.RemoveRange(userToDelete.EventJuries);

                    // Удаляем самого пользователя
                    _db.Users.Remove(userToDelete);
                    _db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Логирование ошибки или вывод в консоль для отладки
                Console.WriteLine($"Ошибка при удалении пользователя: {ex.Message}\n{ex.InnerException?.Message}");
                return false;
            }
        }
    }
}