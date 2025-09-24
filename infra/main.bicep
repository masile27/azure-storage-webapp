@description('The name of the web app that you wish to create.')
param appServiceAppName string

@description('The name of the storage account that you wish to create.')
param storageAccountName string

@description('The name of the app service plan.')
param appServicePlanName string

@description('The name of the resource group where resources should be deployed.')
param resourceGroupName string = resourceGroup().name

@description('Location for all resources.')
param location string = resourceGroup().location

@description('The SKU of App Service Plan.')
param sku string = 'B1'

// Storage Account
resource storageAccount 'Microsoft.Storage/storageAccounts@2021-09-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    minimumTlsVersion: 'TLS1_2'
    allowBlobPublicAccess: false
    supportsHttpsTrafficOnly: true
    defaultToOAuthAuthentication: true // Enable OAuth/RBAC authentication
  }
}

// App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: sku
  }
  kind: 'app'
}

// Web App
resource appServiceApp 'Microsoft.Web/sites@2022-03-01' = {
  name: appServiceAppName
  location: location
  kind: 'app'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      netFrameworkVersion: 'v8.0'
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      appSettings: [
        {
          name: 'AzureStorage__AccountEndpoint'
          value: 'https://${storageAccount.name}.blob.${environment().suffixes.storage}/'
        }
      ]
    }
  }
}

// Assign Storage Blob Data Contributor role to the Web App's managed identity
resource roleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(appServiceApp.id, storageAccount.id, 'StorageBlobDataContributor')
  scope: storageAccount
  properties: {
    principalId: appServiceApp.identity.principalId
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'ba92f5b4-2d11-453d-a403-e96b0029c9fe') // Storage Blob Data Contributor role
    principalType: 'ServicePrincipal'
  }
}

output webAppName string = appServiceApp.name
output webAppUrl string = 'https://${appServiceApp.properties.defaultHostName}'