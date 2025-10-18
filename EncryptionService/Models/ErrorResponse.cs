namespace EncryptionService.Models
{
    public class ErrorResponse
    {
        public string Error { get; set; } = string.Empty;

        public ErrorResponse(string error)
        {
            Error = error;
        }
    }
}
