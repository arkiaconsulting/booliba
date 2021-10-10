output "web_app_name" {
  value = azurerm_app_service.main.name
}

output "web_app_base_address" {
  value = "https://${azurerm_app_service.main.default_site_hostname}"
}
