using EncryptionService.Models;
using EncryptionService.Services;
using Microsoft.AspNetCore.Mvc;

namespace EncryptionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EncryptionController : ControllerBase
    {
        private readonly EncryptionServiceLogic _encryptionService;

        public EncryptionController()
        {
            _encryptionService = new EncryptionServiceLogic();
        }

        [HttpPost("encrypt")]
        public IActionResult Encrypt([FromBody] TextPayload request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Text))
                {
                    return BadRequest(new ErrorResponse("Text cannot be empty"));
                }

                string ciphertext = _encryptionService.Encrypt(request.Text);
                return Ok(new CipherPayload { Ciphertext = ciphertext });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse(ex.Message));
            }
        }

        [HttpPost("decrypt")]
        public IActionResult Decrypt([FromBody] CipherPayload request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Ciphertext))
                {
                    return BadRequest(new ErrorResponse("Ciphertext cannot be empty"));
                }

                string plaintext = _encryptionService.Decrypt(request.Ciphertext);
                return Ok(new TextPayload { Text = plaintext });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse(ex.Message));
            }
        }
    }

    [ApiController]
    [Route("")]
    public class LoaderVerificationController : ControllerBase
    {
        [HttpGet("loader")]
        public IActionResult Health()
        {
            return Ok(new { status = "Healthy" });
        }
    }
}
