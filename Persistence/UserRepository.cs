using Npgsql;
using Microsoft.AspNetCore.Identity;
using BCrypt.Net;
namespace robot_controller_api.Persistence;

public interface IUserDataAccess
{
  Boolean CheckEntry(int id);
  Boolean CheckEmail(string email);
  void DeleteUser(int id);
  List<UserModel> GetUsers();
  List<UserModel> GetAdminUsers();
  void InsertUser(UserModel user);
  void UpdateUser(int userId, UserModel newUser);
  void PatchUser(int userId, LoginModel user);
}

public class UserRepository : IUserDataAccess, IRepository
{
  private IRepository _repo => this;
  public List<UserModel> GetUsers()
  {
    var users = _repo.ExecuteReader<UserModel>("SELECT * FROM public.user");
    return users;
  }

  public List<UserModel> GetAdminUsers()
  {
    var users = _repo.ExecuteReader<UserModel>("SELECT * FROM public.user WHERE role=\'admin\' or role=\'Admin\' or role=\'ADMIN\'");
    return users;
  }
  public void UpdateUser(int id, UserModel newUser)
  {
    var sqlParams = new NpgsqlParameter[]{
                      new("id", id),
                      new("firstname", newUser.FirstName),
                      new("lastname", newUser.LastName),
                      new("description", newUser.Description ?? (object)DBNull.Value),
                      new("role", newUser.Role ?? (object)DBNull.Value)
      };

    _repo.ExecuteReader<UserModel>(
      "UPDATE public.user SET firstname=@firstname, lastname=@lastname, description=@description, " +
      "role=@role, modifieddate=current_timestamp WHERE id=@id;", sqlParams);
    // return result;
  }


  public void InsertUser(UserModel newUser)
  {
    var password = newUser.PasswordHash;
    //var hasher = new PasswordHasher<UserModel>();
    //var hashedPassword = hasher.HashPassword(newUser, password);
    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
    //var pwVerificationResult = hasher.VerifyHashedPassword(newUser, hashedPassword, password);
    newUser.PasswordHash = hashedPassword;
    var sqlParams = new NpgsqlParameter[]{
                      new("id", newUser.Id),
                      new("email", newUser.Email),
                      new("firstname", newUser.FirstName),
                      new("lastname", newUser.LastName),
                      new("passwordhash", newUser.PasswordHash),
                      new("description", newUser.Description ?? (object)DBNull.Value),
                      new("role", newUser.Role ?? (object)DBNull.Value)
            };

    _repo.ExecuteReader<UserModel>("INSERT INTO public.user(email, firstname, lastname, passwordhash, description, role, createddate, modifieddate) VALUES(@email, @firstname, @lastname, @passwordhash, @description, @role, current_timestamp, current_timestamp);", sqlParams);
  }

  public void DeleteUser(int id)
  {
    _repo.ExecuteReader<UserModel>("DELETE FROM public.user WHERE id=" + id, null);
  }

  public Boolean CheckEntry(int id)
  {
    var entries = _repo.ExecuteReader<UserModel>("SELECT * FROM public.user WHERE id=" + id, null);

    if (entries.Count > 0) return true;
    else return false;
  }

  public Boolean CheckEmail(string email)
  {
    var entries = _repo.ExecuteReader<UserModel>("SELECT * FROM public.user WHERE email=" + "\'" + email + "\'", null);

    if (entries.Count > 0) return true;
    else return false;
  }

  public void PatchUser(int userId, LoginModel user)
  {
    var sqlParams = new NpgsqlParameter[]{
                    new("id", userId),
                    new("email", user.Email),
                    new("passwordhash", user.Password),
      };

    _repo.ExecuteReader<LoginModel>(
      "UPDATE public.user SET email=@email, passwordhash=@passwordhash WHERE id=@id;", sqlParams);
  }
}
