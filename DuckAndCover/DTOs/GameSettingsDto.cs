namespace DTOs
{
    public class GameSettingsDto
    {
        public int PlayerCount { get; set; }
        public bool UseBots { get; set; }
        public int BotCount { get; set; }
        public string Rules { get; set; } = "Classic";
    }
}