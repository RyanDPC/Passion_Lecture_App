namespace App.Models
{
    public class ReadingProgress
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int Page { get; set; }
        public string Chapter { get; set; }
        public double Position { get; set; }
        public DateTime LastRead { get; set; }
        public virtual Book Book { get; set; }
    }
}
