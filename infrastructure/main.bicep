targetScope = 'subscription'

var productName = 'urlshrink-api'

@allowed([
  'dev'
  'tst'
  'acc'
  'prd'
])
param environmentName string

param location string = deployment().location
param locationAbbreviation string

var targetResourceGroupName = '${environmentName}-${productName}-${locationAbbreviation}'

resource targetResourceGroup 'Microsoft.Resources/resourceGroups@2022-09-01' = {
  name: targetResourceGroupName
  location: location
  tags: {
    runtimeEnvironment: environmentName
  }
}
