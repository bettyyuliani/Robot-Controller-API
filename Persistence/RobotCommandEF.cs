namespace robot_controller_api.Persistence;

public class RobotCommandEF : RobotContext, IRobotCommandDataAccess
{
  private RobotContext context => this;

  public List<RobotCommand> GetRobotCommands()
  {
    var commands = context.RobotCommands.ToList();
    return commands;
  }

  public int UpdateRobotCommands(int id, RobotCommand updatedCommand)
  {
    RobotCommand command = context.RobotCommands.FirstOrDefault(d=>d.Id==id);
    if (command != null)
    {
      if (updatedCommand.Name != null) command.Name = updatedCommand.Name;
      if (updatedCommand.Description != null) command.Description = updatedCommand.Description;
      command.ModifiedDate = System.DateTime.Now;
      if (updatedCommand.IsMoveCommand != null) command.IsMoveCommand = updatedCommand.IsMoveCommand;
      context.SaveChanges();
    }
    return command.Id;
  }


  public int InsertRobotCommands(RobotCommand newRobotCommand)
  {
    RobotCommand command = newRobotCommand;
    command.CreatedDate = System.DateTime.Now;
    command.ModifiedDate = System.DateTime.Now;

    context.RobotCommands.Add(command);
    context.SaveChanges();

    return command.Id;
  }

  public int DeleteRobotCommands(int id)
  {
    RobotCommand command = context.RobotCommands.FirstOrDefault(d=>d.Id==id);
    context.Remove(command);
    context.SaveChanges();
    return command.Id;
}

  public Boolean CheckEntry(int id)
  {
    RobotCommand command = context.RobotCommands.FirstOrDefault(d=>d.Id==id);
    if (command != null)
    {
      return true;
    }
    return false;
  }

  public Boolean CheckName(string name)
  {
    RobotCommand command = context.RobotCommands.FirstOrDefault(d=>d.Name==name);
    if (command != null)
    {
      return true;
    }
    return false;
  }
}
