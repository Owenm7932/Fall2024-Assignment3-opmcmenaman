using Fall2024_Assignment3_opmcmenaman.Models;

public class MovieActor
{
    public string MovieId { get; set; }
    public Movie Movie { get; set; }

    public string ActorId { get; set; } // Change to match Actor's primary key type
    public Actor Actor { get; set; }
}
