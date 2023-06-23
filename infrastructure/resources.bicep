var productName = 'tinylnk-api'

@allowed([
  'dev'
  'tst'
  'acc'
  'prd'
])
param environmentName string

param location string
param locationAbbreviation string

var defaultResourceName = '${environmentName}-${productName}-${locationAbbreviation}'

resource cosmosDb 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' = {
  name: '${defaultResourceName}-cdb'
  location: location
  kind: 'GlobalDocumentDB'
  properties: {
    databaseAccountOfferType: 'Standard'
    locations: [
      {
        locationName: location
        failoverPriority: 0
      }

    ]
    capabilities: [
      {
        name: 'EnableServerless'
      }
    ]
  }
  resource database 'sqlDatabases' = {
    name: 'shortlinks'
    location: location
    properties: {
      resource: {
        id: 'shortlinks'
      }
    }
    resource container 'containers' = {
      name: 'operation'
      properties: {
        resource: {
          id: 'operation'
          partitionKey: {
            paths: [
              '/ownerId'
            ]
            kind: 'Hash'
          }
          indexingPolicy: {
            automatic: true
            indexingMode: 'Consistent'
            includedPaths: [
              {
                path: '/*'
              }
            ]
            excludedPaths: [
              {
                path: '/"_etag"/?'
              }
            ]
          }
        }
      }
    }
  }
  resource roledef 'sqlRoleDefinitions' existing = {
    name: '00000000-0000-0000-0000-000000000002'
  }
  resource roleassignment 'sqlRoleAssignments' = {
    name: guid('cosmosDbDataContributor')
    properties: {
      principalId: webApp.identity.principalId
      roleDefinitionId: roledef.id
      scope: cosmosDb.id
    }
  }
}

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: '${defaultResourceName}-log'
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
  }
}
resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: '${defaultResourceName}-ai'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalytics.id
  }
}
resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: '${defaultResourceName}-asp'
  location: location
  kind: 'linux'
  sku: {
    name: 'B2'
    tier: 'Basic'
  }
  properties: {
    reserved: true
  }
}
resource webApp 'Microsoft.Web/sites@2022-09-01' = {
  name: '${defaultResourceName}-app'
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  kind: 'linux'
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      appSettings: [
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: applicationInsights.properties.ConnectionString
        }
        {
          name: 'Azure__CosmosDb__Endpoint'
          value: cosmosDb.properties.documentEndpoint
        }
      ]
    }
  }
}
