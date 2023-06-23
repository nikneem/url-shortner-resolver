param principalId string
param roleDefinitionId string
@allowed([
  'ServicePrincipal'
  'ForeignGroup'
  'Group'
  'Device'
  'User'
])
param principalType string = 'ServicePrincipal'

resource roleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(subscription().subscriptionId, principalId, roleDefinitionId)
  properties: {
    principalId: principalId
    principalType: principalType
    roleDefinitionId: roleDefinitionId
  }
}
