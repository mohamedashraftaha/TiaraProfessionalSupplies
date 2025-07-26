namespace TiaraPro.Server.Utils;

public static class  EmailContentGenerator
{
    public static string GetEmailSignature()
    {
        return $@"
            <p>Best regards,</p>
            <p><strong>Tiara AI Team</strong></p>
            <p><img src='https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/Tiara-AI-logo-original.png' alt='Tiara AI Logo' style='width: 100px; height: auto;' /></p>
        ";
    }

    public static string GenerateStatusUpdateEmail(string emailSubject, string txnGuid)
    {
        return $@"
    <html>
    <body style='font-family: Arial, sans-serif; color: #333; background-color: #ffffff; padding: 0; margin: 0;'>
        <table width='100%' cellpadding='0' cellspacing='0' border='0' style='max-width:600px; margin:auto; background-color:#f9f9f9; padding: 30px; border-radius: 12px; box-shadow: 0 4px 12px rgba(0,0,0,0.05);'>
            <tr>
                <td>
                    <h2 style='color: #FFA500;'>Your CBCT Scan is Processing</h2>
                    <p>Dear User,</p>
                    <p><strong>Transaction ID:</strong> {txnGuid}</p>
                    <p>We are currently processing your CBCT scan. You will receive an email as soon as the results are available.</p>
                    <p>Thank you for choosing <strong>Tiara AI</strong> for your scan processing needs!</p>
                    {GetEmailSignature()}
                </td>
            </tr>
        </table>
    </body>
    </html>";
    }


    public static string GenerateErrorEmail(string txnGuid)
    {
        return $@"
    <html>
    <body style='font-family: Arial, sans-serif; color: #333; background-color: #ffffff; padding: 0; margin: 0;'>
        <table width='100%' cellpadding='0' cellspacing='0' border='0' style='max-width:600px; margin:auto; background-color:#fff3f3; padding: 30px; border-radius: 12px; box-shadow: 0 4px 12px rgba(0,0,0,0.05);'>
            <tr>
                <td>
                    <h2 style='color: #D32F2F;'>Scan Processing Error</h2>
                    <p>Dear User,</p>
                    <p><strong>Transaction ID:</strong> {txnGuid}</p>
                    <p>Unfortunately, there was an issue processing your CBCT scan. Please contact our support team for assistance.</p>
                    <p>We apologize for the inconvenience and appreciate your patience.</p>
                    {GetEmailSignature()}
                </td>
            </tr>
        </table>
    </body>
    </html>";
    }


    public static string GenerateSuccessEmail(string txnGuid, string viewUrl, string shortViewUrl = "")
    {

        return $@"
    <html>
    <body style='font-family: Arial, sans-serif; color: #333; line-height: 1.6; background-color: #ffffff; padding: 0; margin: 0;'>
        <table width='100%' cellpadding='0' cellspacing='0' border='0' style='max-width:600px; margin:auto; background-color:#f9f9f9; padding: 30px; border-radius: 12px; box-shadow: 0 4px 12px rgba(0,0,0,0.05);'>
            <tr>
                <td>
                    <h2 style='color: #2E86C1;'>Your CBCT Scan is Ready</h2>
                    <p>Dear User,</p>
                    <p>We're excited to inform you that your CBCT scan has been successfully processed.</p>

                    <p><strong>Transaction ID:</strong> {txnGuid}</p>

                <p>You can access your scan results STL folder using the button below:</p>

                    <div style='margin: 20px 0;'>
                        <a href='{viewUrl}' target='_blank' style='background-color:#007BFF; color:#fff; padding:12px 24px; text-decoration:none; border-radius:6px; display:inline-block; font-size:16px;'>View STL Folder</a>
                    </div>

                    <div style='margin: 10px 0;'>
                          <p>
                            You Can also view your results via the 3D Viewer
                          </p>

                        <a href='{shortViewUrl}' target='_blank' style='background-color:#28a745; color:#fff; padding:12px 24px; text-decoration:none; border-radius:6px; display:inline-block; font-size:16px;'>Open 3D Viewer</a>
                    </div>
                    <p>Thank you for choosing <strong>Tiara AI</strong> for your scan processing needs. We hope the results are insightful and assist you effectively.</p>

                    {GetEmailSignature()}
                </td>
            </tr>
        </table>
    </body>
    </html>";
    }



}