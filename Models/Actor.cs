namespace Fall2024_Assignment3_opmcmenaman.Models
{
    public class Actor
    {
        public string ActorId { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string IMDBLink { get; set; }
        public string PhotoURL { get; set; }
        public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
    }

}
