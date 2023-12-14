# Project BION API in Development  

Starting a new project in ASP.Net Core 6 for template purposes.

Status: ![](https://geps.dev/progress/90)

### Api Endpoints

- Root: *https://localhost:7087/api*
- Register: "/authentication/register"
- Login: "/authentication/login"
- Activate Account: "/authentication/verify-account"
- Forgot Password: "/authentication/forgot-password"
- Reset Password: "/authentication/reset-password"
- Refresh Token: "/token/refresh"

### Api Email SMTP

On appsettings.json file, there are a configuration:
    
    "EmailSmtp": {
        "Host": "host-name",
        "UserName": "user-name",
        "Password": "password",
        "Port": "587",
        "Template": "default"
    },

Theme: *name of html file who lives in BionApi/Email/templates*    

### Security Key
 - Command: *setx SECRET "SeuCodeHere" /M* (Lives in PATH, 256bits)


### Important Topics

- 46-47 (foreign key)
- 48 (db context)
- 54 (Generic)
- 289 (User)
- 294-297
- 309
- 324 (binding, get more information)
- 326-329 (Options) 
- 82-85 (middleware, Review)

### SQL Server Configuraration and others

Server=localhost\SQLEXPRESS;Database=master;Trusted_Connection=True;

C:\Program Files\Microsoft SQL Server\160\Setup Bootstrap\Log\20231130_082321

C:\SQL2022\Express_ENU

C:\Program Files\Microsoft SQL Server\160\SSEI\Resources


*remove folder git command*

git rm -r --cached FolderName

git commit -m "Removed folder from repository"

git push origin master

