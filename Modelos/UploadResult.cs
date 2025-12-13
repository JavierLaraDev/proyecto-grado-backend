namespace ApiGrado.Modelos
{
    public class UploadResult
    {
        // Ruta o URL del archivo subido
        public string FilePath { get; set; }

        // Nombre original del archivo subido
        public string FileName { get; set; }

        // Tamaño del archivo subido en bytes
        public long FileSize { get; set; }

        // Tipo de archivo (por ejemplo, "image/png" o "model/gltf+json")
        public string ContentType { get; set; }
    }
}
