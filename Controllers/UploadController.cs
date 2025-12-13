using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace ApiGrado.Controllers
{
    [Route("api/upload")]
    [ApiController]
    [RequestSizeLimit(50 * 1024 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = 50 * 1024 * 1024)]
    public class UploadController : ControllerBase
    {
        private readonly IAmazonS3 _s3;
        private readonly IConfiguration _config;

        public UploadController(IAmazonS3 s3, IConfiguration configuration)
        {
            _s3 = s3;
            _config = configuration;
        }

        private string SanitizeFileName(string fileName)
        {
            var name = Path.GetFileName(fileName);

            // quitar espacios
            name = Regex.Replace(name, @"\s+", "-");

            // quitar caracteres raros
            name = Regex.Replace(name, @"[^A-Za-z0-9\-\._]", "");

            return name.ToLower(); // evita errores por mayúsculas/minúsculas
        }

        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Archivo vacío");

            string bucket = _config["Backblaze:BucketName"];
            if (string.IsNullOrEmpty(bucket))
                return StatusCode(500, "Bucket no configurado");

            // 1) Normalizamos el nombre
            var originalName = SanitizeFileName(file.FileName);

            // 2) Buscar si ya existe EXACTAMENTE ese archivo
            var listRequest = new ListObjectsV2Request
            {
                BucketName = bucket,
                Prefix = "" // ⚠ NO usar originalName, rompe el filtro
            };

            var listResponse = await _s3.ListObjectsV2Async(listRequest);

            bool exists = listResponse.S3Objects?
                .Any(o => o.Key.Equals(originalName, StringComparison.OrdinalIgnoreCase)) ?? false;

            if (exists)
                return BadRequest("⚠ Ya existe un modelo con ese nombre, cámbialo antes de subir.");

            // 3) Subir archivo con el nombre original
            try
            {
                using var stream = file.OpenReadStream();

                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = stream,
                    BucketName = bucket,
                    Key = originalName,
                    ContentType = "model/gltf-binary"
                };

                var transfer = new TransferUtility(_s3);
                await transfer.UploadAsync(uploadRequest);

                string endpoint = _config["Backblaze:Endpoint"].TrimEnd('/');
                string fileUrl = $"{endpoint}/{bucket}/{originalName}";

                return Ok(new
                {
                    url = fileUrl,
                    fileName = originalName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al subir archivo: {ex.Message}");
            }
        }
    }
}
