# TiaraPro Server Setup

## Configuration Setup

### 1. Copy Configuration Templates
```bash
cp appsettings.json.example appsettings.json
cp appsettings.Development.json.example appsettings.Development.json
```

### 2. Update Configuration Values

Edit `appsettings.json` and replace the placeholder values:

#### JWT Configuration
- `Jwt:SecretKey`: Generate a secure random string (minimum 32 characters)

#### Database Connection
- `ConnectionStrings:DefaultConnection`: Your PostgreSQL connection string

#### Payment Provider (Paymob)
- `Paymob:SecretKey`: Your Paymob secret key
- `Paymob:PublicKey`: Your Paymob public key

#### Email Settings
- `EmailSettings:From`: Your sender email address
- `EmailSettings:SmtpServer`: Your SMTP server
- `EmailSettings:Username`: Your SMTP username
- `EmailSettings:Password`: Your SMTP password

#### AWS S3 Configuration
- `AWS:AccessKey`: Your AWS access key
- `AWS:SecretKey`: Your AWS secret key
- `AWS:Region`: Your AWS region
- `AWS:BucketName`: Your S3 bucket name

#### DentalMesh API
- `DentalMesh:ApiKey`: Your DentalMesh API key
- `DentalMesh:BaseUrl`: API base URL (use test URL for development)

### 3. Environment Variables (Alternative)

You can also use environment variables instead of appsettings.json:

```bash
export JWT__SECRETKEY="your-jwt-secret"
export CONNECTIONSTRINGS__DEFAULTCONNECTION="your-db-connection"
export PAYMOB__SECRETKEY="your-paymob-secret"
# ... etc
```

### 4. Security Notes

- Never commit `appsettings.json` or `appsettings.Development.json` to version control
- Use different secrets for development and production
- Rotate secrets regularly
- Consider using Azure Key Vault or AWS Secrets Manager for production 