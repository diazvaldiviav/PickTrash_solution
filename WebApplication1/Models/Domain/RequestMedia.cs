using WebApplication1.Models.Enums;


namespace WebApplication1.Models.Domain
{
    public class RequestMedia
    {
        public int Id { get; set; }

        // Relación con Request
        public int RequestId { get; set; }
        public Request Request { get; set; }

        // Información del archivo
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string FileType { get; set; }  // imagen, video, etc.
        public long FileSize { get; set; }

        // Metadata
        public DateTime UploadedAt { get; set; }
        public string UploadedBy { get; set; }
        public string Description { get; set; }

        // Tipo de evidencia
        public MediaType Type { get; set; }  // Antes/Durante/Después del servicio
    }
}
