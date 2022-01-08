resource "akcauth_authorization_code_client" "booliba" {
  client_id = "booliba.windows"
  client_name = "Booliba (Windows)"
  allowed_scopes = [ "openid", "profile", akcauth_api_scope.booliba_basic.name ]
  redirect_uris = ["io.identitymodel.native://callback"]
}

resource "akcauth_api_resource" "booliba" {
  name = "booliba"
  display_name = "Booliba"
  scopes = [akcauth_api_scope.booliba_basic.name]
}

resource "akcauth_api_scope" "booliba_basic" {
  name = "booliba.basic"
}