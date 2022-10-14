using Npgsql;
namespace robot_controller_api.Persistence;

public interface IRobotCommandDataAccess
{
  Boolean CheckEntry(int id);
  Boolean CheckName(string name);
  int DeleteRobotCommands(int id);
  List<RobotCommand> GetRobotCommands();
  int InsertRobotCommands(RobotCommand newRobotCommand);
  int UpdateRobotCommands(int commandId, RobotCommand newRobotCommand);
}

public class RobotCommandADO: IRobotCommandDataAccess
{
  private const string CONNECTION_STRING = "Host=localhost;Username=postgres;Password=12345;Database=sit331";

  public Boolean CheckEntry(int id)
  {
    using var conn = new NpgsqlConnection(CONNECTION_STRING);
    conn.Open();

    using var cmd = new NpgsqlCommand("SELECT * FROM robot_command WHERE id=" + id, conn);
    using var dr = cmd.ExecuteReader();

    if (dr.Read())
    {
      return true;
    }
    return false;
  }

  public Boolean CheckName(string name)
  {
    using var conn = new NpgsqlConnection(CONNECTION_STRING);
    conn.Open();

    using var cmd = new NpgsqlCommand("SELECT * FROM robot_command WHERE name=" + "\'" + name + "\'", conn);
    using var dr = cmd.ExecuteReader();

    if (dr.Read())
    {
      return true;
    }
    return false;
  }

  public int DeleteRobotCommands(int id)
  {
    using var conn = new NpgsqlConnection(CONNECTION_STRING);
    conn.Open();

    using var cmd2 = new NpgsqlCommand("DELETE FROM robot_command WHERE id=" + id, conn);

    int _id = -1;
    object a = cmd2.ExecuteScalar();
      if (a != null) {
        _id = (int)a;
      }

    return _id;
  }

  public List<RobotCommand> GetRobotCommands()
  {
    var robotCommands = new List<RobotCommand>();
    using var conn = new NpgsqlConnection(CONNECTION_STRING);
    conn.Open();
    using var cmd = new NpgsqlCommand("SELECT * FROM robot_command", conn);
    using var dr = cmd.ExecuteReader();
    while (dr.Read())
    {
      var id = (int)dr["id"];
      string desc = dr["description"] == null ? null : dr["description"] as string;
      string name = (string)dr["name"];
      Boolean isMove = (Boolean)dr["is_move_command"];
      DateTime createdDate = (DateTime)dr["created_date"];
      DateTime modifiedDate = (DateTime)dr["modified_date"];

      robotCommands.Add(new RobotCommand(id, name, isMove, createdDate, modifiedDate, desc));
    }
    return robotCommands;
  }

  public int InsertRobotCommands(RobotCommand newRobotCommand)
  {
    using var conn = new NpgsqlConnection(CONNECTION_STRING);
    conn.Open();
    int id = newRobotCommand.Id;
    string desc = newRobotCommand.Description;
    string name = newRobotCommand.Name;
    Boolean isMove = newRobotCommand.IsMoveCommand;
    DateTime createdDate = DateTime.Now;
    DateTime modifiedDate = DateTime.Now;

    using var cmd = new NpgsqlCommand("INSERT INTO robot_command (id, name, description, is_move_command, created_date, modified_date)" +
        " VALUES(DEFAULT," +
        "\'" + name + "\'," +
        "\'" + desc + "\'," +
        isMove + "," +
        "\'" + createdDate + "\'," +
        "\'" + modifiedDate + "\')"
        , conn);

    cmd.Connection = conn;

    int _id = -1;
    object a = cmd.ExecuteScalar();
      if (a != null) {
        _id = (int)a;
      }

    return _id;
  }

  public int UpdateRobotCommands(int commandId, RobotCommand newRobotCommand)
  {
    using var conn = new NpgsqlConnection(CONNECTION_STRING);
    conn.Open();

    string desc = newRobotCommand.Description;
    string name = newRobotCommand.Name;
    Boolean isMove = newRobotCommand.IsMoveCommand;
    DateTime createdDate = newRobotCommand.CreatedDate;
    DateTime modifiedDate = DateTime.Now;

    using var cmd = new NpgsqlCommand("UPDATE robot_command SET name= \'" + name + "\', " +
        "description=\'" + desc + "\'," +
        "is_move_command=" + isMove + "," +
        "created_date=\'" + createdDate + "\'," +
        "modified_date=\'" + modifiedDate + "\'" +
        " WHERE id=" + commandId, conn);

    cmd.Connection = conn;
    int _id = -1;
    object a = cmd.ExecuteScalar();
      if (a != null) {
        _id = (int)a;
      }

    return _id;
  }
}
