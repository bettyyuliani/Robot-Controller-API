using Npgsql;
namespace robot_controller_api.Persistence;

public class RobotCommandRepository : IRobotCommandDataAccess, IRepository
{
  private IRepository _repo => this;
  public List<RobotCommand> GetRobotCommands()
  {
    var commands = _repo.ExecuteReader<RobotCommand>("SELECT * FROM public.robot_command");
    return commands;
  }

  public int UpdateRobotCommands(int id, RobotCommand updatedCommand)
  {
    var sqlParams = new NpgsqlParameter[]{
                new("id", id),
                new("name", updatedCommand.Name),
                new("description", updatedCommand.Description ??
                (object)DBNull.Value),
                new("ismovecommand", updatedCommand.IsMoveCommand)
      };

    var result = _repo.ExecuteReader<RobotCommand>(
      "UPDATE robot_command SET name=@name, description=@description, " +
      "is_move_command=@ismovecommand, modified_date=current_timestamp WHERE id=@id RETURNING id;", sqlParams).Single();
    return result.Id;
  }


  public int InsertRobotCommands(RobotCommand newRobotCommand)
  {
    var sqlParams = new NpgsqlParameter[]{
                      new("id", newRobotCommand.Id),
                      new("name", newRobotCommand.Name),
                      new("description", newRobotCommand.Description ??
                      (object)DBNull.Value),
                      new("ismovecommand", newRobotCommand.IsMoveCommand)
            };

    var result = _repo.ExecuteReader<RobotCommand>("INSERT INTO robot_command(id, name, description, is_move_command, created_date, modified_date) VALUES(default, @name, @description, @ismovecommand, current_timestamp, current_timestamp) RETURNING id;", sqlParams).Single();
    return result.Id;
  }

  public int DeleteRobotCommands(int id)
  {
    var result = _repo.ExecuteReader<RobotCommand>("DELETE FROM robot_command WHERE id=" + id + "RETURNING id", null).Single();
    return result.Id;
  }

  public Boolean CheckEntry(int id)
  {
    var entries = _repo.ExecuteReader<RobotCommand>("SELECT * FROM robot_command WHERE id=" + id, null);

    if (entries.Count > 0) return true;
    else return false;
  }

  public Boolean CheckName(string name)
  {
    var entries = _repo.ExecuteReader<RobotCommand>("SELECT * FROM robot_command WHERE name=" + "\'" + name + "\'", null);

    if (entries.Count > 0) return true;
    else return false;
  }

  public interface IRobotCommandDataAccess
  {
    Boolean CheckEntry(int id);
    Boolean CheckName(string name);
    int DeleteRobotCommands(int id);
    List<RobotCommand> GetRobotCommands();
    int InsertRobotCommands(RobotCommand newRobotCommand);
    int UpdateRobotCommands(int commandId, RobotCommand newRobotCommand);
  }

}
