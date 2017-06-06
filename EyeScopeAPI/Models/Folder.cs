namespace EyeScopeAPI.Models
{
    public class Folder
    {
        public string dir { get; set; }
        public bool status { get; set; }

        public Folder(string dir, bool status)
        {
            this.dir = dir;
            this.status = status;
        }
    }
}