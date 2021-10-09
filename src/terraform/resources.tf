resource "azurerm_resource_group" "main" {
  name     = "booliba"
  location = "francecentral"
  tags     = local.default_tags
}
