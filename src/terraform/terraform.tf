terraform {
  required_version = "~> 1.0.0"
  backend "azurerm" {}

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=2.80.0"
    }
  }
}

provider "azurerm" {
  subscription_id = local.subscription_id

  features {}
}
