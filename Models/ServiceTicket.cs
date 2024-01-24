namespace HoneyRaesAPI.Models
{
    public class ServiceTicket
    {

        public int Id { get; set; }

        public string CustomerId { get; set; }

        public string Description { get; set; }

        public bool Emergency { get; set; }

        public DateTime DateCompleted { get; set; }
    }
}
