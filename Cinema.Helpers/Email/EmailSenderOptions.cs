namespace Cinema.Helpers.Email
{
    public sealed class EmailSenderOptions
    {
        public string EmailFrom { get; set; } = string.Empty;

        public string EmailSenderName { get; set; } = string.Empty;

        public string SmtpHost { get; set; } = string.Empty;

        public int SmtpPort { get; set; }

        public string SmtpUser { get; set; } = string.Empty;

        public string SmtpPass { get; set; } = string.Empty;

        public string GenerateMessageIdFrom { get; set; } = string.Empty;
    }
}
