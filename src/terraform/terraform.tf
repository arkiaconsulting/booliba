terraform {
  required_version = "~> 1.1.0"
  backend "azurerm" {}

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=2.80.0"
    }

    azurehelpers = {
      source = "AdamCoulterOz/azurehelpers"
    }

    akcauth = {
      version = "~>1.2"
      source  = "arkiaconsulting/akcauth"
    }
  }
}

provider "azurerm" {
  subscription_id = local.subscription_id

  features {}
}

provider "azurehelpers" {
}

provider "akcauth" {
  server_url = "https://auth.arkia.dev"
  azuread_audience = "api://arkia-identity"
}