using QRCoder;
using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace DressCode.Models
{
    public interface IQRCodeService
    {
        string GenerateQrCodeBase64(string url);
    }
        public class QRCodeService : IQRCodeService
        {
            /// <summary>
            /// Generiše QR kod za URL i vraća ga kao Data URI sa SVG slikom.
            /// </summary>
            public string GenerateQrCodeBase64(string url)
            {
                // 1. Napravi QR kod podatke
                using var qrGenerator = new QRCodeGenerator();
                using QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);

                // 2. Generiši SVG iz QRCodeData
                var svgQr = new SvgQRCode(qrCodeData).GetGraphic(20);
                // svgQr je string koji sadrži čitav <svg>…</svg> markup

                // 3. (Opcionalno) enkodiraj u Base64 i vrati kao data:image/svg+xml;base64,…
                var svgBytes = Encoding.UTF8.GetBytes(svgQr);
                var base64 = Convert.ToBase64String(svgBytes);
                return $"data:image/svg+xml;base64,{base64}";
            }
        }
    }

