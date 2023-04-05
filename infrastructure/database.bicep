param defaultResourceName string = resourceGroup().name
param location string = resourceGroup().location

resource sqlServerUserAssignedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: '${defaultResourceName}-uaid'
   location: location
}

resource databaseServer 'Microsoft.Sql/servers@2022-08-01-preview' = {
  name: '${defaultResourceName}-sql'
  location: location
   identity: {
     type: 'UserAssigned'
      userAssignedIdentities:{
         '${sqlServerUserAssignedIdentity.id}': {}
      }
   }
    properties:{
       administrators: {
         administratorType: 'ActiveDirectory'
          azureADOnlyAuthentication: true
           login: 
       }
    }
}
