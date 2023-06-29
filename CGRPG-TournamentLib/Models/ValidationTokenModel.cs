namespace CGRPG_TournamentLib.Models
{
    public class ValidationTokenModel
    {
        public readonly bool success;
        public readonly string username;
        public string message;
        public int increment_id;
        public string mm_address;

        public ValidationTokenModel(bool success, string message, int incrementId, string mmAddress, string username)
        {
            this.success = success;
            this.username = username;
            this.message = message;
            increment_id = incrementId;
            mm_address = mmAddress;
        }
    }
}