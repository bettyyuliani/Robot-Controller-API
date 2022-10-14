namespace robot_controller_api;

public class RobotCommand
{
    public int Id { get; set;}
    public string Name {get; set;}
    public string? Description {get; set;}
    public bool IsMoveCommand {get; set;}
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate {get; set;}

    public RobotCommand(){}

    public RobotCommand(int id, string name, bool isMoveCommand, DateTime createdDate, DateTime modifiedDate, string? description)
    {
        this.Id = id;
        this.Name = name;
        this.IsMoveCommand = isMoveCommand;
        this.CreatedDate = createdDate;
        this.ModifiedDate = modifiedDate;
        this.Description = description;
    }
}
