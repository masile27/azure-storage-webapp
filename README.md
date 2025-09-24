# Azure Storage Web App

A .NET web application that demonstrates how to use Azure Blob Storage to store files and text content. The application uses Managed Identity for secure authentication with Azure Storage.

## Features

- Upload files to Azure Blob Storage
- Save text content to blob containers
- View list of stored blobs
- Download stored files
- Secure authentication using Managed Identity

## Prerequisites

- An Azure subscription
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code

## Deploy to Azure

Click the button below to deploy this application to Azure:

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FYOUR_GITHUB_USERNAME%2Fazure-storage-webapp%2Fmain%2Finfra%2Fazuredeploy.json)

This will:
1. Create a new Azure Web App
2. Create an Azure Storage Account
3. Set up Managed Identity
4. Configure necessary permissions

## Local Development

1. Clone the repository:
```bash
git clone https://github.com/YOUR_GITHUB_USERNAME/azure-storage-webapp.git
cd azure-storage-webapp
```

2. Update the configuration in `appsettings.json` with your Azure Storage details.

3. Run the application:
```bash
dotnet run
```

## Architecture

- **Web App**: ASP.NET Core application hosted in Azure App Service
- **Storage**: Azure Blob Storage for file and text content storage
- **Authentication**: Managed Identity for secure access to Azure Storage
- **Infrastructure**: Defined using Bicep templates

## CI/CD

This repository includes GitHub Actions workflows for continuous integration and deployment. On each push to the main branch:
1. The application is built and tested
2. Infrastructure is deployed/updated using Bicep
3. The application is deployed to Azure Web App

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.