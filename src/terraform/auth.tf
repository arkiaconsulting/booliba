resource "akcauth_authorization_code_client" "booliba" {
  client_id = "booliba"
  client_name = "Booliba"
  allowed_scopes = [ "openid", "profile", akcauth_api_scope.booliba_basic.name ]
  redirect_uris = ["io.identitymodel.native://callback"]
}

resource "akcauth_api_scope" "booliba_basic" {
  name = "booliba.basic"
}