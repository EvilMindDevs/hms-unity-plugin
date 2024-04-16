namespace HmsPlugin
{
    public class HMSLeaderboardEntry
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public HMSLeaderboardEntry(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
