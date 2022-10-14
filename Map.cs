namespace robot_controller_api;

public class Map
{
    public int Id { get; set; }
    public int Columns { get; set; }
    public int Rows { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }

    public Map(int id, int columns, int rows, string name, DateTime createdDate, DateTime modifiedDate, string? desc)
    {
        this.Id = id;
        //if (rows < 2 && columns < 2 || rows > 100 && columns > 100)
        //{
        //    this.Columns = 25;
        //    this.Rows = 25;
        //}
        //else
        //{
        //    this.Columns = columns;
        //    this.Rows = rows;
        //}
        this.Columns = columns;
        this.Rows = rows;
        this.Name = name;
        this.Description = desc;
        this.CreatedDate = createdDate;
        this.ModifiedDate = modifiedDate;
    }

    public Map() { }
}
