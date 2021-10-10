resource "azurerm_user_assigned_identity" "main" {
  name                = "booliba"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  tags                = local.default_tags
}
